using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseManager
{
	interface ColumnDataTypeDescriptor
	{
		string Name
		{
			get;
		}
		string Quantifier
		{
			get;
		}
	}
}
