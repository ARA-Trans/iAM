using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;
using RoadCare3.Properties;
using System.Net;
using System.Net.Sockets;
using System.IO;
using LogicNP.CryptoLicensing;
using RoadCareDatabaseOperations;
using System.Reflection;

namespace RoadCare3
{
    public partial class FormLogin : Form
    {
        private bool m_bUseIntegratedSecurity = true;

        private static string licensefilepath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\RoadCare\\RoadCare.LogicNP.txt";
        private static int licensetype;
        public static string licCode = "";
        static string validationKey1 = "";
        static string licenseURL = "";

        public FormLogin()
        {
            InitializeComponent();
            tcLogin.TabPages.Remove(tabPageDBF);
            tcLogin.TabPages.Remove(TabAccess);
            rbNetworkAlias.Checked = true;

            ActiveControl = btnLogin;

            label7.Text = $"Version {Assembly.GetExecutingAssembly().GetName().Version} for 12.6.2019";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string activeTabID = tcLogin.SelectedTab.Name;
            m_bUseIntegratedSecurity = chkUseIntegratedSecurity.Checked;
            ConnectionParameters cpNative = CreateConnectionParameters(activeTabID);
            if (cpNative != null)
            {
                try
                {
                    DBMgr.NativeConnectionParameters = cpNative;
                    switch (cpNative.Provider)
                    {
                        case "MSSQL":
                            Settings.Default.DBDATABASE = tbMSSQLDatabaseName.Text;
                            Settings.Default.USERNAME_DIU = tbRoadCareUserName.Text;
                            Settings.Default.DBSERVER = tbMSSQLServerName.Text;
                            Settings.Default.DefaultTab = "MSSQL";
                            Settings.Default.USE_INTEGRATED_SECURITY = chkUseIntegratedSecurity.Checked;
                            break;
                        case "ORACLE":
                            Settings.Default.DBPORT = tbPort.Text;
                            Settings.Default.USERNAME_DIU = tbRoadCareUserName.Text;
                            Settings.Default.DBSERVER = tbOracleServerName.Text;
                            Settings.Default.DBNETWORKALIAS = tbNetworkAlias.Text;
                            Settings.Default.DBSID = tbSID.Text;
                            Settings.Default.USE_INTEGRATED_SECURITY = false;
                            Settings.Default.DefaultTab = "ORACLE";
                            Settings.Default.ORACLE_USE_SID = rbSID.Checked;
                            Settings.Default.ORACLE_DB_USERNAME = txtOracleUserID.Text;
                            Settings.Default.ORACLE_DB_PASSWORD = txtOraclePassword.Text;
                            break;
                        default:
                            break;
                    }
                    Settings.Default.LAST_LOGIN = tbRoadCareUserName.Text;
                    Settings.Default.Save();
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Error connecting to database. Please check the database name and try again.\n" + exc.Message, "RoadCare3");
                    return;
                }

                // This is  a compliment to the update tables in CheckFreshInstall
                DBOp.UpdateTables();




                Global.SecurityOperations.CheckFreshInstall();
                try
                {
                    Global.SecurityOperations.SetCurrentUser(tbRoadCareUserName.Text, tbRoadCarePassword.Text);
                }
                catch
                {
                    MessageBox.Show("User failed to Login. Username and/or Password incorrect.");
                    return;
                }
                if (!Global.SecurityOperations.IsAuthenticated)
                {
                    MessageBox.Show("User failed to Login. Username and/or Password incorrect.");
                    return;
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                Global.WriteOutput("Couldn't create system connection parameters.");
            }
        }

        private void GetLicenseFileInfo()
        {
            if (File.Exists(licensefilepath))
            {
                licensetype = LicenseInfo.GetLicenseType(licensefilepath);  //INDIVIDUAL/workstation, INTRANET/concurrent
                validationKey1 = LicenseInfo.GetLicenseValidationKey(licensefilepath);
                licenseURL = LicenseInfo.GetLicenseURL(licensefilepath);

                CheckValidLicense();
            }
            else
            {
                MessageBox.Show("File " + licensefilepath + " must exist for license activation.");
                btnLogin.Enabled = false;
            }
        }

        private bool CheckValidLicense()
        {
            bool isGoodLicense = false;
            if (validationKey1 != "")
            {
                LicenseInfo.currentlicense = new CryptoLicense(LicenseStorageMode.ToRegistry, validationKey1);
                LicenseInfo.currentlicense.LicenseServiceURL = licenseURL;
                if (licensetype == 2) LicenseInfo.currentlicense.UseHashedMachineCode = false;

                LicenseInfo.currentlicense.Load();
                //int count = license.GetCurrentActivationCount();

                if (LicenseInfo.currentlicense.Status != LicenseStatus.ActivationFailed
                    && LicenseInfo.currentlicense.Status != LicenseStatus.Valid)
                {
                    btnLogin.Enabled = false;
                    labelLicenseNumber.Text = "Unlicensed";
                    labelLicenseNumber.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    isGoodLicense = true;
                    licCode = LicenseInfo.currentlicense.LicenseCode;
                    if (LicenseInfo.currentlicense.ActivationsAreFloating == true)   // a floating license
                    {
                        // Validate license...
                        if (InternetConnectionExists())
                        {
                            if (LicenseInfo.currentlicense.Status != LicenseStatus.Valid)
                            {
                                btnLogin.Enabled = false;
                                if (LicenseInfo.currentlicense.Status == LicenseStatus.ActivationFailed)
                                {
                                    if (LicenseInfo.currentlicense.GetCurrentActivationCount() == LicenseInfo.currentlicense.MaxActivations)  // reach the max or be rejected
                                    {
                                        btnActivate.Enabled = false;
                                        labelLicenseNumber.Text = "No license seats available.";
                                        labelLicenseNumber.ForeColor = System.Drawing.Color.Red;
                                    }
                                    else  // user is blocked
                                    {
                                        btnActivate.Enabled = false;
                                        labelLicenseNumber.Text = "Activation Failed";
                                        labelLicenseNumber.ForeColor = System.Drawing.Color.Red;
                                    }
                                }
                                else
                                {
                                    labelLicenseNumber.Text = "Unlicensed";
                                    labelLicenseNumber.ForeColor = System.Drawing.Color.Red;
                                }
                                isGoodLicense = false;
                            }
                            else
                            {
                                isGoodLicense = true;
                                btnActivate.Enabled = false;
                                btnActivate.Visible = false;

                                if (LicenseInfo.currentlicense.HasMaxUsageDays)
                                {
                                    labelLicenseNumber.Text = "Standard (" + LicenseInfo.currentlicense.RemainingUsageDays + " days left)";
                                    labelLicenseNumber.Text = "Standard";
                                }
                                else
                                {
                                    labelLicenseNumber.Text = "Standard";
                                    labelLicenseNumber.ForeColor = System.Drawing.Color.Green;
                                }
                                // Hook up event
                                LicenseInfo.currentlicense.FloatingHeartBeat += new FloatingHeartBeatHandler(lic_FloatingHeartBeat);
                            }
                        }
                        else
                        {
                            btnActivate.Enabled = false;
                            labelLicenseNumber.Text = "No Connection";
                            labelLicenseNumber.ForeColor = System.Drawing.Color.Red;
                            isGoodLicense = false;
                        }

                    }
                    else   // not a floating license, loaded from Registry
                    {
                        if (LicenseInfo.currentlicense.IsEvaluationLicense())
                        {
                            if (!LicenseInfo.currentlicense.IsEvaluationExpired())
                            {
                                btnActivate.Enabled = false;

                                int myday = LicenseInfo.currentlicense.RemainingUsageDays;
                                btnActivate.Visible = false;
                                labelLicenseNumber.Text = "Standard (Expire at " + String.Format("{0:MMMM d, yyyy}", (System.DateTime.Now).AddDays(LicenseInfo.currentlicense.RemainingUsageDays)) + ")";
                                labelLicenseNumber.ForeColor = System.Drawing.Color.Green;
                                isGoodLicense = true;
                            }
                            else
                            {
                                btnLogin.Enabled = false;
                                btnActivate.Enabled = true;
                                btnActivate.Visible = true;
                                labelLicenseNumber.Text = "License expired.";
                                labelLicenseNumber.ForeColor = System.Drawing.Color.Red;
                                isGoodLicense = false;
                            }

                        }
                        else
                        {
                            btnActivate.Enabled = false;
                            btnActivate.Visible = false;
                            
                            if (LicenseInfo.currentlicense.HasDateExpires)
                            {
                                labelLicenseNumber.Text = "Standard (Expire at " + LicenseInfo.currentlicense.DateExpires.ToShortDateString() + ")";
                            }
                            else
                            {
                                labelLicenseNumber.Text = "Standard";
                            }
                            labelLicenseNumber.ForeColor = System.Drawing.Color.Green;
                            isGoodLicense = true;
                        }

                    }
                }
            }
            else
            {
                btnLogin.Enabled = false;

                labelLicenseNumber.Text = "Unlicensed";
                labelLicenseNumber.ForeColor = System.Drawing.Color.Red;
                isGoodLicense = false;
            }
            return isGoodLicense;
        }

        void lic_FloatingHeartBeat(object sender, FloatingHeartBeatEventArgs e)
        {
            // This event handler will be called on a different thread, so use Invoke to sync to UI thread.
            this.Invoke(new FloatingHeartBeatHandler(lic_HeartBeat_ui), new object[] { sender, e });
        }

        void lic_HeartBeat_ui(object sender, FloatingHeartBeatEventArgs e)
        {

            if (e.Result.ToString() == "Failed")
            {
                MessageBox.Show("Your license seat has been deactivated.  DarwinME will be closed.");
                LicenseInfo.currentlicense = null;

                foreach (System.Diagnostics.Process myProc in System.Diagnostics.Process.GetProcesses())
                {
                    if (myProc.ProcessName.Contains("DarwinME"))
                    {
                        //MessageBox.Show("From FormDarwinME: Found DarwinME, then kill it!");

                        myProc.Kill();
                        break;
                    }
                }

                this.Close();
            }

        }

        private bool InternetConnectionExists()
        {
            try
            {
                System.Net.Sockets.TcpClient clnt = new System.Net.Sockets.TcpClient("www.google.com", 80);
                clnt.Close();
                return true;
            }
            catch (System.Exception /*ex*/)
            {
                return false;
            }
        }

        private ConnectionParameters CreateConnectionParameters(string activeTabID)
        {
            ConnectionParameters cp = null;
            switch (activeTabID)
            {
                case "TabSqlServer":
                    cp = new ConnectionParameters("","","",
                        tbMSSQLUserName.Text,
                        tbMSSQLPassword.Text,
                        m_bUseIntegratedSecurity,
                        tbMSSQLServerName.Text,
                        tbMSSQLDatabaseName.Text,
                        "","","","",
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
                        m_bUseIntegratedSecurity,
                        tbOracleServerName.Text,
                        "", "", "", "", "",
                        "ORACLE",
                        true);
                    break;
                default:
                    throw new NotImplementedException("These tabs are currently not implemented.");
            }
            return cp;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

            #if DEBUG

            #endif

            //GetLicenseFileInfo();
            if (Settings.Default.DefaultTab.ToString() == "MSSQL")
            {
                tcLogin.SelectedTab = TabSqlServer;
            }
            else
            {
                tcLogin.SelectedTab = tabOracle;
                rbSID.Checked = Settings.Default.ORACLE_USE_SID;
                txtOraclePassword.Text = Settings.Default.ORACLE_DB_PASSWORD;
                txtOracleUserID.Text = Settings.Default.ORACLE_DB_USERNAME;
            }

            tbMSSQLDatabaseName.Text = Settings.Default.DBDATABASE;
            tbMSSQLServerName.Text = Settings.Default.DBSERVER;

            tbPort.Text = Settings.Default.DBPORT;
            tbOracleServerName.Text = Settings.Default.DBSERVER;
            tbNetworkAlias.Text = Settings.Default.DBNETWORKALIAS;
            tbSID.Text = Settings.Default.DBSID;
            chkUseIntegratedSecurity.Checked = Settings.Default.USE_INTEGRATED_SECURITY;

            tbRoadCareUserName.Text = Settings.Default.LAST_LOGIN;
            TabSqlServer.Enabled = true;
            #if DEBUG
                        tbRoadCarePassword.Text = "install";
                        tbRoadCareUserName.Text = "install";
                        tbMSSQLDatabaseName.Text = "BridgeCare_2018";
                        tbMSSQLServerName.Text = "PDKB6W6003\\SQLEXPRESS";
                        tbMSSQLUserName.Text = "";
                        tbMSSQLPassword.Text = "";
                        chkUseIntegratedSecurity.Checked = true;
                        tcLogin.SelectedIndex= 0;
            #endif
        }

        private void FormLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btnLogin_Click(btnLogin, null);
            }
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

        private void btnActivate_Click(object sender, EventArgs e)
        {
            FormActivateLicense formActivateLicense = new FormActivateLicense();
            formActivateLicense.ShowDialog();
        }

        private void rbNetworkAlias_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNetworkAlias.Checked)
            {
                tbSID.Enabled = false;
                tbOracleServerName.Enabled = false;
                tbPort.Enabled = false;
                tbNetworkAlias.Enabled = true;
            }
        }

        private void rbSID_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSID.Checked)
            {
                tbOracleServerName.Enabled = true;
                tbPort.Enabled = true;
                tbNetworkAlias.Enabled = false;
                tbSID.Enabled = true;
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

    }
}
