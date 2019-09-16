using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RoadCare3;
using DataObjects;
using RoadCareGlobalOperations;

namespace RCImageView3
{
    /// <summary>
    /// Acts as FormManager-eque helper for RCImageView
    /// </summary>
    public static class ImageViewManager
    {
        static private TreeView m_treeViewImageView;
        static private TreeNode m_treeNodeNetwork;
        static private List<NetworkObject> m_listNetworkObject;
        static private List<String> m_listViews;
        static private FormNavigation m_formNavigation;

        /// <summary>
        /// ImageView Solution Explorer TreeView
        /// </summary>
        public static TreeView TreeView
        {
            set { m_treeViewImageView = value; }
            get { return m_treeViewImageView; }
        }
        /// <summary>
        /// Networks Node in ImageView
        /// </summary>
        public static TreeNode NetworkNode
        {
            set { m_treeNodeNetwork = value; }
            get { return m_treeNodeNetwork; }
        }
        /// <summary>
        /// List of Networks in ImageView
        /// </summary>
        public static List<NetworkObject> Networks
        {
            set { m_listNetworkObject = value; }
            get { return m_listNetworkObject; }
        }

        /// <summary>
        /// List of Views in ImageView
        /// </summary>
        public static List<String> Views
        {
            set { m_listViews = value; }
            get { return m_listViews; }
        }

        /// <summary>
        /// Navigation Window
        /// </summary>
        public static FormNavigation Navigation
        {
            set { m_formNavigation = value; }
            get { return m_formNavigation; }
        }
        /// <summary>
        /// Closes all Open Network forms (recursive)
        /// </summary>
        public static void CloseForms(TreeNode treeNodeToClose)
        {
            foreach(TreeNode tn in treeNodeToClose.Nodes)
            {
                CloseForms(tn);
            }

            if (treeNodeToClose.Tag != null)
            {
                object[] parameters = (object[])treeNodeToClose.Tag;
                if (parameters[0] != null)
                {
                    BaseForm form = (BaseForm)parameters[0];
                    form.Close();
                }
            }
        }
        /// <summary>
        /// Updates all forms
        /// </summary>
        public static void UpdateNavigateTick(TreeNodeCollection Nodes,NavigationObject navigationObject)
        {
            foreach (TreeNode tn in Nodes)
            {
                if (tn.Tag != null)
                {
                    object[] parameters = (object[])tn.Tag;
                    if (parameters[0] != null)
                    {
                        BaseForm form = (BaseForm)parameters[0];
                        if (form != null)
                        {
                            if (form.Visible)
                            {
                                form.NavigationTick(navigationObject);
                                if (form.IsNavigationStop())
                                {
                                    ImageViewManager.Navigation.StopNavigation();
                                }
                            }
                        }
                    }
                }


                if (tn.Nodes != null)
                {
                    if (tn.Nodes.Count > 0)
                    {
                        UpdateNavigateTick(tn.Nodes, navigationObject);
                    }
                }
            }
        }
    }
}
