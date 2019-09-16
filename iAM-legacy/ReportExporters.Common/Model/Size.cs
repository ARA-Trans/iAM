using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using ReportExporters.Common.Rdlc.Enums;
using System.Globalization;

namespace ReportExporters.Common.Model
{
	/// <summary>
	/// Floating-point number (in cm, mm, in, pt, pc) used in RDL
	/// </summary>
	public struct Size
	{
		private Unit Value;

		public Size(Unit _value)
		{
			Unit reportUnit;
			
			if (_value.Value < 0)
			{
				throw new ApplicationException("Value of ReportExporters.Common.Model.Size cannot be negative.");
			}

			switch (_value.Type)
			{
				case UnitType.Mm:
				case UnitType.Inch:
				case UnitType.Cm:
				case UnitType.Point:
				case UnitType.Pica:
					reportUnit = _value;
					break;
				default:
					reportUnit = new Unit(MeasureTools.UnitToMillimeters(_value), UnitType.Mm);
					break;
			}

			this.Value = reportUnit;
		}

		public static implicit operator Unit(Size d)
		{
			return d.Value;
		}

		public static implicit operator Size(Unit d)
		{
			return new Size(d);
		}

		public override string ToString()
		{
			return this.Value.ToString(CultureInfo.InvariantCulture);
		}

		public override bool Equals(object obj)
		{
			if (obj is Size)
			{
				return this.Value.Equals(((Size)obj).Value);
			}
			else
			{
				return base.Equals(obj);
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

	}
}
