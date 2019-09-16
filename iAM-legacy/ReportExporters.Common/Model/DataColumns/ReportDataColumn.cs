using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Model.Style;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ReportExporters.Common.Model.DataColumns
{
	public class ReportDataColumn : ReportColumn
	{
		private ReportDataColumn()
		{
			templateFormat = string.Empty;
			
			this.HeaderStyle = new ReportStyle();
			this.DefaultCellStyle = new ReportTableItemStyle();

			this.HeaderCellViewType = new CellViewText();
			this.DataCellViewType = new CellViewText();
		}

		public ReportDataColumn(string _name)
			: this()
		{
			this.name = _name;
		}

		private TypeConverter valueConverter;
		/// <summary>
		/// Custom TypeConverter for converting value at generating report.
		/// </summary>
		public TypeConverter ValueConverter
		{
			get { return valueConverter; }
			set { valueConverter = value; }
		}

		private string templateFormat;
		/// <summary>
		/// Format string which allow to pass one argument 
		/// Like "Image size is {0} Kb"
		/// </summary>
		public string TemplateFormat
		{
			get { return templateFormat; }
			set { templateFormat = value; }
		}

		private string headerText;
		/// <summary>
		/// Text in column header cell
		/// </summary>
		public string HeaderText
		{
			get { return headerText; }
			set { headerText = value; }
		}

		private ReportStyle headerStyle;
		/// <summary>
		/// Style for column header cell
		/// </summary>
		public ReportStyle HeaderStyle
		{
			get { return headerStyle; }
			set { headerStyle = value; }
		}

		private ReportTableItemStyle defaultCellStyle;
		/// <summary>
		/// Style for column item cell
		/// </summary>
		public ReportTableItemStyle DefaultCellStyle
		{
			get { return defaultCellStyle; }
			set { defaultCellStyle = value; }
		}

		private CellViewType headerCellViewType;
		/// <summary>
		/// Type of ReportControl for column header cell
		/// </summary>
		public CellViewType HeaderCellViewType
		{
			get { return headerCellViewType; }
			set { headerCellViewType = value; }
		}

		private CellViewType dataCellViewType;
		/// <summary>
		/// Type of ReportControl for column item cell
		/// </summary>
		public CellViewType DataCellViewType
		{
			get { return dataCellViewType; }
			set { dataCellViewType = value; }
		}

		private Uri _headerCellHyperlink;
		/// <summary>
		/// Column header cell hyperlink
		/// </summary>
		public Uri HeaderCellHyperlink
		{
			get
			{
				return _headerCellHyperlink;
			}
			set
			{
				_headerCellHyperlink = value;
			}
		}
		
		private ReportHyperlinkColumn _hyperlinkColumn;
		/// <summary>
		/// Column that contains hyperlink for item cell 
		/// </summary>
		public ReportHyperlinkColumn HyperlinkColumn
		{
			get
			{
				return _hyperlinkColumn;
			}
			set
			{
				_hyperlinkColumn = value;
			}
		}
	}


}
