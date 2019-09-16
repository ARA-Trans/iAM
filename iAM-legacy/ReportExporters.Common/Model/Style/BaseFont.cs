using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace ReportExporters.Common.Model.Style
{
	public class BaseFont
	{
		#region Properties

		private FontStyleType? _fontStyle;
		/// <summary>
		/// Font style
		/// </summary>
		public FontStyleType? FontStyle
		{
			get { return _fontStyle; }
			set { _fontStyle = value; }
		}

		private string _fontFamily;
		/// <summary>
		/// Name of the font family. Default is Arial
		/// </summary>
		public string FontFamily
		{
			get { return _fontFamily; }
			set { _fontFamily = value; }
		}

		private Size? _fontSize;
		/// <summary>
		/// Point size of the font.
		/// Default: 10 pt. Min: 1 pt. Max: 200 pt.
		/// </summary>
		public Size? FontSize
		{
			get { return _fontSize; }
			set { _fontSize = value; }
		}

		private FontWeightType? _fontWeight;
		/// <summary>
		/// Thickness of the font
		/// </summary>
		public FontWeightType? FontWeight
		{
			get { return _fontWeight; }
			set { _fontWeight = value; }
		}

		#endregion

		public static implicit operator BaseFont(System.Drawing.Font drawingFont)
		{
			return new BaseFont(drawingFont);
		}

		public static implicit operator BaseFont(FontInfo webcontrolsFontInfo)
		{
			return new BaseFont(webcontrolsFontInfo);
		}

		internal BaseFont()
		{
		}

		public BaseFont(System.Drawing.Font drawingFont)
		{
			if (drawingFont != null)
			{
				this.FontFamily = drawingFont.FontFamily.Name;
				this.FontStyle = drawingFont.Italic ? FontStyleType.Italic : new FontStyleType?();
				this.FontSize = new Unit(drawingFont.SizeInPoints, UnitType.Point);
				this.FontWeight = drawingFont.Bold ? FontWeightType.Bold : new FontWeightType?();
			}
			else
			{
			}
		}

		public BaseFont(FontInfo webcontrolsFontInfo)
		{
			this.FontFamily = webcontrolsFontInfo.Name;

			if (!webcontrolsFontInfo.Size.IsEmpty)
			{
				this.FontSize = webcontrolsFontInfo.Size.Unit;
			}

			this.FontStyle = webcontrolsFontInfo.Italic ? FontStyleType.Italic : new FontStyleType?();
			this.FontWeight = webcontrolsFontInfo.Bold ? FontWeightType.Bold : new FontWeightType?();
		}
	}
}
