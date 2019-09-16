using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;

namespace RoadCare3
{
    public partial class FormAttributeResults : Form
    {
        

        public FormAttributeResults(List<string> tableColumnNames, List<string> markSelected)
        {
            InitializeComponent();

            // First check to see what attribute columns are currently shown in the results view and match them with selections
            // made. Check those boxes in the check list box. 
            foreach (string tableColumnName in tableColumnNames)
            {
                if (markSelected.Contains(tableColumnName))
                {
                    checkedListBoxAttributes.Items.Add(tableColumnName, true);
                }
                else
                {
                    checkedListBoxAttributes.Items.Add(tableColumnName, false);
                }
            }
            
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
