using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RoadCare3
{
    public class SolutionExplorerTreeNode : TreeNode
    {
        NetworkDefinition _networkDefinition;

        public NetworkDefinition NetworkDefinition
        {
            get { return _networkDefinition; }
        }

        public SolutionExplorerTreeNode()
        {
            _networkDefinition = null;
        }

        public SolutionExplorerTreeNode(NetworkDefinition networkDefinition)
        {
            _networkDefinition = networkDefinition;
        }



    }
}
