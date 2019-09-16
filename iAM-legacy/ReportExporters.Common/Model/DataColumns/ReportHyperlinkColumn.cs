using System;
using System.Collections.Generic;
using System.Text;

namespace ReportExporters.Common.Model.DataColumns
{
	public class ReportHyperlinkColumn : ReportColumn
	{
		private ReportHyperlinkColumn()
		{
			
		}

		public static ReportHyperlinkColumn LinkToDataColumn(ReportDataColumn linkedReportDataColumn, Type valueType)
		{
			ReportHyperlinkColumn toRet = new ReportHyperlinkColumn();
			toRet.name = linkedReportDataColumn.Name + "Link";
			toRet.ValueType = valueType;
			
			return toRet;
		}

		public static ReportHyperlinkColumn ReplaceDataColumn(ReportDataColumn reportDataColumnToReplace)
		{
			ReportHyperlinkColumn toRet = new ReportHyperlinkColumn();
			toRet.name = reportDataColumnToReplace.Name;
			toRet.ValueType = reportDataColumnToReplace.ValueType;

			return toRet;
		}
	}
}
