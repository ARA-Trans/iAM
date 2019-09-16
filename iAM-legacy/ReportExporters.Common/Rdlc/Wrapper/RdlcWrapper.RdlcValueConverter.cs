using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ReportExporters.Common.Model.Images;
using System.Xml;

namespace ReportExporters.Common.Rdlc.Wrapper
{
	internal partial class RdlcWrapper
	{
		internal class RdlcValueConverter
		{
			static Dictionary<ImageMIMEType, string> MIMETypeValues;
			
			static RdlcValueConverter()
			{
				MIMETypeValues = new Dictionary<ImageMIMEType, string>();
				MIMETypeValues.Add(ImageMIMEType.Bmp, "image/bmp");
				MIMETypeValues.Add(ImageMIMEType.Jpeg, "image/jpeg");
				MIMETypeValues.Add(ImageMIMEType.Gif, "image/gif");
				MIMETypeValues.Add(ImageMIMEType.Png, "image/png");
				MIMETypeValues.Add(ImageMIMEType.XPng, "image/x-png");
			}
			
			internal static string GetMIMEType(ImageMIMEType mimeType)
			{
				return MIMETypeValues[mimeType];
			}
			
			internal static string GetColorName(Color _color)
			{
				string ColorName = String.Empty;

				string hex_ARGB = string.Format("{0:X8}", _color.ToArgb());
				//remove first two characters
				ColorName = "#" + hex_ARGB.Remove(0, 2);

				return ColorName;
			}
			
			internal static string GetBoolean(Boolean _boolean)
			{	
				return XmlConvert.ToString(_boolean);
			}

		}
	}
}
