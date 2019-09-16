using System;
using System.IO;
using System.Diagnostics;

namespace ReportExporters.Common
{
	public interface IReportExporter
	{
		/// <summary>
		/// Exports in Microsoft Excel format
		/// </summary>
		MemoryStream ExportToXls();

		/// <summary>
		/// Exports in PDF format
		/// </summary>
		MemoryStream ExportToPdf();

		/// <summary>
		/// Exports in Microsoft Excel format using custom device information settings
		/// </summary>
		/// <param name="deviceInfo">XML element with settings
		/// <see cref="http://msdn.microsoft.com/en-us/library/ms155069.aspx"/> 
		/// </param>
		MemoryStream ExportToXls(string deviceInfo);

		/// <summary>
		/// Exports in PDF format using custom device information settings
		/// </summary>
		/// <param name="deviceInfo">XML element with settings
		/// <see cref="http://msdn.microsoft.com/en-us/library/ms154682.aspx"/></param>
		MemoryStream ExportToPdf(string deviceInfo);

		/// <summary>
		/// Exports in image using custom device information settings
		/// </summary>
		/// <param name="deviceInfo">XML element with settings
		/// <see cref="http://msdn.microsoft.com/en-us/library/ms155373.aspx"/></param>
		MemoryStream ExportToImage(string deviceInfo);
	}
}
