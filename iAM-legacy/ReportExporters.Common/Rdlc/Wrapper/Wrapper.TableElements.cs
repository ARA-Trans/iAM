using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;
using System.Web.UI.WebControls;
using ReportExporters.Common.Model.DataColumns;
using ReportExporters.Common.Rdlc.Enums;
using System.Data;
using ReportExporters.Common.Model;
using ReportExporters.Common.Model.TableGroups;
using ReportExporters.Common.Model.Images;
using ReportExporters.Common.Model.Style;

namespace ReportExporters.Common.Rdlc.Wrapper
{
	internal partial class RdlcWrapper
	{
		internal static string GetReportColumnValue(DataColumn dataColumn)
		{
			return string.Format("=Fields!{0}.Value", dataColumn.ColumnName);
		}

		internal static string GetReportColumnValue(ReportColumn rColumn)
		{
			return string.Format("=Fields!{0}.Value", rColumn.Name);
		}

		private static ReportControlItem GetReportCellControlItem(ReportDataColumn reportDataColumn)
		{
			ReportControlItem toRet = null;

			if (reportDataColumn.DataCellViewType is CellViewImage)
			{
				CellViewImage imgCellView = reportDataColumn.DataCellViewType as CellViewImage;

				RImage rImage = new RImage(
					reportDataColumn.Name,
					true,
					GetReportColumnValue(reportDataColumn),
					imgCellView.Properties
				);

				if (reportDataColumn.HyperlinkColumn != null)
				{
					rImage.ImageBox.Action = new ReportExporters.Common.Model.Action(
						GetReportColumnValue(reportDataColumn.HyperlinkColumn));
				}

				toRet = rImage;
			}
			else
			{
				RTextBox rTextbox = new RTextBox(reportDataColumn.Name, GetReportColumnValue(reportDataColumn));

				if (reportDataColumn.HyperlinkColumn != null)
				{
					rTextbox.TextBox.Action = new ReportExporters.Common.Model.Action(
						GetReportColumnValue(reportDataColumn.HyperlinkColumn));
				}

				toRet = rTextbox;
			}

			toRet.Item.Style = reportDataColumn.DefaultCellStyle;

			return toRet;
		}

		private static ReportControlItem GetReportHeaderControlItem(ReportDataColumn reportDataColumn)
		{
			ReportControlItem rHeaderItem = null;

			if (reportDataColumn.HeaderCellViewType is CellViewImage)
			{
				CellViewImage imgCellView = reportDataColumn.HeaderCellViewType as CellViewImage;

				RImage rImage = new RImage(
					"header" + reportDataColumn.Name,
					true,
					reportDataColumn.HeaderText,
					imgCellView.Properties
				);

				if ((reportDataColumn.HyperlinkColumn != null) && (reportDataColumn.HeaderCellHyperlink != null))
				{
					rImage.ImageBox.Action = new ReportExporters.Common.Model.Action(reportDataColumn.HeaderCellHyperlink);
				}

				rHeaderItem = rImage;
			}
			else
			{
				RTextBox rHeaderTextbox = new RTextBox(
					"header" + reportDataColumn.Name, reportDataColumn.HeaderText);

				if ((reportDataColumn.HyperlinkColumn != null) && (reportDataColumn.HeaderCellHyperlink != null))
				{
					rHeaderTextbox.TextBox.Action = new ReportExporters.Common.Model.Action(reportDataColumn.HeaderCellHyperlink);
				}

				rHeaderItem = rHeaderTextbox;
			}

			rHeaderItem.Item.Style = reportDataColumn.HeaderStyle;

			return rHeaderItem;
		}

		internal class RHeader : RTableRowsContainer, IRdlElement
		{
			#region IRdlElement Members

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Header");
				{
					base.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RFooter : RTableRowsContainer, IRdlElement
		{
			#region IRdlElement Members

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Footer");
				{
					base.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RTableGroup : IRdlElement
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

			private RHeader _header;
			public RHeader Header
			{
				get
				{
					return _header;
				}
				set
				{
					_header = value;
				}
			}

			private RFooter _footer;
			public RFooter Footer
			{
				get
				{
					return _footer;
				}
				set
				{
					_footer = value;
				}
			}

			public RTableGroup()
			{
				this.Grouping = new RGrouping();
			}

			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("TableGroup");
				{
					Grouping.WriteTo(xmlWriter);

					if (Sorting != null) Sorting.WriteTo(xmlWriter);
					if (Visibility != null) Visibility.WriteTo(xmlWriter);
					if (Header != null) Header.WriteTo(xmlWriter);
					if (Footer != null) Footer.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RTableCell : ReportItemCointainer, IRdlElement
		{
			public RTableCell() : base()
			{
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("TableCell");
				{
					base.WriteTo(xmlWriter);
				}

				xmlWriter.WriteEndElement();
			}
		}

		internal class RTableColumn : IRdlElement
		{
			private Size _width;
			/// <summary>
			/// Width of table column
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

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("TableColumn");
				{
					xmlWriter.WriteElementString("Width", Width.ToString());
				}
				xmlWriter.WriteEndElement();
			}
		}

		internal class RTableRow : IRdlElement
		{
			private RTableCells _tableCells;
			public RTableCells TableCells
			{
				get
				{
					return _tableCells;
				}
			}

			private Size _height;
			public Size Height
			{
				get { return _height; }
				set { _height = value; }
			}

			private RVisibility _visibility;
			public RVisibility Visibility
			{
				get { return _visibility; }
				set { _visibility = value; }
			}

			public BaseStyle Style
			{
				set
				{
					foreach (RTableCell rTableCell in TableCells)
					{
						foreach (ReportControlItem rItem in rTableCell.ReportItems)
						{
							rItem.Item.Style = value;
						}
					}
				}
			}

			public RTableRow()
			{
				Height = new Unit(0.25, UnitType.Inch);
				_tableCells = new RTableCells();
				Visibility = new RVisibility();
			}

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("TableRow");
				{
					Visibility.WriteTo(xmlWriter);
					TableCells.WriteTo(xmlWriter);
					xmlWriter.WriteElementString("Height", Height.ToString());
				}
				xmlWriter.WriteEndElement();
			}
		}

		internal class RTableRowsContainer : IRdlElement
		{
			private RTableRows _tableRows;
			public RTableRows TableRows
			{
				get
				{
					return _tableRows;
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

			public bool IsEmpty
			{
				get
				{
					return (TableRows.Count == 0);
				}
			}

			protected RTableRowsContainer()
			{
				this._tableRows = new RTableRows();
				this._visibility = new RVisibility();
			}

			#region IRdlElement Members

			public virtual void WriteTo(XmlWriter xmlWriter)
			{
				TableRows.WriteTo(xmlWriter);
			}

			#endregion
		}

		/// <summary>
		/// The Details element defines the details rows for a table.
		/// </summary>
		internal class RDetails : RTableRowsContainer, IRdlElement
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

			public RDetails()
			{
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("Details");
				{
					base.WriteTo(xmlWriter);

					if (Grouping != null) Grouping.WriteTo(xmlWriter);
					if (Sorting != null) Sorting.WriteTo(xmlWriter);
					if (Visibility != null) Visibility.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}
		}

		internal class RTable : RDataRegion
		{
			#region Properties
			
			private RDetails Details;
			private RHeader Header;
			private RFooter Footer;
			
			private RTableGroups TableGroups;
			private RTableColumns TableColumns;
			
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
			
			Size rowHeight;

			private RTable()
				: base()
			{
				this._name = String.Format("table{0}", UniqueIndex);
				Header = new RHeader();
				Details = new RDetails();
				Footer = new RFooter();
				TableColumns = new RTableColumns();
			}

			public RTable(string dataSetName)
				: this()
			{
				DataSetName = dataSetName;
				Item.Width = new Unit(9, UnitType.Inch);
			}

			public RTable(DataTable _sourceDataTable, IList<ReportColumn> _reportDataColumns,
				ReportTableGroupList _reportTableGroups, Size _rowHeight)
				: this()
			{
				this.SourceDataTable = _sourceDataTable;
				this.DataSetName = RDataSet.GetDataSetName(_sourceDataTable);
				this.reportDataColumns = _reportDataColumns;
				this.reportTableGroups = _reportTableGroups;
				this.rowHeight = _rowHeight;
			}

			private void SetUpCells()
			{
				Details.TableRows.Clear();

				if ((reportTableGroups != null) && (reportTableGroups.Count > 0))
				{
					#region apply grouping
					
					TableGroups = new RTableGroups();
					for (int rtgIndex = 0; rtgIndex < reportTableGroups.Count; rtgIndex++)
					{
						ReportTableGroup reportTableGroup = reportTableGroups[rtgIndex];
						//ReportColumn firstColumn = reportTableGroup.ColumnGrouping[0];

						RTableGroup newGroup = new RTableGroup();
						newGroup.Grouping.Name = string.Format("{0}_Group{1}", Name, rtgIndex + 1);

						//Grouping
						foreach (ReportColumn groupingColumn in reportTableGroup.ColumnGrouping)
						{
							RExpression groupingExpression = new RExpression(GetReportColumnValue(groupingColumn));
							newGroup.Grouping.GroupExpressions.Add(groupingExpression);
						}

						//Sorting
						if (reportTableGroup.ColumnSorting.Count > 0)
						{
							RSorting groupSorting = new RSorting();
							foreach (ReportColumn sortingColumn in reportTableGroup.ColumnSorting.Keys)
							{
								RSortBy sortByColumn = new RSortBy();
								sortByColumn.SortExpression = GetReportColumnValue(sortingColumn);
								sortByColumn.Direction = reportTableGroup.ColumnSorting[sortingColumn];
								groupSorting.SortBy.Add(sortByColumn);
							}
							newGroup.Sorting = groupSorting;
						}

						//Header
						{
							RHeader groupHeader = new RHeader();
							{
								RTableRow headerRow = new RTableRow();
								{
									BaseStyle hightlightStyleForRow = null;

									foreach (ReportColumn reportColumn in ReportDataColumns)
									{
										if (reportColumn is ReportDataColumn)
										{
											ReportControlItem tbxHeader;

											if (reportTableGroup.ColumnGrouping.Contains(reportColumn))
											{
												tbxHeader = GetReportCellControlItem(reportColumn as ReportDataColumn);
												if (hightlightStyleForRow == null)
												{
													hightlightStyleForRow = tbxHeader.Item.Style;
												}
											}
											else
											{
												tbxHeader = new RTextBox(String.Empty);
											}

											RTableCell rTableCell = new RTableCell();
											rTableCell.ReportItems.Add(tbxHeader);
											headerRow.TableCells.Add(rTableCell);
										}
									}

									if (hightlightStyleForRow != null)
									{
										hightlightStyleForRow.Border.Style = BorderStyle.None;
										headerRow.Style = hightlightStyleForRow;
									}
								}

								groupHeader.TableRows.Add(headerRow);
							}
							newGroup.Header = groupHeader;
						}

						//Footer
						{
							//RFooter groupFooter = new RFooter();
							//newGroup.Footer = groupFooter;
						}

						TableGroups.Add(newGroup);
					}
					
					#endregion
				}

				RTableCell tCell;
				RTableRow tRow = new RTableRow();
				tRow.Height = this.rowHeight;

				foreach (DataColumn dataColumn in SourceDataTable.Columns)
				{
					tCell = new RTableCell();

					ReportControlItem rDataTableItem;
					int columnIndex = SourceDataTable.Columns.IndexOf(dataColumn);
					int reportItemZIndex = columnIndex + 1;

					ReportColumn reportColumn = reportDataColumns[columnIndex];

					if (reportColumn is ReportDataColumn)
					{
						ReportDataColumn reportDataColumn = reportColumn as ReportDataColumn;

						if ((this.reportTableGroups != null) && (this.reportTableGroups.ContainsColumn(reportColumn)))
						{
							rDataTableItem = new RTextBox(string.Empty);
						}
						else
						{
							rDataTableItem = GetReportCellControlItem(reportDataColumn);
						}

						tCell.ReportItems.Add(rDataTableItem);
						tRow.TableCells.Add(tCell);
					}
					else if (reportColumn is ReportHyperlinkColumn)
					{
					}
				}

				Details.TableRows.Add(tRow);
			}

			private void SetUpColumnsHeaders()
			{
				Header.TableRows.Clear();
				RTableCell tCell;
				RTableRow headerRow = new RTableRow();

				double sumColWidthInMillimeters = 0;

				foreach (ReportColumn reportColumn in ReportDataColumns)
				{
					if (reportColumn is ReportDataColumn)
					{
						ReportDataColumn reportDataColumn = reportColumn as ReportDataColumn;

						Unit _widthOfColumn = reportDataColumn.HeaderStyle.Width;
						if (_widthOfColumn.Value == 0)
						{
							_widthOfColumn = reportDataColumn.DefaultCellStyle.Width;
						}
						
						RTableColumn newColumn = new RTableColumn();
						newColumn.Width = _widthOfColumn;
						this.TableColumns.Add(newColumn);
						sumColWidthInMillimeters += MeasureTools.UnitToMillimeters(_widthOfColumn);

						tCell = new RTableCell();

						ReportControlItem rHeaderItem;
						int columnIndex = ReportDataColumns.IndexOf(reportDataColumn);
						int reportItemZIndex = columnIndex + 1;
						
						rHeaderItem = GetReportHeaderControlItem(reportDataColumn);

						//////////////////////					
						tCell.ReportItems.Add(rHeaderItem);
						headerRow.TableCells.Add(tCell);
					}
					else if (reportColumn is ReportHyperlinkColumn)
					{
					}
				}

				Item.Width = new Unit(sumColWidthInMillimeters, UnitType.Mm);
				Header.TableRows.Add(headerRow);
			}

			public override void WriteTo(XmlWriter xmlWriter)
			{
				SetUpColumnsHeaders();
				SetUpCells();

				xmlWriter.WriteStartElement("Table");
				{
					xmlWriter.WriteAttributeString("Name", Name);

					if (TableGroups != null) TableGroups.WriteTo(xmlWriter);
					if (!Details.IsEmpty)
						Details.WriteTo(xmlWriter);
					if (!Header.IsEmpty)
						Header.WriteTo(xmlWriter);
					if (!Footer.IsEmpty)
						Footer.WriteTo(xmlWriter);

					TableColumns.WriteTo(xmlWriter);

					base.WriteTo(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}

		}

		#region Collections

		internal class RTableColumns : Collection<RTableColumn>, IRdlElement
		{
			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("TableColumns");
				{
					foreach (RTableColumn tableColumn in this)
					{
						tableColumn.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}
		}

		internal class RTableGroups : Collection<RTableGroup>, IRdlElement
		{
			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("TableGroups");
				{
					foreach (RTableGroup tableGroup in this)
					{
						tableGroup.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		internal class RTableRows : Collection<RTableRow>, IRdlElement
		{
			public RTableRows()
			{
			}

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("TableRows");
				{
					foreach (RTableRow tableRow in this)
					{
						tableRow.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}
		}

		internal class RTableCells : Collection<RTableCell>, IRdlElement
		{
			#region IRdlElement Members

			public void WriteTo(XmlWriter xmlWriter)
			{
				xmlWriter.WriteStartElement("TableCells");
				{
					foreach (RTableCell tableCell in this)
					{
						tableCell.WriteTo(xmlWriter);
					}
				}
				xmlWriter.WriteEndElement();
			}

			#endregion
		}

		#endregion
	}
}
