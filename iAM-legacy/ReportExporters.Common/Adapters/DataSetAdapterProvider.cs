using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ReportExporters.Common.Model.ControlItems;

namespace ReportExporters.Common.Adapters
{
	public class DataSetAdapterProvider
	{
		protected DataSet dataSet;

		public DataSetAdapterProvider(DataSet _dataSet)
		{
			dataSet = _dataSet;
		}

		/// <summary>
		/// Create IReportDataAdapter for DataTable(by default DataViewReportDataAdapter).
		/// Override to provide custom ReportDataAdapter with formatting.
		/// </summary>
		protected virtual IReportDataAdapter CreateAdapter(DataTable _dataTable)
		{
			return new DataViewReportDataAdapter(_dataTable.DefaultView);
		}

		/// <summary>
		/// Override to change ReportDataAdapter's order.
		/// Need to change order of exported Excel sheets.
		/// </summary>
		protected virtual void ReorderAdapters(Dictionary<DataTable, IReportDataAdapter> _dataTableAdapters,
			out List<IReportDataAdapter> _reorderedAdapterList)
		{
			//no reordering by default
			_reorderedAdapterList = null;
		}


		/// <summary>
		/// Retreive ReportDataAdapter's list of for dataset tables
		/// </summary>
		public List<IReportDataAdapter> GetAdapters()
		{
			Dictionary<DataTable, IReportDataAdapter> dataTableAdapters = new Dictionary<DataTable, IReportDataAdapter>();

			foreach (DataTable dataTable in dataSet.Tables)
			{
				IReportDataAdapter dataTableAdapter = CreateAdapter(dataTable);
				if (dataTableAdapter != null)
				{
					dataTableAdapters.Add(dataTable, dataTableAdapter);
				}
			}

			List<IReportDataAdapter> reorderedAdapterList;
			ReorderAdapters(dataTableAdapters, out reorderedAdapterList);

			List<IReportDataAdapter> toRet = null;

			if (reorderedAdapterList != null)
			{
				toRet = reorderedAdapterList;
			}
			else
			{
				toRet = new List<IReportDataAdapter>();
				foreach (IReportDataAdapter dataTableAdapter in dataTableAdapters.Values)
				{
					toRet.Add(dataTableAdapter);
				}
			}

			return toRet;
		}
	}
}
