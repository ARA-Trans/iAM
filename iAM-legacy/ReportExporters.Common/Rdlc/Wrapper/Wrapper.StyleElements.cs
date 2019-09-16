using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Model.Style;
using System.Drawing;
using ReportExporters.Common.Rdlc.Enums;
using System.Xml;
using System.Web.UI.WebControls;
using ReportExporters.Common.Model;

namespace ReportExporters.Common.Rdlc.Wrapper
{
	internal partial class RdlcWrapper
	{
		/// <summary>
		/// Style information for the element
		/// </summary>
		internal class RStyle : IRdlElement
		{
			#region Properties

			private BaseStyle CStyle;

			public BaseStyle Style
			{
				get
				{
					return CStyle;
				}
			}

			#endregion

			/// <summary>
			///Line Rectangle Textbox	Image Subreport List Matrix Table Chart Body Sub-total	
			/// </summary>
			IRdlElement StyleOwnerElement;

			internal RStyle(IRdlElement styleOwnerElement, BaseStyle cStyle)
			{
				StyleOwnerElement = styleOwnerElement;
				CStyle = cStyle;
			}

			#region Applied Styles Description

			/// <summary>
			/// Line Rectangle Textbox	Image Subreport List Matrix Table Chart Body Sub-total	
			/// </summary>
			static List<Type> reportItemTypes = new List<Type>(new Type[]{
				typeof(RLine),
				typeof(RRectangle),
				typeof(RTextBox),
				typeof(RImage),
				typeof(RSubreport),
				typeof(RList),
				typeof(RMatrix),
				typeof(RTable),
				typeof(RChart),
				typeof(RSubtotal)
			});

			static Dictionary<string, bool[]> AppliedStypePropertiesForReportItem =
				new Dictionary<string, bool[]>();

			static bool[] StringToBoolArray(string sequenceYesNo)
			{
				string[] arrayYesNo = sequenceYesNo.Split(new char[] { ' ', '\t' });
				List<bool> toRet = new List<bool>();
				foreach (string valueYesNo in arrayYesNo)
				{
					toRet.Add(valueYesNo == "Y");
				}
				return toRet.ToArray();
			}

			const string PROP_BORDER = "BORDER";
			const string PROP_BACKGROUND_COLOR = "BACKGROUND_COLOR";
			const string PROP_FONT = "FONT";
			const string PROP_PADDING = "PADDING";
			const string PROP_BACKGROUND_GRADIENT = "BACKGROUND_GRADIENT";
			const string PROP_BACKGROUND_IMAGE = "BACKGROUND_IMAGE";
			const string PROP_TEXT = "TEXT";
			const string PROP_LANGUAGE = "LANGUAGE";
			const string PROP_CALENDAR = "CALENDAR";
			const string PROP_NUMERAL = "NUMERAL";
			const string PROP_WRITING_MODE = "WRITING_MODE";

			/// <summary>
			/// BORDER (BorderColor & BorderStyle & BorderWidth) 
			/// </summary>
			const string YN_BORDER = "Y Y Y Y Y Y Y Y Y Y Y";
			/// <summary>
			/// BackgroundColor
			/// </summary>
			const string YN_BACKGROUND_COLOR = "N Y Y N N Y Y Y Y Y Y";

			/// <summary>
			/// BACKGROUND_GRADIENT (BackgroundGradientType & BackgroundGradientEndColor)				
			/// </summary>
			const string YN_BACKGROUND_GRADIENT = "N N N N N N N N Y N N";

			/// <summary>
			/// BackgroundImage
			/// </summary>
			const string YN_BACKGROUND_IMAGE = "N Y Y N N Y Y Y N Y Y";
			/// <summary>
			/// FONT (FontStyle & FontFamily & FontSize & FontWeight) 
			/// </summary>
			const string YN_FONT = "N N Y N N N N N N N Y";
			/// <summary>
			/// Format, TextDecoration, TextAlign, VerticalAlign, Color, LineHeight, CanSort, 
			/// Direction, UnicodeBiDi.
			/// </summary>
			const string YN_TEXT = "N N Y N N N N N N N Y";
			/// <summary>
			/// PADDING (PaddingLeft & PaddingRight & PaddingTop & PaddingBottom)  
			/// </summary>
			const string YN_PADDING = "N N Y Y N N N N N N Y";

			/// <summary>
			/// Language
			/// </summary>
			const string YN_LANGUAGE = "N N Y N N N N N N Y N";
			/// <summary>
			/// Calendar
			/// </summary>
			const string YN_CALENDAR = "N N Y N N N N N N N N";
			/// <summary>
			/// NumeralLanguage & NumeralVariant
			/// </summary>
			const string YN_NUMERAL = "N N Y N N N N N N N N";
			/// <summary>
			/// WritingMode
			/// </summary>
			const string YN_WRITING_MODE = "N N Y N N N N N N N Y";

			#endregion

			private bool CanApplyStyle(string propertyName)
			{
				bool toRet = false;
				Type typeOfOwner = this.StyleOwnerElement.GetType();
				int typeIndex = reportItemTypes.IndexOf(typeOfOwner);

				if (AppliedStypePropertiesForReportItem.ContainsKey(propertyName))
				{
					toRet = AppliedStypePropertiesForReportItem[propertyName][typeIndex];
				}
				else
				{
					throw new ApplicationException("Invalid style property group key.");
				}

				return toRet;
			}

			static RStyle()
			{
				// BORDER				
				AppliedStypePropertiesForReportItem.Add(PROP_BORDER, StringToBoolArray(YN_BORDER));
				// BACKGROUNDCOLOR				
				AppliedStypePropertiesForReportItem.Add(PROP_BACKGROUND_COLOR, StringToBoolArray(YN_BACKGROUND_COLOR));
				// BACKGROUND_GRADIENT_TYPE				
				AppliedStypePropertiesForReportItem.Add(PROP_BACKGROUND_GRADIENT, StringToBoolArray(YN_BACKGROUND_GRADIENT));
				// BACKGROUND_IMAGE
				AppliedStypePropertiesForReportItem.Add(PROP_BACKGROUND_IMAGE, StringToBoolArray(YN_BACKGROUND_IMAGE));
				// FONT				
				AppliedStypePropertiesForReportItem.Add(PROP_FONT, StringToBoolArray(YN_FONT));
				// TEXT				
				AppliedStypePropertiesForReportItem.Add(PROP_TEXT, StringToBoolArray(YN_TEXT));
				// PADDING				
				AppliedStypePropertiesForReportItem.Add(PROP_PADDING, StringToBoolArray(YN_PADDING));
				// LANGUAGE				
				AppliedStypePropertiesForReportItem.Add(PROP_LANGUAGE, StringToBoolArray(YN_LANGUAGE));
				// CALENDAR
				AppliedStypePropertiesForReportItem.Add(PROP_CALENDAR, StringToBoolArray(YN_CALENDAR));
				// NUMERAL
				AppliedStypePropertiesForReportItem.Add(PROP_NUMERAL, StringToBoolArray(YN_NUMERAL));
				// WRITING MODE
				AppliedStypePropertiesForReportItem.Add(PROP_WRITING_MODE, StringToBoolArray(YN_WRITING_MODE));
			}

			public void WriteTo(XmlWriter xmlWriter)
			{	
				Type StyleOwnerElementType = StyleOwnerElement.GetType();

				if (CStyle == null) 
				{
					return;
				}

				xmlWriter.WriteStartElement("Style");
				{
					//Border
					if (CanApplyStyle(PROP_BORDER))
					{
						RBorder rBorder = new RBorder(CStyle.Border);
						rBorder.WriteTo(xmlWriter);
					}

					//BackgroundColor
					if ((CStyle.BackgroundColor.HasValue) && (CanApplyStyle(PROP_BACKGROUND_COLOR)))
					{
						xmlWriter.WriteElementString("BackgroundColor", RdlcValueConverter.GetColorName(CStyle.BackgroundColor.Value));
					}

					//BackgroundGradient
					if ((CStyle.BackgroundGradientType.HasValue) &&
							(CStyle.BackgroundGradientEndColor.HasValue) &&
							(CanApplyStyle(PROP_BACKGROUND_GRADIENT)))
					{
						xmlWriter.WriteElementString("BackgroundGradientType", CStyle.BackgroundGradientType.Value.ToString());
						xmlWriter.WriteElementString("BackgroundGradientEndColor", RdlcValueConverter.GetColorName(CStyle.BackgroundGradientEndColor.Value));
					}

					//BackgroundImage
					if ((CStyle.BackgroundImage != null) && (CanApplyStyle(PROP_BACKGROUND_IMAGE)))
					{
						xmlWriter.WriteStartElement("BackgroundImage");
						{
							RBaseImage rBaseImage = new RBaseImage(CStyle.BackgroundImage.Image);
							rBaseImage.WriteTo(xmlWriter);

							if (CStyle.BackgroundImage.BackgroundRepeat.HasValue)
							{
								xmlWriter.WriteElementString("BackgroundRepeat",
									CStyle.BackgroundImage.BackgroundRepeat.Value.ToString());
							}
						}
						xmlWriter.WriteEndElement();
					}

					// Font
					if (CanApplyStyle(PROP_FONT))
					{
						RFont rFont = new RFont(CStyle.Font);
						rFont.WriteTo(xmlWriter);
					}


					// Text properties
					if (CanApplyStyle(PROP_TEXT))
					{
						if (!String.IsNullOrEmpty(CStyle.Format))
							xmlWriter.WriteElementString("Format", CStyle.Format);

						if (CStyle.TextDecoration.HasValue)
							xmlWriter.WriteElementString("TextDecoration", RConverter.ToString(CStyle.TextDecoration.Value));

						if (CStyle.TextAlign.HasValue)
							xmlWriter.WriteElementString("TextAlign", RConverter.ToString(CStyle.TextAlign.Value));

						if (CStyle.VerticalAlign.HasValue)
							xmlWriter.WriteElementString("VerticalAlign", RConverter.ToString(CStyle.VerticalAlign.Value));

						if (CStyle.Color.HasValue)
							xmlWriter.WriteElementString("Color", RdlcValueConverter.GetColorName(CStyle.Color.Value));

						if (CStyle.LineHeight.HasValue)
							xmlWriter.WriteElementString("LineHeight", CStyle.LineHeight.Value.ToString());

						if (CStyle.Direction.HasValue)
							xmlWriter.WriteElementString("Direction", RConverter.ToString(CStyle.Direction.Value));

						if (CStyle.UnicodeBiDi.HasValue)
							xmlWriter.WriteElementString("UnicodeBiDi", RConverter.ToString(CStyle.UnicodeBiDi.Value));
					}

					// Padding					
					if (CanApplyStyle(PROP_PADDING))
					{
						RRect rPadding = new RRect("Padding", "", CStyle.Padding);
						rPadding.WriteTo(xmlWriter);
					}

					// NUMERAL					
					if (CanApplyStyle(PROP_NUMERAL))
					{
						if (!String.IsNullOrEmpty(CStyle.NumeralLanguage))
							xmlWriter.WriteElementString("NumeralLanguage", CStyle.NumeralLanguage);

						if (CStyle.NumeralVariant.HasValue)
							xmlWriter.WriteElementString("NumeralVariant", CStyle.NumeralVariant.Value.ToString());
					}

					// CALENDAR					
					if ((CStyle.Calendar.HasValue) && (CanApplyStyle(PROP_CALENDAR)))
					{
						xmlWriter.WriteElementString("Calendar", RConverter.ToString(CStyle.Calendar));
					}

					// LANGUAGE					
					if (!String.IsNullOrEmpty(CStyle.Language) && (CanApplyStyle(PROP_LANGUAGE)))
					{
						xmlWriter.WriteElementString("Language", RConverter.ToString(CStyle.Language));
					}

					// WRITING MODE					
					if ((CStyle.WritingMode.HasValue) && (CanApplyStyle(PROP_WRITING_MODE)))
					{
						xmlWriter.WriteElementString("WritingMode", RConverter.ToString(CStyle.WritingMode.Value));
					}
				}
				xmlWriter.WriteEndElement();
			}

			//<Style>
			//  <Color>DarkGray</Color>
			//  <BackgroundColor>LightCoral</BackgroundColor>
			//  <BorderStyle>
			//    <Default>Dashed</Default>
			//  </BorderStyle>
			//  <BorderWidth>
			//    <Default>1.7pt</Default>
			//  </BorderWidth>
			//  <FontStyle>Italic</FontStyle>
			//  <FontFamily>Arial Black</FontFamily>
			//  <FontSize>12pt</FontSize>
			//  <FontWeight>200</FontWeight>
			//  <TextDecoration>Underline</TextDecoration>
			//  <TextAlign>Right</TextAlign>
			//  <VerticalAlign>Middle</VerticalAlign>
			//  <PaddingLeft>2pt</PaddingLeft>
			//  <PaddingRight>2pt</PaddingRight>
			//  <PaddingTop>2pt</PaddingTop>
			//  <PaddingBottom>2pt</PaddingBottom>
			//  <Direction>RTL</Direction>
			//  <WritingMode>tb-rl</WritingMode>
			//</Style>
		}


		internal class RFont : IRdlElement
		{
			private BaseFont CFont;

			internal RFont(BaseFont _font)
			{
				CFont = _font;
			}

			public void WriteTo(XmlWriter xmlWriter)
			{
				if (CFont.FontStyle.HasValue)
				{
					xmlWriter.WriteElementString("FontStyle", CFont.FontStyle.Value.ToString());
				}

				if (!String.IsNullOrEmpty(CFont.FontFamily))
				{
					xmlWriter.WriteElementString("FontFamily", CFont.FontFamily);
				}

				if (CFont.FontSize.HasValue)
				{
					xmlWriter.WriteElementString("FontSize", CFont.FontSize.Value.ToString());
				}

				if (CFont.FontWeight.HasValue)
				{
					int fontWeight = (int)CFont.FontWeight.Value;
					string strFontWeight = (fontWeight > 0)
						? fontWeight.ToString()
						: RConverter.ToString(CFont.FontWeight.Value);

					xmlWriter.WriteElementString("FontWeight", strFontWeight);
				}
			}
		}

		internal class RBorder : IRdlElement
		{
			private Border CBorder;

			public RBorder(Border border)
			{
				CBorder = border;
			}

			public void WriteTo(XmlWriter xmlWriter)
			{
				int c = 0;

				//BorderColor
				if (CBorder.Color.HasValue)
				{
					xmlWriter.WriteStartElement("BorderColor");
					{
						xmlWriter.WriteElementString("Default", RdlcValueConverter.GetColorName(CBorder.Color.Value));
					}
					xmlWriter.WriteEndElement();
				}
				else c++;

				//BorderStyle
				if (CBorder.Style.HasValue)
				{
					xmlWriter.WriteStartElement("BorderStyle");
					{
						xmlWriter.WriteElementString("Default", CBorder.Style.Value.ToString());
					}
					xmlWriter.WriteEndElement();
				}
				else c++;

				//BorderWidth
				if (CBorder.Width.HasValue)
				{
					xmlWriter.WriteStartElement("BorderWidth");
					{
						xmlWriter.WriteElementString("Default", CBorder.Width.Value.ToString());
					}
					xmlWriter.WriteEndElement();
				}
				else c++;

				//if (c == 3)
				//{
				//  xmlWriter.WriteStartElement("BorderStyle");
				//  {
				//    xmlWriter.WriteElementString("Default", BorderStyle.Dotted.ToString());
				//  }
				//  xmlWriter.WriteEndElement();

				//}
			}
		}

		internal class RRect : IRdlElement
		{
			private string NameBefore;
			private string NameAfter;
			const string ElementFormat = "{0}{1}{2}";

			private Rect CRect;

			public RRect(Rect rect)
			{
				CRect = rect;
			}

			public RRect(string nameBefore, string nameAfter, Rect rect)
			{
				NameBefore = nameBefore;
				NameAfter = nameAfter;
				CRect = rect;
			}

			public void WriteTo(XmlWriter xmlWriter)
			{
				if (CRect.Bottom.HasValue)
				{
					xmlWriter.WriteElementString(
						String.Format(ElementFormat, NameBefore, "Bottom", NameAfter),
							CRect.Bottom.Value.ToString());
				}

				if (CRect.Top.HasValue)
				{
					xmlWriter.WriteElementString(
						String.Format(ElementFormat, NameBefore, "Top", NameAfter),
							CRect.Top.ToString());
				}

				if (CRect.Right.HasValue)
				{
					xmlWriter.WriteElementString(
						String.Format(ElementFormat, NameBefore, "Right", NameAfter),
							CRect.Right.ToString());
				}

				if (CRect.Left.HasValue)
				{
					xmlWriter.WriteElementString(
						String.Format(ElementFormat, NameBefore, "Left", NameAfter),
							CRect.Left.ToString());
				}
			}
		}
	}
}
