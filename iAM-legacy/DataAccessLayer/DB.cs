using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Utility.ExceptionHandling;

namespace DataAccessLayer
{
    /// <summary>
    /// This class contains database schema manipulation functions.
    /// </summary>
    public static class DB
    {
        public enum OMSFieldTypes { Text, YesNo, Integer, Number, Currency, DateTime, Lookup, OLEObj, Coordinate, Attachment, Quantity, Date, Time, Unknown=100 };

        private static string _connection;
        private static string _tablePrefix = "cgDE_";
        private static Dictionary<int, string> _typeIDTypeName;
        private static string _omsConnection;
        private static string _defaultNumberYears;
        private static string _defaultTargetOCI;
        


        public static string OMSConnectionString
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_omsConnection))
                {
                    string[] settings = (string[])System.Configuration.ConfigurationManager.AppSettings.GetValues("OMSConnectionString");
                    _omsConnection = settings[0];
                }
                return _omsConnection;
            }
            set
            {
                _omsConnection = value;
            }
        }

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_connection))
                {
                    string[] settings = (string[])System.Configuration.ConfigurationManager.AppSettings.GetValues("ConnectionString");
                    _connection = settings[0];
                }
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        public static int DefaultNumberYears
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_defaultNumberYears))
                {
                    string[] settings = (string[])System.Configuration.ConfigurationManager.AppSettings.GetValues("DefaultNumberYears");
                    _defaultNumberYears = settings[0];
                }
                int numberYears = 5;
                try
                {
                    numberYears = int.Parse(_defaultNumberYears);
                }
                catch(Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                }
                return numberYears;
            }
        }


        public static double DefaultTargetOCI
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_defaultTargetOCI))
                {
                    string[] settings = (string[])System.Configuration.ConfigurationManager.AppSettings.GetValues("DefaultTargetOCI");
                    _defaultTargetOCI = settings[0];
                }
                double targetOCI = 75;
                try
                {
                    targetOCI = double.Parse(_defaultTargetOCI);
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                }
                return targetOCI;
            }

        }
        
        
        public static string TablePrefix
        {
            get
            {
                return _tablePrefix;
            }
            set
            {
                _tablePrefix = value;
            }
        }


        public static string IsConnectionValid()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        return "SUCCESS";
                    }
                    else
                    {
                        return "DATABASE_OPEN_FAIL";
                    }
                }
                catch (Exception ex)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(ex, false);
                    return "DATABASE_OPEN_FAIL";
                }
            }
        }

        /// <summary>
        /// Checks if table exists in rcOMS.
        /// </summary>
        /// <param name="tableName">Table name to check</param>
        /// <returns>1 if table exists, 0 if table does not exist</returns>
        public static int CheckIfTableExists(string tableName)
        {
            return CheckIfTableExists(tableName, ConnectionString);
        }

        /// <summary>
        /// Checks if table exists in database with input connection string.
        /// </summary>
        /// <param name="tableName">Table name to check</param>
        /// /// <param name="connectionString">Connection string</param>
        /// <returns>1 if table exists, 0 if table does not exist</returns>
        public static int CheckIfTableExists(string tableName, string connectionString)
        {
            int exists = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("select case when exists((select * from information_schema.tables where table_name = @tableName)) then 1 else 0 end", connection);
                    cmd.Parameters.Add(new SqlParameter("tableName", tableName));
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        exists = Convert.ToInt32(dr[0]);
                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                    return exists;
                }
            }
            return exists;
        }

        /// <summary>
        /// Checks column against what exists.  Changes to match input if necessary.
        /// </summary>
        /// <param name="tableName">Table for which column exists</param>
        /// <param name="columnName">Name of column to check/add</param>
        /// <param name="type">Type of column (i.e. int, varchar, float, etc)</param>
        /// <param name="max_length">Length of variable that have length, -1 if length not necessary, Int.MaxValue uses MAX length</param>
        /// <param name="nullable">Is column nullable</param>
        /// <param name="columnList">List of columns in table</param>
        /// <returns>True if column good or modified successfully</returns>
        public static bool CheckSqlColumn(string tableName, string columnName, string type, int max_length, bool nullable, List<SQLColumn> columnList)
        {
            SQLColumn column = columnList.Find(delegate(SQLColumn sqlColumn) { return sqlColumn.name == columnName; });
            if (column == null)
            {
                string alter = tableName + " ADD " + columnName + " " + type;
                if (max_length > 0 && max_length < int.MaxValue) alter += "(" + max_length.ToString() + ")";
                else if (max_length == int.MaxValue) alter += "(MAX)";
                if (!nullable) alter += " NOT NULL";
                if (!AlterTable(alter)) return false;
                else return true;
            }

            if (column.column_type_string != type || column.is_nullable != nullable)
            {
                string alter = tableName + " ALTER COLUMN " + columnName + " " + type;
                if (max_length > 0)
                {
                    if (max_length < int.MaxValue) alter += "(" + max_length.ToString() + ")";
                    else alter += "(MAX)";
                }
                if (!nullable) alter += " NOT NULL";
                if (!AlterTable(alter)) return false;
            }

            if (max_length > 0 && max_length <= 4000 && max_length != column.max_length)
            {
                string alter = tableName + " ALTER COLUMN " + columnName + " " + type;
                if (max_length > 0)
                {
                    if (max_length < int.MaxValue) alter += "(" + max_length.ToString() + ")";
                    else alter += "(MAX)";
                }
                if (!AlterTable(alter)) return false;
            }


            return true;
        }

        /// <summary>
        /// Retrieves a list of SQL columns in the rcOMS table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>List of SQLColumns (and column properties)</returns>
        public static List<SQLColumn> GetTableColumns(string tableName)
        {
            List<SQLColumn> columnList = new List<SQLColumn>();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('" + tableName + "')", connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        columnList.Add(new SQLColumn(dr));
                    }
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, "Error getting columns from sys.Columns", false);
                    columnList = null;
                }
            }
            return columnList;
        }

        /// <summary>
        /// Given a sql type integer, returns the text version of sql type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetSqlColumnType(int type)
        {
            string typeColumn = null;

            if (_typeIDTypeName == null)
            {
                _typeIDTypeName = new Dictionary<int, string>();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("SELECT user_type_id, name FROM sys.types", connection);
                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            string system_type_id = dr["user_type_id"].ToString();
                            string name = dr["name"].ToString();
                            _typeIDTypeName.Add(Convert.ToInt32(system_type_id), name);
                        }
                    }
                    catch (Exception e)
                    {
                        Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, "Error getting column type in GetSqlColumns.", false);
                        return typeColumn;
                    }
                }
            }
            return _typeIDTypeName[type];
        }

        /// <summary>
        /// Designed to be called from UpdateTables.  Alter table by the alterstatement
        /// </summary>
        /// <param name="alterStatement">the part of the command after ALTER TABLE (example...[SIMULATION] add [SCENARIO_ID] int default 0 NOT NULL") </param>
        public static bool AlterTable(string alterStatement)
        {

            bool isAlterSuccessful = true;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("ALTER TABLE " + alterStatement, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, "Alter table exception", false);
                    isAlterSuccessful = false;
                }
            }
            return isAlterSuccessful;
        }


        /// <summary>
        /// Create a new table with columns in colum list.  Fancy tables use AlterTable to modify.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnList"></param>
        /// <returns></returns>
        public static bool CreateTable(string tableName, List<SQLColumn> columnList)
        {
            bool isCreateTableSuccessful = true;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "CREATE TABLE dbo." + tableName + "(";

                    foreach (SQLColumn column in columnList)
                    {
                        if (sql.LastIndexOf('(') + 1 != sql.Length) sql += ",";
                        sql += column.name + " " + column.column_type_string;

                        if (column.column_type_string == "varchar")
                        {
                            sql += " (10)";
                        }

                        if (column.is_identity) sql += " IDENTITY(1,1) ";
                        if (!column.is_nullable) sql += " not null";
                    }

                    sql += ")";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, "Create table error", false);
                    isCreateTableSuccessful = false;
                }
            }
            return isCreateTableSuccessful;
        }

        /// <summary>
        /// Drops the name table.
        /// </summary>
        /// <param name="tableName">Table name to drop</param>
        /// <returns>True if successful, false if fails.</returns>
        public static bool DropTable(string tableName)
        {
            bool isDropSuccessful = true;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "DROP TABLE " + tableName;
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    //Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, "Dropping table error.  May be OK if attempting to drop non-existent table.", false);
                    isDropSuccessful = false;
                }
            }
            return isDropSuccessful;
        }

        /// <summary>
        /// Returns the column name of the primary key field
        /// </summary>
        /// <param name="table">Name of table for which primary key is sought</param>
        /// <returns>Name of primary key column, null if no primary key is present.</returns>
        public static string GetPrimaryKey(string table)
        {
            string primaryKey = null;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string sql = "SELECT Col.Column_Name from INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab,  INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col WHERE  Col.Constraint_Name = Tab.Constraint_Name AND Col.Table_Name = Tab.Table_Name AND Constraint_Type = 'PRIMARY KEY '  AND Col.Table_Name = '" + table + "'";
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        primaryKey = dr["Column_Name"].ToString();
                    }
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                    primaryKey = null;  //OK, means primary key is not set.
                }
            }
            return primaryKey;
        }

        public static void SQLBulkLoad(String strTableName, String strPathName, char cDelimeter)
        {

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string database = connection.Database;
                string datasource = connection.DataSource;
                string userID = null;
                string password = null;
                string trustedConnection = null;
                
                string[] parameters = connection.ConnectionString.Split(';');

                foreach (string parameter in parameters)
                {
                    string[] values = parameter.Split('=');
                    if (values.Length == 2)
                    {
                        if (values[0].ToUpper().Contains("USER ID"))
                        {
                            userID = values[1];
                        }
                        else if (values[0].ToUpper().Contains("PASSWORD"))
                        {
                            password = values[1];
                        }
                        else if (values[0].ToUpper().Contains("TRUSTED_CONNECTION"))
                        {
                            trustedConnection = values[1];
                        }
                        else if (values[0].ToUpper().Contains("INTEGRATED SECURITY"))
                        {
                            trustedConnection = "true";
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(database) && !string.IsNullOrWhiteSpace(datasource) && ((!string.IsNullOrWhiteSpace(userID) && !string.IsNullOrWhiteSpace(password)) || !string.IsNullOrWhiteSpace(trustedConnection)))
                {

                    string BCP;
                    if (trustedConnection == null)
                    {
                        BCP = "\"" + database + ".dbo." + strTableName + "\" IN \"" + strPathName + "\" -S " + datasource + " -t \"" + cDelimeter + "\" -c -q -U " + userID + " -P " + password;
                    }
                    else
                    {
                        BCP = "\"" + database + ".dbo." + strTableName + "\" IN \"" + strPathName + "\" -S " + datasource + " -t \"" + cDelimeter + "\" -c -q -T";
                    }
                        
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.EnableRaisingEvents = false;
                    proc.StartInfo.FileName = "bcp";
                    proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    proc.StartInfo.Arguments = BCP;
                    proc.Start();
                    proc.WaitForExit();
                }
            }
        }
    }
}
