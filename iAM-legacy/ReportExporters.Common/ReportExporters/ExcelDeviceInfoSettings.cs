using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Model;

namespace ReportExporters.Common.Exporting
{
	/// <summary>
	/// Device information settings for rendering in Microsoft Excel format.
	/// <see cref="http://msdn.microsoft.com/en-us/library/ms155069.aspx"/>
	/// </summary>
	public class ExcelDeviceInfoSettings : BaseDeviceInfoSettings
	{
		public const string FORMAT_XLS = "XLS";

		public ExcelDeviceInfoSettings()
			: base(FORMAT_XLS)
		{
		}

		#region Properties

		private bool? omitDocumentMap;
		public bool? OmitDocumentMap
		{
			get { return omitDocumentMap; }
			set { omitDocumentMap = value; }
		}

		private bool? omitFormulas;
		public bool? OmitFormulas
		{
			get { return omitFormulas; }
			set { omitFormulas = value; }
		}

		private Size? removeSpace;
		public Size? RemoveSpace
		{
			get { return removeSpace; }
			set { removeSpace = value; }
		}

		private bool? simplePageHeaders;
		public bool? SimplePageHeaders
		{
			get { return simplePageHeaders; }
			set { simplePageHeaders = value; }
		}

		#endregion

		public override string ToString()
		{
			string deviceInfoXml = "<DeviceInfo>" +
				"<OutputFormat>" + OutputFormat + "</OutputFormat>" +
				((OmitDocumentMap.HasValue) ? "<OmitDocumentMap>" + OmitDocumentMap.Value.ToString() + "</OmitDocumentMap>" : String.Empty) +
				((OmitFormulas.HasValue) ? "<OmitFormulas>" + OmitFormulas.Value.ToString() + "</OmitFormulas>" : String.Empty) +
				((RemoveSpace.HasValue) ? "<RemoveSpace>" + RemoveSpace.Value.ToString() + "</RemoveSpace>" : String.Empty) +
				((SimplePageHeaders.HasValue) ? "<SimplePageHeaders>" + SimplePageHeaders.Value.ToString() + "</SimplePageHeaders>" : String.Empty) +
			 "</DeviceInfo>";

			return deviceInfoXml;
		}
	}
}
