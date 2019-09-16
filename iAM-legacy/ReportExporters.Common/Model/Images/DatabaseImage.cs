using System;
using System.Collections.Generic;
using System.Text;

namespace ReportExporters.Common.Model.Images
{
	public class DatabaseImage : BaseImage
	{
		public string DatabaseField
		{
			get
			{
				return (string)base.Value;
			}
			set
			{
				base.Value = value;
			}
		}

		public DatabaseImage(ImageMIMEType mimeType) : base(ImageSource.Database, mimeType)
		{
		}
	}
}
