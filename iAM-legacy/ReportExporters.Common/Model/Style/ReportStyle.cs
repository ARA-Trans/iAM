using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Web.UI.WebControls;

namespace ReportExporters.Common.Model.Style
{
	public class ReportStyle : BaseStyle
	{
		public ReportStyle() : base()
		{
		}

		private Size width;
		public Size Width
		{
			get { return width; }
			set { width = value; }
		}

		private Size height;
		public Size Height
		{
			get { return height; }
			set { height = value; }
		}

		private object nullValue = string.Empty;
		/// <summary>
		/// A string displayed in a column cell null value.
		/// </summary>
		public object NullValue
		{
			get { return nullValue; }
			set { nullValue = value; }
		}

		private bool wrap;
		/// <summary>
		/// Specify the content of the cell wrap in the cell
		/// </summary>
		public bool Wrap
		{
			get { return wrap; }
			set { wrap = value; }
		}
	}
}
