using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;
using RoadCareDatabaseOperations;

namespace RoadCare3
{
    public partial class FormCreateRemoteNetworkDefinition : Form
    {
        private ConnectionParameters _cp;

        public ConnectionParameters ConnectionParameters
        {
            get { return _cp; }
        }

        public FormCreateRemoteNetworkDefinition()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (textBoxConnName.Text == "")
            {
                MessageBox.Show("You must enter a Connection Name.", "RoadCare3");
            }
            else
            {
                string activeTabID = tcLogin.SelectedTab.Name;
                _cp = CreateConnectionParameters(activeTabID);
                DBOp.AddToConnectionParameters(_cp, textBoxConnName.Text);
            }
        }

        private ConnectionParameters CreateConnectionParameters(string activeTabID)
        {
            bool integratedSec = chkUseIntegratedSecurity.Checked;
            ConnectionParameters cp = null;
            switch (activeTabID)
            {
                case "TabSqlServer":
                    cp = new ConnectionParameters("", "", "",
                        tbMSSQLUserName.Text,
                        tbMSSQLPassword.Text,
                        integratedSec,
                        tbMSSQLServerName.Text,
                        tbMSSQLDatabaseName.Text,
                        textBoxConnName.Text, richTextBoxSQL.Text, "", "",
                        "MSSQL",
                        true);
                    break;
                case "tabOracle":
                    cp = new ConnectionParameters(
                        tbPort.Text,
                        tbSID.Text,
                        tbNetworkAlias.Text,
                        txtOracleUserID.Text,
                        txtOraclePassword.Text,
                        integratedSec,
                        tbOracleServerName.Text,
                        "", textBoxConnName.Text, richTextBoxSQL.Text, "", "",
                        "ORACLE",
                        true);
                    break;
                default:
                    throw new NotImplementedException("These tabs are currently not implemented.");
            }
            return cp;
        }

        private void chkUseIntegratedSecurity_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseIntegratedSecurity.Checked)
            {
                tbMSSQLUserName.ReadOnly = true;
                tbMSSQLPassword.ReadOnly = true;

                txtOracleUserID.ReadOnly = true;
                txtOraclePassword.ReadOnly = true;
            }
            else
            {
                tbMSSQLUserName.ReadOnly = false;
                tbMSSQLPassword.ReadOnly = false;

                txtOracleUserID.ReadOnly = false;
                txtOraclePassword.ReadOnly = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DBMgr.ExecuteQuery(richTextBoxSQL.Text);
                MessageBox.Show("Query Successfull.", "RoadCare3");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
