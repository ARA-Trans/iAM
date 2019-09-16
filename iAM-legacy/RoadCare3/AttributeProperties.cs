using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using OrderedPropertyGrid;
using PropertyGridEx;
using System.Data.SqlClient;
using System.Collections;

using System.Data;
using DatabaseManager;
using Microsoft.SqlServer.Management.Smo;
using DBSpy.Properties;
using System.Windows.Forms;
using DBSpy;
using System.Text.RegularExpressions;
using RoadCareSecurityOperations;

namespace RoadCare3
{
    public class AttributeProperties
    {
        private List<String> m_strListAttributes = new List<String>();
        string[] providers = new string[] { "MSSQL", "ORACLE"};
        string[] types = new string[] { "STRING", "NUMBER" };
        string[] trueFalse = new string[] { "True", "False" };
        string[] connType = new string[] { "Network Alias", "Basic" };
        String m_strPropertyName;

        private String m_strDatabase;
        private String m_strServer;
        private String m_strUserName;
        private String m_strPassword;
        private String m_strAttributeName;
		private PropertyGridEx.PropertyGridEx m_pgProperties;
		private string viewSelectStatement;
        private string _netDefName;

        public string NetDefName
        {
            get { return _netDefName; }
        }

        #region Attributes

        public String Attribute
        {
            get { return m_strAttributeName; }
            set { m_strAttributeName = value.ToUpper(); }
        }

        public String Server
        {
            get { return m_strServer; }
            set { m_strServer = value; }
        }

        public String Database
        {
            get { return m_strDatabase; }
            set { m_strDatabase = value; }
        }

        public String Login
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }

        public String Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }

        #endregion

        /// <summary>
        /// Used for creating new attributes
        /// </summary>
        public AttributeProperties()
        {
             
        }

        /// <summary>
        /// For properties that already exist in the database use this constructor to view
        /// the attribute properties as a tool window.
        /// </summary>
        /// <param name="strPropertyName"></param>
        public AttributeProperties(String strPropertyName)
        {
            m_strPropertyName = strPropertyName;
        }

		public void SetAttributeProperties(PropertyGridEx.PropertyGridEx pg)
		{
			// Initialize the property grid for attributes.
			bool bReadOnly = true;
			if (pg.Dock == DockStyle.None)
			{
				bReadOnly = false;
			}

			pg.Item.Clear();

			bool dataReadOnly = true;
			if( string.IsNullOrEmpty( m_strPropertyName ) || Global.SecurityOperations.CanModifyRawAttributeData( m_strPropertyName ) )
			{
				dataReadOnly = false;
			}

			pg.Item.Add( "Attribute", "AttributeName" + RoadCare3.Properties.Settings.Default.ATTRIBUTE_DEFAULT.ToString(), bReadOnly, "1. Attribute Information", "Data Attribute.", true );

            pg.Item.Add("Native", true, bReadOnly, "3. Database Information", "Native should be set to true if the database is a native RoadCare database.", true);
            pg.Item[pg.Item.Count - 1].Choices = new CustomChoices(trueFalse, false);

            pg.Item.Add("Provider", RoadCare3.Properties.Settings.Default.LAST_DB_TYPE, true, "3. Database Information", "Database Type (MSSQL, ORACLE, MYSQL).", true);
            pg.Item[pg.Item.Count - 1].Choices = new CustomChoices(providers, false);

            pg.Item.Add("Integrated Security", true, true, "3. Database Information", "", true);

            pg.Item.Add("Type", RoadCare3.Properties.Settings.Default.TYPE_DEFAULT, bReadOnly, "1. Attribute Information", "STRING or NUMBER", true);
            pg.Item[pg.Item.Count - 1].Choices = new CustomChoices(types, false);

            pg.Item.Add("Ascending", true, dataReadOnly, "1. Attribute Information", "True if the number gets smaller with deterioration.", true);
            pg.Item[pg.Item.Count - 1].Choices = new CustomChoices(trueFalse, false);

            pg.Item.Add("Connection Type", RoadCare3.Properties.Settings.Default.CONNECTION_TYPE, true, "5. Oracle Database Information", "Oracle Basic or TNS connection type.", true);
            pg.Item[pg.Item.Count - 1].Choices = new CustomChoices(connType, false);

            pg.Item.Add("Server", RoadCare3.Properties.Settings.Default.LAST_DB_SERVER, true, "3. Database Information", "Name of server on which the attribute data resides.", true);
			pg.Item.Add("Database", RoadCare3.Properties.Settings.Default.LAST_DB.ToString(), true, "4. MSSQL Database Information", "Name of the database in which the attribute data resides.", true);
            pg.Item.Add("Network Alias", RoadCare3.Properties.Settings.Default.DBNETWORKALIAS, true, "5. Oracle Database Information", "Name of the Oracle Service", true);
            pg.Item.Add("SID", RoadCare3.Properties.Settings.Default.DBSID, true, "5. Oracle Database Information", "", true);
            pg.Item.Add("Port", RoadCare3.Properties.Settings.Default.DBPORT, true, "5. Oracle Database Information", "", true);
            pg.Item.Add("Login", RoadCare3.Properties.Settings.Default.LAST_LOGIN.ToString(), true, "3. Database Information", "User name.", true);
            pg.Item.Add("Password", RoadCare3.Properties.Settings.Default.LAST_PASSWORD.ToString(), true, "3. Database Information", "User password", true);
            pg.Item["Password"].IsPassword = true;

			pg.Item.Add("View", "", true, "6. View Statement", "Sql string for generating RoadCare specific views on the data server.", true);
			pg.Item[pg.Item.Count - 1].OnClick += this.CustomEventItem_OnClick;

            pg.Item.Add("Grouping", RoadCare3.Properties.Settings.Default.LAST_GROUP.ToString(), dataReadOnly, "1. Attribute Information", "Used to group attributes of similar types.", true);
            pg.Item.Add("Format", RoadCare3.Properties.Settings.Default.LAST_FORMAT.ToString(), dataReadOnly, "1. Attribute Information", "Determines how numeric data appears in the attribute view. (e.g. f1 = 1.0 f2 = 1.00 etc.)", true);
            pg.Item.Add("Maximum", RoadCare3.Properties.Settings.Default.LAST_MAXIMUM.ToString(), dataReadOnly, "1. Attribute Information", "Maximum allowed data value", true);
            pg.Item.Add("Minimum", RoadCare3.Properties.Settings.Default.LAST_MINIMUM.ToString(), dataReadOnly, "1. Attribute Information", "Minimum allowed data value", true);
            pg.Item.Add("Default_Value", RoadCare3.Properties.Settings.Default.LAST_DEFAULT.ToString(), dataReadOnly, "1. Attribute Information", "The default data value to use if no data is present.", true);
			pg.Item.Add("One", RoadCare3.Properties.Settings.Default.LAST_ONE.ToString(), dataReadOnly, "2. Levels", "Defined level breaks and coloring schemes.", true);
            pg.Item.Add("Two", RoadCare3.Properties.Settings.Default.LAST_TWO.ToString(), dataReadOnly, "2. Levels", "Defined level breaks and coloring schemes.", true);
            pg.Item.Add("Three", RoadCare3.Properties.Settings.Default.LAST_THREE.ToString(), dataReadOnly, "2. Levels", "Defined level breaks and coloring schemes.", true);
            pg.Item.Add("Four", RoadCare3.Properties.Settings.Default.LAST_FOUR.ToString(), dataReadOnly, "2. Levels", "Defined level breaks and coloring schemes.", true);
            pg.Item.Add("Five", RoadCare3.Properties.Settings.Default.LAST_FIVE.ToString(), dataReadOnly, "2. Levels", "Defined level breaks and coloring schemes.", true);

            SolutionExplorerTreeNode selectedNode = (SolutionExplorerTreeNode)FormManager.GetSolutionExplorer().GetTreeView().SelectedNode;
            pg.Item.Add("Network Definition Name", selectedNode.NetworkDefinition.NetDefName, true, "1. Attribute Information", "Network definition to which the attribute belongs.", true);
			m_pgProperties = pg;
		}

        public String SavePropertiesToDatabase()
        {
			if( IsPropertyGridComplete() )
			{
				RoadCare3.Properties.Settings.Default.LAST_DB_TYPE = m_pgProperties.Item["Provider"].Value.ToString();
				RoadCare3.Properties.Settings.Default.LAST_DB_SERVER = m_pgProperties.Item["Server"].Value.ToString();
				RoadCare3.Properties.Settings.Default.LAST_DB = m_pgProperties.Item["Database"].Value.ToString();
				RoadCare3.Properties.Settings.Default.LAST_ONE = m_pgProperties.Item["One"].Value.ToString();
				RoadCare3.Properties.Settings.Default.LAST_TWO = m_pgProperties.Item["Two"].Value.ToString();
				RoadCare3.Properties.Settings.Default.LAST_THREE = m_pgProperties.Item["Three"].Value.ToString();
				RoadCare3.Properties.Settings.Default.LAST_FOUR = m_pgProperties.Item["Four"].Value.ToString();
				RoadCare3.Properties.Settings.Default.LAST_FIVE = m_pgProperties.Item["Five"].Value.ToString();
				RoadCare3.Properties.Settings.Default.LAST_GROUP = m_pgProperties.Item["Grouping"].Value.ToString();
				RoadCare3.Properties.Settings.Default.LAST_DEFAULT = m_pgProperties.Item["Default_Value"].Value.ToString();
				RoadCare3.Properties.Settings.Default.LAST_FORMAT = m_pgProperties.Item["Format"].Value.ToString();
				RoadCare3.Properties.Settings.Default.LAST_MINIMUM = m_pgProperties.Item["Minimum"].Value.ToString();
				RoadCare3.Properties.Settings.Default.LAST_MAXIMUM = m_pgProperties.Item["Maximum"].Value.ToString();
				RoadCare3.Properties.Settings.Default.LAST_LOGIN = m_pgProperties.Item["Login"].Value.ToString();
				RoadCare3.Properties.Settings.Default.LAST_PASSWORD = m_pgProperties.Item["Password"].Value.ToString();
                RoadCare3.Properties.Settings.Default.DBNETWORKALIAS = m_pgProperties.Item["Network Alias"].Value.ToString();
                RoadCare3.Properties.Settings.Default.CONNECTION_TYPE = m_pgProperties.Item["Connection Type"].Value.ToString();
                RoadCare3.Properties.Settings.Default.NETWORK_DEFINITION_NAME = m_pgProperties.Item["Network Definition Name"].Value.ToString();
				RoadCare3.Properties.Settings.Default.Save();

				m_strAttributeName = m_pgProperties.Item["Attribute"].Value.ToString().ToUpper();
                _netDefName = m_pgProperties.Item["Network Definition Name"].Value.ToString();
				if (m_pgProperties.Item["Native"].Value.ToString().ToLower() == "false")
				{
					string createViewStatement = "CREATE VIEW ";
					switch (m_pgProperties.Item["Provider"].Value.ToString())
					{
						case "MSSQL":
                            createViewStatement += "[" + m_strAttributeName + "]" + " AS " + viewSelectStatement;
							break;
						case "ORACLE":
                            createViewStatement += "\"" + m_strAttributeName.ToUpper() + "\" AS " + viewSelectStatement;
							break;
						default:
							throw new NotImplementedException("TODO: Create ANSI implementation for SavePropertiesToDatabase()");
							//break;
					}
					for (int i = 0; i < m_pgProperties.Item.Count; i++)
					{
						if ( m_pgProperties.Item[i].Value == null)
						{
							m_pgProperties.Item[i].Value = "";
						}
					}

					DBMgr.ExecuteNonQuery(createViewStatement, new ConnectionParameters(
							m_pgProperties.Item["Port"].Value.ToString(),
							m_pgProperties.Item["SID"].Value.ToString(),
							m_pgProperties.Item["Network Alias"].Value.ToString(),
							m_pgProperties.Item["Login"].Value.ToString(),
							m_pgProperties.Item["Password"].Value.ToString(),
							Convert.ToBoolean(m_pgProperties.Item["Integrated Security"].Value),
							m_pgProperties.Item["Server"].Value.ToString(),
							m_pgProperties.Item["Database"].Value.ToString(),
							"", "", "", "",
							m_pgProperties.Item["Provider"].Value.ToString(),
							false));
				}
				String strInsert = "INSERT INTO ATTRIBUTES_ (ATTRIBUTE_, ";
				String strValues = " VALUES ('" + m_strAttributeName.ToUpper() + "', '";

				for( int i = 1; i < m_pgProperties.Item.Count; i++ )
				{
					String strDatabaseAttribteName = ( String )Global.m_htFieldMapping[m_pgProperties.Item[i].Name];
					if( m_pgProperties.Item[i].Value.ToString() != "" )
					{
						switch (DBMgr.NativeConnectionParameters.Provider)
						{
							case "MSSQL":
								switch (strDatabaseAttribteName.ToUpper())
								{
									case "GROUPING":
										strInsert += "GROUPING, ";
										break;
									case "TYPE":
										strInsert += "TYPE_, ";
										break;
									case "NATIVE":
										strInsert += "NATIVE_, ";
										break;
									case "MINIMUM":
										strInsert += "MINIMUM_, ";
										break;
									case "PASSWORD":
										strInsert += "PASSWORD_, ";
										break;
									default:
										strInsert += "[" + strDatabaseAttribteName + "], ";
										break;
								}
								strValues += m_pgProperties.Item[i].Value + "', '";
								break;
							case "ORACLE":
								switch (strDatabaseAttribteName.ToUpper())
								{
									case "GROUPING":
										strInsert += "GROUPING, ";
										break;
									case "TYPE":
										strInsert += "TYPE_, ";
										break;
									case "NATIVE":
										strInsert += "NATIVE_, ";
										break;
									case "MINIMUM":
										strInsert += "MINIMUM_, ";
										break;
									case "PASSWORD":
										strInsert += "PASSWORD_, ";
										break;
									default:
										strInsert += "\"" + strDatabaseAttribteName + "\", ";
										break;
								}
								if (m_pgProperties.Item[i].Value.ToString().ToLower() != "true")
								{
									if (m_pgProperties.Item[i].Value.ToString().ToLower() != "false")
									{
										strValues += m_pgProperties.Item[i].Value.ToString().Trim() + "', '";
									}
									else
									{
										strValues += "0', '";
									}
								}
								else
								{
									strValues += "1', '";
								}
								break;
							default:
								throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
								//break;
						}
					}
				}

				strInsert = strInsert.Substring( 0, strInsert.Length - 2 ) + ")";
				strValues = strValues.Substring( 0, strValues.Length - 3 );
				strInsert += strValues + ")";

				try
				{
					DBMgr.ExecuteNonQuery( strInsert );
				}
				catch( Exception sqlE )
				{
					Global.WriteOutput( "Error: Insert new attribute into database failed. " + sqlE.Message );
				}

				if( bool.Parse( m_pgProperties.Item[1].Value.ToString() ) == true )
				{
					// Now create the attribute table
					List<TableParameters> listColumn = new List<TableParameters>();
					listColumn.Add( new TableParameters( "ID_", DataType.Int, false, true, true ) );
					listColumn.Add( new TableParameters( "ROUTES", DataType.VarChar( 4000 ), true ) );
					listColumn.Add( new TableParameters( "BEGIN_STATION", DataType.Float, true ) );
					listColumn.Add( new TableParameters( "END_STATION", DataType.Float, true ) );
					listColumn.Add( new TableParameters( "DIRECTION", DataType.VarChar( 50 ), true ) );
					listColumn.Add( new TableParameters( "FACILITY", DataType.VarChar( 4000 ), true ) );
					listColumn.Add( new TableParameters( "SECTION", DataType.VarChar( 4000 ), true ) );
					listColumn.Add( new TableParameters( "SAMPLE_", DataType.VarChar( 50 ), true ) );
					listColumn.Add( new TableParameters( "DATE_", DataType.DateTime, true ) );
					if( m_pgProperties.Item["Type"].Value.ToString() == "NUMBER" )
					{
						listColumn.Add( new TableParameters( "DATA_", DataType.Float, true ) );
					}
					else
					{
						listColumn.Add( new TableParameters( "DATA_", DataType.VarChar( 200 ), true ) );
					}
					try
					{
						DBMgr.CreateTable( m_pgProperties.Item[0].Value.ToString(), listColumn );
						Global.SecurityOperations.AddAction("ATTRIBUTE", m_strAttributeName, "", "");
						Global.SecurityOperations.SetUserActionPermissions(Global.SecurityOperations.CurrentUser, Global.SecurityOperations.GetAction("ATTRIBUTE", m_strAttributeName, "", ""), "CreateDestroy");
						Global.SecurityOperations.AddActionToGroup( Global.SecurityOperations.GetAction( "ATTRIBUTE", m_strAttributeName, "", "" ), "ALL_ACTIONS" );
					}
					catch( Exception exc )
					{
						Global.WriteOutput( "Error creating attribute: " + exc.Message );
						return null;
					}
				}
			}
            return m_strAttributeName;
        }

        private bool IsPropertyGridComplete()
        {
            // Check the property grid for valid entries...
            bool bIsComplete = true;
			Regex regExAttributeName = new Regex("[^0-9a-zA-Z_]");
			if (regExAttributeName.IsMatch(m_pgProperties.Item["Attribute"].Value.ToString()))
			{
				bIsComplete = false;
				Global.WriteOutput("Error: Illegal characters detected in attribute name.  Please limit attribute names to contain only letters, the underbar character, and numbers.");
			}

			if (m_pgProperties.Item["Attribute"].Value.ToString() == "")
			{
				bIsComplete = false;
				Global.WriteOutput( "Error: Attribute name cannot be blank." );
			}

			if (m_pgProperties.Item["Native"].Value.ToString() == "False")
			{
				if (m_pgProperties.Item["Provider"].Value != null)
				{
					switch (m_pgProperties.Item["Provider"].Value.ToString())
					{
						case "MSSQL":
							if (m_pgProperties.Item["Server"].Value.ToString() == "")
							{
								bIsComplete = false;
								Global.WriteOutput("Error: Server value cannot be blank for non-native attributes.");
							}

							if (m_pgProperties.Item["Database"].Value.ToString() == "")
							{
								bIsComplete = false;
								Global.WriteOutput("Error: Database value cannot be blank for non-native attributes.");
							}
							break;
						case "ORACLE":
							if (m_pgProperties.Item["Network Alias"].Value.ToString() == "" && m_pgProperties.Item["SID"].Value.ToString() == "")
							{
								bIsComplete = false;
								Global.WriteOutput("Error: Either Service Name or SID must be completed. ");
							}
							if (m_pgProperties.Item["Port"].Value.ToString() == "")
							{
								bIsComplete = false;
								Global.WriteOutput("Error: Port cannot be blank. ");
							}
							break;
					}
					if (m_pgProperties.Item["Server"].Value.ToString() == "")
					{
						bIsComplete = false;
						Global.WriteOutput("Error: Server cannot be blank. ");
					}
 
					
				}
				if (m_pgProperties.Item["View"].Value.ToString() == "")
				{
					bIsComplete = false;
					Global.WriteOutput("Error: View value cannot be blank for non-native attributes.");
				}
				else
				{
					// If we have a good SQL statement value, then we need to account for possible concatenation quotes 
					// in the SQL string.
					m_pgProperties.Item["View"].Value = m_pgProperties.Item["View"].Value.ToString().Replace("'", "''");
				}
			}


			if( m_pgProperties.Item["Type"].Value != null )
			{
				if( m_pgProperties.Item["Type"].Value.ToString() == "NUMBER" )
				{
					if( m_pgProperties.Item["Default_Value"].Value.ToString() == "" )
					{
						bIsComplete = false;
						Global.WriteOutput( "Error: Default value cannot be blank for numeric attributes." );
					}
					if( m_pgProperties.Item["Format"].Value.ToString() == "" )
					{
						bIsComplete = false;
						Global.WriteOutput( "Error: A format must be specified for numeric attributes." );
					}
				}
			}
			else
			{
				bIsComplete = false;
				Global.WriteOutput( "Error: A type must be specified for the attribute." );
			}
            return bIsComplete;
        }

        /// <summary>
        /// Updates the database each time a user completes a row in a property grid.
        /// </summary>
        public void UpdateDatabase(String strProperty, String strValue)
        {
            String strUpdate;
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					if (strValue.Trim() == "" || strValue.Trim().ToUpper() == "NULL")
					{
						strUpdate = "Update ATTRIBUTES_ Set " + Global.m_htFieldMapping[strProperty].ToString() + " = NULL Where ATTRIBUTE_ = '" + m_strPropertyName + "'";
					}
					else
					{
						strUpdate = "Update ATTRIBUTES_ Set " + Global.m_htFieldMapping[strProperty].ToString() + " = '" + strValue + "' Where ATTRIBUTE_ = '" + m_strPropertyName + "'";
					}
					break;
				case "ORACLE":
						if( strValue.Trim() == "" || strValue.Trim().ToUpper() == "NULL" )
						{
							strUpdate = "Update ATTRIBUTES_ Set \"" + Global.m_htFieldMapping[strProperty].ToString() + "\" = NULL Where ATTRIBUTE_ = '" + m_strPropertyName + "'";
						}
						else
						{
							if( strProperty.Trim().ToUpper() != "ASCENDING" )
							{
								strUpdate = "Update ATTRIBUTES_ Set \"" + Global.m_htFieldMapping[strProperty].ToString() + "\" = '" + strValue + "' Where ATTRIBUTE_ = '" + m_strPropertyName + "'";
							}
							else
							{
								if( strValue.Trim().ToUpper() == "TRUE" )
								{
									strUpdate = "Update ATTRIBUTES_ Set \"" + Global.m_htFieldMapping[strProperty].ToString() + "\" = '1' Where ATTRIBUTE_ = '" + m_strPropertyName + "'";
								}
								else if( strValue.Trim().ToUpper() == "FALSE" )
								{
									strUpdate = "Update ATTRIBUTES_ Set \"" + Global.m_htFieldMapping[strProperty].ToString() + "\" = '0' Where ATTRIBUTE_ = '" + m_strPropertyName + "'";
								}
								else
								{
									throw new Exception( "Ascending set to unknown value." );
								}
							}
						}
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
					//break;
			}
			try
            {
                DBMgr.ExecuteNonQuery(strUpdate);
            }
            catch (Exception sqlE)
            {
                Global.WriteOutput("Error: Unable to update ATTRIBUTES_ table with new properties. " + sqlE.Message);
            }
        }

        private object CustomEventItem_OnClick(object sender, EventArgs e)
        {
			// open an instance the connect form so the user
			// can define a new connection
			String server = "";
			String database = "";
			String userName = "";
			String password = "";
			String attributeName = "";
			String providerName = "";
			bool bIntegratedSecurity = true;
			string serviceName = "";
			string SID = "";
			string port = "";

			if (m_pgProperties.Item["Server"] != null)
			{
				server = m_pgProperties.Item["Server"].Value.ToString();
			}
			if (m_pgProperties.Item["Database"] != null)
			{
				database = m_pgProperties.Item["Database"].Value.ToString();
			}
			if (m_pgProperties.Item["Login"] != null)
			{
				userName = m_pgProperties.Item["Login"].Value.ToString();
			}
			if (m_pgProperties.Item["Password"] != null)
			{
				password = m_pgProperties.Item["Password"].Value.ToString();
			}
			if (m_pgProperties.Item["Attribute"] != null)
			{
				attributeName = m_pgProperties.Item["Attribute"].Value.ToString();
			}
			if (m_pgProperties.Item["Provider"] != null)
			{
				providerName = m_pgProperties.Item["Provider"].Value.ToString();
			}
			if (m_pgProperties.Item["Integrated Security"] != null)
			{
				bIntegratedSecurity = Convert.ToBoolean(m_pgProperties.Item["Integrated Security"].Value.ToString());
			}
			if (m_pgProperties.Item["Service Name"] != null)
			{
				serviceName = m_pgProperties.Item["Service Name"].Value.ToString();
			}
			if (m_pgProperties.Item["SID"] != null)
			{
				SID = m_pgProperties.Item["SID"].Value.ToString();
			}
			if (m_pgProperties.Item["Port"] != null)
			{
				port = m_pgProperties.Item["Port"].Value.ToString();
			}
			frmConnect f = new frmConnect(server, database, bIntegratedSecurity, SID, serviceName, port, userName, password, providerName, attributeName);
			if (f.ShowDialog() == DialogResult.OK)
			{
				f.Close();
                SolutionExplorerTreeNode selectedNode = (SolutionExplorerTreeNode)FormManager.GetSolutionExplorer().GetTreeView().SelectedNode;
                NetworkDefinition currNetDef = selectedNode.NetworkDefinition;
				frmDataDisplay SQLDataView = new frmDataDisplay(server, database, userName, password, attributeName, providerName, currNetDef.NetDefName);
				if (SQLDataView.ShowDialog() == DialogResult.OK)
				{
					String view = SQLDataView.View;
					viewSelectStatement = SQLDataView.ViewSelectStatement;
					SQLDataView.Close();
					return viewSelectStatement;
				}
			}
			return "";
        }

        public void UpdatePropertyGrid(PropertyGridEx.PropertyGridEx pgProperties)
        {
            // Get Attribute information from the database regarding this attribute.
			String strQuery = "";
            string netDefName = pgProperties.Item["Network Definition Name"].Value.ToString();
			strQuery = "Select ATTRIBUTE_, NATIVE_, PROVIDER, SERVER, DATASOURCE, USERID, PASSWORD_, SQLVIEW, TYPE_, GROUPING, ASCENDING, FORMAT, MAXIMUM, MINIMUM_, DEFAULT_VALUE, LEVEL1, LEVEL2, LEVEL3, LEVEL4, LEVEL5, SID_, SERVICE_NAME, INTEGRATED_SECURITY, PORT, NETWORK_DEFINITION_NAME From ATTRIBUTES_ Where ATTRIBUTE_ = '" + m_strPropertyName + "' AND NETWORK_DEFINITION_NAME = '" + netDefName + "'";
			try
            {
                DataSet ds = DBMgr.ExecuteQuery(strQuery);

                // More random error checking
                if (ds.Tables[0].Rows.Count != 1)
                {
                    throw (new Exception());
                }
                else
                {
					DataRow dr = ds.Tables[0].Rows[0];
					pgProperties.Item["Attribute"].Value = dr["ATTRIBUTE_"].ToString();
					pgProperties.Item["Native"].Value = dr["NATIVE_"].ToString();
					pgProperties.Item["Provider"].Value = dr["PROVIDER"].ToString();
					pgProperties.Item["Server"].Value = dr["SERVER"].ToString();
					pgProperties.Item["Database"].Value = dr["DATASOURCE"].ToString();
					pgProperties.Item["Login"].Value = dr["USERID"].ToString();
					pgProperties.Item["Password"].Value = dr["PASSWORD_"].ToString();
					pgProperties.Item["View"].Value = dr["SQLVIEW"].ToString();
					pgProperties.Item["Type"].Value = dr["TYPE_"].ToString();
					pgProperties.Item["Grouping"].Value = dr["GROUPING"].ToString();
					pgProperties.Item["Ascending"].Value = dr["ASCENDING"].ToString();
					pgProperties.Item["Format"].Value = dr["FORMAT"].ToString();
					pgProperties.Item["Maximum"].Value = dr["MAXIMUM"].ToString();
					pgProperties.Item["Minimum"].Value = dr["MINIMUM_"].ToString();
					pgProperties.Item["Default_Value"].Value = dr["DEFAULT_VALUE"].ToString();
					pgProperties.Item["One"].Value = dr["LEVEL1"].ToString();
					pgProperties.Item["Two"].Value = dr["LEVEL2"].ToString();
					pgProperties.Item["Three"].Value = dr["LEVEL3"].ToString();
					pgProperties.Item["Four"].Value = dr["LEVEL4"].ToString();
					pgProperties.Item["Five"].Value = dr["LEVEL5"].ToString();
					pgProperties.Item["SID"].Value = dr["SID_"].ToString();
					pgProperties.Item["Network Alias"].Value = dr["SERVICE_NAME"].ToString();
					pgProperties.Item["Integrated Security"].Value = dr["INTEGRATED_SECURITY"].ToString();
					pgProperties.Item["Port"].Value = dr["PORT"].ToString();
                    pgProperties.Item["Network Definition Name"].Value = dr["NETWORK_DEFINITION_NAME"].ToString();
                    pgProperties.Refresh();
                }
            }
            catch (Exception sqlE)
            {
                Global.WriteOutput("Error: Cannot update property tool window.  " + sqlE.Message);
            }
        }
    }
}
