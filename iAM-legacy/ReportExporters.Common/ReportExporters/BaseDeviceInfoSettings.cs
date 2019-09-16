using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using ReportExporters.Common.Model;

namespace ReportExporters.Common.Exporting
{
	public class BaseDeviceInfoSettings
	{
		private string outputFormat;
		public string OutputFormat
		{
			get
			{
				return outputFormat;
			}
		}

		protected BaseDeviceInfoSettings(string _outputFormat)
		{
			this.outputFormat = _outputFormat;
		}
	}
}
