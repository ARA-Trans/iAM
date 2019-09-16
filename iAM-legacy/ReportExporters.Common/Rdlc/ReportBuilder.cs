using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Data;
using ReportExporters.Common.Rdlc;
using System.Web.UI.WebControls;
using ReportExporters.Common.Model.DataColumns;
using ReportExporters.Common.Rdlc.Wrapper;
using ReportExporters.Common.Model.TableGroups;
using ReportExporters.Common.Model;
using ReportExporters.Common.Model.Images;

namespace ReportExporters.Common.Rdlc
{
	/// <summary>
	/// Contain custom methods for creating reports
	/// </summary>
	public class ReportBuilder
	{
		private RdlcWrapper rdlcWrapper;
		private XmlWriter xmlWriter;
		private StringBuilder sb;
		private TextWriter tw;

		public ReportBuilder()
		{
			rdlcWrapper = new RdlcWrapper();
			sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
			tw = new StringWriter(sb);
			xmlWriter = new XmlTextWriter(tw);
		}

		/// <summary>
		/// generate xml report using added tables and datasources, embedded images
		/// </summary>
		/// <returns></returns>
		public string BuildRDLC()
		{
			rdlcWrapper.Report.WriteTo(xmlWriter);
			xmlWriter.Flush();
			tw.Flush();
			return sb.ToString();
		}

		public void AddDataSource(DataTable dataTable)
		{
			rdlcWrapper.Report.DataSets.Add(new RDataSet(dataTable));
		}

		public void AddReportTable(DataTable dataTable, IList<ReportColumn> reportDataColumns,
			ReportTableGroupList _reportTableGroups, Size _rowHeight)
		{
			RdlcWrapper.ReportControlItem viewItem = null;

			if ((_reportTableGroups != null) && (_reportTableGroups.IsMatrix))
			{
				RdlcWrapper.RMatrix matrix = new RdlcWrapper.RMatrix(
					dataTable, reportDataColumns, _reportTableGroups);

				viewItem = matrix;
			}
			else
			{
				RdlcWrapper.RTable table = new RdlcWrapper.RTable(
					dataTable, reportDataColumns, _reportTableGroups, _rowHeight);

				viewItem = table;
			}

			rdlcWrapper.Report.Body.ReportItems.Add(viewItem);
		}

		public void AddEmbeddedImages(Dictionary<string, System.Drawing.Image> _embeddedImages)
		{
			foreach (string imageKey in _embeddedImages.Keys)
			{
				rdlcWrapper.Report.EmbeddedImages.Add(
					new RdlcWrapper.REmbeddedImage(
						new EmbeddedImage(imageKey, _embeddedImages[imageKey])
					)
				);
			}
		}

		List<string> SubReportNames = new List<string>();

		public void AddSubReport(string _subReportName)
		{
			Unit height = new Unit(4, UnitType.Mm);
				
			//RDLC Specification:
			//The value of the Top property for the rectangle must be between 0mm and 4064mm.
			//Maximum numbers of subreport rectangles is 4064mm/4mm = 1016

			RdlcWrapper.RRectangle rectSubReport = new RdlcWrapper.RRectangle();

			rectSubReport.PageBreakAtStart = true;
			//rectSubReport.PageBreakAtEnd = true;
			
			rectSubReport.Rectangle.Height = new Size(height);
			rectSubReport.Rectangle.Top = new Size(new Unit(height.Value * SubReportNames.Count, height.Type));

			RdlcWrapper.RSubreport subreport = new RdlcWrapper.RSubreport(_subReportName);
			rectSubReport.ReportItems.Add(subreport);
			SubReportNames.Add(_subReportName);

			rdlcWrapper.Report.Body.ReportItems.Add(rectSubReport);
		}
	}
}
