using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DatabaseManager;
using DBSpy.Properties;
using System.Collections.Generic;
using RoadCareDatabaseOperations;



namespace DBSpy
{
    public partial class frmDataDisplay : Form
    {
        public string mSelectedTable;
        private bool mTableSelected;
        ArrayList arrViews;
        ArrayList arrTables;
		private List<ConnectionParameters> m_listConnections = new List<ConnectionParameters>();

        String m_strDatabase;
        String m_strServer;
        String m_strUserName;
        String m_strPassword;
        String m_strViewName;
		String m_strProvider;
        string _netDefName;

        private String m_strCreateView;
		private string viewSelectStatement;


        /// <summary>
        /// default constructor
        /// </summary>
		public frmDataDisplay(String strServerName, String strDatabaseName, String strUserLogin, String strPassword, String strViewName, String strProvider, string netDefName)
        {
            InitializeComponent();
            m_strDatabase = strDatabaseName;
            m_strPassword = strPassword;
            m_strServer = strServerName;
            m_strUserName = strUserLogin;
			m_strViewName = strViewName;
			m_strProvider = strProvider;
            _netDefName = netDefName;

			lblCreateSQLView.Text = "CREATE VIEW " + m_strViewName + " AS ";

			// get tables and views
			StoreTableAndViewNames(Settings.Default.ConnString);

			// clear internal lists
			lstTables.Items.Clear();
			lstViews.Items.Clear();

			// update the lists from the arrays holding the
			// tables and views
			lstTables.Items.AddRange(arrTables.ToArray());
			lstViews.Items.AddRange(arrViews.ToArray());
        }

        public String View
        {
            get { return m_strCreateView; }
        }

        /// <summary>
        /// Populate to arrays with list of all of the tables and views used
        /// in the database defined by the current connection string
        /// </summary>
        public void StoreTableAndViewNames(string curConnectionString)
        {
            // temporary holder for the schema information for the current
            // database connection
            DataTable SchemaTable;

            // used to hold a list of views and tables
            arrViews = new ArrayList();
            arrTables = new ArrayList();

            // clean up the menu so the menu item does not
            // hang while this function executes
            this.Refresh();

            // make sure we have a connection
            if (Properties.Settings.Default.ConnString != string.Empty)
            {
                // start up the connection using the current connection string
                using (OleDbConnection conn = new OleDbConnection(curConnectionString))
                {
					DataSet ds = new DataSet();
                    try
                    {
                        // open the connection to the database 
                        conn.Open();

                        // Get the Tables
                        SchemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, "TABLE" });

                        // Store the table names in the class scoped array list of table names
                        for (int i = 0; i < SchemaTable.Rows.Count; i++)
                        {
                            arrTables.Add(SchemaTable.Rows[i].ItemArray[2].ToString());
                        }

                        // Get the Views
                        SchemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, "VIEW" });

                        // Store the view names in the class scoped array list of view names
                        for (int i = 0; i < SchemaTable.Rows.Count; i++)
                        {
                            arrViews.Add(SchemaTable.Rows[i].ItemArray[2].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        // break and notify if the connection fails
                        MessageBox.Show(ex.Message, "Connection Error");
                    }
                }

            }
        }

        /// <summary>
        /// Close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // dispose of this form
            this.Dispose();
        }

		///// <summary>
		///// Open a new connection to a database - present the connection string builder
		///// form so the user can define a connection
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//private void OpenANewConnectionToolStripMenuItem_Click(object sender, EventArgs e)
		//{
		//    // open an instance the connect form so the user
		//    // can define a new connection
		//    frmConnect f = new frmConnect(m_strServer, m_strDatabase, m_strUserName, m_strPassword, m_strViewName);
		//    f.Show();
		//}

        /// <summary>
        /// Display the current connection string to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewCurrentConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // display the current connection string
            MessageBox.Show(Properties.Settings.Default.ConnString, "Current Connection");
        }

        /// <summary>
        /// Get and display the current tables and views contained in the database
        /// pointed to by the connection string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadDataForCurrentConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
			//// Get the correct connection parameters object
			//if (lstConnections.SelectedItem != null)
			//{
			//    ConnectionParameters cp = DBOp.GetConnectionParameters(lstConnections.SelectedItem.ToString());

			//    // get tables and views
			//    StoreTableAndViewNames(cp.ConnectionString);

			//    // clear internal lists
			//    lstTables.Items.Clear();
			//    lstViews.Items.Clear();

			//    // update the lists from the arrays holding the
			//    // tables and views
			//    lstTables.Items.AddRange(arrTables.ToArray());
			//    lstViews.Items.AddRange(arrViews.ToArray());
			//}

        }

        private void lstTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            mTableSelected = true;
            string tblName;

            try
            {
                tblName = lstTables.SelectedItem.ToString();
            }
            catch
            {
                return;
            }

             // make sure we have a connection
            if (Properties.Settings.Default.ConnString != string.Empty)
            {
                // start up the connection using the current connection string
                using (OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.ConnString))
                {

                    try
                    {
                        // open the connection to the database 
                        conn.Open();
                        lstFields.Items.Clear();

                        DataTable dtField = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tblName });

                        foreach (DataRow dr in dtField.Rows)
                        {
                            lstFields.Items.Add(dr["COLUMN_NAME"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Connection Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("There is no connection string current defined.", "Connection String");
            }

        }

        private void lstViews_SelectedIndexChanged(object sender, EventArgs e)
        {
            mTableSelected = false;
            string tblName;

            try
            {
                tblName = lstViews.SelectedItem.ToString();
            }
            catch
            {
                return;
            }

            // make sure we have a connection
            if (Properties.Settings.Default.ConnString != string.Empty)
            {
                // start up the connection using the current connection string
                using (OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.ConnString))
                {

                    try
                    {
                        // open the connection to the database 
                        conn.Open();
                        lstFields.Items.Clear();

                        // get the schema table
                        DataTable dtField = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tblName });

                        // read the column names into the fields list
                        foreach (DataRow dr in dtField.Rows)
                        {
                            lstFields.Items.Add(dr["COLUMN_NAME"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Connection Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("There is no connection string current defined.", "Connection String");
            }
        }

        /// <summary>
        /// Collect and display the field information for a selected column name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetFieldInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.ConnString))
                {
                    string sSql = string.Empty;

                    if (mTableSelected == true)
                    {
                        sSql = "SELECT [" + lstFields.SelectedItem.ToString().Trim() + "] FROM [" + lstTables.SelectedItem.ToString().Trim() + "]";
                    }
                    else
                    {
                        sSql = "SELECT [" + lstFields.SelectedItem.ToString().Trim() + "] FROM [" + lstViews.SelectedItem.ToString().Trim() + "]";
                    }
                    
                    OleDbCommand cmd = new OleDbCommand(sSql, conn);
                    conn.Open();
                    
                    OleDbDataReader rdr = cmd.ExecuteReader(CommandBehavior.KeyInfo);
                    DataTable schemaTable = rdr.GetSchemaTable();
                    StringBuilder sb = new StringBuilder();
                    
                    foreach (DataRow myField in schemaTable.Rows)
                    {
                        foreach (DataColumn myProperty in schemaTable.Columns) 
                        {
                            sb.Append(myProperty.ColumnName + " = " + myField[myProperty].ToString() + Environment.NewLine);
                        }

                        // report
                        MessageBox.Show(sb.ToString(), "Field Information");
                        
                        // burn the reader
                        rdr.Close();

                        // exit 
                        return;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Unable to attach to this table with current user; check database security permissions.", "Field Information");
            }

        }

        /// <summary>
        /// Ignore selections from the fields list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            // do nothing
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
			bool connected = false;
            try
            {
                using (OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                }
                tbDataDisplay.Text = "Connection Test successful";
				connected = true;
            }
            catch (OleDbException oleE)
            {
                tbDataDisplay.Text = "Error creating OLE DB connection. " + oleE.Message + "\n";
                return;
            }
            catch (Exception sqlE)
            {
                tbDataDisplay.Text = "Error creating SQL connection. " + sqlE.Message + "\n";
                return;
            }
			if( connected )
			{
				try
				{
					tbDataDisplay.Text += " Select statement successful.";
				}
				catch( Exception ex )
				{
					tbDataDisplay.Text += " Select statement failed.  " + ex.Message;
				}
			}
                
        }

        /// <summary>
        /// Executes SQL non query statements. Examples include UPDATE, INSERT, and DELETE statements
        /// </summary>
        /// <param name="strNonQuery">The statement to execute on the database.</param>
        /// <param name="oleConn">The ole connection object, or null if using a SQL connection</param>
        /// <param name="sqlConn">The sql connection object, or null if using an OLE connection</param>
        public static int ExecuteNonQuery(String strNonQuery, OleDbConnection oleConn, SqlConnection sqlConn)
        {
            int iRows = 0;
            if (oleConn == null)
            {
                SqlCommand cmd = new SqlCommand(strNonQuery, sqlConn);
                try
                {
                    iRows = cmd.ExecuteNonQuery();
                }
                catch (Exception sqlE)
                {
                    throw sqlE;
                }
            }
            else
            {
                OleDbCommand cmd = new OleDbCommand(strNonQuery, oleConn);
                try
                {
                    iRows = cmd.ExecuteNonQuery();
                }
                catch (OleDbException oleE)
                {
                    throw oleE;
                }
            }
            return iRows;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
			m_strCreateView = lblCreateSQLView.Text + tbCreateSQLView.Text;
			viewSelectStatement = tbCreateSQLView.Text;
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

		public string ViewSelectStatement
		{
			get
			{
				return viewSelectStatement;
			}
		}

		public void SetCreateViewLabel(string text)
		{
			lblCreateSQLView.Text = text;
		}

		private void OpenANewConnectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			frmConnect newConnectionForm = new frmConnect(
				Settings.Default.ServerName,
				Settings.Default.DatabaseName,
				Settings.Default.IntegratedSecurity,
				Settings.Default.SID,
				Settings.Default.NetworkAlias,
				Settings.Default.Port,
				Settings.Default.UserID,
				Settings.Default.Password,
				Settings.Default.ProviderString,
				Settings.Default.AttributeName);
			newConnectionForm.ShowDialog();
		}
    }
}
