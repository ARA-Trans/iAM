namespace DBSpy
{
    partial class frmConnect
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConnect));
            this.gbxSqlServer3 = new System.Windows.Forms.GroupBox();
            this.txtSqlServerInitialCat = new System.Windows.Forms.TextBox();
            this.lblSqlServerInitialCat = new System.Windows.Forms.Label();
            this.TabSqlServer = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbSqlConnType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbConnectionName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSqlProviderString = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gbxSqlServer5 = new System.Windows.Forms.GroupBox();
            this.btnSQLserverCancel = new System.Windows.Forms.Button();
            this.btnSqlServerTest = new System.Windows.Forms.Button();
            this.btnSqlServerOK = new System.Windows.Forms.Button();
            this.gbxSqlServer4 = new System.Windows.Forms.GroupBox();
            this.cbxIntegratedSecurity = new System.Windows.Forms.CheckBox();
            this.txtSqlServerPassword = new System.Windows.Forms.TextBox();
            this.txtSqlServerUserID = new System.Windows.Forms.TextBox();
            this.lblSqlServerPassword = new System.Windows.Forms.Label();
            this.lblSqlServerUserID = new System.Windows.Forms.Label();
            this.gbxSqlServer2 = new System.Windows.Forms.GroupBox();
            this.txtSqlServerName = new System.Windows.Forms.TextBox();
            this.lblSqlServerName = new System.Windows.Forms.Label();
            this.btnOracleCancel = new System.Windows.Forms.Button();
            this.btnOracleTest = new System.Windows.Forms.Button();
            this.gbxOracle4 = new System.Windows.Forms.GroupBox();
            this.btnOracleOK = new System.Windows.Forms.Button();
            this.chkUseIntegratedSecurity = new System.Windows.Forms.CheckBox();
            this.txtOraclePassword = new System.Windows.Forms.TextBox();
            this.txtOracleUserID = new System.Windows.Forms.TextBox();
            this.gbxOracle3 = new System.Windows.Forms.GroupBox();
            this.lblOraclePassword = new System.Windows.Forms.Label();
            this.lblOracleUserID = new System.Windows.Forms.Label();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.tabOracle = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbOracleConnType = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbOracleConnectionName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbOracleProvider = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gbxOracle2 = new System.Windows.Forms.GroupBox();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbOracleServerName = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.tbSID = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rbNetworkAlias = new System.Windows.Forms.RadioButton();
            this.tbNetworkAlias = new System.Windows.Forms.TextBox();
            this.gbxSqlServer3.SuspendLayout();
            this.TabSqlServer.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbxSqlServer5.SuspendLayout();
            this.gbxSqlServer4.SuspendLayout();
            this.gbxSqlServer2.SuspendLayout();
            this.gbxOracle4.SuspendLayout();
            this.gbxOracle3.SuspendLayout();
            this.TabControl1.SuspendLayout();
            this.tabOracle.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbxOracle2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxSqlServer3
            // 
            this.gbxSqlServer3.Controls.Add(this.txtSqlServerInitialCat);
            this.gbxSqlServer3.Controls.Add(this.lblSqlServerInitialCat);
            this.gbxSqlServer3.Location = new System.Drawing.Point(6, 275);
            this.gbxSqlServer3.Name = "gbxSqlServer3";
            this.gbxSqlServer3.Size = new System.Drawing.Size(336, 80);
            this.gbxSqlServer3.TabIndex = 999;
            this.gbxSqlServer3.TabStop = false;
            this.gbxSqlServer3.Text = "Database";
            // 
            // txtSqlServerInitialCat
            // 
            this.txtSqlServerInitialCat.Location = new System.Drawing.Point(40, 43);
            this.txtSqlServerInitialCat.Name = "txtSqlServerInitialCat";
            this.txtSqlServerInitialCat.Size = new System.Drawing.Size(248, 20);
            this.txtSqlServerInitialCat.TabIndex = 5;
            // 
            // lblSqlServerInitialCat
            // 
            this.lblSqlServerInitialCat.Location = new System.Drawing.Point(24, 24);
            this.lblSqlServerInitialCat.Name = "lblSqlServerInitialCat";
            this.lblSqlServerInitialCat.Size = new System.Drawing.Size(176, 16);
            this.lblSqlServerInitialCat.TabIndex = 999;
            this.lblSqlServerInitialCat.Text = "SQL Database Name:";
            // 
            // TabSqlServer
            // 
            this.TabSqlServer.Controls.Add(this.groupBox3);
            this.TabSqlServer.Controls.Add(this.groupBox1);
            this.TabSqlServer.Controls.Add(this.gbxSqlServer3);
            this.TabSqlServer.Controls.Add(this.gbxSqlServer5);
            this.TabSqlServer.Controls.Add(this.gbxSqlServer4);
            this.TabSqlServer.Controls.Add(this.gbxSqlServer2);
            this.TabSqlServer.Location = new System.Drawing.Point(4, 22);
            this.TabSqlServer.Name = "TabSqlServer";
            this.TabSqlServer.Size = new System.Drawing.Size(352, 528);
            this.TabSqlServer.TabIndex = 1;
            this.TabSqlServer.Text = "SQL Server";
            this.TabSqlServer.UseVisualStyleBackColor = true;
            this.TabSqlServer.Visible = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbSqlConnType);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.tbConnectionName);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(5, 14);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(336, 83);
            this.groupBox3.TabIndex = 999;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Connection";
            // 
            // cbSqlConnType
            // 
            this.cbSqlConnType.FormattingEnabled = true;
            this.cbSqlConnType.Items.AddRange(new object[] {
            "ATTRIBUTE",
            "ASSET",
            "NETWORK"});
            this.cbSqlConnType.Location = new System.Drawing.Point(127, 50);
            this.cbSqlConnType.Name = "cbSqlConnType";
            this.cbSqlConnType.Size = new System.Drawing.Size(203, 21);
            this.cbSqlConnType.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(24, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 16);
            this.label6.TabIndex = 999;
            this.label6.Text = "Type:";
            // 
            // tbConnectionName
            // 
            this.tbConnectionName.Location = new System.Drawing.Point(127, 21);
            this.tbConnectionName.Name = "tbConnectionName";
            this.tbConnectionName.Size = new System.Drawing.Size(203, 20);
            this.tbConnectionName.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(24, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 16);
            this.label3.TabIndex = 999;
            this.label3.Text = "Name:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSqlProviderString);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 103);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(336, 80);
            this.groupBox1.TabIndex = 999;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Provider";
            // 
            // txtSqlProviderString
            // 
            this.txtSqlProviderString.Location = new System.Drawing.Point(40, 43);
            this.txtSqlProviderString.Name = "txtSqlProviderString";
            this.txtSqlProviderString.Size = new System.Drawing.Size(248, 20);
            this.txtSqlProviderString.TabIndex = 3;
            this.txtSqlProviderString.Text = "SQLOLEDB";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(24, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 999;
            this.label1.Text = "Provider String: ";
            // 
            // gbxSqlServer5
            // 
            this.gbxSqlServer5.Controls.Add(this.btnSQLserverCancel);
            this.gbxSqlServer5.Controls.Add(this.btnSqlServerTest);
            this.gbxSqlServer5.Controls.Add(this.btnSqlServerOK);
            this.gbxSqlServer5.Location = new System.Drawing.Point(6, 471);
            this.gbxSqlServer5.Name = "gbxSqlServer5";
            this.gbxSqlServer5.Size = new System.Drawing.Size(336, 48);
            this.gbxSqlServer5.TabIndex = 999;
            this.gbxSqlServer5.TabStop = false;
            // 
            // btnSQLserverCancel
            // 
            this.btnSQLserverCancel.Location = new System.Drawing.Point(16, 16);
            this.btnSQLserverCancel.Name = "btnSQLserverCancel";
            this.btnSQLserverCancel.Size = new System.Drawing.Size(80, 23);
            this.btnSQLserverCancel.TabIndex = 15;
            this.btnSQLserverCancel.Text = "Cancel";
            this.btnSQLserverCancel.Click += new System.EventHandler(this.btnSQLserverCancel_Click);
            // 
            // btnSqlServerTest
            // 
            this.btnSqlServerTest.Location = new System.Drawing.Point(126, 16);
            this.btnSqlServerTest.Name = "btnSqlServerTest";
            this.btnSqlServerTest.Size = new System.Drawing.Size(75, 23);
            this.btnSqlServerTest.TabIndex = 16;
            this.btnSqlServerTest.Text = "Test";
            this.btnSqlServerTest.Click += new System.EventHandler(this.btnSqlServerTest_Click);
            // 
            // btnSqlServerOK
            // 
            this.btnSqlServerOK.Location = new System.Drawing.Point(231, 16);
            this.btnSqlServerOK.Name = "btnSqlServerOK";
            this.btnSqlServerOK.Size = new System.Drawing.Size(75, 23);
            this.btnSqlServerOK.TabIndex = 17;
            this.btnSqlServerOK.Text = "OK";
            this.btnSqlServerOK.Click += new System.EventHandler(this.btnSqlServerOK_Click);
            // 
            // gbxSqlServer4
            // 
            this.gbxSqlServer4.Controls.Add(this.cbxIntegratedSecurity);
            this.gbxSqlServer4.Controls.Add(this.txtSqlServerPassword);
            this.gbxSqlServer4.Controls.Add(this.txtSqlServerUserID);
            this.gbxSqlServer4.Controls.Add(this.lblSqlServerPassword);
            this.gbxSqlServer4.Controls.Add(this.lblSqlServerUserID);
            this.gbxSqlServer4.Location = new System.Drawing.Point(6, 361);
            this.gbxSqlServer4.Name = "gbxSqlServer4";
            this.gbxSqlServer4.Size = new System.Drawing.Size(336, 104);
            this.gbxSqlServer4.TabIndex = 999;
            this.gbxSqlServer4.TabStop = false;
            this.gbxSqlServer4.Text = "User Credentials";
            // 
            // cbxIntegratedSecurity
            // 
            this.cbxIntegratedSecurity.AutoSize = true;
            this.cbxIntegratedSecurity.Location = new System.Drawing.Point(88, 81);
            this.cbxIntegratedSecurity.Name = "cbxIntegratedSecurity";
            this.cbxIntegratedSecurity.Size = new System.Drawing.Size(137, 17);
            this.cbxIntegratedSecurity.TabIndex = 999;
            this.cbxIntegratedSecurity.Text = "Use Integrated Security";
            this.cbxIntegratedSecurity.UseVisualStyleBackColor = true;
            this.cbxIntegratedSecurity.CheckedChanged += new System.EventHandler(this.cbxIntegratedSecurity_CheckedChanged);
            // 
            // txtSqlServerPassword
            // 
            this.txtSqlServerPassword.Location = new System.Drawing.Point(88, 51);
            this.txtSqlServerPassword.Name = "txtSqlServerPassword";
            this.txtSqlServerPassword.PasswordChar = '*';
            this.txtSqlServerPassword.Size = new System.Drawing.Size(200, 20);
            this.txtSqlServerPassword.TabIndex = 7;
            // 
            // txtSqlServerUserID
            // 
            this.txtSqlServerUserID.Location = new System.Drawing.Point(88, 22);
            this.txtSqlServerUserID.Name = "txtSqlServerUserID";
            this.txtSqlServerUserID.Size = new System.Drawing.Size(200, 20);
            this.txtSqlServerUserID.TabIndex = 6;
            // 
            // lblSqlServerPassword
            // 
            this.lblSqlServerPassword.Location = new System.Drawing.Point(23, 49);
            this.lblSqlServerPassword.Name = "lblSqlServerPassword";
            this.lblSqlServerPassword.Size = new System.Drawing.Size(64, 23);
            this.lblSqlServerPassword.TabIndex = 999;
            this.lblSqlServerPassword.Text = "Password:";
            this.lblSqlServerPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSqlServerUserID
            // 
            this.lblSqlServerUserID.Location = new System.Drawing.Point(23, 21);
            this.lblSqlServerUserID.Name = "lblSqlServerUserID";
            this.lblSqlServerUserID.Size = new System.Drawing.Size(64, 23);
            this.lblSqlServerUserID.TabIndex = 999;
            this.lblSqlServerUserID.Text = "User ID:";
            this.lblSqlServerUserID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gbxSqlServer2
            // 
            this.gbxSqlServer2.Controls.Add(this.txtSqlServerName);
            this.gbxSqlServer2.Controls.Add(this.lblSqlServerName);
            this.gbxSqlServer2.Location = new System.Drawing.Point(6, 189);
            this.gbxSqlServer2.Name = "gbxSqlServer2";
            this.gbxSqlServer2.Size = new System.Drawing.Size(336, 80);
            this.gbxSqlServer2.TabIndex = 999;
            this.gbxSqlServer2.TabStop = false;
            this.gbxSqlServer2.Text = "Data Server";
            // 
            // txtSqlServerName
            // 
            this.txtSqlServerName.Location = new System.Drawing.Point(40, 43);
            this.txtSqlServerName.Name = "txtSqlServerName";
            this.txtSqlServerName.Size = new System.Drawing.Size(248, 20);
            this.txtSqlServerName.TabIndex = 4;
            // 
            // lblSqlServerName
            // 
            this.lblSqlServerName.Location = new System.Drawing.Point(24, 24);
            this.lblSqlServerName.Name = "lblSqlServerName";
            this.lblSqlServerName.Size = new System.Drawing.Size(80, 16);
            this.lblSqlServerName.TabIndex = 999;
            this.lblSqlServerName.Text = "Server Name: ";
            // 
            // btnOracleCancel
            // 
            this.btnOracleCancel.Location = new System.Drawing.Point(16, 16);
            this.btnOracleCancel.Name = "btnOracleCancel";
            this.btnOracleCancel.Size = new System.Drawing.Size(80, 23);
            this.btnOracleCancel.TabIndex = 15;
            this.btnOracleCancel.Text = "Cancel";
            this.btnOracleCancel.Click += new System.EventHandler(this.btnOracleCancel_Click);
            // 
            // btnOracleTest
            // 
            this.btnOracleTest.Location = new System.Drawing.Point(126, 16);
            this.btnOracleTest.Name = "btnOracleTest";
            this.btnOracleTest.Size = new System.Drawing.Size(75, 23);
            this.btnOracleTest.TabIndex = 16;
            this.btnOracleTest.Text = "Test";
            this.btnOracleTest.Click += new System.EventHandler(this.btnOracleTest_Click);
            // 
            // gbxOracle4
            // 
            this.gbxOracle4.Controls.Add(this.btnOracleCancel);
            this.gbxOracle4.Controls.Add(this.btnOracleTest);
            this.gbxOracle4.Controls.Add(this.btnOracleOK);
            this.gbxOracle4.Location = new System.Drawing.Point(6, 472);
            this.gbxOracle4.Name = "gbxOracle4";
            this.gbxOracle4.Size = new System.Drawing.Size(336, 48);
            this.gbxOracle4.TabIndex = 999;
            this.gbxOracle4.TabStop = false;
            // 
            // btnOracleOK
            // 
            this.btnOracleOK.Location = new System.Drawing.Point(231, 16);
            this.btnOracleOK.Name = "btnOracleOK";
            this.btnOracleOK.Size = new System.Drawing.Size(75, 23);
            this.btnOracleOK.TabIndex = 17;
            this.btnOracleOK.Text = "OK";
            this.btnOracleOK.Click += new System.EventHandler(this.btnOracleOK_Click);
            // 
            // chkUseIntegratedSecurity
            // 
            this.chkUseIntegratedSecurity.AutoSize = true;
            this.chkUseIntegratedSecurity.Checked = true;
            this.chkUseIntegratedSecurity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseIntegratedSecurity.Location = new System.Drawing.Point(104, 101);
            this.chkUseIntegratedSecurity.Name = "chkUseIntegratedSecurity";
            this.chkUseIntegratedSecurity.Size = new System.Drawing.Size(137, 17);
            this.chkUseIntegratedSecurity.TabIndex = 9;
            this.chkUseIntegratedSecurity.Text = "Use Integrated Security";
            this.chkUseIntegratedSecurity.UseVisualStyleBackColor = true;
            this.chkUseIntegratedSecurity.CheckedChanged += new System.EventHandler(this.chkUseIntegratedSecurity_CheckedChanged);
            // 
            // txtOraclePassword
            // 
            this.txtOraclePassword.Location = new System.Drawing.Point(88, 64);
            this.txtOraclePassword.Name = "txtOraclePassword";
            this.txtOraclePassword.PasswordChar = '*';
            this.txtOraclePassword.ReadOnly = true;
            this.txtOraclePassword.Size = new System.Drawing.Size(200, 20);
            this.txtOraclePassword.TabIndex = 8;
            // 
            // txtOracleUserID
            // 
            this.txtOracleUserID.Location = new System.Drawing.Point(88, 32);
            this.txtOracleUserID.Name = "txtOracleUserID";
            this.txtOracleUserID.ReadOnly = true;
            this.txtOracleUserID.Size = new System.Drawing.Size(200, 20);
            this.txtOracleUserID.TabIndex = 7;
            // 
            // gbxOracle3
            // 
            this.gbxOracle3.Controls.Add(this.txtOraclePassword);
            this.gbxOracle3.Controls.Add(this.txtOracleUserID);
            this.gbxOracle3.Controls.Add(this.chkUseIntegratedSecurity);
            this.gbxOracle3.Controls.Add(this.lblOraclePassword);
            this.gbxOracle3.Controls.Add(this.lblOracleUserID);
            this.gbxOracle3.Location = new System.Drawing.Point(6, 342);
            this.gbxOracle3.Name = "gbxOracle3";
            this.gbxOracle3.Size = new System.Drawing.Size(336, 124);
            this.gbxOracle3.TabIndex = 999;
            this.gbxOracle3.TabStop = false;
            this.gbxOracle3.Text = "User Credentials";
            // 
            // lblOraclePassword
            // 
            this.lblOraclePassword.Location = new System.Drawing.Point(16, 61);
            this.lblOraclePassword.Name = "lblOraclePassword";
            this.lblOraclePassword.Size = new System.Drawing.Size(64, 23);
            this.lblOraclePassword.TabIndex = 999;
            this.lblOraclePassword.Text = "Password:";
            this.lblOraclePassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblOracleUserID
            // 
            this.lblOracleUserID.Location = new System.Drawing.Point(16, 32);
            this.lblOracleUserID.Name = "lblOracleUserID";
            this.lblOracleUserID.Size = new System.Drawing.Size(64, 23);
            this.lblOracleUserID.TabIndex = 999;
            this.lblOracleUserID.Text = "User ID:";
            this.lblOracleUserID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.TabSqlServer);
            this.TabControl1.Controls.Add(this.tabOracle);
            this.TabControl1.Location = new System.Drawing.Point(3, 0);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(360, 554);
            this.TabControl1.TabIndex = 999;
            this.TabControl1.TabStop = false;
            // 
            // tabOracle
            // 
            this.tabOracle.Controls.Add(this.groupBox4);
            this.tabOracle.Controls.Add(this.groupBox2);
            this.tabOracle.Controls.Add(this.gbxOracle2);
            this.tabOracle.Controls.Add(this.gbxOracle4);
            this.tabOracle.Controls.Add(this.gbxOracle3);
            this.tabOracle.Location = new System.Drawing.Point(4, 22);
            this.tabOracle.Name = "tabOracle";
            this.tabOracle.Size = new System.Drawing.Size(352, 528);
            this.tabOracle.TabIndex = 0;
            this.tabOracle.Text = "Oracle";
            this.tabOracle.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbOracleConnType);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.tbOracleConnectionName);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Location = new System.Drawing.Point(6, 13);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(336, 86);
            this.groupBox4.TabIndex = 999;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Connection";
            // 
            // cbOracleConnType
            // 
            this.cbOracleConnType.FormattingEnabled = true;
            this.cbOracleConnType.Items.AddRange(new object[] {
            "ATTRIBUTE",
            "ASSET",
            "NETWORK"});
            this.cbOracleConnType.Location = new System.Drawing.Point(127, 50);
            this.cbOracleConnType.Name = "cbOracleConnType";
            this.cbOracleConnType.Size = new System.Drawing.Size(203, 21);
            this.cbOracleConnType.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(24, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(97, 16);
            this.label7.TabIndex = 998;
            this.label7.Text = "Type:";
            // 
            // tbOracleConnectionName
            // 
            this.tbOracleConnectionName.Location = new System.Drawing.Point(126, 21);
            this.tbOracleConnectionName.Name = "tbOracleConnectionName";
            this.tbOracleConnectionName.Size = new System.Drawing.Size(204, 20);
            this.tbOracleConnectionName.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(24, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 16);
            this.label5.TabIndex = 999;
            this.label5.Text = "Name:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbOracleProvider);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(6, 105);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(336, 80);
            this.groupBox2.TabIndex = 999;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Provider";
            // 
            // tbOracleProvider
            // 
            this.tbOracleProvider.Location = new System.Drawing.Point(40, 43);
            this.tbOracleProvider.Name = "tbOracleProvider";
            this.tbOracleProvider.ReadOnly = true;
            this.tbOracleProvider.Size = new System.Drawing.Size(248, 20);
            this.tbOracleProvider.TabIndex = 996;
            this.tbOracleProvider.Text = "OraOLEDB.Oracle";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(24, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 16);
            this.label2.TabIndex = 997;
            this.label2.Text = "Provider String: ";
            // 
            // gbxOracle2
            // 
            this.gbxOracle2.Controls.Add(this.rbNetworkAlias);
            this.gbxOracle2.Controls.Add(this.tbNetworkAlias);
            this.gbxOracle2.Controls.Add(this.tbPort);
            this.gbxOracle2.Controls.Add(this.label11);
            this.gbxOracle2.Controls.Add(this.tbOracleServerName);
            this.gbxOracle2.Controls.Add(this.label17);
            this.gbxOracle2.Controls.Add(this.radioButton1);
            this.gbxOracle2.Controls.Add(this.tbSID);
            this.gbxOracle2.Location = new System.Drawing.Point(6, 191);
            this.gbxOracle2.Name = "gbxOracle2";
            this.gbxOracle2.Size = new System.Drawing.Size(336, 145);
            this.gbxOracle2.TabIndex = 999;
            this.gbxOracle2.TabStop = false;
            this.gbxOracle2.Text = "Oracle Instance";
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(104, 97);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(217, 20);
            this.tbPort.TabIndex = 1001;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(6, 100);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(48, 16);
            this.label11.TabIndex = 1003;
            this.label11.Text = "Port:";
            // 
            // tbOracleServerName
            // 
            this.tbOracleServerName.Location = new System.Drawing.Point(104, 73);
            this.tbOracleServerName.Name = "tbOracleServerName";
            this.tbOracleServerName.Size = new System.Drawing.Size(217, 20);
            this.tbOracleServerName.TabIndex = 1000;
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(6, 76);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(83, 16);
            this.label17.TabIndex = 1002;
            this.label17.Text = "Hostname:";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(7, 47);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(46, 17);
            this.radioButton1.TabIndex = 8;
            this.radioButton1.Text = "SID:";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // tbSID
            // 
            this.tbSID.Location = new System.Drawing.Point(104, 47);
            this.tbSID.Name = "tbSID";
            this.tbSID.Size = new System.Drawing.Size(217, 20);
            this.tbSID.TabIndex = 5;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(40, 43);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(248, 20);
            this.textBox1.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(24, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Connection Name:";
            // 
            // rbNetworkAlias
            // 
            this.rbNetworkAlias.AutoSize = true;
            this.rbNetworkAlias.Location = new System.Drawing.Point(6, 24);
            this.rbNetworkAlias.Name = "rbNetworkAlias";
            this.rbNetworkAlias.Size = new System.Drawing.Size(93, 17);
            this.rbNetworkAlias.TabIndex = 1005;
            this.rbNetworkAlias.Text = "Network Alias:";
            this.rbNetworkAlias.UseVisualStyleBackColor = true;
            // 
            // tbNetworkAlias
            // 
            this.tbNetworkAlias.Location = new System.Drawing.Point(104, 23);
            this.tbNetworkAlias.Name = "tbNetworkAlias";
            this.tbNetworkAlias.Size = new System.Drawing.Size(217, 20);
            this.tbNetworkAlias.TabIndex = 1004;
            // 
            // frmConnect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 554);
            this.Controls.Add(this.TabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmConnect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connection";
            this.gbxSqlServer3.ResumeLayout(false);
            this.gbxSqlServer3.PerformLayout();
            this.TabSqlServer.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbxSqlServer5.ResumeLayout(false);
            this.gbxSqlServer4.ResumeLayout(false);
            this.gbxSqlServer4.PerformLayout();
            this.gbxSqlServer2.ResumeLayout(false);
            this.gbxSqlServer2.PerformLayout();
            this.gbxOracle4.ResumeLayout(false);
            this.gbxOracle3.ResumeLayout(false);
            this.gbxOracle3.PerformLayout();
            this.TabControl1.ResumeLayout(false);
            this.tabOracle.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbxOracle2.ResumeLayout(false);
            this.gbxOracle2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox gbxSqlServer3;
        internal System.Windows.Forms.TextBox txtSqlServerInitialCat;
        internal System.Windows.Forms.Label lblSqlServerInitialCat;
		internal System.Windows.Forms.TabPage TabSqlServer;
        internal System.Windows.Forms.GroupBox gbxSqlServer5;
        internal System.Windows.Forms.Button btnSQLserverCancel;
        internal System.Windows.Forms.Button btnSqlServerTest;
        internal System.Windows.Forms.Button btnSqlServerOK;
        internal System.Windows.Forms.GroupBox gbxSqlServer4;
        internal System.Windows.Forms.TextBox txtSqlServerPassword;
        internal System.Windows.Forms.TextBox txtSqlServerUserID;
        internal System.Windows.Forms.Label lblSqlServerPassword;
        internal System.Windows.Forms.Label lblSqlServerUserID;
        internal System.Windows.Forms.GroupBox gbxSqlServer2;
        internal System.Windows.Forms.TextBox txtSqlServerName;
		internal System.Windows.Forms.Label lblSqlServerName;
        internal System.Windows.Forms.Button btnOracleCancel;
        internal System.Windows.Forms.Button btnOracleTest;
        internal System.Windows.Forms.GroupBox gbxOracle4;
        internal System.Windows.Forms.Button btnOracleOK;
        internal System.Windows.Forms.TextBox txtOraclePassword;
        internal System.Windows.Forms.TextBox txtOracleUserID;
        internal System.Windows.Forms.GroupBox gbxOracle3;
        internal System.Windows.Forms.Label lblOraclePassword;
		internal System.Windows.Forms.Label lblOracleUserID;
        internal System.Windows.Forms.TabControl TabControl1;
		internal System.Windows.Forms.TabPage tabOracle;
		private System.Windows.Forms.CheckBox cbxIntegratedSecurity;
		private System.Windows.Forms.CheckBox chkUseIntegratedSecurity;
        internal System.Windows.Forms.GroupBox gbxOracle2;
        private System.Windows.Forms.RadioButton radioButton1;
		internal System.Windows.Forms.TextBox tbSID;
		internal System.Windows.Forms.GroupBox groupBox1;
		internal System.Windows.Forms.TextBox txtSqlProviderString;
		internal System.Windows.Forms.Label label1;
		internal System.Windows.Forms.GroupBox groupBox2;
		internal System.Windows.Forms.TextBox tbOracleProvider;
		internal System.Windows.Forms.Label label2;
		internal System.Windows.Forms.GroupBox groupBox3;
		internal System.Windows.Forms.TextBox tbConnectionName;
		internal System.Windows.Forms.Label label3;
		internal System.Windows.Forms.GroupBox groupBox4;
		internal System.Windows.Forms.TextBox tbOracleConnectionName;
		internal System.Windows.Forms.Label label5;
		internal System.Windows.Forms.TextBox textBox1;
		internal System.Windows.Forms.Label label4;
		internal System.Windows.Forms.Label label6;
		internal System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox cbSqlConnType;
		private System.Windows.Forms.ComboBox cbOracleConnType;
        internal System.Windows.Forms.TextBox tbPort;
        internal System.Windows.Forms.Label label11;
        internal System.Windows.Forms.TextBox tbOracleServerName;
        internal System.Windows.Forms.Label label17;
        private System.Windows.Forms.RadioButton rbNetworkAlias;
        internal System.Windows.Forms.TextBox tbNetworkAlias;
    }
}

