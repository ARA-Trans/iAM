using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace DatabaseManager
{
    public class DataAdapter
    {
        private SqlDataAdapter SDA = null;
        private OleDbDataAdapter ODA = null;
        private SqlCommandBuilder sqlCmdBuilder = null;
        private OleDbCommandBuilder oleCmdBuilder = null;
		private ConnectionParameters adapterConnectionInfo = null;

        #region IDataAdapter Members

        /// <summary>
        /// Used for creating generic data adapters for reading tables for DataGridView
        /// </summary>
        /// <param name="strQuery"></param>
		public DataAdapter(String strQuery ) : this( strQuery, DBMgr.NativeConnectionParameters )
		{
		}

        /// <summary>
        /// This constructor is used for attributes and assets that might be non-native connections.
        /// </summary>
        /// <param name="strQuery">Query to run</param>
        /// <param name="cp">Connection information is stored in this class.</param>
        public DataAdapter(String strQuery, ConnectionParameters cp)
        {
			if( cp.IsOleDBConnection )
			{
				ODA = new OleDbDataAdapter(strQuery, cp.OleDbConnection);
				oleCmdBuilder = new OleDbCommandBuilder( ODA );
			}
			else
			{
				SDA = new SqlDataAdapter( strQuery, cp.SqlConnection );
				sqlCmdBuilder = new SqlCommandBuilder( SDA );
			}

			adapterConnectionInfo = cp;
        }

        public int Fill(DataTable dataTable)
        {
            int iReturn;
            if (SDA != null)
            {
                iReturn = SDA.Fill(dataTable);
            }
            else
            {
                iReturn = ODA.Fill(dataTable);
            }
            return iReturn;
        }

		public int Fill(DataSet dataSet, String tableName)
		{
            int iReturn;
            if (SDA != null)
            {
                iReturn = SDA.Fill(dataSet, tableName);
            }
            else
            {
                iReturn = ODA.Fill(dataSet, tableName);
            }
            return iReturn;
 		}

        public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
        {
            DataTable[] dt;
            if (SDA != null)
            {
                dt = SDA.FillSchema(dataSet, schemaType);
            }
            else
            {
                dt = ODA.FillSchema(dataSet, schemaType);
            }
            return dt;
        }

        public IDataParameter[] GetFillParameters()
        {
            IDataParameter[] ida;
            if (SDA != null)
            {
                ida = SDA.GetFillParameters();
            }
            else
            {
                ida = ODA.GetFillParameters();
            }
            return ida;
        }

        public MissingMappingAction MissingMappingAction
        {
            get
            {
                if (SDA != null)
                {
                    return SDA.MissingMappingAction;
                }
                else
                {
                    return ODA.MissingMappingAction;
                }
            }
            set
            {
                if (SDA != null)
                {
                    SDA.MissingMappingAction = value;
                }
                else
                {
                    ODA.MissingMappingAction = value;
                }
            }
        }

        public MissingSchemaAction MissingSchemaAction
        {
            get
            {
                if (SDA != null)
                {
                    return SDA.MissingSchemaAction;
                }
                else
                {
                    return ODA.MissingSchemaAction;
                }
            }
            set
            {
                if (SDA != null)
                {
                    SDA.MissingSchemaAction = value;
                }
                else
                {
                    ODA.MissingSchemaAction = value;
                }
            }
        }

        public ITableMappingCollection TableMappings
        {
            get { if (SDA != null) { return SDA.TableMappings; } else { return ODA.TableMappings; } }
        }

		public int Update( DataTable dataTable )
		{
			int affectedRows = 0;
			if( SDA != null )
			{
				return SDA.Update( dataTable );
			}
			else
			{
				switch( adapterConnectionInfo.Provider )
				{
					case "MSSQL":
						affectedRows = ODA.Update( dataTable );
						break;
					case "ORACLE":
						string query = "";
						List<string> updateCommands = new List<string>();
						List<DataRow> updatedRows = new List<DataRow>();
						foreach( DataRow row in dataTable.Rows )
						{
							switch( row.RowState )
							{
								case DataRowState.Added:
									if( !EmptyRow( row ) )
									{
										String valuesClauseString;
										query = "INSERT INTO " + dataTable.TableName + " (";
										valuesClauseString = " VALUES( ";
										foreach( DataColumn column in row.Table.Columns )
										{
											if( column.ColumnName.ToUpper() != "ID_" )	//need to skip the auto-increment columns
											{
												if( column.ColumnName.ToUpper() == dataTable.TableName.ToUpper() )
												{
													query += "\"DATA_\", ";
												}
												else
												{
													query += "\"" + column.ColumnName + "\", ";
												}
												if( column.ColumnName.ToUpper().Contains( "DATE" ) )
												{
													valuesClauseString += "TO_DATE('" + ( ( DateTime )row[column] ).ToShortDateString() + "','MM/DD/YYYY'), ";
												}
												else
												{
													valuesClauseString += "'" + row[column].ToString() + "', ";
												}
											}
										}
										valuesClauseString = valuesClauseString.Remove( valuesClauseString.Length - 2 );
										query = query.Remove( query.Length - 2 );

										valuesClauseString += ")";
										query += ")" + valuesClauseString;
										updateCommands.Add( query );
										updatedRows.Add( row );
									}
									break;
								case DataRowState.Deleted:
									query = "DELETE FROM " + dataTable.TableName + " " + BuildWhereClause( row, dataTable.TableName );
									updateCommands.Add( query );
									updatedRows.Add( row );
									break;
								case DataRowState.Modified:
									query = "UPDATE " + dataTable.TableName + " SET ";
									foreach( DataColumn column in row.Table.Columns )
									{
										if( column.ColumnName.ToUpper() != "ID_" && column.ColumnName.ToUpper() != "ROWID" ) //don't want to update primary key columns
										{
											if( column.ColumnName.ToUpper() == dataTable.TableName.ToUpper() )
											{
												query += "DATA_ = ";
											}
											else
											{
												query += "\"" + column.ColumnName + "\" = ";
											}

											if( column.ColumnName.ToUpper().Contains( "DATE" ) )
											{
												query += "TO_DATE('" + ( ( DateTime )row[column] ).ToShortDateString() + "','MM/DD/YYYY'), ";
											}
											else
											{
												query += "'" + row[column].ToString() + "', ";
											}
										}
									}
									query = query.Remove( query.Length - 2 );
									query += " " + BuildWhereClause( row, dataTable.TableName ); //WHERE \"ID_\" = '" + row["ID_", DataRowVersion.Original].ToString() + "'";
									updateCommands.Add( query );
									updatedRows.Add( row );
									break;
							}
						}
						int rowsAffected = -1;
						try
						{
							rowsAffected = DBMgr.ExecuteBatchNonQuery( updateCommands );
							foreach( DataRow row in updatedRows )
							{
								row.AcceptChanges();
							}
						}
						catch( Exception ex )
						{
							throw ex;		//this seems odd, but it's really just to jump over marking the rows as updated incorrectly
						}

						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
				}
				return affectedRows;
			}
		}

		private string BuildWhereClause( DataRow row, string DATAName )
		{
			string clause = "";
			if( row.Table.Columns.Contains( "ID_" ))
			{
				clause = "WHERE ID_ = '" + row["ID_", DataRowVersion.Original] + "'";
			}
			else if( row.Table.Columns.Contains( "ROWID" ) )
			{
				clause = "WHERE ROWID = '" + row["ROWID", DataRowVersion.Original] + "'";
			}
			else
			{
				throw new Exception( "ERROR: no ID field to update/delete on." );
			}

			//This doesn't work for updates because row[column, DataRowVersion.Original] has already been overwritten by the time we get here.
			//simply must have ID_ to do an Update();

			//foreach( DataColumn column in row.Table.Columns )
			//{
			//    switch( column.ColumnName )
			//    {
			//        case "GEOMETRY":
			//            break;
			//        case "DATE_":
			//            clause += "TO_CHAR(DATE_, 'MM/DD/YYYY') = '" + DateTime.Parse( row[column, DataRowVersion.Original].ToString() ).ToString( "MM/dd/yyyy" ) + "' AND ";
			//            break;
			//        default:
			//            if( column.ColumnName.ToUpper() == DATAName.ToUpper() )
			//            {
			//                clause += "\"DATA_\" = '" + row[column, DataRowVersion.Original].ToString() + "' AND ";
			//            }
			//            else
			//            {
			//                string test1 = row[column, DataRowVersion.Original].ToString();
			//                string test2 = row[column, DataRowVersion.Current].ToString();
			//                string test3 = row[column, DataRowVersion.Default].ToString();
			//                //string test4 = row[column, DataRowVersion.Proposed].ToString();
			//                clause += "\"" + column.ColumnName + "\" = '" + row[column, DataRowVersion.Original].ToString() + "' AND ";
			//            }
			//            break;
			//    }
			//}
			//clause = clause.Substring( 0, clause.Length - 5 );
			//if( clause.Length < 6 )
			//{
			//    throw new ArgumentException( "Cannot generate WHERE clause for update/delete." );
			//}
			return clause;
		}

		private bool EmptyRow( DataRow row )
		{
			bool empty = true;
			foreach( DataColumn column in row.Table.Columns )
			{
				if( row[column].ToString() != "" )
				{
					empty = false;
				}
				break;
			}

			return empty;
		}

        #endregion

        public void Dispose()
        {
            if (SDA != null)
            {
                SDA.Dispose();
            }
            else
            {
                ODA.Dispose();
            }
        }
    }
}
