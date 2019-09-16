using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Rdlc.Enums;
using ReportExporters.Common.Model.Images;

namespace ReportExporters.Common.Model
{
	public abstract class CellViewType
	{
	}

	public class CellViewText : CellViewType
	{
	}

	/// <summary>
	/// Use static methods to get new instance
	/// </summary>
	public class CellViewImage : CellViewType
	{
		private ImageProperties _imageProperties;
		public ImageProperties Properties
		{
			get { return _imageProperties; }
			set { _imageProperties = value; }
		}

		private CellViewImage(ImageSource source)
			: base()
		{
			_imageProperties = new ImageProperties(source);
		}

		private CellViewImage(ImageSource source, ImageMIMEType mimeType)
			: this(source)
		{
			_imageProperties.MIMEType = mimeType;
		}

		public static CellViewImage CreateDatabaseImage(ImageMIMEType mimeType)
		{
			return new CellViewImage(ImageSource.Database, mimeType);
		}

		public static CellViewImage CreateEmbeddedImage()
		{
			return new CellViewImage(ImageSource.Embedded);
		}

		public static CellViewImage CreateExternalImage()
		{
			return new CellViewImage(ImageSource.External);
		}
	}
}
