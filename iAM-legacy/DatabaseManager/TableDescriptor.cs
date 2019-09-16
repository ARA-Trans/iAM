using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseManager
{
	public class TableDescriptor
	{
		string name;
		List<ColumnDescriptor> columns;
		List<ColumnDescriptor> keyColumns;

		public TableDescriptor( string tableName )
		{
			name = tableName;
			columns = new List<ColumnDescriptor>();
			keyColumns = new List<ColumnDescriptor>();
		}

		public TableDescriptor( string tableName, List<ColumnDescriptor> tableColumns )
		{
			name = tableName;
			columns = tableColumns;
			keyColumns = new List<ColumnDescriptor>();
		}

		public TableDescriptor( string tableName, List<ColumnDescriptor> tableColumns, List<ColumnDescriptor> primaryKeyColumns )
		{
			name = tableName;
			columns = tableColumns;
			keyColumns = primaryKeyColumns;
		}

		public string CreateStatement
		{
			get
			{
				string createClause = "CREATE TABLE " + name.ToUpper() + "( ";
				//createClause += GenerateColumnSubclause();
				return createClause;
			}
		}
	}
}
