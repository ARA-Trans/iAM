using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;
using RoadCareDatabaseOperations;


namespace DBSpy
{
    public partial class frmConnect : Form
    {
		string m_server;
		string m_database;
		bool m_bIntegratedSecurity;
		string m_SID;
		string m_networkAlias;
		string m_port;
		string m_userName;
		string m_password;
		string m_attributeName;
		string m_provider;
		string m_cpInsert;
		string m_connName;

        /// <summary>
        /// Default constructor
        /// </summary>
        public frmConnect(
			string server, 
			string database, 
			bool bIntegratedSecurity,
			string SID,
			string networkAlias,
			string port,
			string userName,
			string password,
			string provider,
			string attributeName)
        {
            InitializeComponent();

			m_attributeName = attributeName;
			m_server = server;
			m_database = database;
			m_bIntegratedSecurity = bIntegratedSecurity;
			m_SID = SID;
			m_networkAlias = networkAlias;
			m_port = port;
			m_userName = userName;
			m_password = password;
			m_provider = provider;
			m_attributeName = attributeName;
			if (m_attributeName != "")
			{
				tbConnectionName.Text = m_attributeName;
				tbOracleConnectionName.Text = m_attributeName;
				cbSqlConnType.Text = "ATTRIBUTE";
				cbOracleConnType.Text = "ATTRIBUTE";

				tbOracleConnectionName.ReadOnly = true;
				cbOracleConnType.Enabled = false;
				tbConnectionName.ReadOnly = true;
				cbSqlConnType.Enabled = false;
			}
            LoadDefaults();
        }

        public frmConnect()
        {
            InitializeComponent();

			cbSqlConnType.Text = "NETWORK";
			cbOracleConnType.Text = "NETWORK";

			cbSqlConnType.Enabled = false;
			cbOracleConnType.Enabled = false;
        }

		public string AttributeName
		{
			get
			{
				return m_attributeName;
			}
		}

		public string Provider
		{
			get
			{
				return m_provider;
			}
		}

		public string Password
		{
			get
			{
				return m_password;
			}
		}

		public string UserName
		{
			get
			{
				return m_userName;
			}
		}

		public string Port
		{
			get
			{
				return m_port;
			}
		}

		public string NetworkAlias
		{
			get
			{
				return m_networkAlias;
			}
		}

		public string SID
		{
			get
			{
				return m_SID;
			}
		}

		public bool IntegratedSecurity
		{
			get
			{
				return m_bIntegratedSecurity;
			}
		}

		public string Database
		{
			get
			{
				return m_database;
			}
		}

		public string Server
		{
			get
			{
				return m_server;
			}
		}

		public string ConnectionName
		{
			get
			{
				return m_connName;
			}
		}

        public void LoadDefaults()
        {
			switch (m_provider)
			{
				case "ORACLE":
					TabControl1.SelectedTab = tabOracle;
					txtOracleUserID.Text = m_userName;
					txtOraclePassword.Text = m_password;
					tbOracleServerName.Text = m_server;
					tbPort.Text = m_port;
					tbSID.Text = m_SID;
					chkUseIntegratedSecurity.Checked = m_bIntegratedSecurity;
					break;
				case "MSSQL":
					TabControl1.SelectedTab = TabSqlServer;
					txtSqlServerInitialCat.Text = m_database;
					txtSqlServerName.Text = m_server;
					txtSqlServerUserID.Text = m_userName;
					txtSqlServerPassword.Text = m_password;
					chkUseIntegratedSecurity.Checked = m_bIntegratedSecurity;
					break;
				default:
					break;
			}
        }

        /// <summary>
        /// Store the Oracle settings and test the connection
        /// string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOracleOK_Click(object sender, EventArgs e)
        {
			// If the name of the connection is not left blank 
			//if (tbConnectionName.Text != "" && DBOp.GetConnectionParameters(tbConnectionName.Text) == null
			//    && (cbSqlConnType.Text == "ATTRIBUTE" || cbSqlConnType.Text == "ASSET" || cbSqlConnType.Text == "NETWORK"))
			{
                //m_cpInsert = "INSERT INTO CONNECTION_PARAMETERS (CONNECTION_NAME, CONNECTION_ID, SERVER, PROVIDER, NATIVE_, DATABASE_NAME, SERVICE_NAME, SID, PORT, USERID, PASSWORD, INTEGRATED_SEC, VIEW_STATEMENT, IDENTIFIER) VALUES ('" +
                //    tbConnectionName.Text + "', " + "''" + ", '" + txtSqlServerName.Text + "', 'ORACLE', " + "'False'" + ", '" + txtSqlServerInitialCat.Text + "', " + "''" + ", " + "''" + ", " + "''" + ", '" + txtSqlServerUserID.Text + "', '" + txtSqlServerPassword.Text + "', '" + chkUseIntegratedSecurity.Checked + "', " + "''" + ", '" + cbSqlConnType.Text + "')";
				try
				{
					Properties.Settings.Default.NetworkAlias = tbNetworkAlias.Text;
					Properties.Settings.Default.ServerPort = tbPort.Text;
					Properties.Settings.Default.ServerName = tbOracleServerName.Text;
					Properties.Settings.Default.UserID = txtOracleUserID.Text;
					Properties.Settings.Default.Password = txtOraclePassword.Text;
					Properties.Settings.Default.SID = tbSID.Text;
					Properties.Settings.Default.ProviderString = tbOracleProvider.Text;
					Properties.Settings.Default.AttributeName = m_attributeName;
					Properties.Settings.Default.IntegratedSecurity = m_bIntegratedSecurity;
					Properties.Settings.Default.ProviderText = "ORACLE";

					if (tbSID.Text != "")
					{
						Properties.Settings.Default.ConnString = "Provider=OraOLEDB.Oracle;Data Source=(DESCRIPTION=(CID=GTU_APP)(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + tbOracleServerName.Text + ")(PORT=" + tbPort.Text + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + tbSID.Text + ")));User Id=" + txtOracleUserID.Text + ";Password=" + txtOraclePassword.Text + ";";
					}
					if (tbNetworkAlias.Text != "")
					{
						Properties.Settings.Default.ConnString = "Provider=OraOLEDB.Oracle;Data Source=" + tbNetworkAlias.Text + ";" + "User Id=" + txtOracleUserID.Text + ";Password=" + txtOraclePassword.Text + ";";
					}

					// Save the property settings
					Properties.Settings.Default.Save();
					if (Properties.Settings.Default.ConnString != string.Empty)
					{
						using (OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.ConnString))
						{
							try
							{
								// test with an open attempt
								conn.Open();
								this.DialogResult = DialogResult.OK;
								this.Dispose();
							}
							catch (Exception ex)
							{
								// if the connection fails, inform the user
								// so they can fix the properties
								MessageBox.Show(ex.Message, "Connection Error");
							}
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error saving connection information.");
				}
			}
        }

        /// <summary>
        /// Test the Oracle Connection String
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOracleTest_Click(object sender, EventArgs e)
        {
            try
            {
				Properties.Settings.Default.NetworkAlias = tbNetworkAlias.Text;
				Properties.Settings.Default.ServerPort = tbPort.Text;
				Properties.Settings.Default.ServerName = tbOracleServerName.Text;
				Properties.Settings.Default.UserID = txtOracleUserID.Text;
				Properties.Settings.Default.Password = txtOraclePassword.Text;
				Properties.Settings.Default.SID = tbSID.Text;
				Properties.Settings.Default.ProviderString = tbOracleProvider.Text;
				Properties.Settings.Default.AttributeName = m_attributeName;

				if (tbSID.Text != "")
				{
					Properties.Settings.Default.ConnString = "Provider=OraOLEDB.Oracle;Data Source=(DESCRIPTION=(CID=GTU_APP)(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + tbOracleServerName.Text + ")(PORT=" + tbPort.Text + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + tbSID.Text + ")));User Id=" + txtOracleUserID.Text + ";Password=" + txtOraclePassword.Text + ";";
				}
				if (tbNetworkAlias.Text != "")
				{
					Properties.Settings.Default.ConnString = "Provider=OraOLEDB.Oracle;Data Source=(DESCRIPTION=(CID=GTU_APP)(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + tbOracleServerName.Text + ")(PORT=" + tbPort.Text + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + tbNetworkAlias.Text + ")));User Id=" + txtOracleUserID.Text + ";Password=" + txtOraclePassword.Text + ";";
				}

                // Save the property settings
                Properties.Settings.Default.Save();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error saving connection information.");
            }


            //Test Connection
            if (Properties.Settings.Default.ConnString != string.Empty)
            {
                using (OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.ConnString))
                {
                    try
                    {
                        // test the connection with an open attempt
                        conn.Open();
                        MessageBox.Show("Connection attempt successful.", "Connection Test");
                    }
                    catch (Exception ex)
                    {
                        // inform the user if the connection fails
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
        private void btnOracleCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }



        /// <summary>
        /// SQL Server 
        /// Configure for the use of integrated
        /// security
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxIntegratedSecurity_CheckedChanged(object sender, EventArgs e)
        {
            // if the user has checked the SQL Server connection
            // option to use integrated security, configure the
            //user ID and password controls accordingly

            if (cbxIntegratedSecurity.Checked == true)
            {
                txtSqlServerUserID.Text = string.Empty;
                txtSqlServerPassword.Text = string.Empty;

                txtSqlServerUserID.Enabled = false;
                txtSqlServerPassword.Enabled = false;
            }
            else
            {
                txtSqlServerUserID.Enabled = true;
                txtSqlServerPassword.Enabled = true;
            }
        }



        /// <summary>
        /// Close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSQLserverCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }



        /// <summary>
        /// Test the SQL Server connection string
        /// based upon the user supplied settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSqlServerTest_Click(object sender, EventArgs e)
        {
            try
            {
				Properties.Settings.Default.Password = txtSqlServerPassword.Text;
				Properties.Settings.Default.UserID = txtSqlServerUserID.Text;
				Properties.Settings.Default.ServerName = txtSqlServerName.Text;
				Properties.Settings.Default.DatabaseName = txtSqlServerInitialCat.Text;
				Properties.Settings.Default.ProviderString = txtSqlProviderString.Text;
				Properties.Settings.Default.AttributeName = m_attributeName;

				// configure the connection string based upon the use
				// of integrated security
				if (cbxIntegratedSecurity.Checked == true)
				{
				    Properties.Settings.Default.ConnString = 
				        "Provider=" + Properties.Settings.Default.ProviderString +
				        ";Data Source=" + Properties.Settings.Default.ServerName +
				        ";Initial Catalog=" + Properties.Settings.Default.DatabaseName +
				        ";Integrated Security=SSPI;";
				}
				else
				{
				    Properties.Settings.Default.ConnString = 
				        "Provider=" + Properties.Settings.Default.ProviderString +
				        ";Password=" + Properties.Settings.Default.Password +
				        ";User ID=" + Properties.Settings.Default.UserID +
				        ";Data Source=" + Properties.Settings.Default.ServerName +
				        ";Initial Catalog=" + Properties.Settings.Default.DatabaseName;
				}

                // Save the property settings
                Properties.Settings.Default.Save();

            }
            catch (Exception ex)
            {
                // inform the user if the connection was not saved
                MessageBox.Show(ex.Message, "Error saving connection information.");
            }

            //Test Connection
            if (Properties.Settings.Default.ConnString != string.Empty)
            {
                using (OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.ConnString))
                {
                    try
                    {
                        // test the connection with an open attempt
                        conn.Open();
                        MessageBox.Show("Connection Attempt Successful.", "Connection Test");
                    }
                    catch (Exception ex)
                    {
                        // inform the user if the connection test failed
                        MessageBox.Show(ex.Message, "Connection Test");
                    }
                }
            }
        }

		public string InsertCP
		{
			get
			{
				return m_cpInsert;
			}
		}

        /// <summary>
        /// Persist and test an SQL Server connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btnSqlServerOK_Click(object sender, EventArgs e)
		{
			// If the name of the connection is not left blank 
			//if (tbConnectionName.Text != "" && DBOp.GetConnectionParameters(tbConnectionName.Text) == null)
			//{
				m_connName = tbConnectionName.Text;
                //m_cpInsert = "INSERT INTO CONNECTION_PARAMETERS (CONNECTION_NAME, CONNECTION_ID, SERVER, PROVIDER, NATIVE_, DATABASE_NAME, SERVICE_NAME, SID, PORT, USERID, PASSWORD, INTEGRATED_SEC, VIEW_STATEMENT, IDENTIFIER) VALUES ('" +
                //    tbConnectionName.Text + "', " + "''" + ", '" + txtSqlServerName.Text + "', 'MSSQL', " + "'False'" + ", '" + txtSqlServerInitialCat.Text + "', " + "''" + ", " + "''" + ", " + "''" + ", '" + txtSqlServerUserID.Text + "', '" + txtSqlServerPassword.Text + "', '" + chkUseIntegratedSecurity.Checked + "', " + "''" + ", '" + cbSqlConnType.Text + "')";
				try
				{
					Properties.Settings.Default.Password = txtSqlServerPassword.Text;
					Properties.Settings.Default.UserID = txtSqlServerUserID.Text;
					Properties.Settings.Default.ServerName = txtSqlServerName.Text;
					Properties.Settings.Default.DatabaseName = txtSqlServerInitialCat.Text;
					Properties.Settings.Default.ProviderString = txtSqlProviderString.Text;
					Properties.Settings.Default.AttributeName = m_attributeName;
					Properties.Settings.Default.IntegratedSecurity = m_bIntegratedSecurity;
					Properties.Settings.Default.ProviderText = "MSSQL";

					// configure the connection string based upon the use
					// of integrated security
					if (cbxIntegratedSecurity.Checked == true)
					{
						Properties.Settings.Default.ConnString =
							"Provider=" + Properties.Settings.Default.ProviderString +
							";Data Source=" + Properties.Settings.Default.ServerName +
							";Initial Catalog=" + Properties.Settings.Default.DatabaseName +
							";Integrated Security=SSPI;";
					}
					else
					{
						Properties.Settings.Default.ConnString =
							"Provider=" + Properties.Settings.Default.ProviderString +
							";Password=" + Properties.Settings.Default.Password +
							";User ID=" + Properties.Settings.Default.UserID +
							";Data Source=" + Properties.Settings.Default.ServerName +
							";Initial Catalog=" + Properties.Settings.Default.DatabaseName;
					}

					// Save the property settings
					Properties.Settings.Default.Save();
					if (Properties.Settings.Default.ConnString != string.Empty)
					{
						using (OleDbConnection conn = new OleDbConnection(Properties.Settings.Default.ConnString))
						{
							try
							{
								// test the connection with an open attempt
								conn.Open();
								//this.Dispose();
							}
							catch (Exception ex)
							{
								// inform the user if the connection was not saved
								MessageBox.Show(ex.Message, "Connection Test");
							}
						}
					}
					this.DialogResult = DialogResult.OK;
				}
				catch (Exception ex)
				{
					// inform the user if the connection information was not saved
					MessageBox.Show(ex.Message, "Error saving connection information.");
				}
			//}
			//else
			//{
			//    MessageBox.Show("Connection name already exists, please choose another.");
			//}
		}

		private void chkUseIntegratedSecurity_CheckedChanged(object sender, EventArgs e)
		{
			if (chkUseIntegratedSecurity.Checked)
			{
				txtOracleUserID.ReadOnly = true;
				txtOraclePassword.ReadOnly = true;

				txtSqlServerUserID.ReadOnly = true;
				txtSqlServerPassword.ReadOnly = true;
			}
			else
			{
				txtOracleUserID.ReadOnly = false;
				txtOraclePassword.ReadOnly = false;

				txtSqlServerUserID.ReadOnly = false;
				txtSqlServerPassword.ReadOnly = false;
			}
		}
    }
}