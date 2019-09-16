using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RoadCare3
{
    public partial class FormEditBudgets : Form
    {
        List<String> m_listBudgets = new List<String>();
        public String m_strBudget;


        public FormEditBudgets(String strBudgets)
        {
            InitializeComponent();
            if(strBudgets.Length > 0)
            {
                string[] budgets = strBudgets.Split(',');
                for (int i = 0; i < budgets.Length; i++)
                {
                    m_listBudgets.Add(budgets[i]);
                }
            }
        }

        private void FormEditBudgets_Load(object sender, EventArgs e)
        {
            foreach (String str in m_listBudgets)
            {
                listBoxBudgets.Items.Add(str);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            String strAdd = "";
            strAdd = textBoxAdd.Text.Trim();
            if (strAdd.Length == 0) return;
            
            if(strAdd.Contains(" "))
            {    
                Global.WriteOutput("Error: Blank spaces not allowed in Budget names");
                return;
            }
            if(strAdd.Contains("'"))
            {   
                Global.WriteOutput("Error: Single quotes ' not allowed in Budget names");
                return;
            }

            if(strAdd.Contains("\""))
            {    
                Global.WriteOutput("Error: Double quotes \" not allowed in Budget names");
                return;
            }

            if (!listBoxBudgets.Items.Contains(strAdd))
            {

                if (listBoxBudgets.SelectedIndex < 0)
                {
                    listBoxBudgets.Items.Add(strAdd);
                }
                else
                {
                    int nIndex = listBoxBudgets.SelectedIndex;
                    listBoxBudgets.Items.Insert(nIndex, strAdd);
                }
                textBoxAdd.Text = "";
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int nIndex = listBoxBudgets.SelectedIndex;

            if (nIndex >= 0)
            {
                listBoxBudgets.Items.RemoveAt(nIndex);
            }
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            int nIndex = listBoxBudgets.SelectedIndex;
            if (nIndex <= 0) return;

            String strBudget = listBoxBudgets.Items[nIndex].ToString();
            listBoxBudgets.Items.RemoveAt(nIndex);
            listBoxBudgets.Items.Insert(nIndex - 1, strBudget);
            listBoxBudgets.SelectedIndex = nIndex - 1;


        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            int nIndex = listBoxBudgets.SelectedIndex;
            if (nIndex < 0) return;
            if (nIndex == listBoxBudgets.Items.Count-1) return;

            String strBudget = listBoxBudgets.Items[nIndex].ToString();
            
            listBoxBudgets.Items.Insert(nIndex+2, strBudget);
            
            listBoxBudgets.Items.RemoveAt(nIndex);
            listBoxBudgets.SelectedIndex = nIndex+1;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            m_strBudget = "";
            if (listBoxBudgets.Items.Count == 0)
            {
                Global.WriteOutput("Error: At least a single budget must be entered.");
                return;
            }

            for (int i = 0; i < listBoxBudgets.Items.Count; i++)
            {
                if (m_strBudget.Length > 0) m_strBudget += ",";
                m_strBudget += listBoxBudgets.Items[i].ToString();
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
