using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace ReportExporters.Common.Model.Images
{
	public class EmbeddedImage : BaseImage
	{
		public byte[] ImageData
		{
			get
			{
				return base.Value as byte[];
			}
			set
			{
				base.Value = value;
			}
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private EmbeddedImage(ImageMIMEType? mimeType) : base(ImageSource.Embedded, mimeType)
		{
		}

		public EmbeddedImage(string name, Image imageData)
			: this(GetMIMEType(imageData.RawFormat))
		{
			this.Name = name;

			if (imageData != null)
			{
				MemoryStream memStream = new MemoryStream();
				imageData.Save(memStream, imageData.RawFormat);
				this.ImageData = memStream.ToArray();
			}
		}

		public EmbeddedImage(string name, byte[] imageData, System.Drawing.Imaging.ImageFormat imageFormat)
			: this(GetMIMEType(imageFormat))
		{
			this.Name = name;
			this.ImageData = imageData;
		}

		internal override string GetXmlValue()
		{
			string toRet;
			if (ImageData != null)
			{
				toRet = Convert.ToBase64String(ImageData);
			}
			else
			{
				toRet = base.GetXmlValue();
			}
			return toRet;
		}

		private static ImageMIMEType? GetMIMEType(System.Drawing.Imaging.ImageFormat imageFormat)
		{
			ImageMIMEType? toRet;
			Guid formatGuid = imageFormat.Guid;

			if (formatGuid == ImageFormat.Bmp.Guid)
			{
				toRet = ImageMIMEType.Bmp;
			}
			else if (formatGuid == ImageFormat.Gif.Guid)
			{
				toRet = ImageMIMEType.Gif;
			}
			else if (formatGuid == ImageFormat.Jpeg.Guid)
			{
				toRet = ImageMIMEType.Jpeg;
			}
			else if (formatGuid == ImageFormat.Png.Guid)
			{
				toRet = ImageMIMEType.Png;
			}
			else
			{
				throw new ApplicationException(
					string.Format("Not supported ImageFormat {0}", imageFormat.ToString())
				);
			}

			return toRet;
		}
	}
}
