using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ReportExporters.Common.Model.Images;

namespace ReportExporters.Common.Model.Style
{
	public class BaseStyle
	{
		public BaseStyle()
		{
			_padding = new Rect();
			_font = new BaseFont();
			_border = new Border();
		}

		#region Properties

		private Border _border;
		public Border Border
		{
			get { return _border; }
			set { _border = value; }
		}

		private Color? _backgroundColor;
		/// <summary>
		/// Color of the background. If omitted, the background is transparent
		/// </summary>
		public Color? BackgroundColor
		{
			get { return _backgroundColor; }
			set { _backgroundColor = value; }
		}

		private BackgroundGradient? _backgroundGradientType;
		/// <summary>
		/// The type of background gradient
		/// </summary>
		public BackgroundGradient? BackgroundGradientType
		{
			get { return _backgroundGradientType; }
			set { _backgroundGradientType = value; }
		}

		private Color? _backgroundGradientEndColor;
		/// <summary>
		/// End color for the background gradient. If omitted, there is no gradient.
		/// </summary>
		public Color? BackgroundGradientEndColor
		{
			get { return _backgroundGradientEndColor; }
			set { _backgroundGradientEndColor = value; }
		}

		private BackgroundImage _backgroundImage;
		/// <summary>
		///  A background image for the report item.
		/// </summary>
		public BackgroundImage BackgroundImage
		{
			get { return _backgroundImage; }
			set { _backgroundImage = value; }
		}

		private BaseFont _font;
		/// <summary>
		/// Style Font
		/// </summary>
		public BaseFont Font
		{
			get { return _font; }
			set { _font = value; }
		}

		private string _format;
		/// <summary>
		/// .NET Framework formatting string
		/// </summary>
		public string Format
		{
			get { return _format; }
			set { _format = value; }
		}

		private TextDecorationType? _textDecoration;
		/// <summary>
		/// Special text formatting
		/// </summary>
		public TextDecorationType? TextDecoration
		{
			get { return _textDecoration; }
			set { _textDecoration = value; }
		}

		private CHorizontalAlign? _textAlign;
		/// <summary>
		/// Horizontal alignment of the text
		/// </summary>
		public CHorizontalAlign? TextAlign
		{
			get { return _textAlign; }
			set { _textAlign = value; }
		}

		private CVerticalAlign? _verticalAlign;
		/// <summary>
		/// Vertical alignment of the text
		/// </summary>
		public CVerticalAlign? VerticalAlign
		{
			get { return _verticalAlign; }
			set { _verticalAlign = value; }
		}

		private Color? _foregroundColor;
		/// <summary>
		/// The foreground color. Default is Black
		/// </summary>
		public Color? Color
		{
			get { return _foregroundColor; }
			set { _foregroundColor = value; }
		}

		private Rect _padding;
		/// <summary>
		/// Padding between the side  edge of the report item and its contents.
		///	Default: 0 pt. Max: 1000 pt.
		/// </summary>
		public Rect Padding
		{
			get { return _padding; }
			set { _padding = value; }
		}

		private Size? _lineHeight;
		/// <summary>
		/// Height of a line of text. Based on font size.
		/// Min: 1 pt. Max: 1000 pt.
		/// </summary>
		public Size? LineHeight
		{
			get { return _lineHeight; }
			set { _lineHeight = value; }
		}

		private LayoutDirection? _direction;
		/// <summary>
		/// Indicates whether text is written left-to-right or right-to-left.
		/// Does not impact the alignment of text unless using General alignment.
		/// </summary>
		public LayoutDirection? Direction
		{
			get { return _direction; }
			set { _direction = value; }
		}

		private WritingModeType? _writingMode;
		/// <summary>
		/// Indicates whether text is written horizontally or vertically.
		/// </summary>
		public WritingModeType? WritingMode
		{
			get { return _writingMode; }
			set { _writingMode = value; }
		}

		private string _language;
		/// <summary>
		/// The primary language of the text. Default is Report.Language.
		/// </summary>
		public string Language
		{
			get { return _language; }
			set { _language = value; }
		}

		private UnicodeBiDiType? _unicodeBiDi;
		/// <summary>
		/// Indicates the level of embedding with respect to the Bi-directional algorithm.
		/// </summary>
		public UnicodeBiDiType? UnicodeBiDi
		{
			get { return _unicodeBiDi; }
			set { _unicodeBiDi = value; }
		}

		private CalendarType? _calendar;
		/// <summary>
		/// Indicates the calendar to use for formatting dates. 
		/// Must be compatible in the .NET framework with the Language setting. 
		/// Default is the default calendar for the Language of the report item.
		/// </summary>
		public CalendarType? Calendar
		{
			get { return _calendar; }
			set { _calendar = value; }
		}

		private string _numeralLanguage;
		/// <summary>
		/// The digit format to use as described by its primary language.
		/// Any language is legal.
		/// Default is the Language property.
		/// </summary>
		public string NumeralLanguage
		{
			get { return _numeralLanguage; }
			set { _numeralLanguage = value; }
		}

		private int? _numeralVariant;
		/// <summary>
		/// The variant of the digit format to use.
		/// Currently defined values are:
		/// 1: default, follow Unicode context rules
		/// 2: 0123456789
		/// 3: traditional digits for the script as defined in GDI+. Currently supported for: ar | bn | bo | fa | gu | hi | kn | kok | lo | mr | ms | or | pa | sa | ta | te | th | ur and variants.
		/// 4: ko, ja, zh-CHS, zh-CHT only
		/// 5: ko, ja, zh-CHS, zh-CHT only
		/// 6: ko, ja, zh-CHS, zh-CHT only [Wide versions of regular digits]
		/// 7: ko only
		/// </summary>
		public int? NumeralVariant
		{
			get { return _numeralVariant; }
			set { _numeralVariant = value; }
		}

		#endregion

	}
}
