using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Model;
using ReportExporters.Common.Model.DataColumns;
using ReportExporters.Common.Model.TableGroups;

namespace ReportExporters.Common.Adapters
{
	public interface IReportDataAdapter
	{
		ReportColumnCollection GetColumns();
		object GetCellItemValue(ReportColumn rdColumn, int rowIndex);
		int GetRowCount();
		Size GetRowHeight();
		ReportTableGroupList GetTableGroups(ReportColumnCollection columns);
		/// <summary>
		/// Collection of Microsoft.Reporting.WinForms.ReportParameter or
		/// Microsoft.Reporting.WebForms.ReportParameter
		/// Specified type is depending by used IReportExcelExporter
		/// </summary>
		/// <returns></returns>
		IEnumerable<object> GetReportParameters();
	}
}
