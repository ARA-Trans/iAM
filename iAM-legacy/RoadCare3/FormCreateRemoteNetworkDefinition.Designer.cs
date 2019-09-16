namespace RoadCare3
{
    partial class FormCreateRemoteNetworkDefinition
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
            this.tabOracle = new System.Windows.Forms.TabPage();
            this.gbxOracle2 = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.tbOracleServerName = new System.Windows.Forms.TextBox();
            this.tbSID = new System.Windows.Forms.TextBox();
            this.tbNetworkAlias = new System.Windows.Forms.TextBox();
            this.rbSID = new System.Windows.Forms.RadioButton();
            this.rbNetworkAlias = new System.Windows.Forms.RadioButton();
            this.gbxOracle3 = new System.Windows.Forms.GroupBox();
            this.lblOracleUserID = new System.Windows.Forms.Label();
            this.lblOraclePassword = new System.Windows.Forms.Label();
            this.txtOraclePassword = new System.Windows.Forms.TextBox();
            this.txtOracleUserID = new System.Windows.Forms.TextBox();
            this.TabSqlServer = new System.Windows.Forms.TabPage();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tbMSSQLPassword = new System.Windows.Forms.TextBox();
            this.tbMSSQLUserName = new System.Windows.Forms.TextBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbMSSQLServerName = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbMSSQLDatabaseName = new System.Windows.Forms.TextBox();
            this.tcLogin = new System.Windows.Forms.TabControl();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.chkUseIntegratedSecurity = new System.Windows.Forms.CheckBox();
            this.textBoxConnName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBoxSQL = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tabOracle.SuspendLayout();
            this.gbxOracle2.SuspendLayout();
            this.gbxOracle3.SuspendLayout();
            this.TabSqlServer.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tcLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabOracle
            // 
            this.tabOracle.Controls.Add(this.gbxOracle3);
            this.tabOracle.Controls.Add(this.gbxOracle2);
            this.tabOracle.Location = new System.Drawing.Point(4, 22);
            this.tabOracle.Name = "tabOracle";
            this.tabOracle.Size = new System.Drawing.Size(353, 295);
            this.tabOracle.TabIndex = 0;
            this.tabOracle.Text = "Oracle";
            this.tabOracle.UseVisualStyleBackColor = true;
            // 
            // gbxOracle2
            // 
            this.gbxOracle2.Controls.Add(this.rbNetworkAlias);
            this.gbxOracle2.Controls.Add(this.rbSID);
            this.gbxOracle2.Controls.Add(this.tbNetworkAlias);
            this.gbxOracle2.Controls.Add(this.tbSID);
            this.gbxOracle2.Controls.Add(this.tbOracleServerName);
            this.gbxOracle2.Controls.Add(this.tbPort);
            this.gbxOracle2.Controls.Add(this.label11);
            this.gbxOracle2.Controls.Add(this.label17);
            this.gbxOracle2.Location = new System.Drawing.Point(11, 13);
            this.gbxOracle2.Name = "gbxOracle2";
            this.gbxOracle2.Size = new System.Drawing.Size(336, 145);
            this.gbxOracle2.TabIndex = 1;
            this.gbxOracle2.TabStop = false;
            this.gbxOracle2.Text = "Oracle Instance";
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(6, 80);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(83, 16);
            this.label17.TabIndex = 5;
            this.label17.Text = "Hostname:";
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(9, 104);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(48, 16);
            this.label11.TabIndex = 7;
            this.label11.Text = "Port:";
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(107, 101);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(217, 20);
            this.tbPort.TabIndex = 4;
            // 
            // tbOracleServerName
            // 
            this.tbOracleServerName.Location = new System.Drawing.Point(107, 77);
            this.tbOracleServerName.Name = "tbOracleServerName";
            this.tbOracleServerName.Size = new System.Drawing.Size(217, 20);
            this.tbOracleServerName.TabIndex = 3;
            // 
            // tbSID
            // 
            this.tbSID.Location = new System.Drawing.Point(107, 47);
            this.tbSID.Name = "tbSID";
            this.tbSID.Size = new System.Drawing.Size(217, 20);
            this.tbSID.TabIndex = 2;
            // 
            // tbNetworkAlias
            // 
            this.tbNetworkAlias.Location = new System.Drawing.Point(107, 18);
            this.tbNetworkAlias.Name = "tbNetworkAlias";
            this.tbNetworkAlias.Size = new System.Drawing.Size(217, 20);
            this.tbNetworkAlias.TabIndex = 1;
            // 
            // rbSID
            // 
            this.rbSID.AutoSize = true;
            this.rbSID.Location = new System.Drawing.Point(10, 47);
            this.rbSID.Name = "rbSID";
            this.rbSID.Size = new System.Drawing.Size(46, 17);
            this.rbSID.TabIndex = 8;
            this.rbSID.Text = "SID:";
            this.rbSID.UseVisualStyleBackColor = true;
            // 
            // rbNetworkAlias
            // 
            this.rbNetworkAlias.AutoSize = true;
            this.rbNetworkAlias.Location = new System.Drawing.Point(9, 19);
            this.rbNetworkAlias.Name = "rbNetworkAlias";
            this.rbNetworkAlias.Size = new System.Drawing.Size(93, 17);
            this.rbNetworkAlias.TabIndex = 9;
            this.rbNetworkAlias.Text = "Network Alias:";
            this.rbNetworkAlias.UseVisualStyleBackColor = true;
            // 
            // gbxOracle3
            // 
            this.gbxOracle3.Controls.Add(this.txtOracleUserID);
            this.gbxOracle3.Controls.Add(this.txtOraclePassword);
            this.gbxOracle3.Controls.Add(this.lblOraclePassword);
            this.gbxOracle3.Controls.Add(this.lblOracleUserID);
            this.gbxOracle3.Location = new System.Drawing.Point(14, 176);
            this.gbxOracle3.Name = "gbxOracle3";
            this.gbxOracle3.Size = new System.Drawing.Size(336, 104);
            this.gbxOracle3.TabIndex = 2;
            this.gbxOracle3.TabStop = false;
            this.gbxOracle3.Text = "User Credentials";
            // 
            // lblOracleUserID
            // 
            this.lblOracleUserID.Location = new System.Drawing.Point(16, 32);
            this.lblOracleUserID.Name = "lblOracleUserID";
            this.lblOracleUserID.Size = new System.Drawing.Size(64, 23);
            this.lblOracleUserID.TabIndex = 0;
            this.lblOracleUserID.Text = "User ID:";
            this.lblOracleUserID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblOraclePassword
            // 
            this.lblOraclePassword.Location = new System.Drawing.Point(16, 61);
            this.lblOraclePassword.Name = "lblOraclePassword";
            this.lblOraclePassword.Size = new System.Drawing.Size(64, 23);
            this.lblOraclePassword.TabIndex = 1;
            this.lblOraclePassword.Text = "Password:";
            this.lblOraclePassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOraclePassword
            // 
            this.txtOraclePassword.Location = new System.Drawing.Point(88, 64);
            this.txtOraclePassword.Name = "txtOraclePassword";
            this.txtOraclePassword.PasswordChar = '*';
            this.txtOraclePassword.Size = new System.Drawing.Size(200, 20);
            this.txtOraclePassword.TabIndex = 6;
            // 
            // txtOracleUserID
            // 
            this.txtOracleUserID.Location = new System.Drawing.Point(88, 32);
            this.txtOracleUserID.Name = "txtOracleUserID";
            this.txtOracleUserID.Size = new System.Drawing.Size(200, 20);
            this.txtOracleUserID.TabIndex = 5;
            // 
            // TabSqlServer
            // 
            this.TabSqlServer.Controls.Add(this.groupBox6);
            this.TabSqlServer.Controls.Add(this.groupBox10);
            this.TabSqlServer.Controls.Add(this.groupBox9);
            this.TabSqlServer.Location = new System.Drawing.Point(4, 22);
            this.TabSqlServer.Name = "TabSqlServer";
            this.TabSqlServer.Size = new System.Drawing.Size(353, 295);
            this.TabSqlServer.TabIndex = 1;
            this.TabSqlServer.Text = "MSSQL Server";
            this.TabSqlServer.UseVisualStyleBackColor = true;
            this.TabSqlServer.Visible = false;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.tbMSSQLUserName);
            this.groupBox9.Controls.Add(this.tbMSSQLPassword);
            this.groupBox9.Controls.Add(this.label8);
            this.groupBox9.Controls.Add(this.label9);
            this.groupBox9.Location = new System.Drawing.Point(11, 186);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(336, 104);
            this.groupBox9.TabIndex = 5;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "User Credentials";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(23, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 23);
            this.label9.TabIndex = 993;
            this.label9.Text = "User Name:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(23, 49);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(64, 23);
            this.label8.TabIndex = 992;
            this.label8.Text = "Password:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbMSSQLPassword
            // 
            this.tbMSSQLPassword.Location = new System.Drawing.Point(88, 51);
            this.tbMSSQLPassword.Name = "tbMSSQLPassword";
            this.tbMSSQLPassword.PasswordChar = '*';
            this.tbMSSQLPassword.Size = new System.Drawing.Size(200, 20);
            this.tbMSSQLPassword.TabIndex = 11;
            // 
            // tbMSSQLUserName
            // 
            this.tbMSSQLUserName.Location = new System.Drawing.Point(88, 22);
            this.tbMSSQLUserName.Name = "tbMSSQLUserName";
            this.tbMSSQLUserName.Size = new System.Drawing.Size(200, 20);
            this.tbMSSQLUserName.TabIndex = 10;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.tbMSSQLServerName);
            this.groupBox10.Controls.Add(this.label10);
            this.groupBox10.Location = new System.Drawing.Point(11, 12);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(336, 80);
            this.groupBox10.TabIndex = 3;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Data Server";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(24, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 16);
            this.label10.TabIndex = 996;
            this.label10.Text = "Server Name: ";
            // 
            // tbMSSQLServerName
            // 
            this.tbMSSQLServerName.Location = new System.Drawing.Point(40, 43);
            this.tbMSSQLServerName.Name = "tbMSSQLServerName";
            this.tbMSSQLServerName.Size = new System.Drawing.Size(248, 20);
            this.tbMSSQLServerName.TabIndex = 8;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.tbMSSQLDatabaseName);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Location = new System.Drawing.Point(11, 100);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(336, 80);
            this.groupBox6.TabIndex = 4;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Database";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(24, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(176, 16);
            this.label6.TabIndex = 995;
            this.label6.Text = "MSSQL Database Name:";
            // 
            // tbMSSQLDatabaseName
            // 
            this.tbMSSQLDatabaseName.Location = new System.Drawing.Point(40, 43);
            this.tbMSSQLDatabaseName.Name = "tbMSSQLDatabaseName";
            this.tbMSSQLDatabaseName.Size = new System.Drawing.Size(248, 20);
            this.tbMSSQLDatabaseName.TabIndex = 9;
            // 
            // tcLogin
            // 
            this.tcLogin.Controls.Add(this.TabSqlServer);
            this.tcLogin.Controls.Add(this.tabOracle);
            this.tcLogin.Location = new System.Drawing.Point(12, 50);
            this.tcLogin.Name = "tcLogin";
            this.tcLogin.SelectedIndex = 0;
            this.tcLogin.Size = new System.Drawing.Size(361, 321);
            this.tcLogin.TabIndex = 3;
            this.tcLogin.TabStop = false;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(678, 375);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(597, 375);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // chkUseIntegratedSecurity
            // 
            this.chkUseIntegratedSecurity.AutoSize = true;
            this.chkUseIntegratedSecurity.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkUseIntegratedSecurity.Location = new System.Drawing.Point(12, 373);
            this.chkUseIntegratedSecurity.Name = "chkUseIntegratedSecurity";
            this.chkUseIntegratedSecurity.Size = new System.Drawing.Size(137, 17);
            this.chkUseIntegratedSecurity.TabIndex = 7;
            this.chkUseIntegratedSecurity.TabStop = false;
            this.chkUseIntegratedSecurity.Text = "Use Integrated Security";
            this.chkUseIntegratedSecurity.UseVisualStyleBackColor = true;
            this.chkUseIntegratedSecurity.CheckedChanged += new System.EventHandler(this.chkUseIntegratedSecurity_CheckedChanged);
            // 
            // textBoxConnName
            // 
            this.textBoxConnName.Location = new System.Drawing.Point(116, 12);
            this.textBoxConnName.Name = "textBoxConnName";
            this.textBoxConnName.Size = new System.Drawing.Size(235, 20);
            this.textBoxConnName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 23);
            this.label1.TabIndex = 12;
            this.label1.Text = "Connection Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // richTextBoxSQL
            // 
            this.richTextBoxSQL.Location = new System.Drawing.Point(379, 68);
            this.richTextBoxSQL.Name = "richTextBoxSQL";
            this.richTextBoxSQL.Size = new System.Drawing.Size(374, 303);
            this.richTextBoxSQL.TabIndex = 13;
            this.richTextBoxSQL.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(379, 39);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "Test SQL";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormCreateRemoteNetworkDefinition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 410);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBoxSQL);
            this.Controls.Add(this.textBoxConnName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkUseIntegratedSecurity);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tcLogin);
            this.Name = "FormCreateRemoteNetworkDefinition";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Remote Network Definition";
            this.tabOracle.ResumeLayout(false);
            this.gbxOracle2.ResumeLayout(false);
            this.gbxOracle2.PerformLayout();
            this.gbxOracle3.ResumeLayout(false);
            this.gbxOracle3.PerformLayout();
            this.TabSqlServer.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tcLogin.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TabPage tabOracle;
        internal System.Windows.Forms.GroupBox gbxOracle3;
        internal System.Windows.Forms.TextBox txtOracleUserID;
        internal System.Windows.Forms.TextBox txtOraclePassword;
        internal System.Windows.Forms.Label lblOraclePassword;
        internal System.Windows.Forms.Label lblOracleUserID;
        internal System.Windows.Forms.GroupBox gbxOracle2;
        private System.Windows.Forms.RadioButton rbNetworkAlias;
        private System.Windows.Forms.RadioButton rbSID;
        internal System.Windows.Forms.TextBox tbNetworkAlias;
        internal System.Windows.Forms.TextBox tbSID;
        internal System.Windows.Forms.TextBox tbOracleServerName;
        internal System.Windows.Forms.TextBox tbPort;
        internal System.Windows.Forms.Label label11;
        internal System.Windows.Forms.Label label17;
        internal System.Windows.Forms.TabPage TabSqlServer;
        internal System.Windows.Forms.GroupBox groupBox6;
        internal System.Windows.Forms.TextBox tbMSSQLDatabaseName;
        internal System.Windows.Forms.Label label6;
        internal System.Windows.Forms.GroupBox groupBox10;
        internal System.Windows.Forms.TextBox tbMSSQLServerName;
        internal System.Windows.Forms.Label label10;
        internal System.Windows.Forms.GroupBox groupBox9;
        internal System.Windows.Forms.TextBox tbMSSQLUserName;
        internal System.Windows.Forms.TextBox tbMSSQLPassword;
        internal System.Windows.Forms.Label label8;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.TabControl tcLogin;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox chkUseIntegratedSecurity;
        internal System.Windows.Forms.TextBox textBoxConnName;
        internal System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBoxSQL;
        private System.Windows.Forms.Button button1;
    }
}