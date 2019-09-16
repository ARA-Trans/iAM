using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace DatabaseManager
{
    /// <summary>
    ///     This class is set up to manage OLE DB database connections and
    ///     provides tools for interacting with the database. There are several
    ///     functions designed specifically for use in SQL, as it was shown
    ///     using the SQL data objects in conjunction with a SQL database, that
    ///     there is a large peformance increase, ~50%. Please use the SQL
    ///     functions if you are connecting to a SQL server database. Otherwise,
    ///     use the OLE DB functions to connect and run queries.
    /// </summary>
    public static class DBMgr
    {
        public enum ConnectionParameterType
        {
            NETWORK,
            ATTRIBUTE,
            ASSET
        };

        private static ConnectionParameters m_cpNativeParameters;

        public static ConnectionParameters NativeConnectionParameters
        {
            get
            {
                return m_cpNativeParameters;
            }
            set
            {
                m_cpNativeParameters = value;
                if (m_cpNativeParameters.IsOleDBConnection)
                {
                    m_cpNativeParameters.OleDbConnection.Open();
                }
                else
                {
                    m_cpNativeParameters.SqlConnection.Open();
                }
            }
        }

        public static int ExecuteParameterizedNonQuery(string sqlStatement, List<DbParameter> statementParameters)
        {
            return ExecuteParameterizedNonQuery(sqlStatement, statementParameters, m_cpNativeParameters);
        }

        public static int ExecuteParameterizedNonQuery(string sqlStatement, List<DbParameter> statementParameters, ConnectionParameters cp)
        {
            int rowsAffected = 0;
            switch (cp.Provider)
            {
                case "MSSQL":
                    SqlCommand command = new SqlCommand(sqlStatement, cp.SqlConnection);
                    foreach (DbParameter commandParam in statementParameters)
                    {
                        command.Parameters.Add((SqlParameter)commandParam);
                    }
                    rowsAffected = command.ExecuteNonQuery();
                    //throw new NotImplementedException( "TODO: Create MSSQL implementation of DBMgr.ExecuteParamerizedQuery()" );
                    break;

                case "ORACLE":
                    rowsAffected = OracleDatabaseManager.ExecuteParameterizedNonQuery(sqlStatement, statementParameters, cp);
                    break;

                default:
                    throw new NotImplementedException("TODO: Create ANSI implementation for DBMgr.ExecuteParameterizedQuery()");
                    //break;
            }

            return rowsAffected;
        }

        /// <summary>
        ///     Retreive the current connection from the Database Manager,
        ///     remember to cast the connection to the appropriate type.
        /// </summary>
        /// <returns>
        ///     Object is either an Ole Db connection or a SQL connection.
        /// </returns>
        public static DbConnection GetNativeConnection()
        {
            DbConnection nativeConn = null;
            if (m_cpNativeParameters == null) return null;
            if (m_cpNativeParameters.IsOleDBConnection)
            {
                if (m_cpNativeParameters.OleDbConnection.State == ConnectionState.Open)
                {
                    nativeConn = m_cpNativeParameters.OleDbConnection;
                }
            }
            else
            {
                if (m_cpNativeParameters.SqlConnection.State == ConnectionState.Open)
                {
                    nativeConn = m_cpNativeParameters.SqlConnection;
                }
            }

            return nativeConn;
        }

        /// <summary>
        ///     This function executes the query on the native server and
        ///     returns the results of the query in a new DataSet object
        /// </summary>
        /// <param name="strQuery">The query to be executed</param>
        /// <returns>DataSet with results of the query.</returns>
        public static DataSet ExecuteQuery(String strQuery)
        {
            return ExecuteQuery(strQuery, m_cpNativeParameters);
        }

        public static int ExecuteScalar(String strQuery)
        {
            return ExecuteScalar(strQuery, m_cpNativeParameters);
        }

        public static int ExecuteScalar(String strQuery, ConnectionParameters cp)
        {
            int scalarValue = 0;
            // Execute DataSet with Sql connection
            if (!cp.IsOleDBConnection)
            {
                try
                {
                    if (!cp.IsNative)
                    {
                        cp.SqlConnection.Open();
                    }
                    // Set the command connection property to the current sql connection
                    SqlCommand command = new SqlCommand(strQuery, cp.SqlConnection);

                    object preConversionScalar = command.ExecuteScalar();
                    if (preConversionScalar != DBNull.Value)
                    {
                        //if( preConversionScalar == Db
                        scalarValue = Convert.ToInt32(preConversionScalar);
                    }
                    else
                    {
                        scalarValue = -1;
                    }

                    //scalarValue = Convert.ToInt32( command.ExecuteScalar() );
                }
                catch (Exception sqlE)
                {
                    System.Diagnostics.Debug.WriteLine(sqlE.Message);
                    throw sqlE;
                }
                finally
                {
                    if (!cp.IsNative)
                    {
                        cp.SqlConnection.Close();
                    }
                }
            }
            else
            {
                try
                {
                    if (!cp.IsNative)
                    {
                        cp.OleDbConnection.Open();
                    }
                    // Set the command connection property to the current sql connection
                    OleDbCommand command = new OleDbCommand(strQuery, cp.OleDbConnection);

                    object preConversionScalar = command.ExecuteScalar();
                    if (preConversionScalar != DBNull.Value)
                    {
                        //if( preConversionScalar == Db
                        scalarValue = Convert.ToInt32(preConversionScalar);
                    }
                    else
                    {
                        scalarValue = -1;
                    }
                }
                catch (OleDbException oleE)
                {
                    System.Diagnostics.Debug.WriteLine(oleE.Message);
                    throw oleE;
                }
                finally
                {
                    if (!cp.IsNative)
                    {
                        cp.OleDbConnection.Close();
                    }
                }
            }

            return scalarValue;
        }

        public static DataSet ExecuteQuery(String strQuery, ConnectionParameters cp)
        {
            DataSet ds = new DataSet();

            // Execute DataSet with Sql connection
            if (!cp.IsOleDBConnection)
            {
                // Set the command connection property to the current sql connection
                if (!cp.IsNative)
                {
                    try
                    {
                        cp.SqlConnection.Open();
                        SqlCommand command = new SqlCommand(strQuery, cp.SqlConnection);
                        command.CommandTimeout = 300000;
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(ds);
                    }
                    catch (Exception sqlE)
                    {
                        System.Diagnostics.Debug.WriteLine(sqlE.Message);
                        throw sqlE;
                    }
                    finally
                    {
                        cp.SqlConnection.Close();
                    }
                }
                else//isNative
                {
                    using (SqlConnection connection = new SqlConnection(cp.ConnectionString))
                    {
                        try
                        {
                            connection.Open();
                            SqlCommand command = new SqlCommand(strQuery, connection);
                            command.CommandTimeout = 300000;
                            SqlDataAdapter adapter = new SqlDataAdapter(command);
                            adapter.Fill(ds);
                        }
                        catch (Exception sqlE)
                        {
                            //System.Diagnostics.Debug.WriteLine(sqlE.Message);
                            throw sqlE;
                        }
                    }
                }
            }
            else
            {
                using (OleDbDataAdapter queryAdapter = new OleDbDataAdapter(strQuery, cp.ConnectionString))
                {
                    queryAdapter.Fill(ds);
                }
            }

            return ds;
        }

        /// <summary>
        ///     Used to execute non-native queries
        /// </summary>
        /// <param name="strSelect">Query string to run</param>
        /// <returns></returns>
        public static ConnectionParameters GetAttributeConnectionObject(String strAttr)
        {
            ConnectionParameters cp;

            // Get the connection string information from the database
            /// TODO: Change to pull connection information from the connection parameters table in the database.
            strAttr = strAttr.Trim();
            String strQuery = "SELECT SERVER, DATASOURCE, USERID, PASSWORD_, PROVIDER, NATIVE_, SID_, SERVICE_NAME, INTEGRATED_SECURITY, PORT FROM ATTRIBUTES_ WHERE ATTRIBUTE_ = '" + strAttr + "'";
            DataSet ds = new DataSet();
            try
            {
                ds = ExecuteQuery(strQuery);
                DataRow dr = ds.Tables[0].Rows[0];
                bool bNative = Convert.ToBoolean(dr["NATIVE_"]);

                if (bNative)
                {
                    cp = m_cpNativeParameters;
                }
                else
                {
                    string server = dr["SERVER"].ToString();
                    string database = dr["DATASOURCE"].ToString();
                    string userID = dr["USERID"].ToString();
                    string password = dr["PASSWORD_"].ToString();
                    string provider = dr["PROVIDER"].ToString();
                    string SID = dr["SID_"].ToString();
                    string networkAlias = dr["SERVICE_NAME"].ToString();
                    bool bIntegratedSecurity = false;
                    if (String.IsNullOrEmpty(userID) || String.IsNullOrEmpty(password))
                    {
                        bIntegratedSecurity = true;
                    }
                    //bool bIntegratedSecurity = Convert.ToBoolean(dr["INTEGRATED_SECURITY"].ToString());
                    string port = dr["PORT"].ToString();
                    cp = new ConnectionParameters(port, SID, networkAlias, userID, password, bIntegratedSecurity, server, database, "", "", "", "", provider, bNative);
                }
            }
            catch
            {
                return null;
            }
            return cp;
        }

        public static ConnectionParameters GetAssetConnectionObject(String asset)
        {
            ConnectionParameters cp = null;
            // Get the connection string information from the database
            bool bNative = true;
            String strQuery = "SELECT SERVER, DATASOURCE, USERID, PASSWORD_, PROVIDER, NATIVE_ FROM ASSETS WHERE ASSET = '" + asset + "'";
            DataSet ds = new DataSet();
            try
            {
                ds = ExecuteQuery(strQuery);
            }
            catch (Exception e)
            {
                throw e;
            }

            if (ds.Tables[0].Rows[0].ItemArray[5] != null && ds.Tables[0].Rows[0].ItemArray[5].ToString() != "")
            {
                bNative = Convert.ToBoolean(ds.Tables[0].Rows[0].ItemArray[5]);
            }

            String strServer = "";
            String strDatabase = "";
            String strUserName = "";
            String strPassword = "";
            String strProvider = "";
            if (!bNative)
            {
                strServer = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                strDatabase = ds.Tables[0].Rows[0].ItemArray[1].ToString();

                if (ds.Tables[0].Rows[0].ItemArray[2] != null)
                {
                    strUserName = ds.Tables[0].Rows[0].ItemArray[2].ToString();
                }
                if (ds.Tables[0].Rows[0].ItemArray[3] != null)
                {
                    strPassword = ds.Tables[0].Rows[0].ItemArray[3].ToString();
                }
                strProvider = ds.Tables[0].Rows[0].ItemArray[4].ToString();
                //cp = new ConnectionParameters(strServer, strDatabase, strUserName, strPassword, strProvider, bNative);
                cp = null;
            }
            else
            {
                cp = m_cpNativeParameters;
            }
            return cp;
        }

        /// <summary>
        ///     Only executes a list of non-queries. Uses a transaction to
        ///     commit changes after all has succeeded.
        /// </summary>
        /// <param name="listCommandText"></param>
        public static int ExecuteBatchNonQuery(List<String> listCommandText)
        {
            return ExecuteBatchNonQuery(listCommandText, m_cpNativeParameters);
        }

        public static int ExecuteBatchNonQuery(List<String> listCommandText, ConnectionParameters cp)
        {
            int affectedRows = 0;
            if (!cp.IsOleDBConnection)
            {
                SqlTransaction transaction = null;
                try
                {
                    if (!cp.IsNative)
                    {
                        cp.SqlConnection.Open();
                    }
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cp.SqlConnection;
                    transaction = cp.SqlConnection.BeginTransaction();
                    cmd.Transaction = transaction;
                    foreach (String str in listCommandText)
                    {
                        cmd.CommandText = str;
                        affectedRows += cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                    throw exc;
                }
                finally
                {
                    if (!cp.IsNative)
                    {
                        cp.SqlConnection.Close();
                    }
                }
            }
            else
            {
                OleDbTransaction transaction = null;
                string lastCommand = "";
                try
                {
                    if (!cp.IsNative)
                    {
                        cp.OleDbConnection.Open();
                    }
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = cp.OleDbConnection;
                    transaction = cp.OleDbConnection.BeginTransaction();
                    cmd.Transaction = transaction;
                    foreach (String str in listCommandText)
                    {
                        lastCommand = str;
                        cmd.CommandText = str;
                        affectedRows += cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                    throw exc;
                }
                finally
                {
                    if (!cp.IsNative)
                    {
                        //dsmelser
                        //someone got quick with the copy and paste ;]
                        //cp.OleDbConnection.Open();
                        cp.OleDbConnection.Close();
                    }
                }
            }
            return affectedRows;
        }

        /// <summary>
        ///     Executes SQL non query statements. Examples include UPDATE,
        ///     INSERT, and DELETE statements Functions on RoadCare server
        ///     connection object.
        /// </summary>
        /// <param name="strNonQuery">
        ///     The statement to execute on the database.
        /// </param>
        public static int ExecuteNonQuery(String strNonQuery)
        {
            return ExecuteNonQuery(strNonQuery, m_cpNativeParameters);
        }

        /// <summary>
        ///     Executes SQL non query statements. Examples include UPDATE,
        ///     INSERT, and DELETE statements Functions on RoadCare server
        ///     connection object.
        /// </summary>
        /// <param name="strNonQuery">
        ///     The statement to execute on the database.
        /// </param>
        public static int ExecuteNonQuery(String strNonQuery, ConnectionParameters cp)
        {
            int iRows = 0;

            if (!cp.IsOleDBConnection)
            {
                try
                {
                    if (!cp.IsNative)
                    {
                        cp.SqlConnection.Open();
                    }
                    SqlCommand cmd = new SqlCommand(strNonQuery, cp.SqlConnection);
                    cmd.CommandTimeout = 2000;
                    iRows = cmd.ExecuteNonQuery();
                }
                catch (Exception sqlE)
                {
                    System.Diagnostics.Debug.WriteLine(sqlE.Message);
                    throw sqlE;
                }
                finally
                {
                    if (!cp.IsNative)
                    {
                        cp.SqlConnection.Close();
                    }
                }
            }
            else
            {
                OleDbConnection commandConnection = null;
                try
                {
                    commandConnection = new OleDbConnection(cp.ConnectionString);
                    commandConnection.Open();
                    OleDbCommand cmd = new OleDbCommand(strNonQuery, commandConnection);
                    iRows = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    throw new ArgumentException("Error executing non-query [" + strNonQuery + "]: " + ex.Message);
                }
                finally
                {
                    commandConnection.Close();
                }
            }
            return iRows;
        }

        public static List<String> GetTableColumns(String strTableName)
        {
            return GetTableColumns(strTableName, m_cpNativeParameters);
        }

        /// <summary>
        ///     Gets a list of the columns in a given table.
        /// </summary>
        /// <param name="strTableName">
        ///     Name of the table to retrieve column names for.
        /// </param>
        /// <returns></returns>
        public static List<String> GetTableColumns(String strTableName, ConnectionParameters cp)
        {
            // Set up the query and execute it
            List<String> strListColumnNames = new List<String>();
            String strQuery;
            switch (cp.Provider)
            {
                case "MSSQL":
                    strQuery = "Select COLUMN_NAME From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME = '" + strTableName + "'";
                    break;

                case "ORACLE":
                    //dsmelser
                    //this is wrong
                    //strQuery = "SELECT column_name FROM all_tab_cols where table_name = '" + strTableName + "'";
                    strQuery = "SELECT column_name FROM user_tab_cols where table_name = '" + strTableName.ToUpper() + "' ORDER BY COLUMN_ID ASC";
                    break;

                default:
                    strQuery = "Select COLUMN_NAME From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME = '" + strTableName + "'";
                    break;
            }
            DataSet dsColumnNames;
            dsColumnNames = (DataSet)ExecuteQuery(strQuery, cp);

            // Add the results to the list of column names
            foreach (DataRow dr in dsColumnNames.Tables[0].Rows)
            {
                strListColumnNames.Add(dr[0].ToString());
            }
            return strListColumnNames;
        }

        /// <summary>
        ///     Returns a dataset of column names and table information for the
        ///     table strTableName.
        /// </summary>
        /// <param name="strTableName">
        ///     Name of the table to retrieve column names for.
        /// </param>
        /// <returns>outs a DataSet dsColumnNames</returns>
        public static void GetTableColumns(String strTableName, out DataSet dsColumnNames)
        {
            // Set up the query and execute it
            String strQuery = "Select COLUMN_NAME From INFORMATION_SCHEMA.COLUMNS Where TABLE_NAME = '" + strTableName + "'";
            try
            {
                dsColumnNames = ExecuteQuery(strQuery);
            }
            catch (OleDbException oleE)
            {
                System.Diagnostics.Debug.WriteLine(oleE.Message);
                throw oleE;
            }
            return;
        }

        public static DataSet GetTableColumnsWithTypes(String tableName)
        {
            return GetTableColumnsWithTypes(tableName, m_cpNativeParameters);
        }

        public static string IsColumnTypeString(string tableName, string columnName)
        {
            return IsColumnTypeString(tableName, columnName, NativeConnectionParameters);
        }

        public static string IsColumnTypeString(string tableName, string columnName, ConnectionParameters cp)
        {
            string systemType = "";
            switch (cp.Provider)
            {
                case "MSSQL":
                    string query = "SELECT TOP 1 " + columnName + " FROM " + tableName;
                    systemType = DBMgr.ExecuteQuery(query, cp).Tables[0].Columns[columnName].DataType.Name;
                    break;

                case "ORACLE":
                    throw new NotImplementedException();
                    //break;
            }
            return systemType;
        }

        public static string GetTableColumnType(string tableName, string columnName)
        {
            return GetTableColumnType(tableName, columnName, NativeConnectionParameters);
        }

        public static string GetTableColumnType(string tableName, string columnName, ConnectionParameters cp)
        {
            string type;
            try
            {
                String query;
                switch (cp.Provider)
                {
                    case "MSSQL":
                        query = "SELECT column_name, data_type, Character_maximum_length FROM information_schema.columns WHERE table_name = '" + tableName + "' AND column_name = '" + columnName + "'";
                        break;

                    case "ORACLE":
                        query = "SELECT column_name, data_type, char_col_decl_length AS Character_maximum_length FROM USER_TAB_COLUMNS WHERE table_name = '" + tableName + "' AND column_name = '" + columnName + "'"; ;
                        break;

                    default:
                        throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                        //break;
                }
                type = DBMgr.ExecuteQuery(query, cp).Tables[0].Rows[0][0].ToString();
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return type;
        }

        /// <summary>
        ///     Returns the columns and data types for the given table as
        ///     column_name and data_type in the data set. for char fields, the
        ///     number of characters is given as Character_maximum_length
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataSet GetTableColumnsWithTypes(String tableName, ConnectionParameters cp)
        {
            DataSet columnNamesAndTypes;
            try
            {
                String query;
                switch (cp.Provider)
                {
                    case "MSSQL":
                        query = "SELECT column_name, data_type, Character_maximum_length FROM information_schema.columns WHERE table_name = '" + tableName + "'";
                        break;

                    case "ORACLE":
                        query = "SELECT column_name, data_type, char_col_decl_length AS Character_maximum_length FROM USER_TAB_COLUMNS WHERE table_name = '" + tableName + "'";
                        break;

                    default:
                        throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                        //break;
                }
                columnNamesAndTypes = DBMgr.ExecuteQuery(query, cp);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return columnNamesAndTypes;
        }

        public static void CreateTable(string tableName, List<TableParameters> columns)
        {
            CreateTable(tableName, columns, true);
        }

        /// <summary>
        ///     Creates a table on the existing database connection
        /// </summary>
        /// <param name="strTableName">Table name to create</param>
        /// <param name="strListColumnNames">
        ///     List of column names for the new table
        /// </param>
        public static void CreateTable(String strTableName, List<TableParameters> strListColumnNames, bool makeAutoIncrement)
        {
            String strColumnName;
            DataType columnDataType;
            bool bIsNullable;
            if (m_cpNativeParameters.IsOleDBConnection)
            {
                switch (m_cpNativeParameters.Provider)
                {
                    case "MSSQL":
                        String strColAndType = "";
                        foreach (TableParameters tableParam in strListColumnNames)
                        {
                            strColumnName = tableParam.GetColumnName().ToUpper();
                            columnDataType = tableParam.GetColumnDataType();
                            bIsNullable = tableParam.GetIsNullable();
                            strColAndType += strColumnName + " " + columnDataType.ToString() + ",";
                            if (tableParam.GetIsPrimaryKey() == true)
                            {
                                strColAndType += " IDENTITY NOT NULL PRIMARY KEY,";
                            }
                        }

                        // Hack off that extra comma, and add a parantheses
                        strColAndType = strColAndType.Substring(0, strColAndType.Length - 1);

                        OleDbCommand cmd = new OleDbCommand("CREATE TABLE " + strTableName + " (" + strColAndType + ")", m_cpNativeParameters.OleDbConnection);
                        cmd.ExecuteNonQuery();
                        break;

                    case "ORACLE":
                        String strCreateStatement = "CREATE TABLE " + strTableName + " (";
                        String strParameterDefinition = "";
                        List<TableParameters> primaryKeys = new List<TableParameters>();
                        foreach (TableParameters tableParam in strListColumnNames)
                        {
                            strParameterDefinition = "\"" + tableParam.GetColumnName().ToUpper() + "\" ";
                            string columnType = tableParam.GetColumnDataType().ToString().ToLower();
                            switch (columnType)
                            {
                                case "varchar":
                                    if (tableParam.GetIsCLOB() == 1)
                                    {
                                        if (tableParam.GetColumnName().ToUpper() != "GEOMETRY")
                                        {
                                            //throw new ArgumentException( "Attempted to create unnecessary CLOB column [" + tableParam.GetColumnName() + "] on table [" + strTableName + "]." );
                                        }
                                        else
                                        {
                                            strParameterDefinition += "CLOB";
                                        }
                                    }
                                    else
                                    {
                                        if (tableParam.GetColumnDataType().MaximumLength == -1)
                                        {
                                            if (tableParam.GetColumnName().ToUpper() != "GEOMETRY")
                                            {
                                                //throw new ArgumentException( "Attempted to create unnecessary CLOB column [" + tableParam.GetColumnName() + "] on table [" + strTableName + "]." );
                                                strParameterDefinition += "VARCHAR2(";
                                                strParameterDefinition += "4000";//tableParam.GetColumnDataType().MaximumLength;
                                                strParameterDefinition += ")";
                                            }
                                            else
                                            {
                                                strParameterDefinition += "CLOB";
                                            }
                                        }
                                        else
                                        {
                                            strParameterDefinition += "VARCHAR2(";
                                            strParameterDefinition += tableParam.GetColumnDataType().MaximumLength;
                                            strParameterDefinition += ")";
                                        }
                                    }
                                    break;

                                case "datetime":
                                    strParameterDefinition += "DATE";
                                    break;

                                case "bit":
                                    strParameterDefinition += "SMALLINT";
                                    break;

                                default:
                                    strParameterDefinition += tableParam.GetColumnDataType();
                                    //strParameterDefinition += tableParam.GetColumnDataType().MaximumLength;
                                    //strParameterDefinition += "VARCHAR2(";
                                    //strParameterDefinition += "4000";
                                    //strParameterDefinition += ")";
                                    break;
                            }
                            if (!tableParam.GetIsNullable())
                            {
                                strParameterDefinition += " NOT NULL";
                            }
                            strParameterDefinition += ", ";

                            if (tableParam.GetIsPrimaryKey())
                            {
                                primaryKeys.Add(tableParam);
                            }

                            strCreateStatement += strParameterDefinition;
                        }

                        if (primaryKeys.Count > 0)
                        {
                            String strKeyStatement = "CONSTRAINT " + strTableName + "_PK PRIMARY KEY (";
                            foreach (TableParameters key in primaryKeys)
                            {
                                strKeyStatement += key.GetColumnName() + ", ";
                            }
                            strKeyStatement = strKeyStatement.Remove(strKeyStatement.Length - 2);
                            strCreateStatement += strKeyStatement + "))";
                            ExecuteNonQuery(strCreateStatement);

                            //If there is only one primary key column, we need to implement the auto-increment trigger and the sequence it depends upon
                            if (primaryKeys.Count == 1)
                            {
                                string sequenceName = CreateOracleAutoIncrementSequence(strTableName, primaryKeys[0].GetColumnName());
                                CreateOracleAutoIncrementTrigger(strTableName, primaryKeys[0].GetColumnName(), sequenceName);
                            }
                        }
                        else
                        {
                            strCreateStatement = strCreateStatement.Remove(strCreateStatement.Length - 2);
                            strCreateStatement += ")";
                            ExecuteNonQuery(strCreateStatement);
                        }
                        break;

                    default:
                        throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                }
            }
            else
            {
                using (
                    SqlConnection connection =
                        new SqlConnection(DBMgr.NativeConnectionParameters.ConnectionString))
                {
                    string sql = "CREATE TABLE dbo." + strTableName + "(";

                    foreach (TableParameters tableParam in strListColumnNames)
                    {
                        if (sql.LastIndexOf('(') + 1 != sql.Length) sql += ",";
                        sql += tableParam.GetColumnName() + " " +
                               tableParam.GetColumnDataType().ToString();

                        if (tableParam.GetColumnDataType().MaximumLength > 0)
                        {
                            sql += " (" +
                                   tableParam.GetColumnDataType().MaximumLength.ToString() +
                                   ")";
                        }
                        if(tableParam.GetColumnDataType().SqlDataType == SqlDataType.VarCharMax)
                        {
                            sql += " (" +
                                   "MAX" +
                                   ")";
                        }
                        if (tableParam.GetIsIdentity()) sql += " IDENTITY(1,1) ";
                        if (!tableParam.GetIsNullable()) sql += " not null";
                    }

                    sql += ")";
                    try
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand(sql, connection);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.InnerException.Message);
                        throw e;
                    }
                }
                //Server server = new Server(new ServerConnection((SqlConnection)GetNativeConnection()));

                //// Create the new database table with the query information and the WIM device ID on the remote machine...
                //// Create table in my personal database
                //Database db = server.Databases[m_cpNativeParameters.Database];

                //// Create new table, with the name of the Query title
                //Table newTable = new Table(db, strTableName);
                //Column columnAddMe;
                //String strPrimaryKey = "";
                //foreach (TableParameters tableParam in strListColumnNames)
                //{
                //	strColumnName = tableParam.GetColumnName();
                //	columnDataType = tableParam.GetColumnDataType();
                //	bIsNullable = tableParam.GetIsNullable();

                //	columnAddMe = new Column(newTable, strColumnName);
                //	columnAddMe.DataType = columnDataType;
                //	columnAddMe.Nullable = bIsNullable;
                //	if (tableParam.GetIsPrimaryKey() == true)
                //	{
                //		columnAddMe.Nullable = false;
                //		strPrimaryKey = strColumnName;
                //		if (tableParam.GetIsIdentity())
                //		{
                //			columnAddMe.Identity = true;
                //			columnAddMe.IdentityIncrement = 1;
                //			columnAddMe.IdentitySeed = 1;
                //		}
                //	}
                //	if (!newTable.Columns.Contains(columnAddMe.Name))
                //	{
                //		newTable.Columns.Add(columnAddMe);
                //	}
                //}

                //// Physically create the table in the database
                //try
                //{
                //	newTable.Create();
                //}
                //catch (Exception foE)
                //{
                //	MessageBox.Show(foE.InnerException.Message);
                //	throw foE;
                //}

                //if (strPrimaryKey != "")
                //{
                //	Index idx = new Index(newTable, "PKI_" + strTableName);
                //	idx.IndexedColumns.Add(new IndexedColumn(idx, strPrimaryKey));
                //	idx.IsClustered = true;
                //	idx.IsUnique = true;
                //	idx.IndexKeyType = IndexKeyType.DriPrimaryKey;
                //	idx.Create();
                //}
            }
        }

        private static string CreateOracleAutoIncrementSequence(string tableName, string primaryKey)
        {
            string sequenceName = tableName.ToUpper() + "_" + primaryKey.ToUpper() + "_SEQ";
            if (sequenceName.Length > 30)
            {
                sequenceName = sequenceName.Substring(0, 30);
            }
            if (OracleSequenceExists(sequenceName))
            {
                DropOracleSequence(sequenceName);
            }
            string sequenceNonquery = "CREATE SEQUENCE " + sequenceName + " START WITH 1 INCREMENT BY 1 CACHE 100 NOMAXVALUE NOCYCLE ORDER";
            ExecuteNonQuery(sequenceNonquery);
            return sequenceName;
        }

        private static bool OracleSequenceExists(string sequenceName)
        {
            return DBMgr.ExecuteScalar("SELECT COUNT(*) FROM user_sequences WHERE SEQUENCE_NAME = '" + sequenceName + "'") == 1;
        }

        private static void DropOracleSequence(string sequenceName)
        {
            DBMgr.ExecuteNonQuery("DROP SEQUENCE " + sequenceName);
        }

        private static void CreateOracleAutoIncrementTrigger(string tableName, string primaryKey, string sequenceName)
        {
            string triggerName = tableName.ToUpper() + "_" + primaryKey.ToUpper() + "_TRG";
            if (triggerName.Length > 30)
            {
                triggerName = triggerName.Substring(0, 30);
            }
            string triggerNonquery = "CREATE OR REPLACE TRIGGER " + triggerName + " BEFORE INSERT ON "
                + tableName + " FOR EACH ROW DECLARE v_newVal NUMBER(12) := 0; v_incval NUMBER(12) := 0; v_rowCount NUMBER(12) := 0; BEGIN IF INSERTING AND :new." +
                    primaryKey + " IS NULL THEN SELECT  " + sequenceName +
                    ".NEXTVAL INTO v_newVal FROM DUAL; IF v_newVal = 1 THEN SELECT COUNT(*) INTO v_rowCount FROM " + tableName + "; " +
                    "IF v_rowCount > 0 THEN SELECT max(" + primaryKey + ") INTO v_newVal FROM " + tableName +
                    "; v_newVal := v_newVal + 1; LOOP EXIT WHEN v_incval>=v_newVal; SELECT " +
                    sequenceName + ".nextval INTO v_incval FROM dual; END LOOP; END IF; END IF; sqlserver_utilities.identity := v_newVal; :new." +
                    primaryKey + " := v_newVal;  END IF; END;";
            ExecuteNonQuery(triggerNonquery);
        }

        /// <summary>
        ///     Closes an existing data connection based on the class's
        ///     instantiated connection
        /// </summary>
        public static void CloseConnection()
        {
            if (m_cpNativeParameters.IsOleDBConnection)
            {
                if (m_cpNativeParameters.OleDbConnection.State == ConnectionState.Open)
                {
                    m_cpNativeParameters.OleDbConnection.Close();
                }
            }
            else
            {
                if (m_cpNativeParameters.SqlConnection.State == ConnectionState.Open)
                {
                    m_cpNativeParameters.SqlConnection.Close();
                }
            }
        }

        ///// <summary>
        ///// This generates a BCP string and executes it filling a SQL data table with the file information.
        ///// </summary>
        ///// <param name="strTableName">Name of table to perform inserts on.</param>
        ///// <param name="strFileName">Name of file from which information is put into SQL database table.</param>
        //public static void SQLBulkLoad(String strTableName, String strPathName)
        //{
        //    String strDB;
        //    String strDS;
        //    String strBCP;

        //    if ( m_cpNativeParameters.SqlConnection.State == ConnectionState.Open)
        //    {
        //        strDB = m_cpNativeParameters.SqlConnection.Database;
        //        strDS = m_cpNativeParameters.SqlConnection.DataSource;

        //        if (m_cpNativeParameters.IsIntegratedSecurity)
        //        {
        //            strBCP = "\"" + strDB + ".dbo." + strTableName + "\" IN \"" + strPathName + "\" /S " + strDS + " /T -t \",\" -c -q";

        //        }
        //        else
        //        {
        //            strBCP = "\"" + strDB + ".dbo." + strTableName + "\" IN \"" + strPathName + "\" /S " + strDS + " /T -t \",\" -c -q -U " + m_cpNativeParameters.UserName + " -P " + m_cpNativeParameters.Password;

        //        }
        //        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        //        proc.EnableRaisingEvents = false;
        //        proc.StartInfo.FileName = "bcp";
        //        proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        //        proc.StartInfo.Arguments = strBCP;
        //        proc.Start();
        //        proc.WaitForExit();
        //    }
        //}

        public static void SQLBulkLoad(String strTableName, String strPathName, char cDelimeter)
        {
            String strDB;
            String strDS;
            String strBCP;

            if (m_cpNativeParameters.SqlConnection.State == ConnectionState.Open)
            {
                strDB = m_cpNativeParameters.SqlConnection.Database.ToString();
                strDS = m_cpNativeParameters.SqlConnection.DataSource.ToString();
                if (m_cpNativeParameters.IsIntegratedSecurity)
                {
                    strBCP = "\"" + strDB + ".dbo." + strTableName + "\" IN \"" + strPathName + "\" /S " + strDS + " /T -t \"" + cDelimeter + "\" -c -q";
                }
                else
                {
                    strBCP = "\"" + strDB + ".dbo." + strTableName + "\" IN \"" + strPathName + "\" /S " + strDS + " -t\"" + cDelimeter + "\" -c -q -U " + m_cpNativeParameters.UserName + " -P " + m_cpNativeParameters.Password; ;
                }
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = "bcp";
                proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                proc.StartInfo.Arguments = strBCP;
                proc.Start();
                proc.WaitForExit();
            }
        }

        public static void SQLBulkLoad(String strTableName, String strPathName, string cDelimeter)
        {
            String strDB;
            String strDS;
            String strBCP;

            String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strMyDocumentsFolder += "\\RoadCare Projects\\Temp\\";

            if (m_cpNativeParameters.SqlConnection.State == ConnectionState.Open)
            {
                strDB = m_cpNativeParameters.SqlConnection.Database.ToString();
                strDS = m_cpNativeParameters.SqlConnection.DataSource.ToString();

                if (m_cpNativeParameters.IsIntegratedSecurity)
                {
                    strBCP = "\"" + strDB + ".dbo." + strTableName + "\" IN \"" + strPathName + "\" /S " + strDS + " /T -t \"" + cDelimeter + "\" -c -q -o \"" + strMyDocumentsFolder + "output.txt\"";
                }
                else
                {
                    strBCP = "\"" + strDB + ".dbo." + strTableName + "\" IN \"" + strPathName + "\" /S " + strDS + " -t\"" + cDelimeter + "\" -c -q -U " + m_cpNativeParameters.UserName + " -P " + m_cpNativeParameters.Password; ;
                }

                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = "bcp";
                proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                proc.StartInfo.Arguments = strBCP;
                proc.Start();
                proc.WaitForExit();
            }
        }

        /// <summary>
        ///     This generates a BCP string and executes it filling a SQL data
        ///     table with the file information. This overload is for non native
        ///     connections only.
        /// </summary>
        /// <param name="strTableName">
        ///     Name of table to perform inserts on.
        /// </param>
        /// <param name="strFileName">
        ///     Name of file from which information is put into SQL database
        ///     table.
        /// </param>
        public static void SQLBulkLoad(String strTableName, String strPathName, ConnectionParameters cp)
        {
            String strDB;
            String strDS;
            String strBCP;

            strDB = cp.Database;
            strDS = cp.Server;

            strBCP = "\"" + strDB + ".dbo." + strTableName + "\" IN \"" + strPathName + "\" /S " + strDS + " /T -t \",\" -c -q";

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = "bcp";
            proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            proc.StartInfo.Arguments = strBCP;
            proc.Start();
            proc.WaitForExit();
        }

        public static bool OracleBulkLoad(ConnectionParameters oracleCP, string tableName, string dataFilePath, List<string> orderedColumnNames, string seperator)
        {
            return OracleBulkLoad(oracleCP, tableName, dataFilePath, orderedColumnNames, seperator, "");
        }

        public static bool OracleBulkLoad(ConnectionParameters oracleCP, string tableName, string dataFilePath, List<string> orderedColumnNames, string fieldSeperator, string lineSeperator)
        {
            // Need to find the correct arguments to pass to the bulk loader if network alias is being used.

            bool successfulLoad = true;
            string preExtensionFileName = dataFilePath.Remove(dataFilePath.LastIndexOf('.'));
            string controlFileName = preExtensionFileName + ".ctl";
            string logFileName = preExtensionFileName + ".log";

            TextWriter controlWriter = new StreamWriter(controlFileName);

            controlWriter.WriteLine("load data");
            controlWriter.WriteLine("infile '" + dataFilePath + "'" + lineSeperator);
            controlWriter.WriteLine("append");
            controlWriter.WriteLine("into table " + tableName);
            controlWriter.WriteLine("fields terminated by '" + fieldSeperator + "'");
            controlWriter.WriteLine("trailing nullcols");

            string columnSpecifierLine = "(";
            foreach (string columnName in orderedColumnNames)
            {
                if (columnName.ToUpper().Contains("DATE"))
                {
                    columnSpecifierLine += columnName + " char (23), ";
                }
                else
                {
                    string query = "SELECT DATA_TYPE, DATA_LENGTH FROM USER_TAB_COLS WHERE TABLE_NAME = '" + tableName.ToUpper() + "' AND COLUMN_NAME = '" + columnName.ToUpper() + "'";
                    DataSet columnSet = DBMgr.ExecuteQuery(query);

                    if (columnSet.Tables[0].Rows.Count > 0)
                    {
                        DataRow columnInfo = columnSet.Tables[0].Rows[0];
                        switch (columnInfo["DATA_TYPE"].ToString())
                        {
                            //columnSpecifierLine += columnName + " char (8000), ";
                            case "CLOB":
                                if (columnName.ToUpper() == "GEOMETRY")
                                {
                                    columnSpecifierLine += columnName + " char (24000), ";
                                }
                                else
                                {
                                    columnSpecifierLine += columnName + " char (4000), ";
                                }
                                break;

                            default:
                                columnSpecifierLine += columnName + " char (" + columnInfo["DATA_LENGTH"].ToString() + "), ";
                                break;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("ERROR: Could not get data for column " + columnName + " in table " + tableName);
                    }
                }
            }
            columnSpecifierLine = columnSpecifierLine.Remove(columnSpecifierLine.Length - 2) + ")";

            controlWriter.WriteLine(columnSpecifierLine);
            controlWriter.Close();

            string arguments;

            try
            {
                if (oracleCP.OleDbConnection.State != ConnectionState.Open)
                {
                    oracleCP.OleDbConnection.Open();
                }

                //arguments = "userid=" + oracleCP.UserName + "/" + oracleCP.Password + "@" + oracleCP.OracleNameEntry + " control='" + controlFileName + "' log='" + logFileName + "'";
                arguments = "";
                if (oracleCP.SID != "")
                {
                    arguments = "userid=" + oracleCP.UserName
                        + "/" + oracleCP.Password
                        + @"@\""" + oracleCP.SID
                        //we're setting up userid=username/password@\"location\" ...
                        //on the command line
                        //that means we have to use the @ to force a verbatim literal
                        //and then escape the quote or else the line looks like
                        //+ "@\\\"" which seems even more cryptic to me...
                        + @"\"" control=\""" + controlFileName
                        + @"\"" log=\""" + logFileName + @"\"""
                        + @" rows=100000"
                        ;
                }

                // Always use the network alias if possible.
                if (oracleCP.NetworkAlias != "")
                {
                    arguments = "userid=" + oracleCP.UserName
                        + "/" + oracleCP.Password
                        + @"@\""" + oracleCP.NetworkAlias
                        //we're setting up userid=username/password@\"location\" ...
                        //on the command line
                        //that means we have to use the @ to force a verbatim literal
                        //and then escape the quote or else the line looks like
                        //+ "@\\\"" which seems even more cryptic to me...
                        + @"\"" control=\""" + controlFileName
                        + @"\"" log=\""" + logFileName + @"\"""
                        + @" rows=100000"
                        ;
                }

                //ProcessStartInfo loaderStartInfo = new ProcessStartInfo( "sqlldr", arguments );
                //loaderStartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                //Process loaderProc = Process.Start( loaderStartInfo );
                //loaderProc.PriorityClass = ProcessPriorityClass.High;

                //loaderProc.EnableRaisingEvents = false;
                //loaderProc.WaitForExit();

                //if( loaderProc.ExitCode > 0 )
                //{
                //    if( loaderProc.ExitCode > 1 )
                //    {
                //        throw new Exception( "Oracle Bulk Loader gave an error: " + dataFilePath + " into Oracle table " + tableName + '\n' + "sqlldr " + arguments );
                //    }
                //    else
                //    {
                //        throw new Exception( "Oracle Bulk Loader gave a warning: " + dataFilePath + " into Oracle table " + tableName + '\n' + "sqlldr " + arguments );
                //    }
                //}

                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.EnableRaisingEvents = false;

                proc.StartInfo.FileName = "sqlldr";
#if DEBUG
                //                proc.StartInfo.FileName = @"D:\app\glarson-admin\product\11.2.0\client_1\BIN\sqlldr";
#endif
                proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                proc.StartInfo.Arguments = arguments;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                successfulLoad = false;
                throw ex;
            }

            return successfulLoad;
        }

        /// <summary>
        ///     Creates a SQL data reader on a new connection to the existing
        ///     database (using the connection string provided in the DBManager
        ///     contructor.)
        /// </summary>
        /// <param name="strQuery">The query to execute on the database.</param>
        /// <exception cref="Exception"></exception>
        /// <returns>
        ///     The data reader to pull information from the database with.
        /// </returns>
        public static DataTable CreateDataReader(String strQuery)
        {
            return CreateDataReader(strQuery, m_cpNativeParameters);
        }

        public static DataTable CreateDataReader(String strQuery, ConnectionParameters cp)
        {
            SqlDataReader dtR = null;
            DataTable simulationResults = new DataTable();
            if (!cp.IsOleDBConnection)
            {
                SqlConnection connSqlReader = new SqlConnection();
                connSqlReader.ConnectionString = cp.ConnectionString;

                try
                {
                    connSqlReader.Open();

                    SqlCommand command = new SqlCommand(strQuery, connSqlReader);
                    command.CommandTimeout = 500000;

                    try
                    {
                        dtR = command.ExecuteReader(CommandBehavior.Default);
                        simulationResults.Load(dtR);
                        connSqlReader.Close();
                    }
                    catch (Exception sqlE)
                    {
                        System.Diagnostics.Debug.WriteLine(sqlE.Message);
                        throw sqlE;
                    }
                }
                catch (Exception sqlE)
                {
                    throw sqlE;
                }
            }
            else
            {
                throw new Exception("Attempted to get MSSQL datareader for OleDB connection.");
            }

            return simulationResults;
        }

        public static SqlDataReader CreateDataReader(String strQuery, String connString)
        {
            SqlConnection connSqlReader = new SqlConnection();
            connSqlReader.ConnectionString = connString;
            SqlDataReader dtR = null;
            try
            {
                connSqlReader.Open();

                SqlCommand command = new SqlCommand(strQuery, connSqlReader);

                try
                {
                    dtR = command.ExecuteReader(CommandBehavior.Default);
                }
                catch (Exception sqlE)
                {
                    System.Diagnostics.Debug.WriteLine(sqlE.Message);
                    throw sqlE;
                }
            }
            catch (Exception sqlE)
            {
                throw sqlE;
            }
            return dtR;
        }

        /// <summary>
        ///     Creates an OleDb data reader on a new connection with the passed
        ///     connection string attributes
        /// </summary>
        /// <param name="strQuery"></param>
        /// <param name="strProvider"></param>
        /// <param name="strServer"></param>
        /// <param name="strDataSource"></param>
        /// <param name="strPassword"></param>
        /// <param name="strUserName"></param>
        /// <returns>
        ///     A data reader to read information from the database.
        /// </returns>
        public static OleDbDataReader CreateOleDbDataReader(String strQuery)
        {
            return CreateOleDbDataReader(strQuery, m_cpNativeParameters);
        }

        public static OleDbDataReader CreateOleDbDataReader(String strQuery, ConnectionParameters cp)
        {
            OleDbDataReader dtR = null;

            if (cp.IsOleDBConnection)
            {
                // Create and open the new connection
                OleDbConnection connOleDbReader = new OleDbConnection();
                connOleDbReader.ConnectionString = cp.ConnectionString;
                try
                {
                    connOleDbReader.Open();
                }
                catch (OleDbException oleE)
                {
                    throw oleE;
                }
                OleDbCommand command = new OleDbCommand(strQuery, connOleDbReader);
                command.CommandTimeout = 0;

                try
                {
                    dtR = command.ExecuteReader(CommandBehavior.Default);
                }
                catch (OleDbException oleE)
                {
                    System.Diagnostics.Debug.WriteLine(oleE.Message);
                    throw oleE;
                }
            }
            else
            {
                throw new Exception("Attempted to get OleDB datareader for MSSQL connection.");
            }
            return dtR;
        }

        public static OleDbDataReader CreateOleDbDataReader(String strQuery, String connString)
        {
            OleDbDataReader dtR = null;

            // Create and open the new connection
            OleDbConnection connOleDbReader = new OleDbConnection();
            connOleDbReader.ConnectionString = connString;
            try
            {
                connOleDbReader.Open();
            }
            catch (OleDbException oleE)
            {
                throw oleE;
            }

            // Set the command connection property to the current ODBC connection
            OleDbCommand command = new OleDbCommand(strQuery, connOleDbReader);

            try
            {
                dtR = command.ExecuteReader(CommandBehavior.Default);
            }
            catch (OleDbException oleE)
            {
                System.Diagnostics.Debug.WriteLine(oleE.Message);
                throw oleE;
            }
            return dtR;
        }

        public static List<string> GetDatabaseTables()
        {
            return GetDatabaseTables(m_cpNativeParameters);
        }

        public static List<string> GetDatabaseTables(ConnectionParameters cp)
        {
            string query;
            List<string> tableNames = new List<string>();
            switch (cp.Provider)
            {
                case "MSSQL":
                    query = "SELECT TABLE_NAME FROM information_schema.tables";
                    break;

                case "ORACLE":
                    query = "SELECT TABLE_NAME FROM user_tables";
                    break;

                default:
                    throw new NotImplementedException();
                    //break;
            }
            DataSet tables;
            tables = ExecuteQuery(query, cp);

            foreach (DataRow dr in tables.Tables[0].Rows)
            {
                tableNames.Add(dr[0].ToString());
            }
            return tableNames;
        }

        /// <summary>
        ///     Drops the specified column from the specified table using the
        ///     ALTER TABLE SQL statement.
        /// </summary>
        /// <param name="strTableName">Name of the table to alter</param>
        /// <param name="strColumnName">Name of the column to remove</param>
        /// <returns>
        ///     Returns true if the column was successfully removed from the
        ///     table.
        /// </returns>
        public static bool DropColumn(String strTableName, String strColumnName)
        {
            try
            {
                String str = "ALTER TABLE " + strTableName + " DROP COLUMN " + strColumnName;
                ExecuteNonQuery(str);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return true;
        }

        public static bool IsTableInDatabase(String strTableName)
        {
            int total = 0;
            switch (m_cpNativeParameters.Provider)
            {
                case "MSSQL":
                    total = DBMgr.ExecuteScalar("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '" + strTableName + "'");
                    break;

                case "ORACLE":
                    total = DBMgr.ExecuteScalar("SELECT COUNT(*) FROM USER_TABLES WHERE TABLE_NAME = '" + strTableName + "'");
                    break;

                default:
                    throw new Exception("Error: unknown database type specified.");
                    //break;
            }
            return total == 1;
        }

        public static int GetTableCount(string m_strAttribute, ConnectionParameters cp)
        {
            //catching an exception and then just throwing it again without any additional handling
            //is completely indistinguishable from not having the try/catch block

            //int count = 0;
            //try
            //{
            //    count = ExecuteScalar( "SELECT COUNT(*) FROM " + m_strAttribute, cp );
            //}
            //catch (Exception exc)
            //{
            //    throw exc;
            //}
            return ExecuteScalar("SELECT COUNT(*) FROM " + m_strAttribute, cp);
        }

        public static void RemoveTableColumns(string tableName, List<string> columnNames)
        {
            foreach (String columnName in columnNames)
            {
                String sqlDropColumn = "ALTER TABLE " + tableName + " DROP COLUMN " + columnName;
                DBMgr.ExecuteNonQuery(sqlDropColumn);
            }
        }

        public static void AddTableColumns(string tableName, Dictionary<string, string> columnNamesToTypes)
        {
            foreach (String key in columnNamesToTypes.Keys)
            {
                String sqlAddColumn = "ALTER TABLE " + tableName + " ADD " + key + " " + columnNamesToTypes[key];
                DBMgr.ExecuteNonQuery(sqlAddColumn);
            }
        }

        //public static int GetCurrentAutoIncrement(string tableName, string columnName)
        public static int GetCurrentAutoIncrement(string tableName)
        {
            //return GetCurrentAutoIncrement(tableName, columnName, m_cpNativeParameters);
            return GetCurrentAutoIncrement(tableName, m_cpNativeParameters);
        }

        //public static int GetCurrentAutoIncrement(string tableName, string columnName, ConnectionParameters cp)
        public static int GetCurrentAutoIncrement(string tableName, ConnectionParameters cp)
        {
            string query;
            int currentAutoIncrement;
            try
            {
                switch (cp.Provider)
                {
                    case "MSSQL":
                        //wrong wrong wrong
                        //query = "SELECT IDENT_CURRENT (" + columnName + ") FROM " + tableName;
                        query = "SELECT IDENT_CURRENT ('" + tableName + "')";
                        break;

                    case "ORACLE":
                        List<string> keys = GetOraclePrimaryKeys(tableName, cp);
                        if (keys.Count > 1)
                        {
                            throw new DataException("Table [" + tableName + "] contains more than one primary key column.");
                        }
                        else if (keys.Count < 1)
                        {
                            throw new DataException("Table [" + tableName + "] does not contain any primary key columns.");
                        }
                        else
                        {
                            string keyColumn = keys[0];
                            query = "SELECT case WHEN MAX(" + keyColumn + ") IS NULL THEN 1 ELSE MAX(" + keyColumn + ") + 1 END AS ID_ FROM " + tableName;
                        }
                        break;

                    default:
                        throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                        //break;
                }
                currentAutoIncrement = DBMgr.ExecuteScalar(query, cp);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return currentAutoIncrement;
        }

        public static void RenameTable(string oldTableName, string newTableName)
        {
            RenameTable(oldTableName, newTableName, m_cpNativeParameters);
        }

        public static void RenameTable(string oldTableName, string newTableName, ConnectionParameters cp)
        {
            string nonQuery;
            switch (cp.Provider)
            {
                case "MSSQL":
                    nonQuery = "EXEC sp_rename '" + oldTableName + "', '" + newTableName + "', 'OBJECT'";
                    break;

                case "ORACLE":
                    nonQuery = "alter table '" + oldTableName + "rename to " + newTableName;
                    break;

                default:
                    throw new NotImplementedException();
                    //break;
            }
            DBMgr.ExecuteNonQuery(nonQuery, cp);
        }

        public static void RenameColumn(string tableName, string oldColumnName, string newColumnName)
        {
            RenameColumn(tableName, oldColumnName, newColumnName, m_cpNativeParameters);
        }

        public static void RenameColumn(string tableName, string oldColumnName, string newColumnName, ConnectionParameters cp)
        {
            string nonQuery;
            switch (cp.Provider)
            {
                case "MSSQL":
                    nonQuery = "EXEC sp_rename '" + tableName + "." + oldColumnName + "', '" + newColumnName + "', 'COLUMN'";
                    break;

                case "ORACLE":
                    nonQuery = "alter table " + tableName + " rename column " + oldColumnName + " to " + newColumnName;
                    break;

                default:
                    throw new NotImplementedException();
                    //break;
            }
            DBMgr.ExecuteNonQuery(nonQuery, cp);
        }

        public static void DropTable(string tableName)
        {
            List<string> statementBatch = new List<string>();
            if (IsTableInDatabase(tableName))
            {
                statementBatch.Add("DROP TABLE " + tableName);
                //switch( DBMgr.NativeConnectionParameters.Provider )
                //{
                //    case "MSSQL":
                //        break;
                //    case "ORACLE":
                //        statementBatch.Add( GenerateOracleSequenceDrop( tableName ) );
                //        break;
                //    default:
                //        throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
                //        break;
                //}
                DBMgr.ExecuteBatchNonQuery(statementBatch);
            }
        }

        //public static string GenerateOracleSequenceDrop( string tableName )
        //{
        //    string dropStatement = "";
        //    List<string> primaryKeys = GetOraclePrimaryKeys( tableName );
        //    if( primaryKeys.Count == 1 )
        //    {
        //        dropStatement = "DROP SEQUENCE " + tableName + "_" + primaryKeys[0] + "_SEQ";
        //    }

        //    return dropStatement;
        //}

        public static List<string> GetOraclePrimaryKeys(string tableName, ConnectionParameters cp)
        {
            List<string> primaryKeys = new List<string>();
            string keyQuery = "SELECT cols.column_name FROM all_constraints cons, all_cons_columns cols WHERE cols.table_name = '" + tableName.ToUpper() + "' AND cons.constraint_type = 'P' AND cons.constraint_name = cols.constraint_name AND cons.owner = cols.owner ORDER BY cols.table_name, cols.position";
            DataSet keys = ExecuteQuery(keyQuery, cp);

            foreach (DataRow keyRow in keys.Tables[0].Rows)
            {
                primaryKeys.Add(keyRow[0].ToString());
            }

            return primaryKeys;
        }

        public static void ChangeColumnType(string tableName, string columnName, string newType)
        {
            ChangeColumnType(tableName, columnName, newType, m_cpNativeParameters);
        }

        private static void ChangeColumnType(string tableName, string columnName, string newType, ConnectionParameters cp)
        {
            string nonQuery = "";
            switch (cp.Provider)
            {
                case "MSSQL":
                    nonQuery = "ALTER TABLE " + tableName + " ALTER COLUMN " + columnName + " " + newType;
                    break;

                case "ORACLE":
                    throw new NotImplementedException();
                //break;
                default:
                    throw new NotImplementedException();
                    //break;
            }
            DBMgr.ExecuteNonQuery(nonQuery, cp);
        }

        public static ConnectionParameters GetConnectionParameter(ConnectionParameterType connType, string connName)
        {
            ConnectionParameters toReturn = null;
            if (connType == ConnectionParameterType.NETWORK)
            {
                string query = "SELECT CONNECTION_NAME, CONNECTION_ID, SERVER, PROVIDER, NATIVE_, DATABASE_NAME, SERVICE_NAME, SID, PORT, USERID, PASSWORD, INTEGRATED_SEC, VIEW_STATEMENT, IDENTIFIER FROM CONNECTION_PARAMETERS WHERE CONNECTION_NAME = '" + connName + "'";
                DataSet ds = DBMgr.ExecuteQuery(query);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    string connNameDB = dr["CONNECTION_NAME"].ToString();
                    string connID = dr["CONNECTION_ID"].ToString();
                    string serverName = dr["SERVER"].ToString();
                    string provider = dr["PROVIDER"].ToString();
                    bool native = Convert.ToBoolean(dr["NATIVE_"]);
                    string databaseName = dr["DATABASE_NAME"].ToString();
                    string networkAlias = dr["SERVICE_NAME"].ToString();
                    string SID = dr["SID"].ToString();
                    string port = dr["PORT"].ToString();
                    string userID = dr["USERID"].ToString();
                    string password = dr["PASSWORD"].ToString();
                    bool integratedSec = Convert.ToBoolean(dr["INTEGRATED_SEC"]);
                    string viewStmt = dr["VIEW_STATEMENT"].ToString();
                    string identifier = dr["IDENTIFIER"].ToString();

                    toReturn = new ConnectionParameters(port, SID, networkAlias, userID, password, integratedSec, serverName, databaseName, connNameDB, viewStmt, identifier, connID, provider, native);
                }
            }
            if (connType == ConnectionParameterType.ATTRIBUTE)
            {
                throw new NotImplementedException();
            }
            if (connType == ConnectionParameterType.ASSET)
            {
                throw new NotImplementedException();
            }
            return toReturn;
        }
    }

    public class TableParameters
    {
        private String m_strColumnName;
        private DataType m_columnDataType;
        private bool m_bIsNullable;
        private bool m_bIsPrimaryKey = false;
        private bool m_bIdentity = false;
        private int m_IsCLOB = 0;

        public TableParameters(String strColumnName, DataType columnDataType, bool bIsNullable, bool bIsPrimaryKey)
        {
            m_columnDataType = columnDataType;
            m_strColumnName = strColumnName;
            m_bIsNullable = bIsNullable;
            m_bIsPrimaryKey = bIsPrimaryKey;
        }

        public TableParameters(String strColumnName, DataType columnDataType, bool bIsNullable)
        {
            m_columnDataType = columnDataType;
            m_strColumnName = strColumnName;
            m_bIsNullable = bIsNullable;
            m_bIsPrimaryKey = false;
        }

        public TableParameters(String strColumnName, DataType columnDataType, bool bIsNullable, int IsCLOB)
        {
            m_columnDataType = columnDataType;
            m_strColumnName = strColumnName;
            m_bIsNullable = bIsNullable;
            m_bIsPrimaryKey = false;
            m_IsCLOB = IsCLOB;
        }

        public TableParameters(String strColumnName, DataType columnDataType, bool bIsNullable, bool bIsPrimaryKey, bool bIsIdentity)
        {
            m_columnDataType = columnDataType;
            m_strColumnName = strColumnName;
            m_bIsNullable = bIsNullable;
            m_bIsPrimaryKey = bIsPrimaryKey;
            m_bIdentity = bIsIdentity;
        }

        public String GetColumnName()
        {
            return m_strColumnName;
        }

        public DataType GetColumnDataType()
        {
            return m_columnDataType;
        }

        public bool GetIsNullable()
        {
            return m_bIsNullable;
        }

        public bool GetIsPrimaryKey()
        {
            return m_bIsPrimaryKey;
        }

        public bool GetIsIdentity()
        {
            return m_bIdentity;
        }

        public int GetIsCLOB()
        {
            return m_IsCLOB;
        }
    }
}