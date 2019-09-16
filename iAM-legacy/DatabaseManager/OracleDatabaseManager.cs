using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data.Common;
using System.Data.OleDb;

namespace DatabaseManager
{
	public static class OracleDatabaseManager
	{
		public static int ExecuteParameterizedNonQuery( string sqlStatement, List<DbParameter> statementParameters, ConnectionParameters cp )
		{
			int rowsAffected = 0;
			OleDbTransaction commandTransaction = null;
			try
			{
				if( !cp.IsNative )
				{
					cp.OleDbConnection.Open();
				}
				OleDbCommand cmd = new OleDbCommand( sqlStatement, cp.OleDbConnection );
				foreach( DbParameter param in statementParameters )
				{
					cmd.Parameters.Add( (OleDbParameter)param );
				}
				commandTransaction = cp.OleDbConnection.BeginTransaction();
				cmd.Transaction = commandTransaction;
				rowsAffected = cmd.ExecuteNonQuery();
				commandTransaction.Commit();

				//This is necessary because if this function is called repeatedly with the same parameter object
				//it will not have been garbage collected properly and so it will throw
				//"The OleDbParameter is already contained by another OleDbParameterCollection."
				//when the parameter is added.
				cmd.Parameters.Clear();

				//This is necessary to avoid "ORA-01000: maximum open cursors exceeded"
				cmd.Dispose();
			}
			catch( Exception ex )
			{
				commandTransaction.Rollback();
				Debug.WriteLine( ex.Message );
				throw ex;
			}
			finally
			{
				if( !cp.IsNative )
				{
					cp.OleDbConnection.Close();
				}
			}
			return rowsAffected;
		}
	}
}
