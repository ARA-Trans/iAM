using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common;
using ReportExporters.Common.Adapters;
using Microsoft.Reporting.WinForms;
using System.IO;
using ReportExporters.Common.Exporting;

namespace ReportExporters.WinForms
{
	public class WinFormsReportExporter : IReportExporter
	{
		IReportDataAdapter reportDataAdapter;
		IList<IReportDataAdapter> reportDataAdapters;

		bool SingleMode = true;

		public WinFormsReportExporter(IReportDataAdapter _reportDataAdapter)
		{
			reportDataAdapter = _reportDataAdapter;
			SingleMode = true;
		}

		public WinFormsReportExporter(IList<IReportDataAdapter> _reportDataAdapters)
		{
			reportDataAdapters = _reportDataAdapters;
			SingleMode = false;
		}

		private ReportBase genReport;

		protected byte[] ProcessReport(LocalReport report, string reportRenderType, string deviceInfo)
		{
            Warning[] warnings;
			string[] streamids;
			string mimeType;
			string encoding;
			string extension;
			byte[] bytes;

			IReportDataSourceBuilder reportBuilder;
			if (SingleMode)
			{
				reportBuilder = new SingleReportDataSourceBuilder(reportDataAdapter);
			}
			else
			{
				reportBuilder = new MultiReportDataSourceBuilder(reportDataAdapters);
			}

			try
			{
				genReport = reportBuilder.GenerateReport();
				string GeneratedRDLC = genReport.Rdlc;
				report.LoadReportDefinition(new StringReader(GeneratedRDLC));

				if (!SingleMode)
				{
					foreach (ReportBase subreport in genReport.SubReports)
					{
						report.LoadSubreportDefinition(subreport.Name, new StringReader(subreport.Rdlc));
					}
					report.SubreportProcessing += new SubreportProcessingEventHandler(report_SubreportProcessing);
				}

				#region ReportParameters

				//List<ReportParameter> reportParameters = new List<ReportParameter>();
				//IEnumerable<object> objReportParameters = reportDataAdapter.GetReportParameters();

				//if (objReportParameters != null)
				//{
				//  IEnumerator<object> enumerator = objReportParameters.GetEnumerator();
				//  try
				//  {
				//    enumerator.Reset();
				//    while (enumerator.MoveNext())
				//    {
				//      object currentParameter = enumerator.Current;
				//      if (currentParameter is ReportParameter)
				//      {
				//        reportParameters.Add((ReportParameter)currentParameter);
				//      }
				//    }
				//  }
				//  finally
				//  {
				//    enumerator.Dispose();
				//  }
				//}

				#endregion

				report.EnableExternalImages = true;
				report.EnableHyperlinks = true;
				report.DataSources.Add(new ReportDataSource(genReport.DataSourceName, genReport.ExportedTable));
				//report.SetParameters(reportParameters);

				bytes = report.Render(reportRenderType, deviceInfo, out mimeType, out encoding,
					out extension, out streamids, out warnings);
			}
			finally
			{
				//
			}

			return bytes;
		}

		void report_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
		{
			foreach (ReportBase subReport in genReport.SubReports)
			{
				e.DataSources.Add(new ReportDataSource( subReport.DataSourceName, subReport.ExportedTable));
			}
		}

		private MemoryStream GetExportedContent(string reportRenderType, string deviceInfo)
		{
			MemoryStream toRet;

			LocalReport localReport = new LocalReport();
			try
			{
				byte[] reportData = ProcessReport(localReport, reportRenderType, deviceInfo);
				toRet = new MemoryStream(reportData);
			}
			finally
			{
				localReport.Dispose();
			}

			return toRet;
		}

		#region IReportExcelExporter Members

		public MemoryStream ExportToXls()
		{
			return this.ExportToXls(null);
		}

		public MemoryStream ExportToPdf()
		{
			return this.ExportToPdf(null);
		}

		public MemoryStream ExportToXls(string deviceInfo)
		{
			return GetExportedContent(ReportRenderType.Excel, deviceInfo);
		}

		public MemoryStream ExportToPdf(string deviceInfo)
		{
			return GetExportedContent(ReportRenderType.PDF, deviceInfo);
		}

		public MemoryStream ExportToImage(string deviceInfo)
		{
			return GetExportedContent(ReportRenderType.IMAGE, deviceInfo);
		}

		#endregion
	}
}
