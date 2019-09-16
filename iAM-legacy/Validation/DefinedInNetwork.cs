using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseManager;
using System.Data;

namespace Validation_
{
	public class DefinedInNetwork : IValidator
	{
		List<LinearFacility> linearRoads;
		List<SectionFacility> sectionRoads;

		bool correctErrors;

		public DefinedInNetwork(bool fix)
		{
			correctErrors = fix;
		}

		#region IValidator Members

		/// <summary>
		/// Checks all segments to make sure they are in the network definition.
		/// </summary>
		/// <param name="segments">List of segments to validate.</param>
		public void Validate(List<Segment> segments)
		{
			string sectionQuery = "SELECT FACILITY,SECTION FROM NETWORK_DEFINITION WHERE FACILITY IS NOT NULL ORDER BY FACILITY, SECTION";
			string linearQuery = "SELECT ROUTES, BEGIN_STATION, END_STATION, DIRECTION FROM NETWORK_DEFINITION WHERE ROUTES IS NOT NULL ORDER BY ROUTES, DIRECTION";

			DataSet linearNetworkDefinition = DBMgr.ExecuteQuery(linearQuery);
			DataSet sectionNetworkDefinition = DBMgr.ExecuteQuery(sectionQuery);

			linearRoads = GenerateLinearRoads( linearNetworkDefinition );
			sectionRoads = GenerateSectionRoads( sectionNetworkDefinition );

			foreach (Segment segmentToValidate in segments)
			{
				if (segmentToValidate.IsLinear)
				{
					ValidateLinear(segmentToValidate);
				}
				else
				{
					ValidateSection(segmentToValidate);
				}
			}	
		}

		/// <summary>
		/// Turns on or off the default error correction routines.
		/// </summary>
		public bool CorrectErrors
		{
			get
			{
				return correctErrors;
			}
			set
			{
				correctErrors = value;
			}
		}

		#endregion

		private List<SectionFacility> GenerateSectionRoads(DataSet sectionNetworkDefinition)
		{
			List<SectionFacility> sectionRoads = new List<SectionFacility>();
			SectionFacility currentFacility = null;
			foreach (DataRow sectionRow in sectionNetworkDefinition.Tables[0].Rows)
			{
				if (currentFacility != null)
				{
					if (sectionRow["FACILITY"].ToString() != currentFacility.Name)
					{
						sectionRoads.Add(currentFacility);
						currentFacility = new SectionFacility(sectionRow["FACILITY"].ToString());
					}
					currentFacility.Sections.Add(new Section(sectionRow["SECTION"].ToString()));
				}
				else
				{
					currentFacility = new SectionFacility(sectionRow["FACILITY"].ToString());
					currentFacility.Sections.Add(new Section(sectionRow["SECTION"].ToString()));
				}
			}
			if (currentFacility != null)
			{
				sectionRoads.Add(currentFacility);
			}

			return sectionRoads;
		}

		private List<LinearFacility> GenerateLinearRoads(DataSet linearNetworkDefinition)
		{
			List<LinearFacility> linearRoads = new List<LinearFacility>();
			LinearFacility currentFacility = null;
			foreach (DataRow linearRow in linearNetworkDefinition.Tables[0].Rows)
			{
				if (currentFacility != null)
				{
					if (linearRow["ROUTES"].ToString() != currentFacility.Name)
					{
						linearRoads.Add(currentFacility);
						currentFacility = new LinearFacility(linearRow["ROUTES"].ToString());
					}
					currentFacility.Directions.Add(new Direction(linearRow["DIRECTION"].ToString(), double.Parse(linearRow["BEGIN_STATION"].ToString()), double.Parse(linearRow["END_STATION"].ToString())));
				}
				else
				{
					currentFacility = new LinearFacility(linearRow["ROUTES"].ToString());
					currentFacility.Directions.Add(new Direction(linearRow["DIRECTION"].ToString(), double.Parse(linearRow["BEGIN_STATION"].ToString()), double.Parse(linearRow["END_STATION"].ToString())));
				}
			}
			if (currentFacility != null)
			{
				linearRoads.Add(currentFacility);
			}

			return linearRoads;
		}


		/// <summary>
		/// Checks to see whether or not a given segment exists in the NETWORK_DEFINITION table.
		/// </summary>
		/// <param name="segToValidate">network definition information to check against the NETWORK_DEFINITION table.</param>
		private void ValidateSection(Segment segToValidate)
		{
			SectionFacility networkFacility = sectionRoads.Find(delegate(SectionFacility f) { return f.Name == segToValidate.Facility; });
			if (networkFacility != null)
			{
				Section networkSection = networkFacility.Sections.Find(delegate(Section s) { return segToValidate.Section == s.Name; });
				if (networkSection == null)
				{
					segToValidate.AddError(segToValidate.Facility + " " + segToValidate.Section + " does not exist in the network defintion.");
					segToValidate.Exclude = true;
				}
			}
			else
			{
				segToValidate.AddError(segToValidate.Facility + " " + segToValidate.Section + " does not exist in the network defintion.");
				segToValidate.Exclude = true;
			}
		}

		/// <summary>
		/// Checks to see whether or not a given segment exists in the NETWORK_DEFINITION table.
		/// </summary>
		/// <param name="segToValidate">network definition information to check against the NETWORK_DEFINITION table.</param>
		private void ValidateLinear(Segment segToValidate)
		{
			LinearFacility networkFacility = linearRoads.Find(delegate( LinearFacility f ){ return f.Name == segToValidate.Facility; });
			if (networkFacility != null)
			{
				Direction networkDirection = networkFacility.Directions.Find( delegate( Direction d) { return segToValidate.Direction == d.Name && segToValidate.BMP >= d.BMP && segToValidate.BMP <= d.EMP && segToValidate.EMP >= d.BMP && segToValidate.EMP <= d.EMP;} );
				if (networkDirection == null)
				{
					segToValidate.AddError(segToValidate.Facility + " [" + segToValidate.Direction + "] (" + segToValidate.BMP + "-" + segToValidate.EMP + ") does not exist in the network defintion.");
					segToValidate.Exclude = true;
				}
			}
			else
			{
				segToValidate.AddError(segToValidate.Facility + " [" + segToValidate.Direction + "] (" + segToValidate.BMP + "-" + segToValidate.EMP + ") does not exist in the network defintion.");
				segToValidate.Exclude = true;
			}
		}
	}
}
