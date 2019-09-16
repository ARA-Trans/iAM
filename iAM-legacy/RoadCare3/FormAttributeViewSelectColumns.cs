using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;
using DatabaseManager;


namespace RoadCare3
{
    public partial class FormAttributeViewSelectColumns : Form
    {
        Hashtable m_hashAttributeYear;
        public List<String> m_listColumns;
        bool m_bChange = false;

        public FormAttributeViewSelectColumns(List<String>listColumns, Hashtable hashAttributeYear)
        {
            InitializeComponent();
            m_hashAttributeYear = hashAttributeYear;
            m_listColumns = listColumns;
        }

        private void FormAttributeViewSelectColumns_Load(object sender, EventArgs e)
        {
            checkBoxOrder.Checked = Global.MostRecentFirst;

            //Fill Attribute box
            String strSelect = "SELECT DISTINCT ATTRIBUTE_ FROM ATTRIBUTES_ ORDER BY ATTRIBUTE_";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                comboBoxAttribute.Items.Add(row[0].ToString());
            }
            comboBoxAttribute.SelectedIndex = 0;

            foreach (String strColumn in m_listColumns)
            {
                listBoxColumn.Items.Add(strColumn);

            }
            m_bChange = true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            m_listColumns.Clear();

            for(int i = 0; i < listBoxColumn.Items.Count; i++)
            {
                m_listColumns.Add(listBoxColumn.Items[i].ToString());
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            //Make a list of new indexes.
            List<int> listIndex = new List<int>();
            List<String> listUp = new List<String>();


            foreach (String strAttributeYear in listBoxColumn.SelectedItems)
            {
                int nIndex = listBoxColumn.FindStringExact(strAttributeYear);
                if(!listIndex.Contains(nIndex-1) && (nIndex-1) >= 0)
                {
                    listIndex.Add(nIndex-1);
                }
                else
                {
                    listIndex.Add(nIndex);
                }
                listUp.Add(strAttributeYear);
            }
        
            // Delete selected items        
            while (listBoxColumn.SelectedItems.Count > 0)
            {
                listBoxColumn.Items.Remove(listBoxColumn.SelectedItems[0]);
            }
        
            //Re-add moved up one
            int n = 0;
            foreach(String strAttributeYear in listUp)
            {
                int nIndex = listIndex[n];
                listBoxColumn.Items.Insert(nIndex, strAttributeYear);
                n++;
            }
            //Reselect items
            foreach (int nIndex in listIndex)
            {
                listBoxColumn.SetSelected(nIndex, true);
            }

        }



        private void buttonDelete2_Click(object sender, EventArgs e)
        {
            while (listBoxColumn.SelectedItems.Count > 0)
            {
                listBoxColumn.Items.Remove(listBoxColumn.SelectedItems[0]);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            String strAttribute = comboBoxAttribute.Text;

            foreach (String strYear in listBoxYear.SelectedItems)
            {
                if(!m_hashAttributeYear.Contains(strYear))
                {
                    String strAttributeYear = strAttribute + "_" + strYear;
                    if (!listBoxColumn.Items.Contains(strAttributeYear))
                    {
                        listBoxColumn.Items.Add(strAttributeYear);

                    }
                }
                else
                {
                    if (!listBoxColumn.Items.Contains(strYear))
                    {
                        listBoxColumn.Items.Add(strYear);

                    }
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            while (listBoxColumn.SelectedItems.Count > 0)
            {
                listBoxColumn.Items.Remove(listBoxColumn.SelectedItems[0]);
            }
        }

        private void comboBoxAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            //When simulation done, will also attach simulation years to this control.
            UpdateYearList();

        }

        private void UpdateYearList()
        {
            //Fill year box
            listBoxYear.Items.Clear();
            String strAttribute = comboBoxAttribute.Text;
            if (m_hashAttributeYear.Contains(strAttribute))
            {
                if (!checkBoxOrder.Checked)
                {
                    List<String> listYear = (List<String>)m_hashAttributeYear[strAttribute];
                    foreach (String strYear in listYear)
                    {
                        listBoxYear.Items.Add(strYear);
                    }
                    listBoxYear.Items.Add(strAttribute);
                }
                else
                {
                    listBoxYear.Items.Add(strAttribute);

                    List<String> listYear = (List<String>)m_hashAttributeYear[strAttribute];
                    for(int i = listYear.Count -1; i >= 0; i--)
                    {
                        listBoxYear.Items.Add(listYear[i]);
                    }
                }
            }

        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            //Make a list of new indexes.
            List<int> listIndex = new List<int>();
            List<String> listUp = new List<String>();


            foreach (String strAttributeYear in listBoxColumn.SelectedItems)
            {
                int nIndex = listBoxColumn.FindStringExact(strAttributeYear);
                if (!listIndex.Contains(nIndex + 1) && (nIndex + 1) < listBoxColumn.Items.Count)
                {
                    listIndex.Add(nIndex + 1);
                }
                else
                {
                    listIndex.Add(nIndex);
                }
                listUp.Add(strAttributeYear);
            }

            // Delete selected items        
            while (listBoxColumn.SelectedItems.Count > 0)
            {
                listBoxColumn.Items.Remove(listBoxColumn.SelectedItems[0]);
            }

            //Re-add moved up one
            int n = 0;
            foreach (String strAttributeYear in listUp)
            {
                int nIndex = listIndex[n];
                if (nIndex >= listBoxColumn.Items.Count) nIndex = listBoxColumn.Items.Count;
                listBoxColumn.Items.Insert(nIndex, strAttributeYear);
                n++;
            }
            //Reselect items
            foreach (int nIndex in listIndex)
            {
                listBoxColumn.SetSelected(nIndex, true);
            }

        }

        private void checkBoxOrder_CheckedChanged(object sender, EventArgs e)
        {
            if (m_bChange)
            {
                UpdateYearList();
                Global.MostRecentFirst = checkBoxOrder.Checked;
            }
        }

 

    }
}