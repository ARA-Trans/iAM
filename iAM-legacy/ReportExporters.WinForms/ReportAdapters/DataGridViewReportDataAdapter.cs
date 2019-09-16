using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Adapters;
using System.Windows.Forms;
using ReportExporters.Common.Model;
using ReportExporters.Common.Model.Style;
using ReportExporters.Common.Model.DataColumns;
using ReportExporters.Common.Model.TableGroups;
using ReportExporters.WinForms.Utils;
using ReportExporters.Common.Model.Images;

namespace ReportExporters.WinForms.Adapters
{
	public class DataGridViewReportDataAdapter : IReportDataAdapter
	{
		protected DataGridView dataGridView;

		public DataGridViewReportDataAdapter(DataGridView _dataGridView)
		{
			this.dataGridView = _dataGridView;
		}

		#region IReportDataAdapter Members

		public virtual ReportColumnCollection GetColumns()
		{
			ReportColumnCollection toRet = new ReportColumnCollection();

			foreach (DataGridViewColumn dgvColumn in dataGridView.Columns)
			{
				ReportDataColumn rdColumn = toRet.AddNewReportDataColumn(dgvColumn.Name);

				rdColumn.HeaderText = dgvColumn.HeaderText;
				rdColumn.ValueType = dgvColumn.ValueType;
				
				if (dgvColumn is DataGridViewImageColumn)
				{
					CellViewImage databaseCellViewImage = CellViewImage.CreateDatabaseImage(ImageMIMEType.Jpeg);
					//databaseCellViewImage.Sizing = ImageSizing.FitProportional;
					rdColumn.DataCellViewType = databaseCellViewImage;
				}

				{ //initialize style for column header cell
					InitReportStyleFrom(
						rdColumn.HeaderStyle,
						dgvColumn.HeaderCell.HasStyle ? dgvColumn.HeaderCell.Style : dgvColumn.HeaderCell.InheritedStyle);
				}

				{ //initialize style for column content cell
					InitReportStyleFrom(rdColumn.DefaultCellStyle,
						dgvColumn.HasDefaultCellStyle ? dgvColumn.DefaultCellStyle : dgvColumn.InheritedStyle);
				}

				//set column width
				rdColumn.HeaderStyle.Width =
					rdColumn.DefaultCellStyle.Width = 
						new System.Web.UI.WebControls.Unit(dgvColumn.Width, System.Web.UI.WebControls.UnitType.Pixel);
			}

			return toRet;
		}

		public virtual object GetCellItemValue(ReportColumn rdColumn, int rowIndex)
		{
			object toRet = null;

			DataGridViewRow dgvRow = dataGridView.Rows[rowIndex];
			DataGridViewCell dgvCell = dgvRow.Cells[rdColumn.Name];
			toRet = dgvCell.Value;

			return toRet;
		}

		public virtual int GetRowCount()
		{
			return dataGridView.RowCount;
		}

		public virtual ReportTableGroupList GetTableGroups(ReportColumnCollection columns)
		{
			// No grouping by default
			return null;
		}

		public virtual Size GetRowHeight()
		{
			return new System.Web.UI.WebControls.Unit(dataGridView.RowTemplate.Height,
				System.Web.UI.WebControls.UnitType.Pixel);
		}

		public virtual IEnumerable<object> GetReportParameters()
		{
			// No report parameters by default
			return null;
		}

		#endregion

		private void InitReportStyleFrom(ReportStyle reportStyle, DataGridViewCellStyle dgvCellStyle)
		{
			reportStyle.Font = dgvCellStyle.Font;
			reportStyle.Color = dgvCellStyle.ForeColor;
			reportStyle.BackgroundColor = dgvCellStyle.BackColor;

			//couldn't get Border information from DataGrivView
			//reportStyle.Border
			
			reportStyle.Format = dgvCellStyle.Format;

			reportStyle.TextDecoration = WinFormsConverter.ToTextDecoration(dgvCellStyle.Font);

			reportStyle.TextAlign = WinFormsConverter.ToHorizontalAlign(dgvCellStyle.Alignment);
			reportStyle.VerticalAlign = WinFormsConverter.ToVerticalAlign(dgvCellStyle.Alignment);

			reportStyle.Padding = WinFormsConverter.PaddingToRect(dgvCellStyle.Padding);
			reportStyle.Wrap = dgvCellStyle.WrapMode == DataGridViewTriState.True;

			reportStyle.NullValue = dgvCellStyle.NullValue;
		}

	}
}
