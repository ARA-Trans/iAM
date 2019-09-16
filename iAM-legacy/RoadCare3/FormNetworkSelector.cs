using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;

namespace RoadCare3
{
	public partial class FormNetworkSelector : Form
	{
		private int m_networkID;
		public bool m_IsValidID = true;

		public int NetworkID
		{
			get { return m_networkID; }
		}

		public FormNetworkSelector()
		{
			InitializeComponent();

			// Add network names to the combo box so the user can select one.
			string getNetworkNames = "SELECT NETWORK_NAME FROM NETWORKS";
			DataSet networkNames = DBMgr.ExecuteQuery(getNetworkNames);
			foreach (DataRow networkName in networkNames.Tables[0].Rows)
			{
				cbAllNetworks.Items.Add(networkName["NETWORK_NAME"].ToString());
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			SetNetworkID();
		}

		public void SetNetworkID()
		{
			// After selecting a network, get the network ID for it.
			string getNetworkID = "SELECT NETWORKID FROM NETWORKS WHERE NETWORK_NAME = '" + cbAllNetworks.Text + "'";
			try
			{
				m_networkID = int.Parse( DBMgr.ExecuteQuery( getNetworkID ).Tables[0].Rows[0]["NETWORKID"].ToString() );
				this.Close();
			}
			catch( Exception exc )
			{
				Global.WriteOutput( "Error getting network ID for network:" + cbAllNetworks.Text + ". Please verify network exists.  [" + exc.Message + "]" );
				m_IsValidID = false;
			}
		}

		private void FormNetworkSelector_Load(object sender, EventArgs e)
		{
			cbAllNetworks.SelectedIndex = 0;
		}


	}
}
