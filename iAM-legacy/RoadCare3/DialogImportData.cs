using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PropertyGridEx;
using DatabaseManager;
using System.Data.SqlClient;
using System.IO;
using RoadCare3.Properties;

namespace RoadCare3
{
    public partial class DialogImportData : Form
    {
        private string[] providers = new string[] { "MSSQL", "ORACLE", "MYSQL", "ACCESS", "DBF", "SQLBASEOLEDB" };
        private string[] connectionTypes = new string[] { "Network Alias", "Basic" };
        private ConnectionParameters m_cp = null;
        private bool m_bIsLinear;
        private String m_strAttributeName;
        private bool m_bIsStringField;
        private List<String> m_listRouteFacility;

        public DialogImportData(bool bIsLinear, bool bIsStringField, String strAttributeName, List<String> listRouteFacility)
        {
            InitializeComponent();
            m_bIsLinear = bIsLinear;
            m_strAttributeName = strAttributeName;
            m_bIsStringField = bIsStringField;
            m_listRouteFacility = listRouteFacility;
        }

        private void DialogImportData_Load(object sender, EventArgs e)
        {
            LoadConnectionPropertyGrid();
			//dsmelser
			//since we have no other idea, we might as well assume the table we're pulling from has the same name as the attribute
			tbSQLQuery.Text += m_strAttributeName;
        }

        private void LoadConnectionPropertyGrid()
        {
            pgConnection.Item.Add("Provider", "", false, "Connection Information", "Database Type (MSSQL, ORACLE, MYSQL).", true);
            pgConnection.Item[pgConnection.Item.Count - 1].Choices = new CustomChoices(providers, false);

            pgConnection.Item.Add("Connection Type", "", false, "Connection Information", "Connection Type - Network Alias or Basic.", true);
            pgConnection.Item[pgConnection.Item.Count - 1].Choices = new CustomChoices(connectionTypes, false);

            pgConnection.Item.Add("Use Integrated Security", true, false, "Connection Information", "True to use windows authentication.", true);
            pgConnection.Item.Add("Server", "", false, "Connection Information", "Name of server on which the attribute data resides.", true);
            pgConnection.Item.Add("Database", "", false, "Connection Information", "Name of the database in which the attribute data resides.", true);
            pgConnection.Item.Add("SID", "", false, "Connection Information", "", true);
            pgConnection.Item.Add("Network Alias", "", false, "Connection Information", "", true);
            pgConnection.Item.Add("Port", "", false, "Connection Information", "", true);
            pgConnection.Item.Add("Login", "", true, "Connection Information", "User name.", true);
            pgConnection.Item.Add("Password", "", true, "Connection Information", "User password", true);
            pgConnection.Item["Password"].IsPassword = true;
            
			pgConnection.Item["Provider"].Value = Settings.Default.PROVIDER_DIU;
			pgConnection.Item["Server"].Value = Settings.Default.SERVER_DIU;
			pgConnection.Item["Database"].Value = Settings.Default.DATABASE_DIU;
			pgConnection.Item["SID"].Value = Settings.Default.DBSID;
			pgConnection.Item["Port"].Value = Settings.Default.DBPORT;
			pgConnection.Item["Network Alias"].Value = Settings.Default.DBNETWORKALIAS;

			if (pgConnection.Item["Provider"].Value.ToString() == "MSSQL")
			{
				MakeReadOnly("SID");
				MakeReadOnly("Port");
				MakeReadOnly("Network Alias");
                MakeReadOnly("Connection Type");

				ChangeToReadWrite("Database");
                ChangeToReadWrite("Server");
			}
			else // Oracle
			{
                // Turn off everything (except connection type) till connection type is specified.
                MakeReadOnly("SID");
                MakeReadOnly("Port");
                MakeReadOnly("Server");
                MakeReadOnly("Database");
                MakeReadOnly("Network Alias");
			}
            pgConnection.Refresh();
        }

        private void MakeReadOnly(String strLabel)
        {
            for (int i = 0; i < pgConnection.Item.Count; i++)
            {
                if (pgConnection.Item[i].Name.ToString() == strLabel)
                {
                    pgConnection.Item[i].IsReadOnly = true;
                }
            }
        }

        private void ChangeToReadWrite(String strLabel)
        {
            for (int i = 0; i < pgConnection.Item.Count; i++)
            {
                if (pgConnection.Item[i].Name.ToString() == strLabel)
                {
                    pgConnection.Item[i].IsReadOnly = false;
                }
            }
        }

        private void pgConnection_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.Label == "Use Integrated Security")
            {
                if (e.OldValue.ToString() == "False")
                {
                    MakeReadOnly("Login");
                    MakeReadOnly("Password");
                }
                else
                {
                    ChangeToReadWrite("Login");
                    ChangeToReadWrite("Password");
                }
            }
			if (e.ChangedItem.Label == "Provider")
			{
				if (e.OldValue.ToString() == "ORACLE")
				{
                    // SQL
					MakeReadOnly("SID");
					MakeReadOnly("Port");
					MakeReadOnly("Network Alias");
                    MakeReadOnly("Connection Type");

					ChangeToReadWrite("Database");
                    ChangeToReadWrite("Server");
				}
				else
				{
                    // ORACLE
                    if (pgConnection.Item["Connection Type"].Value.ToString() == "Network Alias")
                    {
                        MakeReadOnly("Server");
                        MakeReadOnly("SID");
                        MakeReadOnly("Port");
                        MakeReadOnly("Database");

                        ChangeToReadWrite("Network Alias");
                    }
                    if (pgConnection.Item["Connection Type"].Value.ToString() == "Basic")
                    {
                        MakeReadOnly("Network Alias");
                        MakeReadOnly("Database");

                        ChangeToReadWrite("SID");
                        ChangeToReadWrite("Server");
                        ChangeToReadWrite("Port");
                        ChangeToReadWrite("Database");
                    }
                    if (pgConnection.Item["Connection Type"].Value.ToString() == "")
                    {
                        MakeReadOnly("Server");
                        MakeReadOnly("SID");
                        MakeReadOnly("Port");
                        MakeReadOnly("Database");
                        MakeReadOnly("Network Alias");
                    }
                    ChangeToReadWrite("Connection Type");
				}
			}
            if (e.ChangedItem.Label == "Connection Type")
            {
                if (e.ChangedItem.Value.ToString() == "Network Alias")
                {
                    MakeReadOnly("Server");
                    MakeReadOnly("SID");
                    MakeReadOnly("Port");
                    MakeReadOnly("Database");

                    ChangeToReadWrite("Network Alias");
                }
                else
                {
                    MakeReadOnly("Network Alias");

                    ChangeToReadWrite("SID");
                    ChangeToReadWrite("Server");
                    ChangeToReadWrite("Port");
                    ChangeToReadWrite("Database");
                }
            }
            pgConnection.Refresh();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
			String provider = pgConnection.Item["Provider"].Value.ToString();
			String database = pgConnection.Item["Database"].Value.ToString();
			String server = pgConnection.Item["Server"].Value.ToString();
			String userName = pgConnection.Item["Login"].Value.ToString();
			String password = pgConnection.Item["Password"].Value.ToString();
			bool bIntegratedSecurity = Convert.ToBoolean(pgConnection.Item["Use Integrated Security"].Value);
			string SID = pgConnection.Item["SID"].Value.ToString();
			string networkAlias = pgConnection.Item["Network Alias"].Value.ToString();
			string port = pgConnection.Item["Port"].Value.ToString();

			//string strConnectionString;
			m_cp = new ConnectionParameters(port, SID, networkAlias, userName, password, bIntegratedSecurity, server, database, "", "", "", "", provider, false);
            try
            {
				List<string> tableNames = DBMgr.GetDatabaseTables(m_cp);
                lstTables.Items.Clear();
                lstFields.Items.Clear();
                lstData.Items.Clear();
                foreach (string tableName in tableNames)
                {
                    lstTables.Items.Add(tableName);
                }
                
                Settings.Default.DATABASE_DIU = database;
				Settings.Default.SERVER_DIU = server;
                Settings.Default.PROVIDER_DIU = provider;
                Settings.Default.USERNAME_DIU = userName;
				Settings.Default.DBNETWORKALIAS = networkAlias;
				Settings.Default.DBSID = SID;
				Settings.Default.DBPORT = port;
                Settings.Default.Save();
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: Could not connect to data source. " + exc.Message);
            }
        }

        private void lstTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            String strTable = lstTables.SelectedItem.ToString();
            //String strQuery;
            //DataSet ds = null;
            lstFields.Items.Clear();
            lstData.Items.Clear();
			List<string> columnNames = DBMgr.GetTableColumns( strTable, m_cp );
			foreach( string columnName in columnNames )
			{
				lstFields.Items.Add( columnName );
			}
        }

        private void lstFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                String strTable = lstTables.SelectedItem.ToString();
                String strField = lstFields.SelectedItem.ToString();
				String strQuery = "";
				switch( m_cp.Provider )
				{
					case "MSSQL":
						strQuery = "SELECT DISTINCT TOP 25 " + strField + " FROM " + strTable;
						break;
					case "ORACLE":
						strQuery = "SELECT * FROM (SELECT DISTINCT " + strField + " FROM " + strTable + ") WHERE ROWNUM < 26";
						break;
					default:
						throw new NotImplementedException( "TODO: Implement ANSI version of lstFields_SelectedIndexChanged()" );
						//break;
				}

                lstData.Items.Clear();
                try
                {
                    DataSet ds = DBMgr.ExecuteQuery(strQuery, m_cp);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        lstData.Items.Add(ds.Tables[0].Rows[i].ItemArray[0].ToString());
                    }
                }
                catch (Exception exc)
                {
                    Global.WriteOutput("Error: " + exc.Message);
                }
            }
            catch //(Exception exc)
            {
                return;
            }
        }

        public void SetExampleLabel(String strValue)
        {
            lblExample.Text = strValue;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
			//dsmelser
			//the user may not always test the connection before attempting an import
			//technically, they shouldn't have to, but the functionality for setting up the connectionparameters object
			//isn't split off in any convenient way...
			//cbb - The user must connect before hitting ok, so there should be a CP associated with the connection at this point.
			if( m_cp != null )
			{
				this.Cursor = Cursors.WaitCursor;
				String strSQL = tbSQLQuery.Text;
				DoImport doImport = new DoImport( strSQL, m_bIsLinear, m_bIsStringField, m_listRouteFacility, m_strAttributeName, m_cp );
				doImport.Import();
				String strError = "";
				foreach( String str in doImport.Errors )
				{
					strError += str + "\r\n";
				}
				Global.WriteOutput( strError );
				this.Close();
			}
			else
			{
				Global.WriteOutput( "Error: The connection must be tested before an import attempt is made." );
			}
        }

       
    }
}
