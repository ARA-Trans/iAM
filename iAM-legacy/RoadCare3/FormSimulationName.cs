using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;
using System.Collections;

namespace RoadCare3
{
    public partial class FormSimulationName : Form
    {
        private string m_strSimulationName;
        Hashtable m_htNetwork = new Hashtable();
        bool m_bNewSimulation;
        String m_strNetworkInitialID;
        String m_strNetwork = "";
        String m_strNewNetworkID = "";

        public string SimulationName
        {
            set { m_strSimulationName = value; }
            get { return m_strSimulationName; }
        }

        public String NewNetworkID
        {
            get {return m_strNewNetworkID; }
            set { m_strNewNetworkID = value; }
        }

        public FormSimulationName(bool bNewSimulation, String strNetworkID)
        {
            InitializeComponent();
            m_strNetworkInitialID = strNetworkID;
            m_bNewSimulation = bNewSimulation;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            m_strSimulationName = textBoxSimulationName.Text.ToString();
            m_strNewNetworkID = m_htNetwork[cbNetwork.Text].ToString();
            if (!Global.CheckString(m_strSimulationName)) return;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FormSimulationName_Load(object sender, EventArgs e)
        {
            textBoxSimulationName.Text = SimulationName;
            String strSelect = "SELECT NETWORK_NAME,NETWORKID FROM NETWORKS";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    String strNetwork = dr["NETWORK_NAME"].ToString();
                    String strNetworkID = dr["NETWORKID"].ToString();

                    if (strNetworkID == m_strNetworkInitialID)
                    {
                        m_strNetwork = strNetwork;
                    }

                    if(m_bNewSimulation)
                    {
                        if(strNetworkID == m_strNetworkInitialID)
                        {
                            cbNetwork.Items.Add(strNetwork);
                            m_htNetwork.Add(strNetwork, strNetworkID);
                        }
                    }
                    else
                    {
                        cbNetwork.Items.Add(strNetwork);
                        m_htNetwork.Add(strNetwork, strNetworkID);
                    }
                }
            }
            catch
            {

            }
            cbNetwork.Text = m_strNetwork;
        }
    }
}
