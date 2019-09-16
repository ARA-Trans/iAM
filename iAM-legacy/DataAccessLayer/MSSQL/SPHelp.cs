using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer.MSSQL
{
    public class SPHelp
    {
        Database m_databaseObject;
        List<Column> m_listColumnObject;
        List<Identity> m_listIdentityObject;
        List<Constraint> m_listConstraintObject;

        /// <summary>
        /// Database data about table (name, owner, type, created date)
        /// </summary>
        public Database Database
        {
            get { return m_databaseObject; }
        }

        /// <summary>
        /// List of all tables columns
        /// </summary>
        public List<Column> Columns
        {
            get { return m_listColumnObject; }
        }

        /// <summary>
        /// List of all identity information for columns
        /// </summary>
        public List<Identity> Identities
        {
            get { return m_listIdentityObject; }
        }

        /// <summary>
        /// Primary and foreign keys of this table
        /// </summary>
        public List<Constraint> Constraints
        {
            get { return m_listConstraintObject; }
        }

        /// <summary>
        /// Does this table have an identity primary key
        /// </summary>
        public bool IsIdentity
        {
            get
            {
                if (m_listIdentityObject == null) return false;
                if (m_listIdentityObject.Count == 0) return false;
                return m_listIdentityObject[0].IsIdentity;
            }
        }

        /// <summary>
        /// Tables primary key (only returns first).  Modify property to handle multiple primary keys.
        /// </summary>
        public Constraint PrimaryKey
        {
            get
            {
                if (m_listConstraintObject == null) return null;
                return m_listConstraintObject.Find(delegate(Constraint co) { return co.constraint_type.Contains( "PRIMARY KEY"); });
            }
        }

        /// <summary>
        /// List of a tables foreign keys.
        /// </summary>
        public List<Constraint> ForeignKeys
        {
            get
            {
                if (m_listConstraintObject == null) return null;
                return m_listConstraintObject.FindAll(delegate(Constraint co) { return co.constraint_type == "FOREIGN KEY"; });
            }
        }


        public SPHelp(string tableName)
        {
            Initialize(tableName, DB.ConnectionString);
        }


        /// <summary>
        /// Retrieves information about the SQL Table
        /// </summary>
        /// <param name="tableName">Name of table</param>
        public SPHelp(string tableName,string connectionString)
        {
            Initialize(tableName, connectionString);
        }

        private void Initialize(string tableName, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                DataSet ds = new DataSet();
                string sql = "sp_help[dbo." + tableName + "]";
                SqlCommand cmd = new SqlCommand(sql, connection);
                SqlDataAdapter adapter = null;
                try
                {
                    adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(ds);

                
                    if(ds.Tables.Count > 0) m_databaseObject = new Database(ds.Tables[0]);
                    if (ds.Tables.Count > 1)
                    {
                        m_listColumnObject = new List<Column>();
                        foreach (DataRow row in ds.Tables[1].Rows)
                        {
                            m_listColumnObject.Add(new Column(row));
                        }
                    }

                    if (ds.Tables.Count > 2)
                    {
                        m_listIdentityObject = new List<Identity>();
                        foreach (DataRow row in ds.Tables[2].Rows)
                        {
                            m_listIdentityObject.Add(new Identity(row));
                        }
                    }

                    if (ds.Tables.Count > 6)
                    {
                        m_listConstraintObject = new List<Constraint>();

                        Constraint previous = null;
                        foreach(DataRow row in ds.Tables[6].Rows)
                        {
                            Constraint current = new Constraint(row);
                        
                            if (string.IsNullOrWhiteSpace(current.constraint_name)) previous.references = GetReferenceTable(current.constraint_keys);
                            else m_listConstraintObject.Add(current);

                            previous = current;
                        }
                    }
                }
                catch (Exception e)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }

        /// <summary>
        /// Gets OMS reference table.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        private static string GetReferenceTable(string reference)
        {

            string table = reference.Replace("REFERENCES ", "");
            int find = table.IndexOf('(');
            if (find > 1)
            {
                table = table.Substring(0, find - 1);
            }

            find = table.LastIndexOf('.');
            if (find > 1)
            {
                table = table.Substring(find + 1);
            }

            return table;
        }
    }
}
