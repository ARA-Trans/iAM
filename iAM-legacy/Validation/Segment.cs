using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DatabaseManager;

namespace Validation_
{
	public class Segment
	{
		bool linear;
		string facility;
		string section;
		string direction;
		double sample;
		double beginMilepost;
		double endMilepost;
		DateTime created;

		List<Property> properties;
		List<string> errors;
		//DataRow m_segmentRow;

		bool exclude;

		public override bool Equals(object obj)
		{
			bool equivalent = false;
			if( obj != null )
			{
				if( obj.GetType().Name == "Segment" )
				{
					Segment otherSeg = (Segment) obj;
					if( otherSeg.linear && linear )
					{
						equivalent = ( facility == otherSeg.facility ) && ( beginMilepost == otherSeg.beginMilepost ) && ( endMilepost == otherSeg.endMilepost ) && ( direction == otherSeg.direction );
					}
					else if( !( otherSeg.linear || linear ) ) //section
					{
						equivalent = ( facility == otherSeg.facility ) && ( section == otherSeg.section ) && ( sample == otherSeg.sample ) && ( created.ToString() == otherSeg.created.ToString() );
					}
				}
				else
				{
					equivalent = base.Equals(obj);
				}
			}
			return equivalent;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public Property this[string name]
		{
			get
			{
				return properties.Find(delegate(Property p) { return p.Label.ToUpper() == name.ToUpper(); });
			}
			set
			{
				Property toSet = properties.Find(delegate( Property p )
				{
					return p.Label.ToUpper() == name.ToUpper();
				});
				toSet = value;
			}
		}
		
		
		/// <summary>
		/// Creates a new section based segment.
		/// </summary>
		/// <param name="segmentRow">Needs to have standard linear or section column names.</param>
		public Segment(DataRow segmentRow)
		{
			exclude = false;

			properties = new List<Property>();
			errors = new List<string>();
			if (segmentRow.Table.Columns.Contains("SECTION"))
			{
				// Pull out section based data.
				foreach (DataColumn dc in segmentRow.Table.Columns)
				{
					switch (dc.ColumnName.ToUpper())
					{
						case "FACILITY":
							facility = segmentRow[dc].ToString();
							break;
						case "SECTION":
							section = segmentRow[dc].ToString();
							break;
						case "SAMPLE":
							sample = double.Parse(segmentRow[dc].ToString());
							break;
						case "DATE":
							created = DateTime.Parse(segmentRow[dc].ToString()); ;
							break;
						default:
							if (dc.DataType.Name == "String")
							{
								properties.Add(new StringProperty(dc.ColumnName, segmentRow[dc].ToString()));
							}
							else
							{
								properties.Add(new NumericProperty(dc.ColumnName, double.Parse(segmentRow[dc].ToString())));
							}
							break;
					}
				}
				linear = false;

			}
			else
			{
				foreach (DataColumn dc in segmentRow.Table.Columns)
				{
					switch (dc.ColumnName.ToUpper())
					{
						case "ROUTES":
							facility = segmentRow[dc].ToString();
							break;
						case "BEGIN_STATION":
							beginMilepost = double.Parse(segmentRow[dc].ToString());
							break;
						case "END_STATION":
							endMilepost = double.Parse(segmentRow[dc].ToString());
							break;
						case "DIRECTION":
							direction = segmentRow[dc].ToString();
							break;
						case "DATE":
							created = DateTime.Parse(segmentRow[dc].ToString());
							break;
						default:
							if (dc.DataType.Name == "String")
							{
								properties.Add(new StringProperty(dc.ColumnName, segmentRow[dc].ToString()));
							}
							else
							{
								properties.Add(new NumericProperty(dc.ColumnName, double.Parse(segmentRow[dc].ToString())));
							}
							break;
					}
				}
				linear = true;
			}
		}

		public void AddProperty( string descriptor, double val )
		{
			properties.Add( new NumericProperty( descriptor.ToUpper(), val ));
		}
		
		public void AddProperty(string descriptor, string val)
		{
			properties.Add( new StringProperty( descriptor.ToUpper(), val ));
		}

		public void AddError(string err)
		{
			errors.Add(err);
		}

		public void RemoveError(string err)
		{
			errors.Remove(err);
		}

		public bool IsLinear
		{
			get
			{
				return linear;
			}
		}

		public string Facility
		{
			get
			{
				return facility;
			}
		}

		public DateTime Date
		{
			get
			{
				return created;
			}
		}

		public double Sample
		{
			get
			{
				return sample;
			}
		}

		public string Section
		{
			get
			{
				return section;
			}
		}
		public string Direction
		{
			get
			{
				return direction;
			}
		}

		public double BMP
		{
			get
			{
				return beginMilepost;
			}
		}
		public double EMP
		{
			get
			{
				return endMilepost;
			}
		}

		public bool Exclude
		{
			get
			{
				return exclude;
			}
			set
			{
				exclude = value;
			}
		}

		public List<Property> Properties
		{
			get
			{
				return properties;
			}
		}

		public List<string> Errors
		{
			get
			{
				return errors;
			}
		}
	}
}
