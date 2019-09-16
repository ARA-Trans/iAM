namespace RoadCare3
{
    partial class FormConfigurableInputSummary
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigurableInputSummary));
            this.chlbSections = new System.Windows.Forms.CheckedListBox();
            this.lblSections = new System.Windows.Forms.Label();
            this.btnGenRep = new System.Windows.Forms.Button();
            this.fbdPickDir = new System.Windows.Forms.FolderBrowserDialog();
            this.tbDstDir = new System.Windows.Forms.TextBox();
            this.btnPickDir = new System.Windows.Forms.Button();
            this.lblDstDir = new System.Windows.Forms.Label();
            this.gbConfigProfiles = new System.Windows.Forms.GroupBox();
            this.lblAvailProfiles = new System.Windows.Forms.Label();
            this.lblCurrProfile = new System.Windows.Forms.Label();
            this.tbCurrentProfile = new System.Windows.Forms.TextBox();
            this.btnRemoveProfile = new System.Windows.Forms.Button();
            this.btnLoadProfile = new System.Windows.Forms.Button();
            this.btnSaveProfile = new System.Windows.Forms.Button();
            this.lbAvailableProfiles = new System.Windows.Forms.ListBox();
            this.gbConfigProfiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // chlbSections
            // 
            this.chlbSections.CheckOnClick = true;
            this.chlbSections.FormattingEnabled = true;
            this.chlbSections.Location = new System.Drawing.Point(12, 28);
            this.chlbSections.Name = "chlbSections";
            this.chlbSections.Size = new System.Drawing.Size(310, 124);
            this.chlbSections.TabIndex = 0;
            // 
            // lblSections
            // 
            this.lblSections.AutoSize = true;
            this.lblSections.Location = new System.Drawing.Point(12, 12);
            this.lblSections.Name = "lblSections";
            this.lblSections.Size = new System.Drawing.Size(92, 13);
            this.lblSections.TabIndex = 0;
            this.lblSections.Text = "Included Sections";
            // 
            // btnGenRep
            // 
            this.btnGenRep.Location = new System.Drawing.Point(12, 223);
            this.btnGenRep.Name = "btnGenRep";
            this.btnGenRep.Size = new System.Drawing.Size(310, 23);
            this.btnGenRep.TabIndex = 3;
            this.btnGenRep.Text = "Finish";
            this.btnGenRep.UseVisualStyleBackColor = true;
            this.btnGenRep.Click += new System.EventHandler(this.btnGenRep_Click);
            // 
            // tbDstDir
            // 
            this.tbDstDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbDstDir.Location = new System.Drawing.Point(12, 191);
            this.tbDstDir.Name = "tbDstDir";
            this.tbDstDir.Size = new System.Drawing.Size(219, 20);
            this.tbDstDir.TabIndex = 1;
            // 
            // btnPickDir
            // 
            this.btnPickDir.AutoSize = true;
            this.btnPickDir.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnPickDir.Location = new System.Drawing.Point(237, 189);
            this.btnPickDir.Name = "btnPickDir";
            this.btnPickDir.Size = new System.Drawing.Size(85, 23);
            this.btnPickDir.TabIndex = 2;
            this.btnPickDir.Text = "Choose Folder";
            this.btnPickDir.UseVisualStyleBackColor = true;
            this.btnPickDir.Click += new System.EventHandler(this.btnPickDir_Click);
            // 
            // lblDstDir
            // 
            this.lblDstDir.AutoSize = true;
            this.lblDstDir.Location = new System.Drawing.Point(12, 175);
            this.lblDstDir.Name = "lblDstDir";
            this.lblDstDir.Size = new System.Drawing.Size(92, 13);
            this.lblDstDir.TabIndex = 0;
            this.lblDstDir.Text = "Destination Folder";
            // 
            // gbConfigProfiles
            // 
            this.gbConfigProfiles.Controls.Add(this.lblAvailProfiles);
            this.gbConfigProfiles.Controls.Add(this.lblCurrProfile);
            this.gbConfigProfiles.Controls.Add(this.tbCurrentProfile);
            this.gbConfigProfiles.Controls.Add(this.btnRemoveProfile);
            this.gbConfigProfiles.Controls.Add(this.btnLoadProfile);
            this.gbConfigProfiles.Controls.Add(this.btnSaveProfile);
            this.gbConfigProfiles.Controls.Add(this.lbAvailableProfiles);
            this.gbConfigProfiles.Location = new System.Drawing.Point(338, 12);
            this.gbConfigProfiles.Name = "gbConfigProfiles";
            this.gbConfigProfiles.Size = new System.Drawing.Size(250, 234);
            this.gbConfigProfiles.TabIndex = 0;
            this.gbConfigProfiles.TabStop = false;
            this.gbConfigProfiles.Text = "Report Configuration Profiles";
            // 
            // lblAvailProfiles
            // 
            this.lblAvailProfiles.AutoSize = true;
            this.lblAvailProfiles.Location = new System.Drawing.Point(6, 65);
            this.lblAvailProfiles.Name = "lblAvailProfiles";
            this.lblAvailProfiles.Size = new System.Drawing.Size(87, 13);
            this.lblAvailProfiles.TabIndex = 0;
            this.lblAvailProfiles.Text = "Available Profiles";
            // 
            // lblCurrProfile
            // 
            this.lblCurrProfile.AutoSize = true;
            this.lblCurrProfile.Location = new System.Drawing.Point(6, 21);
            this.lblCurrProfile.Name = "lblCurrProfile";
            this.lblCurrProfile.Size = new System.Drawing.Size(73, 13);
            this.lblCurrProfile.TabIndex = 0;
            this.lblCurrProfile.Text = "Current Profile";
            // 
            // tbCurrentProfile
            // 
            this.tbCurrentProfile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCurrentProfile.Location = new System.Drawing.Point(6, 37);
            this.tbCurrentProfile.Name = "tbCurrentProfile";
            this.tbCurrentProfile.Size = new System.Drawing.Size(157, 20);
            this.tbCurrentProfile.TabIndex = 4;
            this.tbCurrentProfile.TextChanged += new System.EventHandler(this.tbCurrentProfile_TextChanged);
            // 
            // btnRemoveProfile
            // 
            this.btnRemoveProfile.Location = new System.Drawing.Point(169, 110);
            this.btnRemoveProfile.Name = "btnRemoveProfile";
            this.btnRemoveProfile.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveProfile.TabIndex = 8;
            this.btnRemoveProfile.Text = "Remove";
            this.btnRemoveProfile.UseVisualStyleBackColor = true;
            this.btnRemoveProfile.Click += new System.EventHandler(this.btnRemoveProfile_Click);
            // 
            // btnLoadProfile
            // 
            this.btnLoadProfile.Location = new System.Drawing.Point(169, 81);
            this.btnLoadProfile.Name = "btnLoadProfile";
            this.btnLoadProfile.Size = new System.Drawing.Size(75, 23);
            this.btnLoadProfile.TabIndex = 7;
            this.btnLoadProfile.Text = "Load";
            this.btnLoadProfile.UseVisualStyleBackColor = true;
            this.btnLoadProfile.Click += new System.EventHandler(this.btnLoadProfile_Click);
            // 
            // btnSaveProfile
            // 
            this.btnSaveProfile.Location = new System.Drawing.Point(169, 36);
            this.btnSaveProfile.Name = "btnSaveProfile";
            this.btnSaveProfile.Size = new System.Drawing.Size(75, 23);
            this.btnSaveProfile.TabIndex = 5;
            this.btnSaveProfile.Text = "Save";
            this.btnSaveProfile.UseVisualStyleBackColor = true;
            this.btnSaveProfile.Click += new System.EventHandler(this.btnSaveProfile_Click);
            // 
            // lbAvailableProfiles
            // 
            this.lbAvailableProfiles.FormattingEnabled = true;
            this.lbAvailableProfiles.Location = new System.Drawing.Point(6, 81);
            this.lbAvailableProfiles.Name = "lbAvailableProfiles";
            this.lbAvailableProfiles.Size = new System.Drawing.Size(157, 147);
            this.lbAvailableProfiles.TabIndex = 6;
            this.lbAvailableProfiles.SelectedIndexChanged += new System.EventHandler(this.lbAvailableProfiles_SelectedIndexChanged);
            // 
            // FormConfigurableInputSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 258);
            this.Controls.Add(this.gbConfigProfiles);
            this.Controls.Add(this.lblDstDir);
            this.Controls.Add(this.btnPickDir);
            this.Controls.Add(this.tbDstDir);
            this.Controls.Add(this.btnGenRep);
            this.Controls.Add(this.lblSections);
            this.Controls.Add(this.chlbSections);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigurableInputSummary";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configurable Input Summary Report";
            this.gbConfigProfiles.ResumeLayout(false);
            this.gbConfigProfiles.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox chlbSections;
        private System.Windows.Forms.Label lblSections;
        private System.Windows.Forms.Button btnGenRep;
        private System.Windows.Forms.FolderBrowserDialog fbdPickDir;
        private System.Windows.Forms.TextBox tbDstDir;
        private System.Windows.Forms.Button btnPickDir;
        private System.Windows.Forms.Label lblDstDir;
        private System.Windows.Forms.GroupBox gbConfigProfiles;
        private System.Windows.Forms.Label lblCurrProfile;
        private System.Windows.Forms.TextBox tbCurrentProfile;
        private System.Windows.Forms.Button btnRemoveProfile;
        private System.Windows.Forms.Button btnLoadProfile;
        private System.Windows.Forms.Button btnSaveProfile;
        private System.Windows.Forms.ListBox lbAvailableProfiles;
        private System.Windows.Forms.Label lblAvailProfiles;
    }
}