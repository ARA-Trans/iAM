using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation
{
	public class TreatmentCopy
	{
		string _treatmentName;
		int _treatmentYear;

		LocationCopy _originalLocation;

		#region Accessors
	
		public LocationCopy OriginalLocation
		{
			get
			{
				return _originalLocation;
			}
		}

		public string TreatmentName
		{
			get
			{
				return _treatmentName;
			}
		}

		public int TreatmentYear
		{
			get
			{
				return _treatmentYear;
			}
		}

		#endregion

		public TreatmentCopy( string treatmentName, int treatmentYear, LocationCopy originalLocation )
		{
			_treatmentName = treatmentName;
			_treatmentYear = treatmentYear;
			_originalLocation = originalLocation;
		}

		
	}
}
