using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace ReportExporters.Common.Rdlc.Enums
{
	internal class MeasureTools
	{
		const double MM_IN_ONE_POINT = 0.3528;
		const double MM_IN_ONE_PICA = 4.2333;
		const double MM_IN_ONE_INCH = 25.4;
		const double MM_IN_ONE_CENTIMETER = 10;
		
		const int DPI = 96;

		public static Unit GetOnePixelSize()
		{
			return new Unit(1.0 / DPI, UnitType.Inch);
		}

		internal static double UnitToMillimeters(Unit unit)
		{
			double toRet = unit.Value;

			switch (unit.Type)
			{
				case UnitType.Cm:
					{ toRet = unit.Value * MM_IN_ONE_CENTIMETER; break; }
				case UnitType.Point:
					{ toRet = unit.Value * MM_IN_ONE_POINT; break; }
				case UnitType.Pica:
					{ toRet = unit.Value * MM_IN_ONE_PICA; break; }
				case UnitType.Inch:
					{ toRet = unit.Value * MM_IN_ONE_INCH; break; }
				case UnitType.Mm:
					{ toRet = unit.Value; break; }
				case UnitType.Pixel:
					{
						Unit OnePixelWidth = GetOnePixelSize();
						toRet = UnitToMillimeters(OnePixelWidth)* unit.Value; 
						break;
					}

				default:
					{
						throw new ApplicationException("Pixel, Percentage, Em, Ex - are not supported in ReportViewer");
					}
			}

			return toRet;
		}

		internal static double UnitToInchs(Unit unit)
		{
			double toRet = unit.Value;

			switch (unit.Type)
			{
				case UnitType.Inch:
					{
						toRet = unit.Value;
						break;
					}
				case UnitType.Mm:
					{
						toRet = unit.Value / MM_IN_ONE_INCH;
						break;
					}
				case UnitType.Cm:
					{
						toRet = unit.Value * MM_IN_ONE_CENTIMETER / MM_IN_ONE_INCH;
						break;
					}
				case UnitType.Pica:
					{
						toRet = unit.Value * MM_IN_ONE_PICA / MM_IN_ONE_INCH;
						break;
					}
				case UnitType.Point:
					{
						toRet = unit.Value * MM_IN_ONE_POINT / MM_IN_ONE_INCH;
						break;
					}
				default:
					{
						throw new ApplicationException("Pixel, Percentage, Em, Ex - are not supported in ReportViewer");
					}
			}

			return toRet;
		}
	}
}
