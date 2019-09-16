using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Model;
using System.Windows.Forms;
using ReportExporters.Common.Model.Style;
using System.Web.UI.WebControls;

namespace ReportExporters.WinForms.Utils
{
	public class WinFormsConverter
	{
		public static Rect PaddingToRect(Padding padding)
		{
			return new Rect(
				(padding.Left > 0) ? new Unit(padding.Left, UnitType.Pixel) : new Size?(),
				(padding.Top > 0) ? new Unit(padding.Top, UnitType.Pixel) : new Size?(),
				(padding.Right > 0) ? new Unit(padding.Right, UnitType.Pixel) : new Size?(),
				(padding.Bottom > 0) ? new Unit(padding.Bottom, UnitType.Pixel) : new Size?()
			);
		}

		public static CVerticalAlign ToVerticalAlign(System.Windows.Forms.VisualStyles.VerticalAlignment winVerticalAlignment)
		{
			return new CVerticalAlign((ReportExporters.Common.Model.Style.VerticalAlign)(int)winVerticalAlignment);
		}

		public static CVerticalAlign ToVerticalAlign(System.Windows.Forms.DataGridViewContentAlignment winDGVContentAlignment)
		{
			ReportExporters.Common.Model.Style.VerticalAlign _value;

			switch (winDGVContentAlignment)
			{
				case System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft:
				case System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter:
				case System.Windows.Forms.DataGridViewContentAlignment.MiddleRight:
					{
						_value = ReportExporters.Common.Model.Style.VerticalAlign.Middle;
						break;
					}
				case System.Windows.Forms.DataGridViewContentAlignment.BottomLeft:
				case System.Windows.Forms.DataGridViewContentAlignment.BottomCenter:
				case System.Windows.Forms.DataGridViewContentAlignment.BottomRight:
					{
						_value = ReportExporters.Common.Model.Style.VerticalAlign.Bottom;
						break;
					}
				case System.Windows.Forms.DataGridViewContentAlignment.TopLeft:
				case System.Windows.Forms.DataGridViewContentAlignment.TopCenter:
				case System.Windows.Forms.DataGridViewContentAlignment.TopRight:
				default:
					{
						_value = ReportExporters.Common.Model.Style.VerticalAlign.Top;
						break;
					}
			}

			return new CVerticalAlign(_value);
		}

		public static CHorizontalAlign ToHorizontalAlign(HorizontalAlignment winHorizontalAlignment)
		{
			ReportExporters.Common.Model.Style.HorizontalAlign _value =
				(ReportExporters.Common.Model.Style.HorizontalAlign)((int)winHorizontalAlignment + 1);
			return new CHorizontalAlign(_value);
		}

		public static CHorizontalAlign ToHorizontalAlign(System.Windows.Forms.DataGridViewContentAlignment winDGVContentAlignment)
		{
			ReportExporters.Common.Model.Style.HorizontalAlign _value;

			switch (winDGVContentAlignment)
			{
				case System.Windows.Forms.DataGridViewContentAlignment.TopLeft:
				case System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft:
				case System.Windows.Forms.DataGridViewContentAlignment.BottomLeft:
					{
						_value = ReportExporters.Common.Model.Style.HorizontalAlign.Left;
						break;
					}
				case System.Windows.Forms.DataGridViewContentAlignment.TopCenter:
				case System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter:
				case System.Windows.Forms.DataGridViewContentAlignment.BottomCenter:
					{
						_value = ReportExporters.Common.Model.Style.HorizontalAlign.Center;
						break;
					}
				case System.Windows.Forms.DataGridViewContentAlignment.TopRight:
				case System.Windows.Forms.DataGridViewContentAlignment.MiddleRight:
				case System.Windows.Forms.DataGridViewContentAlignment.BottomRight:
					{
						_value = ReportExporters.Common.Model.Style.HorizontalAlign.Right;
						break;
					}
				default:
					{
						_value = ReportExporters.Common.Model.Style.HorizontalAlign.General;
						break;
					}
			}

			return new CHorizontalAlign(_value);
		}

		public static TextDecorationType ToTextDecoration(System.Drawing.Font font)
		{
			if (font == null)
				return TextDecorationType.None;
			
			if (font.Underline)
				return TextDecorationType.Underline;
			else if (font.Strikeout)
				return TextDecorationType.LineThrough;
			else
				return TextDecorationType.None;
		}
	}
}
