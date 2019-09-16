namespace RoadCare3
{
    partial class FormFyTargetsSuggestedProjects
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFyTargetsSuggestedProjects));
            this.tbPrevFy = new System.Windows.Forms.TextBox();
            this.tbCondYr = new System.Windows.Forms.TextBox();
            this.lblCurrFy = new System.Windows.Forms.Label();
            this.lblPriorFy = new System.Windows.Forms.Label();
            this.lblCondYr = new System.Windows.Forms.Label();
            this.cbCurrFy = new System.Windows.Forms.ComboBox();
            this.btnFinish = new System.Windows.Forms.Button();
            this.fbdPickDir = new System.Windows.Forms.FolderBrowserDialog();
            this.btnPickDir = new System.Windows.Forms.Button();
            this.tbDstDir = new System.Windows.Forms.TextBox();
            this.lblDstDir = new System.Windows.Forms.Label();
            this.ofdPickTemplate = new System.Windows.Forms.OpenFileDialog();
            this.tbTemplateFile = new System.Windows.Forms.TextBox();
            this.btnPickTemplate = new System.Windows.Forms.Button();
            this.chbTemplateFile = new System.Windows.Forms.CheckBox();
            this.ttTemplateFile = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // tbPrevFy
            // 
            this.tbPrevFy.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbPrevFy.Location = new System.Drawing.Point(137, 25);
            this.tbPrevFy.Name = "tbPrevFy";
            this.tbPrevFy.ReadOnly = true;
            this.tbPrevFy.Size = new System.Drawing.Size(110, 20);
            this.tbPrevFy.TabIndex = 1;
            this.tbPrevFy.TabStop = false;
            // 
            // tbCondYr
            // 
            this.tbCondYr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCondYr.Location = new System.Drawing.Point(262, 25);
            this.tbCondYr.Name = "tbCondYr";
            this.tbCondYr.ReadOnly = true;
            this.tbCondYr.Size = new System.Drawing.Size(110, 20);
            this.tbCondYr.TabIndex = 2;
            this.tbCondYr.TabStop = false;
            // 
            // lblCurrFy
            // 
            this.lblCurrFy.AutoSize = true;
            this.lblCurrFy.Location = new System.Drawing.Point(12, 9);
            this.lblCurrFy.Name = "lblCurrFy";
            this.lblCurrFy.Size = new System.Drawing.Size(59, 13);
            this.lblCurrFy.TabIndex = 3;
            this.lblCurrFy.Text = "Fiscal Year";
            // 
            // lblPriorFy
            // 
            this.lblPriorFy.AutoSize = true;
            this.lblPriorFy.Location = new System.Drawing.Point(137, 9);
            this.lblPriorFy.Name = "lblPriorFy";
            this.lblPriorFy.Size = new System.Drawing.Size(83, 13);
            this.lblPriorFy.TabIndex = 4;
            this.lblPriorFy.Text = "Prior Fiscal Year";
            // 
            // lblCondYr
            // 
            this.lblCondYr.AutoSize = true;
            this.lblCondYr.Location = new System.Drawing.Point(262, 9);
            this.lblCondYr.Name = "lblCondYr";
            this.lblCondYr.Size = new System.Drawing.Size(102, 13);
            this.lblCondYr.TabIndex = 5;
            this.lblCondYr.Text = "Condition Data Year";
            // 
            // cbCurrFy
            // 
            this.cbCurrFy.FormattingEnabled = true;
            this.cbCurrFy.Location = new System.Drawing.Point(12, 25);
            this.cbCurrFy.Name = "cbCurrFy";
            this.cbCurrFy.Size = new System.Drawing.Size(110, 21);
            this.cbCurrFy.TabIndex = 0;
            this.cbCurrFy.SelectedIndexChanged += new System.EventHandler(this.cbCurrFy_SelectedIndexChanged);
            // 
            // btnFinish
            // 
            this.btnFinish.Location = new System.Drawing.Point(12, 145);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(360, 23);
            this.btnFinish.TabIndex = 3;
            this.btnFinish.Text = "Finish";
            this.btnFinish.UseVisualStyleBackColor = true;
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // btnPickDir
            // 
            this.btnPickDir.Location = new System.Drawing.Point(265, 69);
            this.btnPickDir.Name = "btnPickDir";
            this.btnPickDir.Size = new System.Drawing.Size(107, 23);
            this.btnPickDir.TabIndex = 2;
            this.btnPickDir.Text = "Choose Destination";
            this.btnPickDir.UseVisualStyleBackColor = true;
            this.btnPickDir.Click += new System.EventHandler(this.btnPickDir_Click);
            // 
            // tbDstDir
            // 
            this.tbDstDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbDstDir.Location = new System.Drawing.Point(12, 70);
            this.tbDstDir.Name = "tbDstDir";
            this.tbDstDir.Size = new System.Drawing.Size(247, 20);
            this.tbDstDir.TabIndex = 1;
            // 
            // lblDstDir
            // 
            this.lblDstDir.AutoSize = true;
            this.lblDstDir.Location = new System.Drawing.Point(12, 54);
            this.lblDstDir.Name = "lblDstDir";
            this.lblDstDir.Size = new System.Drawing.Size(127, 13);
            this.lblDstDir.TabIndex = 8;
            this.lblDstDir.Text = "Report Destination Folder";
            // 
            // ofdPickTemplate
            // 
            this.ofdPickTemplate.Filter = "Microsoft Excel 2007 Worksheet (*.xlsx, *.xlsm)|*.xlsx;*.xlsm|All files|*.*";
            // 
            // tbTemplateFile
            // 
            this.tbTemplateFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbTemplateFile.Location = new System.Drawing.Point(12, 114);
            this.tbTemplateFile.Name = "tbTemplateFile";
            this.tbTemplateFile.ReadOnly = true;
            this.tbTemplateFile.Size = new System.Drawing.Size(247, 20);
            this.tbTemplateFile.TabIndex = 9;
            // 
            // btnPickTemplate
            // 
            this.btnPickTemplate.Enabled = false;
            this.btnPickTemplate.Location = new System.Drawing.Point(265, 113);
            this.btnPickTemplate.Name = "btnPickTemplate";
            this.btnPickTemplate.Size = new System.Drawing.Size(107, 23);
            this.btnPickTemplate.TabIndex = 11;
            this.btnPickTemplate.Text = "Choose Template";
            this.btnPickTemplate.UseVisualStyleBackColor = true;
            this.btnPickTemplate.Click += new System.EventHandler(this.btnPickTemplate_Click);
            // 
            // chbTemplateFile
            // 
            this.chbTemplateFile.AutoSize = true;
            this.chbTemplateFile.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chbTemplateFile.Location = new System.Drawing.Point(12, 96);
            this.chbTemplateFile.Name = "chbTemplateFile";
            this.chbTemplateFile.Size = new System.Drawing.Size(127, 17);
            this.chbTemplateFile.TabIndex = 12;
            this.chbTemplateFile.Text = "Custom Template File";
            this.ttTemplateFile.SetToolTip(this.chbTemplateFile, "The template must have a \"Suggested Projects\" sheet.");
            this.chbTemplateFile.UseVisualStyleBackColor = true;
            this.chbTemplateFile.CheckedChanged += new System.EventHandler(this.chbTemplateFile_CheckedChanged);
            // 
            // FormFyTargetsSuggestedProjects
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 179);
            this.Controls.Add(this.chbTemplateFile);
            this.Controls.Add(this.btnPickTemplate);
            this.Controls.Add(this.tbTemplateFile);
            this.Controls.Add(this.lblDstDir);
            this.Controls.Add(this.tbDstDir);
            this.Controls.Add(this.btnPickDir);
            this.Controls.Add(this.btnFinish);
            this.Controls.Add(this.cbCurrFy);
            this.Controls.Add(this.lblCondYr);
            this.Controls.Add(this.lblPriorFy);
            this.Controls.Add(this.lblCurrFy);
            this.Controls.Add(this.tbCondYr);
            this.Controls.Add(this.tbPrevFy);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFyTargetsSuggestedProjects";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FY Targets & Suggested Projects Report";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPrevFy;
        private System.Windows.Forms.TextBox tbCondYr;
        private System.Windows.Forms.Label lblCurrFy;
        private System.Windows.Forms.Label lblPriorFy;
        private System.Windows.Forms.Label lblCondYr;
        private System.Windows.Forms.ComboBox cbCurrFy;
        private System.Windows.Forms.Button btnFinish;
        private System.Windows.Forms.FolderBrowserDialog fbdPickDir;
        private System.Windows.Forms.Button btnPickDir;
        private System.Windows.Forms.TextBox tbDstDir;
        private System.Windows.Forms.Label lblDstDir;
        private System.Windows.Forms.OpenFileDialog ofdPickTemplate;
        private System.Windows.Forms.TextBox tbTemplateFile;
        private System.Windows.Forms.Button btnPickTemplate;
        private System.Windows.Forms.CheckBox chbTemplateFile;
        private System.Windows.Forms.ToolTip ttTemplateFile;
    }
}