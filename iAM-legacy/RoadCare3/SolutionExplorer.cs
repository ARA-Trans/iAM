namespace RoadCare3
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using DatabaseManager;
    using Reports;
    using Reports.DDOT;
    using Reports.LC_PAVEMENTS;
    using RoadCare3.Properties;
    using RoadCareDatabaseOperations;
    using WeifenLuo.WinFormsUI.Docking;
    using DataAccessLayer;

    public partial class SolutionExplorer : ToolWindow
    {
        public DockPanel m_dockPanel;
        private List<String> m_listAttribute = new List<string>();
        private Hashtable m_htFieldMapping = new Hashtable();

		public SolutionExplorer( ref DockPanel dockPanel )
		{
			InitializeComponent();
			m_dockPanel = dockPanel;
			FormManager.SetDockPanel( dockPanel );

            // SecureForm has to be called after we create all the nodes dynamically.
            // Create new parent level nodes for Network Definitions
            List<NetworkDefinition> networkDefinitions = GetNetworkDefinitions();
            foreach (NetworkDefinition netDef in networkDefinitions)
            {
                // Build root level nodes
                BuildRootNodes(netDef);

                // Populate the attribute treeview.
                // Change by G.Larson 12/24/2008 to implement calculated fields
                DataSet ds = DBMgr.ExecuteQuery("SELECT ATTRIBUTE_, NATIVE_, PROVIDER, SERVER, DATASOURCE, USERID, PASSWORD_ FROM ATTRIBUTES_ WHERE NETWORK_DEFINITION_NAME = '" + netDef.NetDefName + "' AND (CALCULATED='0' OR CALCULATED IS NULL) ORDER BY ATTRIBUTE_");
                foreach (DataRow dataRow in ds.Tables[0].Rows)
                {
                    String strAttribute = dataRow["ATTRIBUTE_"].ToString();
                    if (Global.SecurityOperations.CanViewRawAttributeData(strAttribute))
                    {
                        ConnectionParameters attributeCP = DBMgr.GetAttributeConnectionObject(strAttribute);
                        try
                        {
                            m_listAttribute.Add(strAttribute);
                            SolutionExplorerTreeNode tn = new SolutionExplorerTreeNode(netDef);
                            tn.Text = strAttribute;
                            tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeAttribute" + netDef.NetDefName].Nodes.Add(tn);
                            tn.ContextMenuStrip = cmsAttribute;
                            tn.Name = strAttribute;

                            //TODO: add checking to see if any of these are already open
                            tn.ImageKey = RoadCare3.Properties.Settings.Default.ATTRIBUTE_IMAGE_KEY;
                            tn.SelectedImageKey = RoadCare3.Properties.Settings.Default.ATTRIBUTE_IMAGE_KEY_SELECTED;


                            //dsmelser 2008.07.30
                            //added Tag to enable matching for robust icon switching
                            tn.Tag = strAttribute + netDef.NetDefName;
                            //}
                        }
                        catch (Exception sqlE)
                        {
                            Global.WriteOutput("Error: Failed to fill attribute list in solution explorer. Please contact your database administrator. " + sqlE.Message);
                        }
                    }
                }

                // Populate the Networks nodes
                String strNetwork;
                String strNetworkID;
                try
                {
                    ds = DBMgr.ExecuteQuery("SELECT NETWORK_NAME FROM NETWORKS ORDER BY NETWORK_NAME");
                    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    {
                        strNetwork = dataRow[0].ToString();
                        strNetworkID = DBOp.GetNetworkIDFromName(strNetwork);
                        if (Global.SecurityOperations.CanViewNetwork(strNetworkID))
                        {
                            AddNetworkNodesToTreeView(strNetwork, netDef);
                        }
                    }
                }
                catch (Exception sqlE)
                {
                    Global.WriteOutput("Error: Failed to fill network list in solution explorer. Please contact your database administrator. " + sqlE.Message);
                }
                //}

                //since we aren't removing root nodes anymore, we don't need this
                // Populate the Asset nodes
                //if( Global.SecurityOperations.CanViewAnyAsset() )
                //{
                String strAsset;
                try
                {
                    ds = DBMgr.ExecuteQuery("SELECT ASSET FROM ASSETS");
                    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    {
                        strAsset = dataRow[0].ToString();
                        if (Global.SecurityOperations.CanViewAsset(strAsset))
                        {
                            //TreeNode tn = tvSolutionExplorer.Nodes["NodeAsset"].Nodes.Add( strAsset );
                            ConnectionParameters assetCP = DBMgr.GetAssetConnectionObject(strAsset);
                            //TreeNode tn = tvSolutionExplorer.Nodes["NodeAsset"].Nodes.Add(strAsset);

                            SolutionExplorerTreeNode tn = new SolutionExplorerTreeNode(netDef);

                            tn.Name = "NodeAsset" + netDef.NetDefName;
                            tn.Text = strAsset;
                            tn.ContextMenuStrip = cmsAsset;

                            tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeAsset" + netDef.NetDefName].Nodes.Add(tn);
                            if (!assetCP.IsNative)
                            {
                                tn.BackColor = Color.LightBlue;
                            }
                            //dsmelser 2008.07.30
                            //added Tagging for robust icon switching
                            tn.Tag = strAsset + netDef.NetDefName;
                            tn.ImageKey = Settings.Default.ASSET_IMAGE_KEY;
                            tn.SelectedImageKey = Settings.Default.ASSET_IMAGE_KEY_SELECTED;
                            tn.ContextMenuStrip = cmsAsset;
                        }
                    }
                }
                catch (Exception sqlE)
                {
                    Global.WriteOutput("Error: Failed to fill asset list in solution explorer. Please contact your database administrator. " + sqlE.Message);
                }


                //Populate Calculated fields from Attributes
                //Added G.Larson 12/24/2008
                try
                {

                    ds = DBMgr.ExecuteQuery("SELECT ATTRIBUTE_ FROM ATTRIBUTES_ WHERE CALCULATED='1' AND NETWORK_DEFINITION_NAME = '" + netDef.NetDefName + "' AND ASSET IS NULL");
                    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    {
                        String strAttribute = dataRow["ATTRIBUTE_"].ToString();
                        if (strAttribute == "PCI" || strAttribute == "CLIMATE_PCI" || strAttribute == "LOAD_PCI" || strAttribute == "OTHER_PCI") continue;

                        SolutionExplorerTreeNode tn = new SolutionExplorerTreeNode(netDef);
                        tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeCalculatedFields" + netDef.NetDefName].Nodes.Add(tn);
                        tn.Name = strAttribute + netDef.NetDefName;
                        tn.Text = strAttribute;
                        //TODO: add checking to see if any of these are already open
                        tn.ImageKey = RoadCare3.Properties.Settings.Default.ATTRIBUTE_IMAGE_KEY;
                        tn.SelectedImageKey = RoadCare3.Properties.Settings.Default.ATTRIBUTE_IMAGE_KEY_SELECTED;
                        tn.Tag = strAttribute + netDef.NetDefName;
                        tn.ContextMenuStrip = cmsCalculatedFields;
                    }

                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error: Failed to load CALCULATED ATTRIBUTES." + exception.Message);

                }
            }

            // Add security level nodes
            SolutionExplorerTreeNode securityNode = new SolutionExplorerTreeNode();
            securityNode.Name = "NodeSecurity";
            securityNode.Text = "Security";
            securityNode.Tag = "Users";
            securityNode.ImageKey = "Users.ico";
            securityNode.SelectedImageKey = "Users.ico";
            tvSolutionExplorer.Nodes.Add(securityNode);

            SolutionExplorerTreeNode userManagementNode = new SolutionExplorerTreeNode();
            userManagementNode.Name = "NodeUserManagement";
            userManagementNode.Text = "User Management";
            userManagementNode.ImageKey = "blue.ico";
            userManagementNode.SelectedImageKey = "blue.ico";
            tvSolutionExplorer.Nodes["NodeSecurity"].Nodes.Add(userManagementNode);

            SolutionExplorerTreeNode userGroupManagementNode = new SolutionExplorerTreeNode();
            userGroupManagementNode.Name = "NodeUserGroupManagement";
            userGroupManagementNode.Text = "User Group Management";
            userGroupManagementNode.ImageKey = "blue.ico";
            userGroupManagementNode.SelectedImageKey = "blue.ico";
            tvSolutionExplorer.Nodes["NodeSecurity"].Nodes.Add(userGroupManagementNode);

            SolutionExplorerTreeNode actionManagementNode = new SolutionExplorerTreeNode();
            actionManagementNode.Name = "NodeActionManagement";
            actionManagementNode.Text = "Action Management";
            actionManagementNode.ImageKey = "blue.ico";
            actionManagementNode.SelectedImageKey = "blue.ico";
            tvSolutionExplorer.Nodes["NodeSecurity"].Nodes.Add(actionManagementNode);

            SolutionExplorerTreeNode actionGroupManagementNode = new SolutionExplorerTreeNode();
            actionGroupManagementNode.Name = "NodeActionGroupManagement";
            actionGroupManagementNode.Text = "Action Group Management";
            actionGroupManagementNode.ImageKey = "blue.ico";
            actionGroupManagementNode.SelectedImageKey = "blue.ico";
            tvSolutionExplorer.Nodes["NodeSecurity"].Nodes.Add(actionGroupManagementNode);

            SolutionExplorerTreeNode permissionsNode = new SolutionExplorerTreeNode();
            permissionsNode.Name = "NodePermissions";
            permissionsNode.Text = "Permissions";
            permissionsNode.ImageKey = "blue.ico";
            permissionsNode.SelectedImageKey = "blue.ico";
            tvSolutionExplorer.Nodes["NodeSecurity"].Nodes.Add(permissionsNode);
		}

        private void BuildRootNodes(NetworkDefinition netDef)
        {
            // Network definition tree node (This is the new parent level node)
            SolutionExplorerTreeNode netDefNode = new SolutionExplorerTreeNode(netDef);
            netDefNode.Name = netDef.NetDefName;
            netDefNode.Text = netDef.NetDefName;
            netDefNode.Tag = netDef;
            netDefNode.ContextMenuStrip = cmsNetworkDefinition;
            tvSolutionExplorer.Nodes.Add(netDefNode);

            // Route/Section Definition tree node
            SolutionExplorerTreeNode routeNode = new SolutionExplorerTreeNode(netDef);
            routeNode.Name = "NodeRoute" + netDef.NetDefName;
            routeNode.Text = "Route/Section Definition";
            routeNode.Tag = "Route/Section Definition" + netDef.NetDefName;
            tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes.Add(routeNode);

            // Linear
            SolutionExplorerTreeNode linearRefRoutesNode = new SolutionExplorerTreeNode(netDef);
            linearRefRoutesNode.Name = "NodeLinear" + netDef.NetDefName;
            linearRefRoutesNode.Text = "Linear Referenced Routes";
            linearRefRoutesNode.Tag = "Linear Referenced Routes" + netDef.NetDefName;
            linearRefRoutesNode.ContextMenuStrip = contextMenuStripNetworkDefinition;
            linearRefRoutesNode.ImageKey = "roadnetwork.ico";
            linearRefRoutesNode.SelectedImageKey = "roadnetwork.ico";
            tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeRoute" + netDef.NetDefName].Nodes.Add(linearRefRoutesNode);

            // Section
            SolutionExplorerTreeNode sectionRefRoutesNode = new SolutionExplorerTreeNode(netDef);
            sectionRefRoutesNode.Name = "NodeSection" + netDef.NetDefName;
            sectionRefRoutesNode.Text = "Section Referenced Facilities";
            sectionRefRoutesNode.Tag = "Section Referenced Facility" + netDef.NetDefName;
            sectionRefRoutesNode.ContextMenuStrip = contextMenuStripNetworkDefinition;
            sectionRefRoutesNode.ImageKey = "roadnetwork.ico";
            sectionRefRoutesNode.SelectedImageKey = "roadnetwork.ico";
            tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeRoute" + netDef.NetDefName].Nodes.Add(sectionRefRoutesNode);

            // Attributes(RAW)
            SolutionExplorerTreeNode attributesRawNode = new SolutionExplorerTreeNode(netDef);
            attributesRawNode.Name = "NodeAttribute" + netDef.NetDefName;
            attributesRawNode.Text = "Attributes (Raw)";
            attributesRawNode.Tag = "Attributes (Raw)" + netDef.NetDefName;
            attributesRawNode.ContextMenuStrip = cmsAttributeRoot;
            attributesRawNode.ImageKey = "dbs.ico";
            attributesRawNode.SelectedImageKey = "dbs.ico";
            tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes.Add(attributesRawNode);

            // Assets
            SolutionExplorerTreeNode assetsNode = new SolutionExplorerTreeNode(netDef);
            assetsNode.Name = "NodeAsset" + netDef.NetDefName;
            assetsNode.Text = "Assets";
            assetsNode.Tag = "Assets" + netDef.NetDefName;
            assetsNode.ContextMenuStrip = cmsAssetRoot;
            assetsNode.ImageKey = "bluedbs.ico";
            assetsNode.SelectedImageKey = "bluedbs.ico";
            tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes.Add(assetsNode);

            // Calculated Fields
            SolutionExplorerTreeNode calcFieldsNode = new SolutionExplorerTreeNode(netDef);
            calcFieldsNode.Name = "NodeCalculatedFields" + netDef.NetDefName;
            calcFieldsNode.Text = "Calculated Fields";
            calcFieldsNode.Tag = "Calculated Fields" + netDef.NetDefName;
            calcFieldsNode.ContextMenuStrip = cmsCalculatedFieldsRoot;
            calcFieldsNode.ImageKey = "calculator.ico";
            calcFieldsNode.SelectedImageKey = "calculator.ico";
            tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes.Add(calcFieldsNode);

            //  ---PCI
            SolutionExplorerTreeNode pciNode = new SolutionExplorerTreeNode(netDef);
            pciNode.Name = "NodePCI" + netDef.NetDefName;
            pciNode.Text = "PCI";
            pciNode.Tag = "PCI" + netDef.NetDefName;
			tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeCalculatedFields" + netDef.NetDefName].Nodes.Add(pciNode);

            //  ---Calculated Assets
            SolutionExplorerTreeNode calcAssetsNode = new SolutionExplorerTreeNode(netDef);
            calcAssetsNode.Name = "NodeCalculatedAssets" + netDef.NetDefName;
            calcAssetsNode.Text = "Calculated Assets";
            calcAssetsNode.Tag = "Calculated Assets" + netDef.NetDefName;
            calcAssetsNode.ImageIndex = 11;
            calcAssetsNode.SelectedImageIndex = 11;
            tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeCalculatedFields" + netDef.NetDefName].Nodes.Add(calcAssetsNode);


            //  ---Asset to Attribute
            SolutionExplorerTreeNode assetToAttrNode = new SolutionExplorerTreeNode(netDef);
            assetToAttrNode.Name = "NodeAssetsToAttributes" + netDef.NetDefName;
            assetToAttrNode.Text = "Asset To Attribute";
            assetToAttrNode.Tag = "Asset To Attribute" + netDef.NetDefName;
            assetToAttrNode.ImageIndex = 19;
            assetToAttrNode.SelectedImageIndex = 19;
            tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeCalculatedFields" + netDef.NetDefName].Nodes.Add(assetToAttrNode);

            // Networks
            SolutionExplorerTreeNode networksNode = new SolutionExplorerTreeNode(netDef);
            networksNode.Name = "NodeNetwork" + netDef.NetDefName;
            networksNode.Text = "Networks";
            networksNode.Tag = "Networks" + netDef.NetDefName;
            networksNode.ContextMenuStrip = cmsNetworkRoot;
            networksNode.ImageKey = "folderopen.ico";
            networksNode.SelectedImageKey = "folderopen.ico";
            tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes.Add(networksNode);

            // Security
                // User Management
                // Usergroup Management
                // Action Management
                // Action Group Management
                // Permissions

        }

        public List<NetworkDefinition> GetNetworkDefinitions()
        {
            string query = "SELECT * FROM NETWORK_DEFINITIONS";
            DataSet ds = DBMgr.ExecuteQuery(query);
            List<NetworkDefinition> toReturn = new List<NetworkDefinition>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string selectStatement = "";
                string connectionString = "";
                if(row["SELECT_STATEMENT"] != DBNull.Value)
                {
                    selectStatement = row["SELECT_STATEMENT"].ToString();
                }
                if(row["CONNECTION_STRING"] != DBNull.Value)
                {
                    connectionString = row["CONNECTION_STRING"].ToString();
                }
                NetworkDefinition currentNetDef = new NetworkDefinition(row["NETWORK_DEFINITION_NAME"].ToString(), row["IS_NATIVE"].ToString(), selectStatement, connectionString, row["TABLE_NAME"].ToString());
                toReturn.Add(currentNetDef);
            }
            return toReturn;
        }

		/// <summary>
		/// Changes the specified icon as desired.
		/// </summary>
		/// <param name="matchTag">The tag the icon will be matched according to.</param>
		/// <param name="regKey">The ImageKey string for when the icon is not selected.</param>
		/// <param name="selKey">The ImageKey string for the icon when selected.</param>
		/// <returns>ChangeIcon returns true upon success and false otherwise.</returns>
		public bool ChangeIcon(object matchTag, string regKey, string selKey )
		{
			bool success = false;
			TreeNode toChange = null;
			//Can't do this because TopNode is actually just the first node in tvSolutionExplorer.Nodes
			//TreeNode toChange = FindNodeByTag(matchTag, tvSolutionExplorer.TopNode);

			foreach (TreeNode tn in tvSolutionExplorer.Nodes)
			{
				toChange = FindNodeByTag(matchTag, tn);
				if (toChange != null)
				{
					break;
				}
			}

			if (toChange != null)
			{
				toChange.ImageKey = regKey;
				toChange.SelectedImageKey = selKey;
				success = true;
			}

			return success;
		}

		/// <summary>
		/// Recurses the tree structure to find the first Tag match.
		/// </summary>
		/// <param name="matchTag">The Tag object being searched for.</param>
		/// <param name="checkNode">The current node being examined.</param>
		/// <returns>Returns the TreeNode with the matching Tag or null if it was not found.</returns>
		private TreeNode FindNodeByTag(object matchTag, TreeNode checkNode)
		{
			TreeNode located = null;
			if (matchTag.Equals(checkNode.Tag))
			{
				located = checkNode;
			}
			else
			{
				foreach (TreeNode tn in checkNode.Nodes)
				{
					located = FindNodeByTag(matchTag, tn);
					if (located != null)
					{
						break;
					}
				}
			}

			return located;
		}

		protected void SecureForm()
		{
			//Many of the controls that need to be secured are dynamically created and so cannot be handled here.
			//Remove Linear/Section Network definition nodes if the current user has insufficient security

            // Network Definitions is not complete.  For now, assign a Network Definition of DEFAULT.
            // TODO: Make this dynamic.
            NetworkDefinition netDef = new NetworkDefinition("DEFAULT", "True", "", "", "NETWORK_DEFINITION");
			if( !Global.SecurityOperations.CanViewNetworkDefinitionData() )
			{
				tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeRoute" + netDef.NetDefName].Remove();
			}

			if( !Global.SecurityOperations.CanViewRawAttributeNode() )
			{
                tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeAttribute" + netDef.NetDefName].Remove();
			}


			if( !Global.SecurityOperations.CanViewAssetNode() )
			{
                tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeAsset" + netDef.NetDefName].Remove();
			}

			if( !Global.SecurityOperations.CanViewCalculatedFields() )
			{
                tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeCalculatedFields" + netDef.NetDefName].Remove();
			}
			else
			{
				if( !Global.SecurityOperations.CanCreateCalculatedFields() )
				{
                    tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeCalculatedFields" + netDef.NetDefName].ContextMenu = null;
                    tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeCalculatedFields" + netDef.NetDefName].ContextMenuStrip = null;
				}
				if( !Global.SecurityOperations.CanViewCalculatedAssets() )
				{
					//tvSolutionExplorer.Nodes["NodeCalculatedFields"].Nodes.Remove( tvSolutionExplorer.Nodes["NodeCalculatedFields"].Nodes["CalculatedAssets"] );
					tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeCalculatedFields" + netDef.NetDefName].Nodes.Remove(tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeCalculatedFields" + netDef.NetDefName].Nodes["NodeCalculatedAssets" + netDef.NetDefName]);
				}

				if( !Global.SecurityOperations.CanViewPCI() )
				{
                    tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeCalculatedFields" + netDef.NetDefName].Nodes.Remove(tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeCalculatedFields" + netDef.NetDefName].Nodes["NodePCI" + netDef.NetDefName]);
				}

				if( !Global.SecurityOperations.CanViewAssetToAttribute() )
				{
					tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeCalculatedFields" + netDef.NetDefName].Nodes.Remove(tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeCalculatedFields" + netDef.NetDefName].Nodes["NodeAssetsToAttributes" + netDef.NetDefName]);
				}
			}
            
			if( !Global.SecurityOperations.CanViewNetworkNode() )
			{
				tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes.Remove( tvSolutionExplorer.Nodes[netDef.NetDefName].Nodes["NodeNetwork" + netDef.NetDefName] );
			}


			if( !Global.SecurityOperations.CanViewSecurityNode() )
			{
				tvSolutionExplorer.Nodes.Remove( tvSolutionExplorer.Nodes["NodeSecurity"] );
			}
			else
			{
				if( !Global.SecurityOperations.CanViewUserData() )
				{
					tvSolutionExplorer.Nodes["NodeSecurity"].Nodes["NodeUserManagement"].Remove();
				}
				if( !Global.SecurityOperations.CanViewUserGroupData() )
				{
					tvSolutionExplorer.Nodes["NodeSecurity"].Nodes["NodeUserGroupManagement"].Remove();
				}
				if( !Global.SecurityOperations.CanViewActionData() )
				{
					tvSolutionExplorer.Nodes["NodeSecurity"].Nodes["NodeActionManagement"].Remove();
				}
				if( !Global.SecurityOperations.CanViewActionGroupData() )
				{
					tvSolutionExplorer.Nodes["NodeSecurity"].Nodes["NodeActionGroupManagement"].Remove();
				}
				if( !Global.SecurityOperations.CanViewPermissionsData() )
				{
					tvSolutionExplorer.Nodes["NodeSecurity"].Nodes["NodePermissions"].Remove();
				}
			}
		}

		/// <summary>
        /// Populates the tree view "Solution Explorer" with nodes containing data taken from the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SolutionExplorer_Load(object sender, EventArgs e)
        {
            SecureForm();
			CreateReportMenus();
        }

		private void CreateReportMenus()
		{
			List<string> networkReportNames = DBOp.GetNetworkReportNames();
			List<string> simulationReportNames = DBOp.GetSimulationReportNames();

			foreach (string reportName in networkReportNames)
			{
				if (Global.SecurityOperations.CanCreateNetworkReport(reportName))
				{
					networkReportsToolStripMenuItem.DropDownItems.Add(reportName);
				}
			}

			foreach (string reportName in simulationReportNames)
			{
				if( Global.SecurityOperations.CanCreateSimulationReport( reportName ) )
				{
					simulationReportsToolStripMenuItem.DropDownItems.Add( reportName );
				}
			}

			// Set up the event handlers for each new menu strip item
			foreach (ToolStripMenuItem tsmiReports in networkReportsToolStripMenuItem.DropDownItems)
			{
				tsmiReports.Click += new EventHandler(tsmiReports_Click);
			}

			foreach (ToolStripMenuItem tsmiReports in simulationReportsToolStripMenuItem.DropDownItems)
			{
				tsmiReports.Click += new EventHandler(tsmiReports_Click);
			}
		}

		void tsmiReports_Click(object sender, EventArgs e)
		{
			// Run the clicked on report
			string selectedReportName = ((ToolStripMenuItem)sender).Text;
			int reportID = DBOp.GetReportID(selectedReportName);
			string reportType = DBOp.GetReportType(reportID);
			//string advancedSearchText = "";

			String strNetworkID = "";
			String strSimulationID = "";
			String strNetwork = "";
			String strSimulation = "";
			if (reportType == "SIMULATION")
			{
				strNetworkID = tvSolutionExplorer.SelectedNode.Parent.Parent.Tag.ToString();
				strSimulationID = tvSolutionExplorer.SelectedNode.Tag.ToString();
				strNetwork = tvSolutionExplorer.SelectedNode.Parent.Parent.Text.ToString();
				strSimulation = tvSolutionExplorer.SelectedNode.Text.ToString();
			}
			if (reportType == "NETWORK")
			{
				strNetworkID = tvSolutionExplorer.SelectedNode.Tag.ToString();
				strNetwork = tvSolutionExplorer.SelectedNode.Text.ToString();
			}

            GenerateReport(
                selectedReportName,
                strNetworkID,
                strSimulationID,
                strNetwork,
                strSimulation);
		}

        public static void GenerateReport(
            string selectedReportName,
            string strNetworkID,
            string strSimulationID,
            string strNetwork,
            string strSimulation)
        {
            string strBudgetPer = "";
            int networkID = -1;
            if (!int.TryParse(strNetworkID, out networkID))
            {
                Regex numberPicker = new Regex("[0-9]+");
                MatchCollection numberMatches = numberPicker.Matches(strNetworkID);
                if (numberMatches.Count > 0)
                {
                    List<Match> numberMatchesSortableList = new List<Match>();
                    foreach (Match m in numberMatches)
                    {
                        numberMatchesSortableList.Add(m);
                    }
                    numberMatchesSortableList.Sort(
                        delegate(Match a, Match b)
                        {
                            return b.Length.CompareTo(a.Length);
                        });
                    networkID = int.Parse(numberMatchesSortableList[0].Value);
                }
                else
                {
                    throw new Exception("Error discerning network id from interface element.");
                }

            }
            //int.Parse( strNetworkID );

            //Form toMinimize = (Form) this.Parent.Parent.Parent.Parent;
            //toMinimize.Cursor = Cursors.WaitCursor;
            //FormWindowState previousState = toMinimize.WindowState;
            //toMinimize.SendToBack();
            ////toMinimize.WindowState = FormWindowState.Minimized;
            //toMinimize.Refresh();

            switch (selectedReportName)
            {
                //case "CostPerTreatmentYear":
                //    FormCostPerTreatmentYear costPerTreatmentYear = new FormCostPerTreatmentYear(strNetworkID, strSimulationID, Global.SecurityOperations.CurrentUserName);
                //    costPerTreatmentYear.ShowDialog();
                //    break;

                //case"NumRatingAvgPerYear":
                //    FormNumRatingAvg numrating = new FormNumRatingAvg(strNetworkID, strSimulationID);
                //    numrating.ShowDialog();
                //    break;

                case "Condition Citywide (%)":
                    ConditionCitywidePercent conditionCitywidePercent = new ConditionCitywidePercent(networkID, Global.SecurityOperations.CurrentUserName);
                    conditionCitywidePercent.CreateConditionCitywidePercentReport();
                    Global.WriteOutput("Done generating Condition Citywide (%) report at " + DateTime.Now.TimeOfDay);
                    break;
                case "Condition Citywide (Area)":
                    ConditionCitywideMiles conditionCitywideMiles = new ConditionCitywideMiles(networkID, Global.SecurityOperations.CurrentUserName);
                    conditionCitywideMiles.CreateConditionCitywidePercentReport();
                    Global.WriteOutput("Done generating Condition Citywide (Area) report at " + DateTime.Now.TimeOfDay);
                    break;
                case "Miles Per Ward":
                    MileagePerWard mpwReport = new MileagePerWard(networkID, Global.SecurityOperations.CurrentUserName);
                    mpwReport.CreateMileagePerWardReport();
                    Global.WriteOutput("Done generating Mileage Per Ward report at " + DateTime.Now.TimeOfDay);
                    break;
                case "Miles By Functional Class":
                    MileageByFunctionalClass mpfcReport = new MileageByFunctionalClass(networkID, Global.SecurityOperations.CurrentUserName);
                    mpfcReport.CreateMileageByFuncClass();
                    Global.WriteOutput("Done generating Mileage Per Functional Class report at " + DateTime.Now.TimeOfDay);
                    break;
                case "Miles By Functional Class and Ward":
                    MileageByFunctionalClassiAndWard mbfcawReport = new MileageByFunctionalClassiAndWard(networkID, Global.SecurityOperations.CurrentUserName);
                    mbfcawReport.CreateReport();
                    Global.WriteOutput("Done generating Mileage Per Functional Class And Ward report at " + DateTime.Now.TimeOfDay);
                    break;
                case "Condition By Functional Class (%)":
                    ConditionByFunctionalClass cbfc = new ConditionByFunctionalClass(networkID, Global.SecurityOperations.CurrentUserName);
                    cbfc.CreateConditionByFunctionalClassReport();
                    Global.WriteOutput("Done generating Condition By Functional Class report at " + DateTime.Now.TimeOfDay);
                    break;



                case "Section Detail Report":
                    ExportSectionViewReport(networkID);
                    break;
                case "Input Summary Report":
                    InputSummaryReport InputRpt = new InputSummaryReport(strNetworkID, strSimulationID, strNetwork, strSimulation);
                    InputRpt.CreateInputSummaryReport();
                    break;
                case "Total Budget Report":
                    TotalBudgetReport TotalBudgetRpt = new TotalBudgetReport(strNetworkID, strSimulationID, strNetwork, strSimulation);
                    TotalBudgetRpt.CreateTotalBudgetReport();
                    break;
                case "Budget Per District Report":
                    strBudgetPer = "DISTRICT";
                    BudgetPerDistrictReport BudgetPerDistrictRpt = new BudgetPerDistrictReport(strNetworkID, strSimulationID, strNetwork, strSimulation, strBudgetPer);
                    BudgetPerDistrictRpt.CreateBudgetPerReport();
                    break;
                case "Budget Per Treatment Report":
                    strBudgetPer = "TREATMENT";
                    BudgetPerTreatmentReport BudgetPerTreatmentRpt = new BudgetPerTreatmentReport(strNetworkID, strSimulationID, strNetwork, strSimulation, strBudgetPer);
                    BudgetPerTreatmentRpt.CreateBudgetPerTreatmentReport();
                    break;
                case "Lane-Miles Per Treatment Report":
                    strBudgetPer = "TREATMENT";
                    LaneMilePerTreatmentReport LaneMilePerTreatmentRpt = new LaneMilePerTreatmentReport(strNetworkID, strSimulationID, strNetwork, strSimulation, strBudgetPer, false);
                    LaneMilePerTreatmentRpt.CreateLaneMilesPerTreatmentReport();
                    //Allow two reports for MDSHA (LaneMile and Network specific areas)
                    LaneMilePerTreatmentRpt = new LaneMilePerTreatmentReport(strNetworkID, strSimulationID, strNetwork, strSimulation, strBudgetPer, true);
                    LaneMilePerTreatmentRpt.CreateLaneMilesPerTreatmentReport();
                    break;
                case "Lane-Miles Per District Report":
                    strBudgetPer = "DISTRICT";
                    LaneMilesPerDistrictReport LaneMilesPerDistrictRpt = new LaneMilesPerDistrictReport(strNetworkID, strSimulationID, strNetwork, strSimulation, strBudgetPer, false);
                    LaneMilesPerDistrictRpt.CreateLaneMilesPerDistrictReport();
                    //Allow two reports for MDSHA (LaneMile and Network specific areas)
                    LaneMilesPerDistrictRpt = new LaneMilesPerDistrictReport(strNetworkID, strSimulationID, strNetwork, strSimulation, strBudgetPer, true);
                    LaneMilesPerDistrictRpt.CreateLaneMilesPerDistrictReport();
                    break;
                case "Budget Per Condition Report":
                    strBudgetPer = "CONDITION_IRI";
                    BudgetPerConditionReport BudgetPerConditionRpt = new BudgetPerConditionReport(strNetworkID, strSimulationID, strNetwork, strSimulation, strBudgetPer);
                    BudgetPerConditionRpt.CreateBudgetPerConditionReport();
                    break;
                case "Lane-Miles Per Condition Report":
                    strBudgetPer = "CONDITION_IRI";
                    LaneMilesPerConditionReport LaneMilesPerConditionRpt = new LaneMilesPerConditionReport(strNetworkID, strSimulationID, strNetwork, strSimulation, strBudgetPer, false);
                    LaneMilesPerConditionRpt.CreateLaneMilesPerConditionReport();    // M&R Lane Miles Per Condition
                    //Allow two reports for MDSHA (LaneMile and Network specific areas)
                    LaneMilesPerConditionRpt = new LaneMilesPerConditionReport(strNetworkID, strSimulationID, strNetwork, strSimulation, strBudgetPer, true);
                    LaneMilesPerConditionRpt.CreateLaneMilesPerConditionReport();    // M&R Lane Miles Per Condition
                    break;
                case "Total Lane-Miles Per Condition Report":
                    strBudgetPer = "IRI_PER_SCALE";
                    TotalLaneMilesPerConditionReport TotalLaneMilesPerConditionRpt = new TotalLaneMilesPerConditionReport(strNetworkID, strSimulationID, strNetwork, strSimulation, strBudgetPer, false);
                    TotalLaneMilesPerConditionRpt.CreateTotalLaneMilesPerConditionReport();
                    //Allow two reports for MDSHA (LaneMile and Network specific areas)
                    TotalLaneMilesPerConditionRpt = new TotalLaneMilesPerConditionReport(strNetworkID, strSimulationID, strNetwork, strSimulation, strBudgetPer, true);
                    TotalLaneMilesPerConditionRpt.CreateTotalLaneMilesPerConditionReport();
                    break;
                case "Detailed Results Report":
                    strBudgetPer = "";
                    DetailedResultsReport DetailedResultsRpt = new DetailedResultsReport(strNetworkID, strSimulationID, strNetwork, strSimulation, strBudgetPer);
                    DetailedResultsRpt.CreateDetailedResultsReport();
                    break;
                case "Lane Mile Years Per Condition":
                    strBudgetPer = "IRI_PER_SCALE";
                    TotalLaneMileYearsPerConditionReport TotalLaneMileYearsPerConditionRpt = new TotalLaneMileYearsPerConditionReport(strNetworkID, strSimulationID, strNetwork, strSimulation, strBudgetPer, false);
                    TotalLaneMileYearsPerConditionRpt.CreateTotalLaneMileYearsPerConditionReport();
                    //Allow two reports for MDSHA (LaneMile and Network specific areas)
                    TotalLaneMileYearsPerConditionRpt = new TotalLaneMileYearsPerConditionReport(strNetworkID, strSimulationID, strNetwork, strSimulation, strBudgetPer, true);
                    TotalLaneMileYearsPerConditionRpt.CreateTotalLaneMileYearsPerConditionReport();
                    break;

                case "Weighted Average PCI by Ward":
                    WeightedAveragePCIByWard wapbwReport = new WeightedAveragePCIByWard(networkID, Global.SecurityOperations.CurrentUserName);
                    wapbwReport.CreateReport();
                    Global.WriteOutput("Done generating Weighted Average PCI by Ward report at " + DateTime.Now.TimeOfDay);
                    break;
                case "Weighted Average PCI by Functional Class":
                    WeightedAveragePCIByClass wapbfcReport = new WeightedAveragePCIByClass(networkID, Global.SecurityOperations.CurrentUserName);
                    wapbfcReport.CreateReport();
                    Global.WriteOutput("Done generating Weighted Average PCI by Functional Class report at " + DateTime.Now.TimeOfDay);
                    break;

                case "PGROUP To GIS Report":
                    PGROUPReport pgroupReport = new PGROUPReport(networkID, strSimulationID);
                    Global.WriteOutput("Done generating PGROUP To GIS Report at " + DateTime.Now.TimeOfDay);
                    break;
                case "PGROUP Aggregation Report":
                    AggregatePGroupReport pGroupReport = new AggregatePGroupReport(networkID, strSimulationID);
                    Global.WriteOutput("Done generating PGROUP Aggregation Report at " + DateTime.Now.TimeOfDay);
                    break;

                case "PennDot Bridge Report":
                    Global.WriteOutput("Done generating PennDot Bridge Report at " + DateTime.Now.TimeOfDay);
                    var penndot = new Reports.PennDotBridge.BridgeReport(networkID.ToString(), strSimulationID);
                    penndot.CreateReport();
                    break;

                default:
                    Global.WriteOutput("Requested report is not implemented.");
                    break;
            }
            //toMinimize.WindowState = previousState;
            //toMinimize.Cursor = Cursors.Arrow;
            //toMinimize.Show();
        }

        private static void ExportSectionViewReport(int networkID)
		{
			// Get existing section view if one exists that matches the network ID the user selected.
			FormSectionView formSectionView = FormManager.ListSectionView.Find(delegate(FormSectionView fsv) { return fsv.NetworkID == networkID.ToString(); });
			if (formSectionView == null)
			{
				Global.WriteOutput("Please open a network section view and review filters before running this report.");
			}
			else
			{
				formSectionView.ExportSectionReport();
			}
		}

        public void AddViewersAfterRollup(String strNetwork, String strNetworkID)
        {
            NetworkDefinition currNetDef = ((SolutionExplorerTreeNode)tvSolutionExplorer.SelectedNode).NetworkDefinition;
            SolutionExplorerTreeNode tnNetwork = new SolutionExplorerTreeNode(currNetDef);
            
            // Remove the old network
            tvSolutionExplorer.Nodes[currNetDef.NetDefName].Nodes["NodeNetwork" + currNetDef.NetDefName].Nodes.RemoveByKey(strNetwork + currNetDef.NetDefName);

            // Now re-add it with the new stuff beneath it.
            tvSolutionExplorer.Nodes[currNetDef.NetDefName].Nodes["NodeNetwork" + currNetDef.NetDefName].Nodes.Add(tnNetwork);
            tnNetwork.Tag = strNetworkID;
            tnNetwork.Name = strNetwork + currNetDef.NetDefName;
            tnNetwork.Text = strNetwork;

            //dsmelser 2008.07.30
            //added tagging for robust icon switching

            // Add Dynamic segmentation, viewers, committed projects, and simulations to THIS network
            SolutionExplorerTreeNode dynSegNode = new SolutionExplorerTreeNode(currNetDef);
            tnNetwork.Nodes.Add(dynSegNode);
            dynSegNode.Tag = strNetworkID + " Dynamic Segmentation";
            dynSegNode.Name = "NodeDynamicSegmentation" + currNetDef.NetDefName;
            dynSegNode.Text = "Dynamic Segmentation";

            SolutionExplorerTreeNode segLogicNode = new SolutionExplorerTreeNode(currNetDef);
            dynSegNode.Nodes.Add(segLogicNode);
            segLogicNode.Tag = strNetworkID + " Segmentation Logic";
            segLogicNode.Name = "NodeSegmentationLogic" + currNetDef.NetDefName;
            segLogicNode.Text = "Segmentation Logic";
            segLogicNode.ImageKey = Settings.Default.SEGMENTATION_LOGIC_IMAGE_KEY;
            segLogicNode.SelectedImageKey = Settings.Default.SEGMENTATION_LOGIC_IMAGE_KEY_SELECTED;

            SolutionExplorerTreeNode segResultsNode = new SolutionExplorerTreeNode(currNetDef);
            dynSegNode.Nodes.Add(segResultsNode);
            segResultsNode.Tag = strNetworkID + " Segmentation Results";
            segResultsNode.Name = "NodeSegmentationResults" + currNetDef.NetDefName;
            segResultsNode.Text = "Segmentation Results";
            segResultsNode.ImageKey = Settings.Default.SEGMENTATION_RESULTS_IMAGE_KEY;
            segResultsNode.SelectedImageKey = Settings.Default.SEGMENTATION_RESULTS_IMAGE_KEY_SELECTED;

            SolutionExplorerTreeNode rollupNode = new SolutionExplorerTreeNode(currNetDef);
            dynSegNode.Nodes.Add(rollupNode);
            rollupNode.Tag = strNetworkID + " Rollup";
            rollupNode.Name = "NodeRollup" + currNetDef.NetDefName;
            rollupNode.Text = "Rollup";
            rollupNode.ImageKey = Settings.Default.ROLLUP_IMAGE_KEY;
            rollupNode.SelectedImageKey = Settings.Default.ROLLUP_IMAGE_KEY_SELECTED;

            SolutionExplorerTreeNode viewersNode = new SolutionExplorerTreeNode(currNetDef);
            tnNetwork.Nodes.Add(viewersNode);
            viewersNode.Text = "Viewers";
            viewersNode.Tag = strNetworkID + " Viewers";
            viewersNode.Name = "NodeViewers" + currNetDef.NetDefName;

            //tempNode = tn.Nodes.Add("Construction History View");
            //tempNode.Tag = strNetworkID + " Construction History View";
            //tempNode.ImageKey = Settings.Default.CONSTRUCTION_HISTORY_VIEW_IMAGE_KEY;
            //tempNode.SelectedImageKey = Settings.Default.CONSTRUCTION_HISTORY_VIEW_IMAGE_KEY_SELECTED;
            SolutionExplorerTreeNode tnAttribute = new SolutionExplorerTreeNode(currNetDef);
            viewersNode.Nodes.Add(tnAttribute);
            tnAttribute.Text = "Attribute View";
            tnAttribute.Tag = strNetworkID + " Attribute View";
            tnAttribute.ContextMenuStrip = cmsAttributeViewRoot;
            tnAttribute.Name = "NodeAttributeView" + currNetDef.NetDefName;

            AddAttributteNodes(strNetwork, tnAttribute);

            SolutionExplorerTreeNode sectionViewNode = new SolutionExplorerTreeNode(currNetDef);
            viewersNode.Nodes.Add(sectionViewNode);
            sectionViewNode.Text = "Section View";
            sectionViewNode.Name = "NodeSectionView" + currNetDef.NetDefName;
            sectionViewNode.Tag = strNetworkID + " Section View";
            sectionViewNode.ImageKey = Settings.Default.SECTION_VIEW_IMAGE_KEY;
            sectionViewNode.SelectedImageKey = Settings.Default.SECTION_VIEW_IMAGE_KEY_SELECTED;

            SolutionExplorerTreeNode assetViewNode = new SolutionExplorerTreeNode(currNetDef);
            viewersNode.Nodes.Add(assetViewNode);
            assetViewNode.Text = "Asset View";
            assetViewNode.Name = "NodeAssetView" + currNetDef.NetDefName;
            assetViewNode.Tag = strNetworkID + " Asset View";
            assetViewNode.ImageKey = Settings.Default.ASSET_VIEW_IMAGE_KEY;
            assetViewNode.SelectedImageKey = Settings.Default.ASSET_VIEW_IMAGE_KEY_SELECTED;
            assetViewNode.ContextMenuStrip = cmsAssetView;

            SolutionExplorerTreeNode gisViewNode = new SolutionExplorerTreeNode(currNetDef);
            viewersNode.Nodes.Add(gisViewNode);
            gisViewNode.Text = "GIS View";
            gisViewNode.Name = "NodeGISView" + currNetDef.NetDefName;
            gisViewNode.Tag = strNetworkID + " GIS View";
            gisViewNode.ImageKey = Settings.Default.GIS_VIEW_IMAGE_KEY;
            gisViewNode.SelectedImageKey = Settings.Default.GIS_VIEW_IMAGE_KEY_SELECTED;

            SolutionExplorerTreeNode simulationsNode = new SolutionExplorerTreeNode(currNetDef);
            tnNetwork.Nodes.Add(simulationsNode);
            simulationsNode.Tag = strNetworkID + " Simulations";
            simulationsNode.Name = "NodeSimulations" + currNetDef.NetDefName;
            simulationsNode.Text = "Simulations";

            AddSimulationNodes(strNetworkID, simulationsNode);
        }

        private void AddNetworkNodesToTreeView(String strNetwork, NetworkDefinition currNetDef)
        {
            SolutionExplorerTreeNode tn = new SolutionExplorerTreeNode(currNetDef);
            SolutionExplorerTreeNode tempNode = new SolutionExplorerTreeNode(currNetDef);

            SolutionExplorerTreeNode tnNetwork = new SolutionExplorerTreeNode(currNetDef);
            tvSolutionExplorer.Nodes[currNetDef.NetDefName].Nodes["NodeNetwork" + currNetDef.NetDefName].Nodes.Add(tnNetwork);
            tnNetwork.Name = strNetwork + currNetDef.NetDefName;
            tnNetwork.Text = strNetwork;

			tnNetwork.ContextMenuStrip = cmsNetwork;

            // Check SECTION_networkID to see if this table exists, if it does show viewers
            // if it doesnt dont show any viewers under it
            String strQuery = "Select NETWORKID From NETWORKS Where NETWORK_NAME = '" + strNetwork + "'";
            String strNetworkID = "";
            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(strQuery);
                strNetworkID = ds.Tables[0].Rows[0].ItemArray[0].ToString(); 
            }
            catch (Exception sqlE)
            {
                Global.WriteOutput("Error selecting NETWORK from NETWORKS. " + sqlE.Message);
                return;
            }
            tnNetwork.Tag = strNetworkID;
            
            // Add Dynamic segmentation, viewers, committed projects, and simulations to each network
			//tnNetwork.Tag = strNetworkID;

            if (Global.SecurityOperations.CanViewDynamicSegmentation(strNetworkID))
            {

                tnNetwork.Nodes.Add(tn);
                tn.Text = "Dynamic Segmentation";
                tn.Name = "NodeDynamicSegmentation" + currNetDef.NetDefName;
                //dsmelser 2008.07.30
                //added node tagging for robust icon switching
                tn.Tag = strNetworkID + " Dynamic Segmentation" + currNetDef.NetDefName;

                if (Global.SecurityOperations.CanViewSegmentationLogic(strNetworkID))
                {
                    SolutionExplorerTreeNode toAdd = new SolutionExplorerTreeNode(currNetDef);
                    tn.Nodes.Add(toAdd);
                    toAdd.Text = "Segmentation Logic";
                    toAdd.Name = "NodeSegmentation Logic" + currNetDef.NetDefName;
                    toAdd.Tag = strNetworkID + " Segmentation Logic";
                    toAdd.ImageKey = Settings.Default.SEGMENTATION_LOGIC_IMAGE_KEY;
                    toAdd.SelectedImageKey = Settings.Default.SEGMENTATION_LOGIC_IMAGE_KEY_SELECTED;
                }

                if (Global.SecurityOperations.CanViewSegmentationResults(strNetworkID))
                {
                    SolutionExplorerTreeNode toAdd = new SolutionExplorerTreeNode(currNetDef);
                    tn.Nodes.Add(toAdd);
                    toAdd.Text = "Segmentation Results";
                    toAdd.Name = "NodeSegmentation Results" + currNetDef.NetDefName;
                    toAdd.Tag = strNetworkID + " Segmentation Results";
                    toAdd.ImageKey = Settings.Default.SEGMENTATION_RESULTS_IMAGE_KEY;
                    toAdd.SelectedImageKey = Settings.Default.SEGMENTATION_RESULTS_IMAGE_KEY_SELECTED;
                }

                if (Global.SecurityOperations.CanViewRollup(strNetworkID))
                {
                    SolutionExplorerTreeNode toAdd = new SolutionExplorerTreeNode(currNetDef);
                    tn.Nodes.Add(toAdd);
                    toAdd.Text = "Rollup";
                    toAdd.Name = "NodeRollup" + currNetDef.NetDefName;
                    toAdd.Tag = strNetworkID + " Rollup";
                    toAdd.ImageKey = Settings.Default.ROLLUP_IMAGE_KEY;
                    toAdd.SelectedImageKey = Settings.Default.ROLLUP_IMAGE_KEY_SELECTED;
                }
            }

            //if( Global.SecurityOperations.CanViewAnyViewers( strNetworkID ) )
            //{
            SolutionExplorerTreeNode toAddViewers = new SolutionExplorerTreeNode(currNetDef);
            tnNetwork.Nodes.Add(toAddViewers);
            toAddViewers.Text = "Viewers";
            toAddViewers.Name = "NodeViewers" + currNetDef.NetDefName;
            toAddViewers.Tag = strNetworkID + " Viewers";

            if (Global.SecurityOperations.CanViewConstructionHistoryView(strNetworkID))
            {
                //tempNode = tn.Nodes.Add( "Construction History View" );
                //tempNode.Tag = strNetworkID + " Construction History View";
                //tempNode.ImageKey = Settings.Default.CONSTRUCTION_HISTORY_VIEW_IMAGE_KEY;
                //tempNode.SelectedImageKey = Settings.Default.CONSTRUCTION_HISTORY_VIEW_IMAGE_KEY_SELECTED;
            }

            if (Global.SecurityOperations.CanViewAttributeView(strNetworkID))
            {
                SolutionExplorerTreeNode tnAttribute = new SolutionExplorerTreeNode(currNetDef);
                toAddViewers.Nodes.Add(tnAttribute);
                tnAttribute.Text = "Attribute View";
                tnAttribute.Name = "NodeAttributeView" + currNetDef.NetDefName;
                tnAttribute.Tag = strNetworkID + " Attribute View";
                tnAttribute.ContextMenuStrip = cmsAttributeViewRoot;

                AddAttributteNodes(strNetwork, tnAttribute);
            }

            if (Global.SecurityOperations.CanViewSectionView(strNetworkID))
            {
                SolutionExplorerTreeNode toAdd = new SolutionExplorerTreeNode(currNetDef);
                toAddViewers.Nodes.Add(toAdd);
                toAdd.Text = "Section View";
                toAdd.Tag = strNetworkID + " Section View";
                toAdd.Name = "NodeSectionView" + currNetDef.NetDefName;
                toAdd.ImageKey = Settings.Default.SECTION_VIEW_IMAGE_KEY;
                toAdd.SelectedImageKey = Settings.Default.SECTION_VIEW_IMAGE_KEY_SELECTED;
                toAdd.ContextMenuStrip = new ContextMenuStrip();
                toAdd.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Section Detail Report", null, tsmiReports_Click));
            }

            if (Global.SecurityOperations.CanViewAssetView(strNetworkID))
            {
                SolutionExplorerTreeNode toAdd = new SolutionExplorerTreeNode(currNetDef);
                toAddViewers.Nodes.Add(toAdd);
                toAdd.Text = "Asset View";
                toAdd.Name = "NodeAssetView" + currNetDef.NetDefName;
                toAdd.Tag = strNetworkID + " Asset View";
                toAdd.ImageKey = Settings.Default.ASSET_VIEW_IMAGE_KEY;
                toAdd.SelectedImageKey = Settings.Default.ASSET_VIEW_IMAGE_KEY_SELECTED;
                toAdd.ContextMenuStrip = cmsAssetView;
            }

            if (Global.SecurityOperations.CanViewGISView(strNetworkID))
            {
                SolutionExplorerTreeNode toAdd = new SolutionExplorerTreeNode(currNetDef);
                toAddViewers.Nodes.Add(toAdd);
                toAdd.Text = "GIS View";
                toAdd.Name = "NodeGISView" + currNetDef.NetDefName;
                toAdd.Tag = strNetworkID + " GIS View";
                toAdd.ImageKey = Settings.Default.GIS_VIEW_IMAGE_KEY;
                toAdd.SelectedImageKey = Settings.Default.GIS_VIEW_IMAGE_KEY_SELECTED;
            }
            //}

            //if( Global.SecurityOperations.CanViewAnySimulations( strNetworkID ) )
            //{
            SolutionExplorerTreeNode toAddSims = new SolutionExplorerTreeNode(currNetDef);
            tnNetwork.Nodes.Add(toAddSims);
            toAddSims.Text = "Simulations";
            toAddSims.Name = "NodeSimulation" + currNetDef.NetDefName;
            toAddSims.Tag = strNetworkID + " Simulations";
            AddSimulationNodes(strNetworkID, toAddSims);
            //}
        }

        /// <summary>
        /// Add nodes for all simulations for a given network
        /// </summary>
        /// <param name="strNetworkID">NetworkID</param>
        /// <param name="tn">Simulation treenode</param>
        private void AddSimulationNodes(String strNetworkID, SolutionExplorerTreeNode tn)
        {
            String strQuery = "Select SIMULATIONID,SIMULATION From SIMULATIONS Where NETWORKID = '" + strNetworkID + "' ORDER BY SIMULATION";
            DataSet ds = null;
            try
            {
                ds = DBMgr.ExecuteQuery(strQuery);
            }
            catch (Exception sqlE)
            {
                Global.WriteOutput("Error: Opening simulation table.  Message - " + sqlE.Message);
                return;
            }

            tn.ContextMenuStrip = cmsSimulationRoot;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                SolutionExplorerTreeNode tnSimulation = new SolutionExplorerTreeNode(tn.NetworkDefinition);
                tn.Nodes.Add(tnSimulation);
                tnSimulation.Text = row.ItemArray[1].ToString();
                tnSimulation.ContextMenuStrip = cmsSimulation;

				string strSimulationID = row.ItemArray[0].ToString();
				tnSimulation.Tag = strSimulationID;

				if( Global.SecurityOperations.CanViewSimulationAnalysis( strNetworkID, strSimulationID ) )
				{
                    SolutionExplorerTreeNode tnAnalysis = new SolutionExplorerTreeNode(tn.NetworkDefinition);
                    tnSimulation.Nodes.Add(tnAnalysis);
                    tnAnalysis.Name = row.ItemArray[0].ToString();
                    tnAnalysis.Text = "Analysis";
					tnAnalysis.ContextMenuStrip = cmsAnalysis;
					//dsmelser 2008.07.31
					//added tagging for robust icon switching
					//row.ItemArray[0].ToString() is the simulation ID
					tnAnalysis.Tag = strNetworkID + " " + row.ItemArray[0].ToString() + " Analysis";
					tnAnalysis.ImageKey = Settings.Default.SIMULATION_IMAGE_KEY;
					tnAnalysis.SelectedImageKey = Settings.Default.SIMULATION_IMAGE_KEY_SELECTED;
				}

				if( Global.SecurityOperations.CanViewSimulationInvestment( strNetworkID, strSimulationID ) )
				{
                    SolutionExplorerTreeNode tnInvestment = new SolutionExplorerTreeNode(tn.NetworkDefinition);
                    tnSimulation.Nodes.Add(tnInvestment);
                    tnInvestment.Name = row.ItemArray[0].ToString();
                    tnInvestment.Text = "Investment";
					tnInvestment.ContextMenuStrip = cmsInvestment;
					tnInvestment.Tag = strNetworkID + " " + row.ItemArray[0].ToString() + " Investment";
					tnInvestment.ImageKey = Settings.Default.SIMULATION_IMAGE_KEY;
					tnInvestment.SelectedImageKey = Settings.Default.SIMULATION_IMAGE_KEY_SELECTED;
				}

				if( Global.SecurityOperations.CanViewSimulationPerformance( strNetworkID, strSimulationID ) )
				{
                    SolutionExplorerTreeNode tnPerformance = new SolutionExplorerTreeNode(tn.NetworkDefinition);
                    tnSimulation.Nodes.Add(tnPerformance);
                    tnPerformance.Name = row.ItemArray[0].ToString();
                    tnPerformance.Text = "Performance";
                    tnPerformance.ContextMenuStrip = cmsPerformance;
                    tnPerformance.Tag = strNetworkID + " " + row.ItemArray[0].ToString() + " Performance";
                    tnPerformance.ImageKey = Settings.Default.SIMULATION_IMAGE_KEY;
                    tnPerformance.SelectedImageKey = Settings.Default.SIMULATION_IMAGE_KEY_SELECTED;
				}

				if( Global.SecurityOperations.CanViewSimulationTreatment( strNetworkID, strSimulationID ) )
				{
                    SolutionExplorerTreeNode tnTreatment = new SolutionExplorerTreeNode(tn.NetworkDefinition);
                    tnSimulation.Nodes.Add(tnTreatment);
                    tnTreatment.Name = row.ItemArray[0].ToString();
                    tnTreatment.Text = "Treatment";
					tnTreatment.ContextMenuStrip = cmsTreatment;
					tnTreatment.Tag = strNetworkID + " " + row.ItemArray[0].ToString() + " Treatment";
					tnTreatment.ImageKey = Settings.Default.SIMULATION_IMAGE_KEY;
					tnTreatment.SelectedImageKey = Settings.Default.SIMULATION_IMAGE_KEY_SELECTED;
				}

				if( Global.SecurityOperations.CanViewSimulationCommitted( strNetworkID, strSimulationID ) )
				{
                    SolutionExplorerTreeNode tnCommitted = new SolutionExplorerTreeNode(tn.NetworkDefinition);
					tnSimulation.Nodes.Add( tnCommitted );
                    tnCommitted.Text = "Committed";
                    tnCommitted.Name = row.ItemArray[0].ToString();
                    tnCommitted.Tag = strNetworkID + " " + row.ItemArray[0].ToString() + " Committed";
                    tnCommitted.ImageKey = Settings.Default.SIMULATION_IMAGE_KEY;
                    tnCommitted.SelectedImageKey = Settings.Default.SIMULATION_IMAGE_KEY_SELECTED;
				}

				if( Global.SecurityOperations.CanViewSimulationResults( strNetworkID, strSimulationID ) )
				{
                    SolutionExplorerTreeNode tnResults = new SolutionExplorerTreeNode(tn.NetworkDefinition);
					tnSimulation.Nodes.Add( tnResults );
                    tnResults.Name = row.ItemArray[0].ToString();
                    tnResults.Text = "Results";
                    tnResults.Tag = strNetworkID + " " + row.ItemArray[0].ToString() + " Results";
                    tnResults.ImageKey = Settings.Default.SIMULATION_IMAGE_KEY;
                    tnResults.SelectedImageKey = Settings.Default.SIMULATION_IMAGE_KEY_SELECTED;
				}
            }
        }

        private void AddAttributteNodes(string strNetwork, SolutionExplorerTreeNode tn)
        {
            String strNetworkID = tn.Parent.Parent.Tag.ToString();
            Hashtable hashAttributeYear = Global.GetAttributeYear(strNetworkID);
            List<String> listAttribute = new List<String>();
            foreach (String key in hashAttributeYear.Keys)
            {
                listAttribute.Add(key);
            }
            listAttribute.Sort();


            foreach (String strAttribute in listAttribute)
            {
				//if( Global.SecurityOperations.CanViewAttributeViewAttribute( strNetworkID, strAttribute ) )
                {
					if( hashAttributeYear.ContainsKey( strAttribute ) )
					{
                        SolutionExplorerTreeNode tnYear = new SolutionExplorerTreeNode(tn.NetworkDefinition);
						tn.Nodes.Add( tnYear );
                        tnYear.Text = strAttribute;
                        tnYear.Name = strAttribute + tn.NetworkDefinition.NetDefName;

						//dsmelser 2008.07.31
						//added tagging for robust icon switching
						tnYear.Tag = strNetworkID + " " + strAttribute + tn.NetworkDefinition.NetDefName;
						tnYear.ImageKey = Settings.Default.ATTRIBUTE_VIEW_IMAGE_KEY;
						tnYear.SelectedImageKey = Settings.Default.ATTRIBUTE_VIEW_IMAGE_KEY_SELECTED;

						List<String> listYear = ( List<String> )hashAttributeYear[strAttribute];

						foreach( String strYear in listYear )
						{
							//dsmelser 2008.07.31
							//added tagging for robust icon switching
                            SolutionExplorerTreeNode yearNode = new SolutionExplorerTreeNode(tn.NetworkDefinition);
							tnYear.Nodes.Add( yearNode );
                            yearNode.Text = strYear;
                            yearNode.Name = strYear + tn.NetworkDefinition.NetDefName;
                            yearNode.Tag = strNetworkID + " " + strAttribute + " " + strYear + tn.NetworkDefinition.NetDefName;
                            yearNode.ImageKey = Settings.Default.ATTRIBUTE_VIEW_SPECIFIC_YEAR_IMAGE_KEY;
                            yearNode.SelectedImageKey = Settings.Default.ATTRIBUTE_VIEW_SPECIFIC_YEAR_IMAGE_KEY_SELECTED;
						}
						//dsmelser 2008.07.31
						//added tagging for robust icon switching
                        SolutionExplorerTreeNode tempNode = new SolutionExplorerTreeNode(tn.NetworkDefinition);
                        tnYear.Nodes.Add(tempNode);
                        tempNode.Text = strAttribute;
                        tempNode.Name = strAttribute + tn.NetworkDefinition.NetDefName;
						tempNode.Tag = strNetworkID + " " + strAttribute + " " + strAttribute + tn.NetworkDefinition.NetDefName;
						tempNode.ImageKey = Settings.Default.ATTRIBUTE_VIEW_SPECIFIC_YEAR_IMAGE_KEY;
						tempNode.SelectedImageKey = Settings.Default.ATTRIBUTE_VIEW_SPECIFIC_YEAR_IMAGE_KEY_SELECTED;
					}
				}
            }
        }

        private void AddMenuToAllNodes(TreeNode treeNode)
        {
            // Add menu to node
            treeNode.ContextMenuStrip = menuStripSolutionExplorer;

            // Add menu to each node recursively
            foreach (TreeNode tn in treeNode.Nodes)
            {
                AddMenuToAllNodes(tn);
            }
        }

        private void tvSolutionExplorer_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                tvSolutionExplorer.SelectedNode = e.Node;
            }
        }

		private void tvSolutionExplorer_DoubleClick(object sender, EventArgs e)
		{
			// Checks to see what the user has clicked on in the solution explorer tree and either opens the tree nodes children,
			// or opens an associated form/tool window.
			String strAttribute = "";
			String strNetwork = "";
			String strAsset = "";

			//dsmelser 2008.10.23
			//it's possible to doubleclick the solution explorer without selecting a node
			if (tvSolutionExplorer.SelectedNode != null)
			{
                SolutionExplorerTreeNode currentNode = (SolutionExplorerTreeNode)tvSolutionExplorer.SelectedNode;
				if( tvSolutionExplorer.SelectedNode.Level != 0 )
				{
					#region AttributeViewerForm
					// If an attribute under View Attribute click;

					if (IsViewAttributeYear(tvSolutionExplorer.SelectedNode, out strAttribute, out strNetwork))
					{
						Hashtable hashAttributeYear = Global.GetAttributeYear(tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Parent.Tag.ToString());
						if (!FormManager.IsAttributeViewOpen(strNetwork))
						{
							FormAttributeView formAttributeView = new FormAttributeView(strNetwork, hashAttributeYear);
							formAttributeView.Show(m_dockPanel);
							FormManager.AddAttributeViewForm(formAttributeView);
						}
						List<String> listAdd = new List<String>();

                        List<String> listAttribute = new List<String>();
                        foreach (String key in hashAttributeYear.Keys)
                        {
                            listAttribute.Add(key);
                        }
                        listAttribute.Sort();

                        if (listAttribute.Contains(tvSolutionExplorer.SelectedNode.Text))
						{
							listAdd.Add(tvSolutionExplorer.SelectedNode.Text);
						}
						else
						{
							listAdd.Add(strAttribute + "_" + tvSolutionExplorer.SelectedNode.Text);
						}
						FormManager.GetCurrentAttributeView(strNetwork).UpdateAttributeView(listAdd, true);
					}


					if (IsSimulationAttributeYear(tvSolutionExplorer.SelectedNode))
					{
						throw new NotImplementedException();
					}

					if (IsViewAttribute(tvSolutionExplorer.SelectedNode, out strNetwork))
					{
						FormManager.IsAttributeViewOpen(strNetwork);
					}
					if (IsViewAttribute(tvSolutionExplorer.SelectedNode, out strAttribute, out strNetwork))
					{
						Hashtable hashAttributeYear = Global.GetAttributeYear(tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Tag.ToString());
						if (!FormManager.IsAttributeViewOpen(strNetwork))
						{
							FormAttributeView formAttributeView = new FormAttributeView(strNetwork, hashAttributeYear);
							formAttributeView.Show(m_dockPanel);
							FormManager.AddAttributeViewForm(formAttributeView);
						}


						List<String> listYear = (List<String>)hashAttributeYear[strAttribute];
						List<String> listAdd = new List<String>();

						if (Global.MostRecentFirst)
						{
							listAdd.Add(strAttribute);
							for (int i = listYear.Count - 1; i >= 0; i--)
							{
								listAdd.Add(strAttribute + "_" + listYear[i]);
							}
						}
						else
						{
							foreach (String strYear in listYear)
							{
								listAdd.Add(strAttribute + "_" + strYear);
							}
							listAdd.Add(strAttribute);
						}

						FormManager.GetCurrentAttributeView(strNetwork).UpdateAttributeView(listAdd, true);
					}

					#endregion

                    // Opens the Attribute document tab
                    if (currentNode.NetworkDefinition != null)
                    {
                        if (tvSolutionExplorer.SelectedNode.Parent.Name == "NodeCalculatedFields" + currentNode.NetworkDefinition.NetDefName)
                        {
                            strAttribute = tvSolutionExplorer.SelectedNode.Text;
                            if (strAttribute != "PCI" && strAttribute != "Calculated Assets" && strAttribute != "Asset To Attribute")
                            {
                                if (!FormManager.IsFormCalculatedFieldOpen(strAttribute))
                                {
                                    FormCalculatedField formCalculatedField = new FormCalculatedField(strAttribute, currentNode.NetworkDefinition);
                                    formCalculatedField.Show(m_dockPanel);
                                    formCalculatedField.Tag = strAttribute;
                                    FormManager.AddFormCalculatedField(formCalculatedField);
                                }
                            }
                            else if (strAttribute == "PCI")
                            {
                                FormPCIDocument pciDocument = new FormPCIDocument();
                                FormManager.AddPCIDocument(pciDocument);
                                pciDocument.TabText = "PCI";
                                pciDocument.Show(m_dockPanel);
                            }
                        }
                    }



					if( tvSolutionExplorer.SelectedNode.Name == "NodeUserManagement" )
					{
						FormSecurityUsers formSecurityUsers;
						if( !FormManager.IsFormSecurityUsersOpen( out formSecurityUsers ) )
						{
							formSecurityUsers = new FormSecurityUsers();
							formSecurityUsers.Tag = "SecurityUsers";
							FormManager.AddFormSecurityUsers( formSecurityUsers );
							formSecurityUsers.Text = "Users";
							formSecurityUsers.TabText = "Users";
							formSecurityUsers.Show( m_dockPanel );
						}
						else
						{
							formSecurityUsers.Focus();
						}
					}
					if( tvSolutionExplorer.SelectedNode.Name == "NodeUserGroupManagement" )
					{
						FormSecurityUserGroups formSecurityUserGroups;
						if( !FormManager.IsFormSecurityUserGroupsOpen( out formSecurityUserGroups ) )
						{
							formSecurityUserGroups = new FormSecurityUserGroups();
							formSecurityUserGroups.Tag = "SecurityUserGroups";
							FormManager.AddFormSecurityUserGroups( formSecurityUserGroups );
							formSecurityUserGroups.Text = "Usergroups";
							formSecurityUserGroups.TabText = "Usergroups";
							formSecurityUserGroups.Show( m_dockPanel );
						}
						else
						{
							formSecurityUserGroups.Focus();
						}
					}
					if( tvSolutionExplorer.SelectedNode.Name == "NodeActionManagement" )
					{
						FormSecurityActions formSecurityActions;
						if( !FormManager.IsFormSecurityActionsOpen( out formSecurityActions ) )
						{
							formSecurityActions = new FormSecurityActions();
							formSecurityActions.Tag = "SecurityActions";
							FormManager.AddFormSecurityActions( formSecurityActions );
							formSecurityActions.Text = "Actions";
							formSecurityActions.TabText = "Actions";
							formSecurityActions.Show( m_dockPanel );
						}
						else
						{
							formSecurityActions.Focus();
						}
					}
					if( tvSolutionExplorer.SelectedNode.Name == "NodeActionGroupManagement" )
					{
						FormSecurityActionGroups formSecurityActionGroups;
						if( !FormManager.IsFormSecurityActionGroupsOpen( out formSecurityActionGroups ) )
						{
							formSecurityActionGroups = new FormSecurityActionGroups();
							formSecurityActionGroups.Tag = "SecurityActionGroups";
							FormManager.AddFormSecurityActionGroups( formSecurityActionGroups );
							formSecurityActionGroups.Text = "Actiongroups";
							formSecurityActionGroups.TabText = "Actiongroups";
							formSecurityActionGroups.Show( m_dockPanel );
						}
						else
						{
							formSecurityActionGroups.Focus();
						}
					}
					if( tvSolutionExplorer.SelectedNode.Name == "NodePermissions" )
					{
						FormSecurityPermissions formSecurityPermissions;
						if( !FormManager.IsFormSecurityPermissionsOpen( out formSecurityPermissions ) )
						{
							formSecurityPermissions = new FormSecurityPermissions();
							formSecurityPermissions.Tag = "SecurityPermissions";
							FormManager.AddFormSecurityPermissions( formSecurityPermissions );
							formSecurityPermissions.Text = "Permissions";
							formSecurityPermissions.TabText = "Permissions";
							formSecurityPermissions.Show( m_dockPanel );
						}
						else
						{
							formSecurityPermissions.Focus();
						}
					}
		
					// If an attribute under View Asset click;
					if (IsAsset(tvSolutionExplorer.SelectedNode, out strAsset))
					{
						if (!FormManager.IsAssetsFormOpen(strAsset))
						{
							FormAssets formAssets = new FormAssets(strAsset);
							formAssets.Tag = strAsset;
							formAssets.TabText = "Asset-" + strAsset;
							FormManager.AddAssetsForm(formAssets);
							formAssets.Show(m_dockPanel);
						}
					}

					// Opens the Attribute document tab
                    if (currentNode.NetworkDefinition != null)
                    {
                        if (tvSolutionExplorer.SelectedNode.Parent.Name == "NodeAttribute" + currentNode.NetworkDefinition.NetDefName)
                        {
                            strAttribute = tvSolutionExplorer.SelectedNode.Text;
                            if (!FormManager.IsRawAttributeFormOpen(strAttribute))
                            {
                                // Shows the attribute selected
                                //if (DBMgr.TableExist(strAttribute))
                                //{
                                FormAttributeDocument attributeDocument = new FormAttributeDocument(strAttribute);
                                attributeDocument.Tag = strAttribute;
                                FormManager.AddRawAttributeForm(attributeDocument);
                                attributeDocument.Show(m_dockPanel);
                                //}

                                #region Some Tips for .Show
                                //int iTemp = dpc.IndexOf(m_solutionExplorer.Pane);
                                //formAttributeView.Show(dockPanelMain, DockState.DockRight);

                                // Causes a form to share a percentage of a space of another form.
                                //formAttributeView.Show(dockPanelMain.Panes[0], DockAlignment.Right, 1);

                                // Puts the pane on the panel in the rightmost position appeared as tabbed with another pane. docked right.
                                //formAttributeView.Show(dockPanelMain);

                                // Shows as non-dockable non modal form.
                                //formAttributeView.Show(dockPanelMain);

                                //paneAttributeView.MdiParent = this;
                                #endregion
                            }
                        }
                    }

					// Opens the network definiton tab based on linear or section selection
					OpenNetworkDefinitionPage();

					// Opens the segmentation tab
					if (tvSolutionExplorer.SelectedNode.Text == "Segmentation Logic")
					{
						String strNetworkName = tvSolutionExplorer.SelectedNode.Parent.Parent.Text;
						if (!FormManager.IsFormSegmentationOpen(strNetworkName))
						{
							FormSegmentation formSegmentation = new FormSegmentation(strNetworkName);
							formSegmentation.Tag = strNetworkName;
							FormManager.AddFormSegmentation(formSegmentation);
							formSegmentation.Show(m_dockPanel);
						}
					}

					// View and modify results of dynamic segmentation tab
					if (tvSolutionExplorer.SelectedNode.Text == "Segmentation Results")
					{
						String strNetworkName = tvSolutionExplorer.SelectedNode.Parent.Parent.Text;
						if (!FormManager.IsFormSegmentationResultOpen(strNetworkName))
						{
							FormSegmentationResult formSegmentationResult = new FormSegmentationResult(strNetworkName);
							formSegmentationResult.Tag = strNetworkName;
							FormManager.AddFormSegmentationResult(formSegmentationResult);
							formSegmentationResult.Show(m_dockPanel);
						}

					}

					// Opens the database rollup tab
					if (tvSolutionExplorer.SelectedNode.Text == "Rollup")
					{
						String strNetworkName = tvSolutionExplorer.SelectedNode.Parent.Parent.Text;
						if (!FormManager.IsFormRollupSegmentationOpen(strNetworkName))
						{
							FormRollupSegmentation formRollup = new FormRollupSegmentation(strNetworkName);
							formRollup.Tag = strNetworkName;
							FormManager.AddFormRollupSegmentation(formRollup);
							formRollup.Show(m_dockPanel);
						}

					}

					// Opens the construction history tab
					if (tvSolutionExplorer.SelectedNode.Text == "Construction History View")
					{
						String strNetworkName = tvSolutionExplorer.SelectedNode.Parent.Parent.Text;
						if (!FormManager.IsFormSegmentedConstructionOpen(strNetworkName))
						{
							FormSegmentedConstruction formConstruction = new FormSegmentedConstruction(strNetworkName);
							formConstruction.Tag = strNetworkName;
							FormManager.AddFormSegmentedConstruction(formConstruction);
							formConstruction.Show(m_dockPanel);
						}

					}
					if (tvSolutionExplorer.SelectedNode.Text == "Construction Data")
					{
						if (!FormManager.IsFormConstructionHistoryOpen())
						{
							FormConstructionHistory construction = new FormConstructionHistory();
							construction.Tag = tvSolutionExplorer.SelectedNode.Tag;
							FormManager.AddFormConstructionHistory(construction);
							construction.Show(m_dockPanel);
						}
					}

					if (tvSolutionExplorer.SelectedNode.Text == "Section View")
					{
						String strNetworkID = (String)tvSolutionExplorer.SelectedNode.Parent.Parent.Tag;
						String strNetworkName = tvSolutionExplorer.SelectedNode.Parent.Parent.Text;
						FormSectionView formSectionView;
						if (!FormManager.IsFormSectionViewOpen(strNetworkID, out formSectionView))
						{
							Hashtable hashAttributeYear = Global.GetAttributeYear(tvSolutionExplorer.SelectedNode.Parent.Parent.Tag.ToString());
							formSectionView = new FormSectionView(strNetworkName, hashAttributeYear);
							formSectionView.Tag = tvSolutionExplorer.SelectedNode.Tag;
							formSectionView.Show(m_dockPanel);
						}
					}

					if (tvSolutionExplorer.SelectedNode.Text == "GIS View")
					{
						String strNetworkName = tvSolutionExplorer.SelectedNode.Parent.Parent.Text;
						FormGISView formGISViewer;
						FormGISLayerManager formGISLayerManager;

						Hashtable hashAttributeYear = Global.GetAttributeYear(tvSolutionExplorer.SelectedNode.Parent.Parent.Tag.ToString());
						if (!FormManager.IsFormGISViewOpen(strNetworkName, out formGISViewer))
						{
							formGISViewer = new FormGISView(strNetworkName, hashAttributeYear);
							formGISViewer.Tag = strNetworkName;
							formGISViewer.TabText = tvSolutionExplorer.SelectedNode.Parent.Text;
							FormManager.AddFormGISView(formGISViewer);
							formGISViewer.Show(m_dockPanel);
						}

						AttributeTab attributeTab;
						if (!FormManager.IsAttributeTabOpen(out attributeTab))
						{
							attributeTab = new AttributeTab(strNetworkName, hashAttributeYear);
							FormManager.AddAttributeTab(attributeTab);
							attributeTab.Show(m_dockPanel, DockState.DockRight);
						}
						AssetTab assetTab;
						if (!FormManager.IsAssetTabOpen(out assetTab))
						{
							assetTab = new AssetTab(strNetworkName, hashAttributeYear);
							FormManager.AddAssetTab(assetTab);
							assetTab.Show(m_dockPanel, DockState.DockRight);
						}

						if (!FormManager.IsFormGISLayerManagerOpen(out formGISLayerManager))
						{
							hashAttributeYear = Global.GetAttributeYear(tvSolutionExplorer.SelectedNode.Parent.Parent.Tag.ToString());
							formGISLayerManager = new FormGISLayerManager(strNetworkName, hashAttributeYear);
							FormManager.AddFormGISLayerManager(formGISLayerManager);
							formGISLayerManager.Show(m_dockPanel, DockState.DockRight);
						}
						formGISLayerManager.MapImage = formGISViewer.MapImage;
						formGISLayerManager.ClearLayerManagerTreeView();
						formGISLayerManager.Tag = strNetworkName;
					}

					// Opens the Simulation Analysis tab
					if (tvSolutionExplorer.SelectedNode.Text == "Analysis" && tvSolutionExplorer.SelectedNode.Level == 5)
					{
						String strNetworkName = tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Text;
						String strSimulationName = tvSolutionExplorer.SelectedNode.Parent.Text;
						String strSimID = tvSolutionExplorer.SelectedNode.Name;


						FormAnalysis formAnalysis;
						if (!FormManager.IsFormAnalysisOpen(strSimID, out formAnalysis))
						{
							Hashtable hashAttributeYear = Global.GetAttributeYear(tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Tag.ToString());
							formAnalysis = new FormAnalysis(strNetworkName, strSimulationName, strSimID, hashAttributeYear);

							// For the simulation pages, we need to use the strSimID variable instead of the network name
							// to identify the individual open simulation pages in their respective lists.
							formAnalysis.Tag = strSimID;
							FormManager.AddFormAnalysis(formAnalysis);
							formAnalysis.TabText = strSimulationName + "-Analysis";
							formAnalysis.Show(m_dockPanel);
						}
					}

					if (tvSolutionExplorer.SelectedNode.Text == "Investment" && tvSolutionExplorer.SelectedNode.Level == 5)
					{
						String strNetworkName = tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Text;
						String strSimulationName = tvSolutionExplorer.SelectedNode.Parent.Text;
						String strSimID = tvSolutionExplorer.SelectedNode.Name;

						FormInvestment formInvestment;
						if (!FormManager.IsFormInvestmentOpen(strSimID, out formInvestment))
                        {
                            Hashtable hashAttributeYear = Global.GetAttributeYear(tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Tag.ToString());
                            formInvestment = new FormInvestment(strNetworkName, strSimulationName, strSimID, hashAttributeYear);
							formInvestment.Tag = strSimID;
							FormManager.AddFormInvestment(formInvestment);
							formInvestment.TabText = strSimulationName + "-Investment";
							formInvestment.Show(m_dockPanel);

						}
					}


					if (tvSolutionExplorer.SelectedNode.Text == "Performance" && tvSolutionExplorer.SelectedNode.Level == 5)
					{
						String strNetworkName = tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Text;
						String strSimulationName = tvSolutionExplorer.SelectedNode.Parent.Text;
						String strSimID = tvSolutionExplorer.SelectedNode.Name;

						FormPerformanceEquations formPerformance;
						if (!FormManager.IsFormPerformanceEquationsOpen(strSimID, out formPerformance))
						{
							Hashtable hashAttributeYear = Global.GetAttributeYear(tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Tag.ToString());
							formPerformance = new FormPerformanceEquations(strNetworkName, strSimulationName, strSimID, hashAttributeYear);
							formPerformance.Tag = strSimID;

							FormManager.AddFormPerformanceEquations(formPerformance);
							formPerformance.TabText = strSimulationName + "-Performance";
							formPerformance.Show(m_dockPanel);
						}

					}

					if (tvSolutionExplorer.SelectedNode.Text == "Treatment" && tvSolutionExplorer.SelectedNode.Level == 5)
					{
						String strNetworkName = tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Text;
						String strSimulationName = tvSolutionExplorer.SelectedNode.Parent.Text;
						String strSimID = tvSolutionExplorer.SelectedNode.Name;
						FormTreatment formTreatment;

						if (!FormManager.IsFormTreatmentOpen(strSimID, out formTreatment))
						{
							Hashtable hashAttributeYear = Global.GetAttributeYear(tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Tag.ToString());
							formTreatment = new FormTreatment(strNetworkName, strSimulationName, strSimID, hashAttributeYear);
							formTreatment.Tag = strSimID;
							FormManager.AddFormTreatment(formTreatment);
							formTreatment.TabText = strSimulationName + "-Treatments";
							formTreatment.Show(m_dockPanel);
						}
					}

					if (tvSolutionExplorer.SelectedNode.Text == "Committed" && tvSolutionExplorer.SelectedNode.Level == 5)
					{
						String strNetworkName = tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Text;
						String strNetworkID = tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Tag.ToString();
						String strSimulationName = tvSolutionExplorer.SelectedNode.Parent.Text;
						String strSimID = tvSolutionExplorer.SelectedNode.Name;
						FormSimulationResults formSimulation;

						if( DBMgr.IsTableInDatabase( "SIMULATION_" + strNetworkID + "_" + strSimID ) )
						{
							if( !FormManager.IsFormSimulationCommittedOpen( strSimID, out formSimulation ) )
							{
								formSimulation = new FormSimulationResults( strNetworkName, strSimulationName, strSimID, strNetworkID, true );
								formSimulation.Tag = strSimID;
								FormManager.AddFormSimulationCommitted( formSimulation );
								formSimulation.TabText = strSimulationName + "-Committed";
								formSimulation.Show( m_dockPanel );

							}
						}
						else
						{
							Global.WriteOutput( "Error: No results yet exist, please run a simulation before opening the committed projects tab." );
						}
					}


					if (tvSolutionExplorer.SelectedNode.Text == "Results" && tvSolutionExplorer.SelectedNode.Level == 5)
					{
						String strNetworkName = tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Text;
						String strNetworkID = tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Tag.ToString();
						String strSimulationName = tvSolutionExplorer.SelectedNode.Parent.Text;
						String strSimID = tvSolutionExplorer.SelectedNode.Name;

						if (DBMgr.IsTableInDatabase("SIMULATION_" + strNetworkID + "_" + strSimID))
						{
							FormSimulationResults formSimulation;
							if (!FormManager.IsFormSimulationResultsOpen(strSimID, out formSimulation))
							{
								formSimulation = new FormSimulationResults(strNetworkName, strSimulationName, strSimID, strNetworkID, false);
								formSimulation.Tag = strSimID;
								FormManager.AddFormSimulationResults(formSimulation);
								formSimulation.TabText = strSimulationName + "-Results";
								formSimulation.Show(m_dockPanel);
							}
						}
						else
						{
							Global.WriteOutput("Error: No results yet exist, please run a simulation before opening the results tab.");
						}
					}

					if (tvSolutionExplorer.SelectedNode.Text == "Asset View")
					{
						Hashtable hashAttributeYear = Global.GetAttributeYear(tvSolutionExplorer.SelectedNode.Parent.Parent.Tag.ToString());
						String strNetworkID = tvSolutionExplorer.SelectedNode.Parent.Parent.Tag.ToString();
						FormAssetView formAssetView = new FormAssetView(strNetworkID, hashAttributeYear);
						formAssetView.Show(m_dockPanel);
					}

                    if (currentNode.NetworkDefinition != null)
                    {
                        if (tvSolutionExplorer.SelectedNode.Name == "NodeCalculatedAssets" + currentNode.NetworkDefinition.NetDefName)
                        {
                            FormAssetsCalculated formAssetsCalculated;
                            if (!FormManager.IsFormCalculatedAssetsOpen())
                            {
                                formAssetsCalculated = new FormAssetsCalculated();
                                FormManager.AddFormCalculatedAssets(formAssetsCalculated);
                                formAssetsCalculated.Show(m_dockPanel);
                            }
                        }
                    }

                    if (tvSolutionExplorer.SelectedNode.Text == "Asset To Attribute")
                    {

                        FormAssetToAttribute formAssetToAttribute;
                        if (!FormManager.IsFormAssetToAttributeOpen())
                        {
                            formAssetToAttribute = new FormAssetToAttribute();
                            FormManager.AddFormAssetToAttribute(formAssetToAttribute);
                            formAssetToAttribute.Show(m_dockPanel);
                        }
                    }

				}
				else // Top level node
				{
				}
			}
		}

        private bool IsSimulationAttributeYear(TreeNode tn)
        {
            String strAttribute = "";
            String strNetwork = "";
            String strNetworkID = "";
            String strSimulationID;
            String strYear = "";
            if (tn.Level < 5)//Simulation attributes are at 5, years are at level 6.
            {
                return false;
            }

            if (tn.Parent.Parent.Text == "Simulation Attribute" )//We are on a level 6 TreeNode that is not in Attribute view.
            {
                strSimulationID = tn.Parent.Parent.Parent.Tag.ToString();
                strNetworkID = tn.Parent.Parent.Parent.Parent.Parent.Tag.ToString();
                strAttribute = tn.Parent.Text;
                strNetwork = tn.Parent.Parent.Parent.Parent.Parent.Text;
                strYear = tn.Text;
                return true;
            }
            else if (tn.Parent.Text == "Simulation Attribute")
            {
                strSimulationID = tn.Parent.Parent.Tag.ToString();
                strNetworkID = tn.Parent.Parent.Parent.Parent.Tag.ToString();
                strAttribute = tn.Text;
                strNetwork = tn.Parent.Parent.Parent.Parent.Text;
                return true;
            }
            return false;
        }

        private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Bring up the add new attribute dialog window.
            switch (tvSolutionExplorer.SelectedNode.Name)
            {
                case "NodeNetwork":
                    //NetworkProperties networkProp = new NetworkProperties("");
                    //String strNewNetwork = ShowProperties(true, networkProp, "Add New Network");
                    //if (strNewNetwork != "")
                    //{
                    //    AddNetworkNodesToTreeView(strNewNetwork);
                    //    //AddMenuToAllNodes(tvSolutionExplorer.SelectedNode);
                    //}
                    break;
                case "NodeAttribute":
                    //AttributeProperties attributeProp = new AttributeProperties();
                    //String strNewAttribute = ShowProperties(true, attributeProp, "Add New Attribute");                    
                    //if (strNewAttribute != "")
                    //{
                    //    tvSolutionExplorer.Nodes["NodeAttribute"].Nodes.Add(strNewAttribute);
                    //}
                    break;
                case "NodeAsset":
                    //String strNewAsset = CreateNewAsset();
                    //if (strNewAsset != "")
                    //{
                    //    tvSolutionExplorer.Nodes["NodeAsset"].Nodes.Add(strNewAsset);
                    //}
                    break;
                case "NodeSimulation":
                    //String strNetwork = tvSolutionExplorer.SelectedNode.Parent.Text;
                    //String strNetworkID = tvSolutionExplorer.SelectedNode.Parent.Tag.ToString();
                    //CreateNewSimulation(strNetworkID);
                    break;
                default:
                    break;
            }
            return;
        }

        /// <summary>
        /// Generate a new Simulation under a given network.
        /// </summary>
		private String CreateSimulation(String strNetworkID, bool bIsNewSimulation)
		{
            SolutionExplorerTreeNode selectedNode = (SolutionExplorerTreeNode)tvSolutionExplorer.SelectedNode;
            NetworkDefinition currNetDef = selectedNode.NetworkDefinition;
            SolutionExplorerTreeNode tempNode = new SolutionExplorerTreeNode(currNetDef);
			String strSimulationID = "";
			FormSimulationName formSimulationName = new FormSimulationName(bIsNewSimulation, strNetworkID);
			if (formSimulationName.ShowDialog() == DialogResult.OK)
			{
				if (!Global.CheckString(formSimulationName.SimulationName)) return null;
				String strInsert = "";
					switch (DBMgr.NativeConnectionParameters.Provider)
					{
						case "MSSQL":
							strInsert = "INSERT INTO SIMULATIONS (NETWORKID,SIMULATION,DATE_CREATED,ANALYSIS,BUDGET_CONSTRAINT,WEIGHTING,COMMITTED_START,COMMITTED_PERIOD)"
								+ " VALUES ('"
								+ formSimulationName.NewNetworkID + "','"
								+ formSimulationName.SimulationName + "','"
								+ DateTime.Now.Date.ToString() + "','"
								+ "Incremental Benefit/Cost" + "','"
								+ "As Budget Permits" + "','"
								+ "none" + "','"
								+ DateTime.Now.Year.ToString() + "','"
								+ "5" + "')";
							break;
						case "ORACLE":
							strInsert = "INSERT INTO SIMULATIONS (NETWORKID,SIMULATION,DATE_CREATED,ANALYSIS,BUDGET_CONSTRAINT,WEIGHTING,COMMITTED_START,COMMITTED_PERIOD)"
								+ " VALUES ('"
								+ formSimulationName.NewNetworkID + "','"
								+ formSimulationName.SimulationName + "','"
								+ DateTime.Now.Date.ToString("dd/MMM/yyyy") + "','"
								+ "Incremental Benefit/Cost" + "','"
								+ "As Budget Permits" + "','"
								+ "none" + "','"
								+ DateTime.Now.Year.ToString() + "','"
								+ "5" + "')";
							break;
						default:
							throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
							//break;
					}

				try
				{
					DBMgr.ExecuteNonQuery(strInsert);
				}
				catch (Exception exception)
				{
					Global.WriteOutput("Error inserting new Simulation." + exception.Message);
					return null;
				}
				String strSelect;

				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						strSelect = "SELECT IDENT_CURRENT ('SIMULATIONS') FROM SIMULATIONS";
						break;
					case "ORACLE":
						//strSelect = "SELECT LAST_NUMBER FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'SIMULATIONS_SIMULATIONID_SEQ'";
						//last number is wrong here...
						//SEQUENCE is not guaranteed to be increased by the correct amount...
						//strSelect = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'SIMULATIONS_SIMULATIONID_SEQ'";

						//CAN'T RELY ON LAST_NUMBER-CACHE_SIZE either, USER_SEQUENCES doesn't have any useful information...
						//this works until some kind of rollover...
						strSelect = "SELECT MAX(SIMULATIONID) FROM SIMULATIONS";
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}
				DataSet ds = new DataSet();

				try
				{
					ds = DBMgr.ExecuteQuery(strSelect);
				}
				catch (Exception exception)
				{
					Global.WriteOutput("Error inserting new Simulation." + exception.Message);
					return null;
				}

				strSimulationID = ds.Tables[0].Rows[0].ItemArray[0].ToString();
				strInsert = "INSERT INTO INVESTMENTS (SIMULATIONID,FIRSTYEAR,NUMBERYEARS,INFLATIONRATE,DISCOUNTRATE,BUDGETORDER,DESCRIPTION) VALUES('"
					+ strSimulationID + "','"
					+ DateTime.Now.Year.ToString() + "','"
					+ "5','"
					+ "2','"
					+ "3','"
                    + "Rehabilitation,Maintenance,Construction','"
                    + "new simulation')";

				try
				{
					DBMgr.ExecuteNonQuery(strInsert);
				}
				catch (Exception exception)
				{
					Global.WriteOutput("Error inserting new Simulation." + exception.Message);
					return null;
				}

				strInsert = "INSERT INTO TREATMENTS (SIMULATIONID,TREATMENT,BEFOREANY,BEFORESAME,DESCRIPTION) VALUES('"
					+ strSimulationID + "','"
					+ "No Treatment" + "','"
					+ "1','"
					+ "1','"
					+ "Default Treatment')";

				try
				{
					DBMgr.ExecuteNonQuery(strInsert);
				}
				catch (Exception exception)
				{
					Global.WriteOutput("Error inserting new Simulation." + exception.Message);
					return null;
				}


				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						strSelect = "SELECT IDENT_CURRENT ('TREATMENTS') FROM TREATMENTS";
						break;
					case "ORACLE":
						//strSelect = "SELECT TREATMENTS_TREATMENTID_SEQ.CURRVAL FROM DUAL";
						//strSelect = "SELECT LAST_NUMBER  - CACHE_SIZE FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'TREATMENTS_TREATMENTID_SEQ'";
						strSelect = "SELECT MAX(TREATMENTID) FROM TREATMENTS";
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}

				try
				{
					ds = DBMgr.ExecuteQuery(strSelect);
				}
				catch (Exception exception)
				{
					Global.WriteOutput("Error inserting new Simulation." + exception.Message);
					return null;
				}
				String strTreatmentID = ds.Tables[0].Rows[0].ItemArray[0].ToString();
				String strQuery = "SELECT ATTRIBUTE_ FROM ATTRIBUTES_ WHERE lower(ATTRIBUTE_) = 'age'";
				String strAge = "";
				try
				{
					ds = DBMgr.ExecuteQuery(strQuery);
					strAge = ds.Tables[0].Rows[0].ItemArray[0].ToString();
					strInsert = "INSERT INTO CONSEQUENCES (TREATMENTID,ATTRIBUTE_,CHANGE_) VALUES('"
					+ strTreatmentID + "','"
					+ strAge + "','"
					+ "+1')";

					DBMgr.ExecuteNonQuery(strInsert);
				}
				catch (Exception exc)
				{
					Global.WriteOutput("Error: AGE attribute does not exist. Simulation will not run without an AGE attribute." + exc.Message);
				}

                SolutionExplorerTreeNode simulationNode = new SolutionExplorerTreeNode(currNetDef);
				if (bIsNewSimulation)
				{
					//tvSolutionExplorer.SelectedNode.Nodes.Add(formSimulationName.SimulationName);
                    tvSolutionExplorer.SelectedNode.Nodes.Add(simulationNode);
				}
				else
				{
                    TreeNodeCollection tnNetworkNodes = tvSolutionExplorer.Nodes[currNetDef.NetDefName].Nodes["NodeNetwork" + currNetDef.NetDefName].Nodes;
					for (int i = 0; i < tnNetworkNodes.Count; i++)
					{
						// Find the correct network node to add the new simulation node to.
						if (tnNetworkNodes[i].Tag.ToString() == formSimulationName.NewNetworkID)
						{
                            tnNetworkNodes[i].Nodes["NodeSimulation" + currNetDef.NetDefName].Nodes.Add(simulationNode);
						}
					}
				}
                simulationNode.Text = formSimulationName.SimulationName;
                simulationNode.Name = strSimulationID;
                simulationNode.Tag = strSimulationID;

				//dsmelser 2008.07.31
				//added tagging for robust icon switching
                SolutionExplorerTreeNode tnAnalysis = new SolutionExplorerTreeNode(currNetDef);
                simulationNode.Nodes.Add(tnAnalysis);
				tnAnalysis.ContextMenuStrip = cmsAnalysis;
				tnAnalysis.Tag = formSimulationName.NewNetworkID + " " + strSimulationID + " Analysis";
                tnAnalysis.Text = "Analysis";
                tnAnalysis.Name = strSimulationID;

                SolutionExplorerTreeNode tnInvestment = new SolutionExplorerTreeNode(currNetDef);
                simulationNode.Nodes.Add(tnInvestment);
                tnInvestment.Text = "Investment";
                tnInvestment.Name = strSimulationID;
				tnInvestment.ContextMenuStrip = cmsInvestment;
				tnInvestment.Tag = formSimulationName.NewNetworkID + " " + strSimulationID + " Investment";

                SolutionExplorerTreeNode tnPerformance = new SolutionExplorerTreeNode(currNetDef);
                simulationNode.Nodes.Add(tnPerformance);
                tnPerformance.Text = "Performance";
                tnPerformance.Name = strSimulationID;
                tnPerformance.ContextMenuStrip = cmsPerformance;
                tnPerformance.Tag = formSimulationName.NewNetworkID + " " + strSimulationID + " Performance";

                SolutionExplorerTreeNode tnTreatment = new SolutionExplorerTreeNode(currNetDef);
                simulationNode.Nodes.Add(tnTreatment);
                tnTreatment.Text = "Treatment";
                tnTreatment.Name = strSimulationID;
                tnTreatment.ContextMenuStrip = cmsTreatment;
                tnTreatment.Tag = formSimulationName.NewNetworkID + " " + strSimulationID + " Treatment";

                SolutionExplorerTreeNode tnCommitted = new SolutionExplorerTreeNode(currNetDef);
                simulationNode.Nodes.Add(tnCommitted);
                tnCommitted.Text = "Committed";
                tnCommitted.Name = strSimulationID;
                tnCommitted.Tag = formSimulationName.NewNetworkID + " " + strSimulationID + " Committed";

                SolutionExplorerTreeNode tnResults = new SolutionExplorerTreeNode(currNetDef);
                simulationNode.Nodes.Add(tnResults);
                tnResults.Text = "Results";
                tnResults.Name = strSimulationID;
                tnResults.Tag = formSimulationName.NewNetworkID + " " + strSimulationID + " Results";

                simulationNode.ContextMenuStrip = cmsSimulation;
			}


            //Load OCI_WEIGHTS

            String omsConnectionString = ImportOMS.GetOMSConnectionString(DBMgr.GetNativeConnection().ConnectionString);
            if(!String.IsNullOrWhiteSpace(omsConnectionString))
            {
                List<String> assets = ImportOMS.GetOMSAssets();
                PrepareAnalysis.ConditionCategoryWeights(strSimulationID, assets);
                //CreateScenario.InsertOMSActivities(strSimulationID, assets);
                foreach (String asset in assets)
                {
                    CreateScenario.LoadPerformanceCurvesToDecisionEngine(Convert.ToInt32(strSimulationID), asset, true);
                }
            }
			return strSimulationID;
		}

        private String CreateNewAsset()
        {
            // Create a new assets property grid.  This information will be saved into a new table in the database
            // When the user selects properties on the new asset, the database will provide the information to dynamically create
            // the new property grid.
            String strNewAsset = "";
			FormEditAsset newAssetForm = new FormEditAsset();
			newAssetForm.Text = "Add New Asset";
			if (newAssetForm.ShowDialog() == DialogResult.OK)
			{
				strNewAsset = newAssetForm.Asset;
				newAssetForm.Close();
			}
            return strNewAsset;
        }

        /// <summary>
        /// Queries the table for the name and returns with the material information.
        /// </summary>
        /// <returns></returns>
        private CustomClass GetProperties(String strName)
        {
            // First, query the table.
			CustomClass cc = new CustomClass(strName);
			string columnQuery = "";

			switch( DBMgr.NativeConnectionParameters.Provider )
			{
				case "MSSQL":
					columnQuery = "SELECT TOP 1 * From " + strName;
					break;
				case "ORACLE":
					columnQuery = "SELECT * From " + strName + " WHERE ROWNUM = 1";
					break;
				default:
					throw new NotImplementedException( "TODO: Implement ANSI version of GetProperties()" );
					//break;
			}
			
			
            try
            {
				DataSet ds = DBMgr.ExecuteQuery( columnQuery );
                foreach (DataColumn dataColumn in ds.Tables[0].Columns)
                {
                    CustomProperty cp = new CustomProperty(dataColumn.ColumnName, dataColumn.DataType, false, true);
                    cc.Add(cp);
                }
            }
            catch (Exception sqlE)
            {
				Global.WriteOutput("Error: Failed to select from ASSETS table. " + sqlE.Message);
            }
            return cc;
        }

        private void TreeViewSelectionChanged(SolutionExplorerTreeNode tn)
        {
            String strSelectedProperty = tvSolutionExplorer.SelectedNode.Text;
            if (Global.GetPropertyWindow() != null)
            {
                if (!Global.GetPropertyWindow().IsDisposed)
                {
                    if (tvSolutionExplorer.SelectedNode.Parent != null)
                    {
                        if (tvSolutionExplorer.SelectedNode.Parent.Name == "NodeNetwork" + tn.NetworkDefinition.NetDefName)
                        {
                            // Instantiate the network properties class
                            NetworkProperties networkProp = new NetworkProperties(strSelectedProperty);
                            ShowProperties(false, networkProp, "");
                        }
                        if (tvSolutionExplorer.SelectedNode.Parent.Name == "NodeAttribute" + tn.NetworkDefinition.NetDefName)
                        {
                            AttributeProperties attributeProp = new AttributeProperties(strSelectedProperty);
                            ShowProperties(false, attributeProp, "");
                        }
                        if (tvSolutionExplorer.SelectedNode.Parent.Name == "NodeAsset" + tn.NetworkDefinition.NetDefName)
                        {
                            // Set up the asset property grid, get the selected object out of that grid, and pass that object as the property class to show properties!
                            AssetProperties assetProp = new AssetProperties(strSelectedProperty);
                            ShowProperties(false, assetProp, "");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Allows a properties class to be displayed in either a modal dialog property grid, or a tool window property grid.
        /// </summary>
        /// <param name="bIsNewWindow">True if showing a new modal dialog, false if showing the tool window property grid.</param>
        private String ShowProperties(bool bIsNewWindow, object oPropertyClass, String strDialogTitle)
        {
            String strProperty = "";
            // Check for creation of modal or tool window properties pane
            if (!bIsNewWindow)
            {
                // If we have a property pane, we need to check to see if it already exists, if it does, then we give the
                // control focus on the screen, otherwise (the if statement) we show it in the default location.
                Global.ShowPropertyGrid(oPropertyClass, false, "");
                if (!Global.GetPropertyWindow().Visible)
                {
                    if (this.Pane.IsAutoHide == true)
                    {
                        Global.GetPropertyWindow().Show(m_dockPanel, DockState.DockLeft);
                    }
                    else
                    {
                        Global.GetPropertyWindow().Show(this.Pane, DockAlignment.Bottom, 0.5);
                    }
                }
                else
                {
                    Global.GetPropertyWindow().Focus();
                }
            }
            else
            {
                Global.ShowPropertyGrid(oPropertyClass, true, strDialogTitle);
				FormPropertiesModal modalPropertyWindow = Global.GetModalPropertyWindow();
				modalPropertyWindow.ShowDialog();
                if (modalPropertyWindow.IsValidated)
                {
                    strProperty = Global.GetModalPropertyWindow().GetAddedProperty();
                }
            }
            return strProperty;
        }

        private bool IsAsset(TreeNode tn, out string strAsset)
        {
            strAsset = "";
            if (tn.Level != 2)
            {
                return false;
            }

            if (tn.Parent.Text != "Assets")
            {
                return false;
            }

            strAsset = tn.Text;
            return true;
        }

        private void tvSolutionExplorer_AfterSelect(object sender, TreeViewEventArgs e)
        {

            if (tvSolutionExplorer.SelectedNode.Parent != null)
            {
                SolutionExplorerTreeNode tn = (SolutionExplorerTreeNode)tvSolutionExplorer.SelectedNode;
                if (tn.NetworkDefinition != null)
                {
                    if (tvSolutionExplorer.SelectedNode.Parent.Name == "NodeAttribute" + tn.NetworkDefinition.NetDefName)
                    {
                        TreeViewSelectionChanged(tn);
                    }
                    if (tvSolutionExplorer.SelectedNode.Parent.Name == "NodeNetwork")
                    {
                        TreeViewSelectionChanged(tn);
                    }
                    if (tvSolutionExplorer.SelectedNode.Parent.Name == "NodeMaterial")
                    {
                        TreeViewSelectionChanged(tn);
                    }
                    if (tvSolutionExplorer.SelectedNode.Parent.Name == "NodeAsset" + tn.NetworkDefinition.NetDefName)
                    {
                        TreeViewSelectionChanged(tn);
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if this is a year selection.  If true it returns the attribute and network name.
        /// </summary>
        /// <param name="tn">The Solution Explore TreeNode selected.</param>
        /// <param name="strAttribute">Attribute which the year belongs to.</param>
        /// <param name="strNetwork">Network which attribute belongs to.</param>
        /// <returns></returns>
        private bool IsViewAttributeYear(TreeNode tn, out String strAttribute, out String strNetwork)
        {
            strAttribute = "";
            strNetwork = "";
            if (tn.Level != 6)//All Network/Attribute View/Attribute/year nodes reside at level 6.
            {
                return false;
            }
            
            if (tn.Parent.Parent.Text != "Attribute View")//We are on a level 6 TreeNode that is not in Attribute view.
            {
                return false;
            }

            strAttribute = tn.Parent.Text;
            strNetwork = tn.Parent.Parent.Parent.Parent.Text;
            return true;
        }

        /// <summary>
        /// Returns true if tnNode selected is a Attribute under attribute view.
        /// </summary>
        /// <param name="tn">The Solution Explore TreeNode selected.</param>
        /// <param name="strAttribute">Attribute which the year belongs to.</param>
        /// <param name="strNetwork">Network which attribute belongs to.</param>
        /// <returns></returns>
        private bool IsViewAttribute(TreeNode tn, out String strAttribute, out String strNetwork)
        {
            strAttribute = "";
            strNetwork = "";
            if (tn.Level != 4)//All Network/Attribute View/Attribute nodes reside at level 4.
            {
                return false;
            }

            if (tn.Parent.Text != "Attribute View")//We are on a level 5 TreeNode that is not in Attribute view.
            {

                return false;
            }

            strAttribute = tn.Text;
            strNetwork = tn.Parent.Parent.Parent.Text;
            return true;
        }

        private bool IsViewAttribute(TreeNode tn, out String strNetwork)
        {
            strNetwork = "";
            if (tn.Level != 3)//All Network/Attribute View/Attribute nodes reside at level 4.
            {
                return false;
            }

            if (tn.Text != "Attribute View")
            {

                return false;
            }
            strNetwork = tn.Parent.Parent.Text;
            return true;
        }

        private void SolutionExplorer_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormManager.RemoveSolutionExplorer(this);
        }

        public TreeView GetTreeView()
        {
            return this.tvSolutionExplorer;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (tvSolutionExplorer.SelectedNode.Parent == null)
            //{
            //    Global.WriteOutput("Error:Root nodes may not be deleted.");
            //    return;
            //}
            //if (tvSolutionExplorer.SelectedNode.Parent.Name == null) return;
            //switch (tvSolutionExplorer.SelectedNode.Parent.Name)
            //{
            //    case "NodeSimulation":

            //        //String strSimulationID = tvSolutionExplorer.SelectedNode.Tag.ToString();
            //        //String strDelete = "DELETE FROM SIMULATIONS WHERE SIMULATIONID ='" + strSimulationID + "'";
            //        //try
            //        //{
            //        //    DBMgr.ExecuteNonQuery(strDelete);
            //        //    tvSolutionExplorer.SelectedNode.Remove();
            //        //}
            //        //catch (Exception exception)
            //        //{
            //        //    Global.WriteOutput("Error deleting selected simulation." + exception.Message);
            //        //    return;
            //        //}

            //        ////TODO:  Close tabs for this simulation.
            //        //break;

            //    case "NodeNetwork":
            //        //String strNetworkID = tvSolutionExplorer.SelectedNode.Tag.ToString();
            //        //String strDeleteNetwork = "DELETE FROM NETWORKS WHERE NETWORKID ='" + strNetworkID + "'";
            //        //try
            //        //{
            //        //    DBMgr.ExecuteNonQuery(strDeleteNetwork);
            //        //    tvSolutionExplorer.SelectedNode.Remove();
            //        //}
            //        //catch (Exception exception)
            //        //{
            //        //    Global.WriteOutput("Error deleting selected simulation." + exception.Message);
            //        //    return;
            //        //}

            //        ////TODO:  Close tabs for this simulation.
            //        break;

            //    default:
            //        Global.WriteOutput("Error: Deleting " + tvSolutionExplorer.SelectedNode.Text + " is not implemented or allowed.");
            //        break;
            //}
        }

		private bool IsAttributeNameValid(string assetTitle)
		{
			bool bIsMatch = false;
			Regex tableColumnNameValidator = new Regex("^[A-Za-z]([A-Za-z0-9_]*[^0-9]|[A-Za-z0-9]*)$");
			bIsMatch = tableColumnNameValidator.IsMatch(assetTitle);
			return bIsMatch;
		}

        private void tsmiAddNewAttributeRoot_Click(object sender, EventArgs e)
        {
            AttributeProperties attributeProp = new AttributeProperties();
            String strNewAttribute = ShowProperties(true, attributeProp, "Add New Attribute");
            if (strNewAttribute != "" && IsAttributeNameValid(strNewAttribute))
            {
                NetworkDefinition currentNetDef = ((SolutionExplorerTreeNode)tvSolutionExplorer.SelectedNode).NetworkDefinition;
                SolutionExplorerTreeNode tn = new SolutionExplorerTreeNode(currentNetDef);
                tvSolutionExplorer.Nodes[currentNetDef.NetDefName].Nodes["NodeAttribute" + currentNetDef.NetDefName].Nodes.Add(tn);
                tn.ContextMenuStrip = cmsAttribute;
                tn.Name = strNewAttribute;
                tn.Text = strNewAttribute;
				//dsmelser 2008.07.31
				//added tagging for robust icon switching
				tn.Tag = strNewAttribute;

				//dsmelser 2008.07.31
				//moved string constant to user properties for easy manipulation
                //tn.ImageKey = "db.ico";
                //tn.SelectedImageKey = "db.ico";
				tn.ImageKey = Settings.Default.ATTRIBUTE_IMAGE_KEY;
				tn.SelectedImageKey = Settings.Default.ATTRIBUTE_IMAGE_KEY_SELECTED;
				tvSolutionExplorer.Refresh();
            }
        }

        private void tsmiAddNewNetwork_Click(object sender, EventArgs e)
        {
            NetworkDefinition currNetDef = ((SolutionExplorerTreeNode)(tvSolutionExplorer.SelectedNode)).NetworkDefinition;
            NetworkProperties networkProp = new NetworkProperties("");
            String strNewNetwork = ShowProperties(true, networkProp, "Add New Network");
            if (strNewNetwork != "")
            {
                AddNetworkNodesToTreeView(strNewNetwork, currNetDef);
            }
        }

        private void tsmiAddNewSimulation_Click(object sender, EventArgs e)
        {
            String strNetwork = tvSolutionExplorer.SelectedNode.Parent.Text;
            String strNetworkID = tvSolutionExplorer.SelectedNode.Parent.Tag.ToString();
            CreateSimulation(strNetworkID, true);

        }

        private void tsmiDeleteSimulation_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Deleting a simulation cannot be undone, continue?", "Delete Simulation Confirmation", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            String strSimulationID = tvSolutionExplorer.SelectedNode.Tag.ToString();
            List<String> listCommands = new List<String>();
            listCommands = DeleteSimulationCommands(tvSolutionExplorer.SelectedNode.Parent.Parent.Tag.ToString(), strSimulationID, listCommands);
            try
            {
                DBMgr.ExecuteBatchNonQuery(listCommands);
                tvSolutionExplorer.SelectedNode.Remove();
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error deleting selected simulation." + exception.Message);
                return;
            }

            //TODO:  Close tabs for this simulation.
            FormManager.CloseSingleSimulation(strSimulationID);
        }

        private void tsmiDeleteNetwork_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Deleting a network cannot be undone, continue?", "Delete Network Confirmation", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            String strNetworkID = tvSolutionExplorer.SelectedNode.Tag.ToString();
            List<String> listCommands = new List<String>();
            listCommands = DeleteNetworkCommands(strNetworkID, listCommands);
            try
            {
                DBMgr.ExecuteBatchNonQuery(listCommands);
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: Problem deleting network, aborting operation. " + exc.Message);
                return;
            }

			//Global.SecurityOperations.RemoveNetworkActions( strNetworkID );



            FormManager.CloseNetworkTabs();
            FormManager.CloseSimulationTabs();

            tvSolutionExplorer.SelectedNode.Remove();

		}

        private void tsmiAttributeProperties_Click(object sender, EventArgs e)
        {
            String strSelectedNode = tvSolutionExplorer.SelectedNode.Text;
            AttributeProperties attributeProp = new AttributeProperties(strSelectedNode);
            ShowProperties(false, attributeProp, "");
        }

        private void tsmiAssetProperties_Click(object sender, EventArgs e)
        {
            
            String strSelectedNode = tvSolutionExplorer.SelectedNode.Text;
			//CustomClass assetProperties = GetProperties(strSelectedNode);
			AssetProperties assetProp = new AssetProperties(strSelectedNode);
			ShowProperties(false, assetProp, "");
        }

        private void tsmiPropertiesNetwork_Click(object sender, EventArgs e)
        {
            String strSelectedNode = tvSolutionExplorer.SelectedNode.Text;
            NetworkProperties networkProp = new NetworkProperties(strSelectedNode);
            ShowProperties(false, networkProp, "");
        }

        private void tsmiAddNewAsset_Click(object sender, EventArgs e)
        {
            if (FormManager.GetFormCalculatedAssets() != null)
            {
                FormManager.GetFormCalculatedAssets().Close();
            }
            String strNewAsset = CreateNewAsset();
            if (strNewAsset != "" && IsAttributeNameValid(strNewAsset))
            {
                TreeNode tn = tvSolutionExplorer.Nodes["NodeAsset"].Nodes.Add(strNewAsset);
                tn.ContextMenuStrip = cmsAsset;
				//dsmelser 2008.07.31
				//added tagging for robust icon switching
				tn.Tag = strNewAsset;
            }
        }

        private void tsmiCreateFromShapefileAsset_Click(object sender, EventArgs e)
        {
            FormImportAsset formImportAsset = new FormImportAsset();
            if (formImportAsset.ShowDialog() == DialogResult.OK)
            {
                if (formImportAsset.AssetName != "" && IsAttributeNameValid(formImportAsset.AssetName))
                {
                    TreeNode tn = tvSolutionExplorer.Nodes["NodeAsset"].Nodes.Add(formImportAsset.AssetName);
                    tn.ContextMenuStrip = cmsAsset;
					//dsmelser 2008.07.31
					//added tagging for robust icon switching
					tn.Tag = formImportAsset.AssetName;
                }
            }
        }

        private void tsmiDeleteAsset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Deleting an asset cannot be undone, continue?", "Delete Asset Confirmation", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
			String strSelectedAsset = "";
			try
			{
				strSelectedAsset = tvSolutionExplorer.SelectedNode.Text;
				DeleteAsset assetRemover = new DeleteAsset(strSelectedAsset);
				assetRemover.Delete();
				tvSolutionExplorer.SelectedNode.Remove();
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error dropping table or view " + strSelectedAsset + "transaction aborted. " + exc.Message);
			}
            
        }

        private void tsmiAttributeDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Deleting an attribute cannot be undone, continue?", "Delete attribute Confirmation", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            // Create a transaction to delete the attribute and then remove it from the solution explorer list.
            String strSelectedAttribute = tvSolutionExplorer.SelectedNode.Text;
            try
            {
                DeleteAttribute deleteAttribute = new DeleteAttribute(strSelectedAttribute);
                deleteAttribute.Delete();
                tvSolutionExplorer.SelectedNode.Remove();
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: Could not delete attribute table. " + exc.Message);
                return;
            }
            
        }

        private void tsmiCloneSimulation_Click(object sender, EventArgs e)
        {
            String strSimulationID = tvSolutionExplorer.SelectedNode.Tag.ToString();
            String strNetworkID = tvSolutionExplorer.SelectedNode.Parent.Parent.Tag.ToString();
            CloneSimulation(strNetworkID);
        }

		private void CloneSimulation(String networkID)
		{

			// First create a new simulation to paste all the relevant data into.
			String oldSimID = tvSolutionExplorer.SelectedNode.Tag.ToString();
			String simID = CreateSimulation(networkID, false);

			if( simID != "" )
			{
				Global.WriteOutput( "New simulation created." );

				// Now call the copy/paste functions for each part of the simulation
				Global.WriteOutput( "Copying simulation Analysis" );
				SimulationManager.CopyAnalysisID( oldSimID );
				Global.WriteOutput( "Copying simulation Investment" );
				SimulationManager.CopyInvestmentID( oldSimID );
				Global.WriteOutput( "Copying simulation Performance" );
				SimulationManager.CopyPerformanceID( oldSimID );
				Global.WriteOutput( "Copying simulation Treatments" );
				SimulationManager.CopyTreatmentID( oldSimID );
                Global.WriteOutput("Copying simulation Committed Projects");
                SimulationManager.CopyCommittedID(oldSimID);


				Global.WriteOutput( "Pasting simulation Analysis" );
				SimulationManager.PasteAnalysis( simID );
				Global.WriteOutput( "Pasting simulation Investment" );
				SimulationManager.PasteInvestment( simID );
				Global.WriteOutput( "Pasting simulation Performance" );
				SimulationManager.PastePerformance( simID );
				Global.WriteOutput( "Pasting simulation Treatments" );
				SimulationManager.PasteTreatment( simID );

                if (IsMatchingNetworkID(oldSimID, simID))
                {
                    Global.WriteOutput("Pasting simulation Committed Projects");
                    SimulationManager.PasteCommitted(simID);
                }
                else
                {
                    Global.WriteOutput("Committed projects cannot be cloned between networks.  Please use Export and Import");
                }
				Global.WriteOutput( "Done cloning simulation." );

			}
		}

        private bool IsMatchingNetworkID(string oldSimID, string simID)
        {
            bool isMatching = false;

            string select = "SELECT NETWORKID FROM SIMULATIONS WHERE SIMULATIONID='" + oldSimID + "' OR SIMULATIONID='" + simID + "'";

            DataSet ds = DBMgr.ExecuteQuery(select);
            if(ds.Tables[0].Rows.Count == 2)
            {
                int oldNetworkID = Convert.ToInt32(ds.Tables[0].Rows[0]["NETWORKID"]);
                int newNetworkID = Convert.ToInt32(ds.Tables[0].Rows[1]["NETWORKID"]);
                if (oldNetworkID == newNetworkID)
                {
                    isMatching = true;
                }
            }

            return isMatching;
        }

		private List<String> DeleteNetworkCommands( String strNetworkID, List<String> listCommands )
		{
			if( DBMgr.IsTableInDatabase( "SECTION_" + strNetworkID ) )
			{
				listCommands.Add( "DROP TABLE SECTION_" + strNetworkID );
			}
			if( DBMgr.IsTableInDatabase( "SEGMENT_" + strNetworkID + "_" + "NS0" ) )
			{
				listCommands.Add( "DROP TABLE SEGMENT_" + strNetworkID + "_" + "NS0" );
			}

			// Get a list of simulations where the networkID matches strNetworkID
			String strQuery = "SELECT SIMULATIONID FROM SIMULATIONS WHERE NETWORKID = '" + strNetworkID + "'";
			try
			{
				DataSet ds = DBMgr.ExecuteQuery( strQuery );
				foreach( DataRow dr in ds.Tables[0].Rows )
				{
					listCommands = DeleteSimulationCommands( strNetworkID, dr[0].ToString(), listCommands );
				}
			}
			catch( Exception exc )
			{
				Global.WriteOutput( "Error: Couldn't delete network, unable to find network. " + exc.Message );
				return null;
			}
			listCommands.Add( "DELETE FROM NETWORKS WHERE NETWORKID = '" + strNetworkID + "'" );

			//Global.SecurityOperations.RemoveNetworkActions( strNetworkID );

			return listCommands;
		}

        private List<String> DeleteSimulationCommands(String strNetworkID, String strSimulationID, List<String> listCommands)
        {
            // TARGET_N#_S# REPORT_N#_S# BENEFITCOST_N#_S# SIMULATION_N#_S#
			if (DBMgr.IsTableInDatabase("TARGET_" + strNetworkID + "_" + strSimulationID))
			{
				listCommands.Add("DROP TABLE TARGET_" + strNetworkID + "_" + strSimulationID);
			}
			else
			{
				Global.WriteOutput("Warning: Table TARGET_" + strNetworkID + "_" + strSimulationID + " not found in database. ");
			}
			if (DBMgr.IsTableInDatabase("REPORT_" + strNetworkID + "_" + strSimulationID))
			{
				listCommands.Add("DROP TABLE REPORT_" + strNetworkID + "_" + strSimulationID);
			}
			else
			{
				Global.WriteOutput("Warning: Table REPORT_" + strNetworkID + "_" + strSimulationID + " not found in database. ");
			}
			if (DBMgr.IsTableInDatabase("BENEFITCOST_" + strNetworkID + "_" + strSimulationID))
			{
				listCommands.Add("DROP TABLE BENEFITCOST_" + strNetworkID + "_" + strSimulationID);
			}
			else
			{
				Global.WriteOutput("Warning: Table BENEFITCOST_" + strNetworkID + "_" + strSimulationID + " not found in database. ");
			}
			if (DBMgr.IsTableInDatabase("SIMULATION_" + strNetworkID + "_" + strSimulationID))
			{
				listCommands.Add("DROP TABLE SIMULATION_" + strNetworkID + "_" + strSimulationID);
			}
			else
			{
				Global.WriteOutput("Warning: Table SIMULATION_" + strNetworkID + "_" + strSimulationID + " not found in database. ");
			}

            // Now delete the simulation row from the simulations table
            String strDelete = "DELETE FROM SIMULATIONS WHERE SIMULATIONID = '" + strSimulationID + "'";
			Global.SecurityOperations.RemoveSimulationActions( strNetworkID, strSimulationID );
            listCommands.Add(strDelete);
            return listCommands;
        }

        private void csmiInvestmentCopy_Click(object sender, EventArgs e)
        {
            SimulationManager.CopyInvestmentID(tvSolutionExplorer.SelectedNode.Parent.Tag.ToString());
        }

        private void cmsiInvestmentPaste_Click(object sender, EventArgs e)
        {
            SimulationManager.PasteInvestment(tvSolutionExplorer.SelectedNode.Parent.Tag.ToString());
        }

        private void csmiPerformanceCopy_Click(object sender, EventArgs e)
        {
            SimulationManager.CopyPerformanceID(tvSolutionExplorer.SelectedNode.Parent.Tag.ToString());
        }

        private void csmiPerfomancePaste_Click(object sender, EventArgs e)
        {
            SimulationManager.PastePerformance(tvSolutionExplorer.SelectedNode.Parent.Tag.ToString());
        }

        private void tmsiCopy_Click(object sender, EventArgs e)
        {
            SimulationManager.CopyTreatmentID(tvSolutionExplorer.SelectedNode.Parent.Tag.ToString());
        }

        private void tsmiPaste_Click(object sender, EventArgs e)
        {
            SimulationManager.PasteTreatment(tvSolutionExplorer.SelectedNode.Parent.Tag.ToString());
        }

        private void tsmiAnalyisCopy_Click(object sender, EventArgs e)
        {
            SimulationManager.CopyAnalysisID(tvSolutionExplorer.SelectedNode.Parent.Tag.ToString());
        }

        private void tmsiAnalysisPaste_Click(object sender, EventArgs e)
        {
            SimulationManager.PasteAnalysis(tvSolutionExplorer.SelectedNode.Parent.Tag.ToString());
        }

        private void ToolStripMenuItemCreateSubnetwork_Click(object sender, EventArgs e)
        {
            //Get current NetworkID from TAG.
            NetworkDefinition currNetDef = ((SolutionExplorerTreeNode)tvSolutionExplorer.SelectedNode).NetworkDefinition;
            String strNetworkID = tvSolutionExplorer.SelectedNode.Tag.ToString();
            String strNetwork = tvSolutionExplorer.SelectedNode.Text.ToString();
            Hashtable hashAttributeYear = Global.GetAttributeYear(strNetworkID);
            FormCreateSubnetwork form = new FormCreateSubnetwork(strNetworkID, strNetwork, hashAttributeYear);
            if (form.ShowDialog() == DialogResult.OK)
            {
                //Add new network node to tree.
                AddNetworkNodesToTreeView(form.NewNetworkName, currNetDef);
            }
        }

        private void tsmiRenameAsset_Click( object sender, EventArgs e )
		{
			TreeNode tnSelected = tvSolutionExplorer.SelectedNode;

		}

		private void tsmiCreateAttributeViewReport_Click( object sender, EventArgs e )
		{
			if( FormManager.IsAttributeViewOpen( tvSolutionExplorer.SelectedNode.Parent.Parent.Text ) )
			{
				FormAttributeView frmAttributeView = FormManager.GetCurrentAttributeView( tvSolutionExplorer.SelectedNode.Parent.Parent.Text );
				frmAttributeView.GenerateReport();
			}
			else
			{
				Global.WriteOutput( "Error: Attribute View not open (no attributes selected)." );
			}
		}

		private void importGeometriesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Opens the network definition page and then opens the import geometries page.
			FormNetworkDefinition formNetworkDefinition = OpenNetworkDefinitionPage();
			if (formNetworkDefinition != null)
			{
				formNetworkDefinition.ImportGeometries();
			}
		}

		private void importShapefileToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			// Opens the network definition page, then opens the shapefile import utility.
			FormNetworkDefinition formNetworkDefinition = OpenNetworkDefinitionPage();
			if (formNetworkDefinition != null)
			{
				formNetworkDefinition.ImportShapefile();
			}
		}

		private FormNetworkDefinition OpenNetworkDefinitionPage()
		{
			// Opens the network definiton tab based on linear or section selection
			FormNetworkDefinition networkDefintion = null;
            SolutionExplorerTreeNode currentNode = (SolutionExplorerTreeNode)tvSolutionExplorer.SelectedNode;
            if (currentNode.NetworkDefinition != null)
            {
                if (tvSolutionExplorer.SelectedNode.Name == "NodeLinear" + currentNode.NetworkDefinition.NetDefName)
                {
                    if (!FormManager.IsFormNetworkDefinitionLinearOpen(out networkDefintion))
                    {
                        networkDefintion = new FormNetworkDefinition(true, currentNode);
                        networkDefintion.Tag = tvSolutionExplorer.SelectedNode.Tag;
                        networkDefintion.Show(m_dockPanel);
                        FormManager.AddFormNetworkDefinitionLinear(networkDefintion);
                    }
                }
                if (tvSolutionExplorer.SelectedNode.Name == "NodeSection" + currentNode.NetworkDefinition.NetDefName)
                {
                    if (!FormManager.IsFormNetworkDefinitionSectionOpen(out networkDefintion))
                    {
                        networkDefintion = new FormNetworkDefinition(false, currentNode);
                        networkDefintion.Tag = tvSolutionExplorer.SelectedNode.Tag;
                        networkDefintion.Show(m_dockPanel);
                        FormManager.AddFormNetworkDefinitionSection(networkDefintion);
                    }
                }
            }
			return networkDefintion;
		}

		private void verifyNetworkToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenNetworkDefinitionPage().OnVerify();
		}

		private void doAssetRollupToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			AssetRollup.AssetRollup AssetRollup = new AssetRollup.AssetRollup("", "", "", "", (String)tvSolutionExplorer.SelectedNode.Parent.Parent.Tag);
			AssetRollup.DoAssetRollup();
		}

		private void runSimulationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			String strNetworkName = tvSolutionExplorer.SelectedNode.Parent.Parent.Text;
			String strSimulationName = tvSolutionExplorer.SelectedNode.Text;
			String strSimID = tvSolutionExplorer.SelectedNode.Tag.ToString();


			FormAnalysis formAnalysis;
			if (!FormManager.IsFormAnalysisOpen(strSimID, out formAnalysis))
			{
				Hashtable hashAttributeYear = Global.GetAttributeYear(tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Tag.ToString());
				formAnalysis = new FormAnalysis(strNetworkName, strSimulationName, strSimID, hashAttributeYear);

				// For the simulation pages, we need to use the strSimID variable instead of the network name
				// to identify the individual open simulation pages in their respective lists.
				formAnalysis.Tag = strSimID;
				FormManager.AddFormAnalysis(formAnalysis);
				formAnalysis.TabText = strSimulationName + "-Analysis";
				formAnalysis.Show(m_dockPanel);
				formAnalysis.RunSimulation();
			}
		}

		private void tsmiEdit_Click(object sender, EventArgs e)
		{
			String selectedAsset = tvSolutionExplorer.SelectedNode.Text;
			bool bCreateNew = false;
			FormEditAsset editAssetForm = new FormEditAsset(selectedAsset, bCreateNew);
			editAssetForm.Text = "Edit Asset";
			editAssetForm.ShowDialog();
		}

		private void cmsAttributeRoot_Opening( object sender, CancelEventArgs e )
		{
			tsmiAddNewAttributeRoot.Enabled = Global.SecurityOperations.CanCreateRawAttribute();
			//tsmiAnalyisCopy.Enabled = Global.SecurityOperations.CanViewRawAttributeData( tvSolutionExplorer.SelectedNode.Text );
			//tmsiAnalysisPaste.Enabled = Global.SecurityOperations.CanModifyRawAttributeData( tvSolutionExplorer.SelectedNode.Text );
		}

		private void cmsAttribute_Opening( object sender, CancelEventArgs e )
		{
			tsmiAttributeProperties.Enabled = Global.SecurityOperations.CanViewRawAttributeData( tvSolutionExplorer.SelectedNode.Text );
			tsmiAttributeDelete.Enabled = Global.SecurityOperations.CanDeleteRawAttributeData( tvSolutionExplorer.SelectedNode.Text );

		}

		private void cmsAssetRoot_Opening( object sender, CancelEventArgs e )
		{
			tsmiAddNewAsset.Enabled = Global.SecurityOperations.CanCreateRawAsset();
			tsmiCreateFromShapefileAsset.Enabled = Global.SecurityOperations.CanCreateRawAsset();
			createFromOtherToolStripMenuItem.Enabled = Global.SecurityOperations.CanCreateRawAsset();

		}

		private void cmsAsset_Opening( object sender, CancelEventArgs e )
		{

		}

		private void cmsNetworkRoot_Opening( object sender, CancelEventArgs e )
		{
			tsmiAddNewNetwork.Enabled = Global.SecurityOperations.CanCreateNetworks();
		}

		private void cmsNetwork_Opening( object sender, CancelEventArgs e )
		{
			tsmiPropertiesNetwork.Enabled = Global.SecurityOperations.CanViewNetwork( tvSolutionExplorer.SelectedNode.Tag.ToString() );
			tsmiDeleteNetwork.Enabled = Global.SecurityOperations.CanDeleteNetwork( tvSolutionExplorer.SelectedNode.Tag.ToString() );
			ToolStripMenuItemCreateSubnetwork.Enabled = Global.SecurityOperations.CanCreateSubNetworks( tvSolutionExplorer.SelectedNode.Tag.ToString() );
		}

		private void cmsAttributeViewRoot_Opening( object sender, CancelEventArgs e )
		{
			//string Network = tvSolutionExplorer.SelectedNode.Parent.Parent.Tag.ToString();
			tsmiCreateAttributeViewReport.Enabled = Global.SecurityOperations.CanCreateAttributeViewReport( tvSolutionExplorer.SelectedNode.Parent.Parent.Tag.ToString() );
		}

		private void cmsAssetView_Opening( object sender, CancelEventArgs e )
		{
			//string Network = tvSolutionExplorer.SelectedNode.Parent.Parent.Tag.ToString();
			doAssetRollupToolStripMenuItem1.Enabled = Global.SecurityOperations.CanRollupAssets( tvSolutionExplorer.SelectedNode.Parent.Parent.Tag.ToString() );

		}

		private void cmsSimulationRoot_Opening( object sender, CancelEventArgs e )
		{
			tsmiAddNewSimulation.Enabled = Global.SecurityOperations.CanCreateSimulation( tvSolutionExplorer.SelectedNode.Parent.Tag.ToString() );
		}

		private void cmsSimulation_Opening( object sender, CancelEventArgs e )
		{
			string strNetworkID = tvSolutionExplorer.SelectedNode.Parent.Parent.Tag.ToString();
			string strSimulationID = tvSolutionExplorer.SelectedNode.Tag.ToString();
			runSimulationToolStripMenuItem.Enabled = Global.SecurityOperations.CanRunSimulation( strNetworkID, strSimulationID );
			tsmiCloneSimulation.Enabled = Global.SecurityOperations.CanCreateSimulation( strNetworkID );
			tsmiDeleteSimulation.Enabled = Global.SecurityOperations.CanDeleteSimulation( strNetworkID, strSimulationID );
		}

		private void cmsAnalysis_Opening( object sender, CancelEventArgs e )
		{
			string strNetworkID = tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Tag.ToString();
			string strSimulationID = tvSolutionExplorer.SelectedNode.Parent.Tag.ToString();
			tsmiAnalyisCopy.Enabled = Global.SecurityOperations.CanViewSimulationAnalysis( strNetworkID, strSimulationID );
			tmsiAnalysisPaste.Enabled = Global.SecurityOperations.CanCreateSimulationAnalysis( strNetworkID, strSimulationID );
		}

		private void cmsPerformance_Opening( object sender, CancelEventArgs e )
		{
			string strNetworkID = tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Tag.ToString();
			string strSimulationID = tvSolutionExplorer.SelectedNode.Parent.Tag.ToString();
			csmiPerformanceCopy.Enabled = Global.SecurityOperations.CanViewSimulationPerformance( strNetworkID, strSimulationID );
			csmiPerfomancePaste.Enabled = Global.SecurityOperations.CanCreateSimulationPerformance( strNetworkID, strSimulationID );

		}

		private void cmsTreatment_Opening( object sender, CancelEventArgs e )
		{
			string strNetworkID = tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Tag.ToString();
			string strSimulationID = tvSolutionExplorer.SelectedNode.Parent.Tag.ToString();
			tmsiCopy.Enabled = Global.SecurityOperations.CanViewSimulationTreatment( strNetworkID, strSimulationID );
			tsmiPaste.Enabled = Global.SecurityOperations.CanCreateSimulationTreatment( strNetworkID, strSimulationID );

		}

		private void cmsInvestment_Opening( object sender, CancelEventArgs e )
		{
			string strSimulationID = tvSolutionExplorer.SelectedNode.Parent.Tag.ToString();
			string strNetworkID = tvSolutionExplorer.SelectedNode.Parent.Parent.Parent.Tag.ToString();
			csmiInvestmentCopy.Enabled = Global.SecurityOperations.CanViewSimulationInvestment( strNetworkID, strSimulationID );
			cmsiInvestmentPaste.Enabled = Global.SecurityOperations.CanCreateSimulationInvestment( strNetworkID, strSimulationID );
			
		}

		private void contextMenuStripNetworkDefinition_Opening( object sender, CancelEventArgs e )
		{
			importGeometriesToolStripMenuItem.Enabled = Global.SecurityOperations.CanCreateNetworkDefinitionData();
			//importShapefileToolStripMenuItem1.Enabled = Global.SecurityOperations.CanCreateNetworkDefinitionData();
			verifyNetworkToolStripMenuItem.Enabled = Global.SecurityOperations.CanViewNetworkDefinitionData();
		}

        /// <summary>
        /// Adding a new calculated field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addNewCalculatedtmsi_Click(object sender, EventArgs e)
        {
            SolutionExplorerTreeNode tn = (SolutionExplorerTreeNode)tvSolutionExplorer.SelectedNode;
			CalculatedFieldsProperties calcProp = new CalculatedFieldsProperties(tn.NetworkDefinition);
            String calcFieldName = ShowProperties(true, calcProp, "Add New Calculated Field");
			if (calcFieldName != "" && IsAttributeNameValid(calcFieldName))
			{
                SolutionExplorerTreeNode toAdd = new SolutionExplorerTreeNode(tn.NetworkDefinition);
                tvSolutionExplorer.Nodes[tn.NetworkDefinition.NetDefName].Nodes["NodeCalculatedFields" + tn.NetworkDefinition.NetDefName].Nodes.Add(toAdd);
                toAdd.ContextMenuStrip = cmsAttribute;
                //dsmelser 2008.07.31
				//added tagging for robust icon switching
                toAdd.Tag = calcFieldName + tn.NetworkDefinition.NetDefName;
                toAdd.Name = calcFieldName + tn.NetworkDefinition.NetDefName;
                toAdd.Text = calcFieldName;
				//dsmelser 2008.07.31
				//moved string constant to user properties for easy manipulation
				//tn.ImageKey = "db.ico";
				//tn.SelectedImageKey = "db.ico";
                toAdd.ImageKey = Settings.Default.ATTRIBUTE_IMAGE_KEY;
                toAdd.SelectedImageKey = Settings.Default.ATTRIBUTE_IMAGE_KEY_SELECTED;
			}
        }

        private void deleteCalculatedtmsi_Click(object sender, EventArgs e)
        {
            String strDeletedAttribute = tvSolutionExplorer.SelectedNode.Text;
            try
            {
                String strDelete = "DELETE FROM ATTRIBUTES_ WHERE ATTRIBUTE_='" + strDeletedAttribute + "'";
                DBMgr.ExecuteNonQuery(strDelete);
                tvSolutionExplorer.Nodes.Remove(tvSolutionExplorer.SelectedNode);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Deleting Calculated Field ATTRIBUTE. " + exception.Message);
            }
        }

		private void createFromRemoteDataSourceToolStripMenuItem_Click(object sender, EventArgs e)
		{
            //throw new NotImplementedException();
            //frmConnect ndConnect = new frmConnect();
			
            //if (ndConnect.ShowDialog() == DialogResult.OK)
            //{
            //    string insertCP = ndConnect.InsertCP;
            //    frmDataDisplay ndDataDisplay = new frmDataDisplay(ndConnect.Server,
            //        ndConnect.Database,
            //        ndConnect.UserName,
            //        ndConnect.Password,
            //        "NETWORK_DEFINITION_VIEW",
            //        ndConnect.Provider);
            //    ndConnect.Close();
            //    ndDataDisplay.SetCreateViewLabel("CREATE VIEW NETWORK_DEFINITION_VIEW AS ");

            //    if (ndDataDisplay.ShowDialog() == DialogResult.OK)
            //    {
            //        // Create the local view as NETWORK_DEFINITION_VIEW and update the NETWORK_DEFINITION table.
            //        try
            //        {
            //            if (DBMgr.IsTableInDatabase("NETWORK_DEFINITION_VIEW"))
            //            {
            //                DBMgr.ExecuteNonQuery("DROP VIEW NETWORK_DEFINITION_VIEW");
            //            }
            //        }
            //        catch (Exception exc)
            //        {
            //            Global.WriteOutput("Error dropping NETWORK_DEFINITION_VIEW, aborting operation. " + exc.Message);
            //        }
            //        try
            //        {
            //            List<string> transaction = new List<string>();
            //            transaction.Add("CREATE VIEW NETWORK_DEFINITION_VIEW AS (" +
            //                ndDataDisplay.ViewSelectStatement +
            //                ")");
            //            transaction.Add(insertCP);
            //            string viewStatement = ndDataDisplay.ViewSelectStatement.Replace("'", "''");
            //            transaction.Add("UPDATE CONNECTION_PARAMETERS SET VIEW_STATEMENT = '" + viewStatement + "'");
            //            DBMgr.ExecuteBatchNonQuery(transaction);
            //            Global.WriteOutput("Successfully created Remote Network Definition.  Use the Update button on the Network Definition page to import the data.");
            //        }
            //        catch (Exception exc)
            //        {
            //            Global.WriteOutput("Error creating NETWORK_DEFINITION_VIEW. " + exc.Message);
            //        }
            //    }
            //}
		}

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvSolutionExplorer.LabelEdit = true;
            tvSolutionExplorer.SelectedNode.BeginEdit();
        }

        private void tvSolutionExplorer_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            tvSolutionExplorer.LabelEdit = false;
            string updatedName = e.Label;
            TreeNode tnRename = tvSolutionExplorer.SelectedNode;

            if (tnRename.Parent.Text == "Simulations")
            {
                string simulationID = tnRename.Tag.ToString();
                DBOp.RenameSimulation(updatedName, simulationID);
            }
            else if (tnRename.Parent.Text == "Networks")
            {
                string networkID = tnRename.Tag.ToString();
                DBOp.RenameNetwork(updatedName, networkID);
            }
        }

        private void renameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tvSolutionExplorer.LabelEdit = true;
            tvSolutionExplorer.SelectedNode.BeginEdit();
        }

        private void defineAreaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SolutionExplorerTreeNode selectedNode = (SolutionExplorerTreeNode)tvSolutionExplorer.SelectedNode;
            string networkID = selectedNode.Tag.ToString();
            string networkAreaEquation = DBOp.GetNetworkSpecificArea(networkID);
            FormEditEquation formArea = new FormEditEquation(networkAreaEquation,true);

            if (formArea.ShowDialog() == DialogResult.OK)
            {
                networkAreaEquation = formArea.Equation;
                DBOp.UpdateNetworkSpecificArea(networkID, networkAreaEquation);
            }
        }

        private void propertiesCalculatedtmsi_Click(object sender, EventArgs e)
        {

        }

        private void tmsiAddNew_Click(object sender, EventArgs e)
        {
			//FormCreateNewNetDef newNetworkDefForm = new FormCreateNewNetDef();
			//newNetworkDefForm.ShowDialog();
        }

        private void alterRemoteAttributesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string netDefName = ((SolutionExplorerTreeNode)tvSolutionExplorer.SelectedNode).NetworkDefinition.NetDefName;
            FormAlterRemoteAttributes alterAttributesForm = new FormAlterRemoteAttributes(netDefName);
            alterAttributesForm.ShowDialog();
        }
    }
}
