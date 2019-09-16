using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;
using ReportExporters.Common;
using ReportExporters.Common.Adapters;
using ReportExporters.Common.Model;
using ReportExporters.Common.Rdlc;
using System.Web.UI.WebControls;
using ReportExporters.Common.Model.DataColumns;
using ReportExporters.Common.Rdlc.Wrapper;
using ReportExporters.Common.Model.TableGroups;
using System.Drawing;
using System.Reflection;
using ReportExporters.Common.Model.Images;
using ReportExporters.Common.Model.Style;
using System.Collections.ObjectModel;

namespace ReportExporters.Common
{
	public class ReportBase
	{
		static int Counter = 0;
		
		private	int UniqueIndex
		{
			get
			{
				return Counter++;
			}
		}
		
		public ReportBase()
		{
			globalEmbeddedImages = new Dictionary<string, System.Drawing.Image>();
			subReports = new Collection<ReportBase>();
			exportedDataSet = new DataSet();
			exportedDataSet.DataSetName = DataSetName;
			exportedTable = new DataTable(DataTableName);
			exportedDataSet.Tables.Add(exportedTable);
		}

		#region Properties

		private string name;
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private Collection<ReportBase> subReports;
		public Collection<ReportBase> SubReports
		{
			get
			{
				return subReports;
			}
		}

		private DataSet exportedDataSet;
		public DataSet ExportedDataSet
		{
			get
			{
				return exportedDataSet;
			}
			set
			{
				exportedDataSet = value;
			}
		}

		private DataTable exportedTable;
		public DataTable ExportedTable
		{
			get
			{
				return exportedTable;
			}
			set
			{
				exportedTable = value;
			}
		}

		private Dictionary<string, System.Drawing.Image> globalEmbeddedImages;
		public Dictionary<string, System.Drawing.Image> GlobalEmbeddedImages
		{
			get
			{
				return globalEmbeddedImages;
			}
			//set
			//{	
			//  globalEmbeddedImages = value;
			//}
		}

		private string rdlc;
		public string Rdlc
		{
			get { return rdlc; }
			set { rdlc = value; }
		}

		public string DataSourceName
		{
			get
			{
				return RDataSet.GetDataSetName(exportedTable);
			}
		}

		internal string DataSetName
		{
			get
			{
				return "DataSet" + UniqueIndex.ToString();//DataTableName  + "_DS";
			}
		}

		internal string DataTableName
		{
			get
			{
				return "DataGrid";
			}
		}

		#endregion
	}

	public interface IReportDataSourceBuilder
	{
		ReportBase GenerateReport();
	}

	/// <summary>
	/// Create report DataSource for DataTable content
	/// </summary>
	public class SingleReportDataSourceBuilder : IReportDataSourceBuilder
	{
		public SingleReportDataSourceBuilder(IReportDataAdapter _reportDataAdapter)
		{
			reportBuilder = new ReportBuilder();
			reportDataAdapter = _reportDataAdapter;
		}

		#region Properties

		IReportDataAdapter reportDataAdapter;
		ReportColumnCollection Columns;
		ReportTableGroupList TableGroups;

		private ReportBuilder reportBuilder;
		private ReportBase _report;


		#endregion

		/// <summary>
		/// Build report for DataGridView content.
		/// Call this method if you have already set all required parameters.
		/// </summary>
		/// <returns>Report</returns>
		public ReportBase GenerateReport()
		{
			_report = new ReportBase();

			Columns = reportDataAdapter.GetColumns();
			TableGroups = reportDataAdapter.GetTableGroups(Columns);

			CreateHeaders();
			CopyRowData();

			reportBuilder.AddDataSource(_report.ExportedTable);
			reportBuilder.AddReportTable(_report.ExportedTable, Columns, TableGroups, reportDataAdapter.GetRowHeight());
			reportBuilder.AddEmbeddedImages(_report.GlobalEmbeddedImages);

			string GeneratedRDLC = reportBuilder.BuildRDLC();
			_report.Rdlc = GeneratedRDLC;

#if DEBUG
			DateTime currentTime = DateTime.Now;
			string strTime = string.Format("{0}-{1}_{2}-{3}-{4}", currentTime.Day, currentTime.Month, currentTime.Hour, currentTime.Minute, currentTime.Second);
			string tempFileName = Path.Combine(
					Path.GetTempPath(),
					string.Format("testReportExport{0}.rdlc", strTime));
			File.WriteAllText(tempFileName, GeneratedRDLC, Encoding.Unicode);
#endif

			return _report;
		}

		/// <summary>
		/// Create DataTable for visible columns of DataGridView 
		/// </summary>
		/// <param name="dgv"></param>
		private void CreateHeaders()
		{
			//add columns to table
			for (int colIndex = 0; colIndex < Columns.Count; colIndex++)
			{
				ReportColumn rColumn = Columns[colIndex];
				Type columnType = rColumn.ValueType;

				if (rColumn is ReportDataColumn)
				{
					ReportDataColumn rdColumn = rColumn as ReportDataColumn;

					//Replace True/False to "+/-"
					if (rdColumn.ValueType == typeof(bool) || rdColumn.ValueType == null)
					{
						columnType = typeof(string);
					}
					else if (rdColumn.ValueType == typeof(System.Drawing.Image) || rdColumn.ValueType == typeof(System.Drawing.Bitmap))
					{
						columnType = typeof(byte[]);
					}

					else if (!String.IsNullOrEmpty(rdColumn.DefaultCellStyle.Format))
					{
						columnType = typeof(string);
					}

				}
				else if (rColumn is ReportHyperlinkColumn)
				{
					if (rColumn.ValueType == typeof(Uri))
					{
						columnType = typeof(string);
					}
					else if (rColumn.ValueType != typeof(string))
					{
						throw new ApplicationException("HyperLinkColumn should have content type System.String or System.Uri.");
					}
				}
				else
				{
					throw new ApplicationException("Unknown ReportColumn type.");
				}

				_report.ExportedTable.Columns.Add(rColumn.Name, columnType);
			}
		}

		/// <summary>
		/// Copy cell items from visible columns retrived by IReportDataAdapter 
		/// </summary>
		/// <param name="dgv"></param>
		private void CopyRowData()
		{
			DataRow dtRow;

			_report.ExportedTable.Rows.Clear();

			int RowCount = reportDataAdapter.GetRowCount();

			//create rows
			for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
			{
				dtRow = _report.ExportedTable.NewRow();
				_report.ExportedTable.Rows.Add(dtRow);
			}

			//add columns to table
			for (int colIndex = 0; colIndex < Columns.Count; colIndex++)
			{
				ReportColumn rColumn = Columns[colIndex];

				//proccess rows
				for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
				{
					object cellValue = reportDataAdapter.GetCellItemValue(rColumn, rowIndex);
					//Exported value
					object expValue = null;

					if (cellValue != null)
					{
						if (rColumn is ReportDataColumn)
						{
							ReportDataColumn rdColumn = rColumn as ReportDataColumn;
							string valueFormat = rdColumn.DefaultCellStyle.Format;
							if (!String.IsNullOrEmpty(valueFormat) || !String.IsNullOrEmpty(rdColumn.TemplateFormat))
							{
								//like {0:[valueFormat]} or {0}
								string fullValueFormat =
										"{0" + (String.IsNullOrEmpty(valueFormat) ? "" : ":" + valueFormat)
										+ "}";

								//like  "Name is {0:[valueFormat]}."
								string fullTemplateFormat = String.IsNullOrEmpty(rdColumn.TemplateFormat)
										 ? fullValueFormat
										 : string.Format(rdColumn.TemplateFormat, fullValueFormat);
								expValue = string.Format(fullTemplateFormat, cellValue);
							}
							else if (rdColumn.ValueConverter != null)
							{
								expValue = rdColumn.ValueConverter.ConvertToString(cellValue);
							}
							else if (rdColumn.ValueType == typeof(System.Drawing.Image) ||
															 rdColumn.ValueType == typeof(System.Drawing.Bitmap))
							{
								System.Drawing.Image imgValue = cellValue as System.Drawing.Image;
								MemoryStream memStream = new MemoryStream();
								imgValue.Save(memStream, imgValue.RawFormat);
								expValue = memStream.ToArray();
							}
							else
							{
								expValue = cellValue;
							}

							ProccessStyle(rdColumn.DefaultCellStyle);
							ProccessStyle(rdColumn.HeaderStyle);
						}
						else if (rColumn is ReportHyperlinkColumn)
						{
							expValue = cellValue.ToString();
						}
					}
					else
					{
						// No value available
					}

					if ((expValue != null) && (expValue.GetType() != typeof(System.Object)))
					{
						_report.ExportedTable.Rows[rowIndex][rColumn.Name] = expValue;
					}
				}
			}
		}

		private void ProccessStyle(BaseStyle style)
		{
			if (style.BackgroundImage != null)
			{
				BackgroundImage cellBackgroundImage = style.BackgroundImage;
				if (cellBackgroundImage.Image is EmbeddedImage)
				{
					EmbeddedImage embeddedImage = (EmbeddedImage)cellBackgroundImage.Image;

					if (!_report.GlobalEmbeddedImages.ContainsKey(embeddedImage.Name))
					{
						_report.GlobalEmbeddedImages.Add(embeddedImage.Name,
							System.Drawing.Image.FromStream(new MemoryStream(embeddedImage.ImageData)));

						embeddedImage.Value = embeddedImage.Name;
					}
					else
					{

					}
				}
			}

		}
	}

	public class MultiReportDataSourceBuilder : IReportDataSourceBuilder
	{
		IList<IReportDataAdapter> reportDataAdapterCollection;

		public MultiReportDataSourceBuilder(IList<IReportDataAdapter> _reportDataAdapters)
		{
			reportBuilder = new ReportBuilder();
			reportDataAdapterCollection = _reportDataAdapters;
		}

		#region Properties

		private ReportBuilder reportBuilder;
		private ReportBase _report;

		#endregion

		/// <summary>
		/// Build report for DataGridView content.
		/// Call this method if you have already set all required parameters.
		/// </summary>
		/// <returns>Report</returns>
		public ReportBase GenerateReport()
		{
			_report = new ReportBase();
			
			for(int aIndex = 0; aIndex < reportDataAdapterCollection.Count ; aIndex++ )
			{
				IReportDataAdapter rdAdapter = reportDataAdapterCollection[aIndex];
				
				string subreportName = "Report" + (aIndex + 1).ToString();
				
				SingleReportDataSourceBuilder srdcBuilder = 
					new SingleReportDataSourceBuilder(rdAdapter);
				
				ReportBase subReport = srdcBuilder.GenerateReport();
				subReport.Name = subreportName;
				
				_report.SubReports.Add(subReport);
				this.reportBuilder.AddSubReport(subreportName);
			}

			_report.ExportedTable.Columns.Add("DummyColumn", typeof(string));
			reportBuilder.AddDataSource(_report.ExportedTable);

			string GeneratedRDLC = reportBuilder.BuildRDLC();
			_report.Rdlc = GeneratedRDLC;

#if DEBUG
			DateTime currentTime = DateTime.Now;
			string strTime = string.Format("{0}-{1}_{2}-{3}-{4}", currentTime.Day, currentTime.Month, currentTime.Hour, currentTime.Minute, currentTime.Second);
			string tempFileName = Path.Combine(
					Path.GetTempPath(),
					string.Format("testMainReportExport{0}.rdlc", strTime));
			File.WriteAllText(tempFileName, GeneratedRDLC, Encoding.Unicode);
#endif

			return _report;
		}
	}
}
