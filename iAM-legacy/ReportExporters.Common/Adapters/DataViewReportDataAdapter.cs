using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Model.DataColumns;
using ReportExporters.Common.Model.TableGroups;
using System.Data;
using ReportExporters.Common.Model;
using System.Web.UI.WebControls;
using ReportExporters.Common.Model.Images;
using ReportExporters.Common.Model.Style;

namespace ReportExporters.Common.Adapters
{
	/// <summary>
	/// Simple implementation IReportDataAdapter for System.Data.DataView component.
	/// No formatting available.
	/// RowHeight & ColumnWitdh have default values.
	/// </summary>
	public class DataViewReportDataAdapter : IReportDataAdapter
	{
		protected DataView dataView;
		
		private Unit DefaultColumnWidth = new Unit(1.25, UnitType.Inch);
		private Unit DefaultRowHeight = new Unit(0.25, UnitType.Inch);
		private Border DefaultBorder = new Border(System.Drawing.Color.Black, BorderStyle.Solid, new Unit(1, UnitType.Point));

		public DataViewReportDataAdapter(DataView _dataView)
		{
			dataView = _dataView;
		}

		#region IReportDataAdapter Members

		public virtual ReportColumnCollection GetColumns()
		{
			ReportColumnCollection toRet = new ReportColumnCollection();

			foreach (DataColumn dataColumn in dataView.Table.Columns)
			{
				ReportDataColumn rdColumn = toRet.AddNewReportDataColumn(dataColumn.ColumnName);

				rdColumn.HeaderText = dataColumn.Caption;
				rdColumn.ValueType = dataColumn.DataType;
				
				if (rdColumn.ValueType == typeof(System.Drawing.Image))
			  {
					rdColumn.DataCellViewType = CellViewImage.CreateDatabaseImage(ImageMIMEType.Jpeg);
			  }
			  else if (rdColumn.ValueType == typeof(System.Drawing.Bitmap))
			  {
					rdColumn.DataCellViewType = CellViewImage.CreateDatabaseImage(ImageMIMEType.Bmp);
			  }

				rdColumn.HeaderStyle.Border = 
					rdColumn.DefaultCellStyle.Border = DefaultBorder;
					
				rdColumn.DefaultCellStyle.Width = new Size(DefaultColumnWidth);
				rdColumn.DefaultCellStyle.Wrap = true;
			}

			return toRet;
		}

		public virtual object GetCellItemValue(ReportColumn rdColumn, int rowIndex)
		{
			object toRet = null;

			DataRowView dataRowView = dataView[rowIndex];
			toRet = dataRowView[rdColumn.Name];
			
			return toRet;
		}

		public virtual int GetRowCount()
		{
			return dataView.Count;
		}

		public virtual ReportTableGroupList GetTableGroups(ReportColumnCollection columns)
		{
			// No grouping by default
			return null;
		}

		public virtual Size GetRowHeight()
		{
			return DefaultRowHeight;
		}

		public virtual IEnumerable<object> GetReportParameters()
		{
			// No report parameters by default
			return null;
		}

		#endregion
	}
}
