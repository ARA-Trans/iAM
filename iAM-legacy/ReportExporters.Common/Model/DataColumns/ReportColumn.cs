using System;
using System.Collections.Generic;
using System.Text;

namespace ReportExporters.Common.Model.DataColumns
{
	public class ReportColumn
	{
		protected string name;
		public string Name
		{
			get { return name; }
			//set { name = value; }
		}

		private int index;
		public int Index
		{
			get { return index; }
		}

		public int ColumnIndex
		{
			set { index = value; }
		}

		private Type valueType;
		public Type ValueType
		{
			get { return valueType; }
			set { valueType = value; }
		}
	}
}
