using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace DataEntryChecker
{
    public static class DataCheck
    {
        private static bool m_bNodeExists = false;
        private static String m_strAddedNode = "";

        /// <summary>
        ///  Function to test whether the string is valid number or not
        /// </summary>
        /// <param name="strNumber"></param>
        /// <returns></returns>
        public static bool IsNumber(String strNumber)
        {
            System.Text.RegularExpressions.Regex objNotNumberPattern = new System.Text.RegularExpressions.Regex("[^0-9.-]");
            System.Text.RegularExpressions.Regex objTwoDotPattern = new System.Text.RegularExpressions.Regex("[0-9]*[.][0-9]*[.][0-9]*");
            System.Text.RegularExpressions.Regex objTwoMinusPattern = new System.Text.RegularExpressions.Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            System.Text.RegularExpressions.Regex objNumberPattern = new System.Text.RegularExpressions.Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                   !objTwoDotPattern.IsMatch(strNumber) &&
                   !objTwoMinusPattern.IsMatch(strNumber) &&
                   objNumberPattern.IsMatch(strNumber);
        }

        /// <summary>
        /// Recursive function checks for node with matching DISPLAY TEXT value.
        /// Requires function CheckNodes(TreeView tn) and a string member variable m_strAddedNode, which is the node DISPLAY NAME
        /// to be added to the treeview.  Also requires a member boolean m_bNodeExists initialized to false.
        /// </summary>
        /// <param name="treeNode"></param>
        private static void CheckNodes(TreeNode treeNode)
        {
            if (treeNode.Text == m_strAddedNode)
            {
                m_bNodeExists = true;
                return;
            }
            foreach (TreeNode tn in treeNode.Nodes)
            {
                CheckNodes(tn);
            }
        }

        /// <summary>
        /// Recursively check a tree view for a node with a matching DISPLAY TEXT value. Requires a treeview named tv as member
        /// variable.
        /// </summary>
        /// <returns></returns>
        private static bool NodeExists(TreeView tv)
        {
            TreeNodeCollection nodes = tv.Nodes;
            foreach (TreeNode n in nodes)
            {
                if (!m_bNodeExists)
                {
                    CheckNodes(n);
                }
                else
                {
                    return m_bNodeExists;
                }
            }
            return m_bNodeExists;
        }

        /// <summary>
        /// Populates a combo box populated with the List of Strings passed to it.
        /// </summary>
        /// <param name="cb">The combo box to populate</param>
        /// <param name="list">String list to add to the combo box</param>
        public static void PopulateComboBox(ref ComboBox cb, List<String> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                cb.Items.Add(list[i].ToString());
            }
        }

        
    }
}
