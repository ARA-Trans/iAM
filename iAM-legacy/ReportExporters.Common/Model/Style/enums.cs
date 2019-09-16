using System;
using System.Collections.Generic;
using System.Text;

namespace ReportExporters.Common.Model.Style
{
	public enum TextDecorationType
	{
		/// <summary>
		/// Default
		/// </summary>
		None,
		Underline,
		Overline,
		LineThrough
	}

	/// <summary>
	/// The type of background gradient
	/// </summary>
	public enum BackgroundGradient
	{
		/// <summary>
		/// Default
		/// </summary>
		None,
		LeftRight,
		TopBottom,
		Center,
		DiagonalLeft,
		DiagonalRight,
		HorizontalCenter,
		VerticalCenter
	}

	public enum HorizontalAlign
	{
		/// <summary>
		/// Default value
		/// </summary>
		General = 0,
		Left = 1,
		Center = 2,
		Right = 3
	}

	public partial struct CHorizontalAlign
	{
		HorizontalAlign _value;

		public HorizontalAlign Value
		{
			get
			{
				return _value;
			}
		}

		public CHorizontalAlign(HorizontalAlign _horizontalAlignType)
		{
			_value = _horizontalAlignType;
		}

		public override string ToString()
		{
			return _value.ToString();
		}

		public static implicit operator CHorizontalAlign(HorizontalAlign _horizontalAlignType)
		{
			return new CHorizontalAlign(_horizontalAlignType);
		}

		#region WebForms

		public static implicit operator CHorizontalAlign(System.Web.UI.WebControls.HorizontalAlign webHorizontalAlign)
		{
			return new CHorizontalAlign(webHorizontalAlign);
		}

		public CHorizontalAlign(System.Web.UI.WebControls.HorizontalAlign webHorizontalAlign)
		{
			switch (webHorizontalAlign)
			{
				case System.Web.UI.WebControls.HorizontalAlign.NotSet:
				case System.Web.UI.WebControls.HorizontalAlign.Justify:
					_value = HorizontalAlign.General;
					break;
				default:
					_value = (HorizontalAlign)((int)webHorizontalAlign);
					break;
			}
		}

		#endregion

	}

	public enum VerticalAlign
	{
		/// <summary>
		/// Default value
		/// </summary>
		Top = 0,
		Middle = 1,
		Bottom = 2
	}

	public partial struct CVerticalAlign
	{
		VerticalAlign _value;

		public VerticalAlign Value
		{
			get
			{
				return _value;
			}
		}

		public CVerticalAlign(VerticalAlign _verticalAlignType)
		{
			_value = _verticalAlignType;
		}

		public override string ToString()
		{
			return _value.ToString();
		}

		public static implicit operator CVerticalAlign(VerticalAlign _verticalAlignType)
		{
			return new CVerticalAlign(_verticalAlignType);
		}

		#region WebForms

		public static implicit operator CVerticalAlign(System.Web.UI.WebControls.VerticalAlign webVerticalAlign)
		{
			return new CVerticalAlign(webVerticalAlign);
		}

		public CVerticalAlign(System.Web.UI.WebControls.VerticalAlign webVerticalAlign)
		{
			switch (webVerticalAlign)
			{
				case System.Web.UI.WebControls.VerticalAlign.NotSet:
					_value = VerticalAlign.Top;
					break;
				default:
					_value = (VerticalAlign)((int)webVerticalAlign - 1);
					break;
			}
		}

		#endregion



	}

	public enum LayoutDirection
	{
		/// <summary>
		/// left-to-right (Default)
		/// </summary>
		LTR,
		/// <summary>
		/// right-to-left
		/// </summary>
		RTL
	}

	public enum WritingModeType
	{
		/// <summary>
		/// Default
		/// </summary>
		lr_tb,
		tb_rl
	}

	public enum UnicodeBiDiType
	{
		/// <summary>
		/// Default
		/// </summary>
		Normal,
		Embed,
		BiDi_Override
	}

	public enum CalendarType
	{
		Gregorian,
		Gregorian__Arabic,
		Gregorian__Middle_East__French,
		Gregorian__Transliterated__English,
		Gregorian__Transliterated__French,
		Gregorian__US__English,
		Hebrew,
		Hijri,
		Japanese,
		Korea,
		Taiwan,
		Thai__Buddhist
	}

	public enum FontStyleType
	{
		/// <summary>
		/// Default
		/// </summary>
		Normal,
		Italic
	}

	/// <summary>
	/// Font Weight
	/// Lighter & Bolder should be writed as string, other as integer.
	/// </summary>
	public enum FontWeightType
	{
		Lighter = -2,
		Thin = 100,
		Extra__Light = 200,
		Light = 300,
		/// <summary>
		/// Default
		/// </summary>
		Normal = 400,
		Medium = 500,
		Semi_Bold = 600,
		Bold = 700,
		Extra__Bold = 800,
		Heavy = 900,
		Bolder = -1,
	}

}