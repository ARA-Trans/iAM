using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;
using ReportExporters.Common.Rdlc.Enums;
using ReportExporters.Common.Model.DataColumns;
using System.Web.UI.WebControls;
using System.Data;
using ReportExporters.Common.Model.TableGroups;
using ReportExporters.Common.Model;
using ReportExporters.Common.Model.Style;

namespace ReportExporters.Common.Rdlc.Wrapper
{
	internal partial class RdlcWrapper
	{
		internal class OneReportItemContainer : IRdlElement
		{
			private ReportControlItem _reportItem;
			/// <summary>
			/// One item for ReportItems element
			/// </summary>
			public ReportControlItem ReportItem
			{
				get
				{
					return _reportItem;
				}
				set
				{
					_reportItem = value;
				}
			}

			#region IRdlElement Members

			public virtual void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("ReportItems");
				{
					if (ReportItem == null)
					{
						ReportItem = new RTextBox(String.Empty);
					}

					ReportItem.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class OneReportItemContainerWithGroupingSortingVisibility : OneReportItemContainer, IRdlElement
		{
			private RGrouping _grouping;
			public RGrouping Grouping
			{
				get
				{
					return _grouping;
				}
				set
				{
					_grouping = value;
				}
			}

			private RSorting _sorting;
			public RSorting Sorting
			{
				get
				{
					return _sorting;
				}
				set
				{
					_sorting = value;
				}
			}

			private RVisibility _visibility;
			public RVisibility Visibility
			{
				get
				{
					return _visibility;
				}
				set
				{
					_visibility = value;
				}
			}

			public OneReportItemContainerWithGroupingSortingVisibility()
				: base()
			{
			}

			#region IRdlElement Members

			public override void WriteTo(XmlWriter xmlWriter)
			{
				if (Grouping != null) Grouping.WriteTo(xmlWriter);
				if (Sorting != null) Sorting.WriteTo(xmlWriter);
				if (Visibility != null) Visibility.WriteTo(xmlWriter);

				base.WriteTo(xmlWriter);
			}

			#endregion
		}

		internal class OneReportItemContainerWithGroupingSortingVisibilityWithSubtotal :
			OneReportItemContainerWithGroupingSortingVisibility, IRdlElement
		{
			private RSubtotal _subtotal;
			public RSubtotal Subtotal
			{
				get
				{
					return _subtotal;
				}
				set
				{
					_subtotal = value;
				}
			}

			public OneReportItemContainerWithGroupingSortingVisibilityWithSubtotal()
				: base()
			{
			}

			#region IRdlElement Members

			public override void WriteTo(XmlWriter xmlWriter)
			{
				base.WriteTo(xmlWriter);
				if (Subtotal != null) Subtotal.WriteTo(xmlWriter);
			}

			#endregion
		}

		internal class RCorner : OneReportItemContainer, IRdlElement
		{
			public RCorner()
				: base()
			{
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				if (ReportItem != null)
				{
					xmlWriter.WriteStartElement("Corner");
					{
						base.WriteTo(xmlWriter);
					}
					xmlWriter.WriteEndElement();
				}
			}
		}

		#region Columns

		internal class RColumnGroupings : Collection<RColumnGrouping>, IRdlElement
		{
			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("ColumnGroupings");
				{
					foreach (RColumnGrouping columnGrouping in this)
					{
						columnGrouping.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RColumnGrouping : IRdlElement
		{
			private Size _height;
			public Size Height
			{
				get
				{
					return _height;
				}
				set
				{
					_height = value;
				}
			}

			private RDynamicColumns _dynamicColumns;
			public RDynamicColumns DynamicColumns
			{
				get
				{
					return _dynamicColumns;
				}
				set
				{
					_dynamicColumns = value;
				}
			}

			private RStaticColumns _staticColumns;
			public RStaticColumns StaticColumns
			{
				get
				{
					return _staticColumns;
				}
				set
				{
					_staticColumns = value;
				}
			}

			//private bool _fixedHeader;
			//public bool FixedHeader
			//{
			//  get
			//  {
			//    return _fixedHeader;
			//  }
			//  set
			//  {
			//    _fixedHeader = value;
			//  }
			//}

			public RColumnGrouping()
			{
			}

			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("ColumnGrouping");
				{
					xmlWriter.WriteElementString("Height", Height.ToString());

					if (DynamicColumns != null) DynamicColumns.WriteTo(xmlWriter);
					if (StaticColumns != null) StaticColumns.WriteTo(xmlWriter);

					//xmlWriter.WriteElementString("FixedHeader", FixedHeader.ToString());
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RDynamicColumns : OneReportItemContainerWithGroupingSortingVisibilityWithSubtotal, IRdlElement
		{
			public RDynamicColumns()
				: base()
			{
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("DynamicColumns");
				{
					base.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}
		}

		internal class RStaticColumns : Collection<RStaticColumn>, IRdlElement
		{
			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("StaticColumns");
				{
					foreach (RStaticColumn staticColumn in this)
					{
						staticColumn.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RStaticColumn : OneReportItemContainer, IRdlElement
		{
			public RStaticColumn()
				: base()
			{
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("StaticColumn");
				{
					base.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}
		}

		#endregion

		#region Rows

		internal class RRowGroupings : Collection<RRowGrouping>, IRdlElement
		{
			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("RowGroupings");
				{
					foreach (RRowGrouping rowGrouping in this)
					{
						rowGrouping.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RRowGrouping : IRdlElement
		{
			private Size _width;
			public Size Width
			{
				get
				{
					return _width;
				}
				set
				{
					_width = value;
				}
			}

			private RDynamicRows _dynamicRows;
			public RDynamicRows DynamicRows
			{
				get
				{
					return _dynamicRows;
				}
				set
				{
					_dynamicRows = value;
				}
			}

			private RStaticRows _staticRows;
			public RStaticRows StaticRows
			{
				get
				{
					return _staticRows;
				}
				set
				{
					_staticRows = value;
				}
			}

			//private bool _fixedHeader;
			//public bool FixedHeader
			//{
			//  get
			//  {
			//    return _fixedHeader;
			//  }
			//  set
			//  {
			//    _fixedHeader = value;
			//  }
			//}

			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("RowGrouping");
				{
					xmlWriter.WriteElementString("Width", Width.ToString());

					if (DynamicRows != null) DynamicRows.WriteTo(xmlWriter);
					if (StaticRows != null) StaticRows.WriteTo(xmlWriter);

					//xmlWriter.WriteElementString("FixedHeader", FixedHeader.ToString());
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RDynamicRows : OneReportItemContainerWithGroupingSortingVisibilityWithSubtotal, IRdlElement
		{
			public RDynamicRows()
				: base()
			{
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("DynamicRows");
				{
					base.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}
		}

		internal class RStaticRows : Collection<RStaticRow>, IRdlElement
		{
			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("StaticRows");
				{
					foreach (RStaticRow staticRow in this)
					{
						staticRow.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RStaticRow : OneReportItemContainer, IRdlElement
		{
			public RStaticRow()
				: base()
			{
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("StaticRow");
				{
					base.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}
		}

		#endregion

		internal class RSubtotal : OneReportItemContainer, IRdlElement
		{
			private RStyle _style;
			public RStyle Style
			{
				get { return _style; }
				set { _style = value; }
			}

			private Position _position;
			public Position Position
			{
				get { return _position; }
				set { _position = value; }
			}

			//DataElementName
			//DataElementOutput

			#region IRdlElement Members

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Subtotal");
				{
					base.WriteTo(xmlWriter);
					if (Style != null) Style.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RMatrixColumns : Collection<RMatrixColumn>, IRdlElement
		{
			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("MatrixColumns");
				{
					foreach (RMatrixColumn matrixColumn in this)
					{
						matrixColumn.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RMatrixColumn : IRdlElement
		{
			private Size _width;
			/// <summary>
			/// Width of each detail cell in this column
			/// </summary>
			public Size Width
			{
				get
				{
					return _width;
				}
				set
				{
					_width = value;
				}
			}

			public RMatrixColumn()
			{
			}

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("MatrixColumn");
				{
					xmlWriter.WriteElementString("Width", Width.ToString());
				}
				xmlWriter.WriteEndElement();
			}
		}

		internal class RMatrixRows : Collection<RMatrixRow>, IRdlElement
		{
			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("MatrixRows");
				{
					foreach (RMatrixRow matrixRow in this)
					{
						matrixRow.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RMatrixRow : IRdlElement
		{
			private Size _height;
			/// <summary>
			/// Height of each detail cell in this row.
			/// </summary>
			public Size Height
			{
				get
				{
					return _height;
				}
				set
				{
					_height = value;
				}
			}

			private RMatrixCells _matrixCells;
			public RMatrixCells MatrixCells
			{
				get
				{
					return _matrixCells;
				}
				set
				{
					_matrixCells = value;
				}
			}

			public RMatrixRow()
			{
				this.MatrixCells = new RMatrixCells();
			}

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("MatrixRow");
				{
					MatrixCells.WriteTo(xmlWriter);
					xmlWriter.WriteElementString("Height", Height.ToString());
				}
				xmlWriter.WriteEndElement();
			}
		}

		internal class RMatrixCells : Collection<RMatrixCell>, IRdlElement
		{
			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("MatrixCells");
				{
					foreach (RMatrixCell matrixCell in this)
					{
						matrixCell.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RMatrixCell : OneReportItemContainer, IRdlElement
		{
			public RMatrixCell()
				: base()
			{
			}

			#region IRdlElement Members

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("MatrixCell");
				{
					base.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RMatrix : RDataRegion
		{
			#region Properties

			private RCorner _corner;
			public RCorner Corner
			{
				get
				{
					return _corner;
				}
				set
				{
					_corner = value;
				}
			}

			private RColumnGroupings _columnGroupings;
			public RColumnGroupings ColumnGroupings
			{
				get
				{
					return _columnGroupings;
				}
				set
				{
					_columnGroupings = value;
				}
			}

			private RRowGroupings _rowGroupings;
			public RRowGroupings RowGroupings
			{
				get
				{
					return _rowGroupings;
				}
				set
				{
					_rowGroupings = value;
				}
			}

			private RMatrixRows _matrixRows;
			public RMatrixRows MatrixRows
			{
				get
				{
					return _matrixRows;
				}
				set
				{
					_matrixRows = value;
				}
			}

			private RMatrixColumns _matrixColumns;
			public RMatrixColumns MatrixColumns
			{
				get
				{
					return _matrixColumns;
				}
				set
				{
					_matrixColumns = value;
				}
			}

			private LayoutDirection _layoutDirection;
			public LayoutDirection LayoutDirection
			{
				get
				{
					return _layoutDirection;
				}
				set
				{
					_layoutDirection = value;
				}
			}

			#endregion

			private DataTable SourceDataTable;

			private IList<ReportColumn> reportDataColumns;
			public IList<ReportColumn> ReportDataColumns
			{
				get
				{
					return reportDataColumns;
				}
			}

			ReportTableGroupList reportTableGroups;

			private RMatrix()
				: base()
			{
				this._name = String.Format("matrix{0}", UniqueIndex);
				this.ColumnGroupings = new RColumnGroupings();
				this.RowGroupings = new RRowGroupings();
				this.MatrixRows = new RMatrixRows();
				this.MatrixColumns = new RMatrixColumns();
				this.LayoutDirection = LayoutDirection.LTR;
			}

			public RMatrix(DataTable _sourceDataTable, IList<ReportColumn> _reportDataColumns,
				ReportTableGroupList _reportTableGroups)
				: this()
			{
				this.SourceDataTable = _sourceDataTable;
				this.DataSetName = RDataSet.GetDataSetName(_sourceDataTable);
				this.reportDataColumns = _reportDataColumns;
				this.reportTableGroups = _reportTableGroups;
			}

			private void SetUpColumnsGroupings()
			{
				this.ColumnGroupings.Clear();
				
				
				RColumnGrouping columnGrouping0 = new RColumnGrouping();
				{
					columnGrouping0.Height = new Unit(0.3, UnitType.Inch);

					RDynamicColumns dynamicColumns = new RDynamicColumns();
					{
						dynamicColumns.Grouping = new RGrouping(this, this.ColumnGroupings);
						dynamicColumns.Grouping.GroupExpressions.Add(
							new RExpression(
									GetReportColumnValue(ReportDataColumns[2])
								));
								
						dynamicColumns.ReportItem = GetReportCellControlItem(ReportDataColumns[2] as ReportDataColumn); 
					}
					columnGrouping0.DynamicColumns = dynamicColumns;
				}
				this.ColumnGroupings.Add(columnGrouping0);
				
				//Static headers
				RColumnGrouping columnGrouping1 = new RColumnGrouping();
				{
					columnGrouping1.Height = new Unit(0.3, UnitType.Inch);

					RStaticColumns staticColumns = new RStaticColumns();
					{
						foreach (ReportColumn reportColumn in ReportDataColumns)
						{
							RStaticColumn staticColumn = new RStaticColumn();
							if (reportColumn is ReportDataColumn)
							{
								ReportDataColumn reportDataColumn = reportColumn as ReportDataColumn;
								staticColumn.ReportItem = GetReportHeaderControlItem(reportDataColumn);
								staticColumns.Add(staticColumn);
							}
							else if (reportColumn is ReportHyperlinkColumn)
							{
							}
						}
					}

					columnGrouping1.StaticColumns = staticColumns;
				}
				this.ColumnGroupings.Add(columnGrouping1);
			}

			private void SetUpRowGroupings()
			{
				this.RowGroupings.Clear();

				RRowGrouping rowGrouping = new RRowGrouping();
				{
					rowGrouping.Width = new Unit(0.25, UnitType.Inch);

					//DynamicRows
					{
						RDynamicRows dynamicRows = new RDynamicRows();

						RGrouping rowGrouping1 = new RGrouping(this, this.RowGroupings);
						{
							RExpression groupingExpression1 =
								new RExpression(GetReportColumnValue(ReportDataColumns[7]));
							//new RExpression(String.Empty);

							rowGrouping1.GroupExpressions.Add(groupingExpression1);
						}
						dynamicRows.Grouping = rowGrouping1;
						dynamicRows.ReportItem = GetReportCellControlItem(ReportDataColumns[7] as ReportDataColumn);

						rowGrouping.DynamicRows = dynamicRows;
					}

					//StaticRows
					{
						//RStaticRows staticRows = new RStaticRows();
						//RStaticRow staticRow1 = new RStaticRow();

						//staticRow1.ReportItem = GetReportCellControlItem(ReportDataColumns[7] as ReportDataColumn);

						//staticRows.Add(staticRow1);
						//rowGrouping.StaticRows = staticRows;
					}
				}
				this.RowGroupings.Add(rowGrouping);
			}

			private void SetUpCells()
			{
				#region MatrixColumns
				{
					MatrixColumns.Clear();
					double sumColWidthInMillimeters = 0;

					foreach (ReportColumn reportColumn in ReportDataColumns)
					{
						if (reportColumn is ReportDataColumn)
						{
							ReportDataColumn reportDataColumn = reportColumn as ReportDataColumn;
							Unit _widthOfColumn = reportDataColumn.HeaderStyle.Width;
							RMatrixColumn matrixColumn = new RMatrixColumn();
							matrixColumn.Width = _widthOfColumn;
							MatrixColumns.Add(matrixColumn);
							sumColWidthInMillimeters += MeasureTools.UnitToMillimeters(_widthOfColumn);
						}
						else if (reportColumn is ReportHyperlinkColumn)
						{
						}
					}

					Item.Width = new Unit(sumColWidthInMillimeters, UnitType.Mm);
				}

				#endregion

				#region MatrixRows

				this.MatrixRows.Clear();

				RMatrixRow matrixRow1 = new RMatrixRow();
				{
					matrixRow1.Height = new Unit(0.25, UnitType.Inch);

					RMatrixCell mCell;
					foreach (DataColumn dataColumn in SourceDataTable.Columns)
					{
						mCell = new RMatrixCell();

						ReportControlItem rDataTableItem;
						int columnIndex = SourceDataTable.Columns.IndexOf(dataColumn);
						int reportItemZIndex = columnIndex + 1;

						ReportColumn reportColumn = reportDataColumns[columnIndex];

						if (reportColumn is ReportDataColumn)
						{
							ReportDataColumn reportDataColumn = reportColumn as ReportDataColumn;

							if (this.reportTableGroups.ContainsColumn(reportColumn))
							{
								rDataTableItem = new RTextBox(string.Empty);
							}
							else
							{
								rDataTableItem = GetReportCellControlItem(reportDataColumn);

								//if (rDataTableItem is RTextBox)
								//{
								//  (rDataTableItem as RTextBox).Value =
								//    string.Format("=CDbl(Fields!{0}.Value)", reportDataColumn.Name);
								//}
								
								//=Sum(CDbl(Fields!Sales.Value))
							}

							mCell.ReportItem = rDataTableItem;
							matrixRow1.MatrixCells.Add(mCell);
						}
						else if (reportColumn is ReportHyperlinkColumn)
						{
						}
					}
				}
				this.MatrixRows.Add(matrixRow1);

				#endregion
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				SetUpCells();
				SetUpColumnsGroupings();
				SetUpRowGroupings();

				xmlWriter.WriteStartElement("Matrix");
				{
					xmlWriter.WriteAttributeString("Name", Name);

					if (Corner != null)
						Corner.WriteTo(xmlWriter);

					ColumnGroupings.WriteTo(xmlWriter);
					RowGroupings.WriteTo(xmlWriter);

					MatrixColumns.WriteTo(xmlWriter);
					MatrixRows.WriteTo(xmlWriter);

					base.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}
		}

	}
}
