using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseManager
{
	public class ColumnPair
	{
		string localColumn;
		string referentColumn;

		public ColumnPair( string localColumnName, string referentColumnName )
		{
			localColumn = localColumnName;
			referentColumn = referentColumnName;
		}
	}
}
