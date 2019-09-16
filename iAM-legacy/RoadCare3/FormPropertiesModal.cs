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
using Microsoft.SqlServer.Management.Smo;
using System.Collections;

namespace RoadCare3
{
    public partial class FormPropertiesModal : ToolWindow
    {
        private String m_strAddedProperty = "";
        private String m_strPropertyType = "";

        private AttributeProperties attrProp;
        private NetworkProperties networkProp;
		private AssetProperties assetProp;
		private CalculatedFieldsProperties calcProp;

        //bool m_bDatabaseEntered = false;
        //bool m_bServerEntered = false;
        //bool m_bUserNameEntered = false;
        //bool m_bPasswordEntered = false;
        //bool m_bProviderEntered = false;
        //bool m_bAttributeEntered = false;
        //bool m_bView = false;
        //bool m_bNative = true;

		bool m_bPassesValidation = true;

        public FormPropertiesModal()
        {
            InitializeComponent();
        }

		public bool IsValidated
		{
			get { return m_bPassesValidation; }
		}

		private void FormPropertiesModal_Load(object sender, EventArgs e)
		{
			SecureForm();
		}

        public String GetAddedProperty()
        {
            return m_strAddedProperty;
        }

        /// <summary>
        /// Attaches a Property Grid class object to the property grid tool window, or modal dialog box.
        /// </summary>
        /// <param name="o">A custom property class (such as Attributes.cs) is attached to the property grid.</param>
        public void SetPropertyGrid(object o, bool bIsModal)
        {
            m_strPropertyType = o.GetType().Name.ToString();
            if (m_strPropertyType == "AttributeProperties")
            {
                attrProp = (AttributeProperties)o;
                attrProp.SetAttributeProperties(pgProperties);
            }
            if (m_strPropertyType == "NetworkProperties")
            {
                networkProp = (NetworkProperties)o;
                networkProp.SetNetworkProperties(pgProperties, bIsModal);
            }
			if (m_strPropertyType == "AssetProperties")
			{
				assetProp = (AssetProperties)o;
				assetProp.SetAssetProperties(pgProperties, bIsModal);
			}
			if (m_strPropertyType == "CalculatedFieldsProperties")
			{
				calcProp = (CalculatedFieldsProperties)o;
				calcProp.CreateCalculatedFieldPropertyGrid(pgProperties);
			}
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
			String strError; 
			if (m_strPropertyType == "NetworkProperties")
            {
				//dsmelser 2008.08.04
				//added validation for network properties (currently only checks for existing network name)
                string currNetDef = "DEFAULT";
				strError = VerifyNetworkProperties(currNetDef);
				if (strError == "")
				{
					m_strAddedProperty = networkProp.SavePropertiesToDatabase(pgProperties);
				}
				else
				{
					Global.WriteOutput(strError);
					m_bPassesValidation = false;
				}
            }
            if (m_strPropertyType == "AttributeProperties")
            {
                m_strAddedProperty = attrProp.SavePropertiesToDatabase();
				if (m_strAddedProperty == null)
				{
					Global.WriteOutput("Error: " + "A user input error has been detected, aborting new attribute creation. Please verify all fields have been entered correctly.");
					m_bPassesValidation = false;
				}
				SetPropertyGridDefaults();
            }
			if (m_strPropertyType == "CalculatedFieldsProperties")
			{
				string error;
				if (calcProp.VerifyCalcFieldProperties(out error, pgProperties))
				{
					m_strAddedProperty = calcProp.CreateCalculatedField(pgProperties);
				}
				else
				{
					Global.WriteOutput("Error: Please make sure the following fields have values: " + error + ".");
					m_bPassesValidation = false;
				}
			}
			if (m_bPassesValidation)
			{
				this.Close();
			}
			return;
        }

		private void SetPropertyGridDefaults()
		{
			RoadCare3.Properties.Settings.Default.ATTRIBUTE_DEFAULT = RoadCare3.Properties.Settings.Default.ATTRIBUTE_DEFAULT + 1;
			if (pgProperties.Item["Provider"] != null)
			{
				RoadCare3.Properties.Settings.Default.PROVIDER_DIU = pgProperties.Item["Provider"].Value.ToString();
			}
			if (pgProperties.Item["Server"].Value != null)
			{
				RoadCare3.Properties.Settings.Default.SERVER_DIU = pgProperties.Item["Server"].Value.ToString();
			}
			if (pgProperties.Item["Database"].Value != null)
			{
				RoadCare3.Properties.Settings.Default.DATABASE_DIU = pgProperties.Item["Database"].Value.ToString();
			}
			if (pgProperties.Item["Login"].Value != null)
			{
				RoadCare3.Properties.Settings.Default.USERNAME_DIU = pgProperties.Item["Login"].Value.ToString();
			}
			RoadCare3.Properties.Settings.Default.Save();
		}

		/// <summary>
		/// Checks Network properties for errors before submitting db changes.  Currently, it only checks for an existing network name.
		/// </summary>
		/// <returns>Returns an empty string if the network properties are acceptable, otherwise it returns an error message.</returns>
		private string VerifyNetworkProperties(string currNetDef)
		{
			String error = "";
			SolutionExplorer currentExplorer = FormManager.GetSolutionExplorer();
			if (currentExplorer != null)
			{
				TreeView currentTree = currentExplorer.GetTreeView();
				if (currentTree != null)
				{
					//dsmelser 2008.08.05
					//the name of the Node that contains the network nodes is NodeNetwork, not Networks
					//foreach (TreeNode tn in currentTree.Nodes)
					//{
					//	error += tn.Name + " ";
					//}

					TreeNode networksNode = currentTree.Nodes[currNetDef].Nodes["NodeNetwork" + currNetDef];
					if (networksNode != null)
					{
						foreach (TreeNode network in networksNode.Nodes)
						{
							//dsmelser 2008.08.05
							if ((string)pgProperties.Item["Network Name"].Value == network.Text)
							{
								error += "Error: Network name[" + network.Text + "] already exists. ";
							}

							//dsmelser 2008.08.05
							//networkProp is not set yet at this point in the code
							//if (networkProp.Network_Name == network.Text)
							//{
							//	error += "Error: Network name[" + network.Text + "] already exists. ";
							//}
						}
					}
					else
					{
						error += "Networks Node not found for current TreeView ";
					}
				}
				else
				{
					error += "Error: TreeView not found for current Solution Explorer ";
				}
			}
			else
			{
				//error += "Error: SolutionExplorer not found.";
			}

			return error.Trim() ;
		}

        private bool VerifyAttributeProperties(out String strError)
        {
            bool bReturn = true;
            strError = "";
            Hashtable htProperties = new Hashtable();
            for (int i = 0; i < pgProperties.Item.Count; i++)
            {
                htProperties.Add(pgProperties.Item[i].Name, pgProperties.Item[i].Value);
            }

            if (htProperties["Native"].ToString() == "False")
            {
                if (htProperties["Server"] == null || htProperties["Server"].ToString().Trim() == "")
                {
                    strError += "Server, ";
                }
                if (htProperties["Database"] == null || htProperties["Database"].ToString().Trim() == "")
                {
                    strError += "Database, ";
                }
            }

            if (htProperties["Type"].ToString() == "NUMBER")
            {
                if (htProperties["One"] == null || htProperties["One"].ToString().Trim() == "")
                {
                    strError += "One, ";
                }
                if (htProperties["Two"] == null || htProperties["Two"].ToString().Trim() == "")
                {
                    strError += "Two, ";
                }
                if (htProperties["Three"] == null || htProperties["Three"].ToString().Trim() == "")
                {
                    strError += "Three, ";
                }
                if (htProperties["Four"] == null || htProperties["Four"].ToString().Trim() == "")
                {
                    strError += "Four, ";
                }
                if (htProperties["Five"] == null || htProperties["Five"].ToString().Trim() == "")
                {
                    strError += "Five, ";
                }
                if (htProperties["Default_Value"] == null || htProperties["Default_Value"].ToString().Trim() == "")
                {
                    strError += "Default_Value, ";
                }
                try
                {
                    // Error check for correct formatting.
                    float f = 1f;
                    String strFormatValue = htProperties["Format"].ToString();
                    String str = f.ToString(strFormatValue);
                    float.Parse(str);
                }
                catch
                {
                    strError += "Format, ";
                }
            }
            if (htProperties["Attribute"] == null || htProperties["Attribute"].ToString().Trim() == "")
            {
                strError += "Attribute, ";
            }
            if (strError.Length == 0)
            {
                bReturn = true;
            }
            else
            {
                strError = strError.Substring(0, strError.Length - 2);
                bReturn = false;
            }
            return bReturn;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            return;
        }

        private void pgProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
			if( m_strPropertyType == "AttributeProperties" )
            {
                if (e.ChangedItem.Label == "Attribute")
                {
                    attrProp.Attribute = pgProperties.Item["Attribute"].Value.ToString();
                }
                if (e.ChangedItem.Label == "Native" && (e.ChangedItem.Value.ToString() == "" || e.ChangedItem.Value.ToString() == "False"))
                {
                    pgProperties.Item["Server"].IsReadOnly = false;
                    pgProperties.Item["Provider"].IsReadOnly = false;
                    pgProperties.Item["View"].IsReadOnly = false;
                    pgProperties.Item["Integrated Security"].IsReadOnly = false;

                    if (pgProperties.Item["Provider"].Value.ToString() == "MSSQL")
                    {
                        pgProperties.Item["Database"].IsReadOnly = false;
                        pgProperties.Item["Server"].IsReadOnly = false;

                        pgProperties.Item["SID"].IsReadOnly = true;
                        pgProperties.Item["Port"].IsReadOnly = true;
                        pgProperties.Item["Connection Type"].IsReadOnly = true;
                        pgProperties.Item["Network Alias"].IsReadOnly = true;
                    }
                    if (pgProperties.Item["Provider"].Value.ToString() == "ORACLE")
                    {
                        pgProperties.Item["SID"].IsReadOnly = false;
                        pgProperties.Item["Port"].IsReadOnly = false;
                        pgProperties.Item["Connection Type"].IsReadOnly = false;
                        pgProperties.Item["Network Alias"].IsReadOnly = false;

                        pgProperties.Item["Database"].IsReadOnly = true;
                    }
                    if (pgProperties.Item["Integrated Security"].Value.ToString() == "False")
                    {
                        pgProperties.Item["Login"].IsReadOnly = false;
                        pgProperties.Item["Password"].IsReadOnly = false;
                    }
                    else
                    {
                        pgProperties.Item["Login"].IsReadOnly = true;
                        pgProperties.Item["Password"].IsReadOnly = true;
                    }
                }
                else if (e.ChangedItem.Label == "Native")
                {
                    pgProperties.Item["Provider"].IsReadOnly = true;
                    pgProperties.Item["View"].IsReadOnly = true;
                    pgProperties.Item["Integrated Security"].IsReadOnly = true;
                    pgProperties.Item["Login"].IsReadOnly = true;
                    pgProperties.Item["Password"].IsReadOnly = true;
                    pgProperties.Item["SID"].IsReadOnly = true;
                    pgProperties.Item["Port"].IsReadOnly = true;
                    pgProperties.Item["Server"].IsReadOnly = true;
                    pgProperties.Item["Connection Type"].IsReadOnly = true;
                    pgProperties.Item["Network Alias"].IsReadOnly = true;
                    pgProperties.Item["Database"].IsReadOnly = true;
                }
                if(e.ChangedItem.Label == "Connection Type")
                {
                    if (pgProperties.Item["Connection Type"].Value.ToString() == "Basic")
                    {
                        pgProperties.Item["Server"].IsReadOnly = false;
                        pgProperties.Item["SID"].IsReadOnly = false;
                        pgProperties.Item["Port"].IsReadOnly = false;

                        pgProperties.Item["Network Alias"].IsReadOnly = true;
                    }
                    if (pgProperties.Item["Connection Type"].Value.ToString() == "")
                    {
                        pgProperties.Item["Server"].IsReadOnly = true;
                        pgProperties.Item["SID"].IsReadOnly = true;
                        pgProperties.Item["Port"].IsReadOnly = true;
                        pgProperties.Item["Network Alias"].IsReadOnly = true;

                        pgProperties.Item["Connection Type"].IsReadOnly = false;
                    }
                    if (pgProperties.Item["Connection Type"].Value.ToString() == "Network Alias")
                    {
                        pgProperties.Item["Server"].IsReadOnly = true;
                        pgProperties.Item["SID"].IsReadOnly = true;
                        pgProperties.Item["Port"].IsReadOnly = true;

                        pgProperties.Item["Network Alias"].IsReadOnly = false;
                    }
                }
                if (e.ChangedItem.Label == "Provider")
                {
                    switch (e.ChangedItem.Value.ToString())
                    {
                        case "MSSQL":
                            pgProperties.Item["SID"].IsReadOnly = true;
                            pgProperties.Item["Port"].IsReadOnly = true;
                            pgProperties.Item["Connection Type"].IsReadOnly = true;
                            pgProperties.Item["Network Alias"].IsReadOnly = true;

                            pgProperties.Item["Database"].IsReadOnly = false;
                            pgProperties.Item["Server"].IsReadOnly = false;
                            break;
                        case "ORACLE":
                            if (pgProperties.Item["Connection Type"].Value.ToString() == "Basic")
                            {
                                pgProperties.Item["Server"].IsReadOnly = false;
                                pgProperties.Item["SID"].IsReadOnly = false;
                                pgProperties.Item["Port"].IsReadOnly = false;

                                pgProperties.Item["Network Alias"].IsReadOnly = true;
                            }
                            if (pgProperties.Item["Connection Type"].Value.ToString() == "")
                            {
                                pgProperties.Item["Server"].IsReadOnly = true;
                                pgProperties.Item["SID"].IsReadOnly = true;
                                pgProperties.Item["Port"].IsReadOnly = true;
                                pgProperties.Item["Network Alias"].IsReadOnly = true;

                                pgProperties.Item["Connection Type"].IsReadOnly = false;
                            }
                            if (pgProperties.Item["Connection Type"].Value.ToString() == "Network Alias")
                            {
                                pgProperties.Item["Server"].IsReadOnly = true;
                                pgProperties.Item["SID"].IsReadOnly = true;
                                pgProperties.Item["Port"].IsReadOnly = true;

                                pgProperties.Item["Network Alias"].IsReadOnly = false;
                            }
                            pgProperties.Item["Connection Type"].IsReadOnly = false;
                            pgProperties.Item["Database"].IsReadOnly = true;
                            break;
                        default:
                            pgProperties.Item["SID"].IsReadOnly = true;
                            pgProperties.Item["Port"].IsReadOnly = true;
                            pgProperties.Item["Database"].IsReadOnly = true;
                            pgProperties.Item["Connection Type"].IsReadOnly = false;
                            pgProperties.Item["Network Alias"].IsReadOnly = true;
                            break;
                    }
                }
                if (e.ChangedItem.Label == "Integrated Security" && (e.ChangedItem.Value.ToString() == "False" || e.ChangedItem.Value.ToString() == ""))
                {
                    pgProperties.Item["Login"].IsReadOnly = false;
                    pgProperties.Item["Password"].IsReadOnly = false;
                }
                else if (e.ChangedItem.Label == "Integrated Security")
                {
                    pgProperties.Item["Login"].IsReadOnly = true;
                    pgProperties.Item["Password"].IsReadOnly = true;
                }

                if (e.ChangedItem.Label == "Type")
                {
                    if (e.ChangedItem.Value.ToString() == "STRING")
                    {
                        pgProperties.Item["Maximum"].IsReadOnly = true;
                        pgProperties.Item["Minimum"].IsReadOnly = true;
                        pgProperties.Item["One"].IsReadOnly = true;
                        pgProperties.Item["Two"].IsReadOnly = true;
                        pgProperties.Item["Three"].IsReadOnly = true;
                        pgProperties.Item["Four"].IsReadOnly = true;
                        pgProperties.Item["Five"].IsReadOnly = true;
                        pgProperties.Item["Format"].IsReadOnly = true;
                        pgProperties.Item["Ascending"].IsReadOnly = true;
                    }
                    if (e.ChangedItem.Value.ToString() == "NUMBER")
                    {
                        pgProperties.Item["Maximum"].IsReadOnly = false;
                        pgProperties.Item["Minimum"].IsReadOnly = false;
                        pgProperties.Item["One"].IsReadOnly = false;
                        pgProperties.Item["Two"].IsReadOnly = false;
                        pgProperties.Item["Three"].IsReadOnly = false;
                        pgProperties.Item["Four"].IsReadOnly = false;
                        pgProperties.Item["Five"].IsReadOnly = false;
                        pgProperties.Item["Format"].IsReadOnly = false;
                        pgProperties.Item["Ascending"].IsReadOnly = false;
                    }
                }
                pgProperties.Refresh();
            }
        }

		protected void SecureForm()
		{
			//throw new NotImplementedException();
		}
	}
}