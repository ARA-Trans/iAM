namespace RoadCare3
{
    partial class FormLmDmVmtConditionDualReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLmDmVmtConditionDualReport));
            this.lblNetwork = new System.Windows.Forms.Label();
            this.tbCurrentProfile = new System.Windows.Forms.TextBox();
            this.lblCurrentProfile = new System.Windows.Forms.Label();
            this.cbAvailableProfiles = new System.Windows.Forms.ComboBox();
            this.lblAvailableProfiles = new System.Windows.Forms.Label();
            this.btnSaveProfile = new System.Windows.Forms.Button();
            this.chbAsNewProfile = new System.Windows.Forms.CheckBox();
            this.btnLoadProfile = new System.Windows.Forms.Button();
            this.btnDeleteProfile = new System.Windows.Forms.Button();
            this.tcIndexSettings = new System.Windows.Forms.TabControl();
            this.tpAttributeIndexes = new System.Windows.Forms.TabPage();
            this.lblSep5 = new System.Windows.Forms.Label();
            this.lblSep4 = new System.Windows.Forms.Label();
            this.lblSep3 = new System.Windows.Forms.Label();
            this.lblSep2 = new System.Windows.Forms.Label();
            this.lblSep1 = new System.Windows.Forms.Label();
            this.btnGetDefaultBounds = new System.Windows.Forms.Button();
            this.tbIdxExpr5 = new System.Windows.Forms.TextBox();
            this.tbIdxExpr4 = new System.Windows.Forms.TextBox();
            this.tbIdxExpr3 = new System.Windows.Forms.TextBox();
            this.tbIdxExpr2 = new System.Windows.Forms.TextBox();
            this.tbIdxExpr1 = new System.Windows.Forms.TextBox();
            this.tbLevelBound5 = new System.Windows.Forms.TextBox();
            this.tbLevelBound4 = new System.Windows.Forms.TextBox();
            this.tbLevelBound3 = new System.Windows.Forms.TextBox();
            this.tbLevelBound2 = new System.Windows.Forms.TextBox();
            this.tbLevelBound1 = new System.Windows.Forms.TextBox();
            this.lblLevel5 = new System.Windows.Forms.Label();
            this.lblLevel4 = new System.Windows.Forms.Label();
            this.lblLevel3 = new System.Windows.Forms.Label();
            this.lblLevel2 = new System.Windows.Forms.Label();
            this.lblLevel1 = new System.Windows.Forms.Label();
            this.lblIndexExpressions = new System.Windows.Forms.Label();
            this.lblLevelBounds = new System.Windows.Forms.Label();
            this.lblIndexName = new System.Windows.Forms.Label();
            this.tbIndexName = new System.Windows.Forms.TextBox();
            this.lblSourceAttributes = new System.Windows.Forms.Label();
            this.chlbSourceAttributes = new System.Windows.Forms.CheckedListBox();
            this.tpDerivedIndexes = new System.Windows.Forms.TabPage();
            this.lblExpression = new System.Windows.Forms.Label();
            this.tbExpression = new System.Windows.Forms.TextBox();
            this.btnRemoveIndex = new System.Windows.Forms.Button();
            this.lblIndexes = new System.Windows.Forms.Label();
            this.btnAddIndex = new System.Windows.Forms.Button();
            this.chlbDerivedIndexes = new System.Windows.Forms.CheckedListBox();
            this.btnRenameIndex = new System.Windows.Forms.Button();
            this.lblSimulation = new System.Windows.Forms.Label();
            this.tbNetwork = new System.Windows.Forms.TextBox();
            this.tbSimulation = new System.Windows.Forms.TextBox();
            this.gbProfileControl = new System.Windows.Forms.GroupBox();
            this.btnFinish = new System.Windows.Forms.Button();
            this.lblDstDir = new System.Windows.Forms.Label();
            this.btnPickDir = new System.Windows.Forms.Button();
            this.tbDstDir = new System.Windows.Forms.TextBox();
            this.fbdPickDir = new System.Windows.Forms.FolderBrowserDialog();
            this.tcIndexSettings.SuspendLayout();
            this.tpAttributeIndexes.SuspendLayout();
            this.tpDerivedIndexes.SuspendLayout();
            this.gbProfileControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblNetwork
            // 
            this.lblNetwork.AutoSize = true;
            this.lblNetwork.Location = new System.Drawing.Point(19, 14);
            this.lblNetwork.Name = "lblNetwork";
            this.lblNetwork.Size = new System.Drawing.Size(47, 13);
            this.lblNetwork.TabIndex = 0;
            this.lblNetwork.Text = "Network";
            // 
            // tbCurrentProfile
            // 
            this.tbCurrentProfile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCurrentProfile.Location = new System.Drawing.Point(159, 32);
            this.tbCurrentProfile.Name = "tbCurrentProfile";
            this.tbCurrentProfile.ReadOnly = true;
            this.tbCurrentProfile.Size = new System.Drawing.Size(142, 20);
            this.tbCurrentProfile.TabIndex = 0;
            // 
            // lblCurrentProfile
            // 
            this.lblCurrentProfile.AutoSize = true;
            this.lblCurrentProfile.Location = new System.Drawing.Point(156, 15);
            this.lblCurrentProfile.Name = "lblCurrentProfile";
            this.lblCurrentProfile.Size = new System.Drawing.Size(73, 13);
            this.lblCurrentProfile.TabIndex = 0;
            this.lblCurrentProfile.Text = "Current Profile";
            // 
            // cbAvailableProfiles
            // 
            this.cbAvailableProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAvailableProfiles.FormattingEnabled = true;
            this.cbAvailableProfiles.Location = new System.Drawing.Point(9, 31);
            this.cbAvailableProfiles.Name = "cbAvailableProfiles";
            this.cbAvailableProfiles.Size = new System.Drawing.Size(142, 21);
            this.cbAvailableProfiles.Sorted = true;
            this.cbAvailableProfiles.TabIndex = 1;
            // 
            // lblAvailableProfiles
            // 
            this.lblAvailableProfiles.AutoSize = true;
            this.lblAvailableProfiles.Location = new System.Drawing.Point(6, 15);
            this.lblAvailableProfiles.Name = "lblAvailableProfiles";
            this.lblAvailableProfiles.Size = new System.Drawing.Size(75, 13);
            this.lblAvailableProfiles.TabIndex = 0;
            this.lblAvailableProfiles.Text = "Stored Profiles";
            // 
            // btnSaveProfile
            // 
            this.btnSaveProfile.Location = new System.Drawing.Point(158, 56);
            this.btnSaveProfile.Name = "btnSaveProfile";
            this.btnSaveProfile.Size = new System.Drawing.Size(68, 23);
            this.btnSaveProfile.TabIndex = 4;
            this.btnSaveProfile.Text = "Save";
            this.btnSaveProfile.UseVisualStyleBackColor = true;
            this.btnSaveProfile.Click += new System.EventHandler(this.btnSaveProfile_Click);
            // 
            // chbAsNewProfile
            // 
            this.chbAsNewProfile.AutoSize = true;
            this.chbAsNewProfile.Checked = true;
            this.chbAsNewProfile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbAsNewProfile.Location = new System.Drawing.Point(232, 60);
            this.chbAsNewProfile.Name = "chbAsNewProfile";
            this.chbAsNewProfile.Size = new System.Drawing.Size(63, 17);
            this.chbAsNewProfile.TabIndex = 5;
            this.chbAsNewProfile.Text = "As New";
            this.chbAsNewProfile.UseVisualStyleBackColor = true;
            this.chbAsNewProfile.CheckedChanged += new System.EventHandler(this.chbAsNewProfile_CheckedChanged);
            // 
            // btnLoadProfile
            // 
            this.btnLoadProfile.Location = new System.Drawing.Point(83, 56);
            this.btnLoadProfile.Name = "btnLoadProfile";
            this.btnLoadProfile.Size = new System.Drawing.Size(69, 23);
            this.btnLoadProfile.TabIndex = 3;
            this.btnLoadProfile.Text = "Load";
            this.btnLoadProfile.UseVisualStyleBackColor = true;
            this.btnLoadProfile.Click += new System.EventHandler(this.btnLoadProfile_Click);
            // 
            // btnDeleteProfile
            // 
            this.btnDeleteProfile.Location = new System.Drawing.Point(8, 56);
            this.btnDeleteProfile.Name = "btnDeleteProfile";
            this.btnDeleteProfile.Size = new System.Drawing.Size(69, 23);
            this.btnDeleteProfile.TabIndex = 2;
            this.btnDeleteProfile.Text = "Delete";
            this.btnDeleteProfile.UseVisualStyleBackColor = true;
            this.btnDeleteProfile.Click += new System.EventHandler(this.btnDeleteProfile_Click);
            // 
            // tcIndexSettings
            // 
            this.tcIndexSettings.Controls.Add(this.tpAttributeIndexes);
            this.tcIndexSettings.Controls.Add(this.tpDerivedIndexes);
            this.tcIndexSettings.Location = new System.Drawing.Point(13, 173);
            this.tcIndexSettings.Name = "tcIndexSettings";
            this.tcIndexSettings.SelectedIndex = 0;
            this.tcIndexSettings.Size = new System.Drawing.Size(311, 310);
            this.tcIndexSettings.TabIndex = 10;
            // 
            // tpAttributeIndexes
            // 
            this.tpAttributeIndexes.Controls.Add(this.lblSep5);
            this.tpAttributeIndexes.Controls.Add(this.lblSep4);
            this.tpAttributeIndexes.Controls.Add(this.lblSep3);
            this.tpAttributeIndexes.Controls.Add(this.lblSep2);
            this.tpAttributeIndexes.Controls.Add(this.lblSep1);
            this.tpAttributeIndexes.Controls.Add(this.btnGetDefaultBounds);
            this.tpAttributeIndexes.Controls.Add(this.tbIdxExpr5);
            this.tpAttributeIndexes.Controls.Add(this.tbIdxExpr4);
            this.tpAttributeIndexes.Controls.Add(this.tbIdxExpr3);
            this.tpAttributeIndexes.Controls.Add(this.tbIdxExpr2);
            this.tpAttributeIndexes.Controls.Add(this.tbIdxExpr1);
            this.tpAttributeIndexes.Controls.Add(this.tbLevelBound5);
            this.tpAttributeIndexes.Controls.Add(this.tbLevelBound4);
            this.tpAttributeIndexes.Controls.Add(this.tbLevelBound3);
            this.tpAttributeIndexes.Controls.Add(this.tbLevelBound2);
            this.tpAttributeIndexes.Controls.Add(this.tbLevelBound1);
            this.tpAttributeIndexes.Controls.Add(this.lblLevel5);
            this.tpAttributeIndexes.Controls.Add(this.lblLevel4);
            this.tpAttributeIndexes.Controls.Add(this.lblLevel3);
            this.tpAttributeIndexes.Controls.Add(this.lblLevel2);
            this.tpAttributeIndexes.Controls.Add(this.lblLevel1);
            this.tpAttributeIndexes.Controls.Add(this.lblIndexExpressions);
            this.tpAttributeIndexes.Controls.Add(this.lblLevelBounds);
            this.tpAttributeIndexes.Controls.Add(this.lblIndexName);
            this.tpAttributeIndexes.Controls.Add(this.tbIndexName);
            this.tpAttributeIndexes.Controls.Add(this.lblSourceAttributes);
            this.tpAttributeIndexes.Controls.Add(this.chlbSourceAttributes);
            this.tpAttributeIndexes.Location = new System.Drawing.Point(4, 22);
            this.tpAttributeIndexes.Name = "tpAttributeIndexes";
            this.tpAttributeIndexes.Padding = new System.Windows.Forms.Padding(3);
            this.tpAttributeIndexes.Size = new System.Drawing.Size(303, 284);
            this.tpAttributeIndexes.TabIndex = 0;
            this.tpAttributeIndexes.Text = "Measure Indexes";
            this.tpAttributeIndexes.UseVisualStyleBackColor = true;
            // 
            // lblSep5
            // 
            this.lblSep5.AutoSize = true;
            this.lblSep5.Location = new System.Drawing.Point(90, 234);
            this.lblSep5.Name = "lblSep5";
            this.lblSep5.Size = new System.Drawing.Size(10, 13);
            this.lblSep5.TabIndex = 0;
            this.lblSep5.Text = ":";
            // 
            // lblSep4
            // 
            this.lblSep4.AutoSize = true;
            this.lblSep4.Location = new System.Drawing.Point(90, 208);
            this.lblSep4.Name = "lblSep4";
            this.lblSep4.Size = new System.Drawing.Size(10, 13);
            this.lblSep4.TabIndex = 0;
            this.lblSep4.Text = ":";
            // 
            // lblSep3
            // 
            this.lblSep3.AutoSize = true;
            this.lblSep3.Location = new System.Drawing.Point(90, 182);
            this.lblSep3.Name = "lblSep3";
            this.lblSep3.Size = new System.Drawing.Size(10, 13);
            this.lblSep3.TabIndex = 0;
            this.lblSep3.Text = ":";
            // 
            // lblSep2
            // 
            this.lblSep2.AutoSize = true;
            this.lblSep2.Location = new System.Drawing.Point(90, 156);
            this.lblSep2.Name = "lblSep2";
            this.lblSep2.Size = new System.Drawing.Size(10, 13);
            this.lblSep2.TabIndex = 0;
            this.lblSep2.Text = ":";
            // 
            // lblSep1
            // 
            this.lblSep1.AutoSize = true;
            this.lblSep1.Location = new System.Drawing.Point(90, 130);
            this.lblSep1.Name = "lblSep1";
            this.lblSep1.Size = new System.Drawing.Size(10, 13);
            this.lblSep1.TabIndex = 0;
            this.lblSep1.Text = ":";
            // 
            // btnGetDefaultBounds
            // 
            this.btnGetDefaultBounds.Location = new System.Drawing.Point(9, 256);
            this.btnGetDefaultBounds.Name = "btnGetDefaultBounds";
            this.btnGetDefaultBounds.Size = new System.Drawing.Size(76, 23);
            this.btnGetDefaultBounds.TabIndex = 13;
            this.btnGetDefaultBounds.Text = "Get Defaults";
            this.btnGetDefaultBounds.UseVisualStyleBackColor = true;
            this.btnGetDefaultBounds.Click += new System.EventHandler(this.btnGetDefaultBounds_Click);
            // 
            // tbIdxExpr5
            // 
            this.tbIdxExpr5.Location = new System.Drawing.Point(106, 231);
            this.tbIdxExpr5.Name = "tbIdxExpr5";
            this.tbIdxExpr5.Size = new System.Drawing.Size(190, 20);
            this.tbIdxExpr5.TabIndex = 12;
            this.tbIdxExpr5.TextChanged += new System.EventHandler(this.SetProfileLevelExpression);
            // 
            // tbIdxExpr4
            // 
            this.tbIdxExpr4.Location = new System.Drawing.Point(106, 205);
            this.tbIdxExpr4.Name = "tbIdxExpr4";
            this.tbIdxExpr4.Size = new System.Drawing.Size(190, 20);
            this.tbIdxExpr4.TabIndex = 10;
            this.tbIdxExpr4.TextChanged += new System.EventHandler(this.SetProfileLevelExpression);
            // 
            // tbIdxExpr3
            // 
            this.tbIdxExpr3.Location = new System.Drawing.Point(106, 179);
            this.tbIdxExpr3.Name = "tbIdxExpr3";
            this.tbIdxExpr3.Size = new System.Drawing.Size(190, 20);
            this.tbIdxExpr3.TabIndex = 8;
            this.tbIdxExpr3.TextChanged += new System.EventHandler(this.SetProfileLevelExpression);
            // 
            // tbIdxExpr2
            // 
            this.tbIdxExpr2.Location = new System.Drawing.Point(106, 153);
            this.tbIdxExpr2.Name = "tbIdxExpr2";
            this.tbIdxExpr2.Size = new System.Drawing.Size(190, 20);
            this.tbIdxExpr2.TabIndex = 6;
            this.tbIdxExpr2.TextChanged += new System.EventHandler(this.SetProfileLevelExpression);
            // 
            // tbIdxExpr1
            // 
            this.tbIdxExpr1.Location = new System.Drawing.Point(106, 127);
            this.tbIdxExpr1.Name = "tbIdxExpr1";
            this.tbIdxExpr1.Size = new System.Drawing.Size(190, 20);
            this.tbIdxExpr1.TabIndex = 4;
            this.tbIdxExpr1.TextChanged += new System.EventHandler(this.SetProfileLevelExpression);
            // 
            // tbLevelBound5
            // 
            this.tbLevelBound5.Location = new System.Drawing.Point(32, 231);
            this.tbLevelBound5.Name = "tbLevelBound5";
            this.tbLevelBound5.Size = new System.Drawing.Size(52, 20);
            this.tbLevelBound5.TabIndex = 11;
            this.tbLevelBound5.TextChanged += new System.EventHandler(this.SetProfileLevelBound);
            // 
            // tbLevelBound4
            // 
            this.tbLevelBound4.Location = new System.Drawing.Point(32, 205);
            this.tbLevelBound4.Name = "tbLevelBound4";
            this.tbLevelBound4.Size = new System.Drawing.Size(52, 20);
            this.tbLevelBound4.TabIndex = 9;
            this.tbLevelBound4.TextChanged += new System.EventHandler(this.SetProfileLevelBound);
            // 
            // tbLevelBound3
            // 
            this.tbLevelBound3.Location = new System.Drawing.Point(32, 179);
            this.tbLevelBound3.Name = "tbLevelBound3";
            this.tbLevelBound3.Size = new System.Drawing.Size(52, 20);
            this.tbLevelBound3.TabIndex = 7;
            this.tbLevelBound3.TextChanged += new System.EventHandler(this.SetProfileLevelBound);
            // 
            // tbLevelBound2
            // 
            this.tbLevelBound2.Location = new System.Drawing.Point(32, 153);
            this.tbLevelBound2.Name = "tbLevelBound2";
            this.tbLevelBound2.Size = new System.Drawing.Size(52, 20);
            this.tbLevelBound2.TabIndex = 5;
            this.tbLevelBound2.TextChanged += new System.EventHandler(this.SetProfileLevelBound);
            // 
            // tbLevelBound1
            // 
            this.tbLevelBound1.Location = new System.Drawing.Point(32, 127);
            this.tbLevelBound1.Name = "tbLevelBound1";
            this.tbLevelBound1.Size = new System.Drawing.Size(52, 20);
            this.tbLevelBound1.TabIndex = 3;
            this.tbLevelBound1.Tag = "";
            this.tbLevelBound1.TextChanged += new System.EventHandler(this.SetProfileLevelBound);
            // 
            // lblLevel5
            // 
            this.lblLevel5.AutoSize = true;
            this.lblLevel5.Location = new System.Drawing.Point(7, 234);
            this.lblLevel5.Name = "lblLevel5";
            this.lblLevel5.Size = new System.Drawing.Size(19, 13);
            this.lblLevel5.TabIndex = 0;
            this.lblLevel5.Text = "5 :";
            // 
            // lblLevel4
            // 
            this.lblLevel4.AutoSize = true;
            this.lblLevel4.Location = new System.Drawing.Point(7, 208);
            this.lblLevel4.Name = "lblLevel4";
            this.lblLevel4.Size = new System.Drawing.Size(19, 13);
            this.lblLevel4.TabIndex = 0;
            this.lblLevel4.Text = "4 :";
            // 
            // lblLevel3
            // 
            this.lblLevel3.AutoSize = true;
            this.lblLevel3.Location = new System.Drawing.Point(7, 182);
            this.lblLevel3.Name = "lblLevel3";
            this.lblLevel3.Size = new System.Drawing.Size(19, 13);
            this.lblLevel3.TabIndex = 0;
            this.lblLevel3.Text = "3 :";
            // 
            // lblLevel2
            // 
            this.lblLevel2.AutoSize = true;
            this.lblLevel2.Location = new System.Drawing.Point(7, 156);
            this.lblLevel2.Name = "lblLevel2";
            this.lblLevel2.Size = new System.Drawing.Size(19, 13);
            this.lblLevel2.TabIndex = 0;
            this.lblLevel2.Text = "2 :";
            // 
            // lblLevel1
            // 
            this.lblLevel1.AutoSize = true;
            this.lblLevel1.Location = new System.Drawing.Point(7, 130);
            this.lblLevel1.Name = "lblLevel1";
            this.lblLevel1.Size = new System.Drawing.Size(19, 13);
            this.lblLevel1.TabIndex = 0;
            this.lblLevel1.Text = "1 :";
            // 
            // lblIndexExpressions
            // 
            this.lblIndexExpressions.AutoSize = true;
            this.lblIndexExpressions.Location = new System.Drawing.Point(103, 110);
            this.lblIndexExpressions.Name = "lblIndexExpressions";
            this.lblIndexExpressions.Size = new System.Drawing.Size(184, 13);
            this.lblIndexExpressions.TabIndex = 0;
            this.lblIndexExpressions.Text = "Expressions to Compute Index Values";
            // 
            // lblLevelBounds
            // 
            this.lblLevelBounds.AutoSize = true;
            this.lblLevelBounds.Location = new System.Drawing.Point(7, 110);
            this.lblLevelBounds.Name = "lblLevelBounds";
            this.lblLevelBounds.Size = new System.Drawing.Size(72, 13);
            this.lblLevelBounds.TabIndex = 0;
            this.lblLevelBounds.Text = "Level Bounds";
            // 
            // lblIndexName
            // 
            this.lblIndexName.AutoSize = true;
            this.lblIndexName.Location = new System.Drawing.Point(151, 63);
            this.lblIndexName.Name = "lblIndexName";
            this.lblIndexName.Size = new System.Drawing.Size(131, 13);
            this.lblIndexName.TabIndex = 0;
            this.lblIndexName.Text = "Name of Associated Index";
            // 
            // tbIndexName
            // 
            this.tbIndexName.Location = new System.Drawing.Point(154, 79);
            this.tbIndexName.Name = "tbIndexName";
            this.tbIndexName.Size = new System.Drawing.Size(142, 20);
            this.tbIndexName.TabIndex = 2;
            this.tbIndexName.TextChanged += new System.EventHandler(this.tbIndexName_TextChanged);
            // 
            // lblSourceAttributes
            // 
            this.lblSourceAttributes.AutoSize = true;
            this.lblSourceAttributes.Location = new System.Drawing.Point(3, 3);
            this.lblSourceAttributes.Name = "lblSourceAttributes";
            this.lblSourceAttributes.Size = new System.Drawing.Size(116, 13);
            this.lblSourceAttributes.TabIndex = 0;
            this.lblSourceAttributes.Text = "Performance Measures";
            // 
            // chlbSourceAttributes
            // 
            this.chlbSourceAttributes.Location = new System.Drawing.Point(6, 20);
            this.chlbSourceAttributes.Name = "chlbSourceAttributes";
            this.chlbSourceAttributes.Size = new System.Drawing.Size(142, 79);
            this.chlbSourceAttributes.Sorted = true;
            this.chlbSourceAttributes.TabIndex = 1;
            this.chlbSourceAttributes.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chlbSourceAttributes_ItemCheck);
            this.chlbSourceAttributes.SelectedIndexChanged += new System.EventHandler(this.chlbSourceAttributes_SelectedIndexChanged);
            // 
            // tpDerivedIndexes
            // 
            this.tpDerivedIndexes.Controls.Add(this.lblExpression);
            this.tpDerivedIndexes.Controls.Add(this.tbExpression);
            this.tpDerivedIndexes.Controls.Add(this.btnRemoveIndex);
            this.tpDerivedIndexes.Controls.Add(this.lblIndexes);
            this.tpDerivedIndexes.Controls.Add(this.btnAddIndex);
            this.tpDerivedIndexes.Controls.Add(this.chlbDerivedIndexes);
            this.tpDerivedIndexes.Controls.Add(this.btnRenameIndex);
            this.tpDerivedIndexes.Location = new System.Drawing.Point(4, 22);
            this.tpDerivedIndexes.Name = "tpDerivedIndexes";
            this.tpDerivedIndexes.Padding = new System.Windows.Forms.Padding(3);
            this.tpDerivedIndexes.Size = new System.Drawing.Size(303, 284);
            this.tpDerivedIndexes.TabIndex = 1;
            this.tpDerivedIndexes.Text = "Derived Indexes";
            this.tpDerivedIndexes.UseVisualStyleBackColor = true;
            // 
            // lblExpression
            // 
            this.lblExpression.AutoSize = true;
            this.lblExpression.Location = new System.Drawing.Point(3, 111);
            this.lblExpression.Name = "lblExpression";
            this.lblExpression.Size = new System.Drawing.Size(58, 13);
            this.lblExpression.TabIndex = 0;
            this.lblExpression.Text = "Expression";
            // 
            // tbExpression
            // 
            this.tbExpression.AcceptsReturn = true;
            this.tbExpression.AcceptsTab = true;
            this.tbExpression.Location = new System.Drawing.Point(6, 127);
            this.tbExpression.Multiline = true;
            this.tbExpression.Name = "tbExpression";
            this.tbExpression.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbExpression.Size = new System.Drawing.Size(291, 147);
            this.tbExpression.TabIndex = 5;
            this.tbExpression.WordWrap = false;
            this.tbExpression.TextChanged += new System.EventHandler(this.tbExpression_TextChanged);
            // 
            // btnRemoveIndex
            // 
            this.btnRemoveIndex.Location = new System.Drawing.Point(154, 77);
            this.btnRemoveIndex.Name = "btnRemoveIndex";
            this.btnRemoveIndex.Size = new System.Drawing.Size(69, 23);
            this.btnRemoveIndex.TabIndex = 4;
            this.btnRemoveIndex.Text = "Remove";
            this.btnRemoveIndex.UseVisualStyleBackColor = true;
            this.btnRemoveIndex.Click += new System.EventHandler(this.btnRemoveIndex_Click);
            // 
            // lblIndexes
            // 
            this.lblIndexes.AutoSize = true;
            this.lblIndexes.Location = new System.Drawing.Point(3, 3);
            this.lblIndexes.Name = "lblIndexes";
            this.lblIndexes.Size = new System.Drawing.Size(44, 13);
            this.lblIndexes.TabIndex = 0;
            this.lblIndexes.Text = "Indexes";
            // 
            // btnAddIndex
            // 
            this.btnAddIndex.Location = new System.Drawing.Point(154, 19);
            this.btnAddIndex.Name = "btnAddIndex";
            this.btnAddIndex.Size = new System.Drawing.Size(69, 23);
            this.btnAddIndex.TabIndex = 2;
            this.btnAddIndex.Text = "Add";
            this.btnAddIndex.UseVisualStyleBackColor = true;
            this.btnAddIndex.Click += new System.EventHandler(this.btnAddIndex_Click);
            // 
            // chlbDerivedIndexes
            // 
            this.chlbDerivedIndexes.Location = new System.Drawing.Point(6, 20);
            this.chlbDerivedIndexes.Name = "chlbDerivedIndexes";
            this.chlbDerivedIndexes.Size = new System.Drawing.Size(142, 79);
            this.chlbDerivedIndexes.Sorted = true;
            this.chlbDerivedIndexes.TabIndex = 1;
            this.chlbDerivedIndexes.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chlbDerivedIndexes_ItemCheck);
            this.chlbDerivedIndexes.SelectedIndexChanged += new System.EventHandler(this.chlbDerivedIndexes_SelectedIndexChanged);
            // 
            // btnRenameIndex
            // 
            this.btnRenameIndex.Location = new System.Drawing.Point(154, 48);
            this.btnRenameIndex.Name = "btnRenameIndex";
            this.btnRenameIndex.Size = new System.Drawing.Size(69, 23);
            this.btnRenameIndex.TabIndex = 3;
            this.btnRenameIndex.Text = "Rename";
            this.btnRenameIndex.UseVisualStyleBackColor = true;
            this.btnRenameIndex.Click += new System.EventHandler(this.btnRenameIndex_Click);
            // 
            // lblSimulation
            // 
            this.lblSimulation.AutoSize = true;
            this.lblSimulation.Location = new System.Drawing.Point(19, 44);
            this.lblSimulation.Name = "lblSimulation";
            this.lblSimulation.Size = new System.Drawing.Size(55, 13);
            this.lblSimulation.TabIndex = 0;
            this.lblSimulation.Text = "Simulation";
            // 
            // tbNetwork
            // 
            this.tbNetwork.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbNetwork.Location = new System.Drawing.Point(80, 12);
            this.tbNetwork.Name = "tbNetwork";
            this.tbNetwork.ReadOnly = true;
            this.tbNetwork.Size = new System.Drawing.Size(233, 20);
            this.tbNetwork.TabIndex = 0;
            this.tbNetwork.TabStop = false;
            // 
            // tbSimulation
            // 
            this.tbSimulation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbSimulation.Location = new System.Drawing.Point(80, 42);
            this.tbSimulation.Name = "tbSimulation";
            this.tbSimulation.ReadOnly = true;
            this.tbSimulation.Size = new System.Drawing.Size(233, 20);
            this.tbSimulation.TabIndex = 0;
            this.tbSimulation.TabStop = false;
            // 
            // gbProfileControl
            // 
            this.gbProfileControl.Controls.Add(this.tbCurrentProfile);
            this.gbProfileControl.Controls.Add(this.lblCurrentProfile);
            this.gbProfileControl.Controls.Add(this.cbAvailableProfiles);
            this.gbProfileControl.Controls.Add(this.lblAvailableProfiles);
            this.gbProfileControl.Controls.Add(this.btnSaveProfile);
            this.gbProfileControl.Controls.Add(this.chbAsNewProfile);
            this.gbProfileControl.Controls.Add(this.btnLoadProfile);
            this.gbProfileControl.Controls.Add(this.btnDeleteProfile);
            this.gbProfileControl.Location = new System.Drawing.Point(13, 68);
            this.gbProfileControl.Name = "gbProfileControl";
            this.gbProfileControl.Size = new System.Drawing.Size(310, 93);
            this.gbProfileControl.TabIndex = 13;
            this.gbProfileControl.TabStop = false;
            // 
            // btnFinish
            // 
            this.btnFinish.Location = new System.Drawing.Point(13, 539);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(310, 23);
            this.btnFinish.TabIndex = 8;
            this.btnFinish.Text = "Finish";
            this.btnFinish.UseVisualStyleBackColor = true;
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // lblDstDir
            // 
            this.lblDstDir.AutoSize = true;
            this.lblDstDir.Location = new System.Drawing.Point(13, 493);
            this.lblDstDir.Name = "lblDstDir";
            this.lblDstDir.Size = new System.Drawing.Size(92, 13);
            this.lblDstDir.TabIndex = 0;
            this.lblDstDir.Text = "Destination Folder";
            // 
            // btnPickDir
            // 
            this.btnPickDir.AutoSize = true;
            this.btnPickDir.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnPickDir.Location = new System.Drawing.Point(238, 507);
            this.btnPickDir.Name = "btnPickDir";
            this.btnPickDir.Size = new System.Drawing.Size(85, 23);
            this.btnPickDir.TabIndex = 7;
            this.btnPickDir.Text = "Choose Folder";
            this.btnPickDir.UseVisualStyleBackColor = true;
            this.btnPickDir.Click += new System.EventHandler(this.btnPickDir_Click);
            // 
            // tbDstDir
            // 
            this.tbDstDir.Location = new System.Drawing.Point(13, 509);
            this.tbDstDir.Name = "tbDstDir";
            this.tbDstDir.Size = new System.Drawing.Size(219, 20);
            this.tbDstDir.TabIndex = 6;
            this.tbDstDir.TextChanged += new System.EventHandler(this.tbDstDir_TextChanged);
            // 
            // FormLmDmVmtConditionDualReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 573);
            this.Controls.Add(this.lblDstDir);
            this.Controls.Add(this.btnPickDir);
            this.Controls.Add(this.tbDstDir);
            this.Controls.Add(this.btnFinish);
            this.Controls.Add(this.gbProfileControl);
            this.Controls.Add(this.tbSimulation);
            this.Controls.Add(this.tbNetwork);
            this.Controls.Add(this.lblSimulation);
            this.Controls.Add(this.tcIndexSettings);
            this.Controls.Add(this.lblNetwork);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormLmDmVmtConditionDualReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LM DM VMT Condition - Index Settings";
            this.tcIndexSettings.ResumeLayout(false);
            this.tpAttributeIndexes.ResumeLayout(false);
            this.tpAttributeIndexes.PerformLayout();
            this.tpDerivedIndexes.ResumeLayout(false);
            this.tpDerivedIndexes.PerformLayout();
            this.gbProfileControl.ResumeLayout(false);
            this.gbProfileControl.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNetwork;
        private System.Windows.Forms.TextBox tbCurrentProfile;
        private System.Windows.Forms.Label lblCurrentProfile;
        private System.Windows.Forms.ComboBox cbAvailableProfiles;
        private System.Windows.Forms.Label lblAvailableProfiles;
        private System.Windows.Forms.Button btnSaveProfile;
        private System.Windows.Forms.CheckBox chbAsNewProfile;
        private System.Windows.Forms.Button btnLoadProfile;
        private System.Windows.Forms.Button btnDeleteProfile;
        private System.Windows.Forms.TabControl tcIndexSettings;
        private System.Windows.Forms.TabPage tpAttributeIndexes;
        private System.Windows.Forms.TabPage tpDerivedIndexes;
        private System.Windows.Forms.Label lblIndexName;
        private System.Windows.Forms.TextBox tbIndexName;
        private System.Windows.Forms.Label lblSourceAttributes;
        private System.Windows.Forms.CheckedListBox chlbSourceAttributes;
        private System.Windows.Forms.Label lblSimulation;
        private System.Windows.Forms.TextBox tbNetwork;
        private System.Windows.Forms.TextBox tbSimulation;
        private System.Windows.Forms.TextBox tbIdxExpr5;
        private System.Windows.Forms.TextBox tbIdxExpr4;
        private System.Windows.Forms.TextBox tbIdxExpr3;
        private System.Windows.Forms.TextBox tbIdxExpr2;
        private System.Windows.Forms.TextBox tbIdxExpr1;
        private System.Windows.Forms.TextBox tbLevelBound5;
        private System.Windows.Forms.TextBox tbLevelBound4;
        private System.Windows.Forms.TextBox tbLevelBound3;
        private System.Windows.Forms.TextBox tbLevelBound2;
        private System.Windows.Forms.TextBox tbLevelBound1;
        private System.Windows.Forms.Label lblLevel5;
        private System.Windows.Forms.Label lblLevel4;
        private System.Windows.Forms.Label lblLevel3;
        private System.Windows.Forms.Label lblLevel2;
        private System.Windows.Forms.Label lblLevel1;
        private System.Windows.Forms.Label lblIndexExpressions;
        private System.Windows.Forms.Label lblLevelBounds;
        private System.Windows.Forms.GroupBox gbProfileControl;
        private System.Windows.Forms.Label lblExpression;
        private System.Windows.Forms.TextBox tbExpression;
        private System.Windows.Forms.Button btnRemoveIndex;
        private System.Windows.Forms.Label lblIndexes;
        private System.Windows.Forms.Button btnAddIndex;
        private System.Windows.Forms.CheckedListBox chlbDerivedIndexes;
        private System.Windows.Forms.Button btnRenameIndex;
        private System.Windows.Forms.Button btnFinish;
        private System.Windows.Forms.Label lblDstDir;
        private System.Windows.Forms.Button btnPickDir;
        private System.Windows.Forms.TextBox tbDstDir;
        private System.Windows.Forms.Button btnGetDefaultBounds;
        private System.Windows.Forms.Label lblSep5;
        private System.Windows.Forms.Label lblSep4;
        private System.Windows.Forms.Label lblSep3;
        private System.Windows.Forms.Label lblSep2;
        private System.Windows.Forms.Label lblSep1;
        private System.Windows.Forms.FolderBrowserDialog fbdPickDir;
    }
}