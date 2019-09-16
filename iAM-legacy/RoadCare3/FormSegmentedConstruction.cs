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
using WeifenLuo.WinFormsUI.Docking;
using RollupSegmentation;
using RoadCare3.Properties;

namespace RoadCare3
{
    public partial class FormSegmentedConstruction : BaseForm
    {
        String m_strNetwork;
        String m_strNetworkID;
        BindingSource binding;
        DataAdapter dataAdapter;
        DataTable table;

        public FormSegmentedConstruction(String strNetwork)
        {
            InitializeComponent();
            m_strNetwork = strNetwork;
            String strSelect = "SELECT NETWORKID FROM NETWORKS WHERE NETWORK_NAME='" + m_strNetwork + "'";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                m_strNetworkID = ds.Tables[0].Rows[0].ItemArray[0].ToString();

            }
            catch (Exception e)
            {
                String strError = "Error in getting NETWORKID for NETWORK_NAME = " + m_strNetwork + ". SQL error = " + e.Message;
                Global.WriteOutput(strError);
                MessageBox.Show(strError);

            }       

        }

        private void FormSegmentedConstruction_Load(object sender, EventArgs e)
        {
			SecureForm();
			FormLoad(Settings.Default.CONSTRUCTION_HISTORY_VIEW_IMAGE_KEY, Settings.Default.CONSTRUCTION_HISTORY_VIEW_IMAGE_KEY_SELECTED);

            this.TabText = "Construction View";
            labelHistory.Text = "Construction History:" + m_strNetwork;

            FillRouteTable();
        }

		protected void SecureForm()
		{
			//throw new NotImplementedException();
			//TODO: Add security code after the form is completed.
		}

        public String NetworkID
        {
            get { return m_strNetworkID; }
        }

        private void FillRouteTable()
        {
            comboBoxRouteFacility.Items.Add("All");
            String strSelect = "SELECT DISTINCT FACILITY FROM SECTION_" + m_strNetworkID.ToString();
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                comboBoxRouteFacility.Items.Add(row[0].ToString());
            }
            comboBoxRouteFacility.Text = "All";
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            UpdateSection();
        }

        private void UpdateSection()
        {
            //Fill the section table.  Only the SECTION_networkid
            String strSelect = "SELECT FACILITY,SECTION";

            //Then at a minumum
            strSelect += " FROM SECTION_" + m_strNetworkID.ToString();
            String strWhere;
            String strSearch = "";
            // Now for each table that is attached we add
            if (comboBoxRouteFacility.Text != "All")
            {
                strWhere = " WHERE FACILITY='" + comboBoxRouteFacility.Text + "'";
                strSelect += strWhere;

                if (strSearch != "")
                {
                    strSelect += " AND ";
                    strSelect += "(" + strSearch + ")";
                }
            }
            else
            {
                if (strSearch != "")
                {
                    strSelect += " AND ";
                    strSelect += "(" + strSearch + ")";
                }

            }

            try
            {
                binding = new BindingSource();
                dataAdapter = new DataAdapter(strSelect);

                // Populate a new data table and bind it to the BindingSource.
                table = new DataTable();

                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table);
                binding.DataSource = table;
                dgvSection.DataSource = binding;
                dgvSection.ReadOnly = true;
                dgvSection.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;

                dgvSection.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvSection.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Building Construction History View Facility/Section. SQL message is " + exception.Message);
            }

        }

        private void FormSegmentedConstruction_FormClosed(object sender, FormClosedEventArgs e)
        {
			FormUnload();

            FormManager.RemoveFormSegmentedConstruction(this);
        }


    }
}