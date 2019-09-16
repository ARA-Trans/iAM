using System;
using System.Collections.Generic;
using System.Text;

namespace ReportExporters.Common.Model.Images
{
	public class ExternalImage : BaseImage
	{
		public string Location
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

		public ExternalImage() : base(ImageSource.External)
		{
		}
	}
}
