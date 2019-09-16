using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RoadCareGlobalOperations;
using WeifenLuo.WinFormsUI.Docking;
using RoadCare3;
using DatabaseManager;
using DataObjects;
using System.Collections;

namespace RCImageView3
{
    public partial class FormImageViewSolutionExplorer : ToolWindow
    {
        TreeNode m_tnNetworkRoot;

        public FormImageViewSolutionExplorer()
        {
            InitializeComponent();
            this.CloseButton = false;
        }

        private void FormImageViewSolutionExplorer_Load(object sender, EventArgs e)
        {
            ImageViewManager.TreeView = treeViewSolutionExplorer;
            LoadNetworks();
            LoadViews();

            LoadNetworkDefinition();
            LoadAttributes();
        }

        private void LoadViews()
        {
            TreeNode treeNodeViews = new TreeNode("Views", 19, 19);
            treeNodeViews.ContextMenuStrip = contextMenuStripViews;
            treeViewSolutionExplorer.Nodes.Add(treeNodeViews);
            try
            {
                ImageViewManager.Views = GlobalDatabaseOperations.GetViews();
            }
            catch(Exception except)
            {
                OutputWindow.WriteOutput("Error: Loading VIEWS from IMAGELOCATION table. " + except.Message);
                return;
            }

            foreach (String sView in ImageViewManager.Views)
            {
                TreeNode treeNodeView = new TreeNode(sView, 20, 20);
                treeNodeViews.Nodes.Add(treeNodeView);
                //FormImageViewer formImageView = new FormImageViewer(sView);
                //treeNodeView.Tag = formImageView;
                object[] parameters = new object[3];
                parameters[0] = null;
                parameters[1] = "ImageViewer";
                parameters[2] = sView;
                treeNodeView.Tag = parameters;
            }
            treeNodeViews.ExpandAll();
        }
        


        private void LoadNetworks()
        {
            TreeNode treeNodeNetwork = new TreeNode("Networks", 19, 19);
            m_tnNetworkRoot = treeNodeNetwork;
            treeNodeNetwork.ContextMenuStrip = contextMenuStripNetwork;
            treeViewSolutionExplorer.Nodes.Add(treeNodeNetwork);
            ImageViewManager.NetworkNode = treeNodeNetwork;

            try
            {
                ImageViewManager.Networks = GlobalDatabaseOperations.GetNetworks();
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Loading Networks. " + except.Message);
            }

            foreach (NetworkObject networkObject in ImageViewManager.Networks)
            {
                AddNewNetwork(networkObject);
            }
        }

        private void AddNewNetwork(NetworkObject network)
        {
            TreeNode tn = new TreeNode(network.Network, 19, 19);
            tn.ContextMenuStrip = contextMenuStripNetworkNode;
            m_tnNetworkRoot.Nodes.Add(tn);
            TreeNode treeNodeSegmentation = new TreeNode("Dynamic Segmentation", 14, 14);
            FormImageViewSegmentation formSegmentation = new FormImageViewSegmentation(network);
            object[] parameters = new object[3];
            parameters[0] = null;
            parameters[1] = "DynamicSegmentation";
            parameters[2] = network;
            treeNodeSegmentation.Tag = parameters;
            tn.Nodes.Add(treeNodeSegmentation);

            AddNetworkViewers(network,tn);
        }


        private void AddNetworkViewers(NetworkObject network, TreeNode tnNetwork)
        {
            try
            {
                int nCount = GlobalDatabaseOperations.GetSectionCountForNetwork(network.NetworkID);
            }
            catch//Catch is common occurence here.  If table is not rolled up.  Exception is thrown.
            {
                return;
            }
         //   TreeNode tn = new TreeNode("Construction History View", 13, 13);
         //   tnNetwork.Nodes.Add(tn);

            TreeNode tn = new TreeNode("Attribute View", 8, 8);
            tnNetwork.Nodes.Add(tn);
            this.AddAttributteNodes(network, tn);

            tn = new TreeNode("Section View", 10, 10);
            object[]  parameters = new object[4];
            parameters[0] = null;
            parameters[1] = "SectionView";
            parameters[2] = network.Network;
            parameters[3] = network.NetworkID;
            tn.Tag = parameters;
            tnNetwork.Nodes.Add(tn);

            tn = new TreeNode("Asset View", 12, 12);
            //FormAssetView formAssetView = new FormAssetView(network.NetworkID, Global.GetAttributeYear(network.NetworkID));
            //tn.Tag = formAssetView;
            parameters = new object[3];
            parameters[0] = null;
            parameters[1] = "AssetView";
            parameters[2] = network.NetworkID;
            tn.Tag = parameters;
            tnNetwork.Nodes.Add(tn);

            tn = new TreeNode("GIS View", 18, 18);
            parameters = new object[4];
            parameters[0] = null;
            parameters[1] = "GISViewer";
            parameters[2] = network.Network;
            parameters[3] = network.NetworkID;
            tn.Tag = parameters;
            tnNetwork.Nodes.Add(tn);

            tn = new TreeNode("Google View", 18, 18);
            parameters = new object[4];
            parameters[0] = null;
            parameters[1] = "GoogleView";
            tn.Tag = parameters;
            tnNetwork.Nodes.Add(tn);

            tn = new TreeNode("Feature View", 19, 19);
            //FormGoogleMap formGoogle = new FormGoogleMap();
            //tn.Tag = formGoogle;
            //tnNetwork.Nodes.Add(tn);
        }

        private void AddAttributteNodes(NetworkObject networkObject, TreeNode tn)
        {
            Hashtable hashAttributeYear = Global.GetAttributeYear(networkObject.NetworkID);
            //FormAttributeView formAttribute = new FormAttributeView(networkObject.Network, hashAttributeYear);

            object[] parameters = new object[4];
            parameters[0] = null;
            parameters[1] = "Attribute";
            parameters[2] = networkObject.Network;
            parameters[3] = networkObject.NetworkID;
            
            List<String> listAttribute = new List<String>();
            foreach (String key in hashAttributeYear.Keys)
            {
                listAttribute.Add(key);
            }
            listAttribute.Sort();

            foreach (String strAttribute in listAttribute)
            {
                if (hashAttributeYear.ContainsKey(strAttribute))
                {
                    TreeNode tnAttribute = new TreeNode(strAttribute, 19, 19);
                    tnAttribute.Tag = parameters;
                    tn.Nodes.Add(tnAttribute);

                    TreeNode tempNode = null;
                    List<String> listYear = (List<String>)hashAttributeYear[strAttribute];
                    foreach (String strYear in listYear)
                    {
                        tempNode = new TreeNode(strYear, 2, 2);
                        tempNode.Tag = parameters;
                        tnAttribute.Nodes.Add(tempNode);
                    }
                    tempNode = new TreeNode(strAttribute, 2, 2);
                    tempNode.Tag = parameters;
                    tnAttribute.Nodes.Add(tempNode);
                }
            }
        }

        private void LoadNetworkDefinition()
        {
           
            TreeNode treeNodeNetworkDefinition = new TreeNode("Network Definition",19,19);
            treeViewSolutionExplorer.Nodes.Add(treeNodeNetworkDefinition);

            TreeNode treeNodeLinear = new TreeNode("Linear Referenced Routes", 14,14);
            //FormNetworkDefinition formLinearDefinition = new FormNetworkDefinition(true);
            //treeNodeLinear.Tag = formLinearDefinition;
            object[] parameters = new object[2];
            parameters[0] = null;
            parameters[1] = "LinearNetworkDefinition";
            treeNodeLinear.Tag = parameters;
            treeNodeNetworkDefinition.Nodes.Add(treeNodeLinear);

            TreeNode treeNodeSection = new TreeNode("Section Referenced Routes", 14, 14);
            parameters = new object[2];
            parameters[0] = null;
            parameters[1] = "SectionNetworkDefinition";
            treeNodeSection.Tag = parameters;
            treeNodeNetworkDefinition.Nodes.Add(treeNodeSection);


        }


        private void LoadAttributes()
        {
            TreeNode treeNodeNetworkAttribute = new TreeNode("Raw Attributes", 8, 8);
            treeNodeNetworkAttribute.ContextMenuStrip = contextMenuStripRawAttribute;
            treeViewSolutionExplorer.Nodes.Add(treeNodeNetworkAttribute);

            List<String> listAttributes;

            try
            {
                listAttributes = GlobalDatabaseOperations.GetRawAttributes();
                foreach (String strAttribute in listAttributes)
                {
                    TreeNode treeNodeAttribute = new TreeNode(strAttribute, 2, 2);
                    treeNodeAttribute.ContextMenuStrip = contextMenuStripRawAttributeNode;
//                    FormAttributeDocument formAttributeDocument = new FormAttributeDocument(strAttribute);
//                    treeNodeAttribute.Tag = formAttributeDocument;
                    object[] parameters = new object[3];
                    parameters[0] = null;
                    parameters[1] = "RawAttribute";
                    parameters[2] = strAttribute;
                    treeNodeAttribute.Tag = parameters;
                    treeNodeNetworkAttribute.Nodes.Add(treeNodeAttribute);
                }
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Retrieve Raw Attibutes. " + except.Message);
            }
        }




        private void treeViewSolutionExplorer_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (treeViewSolutionExplorer.SelectedNode.Tag != null)
            {
                object[]parameters = (object[])treeViewSolutionExplorer.SelectedNode.Tag;
                if (parameters[0] == null)
                {
                    CreateForms(parameters);
                }

                List<String> listAttributes = IsAttributeViewNode(treeViewSolutionExplorer.SelectedNode);
                if(listAttributes != null)
                {
                    BaseForm form = (BaseForm)parameters[0];
                    form.UpdateNode(OutputWindow.DockPanel, listAttributes);

                }
                else
                {
                    BaseForm form = (BaseForm)parameters[0];
                   form.UpdateNode(OutputWindow.DockPanel, new Object());
                }
            }
        }

        private List<string> IsAttributeViewNode(TreeNode tn)
        {
            List<string> listAttributes = new List<string>();
            if (tn.Parent != null)
            {
                if (tn.Parent.Text == "Attribute View")
                {
                    String strAttribute = tn.Text;
                    foreach (TreeNode treeNode in tn.Nodes)
                    {
                        if (strAttribute != treeNode.Text)
                        {
                            listAttributes.Add(strAttribute + "_" + treeNode.Text);

                        }
                        else
                        {
                            listAttributes.Add(strAttribute);
                        }
                    }
                    return listAttributes;
                }
                
                if (treeViewSolutionExplorer.SelectedNode.Parent.Parent != null)
                {
                    String strAttribute = tn.Parent.Text;
                    if (treeViewSolutionExplorer.SelectedNode.Parent.Parent.Text == "Attribute View")
                    {
                        if (strAttribute == tn.Text)
                        {
                            listAttributes.Add(strAttribute);
                        }
                        else
                        {

                            listAttributes.Add(strAttribute + "_" + tn.Text);
                        }
                    }
                    return listAttributes;
                }
            }

            return null;
        }



        /// <summary>
        /// Add new Attribute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteRawAttributeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            String strSelectedAttribute = treeViewSolutionExplorer.SelectedNode.Text;
            if (MessageBox.Show(this, "Deleting an attribute cannot be undone, continue with DELETING Atttribute:" + strSelectedAttribute + "?", "Delete attribute Confirmation", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            // Create a transaction to delete the attribute and then remove it from the solution explorer list.
            try
            {
                DeleteAttribute deleteAttribute = new DeleteAttribute(strSelectedAttribute);
                deleteAttribute.DeleteFromImageView();
                if (treeViewSolutionExplorer.SelectedNode.Tag != null)
                {
                    object[] parameters = (object[])treeViewSolutionExplorer.SelectedNode.Tag;
                    if (parameters[0] != null)
                    {
                        BaseForm form = (BaseForm)parameters[0];
                        if (form != null)
                        {
                            form.Close();
                        }
                    }
                }
                
                treeViewSolutionExplorer.SelectedNode.Remove();
            }
            catch (Exception exc)
            {
                OutputWindow.WriteOutput("Error: Could not delete attribute table. " + exc.Message);
                return;
            }
        }

        private void FormImageViewSolutionExplorer_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void treeViewSolutionExplorer_MouseDown(object sender, MouseEventArgs e)
        {
            {
                if (e.Button == MouseButtons.Right)
                {

                    this.treeViewSolutionExplorer.SelectedNode = this.treeViewSolutionExplorer.GetNodeAt(e.X, e.Y);

                }
            }
        }

        private void addNewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NetworkProperties networkProp = new NetworkProperties("");
            String strNewNetwork = ShowProperties(true, networkProp, "Add New Network");
            try
            {
                List<NetworkObject> listNetworks = GlobalDatabaseOperations.GetNetworks();
                NetworkObject networkObject = listNetworks.Find(delegate(NetworkObject no) { return no.Network == strNewNetwork; });
                if (networkObject != null)
                {
                    AddNewNetwork(networkObject);
                }
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Error adding Network:" + strNewNetwork + ". " + except.Message);
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
                        Global.GetPropertyWindow().Show(OutputWindow.DockPanel, DockState.DockLeft);
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

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeViewSolutionExplorer.SelectedNode;
            foreach (TreeNode treeNode in treeViewSolutionExplorer.SelectedNode.Nodes)
            {
                if (treeNode.Tag != null)
                {
                    object[] parameters = (object[])treeNode.Tag;
                    if (parameters[0] != null)
                    {
                        BaseForm form = (BaseForm)parameters[0];
                        form.Close();
                    }
                }
            }
            try
            {
                GlobalDatabaseOperations.RemoveNetwork(tn.Text);
				//Global.SecurityOperations.RemoveNetworkActions( tn.Text );
                tn.Remove();
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Removing Network:" + tn.Text + ". " + except.Message);
            }
        }


        private void CreateForms(object[] arguments)
        {
            switch (arguments[1].ToString())
            {
                case "ImageViewer":
                    arguments[0] = new FormImageViewer(arguments[2].ToString());
                    break;
                case "RawAttribute":
                    FormAttributeDocument formAttributeDocument = new FormAttributeDocument(arguments[2].ToString());
                    arguments[0] = formAttributeDocument;
                    ImageViewManager.Navigation.AddFormEvent(formAttributeDocument);
                    break;
                case "LinearNetworkDefinition":
                    arguments[0] = new FormNetworkDefinition(true, null);
                    break;
                case "SectionNetworkDefinition":
                    arguments[0] = new FormNetworkDefinition(false, null);
                    break;
                case "GISViewer":
                    FormGISView formGISViewer = new FormGISView(arguments[2].ToString(), Global.GetAttributeYear(arguments[3].ToString()));
                    formGISViewer.Tag = arguments[2].ToString();
                    formGISViewer.TabText = arguments[2].ToString();
                    arguments[0] = formGISViewer;
                    ImageViewManager.Navigation.AddFormEvent(formGISViewer);
                    break;
                case "AssetView":
                    FormAssetView formAssetView = new FormAssetView(arguments[2].ToString(), Global.GetAttributeYear(arguments[2].ToString()));
                    arguments[0] = formAssetView;
                    break;
                case "Attribute":
                    Hashtable hashAttributeYear = Global.GetAttributeYear(arguments[3].ToString());
                    FormAttributeView formAttribute = new FormAttributeView(arguments[2].ToString(), hashAttributeYear);
                    arguments[0] = formAttribute;
                    ImageViewManager.Navigation.AddFormEvent(formAttribute);
                    break;
                case "SectionView":
                    FormSectionView formSectionView = new FormSectionView(arguments[2].ToString(), Global.GetAttributeYear(arguments[3].ToString()));
                    arguments[0] = formSectionView;
                    ImageViewManager.Navigation.AddFormEvent(formSectionView);
                    break;
                case "GoogleView":
                    FormGoogleMap formGoogleMap = new FormGoogleMap();
                    arguments[0] = formGoogleMap;
                    ImageViewManager.Navigation.AddFormEvent(formGoogleMap);
                    break;
                case "DynamicSegmentation":
                    NetworkObject network = (NetworkObject)arguments[2];
                    FormImageViewSegmentation formSegmentation = new FormImageViewSegmentation(network);
                    arguments[0] = formSegmentation; 
                    break;
            }
            return;
        }

        private void updateImagePathToolStripMenuItem_Click(object sender, EventArgs e)
        {

            String strImagePath = RCImageView3.Properties.Settings.Default.IMAGEPATH;
            folderBrowserDialogImagePath.SelectedPath = strImagePath;
            if (folderBrowserDialogImagePath.ShowDialog() == DialogResult.OK)
            {
                strImagePath = folderBrowserDialogImagePath.SelectedPath;
                RCImageView3.Properties.Settings.Default.IMAGEPATH = strImagePath;
            }
            ImageViewManager.Navigation.Navigation.ImagePath = strImagePath;
        }
    }
}
