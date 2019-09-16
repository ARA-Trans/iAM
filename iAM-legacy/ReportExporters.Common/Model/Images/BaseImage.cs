using System;
using System.Collections.Generic;
using System.Text;

namespace ReportExporters.Common.Model.Images
{
	public class BaseImage : ImageProperties
	{
		internal object _value;
		internal object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		internal BaseImage(ImageSource source)
			: base(source)
		{
		}

		protected BaseImage(ImageSource source, ImageMIMEType? mimeType)
			: this(source)
		{
			this.MIMEType = mimeType;
		}
		
		internal virtual string GetXmlValue()
		{
			return Value.ToString();
		}
	}

	public class ImageProperties
	{
		private ImageMIMEType? _mimeType;
		/// <summary>
		/// The MIMEType for the image.
		/// </summary>
		public ImageMIMEType? MIMEType
		{
			get { return _mimeType; }
			set { _mimeType = value; }
		}

		protected ImageSource _source;
		/// <summary>
		/// Identifies the source of the image.
		/// </summary>
		public ImageSource Source
		{
			get { return _source; }
		}

		private ImageSizing? _sizing;
		/// <summary>
		/// The behavior if the image does not fit within the specified size.
		/// </summary>
		public ImageSizing? Sizing
		{
			get { return _sizing; }
			set { _sizing = value; }
		}
		
		internal ImageProperties(ImageSource source)
		{
			this._source = source;
		}
	}
}
