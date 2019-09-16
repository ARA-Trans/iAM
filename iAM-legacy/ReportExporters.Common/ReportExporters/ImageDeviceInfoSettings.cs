using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Model;
using System.Web.UI.WebControls;

namespace ReportExporters.Common.Exporting
{
	public class ImageDeviceInfoSettings : BaseDeviceInfoSettings
	{
		public const string FORMAT_BMP = "BMP";
		public const string FORMAT_EMF = "EMF";
		public const string FORMAT_GIF = "GIF";
		public const string FORMAT_JPEG = "JPEG";
		public const string FORMAT_PNG = "PNG";
		public const string FORMAT_TIFF = "TIFF";

		public ImageDeviceInfoSettings(string _outputFormat)
			: base(_outputFormat)
		{
			Margin = new Rect(
				new Unit(1, UnitType.Inch),
				new Unit(0.5, UnitType.Inch),
				new Unit(1, UnitType.Inch),
				new Unit(0.5, UnitType.Inch)
			);
		}

		#region Properties

		private int? columns;
		/// <summary>
		/// The number of columns to set for the report. This value overrides the report's original settings.
		/// </summary>
		public int? Columns
		{
			get { return columns; }
			set { columns = value; }
		}

		private Size? columnSpacing;
		/// <summary>
		/// The column spacing to set for the report. This value overrides the report's original settings.
		/// </summary>
		public Size? ColumnSpacing
		{
			get { return columnSpacing; }
			set { columnSpacing = value; }
		}

		private Size? pageWidth;
		/// <summary>
		/// The page width, in inches, to set for the report.
		/// </summary>
		public Size? PageWidth
		{
			get { return pageWidth; }
			set { pageWidth = value; }
		}

		private Size? pageHeight;
		/// <summary>
		/// The page height, in inches, to set for the report.
		/// </summary>
		public Size? PageHeight
		{
			get { return pageHeight; }
			set { pageHeight = value; }
		}

		private Rect margin;
		/// <summary>
		/// The margin value, in inches, to set for the report.
		/// </summary>
		public Rect Margin
		{
			get
			{
				return margin;
			}
			set
			{
				margin = value;
			}
		}

		private int? startPage;
		/// <summary>
		/// The first page of the report to render. 
		/// A value of 0 indicates that all pages are rendered. 
		/// The default value is 1.
		/// </summary>
		public int? StartPage
		{
			get { return startPage; }
			set { startPage = value; }
		}

		private int? endPage;
		/// <summary>
		/// The last page of the report to render. 
		/// The default value is the value for StartPage.
		/// </summary>
		public int? EndPage
		{
			get { return endPage; }
			set { endPage = value; }
		}

		private int? colorDepth;
		/// <summary>
		/// The pixel depth of the color range supported by the image output. 
		/// Valid values are 1, 4, 8, 24, and 32. 
		/// The default value is 24.
		/// ColorDepth is only supported for TIFF rendering 
		/// and is otherwise ignored by the report server for other image output formats. 
		/// </summary>
		public int? ColorDepth
		{
			get { return colorDepth; }
			set { colorDepth = value; }
		}

		private int? dpiX;
		/// <summary>
		/// The resolution of the output device in x-direction. The default value is 96.
		/// </summary>
		public int? DpiX
		{
			get { return dpiX; }
			set { dpiX = value; }
		}

		private int? dpiY;
		/// <summary>
		/// The resolution of the output device in y-direction. The default value is 96.
		/// </summary>
		public int? DpiY
		{
			get { return dpiY; }
			set { dpiY = value; }
		}

		#endregion

		public override string ToString()
		{
			string deviceInfoXml = "<DeviceInfo>" +
				"<OutputFormat>" + OutputFormat + "</OutputFormat>" +
				((Columns.HasValue) ? "<Columns>" + Columns.Value.ToString() + "</Columns>" : String.Empty) +
				((ColumnSpacing.HasValue) ? "<ColumnSpacing>" + ColumnSpacing.Value.ToString() + "</ColumnSpacing>" : String.Empty) +
				((PageWidth.HasValue) ? "<PageWidth>" + PageWidth.Value.ToString() + "</PageWidth>" : String.Empty) +
				((PageHeight.HasValue) ? "<PageHeight>" + PageHeight.Value.ToString() + "</PageHeight>" : String.Empty) +
				"<MarginTop>" + Margin.Top.ToString() + "</MarginTop>" +
				"<MarginLeft>" + Margin.Left.ToString() + "</MarginLeft>" +
				"<MarginRight>" + Margin.Right.ToString() + "</MarginRight>" +
				"<MarginBottom>" + Margin.Bottom.ToString() + "</MarginBottom>" +
				((StartPage.HasValue) ? "<StartPage>" + StartPage + "</StartPage>" : String.Empty) +
				((EndPage.HasValue) ? "<EndPage>" + EndPage + "</EndPage>" : String.Empty) +
				((ColorDepth.HasValue) ? "<ColorDepth>" + ColorDepth.Value.ToString() + "</ColorDepth>" : String.Empty) +
				((DpiX.HasValue) ? "<DpiX>" + DpiX.Value.ToString() + "</DpiX>" : String.Empty) +
				((DpiY.HasValue) ? "<DpiY>" + DpiY.Value.ToString() + "</DpiY>" : String.Empty) + 
			 "</DeviceInfo>";

			return deviceInfoXml;
		} 

	}
}
