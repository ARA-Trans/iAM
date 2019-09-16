using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Model.Images;

namespace ReportExporters.Common.Model.ControlItems
{
	public class CIImageBox : ControlItem
	{
		private BaseImage _image;
		public BaseImage Image
		{
			get
			{
				return _image;
			}
		}

		private Action _action;
		public Action Action
		{
			get
			{
				return _action;
			}
			set
			{
				_action = value;
			}
		}

		public CIImageBox(ExternalImage externalImage)
			: base()
		{
			_image = externalImage;
		}

		public CIImageBox(EmbeddedImage embeddedImage)
			: base()
		{
			_image = embeddedImage;
		}

		internal CIImageBox(DatabaseImage databaseImage)
			: base()
		{
			_image = databaseImage;
		}

		internal CIImageBox(ImageProperties imageProperties, string value)
			: base()
		{
			_image = new BaseImage(imageProperties.Source);
			_image.MIMEType = imageProperties.MIMEType;
			_image.Sizing = imageProperties.Sizing;
			_image.Value = value;
		}
	}
}
