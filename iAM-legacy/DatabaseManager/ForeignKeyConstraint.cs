using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseManager
{
	public enum ReferentialAction
	{
		CASCADE,
		RESTRICT,
		NO_ACTION,
		SET_NULL,
		SET_DEFAULT
	}

	public class ForeignKeyConstraint
	{
		string referentTable;
		List<ColumnPair> columns;
		ReferentialAction action;

		public ForeignKeyConstraint( string referentName, List<ColumnPair> columnReferences, ReferentialAction constraintAction )
		{

			if( referentName != "" )
			{
				if( columnReferences != null && columnReferences.Count != 0 )
				{
					referentTable = referentName;
					columns = columnReferences;
					action = constraintAction;
				}
				else
				{
					throw new ArgumentException( "ERROR: must specify columns." );
				}
			}
			else
			{
				throw new ArgumentException( "ERROR: must specify referent table." );
			}
		}
	}
}
