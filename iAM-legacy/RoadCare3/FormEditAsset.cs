using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PropertyGridEx;
using DBSpy;
using DatabaseManager;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;

namespace RoadCare3
{
	public partial class FormEditAsset : Form
	{
		private string[] providers = new string[] { "MSSQL", "ORACLE", "MYSQL", "ACCESS", "DBF" };
		private string m_assetName = "";
		private bool m_bCreateNew = true;
		private Dictionary<string, string> m_columnsToAdd = new Dictionary<string, string>();
		private List<string> m_columnsToRemove = new List<string>();
		//private AssetProperties assetProperties;

		public FormEditAsset()
		{
			InitializeComponent();
		}

		public FormEditAsset(string assetName, bool bCreateNew)
		{
			InitializeComponent();
			m_assetName = assetName;
			m_bCreateNew = bCreateNew;
		}

		private void FormEditAsset_Load(object sender, EventArgs e)
		{
			if (m_bCreateNew)
			{
				LoadConnectionPropertyGrid();
			}
			else
			{
				cbIsNative.Enabled = false;
				EditConnectionPropertyGrid();
				if (cbRemoveProperty.Items.Contains("ID"))
				{
					cbRemoveProperty.Items.Remove("ID");
				}
				if (cbRemoveProperty.Items.Contains("GEO_ID"))
				{
					cbRemoveProperty.Items.Remove("GEO_ID");
				}
				if (cbRemoveProperty.Items.Contains("ENTRY_DATE"))
				{
					cbRemoveProperty.Items.Remove("ENTRY_DATE");
				}
			}
		}

		private void LoadConnectionPropertyGrid()
		{
			pgProperties.Item.Add("Provider", "", true, "Database Information", "Database Type (MSSQL, ORACLE, MYSQL).", true);
			pgProperties.Item[pgProperties.Item.Count - 1].Choices = new CustomChoices(providers, false);

			pgProperties.Item.Add("Server", "", true, "Database Information", "Name of server on which the attribute data resides.", true);
			pgProperties.Item.Add("Database", "", true, "Database Information", "Name of the database in which the attribute data resides.", true);

			pgProperties.Item.Add("Use Integrated Security", true, true, "Database Information", "True to use windows authentication.", true);

			pgProperties.Item.Add("User Name", "", true, "Database Information", "User name.", true);
			pgProperties.Item.Add("Password", "", true, "Database Information", "User password", true);
			pgProperties.Item.Add("View", "", true, "Database Information", "SQL statement used to import/view remote data", true);

			pgProperties.Item["Provider"].Value = RoadCare3.Properties.Settings.Default.PROVIDER_DIU;
			pgProperties.Item["Server"].Value = RoadCare3.Properties.Settings.Default.SERVER_DIU;
			pgProperties.Item["Database"].Value = RoadCare3.Properties.Settings.Default.DATABASE_DIU;
			pgProperties.Item["User Name"].Value = RoadCare3.Properties.Settings.Default.USERNAME_DIU;

			pgProperties.Item["View"].OnClick += this.CustomEventItem_OnClick;
			pgProperties.Refresh();
		}

		private void EditConnectionPropertyGrid()
		{
			pgProperties.Item["Asset Name"].IsReadOnly = true;

			// Get all the columns from the database for this asset that are not
			// and load them into the property grid if they are not Database Information columns
			ConnectionParameters cp = DBMgr.GetAssetConnectionObject(m_assetName);
			DataSet assetColumnsAndTypes = DBMgr.GetTableColumnsWithTypes(m_assetName, cp);
			foreach (DataRow columnAndType in assetColumnsAndTypes.Tables[0].Rows)
			{
				if (pgProperties.Item[columnAndType["column_name"].ToString()] == null)
				{
					pgProperties.Item.Add(columnAndType["column_name"].ToString(), columnAndType["data_type"].ToString(), true, "Asset Properties", "", true);
					if (!cbRemoveProperty.Items.Contains(columnAndType["column_name"].ToString()))
					{
						cbRemoveProperty.Items.Add(columnAndType["column_name"].ToString());
					}
				}
			}

			// Remove ID and entry date fields
			if (pgProperties.Item["ID"] != null)
			{
				pgProperties.Item.Remove("ID");
			}
			if (pgProperties.Item["GEO_ID"] != null)
			{
				pgProperties.Item.Remove("GEO_ID");
			}
			if (pgProperties.Item["ENTRY_DATE"] != null)
			{
				pgProperties.Item.Remove("ENTRY_DATE");
			}
			pgProperties.Refresh();
		}

		private object CustomEventItem_OnClick(object sender, EventArgs e)
		{
			// open an instance the connect form so the user
            // can define a new connection
			String server = pgProperties.Item["Server"].Value.ToString();
			String database = pgProperties.Item["Database"].Value.ToString();
			String userName = pgProperties.Item["User Name"].Value.ToString();
			String password = pgProperties.Item["Password"].Value.ToString();
			String assetName = pgProperties.Item["Asset Name"].Value.ToString();
			String providerName = pgProperties.Item["Provider"].Value.ToString();

            //frmConnect f = new frmConnect(server, database, userName, password, assetName);
			//frmConnect f = new frmConnect(server, database, true, "", "", "", userName, password, providerName, assetName);
			//frmConnect f = null;
			throw new NotImplementedException("Refactor at exception.");
        //    if (f.ShowDialog() == DialogResult.OK)
        //    {
        //        frmDataDisplay SQLDataView = new frmDataDisplay(server,database,userName,password,assetName,providerName);
        //        if (SQLDataView.ShowDialog() == DialogResult.OK)
        //        {
        //            String view = SQLDataView.View;
        //            SQLDataView.Close();
        //            return view;
        //        }
        //    }
        //    return "";
        }

		private void cbIsNative_CheckedChanged(object sender, EventArgs e)
		{
			if (cbIsNative.Checked)
			{
				pgProperties.Item["Server"].IsReadOnly = true;
				pgProperties.Item["Provider"].IsReadOnly = true;
				pgProperties.Item["User Name"].IsReadOnly = true;
				pgProperties.Item["Password"].IsReadOnly = true;
				pgProperties.Item["Database"].IsReadOnly = true;
				pgProperties.Item["View"].IsReadOnly = true;
				pgProperties.Item["Use Integrated Security"].IsReadOnly = true;
			}
			else
			{
				pgProperties.Item["Server"].IsReadOnly = false;
				pgProperties.Item["Provider"].IsReadOnly = false;
				pgProperties.Item["Use Integrated Security"].IsReadOnly = false;
				if (!bool.Parse(pgProperties.Item["Use Integrated Security"].Value.ToString()))
				{
					pgProperties.Item["User Name"].IsReadOnly = false;
					pgProperties.Item["Password"].IsReadOnly = false;
				}
				pgProperties.Item["Database"].IsReadOnly = false;
				pgProperties.Item["View"].IsReadOnly = false;
			}
			pgProperties.Refresh();
		}

		private void cmdAdd_Click(object sender, EventArgs e)
		{
			String propertyName = txtName.Text;
			if (pgProperties.Item[propertyName] == null)
			{
				bool bIsReadOnly = true;
				bool bIsVisible = true;
				String propertyType = cbPropertyType.Text;

				PropertyGridEx.CustomProperty propertyToAdd = new PropertyGridEx.CustomProperty(propertyName, propertyType, bIsReadOnly, "Asset Properties", "", bIsVisible);
				pgProperties.Item.Add(propertyToAdd);
				cbRemoveProperty.Items.Add(propertyName);
				m_columnsToAdd.Add(propertyName, propertyType);
			}
			else
			{
				Global.WriteOutput("Error: Property with that name already exists.");
			}
			txtName.Text = "";
			pgProperties.Refresh();
		}

		private void pgProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.Label == "Use Integrated Security" && !cbIsNative.Checked)
			{
				if (e.OldValue.ToString() == "False")
				{
					MakeReadOnly("User Name");
					MakeReadOnly("Password");
				}
				else
				{
					ChangeToReadWrite("User Name");
					ChangeToReadWrite("Password");
				}
				pgProperties.Refresh();
			}
		}

		private void MakeReadOnly(String strLabel)
		{
			for (int i = 0; i < pgProperties.Item.Count; i++)
			{
				if (pgProperties.Item[i].Name.ToString() == strLabel)
				{
					pgProperties.Item[i].IsReadOnly = true;
				}
			}
		}

		private void ChangeToReadWrite(String strLabel)
		{
			for (int i = 0; i < pgProperties.Item.Count; i++)
			{
				if (pgProperties.Item[i].Name.ToString() == strLabel)
				{
					pgProperties.Item[i].IsReadOnly = false;
				}
			}
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			if (m_bCreateNew)
			{
				if (pgProperties.Item["Asset Name"].Value.ToString() != "")
				{
					m_assetName = pgProperties.Item["Asset Name"].Value.ToString();


					if (cbIsNative.Checked)
					{
						CreateNativeAssetTables();
					}
					else //if (pgProperties.Item["Server"] == null || pgProperties.Item["Database"] == null || pgProperties.Item["Provider"] == null || pgProperties.Item["View"] == null)
					{
						CreateNonNativeAsset();
					}
				}
				else
				{
					Global.WriteOutput("Error: Please fill in the 'Asset Name' property before continuing.");
					return;
				}
			}
			else
			{
				// Add/Remove the columns in the respective lists using
				// a SQL ALTER TABLE statement.
				DBMgr.AddTableColumns(m_assetName, m_columnsToAdd);
				DBMgr.RemoveTableColumns(m_assetName, m_columnsToRemove);
			}
			DialogResult = DialogResult.OK;
			
			if (DialogResult == DialogResult.OK)
			{
				//RoadCare3.Properties.Settings.Default.PROVIDER_DIU = pgProperties.Item["Provider"].Value.ToString();
				//RoadCare3.Properties.Settings.Default.SERVER_DIU = pgProperties.Item["Server"].Value.ToString();
				//RoadCare3.Properties.Settings.Default.DATABASE_DIU = pgProperties.Item["Database"].Value.ToString();
				//RoadCare3.Properties.Settings.Default.USERNAME_DIU = pgProperties.Item["User Name"].Value.ToString();

				// Save the property settings
				Properties.Settings.Default.Save();
			}
		}

		private void CreateNonNativeAsset()
		{
			String insertAssetDatabaseInfo = "Insert INTO ASSETS (ASSET, DATE_CREATED, CREATOR_ID, LAST_MODIFIED) " +
								   "Values ('" + pgProperties.Item["Asset Name"].Value
								   + "', '" + DateTime.Now
								   + "', '" + DBMgr.NativeConnectionParameters.UserName
								   + "', '" + DateTime.Now
								   + "')";
			try
			{
				DBMgr.ExecuteNonQuery(insertAssetDatabaseInfo);
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Insert new asset failed. " + exc.Message);
				return;
			}

			// Now update the new asset with its non native connection information
			String updateNonNativeAsset = "UPDATE ASSETS SET "
				+ "SERVER = '" + pgProperties.Item["Server"].Value.ToString()
				+ "', DATASOURCE = '" + pgProperties.Item["Database"].Value.ToString()
				+ "', USERID = '" + pgProperties.Item["User Name"].Value.ToString()
				+ "', PASSWORD = '" + pgProperties.Item["Password"].Value.ToString()
				+ "', PROVIDER = '" + pgProperties.Item["Provider"].Value.ToString()
				+ "', NATIVE = '" + cbIsNative.Checked + "'"
				+ " WHERE ASSET = '" + m_assetName + "'";
			try
			{
				DBMgr.ExecuteNonQuery(updateNonNativeAsset);
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Could not update non-native asset entry in ASSETS table. " + exc.Message);
			}
		}

		private void CreateNativeAssetTables()
		{
			List<TableParameters> listTP = new List<TableParameters>();
			List<TableParameters> listTPHistory = new List<TableParameters>();

			String strProperty;
			String strDataType;

			this.DialogResult = DialogResult.OK;

			// Now add several default rows to the asset being created in the database as table "ASSET NAME"
			listTP.Add(new TableParameters("ID", DataType.Int, false, true, true));
			listTP.Add(new TableParameters("FACILITY", DataType.VarChar(-1), true, false, false));
			listTP.Add(new TableParameters("SECTION", DataType.VarChar(-1), true, false, false));
			listTP.Add(new TableParameters("BEGIN_STATION", DataType.Float, true, false, false));
			listTP.Add(new TableParameters("END_STATION", DataType.Float, true, false, false));
			listTP.Add(new TableParameters("DIRECTION", DataType.VarChar(50), true, false, false));
			listTP.Add(new TableParameters("ENTRY_DATE", DataType.DateTime, true, false, false));
			listTP.Add(new TableParameters("GEOMETRY", DataType.VarChar(-1), true, false, false));
			listTP.Add(new TableParameters("EnvelopeMaxX", DataType.Float, true, false, false));
			listTP.Add(new TableParameters("EnvelopeMinX", DataType.Float, true, false, false));
			listTP.Add(new TableParameters("EnvelopeMaxY", DataType.Float, true, false, false));
			listTP.Add(new TableParameters("EnvelopeMinY", DataType.Float, true, false, false));
			
			listTPHistory.Add(new TableParameters("ID", DataType.Int, false, true, true));
			listTPHistory.Add(new TableParameters("ATTRIBUTE_ID", DataType.Int, false, false, false));
			listTPHistory.Add(new TableParameters("FIELD", DataType.VarChar(50), false, false, false));
			listTPHistory.Add(new TableParameters("VALUE", DataType.VarChar(50), true, false, false));
			listTPHistory.Add(new TableParameters("USER_ID", DataType.VarChar(200), true, false, false));
			listTPHistory.Add(new TableParameters("WORKACTIVITY", DataType.VarChar(-1), true, false, false));
			listTPHistory.Add(new TableParameters("WORKACTIVITY_ID", DataType.VarChar(-1), true, false, false));
			listTPHistory.Add(new TableParameters("DATE_MODIFIED", DataType.DateTime, false, false, false));

			for (int i = 0; i < pgProperties.Item.Count; i++)
			{
				if (pgProperties.Item[i].Category != "Database Information" && pgProperties.Item[i].Category != "Required Fields" && pgProperties.Item[i].Name != "Asset Name")
				{
					strProperty = pgProperties.Item[i].Name;
					strDataType = pgProperties.Item[i].Value.ToString();

					DataType dataTypeToInsert = Global.ConvertStringToDataType(strDataType);

					// Create the table params list to pass into the DBMgr.CreateTable function.
					listTP.Add(new TableParameters(strProperty, dataTypeToInsert, false, false));
				}
			}
			String strInsert;

			// Now create the row in the ASSETS tables.
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					strInsert = "Insert INTO ASSETS (ASSET, DATE_CREATED, CREATOR_ID, LAST_MODIFIED) " +
										   "Values ('" + pgProperties.Item["Asset Name"].Value
										   + "', '" + DateTime.Now
										   + "', '" + DBMgr.NativeConnectionParameters.UserName
										   + "', '" + DateTime.Now
										   + "')";
					break;
				case "ORACLE":
					strInsert = "Insert INTO ASSETS (ASSET, DATE_CREATED, CREATOR_ID, LAST_MODIFIED) Values ('" +
						pgProperties.Item["Asset Name"].Value +
						"', TO_DATE('" + DateTime.Now.ToShortDateString() + "', 'MM/DD/YYYY')" +
						", '" + DBMgr.NativeConnectionParameters.UserName +
						"', TO_DATE('" + DateTime.Now.ToShortDateString() + "', 'MM/DD/YYYY')" +
						")";
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
					//break;
			}

			try
			{
				DBMgr.ExecuteNonQuery(strInsert);
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Insert new asset failed. " + exc.Message);
				return;
			}

			// Now create the table in the database <asset name>
			try
			{
				DBMgr.CreateTable(pgProperties.Item["Asset Name"].Value.ToString(), listTP);
			}
			catch (FailedOperationException exc)
			{
				Global.WriteOutput("Error: Create asset data table failed. " + exc.Message);
				return;
			}

			// Now create the <asset name>_CHANGELOG table for this asset in the database.
			try
			{
				DBMgr.CreateTable(pgProperties.Item["Asset Name"].Value.ToString() + "_" + "CHANGELOG", listTPHistory);
			}
			catch (FailedOperationException exc)
			{
				Global.WriteOutput("Error: Create asset Changelog failed. " + exc.Message);
				return;
			}
			
			
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void cmdRemove_Click(object sender, EventArgs e)
		{
			if (pgProperties.Item[cbRemoveProperty.Text] != null)
			{
				if (pgProperties.Item[cbRemoveProperty.Text].Category != "Database Information" && pgProperties.Item[cbRemoveProperty.Text].Category != "Required Fields" && cbRemoveProperty.Text != "Asset Name")
				{
					pgProperties.Item.Remove(cbRemoveProperty.Text);
					m_columnsToRemove.Add(cbRemoveProperty.Text);
					cbRemoveProperty.Items.Remove(cbRemoveProperty.Text);
					cbRemoveProperty.Text = "";
					pgProperties.Refresh();
				}
			}
		}

		public String Asset
		{
			get { return m_assetName; }
			set { m_assetName = value; }
		}
	}
}
