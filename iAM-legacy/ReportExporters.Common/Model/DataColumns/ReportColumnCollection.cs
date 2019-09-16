using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace ReportExporters.Common.Model.DataColumns
{

	public class ReportColumnCollection : Collection<ReportColumn>
	{
		public ReportDataColumn AddNewReportDataColumn()
		{
			return this.AddNewReportDataColumn(string.Format("Column{0}", this.Count));
		}

		public ReportDataColumn AddNewReportDataColumn(string name)
		{
			ReportDataColumn reportDataColumnToInsert = null;

			if (String.IsNullOrEmpty(name))
			{
				reportDataColumnToInsert = this.AddNewReportDataColumn();
			}
			else
			{
				reportDataColumnToInsert = new ReportDataColumn(name);
				this.Add(reportDataColumnToInsert);
			}
			
			return reportDataColumnToInsert;
		}

		public ReportColumn FindByName(string name)
		{
			foreach (ReportColumn _item in this)
			{
				if (_item.Name == name)
				{
					return _item;
				}
			}

			return null;
		}

		protected override void InsertItem(int index, ReportColumn item)
		{
			if (!this.Contains(item))
			{
				CheckReportDataColumnNameForDublicates(item, item.Name);
				base.InsertItem(index, item);
				item.ColumnIndex = index;
			}
			else
			{
			}
		}

		public bool CheckReportDataColumnNameForDublicates(ReportColumn reportDataColumn, string reportDataColumnName)
		{
			bool toRet = true;

			foreach (ReportColumn _iterator in this)
			{
				if ((!String.IsNullOrEmpty(_iterator.Name)) && (_iterator.Name == reportDataColumnName))
				{
					if (reportDataColumn != _iterator)
					{
						toRet = false;
						break;
					}
				}
			}

			return toRet;
		}

	}
}
