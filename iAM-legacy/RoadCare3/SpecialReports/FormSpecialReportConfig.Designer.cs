namespace RoadCare3
{
    partial class FormSpecialReportConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSpecialReportConfig));
            this.cbNets = new System.Windows.Forms.ComboBox();
            this.lblNetwork = new System.Windows.Forms.Label();
            this.lblSimulation = new System.Windows.Forms.Label();
            this.cbSims = new System.Windows.Forms.ComboBox();
            this.lblReportType = new System.Windows.Forms.Label();
            this.cbReps = new System.Windows.Forms.ComboBox();
            this.btnGenRep = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.fbdSimpleGen = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // cbNets
            // 
            this.cbNets.FormattingEnabled = true;
            this.cbNets.Location = new System.Drawing.Point(12, 28);
            this.cbNets.Name = "cbNets";
            this.cbNets.Size = new System.Drawing.Size(360, 21);
            this.cbNets.TabIndex = 0;
            this.cbNets.SelectedIndexChanged += new System.EventHandler(this.cbNets_SelectedIndexChanged);
            // 
            // lblNetwork
            // 
            this.lblNetwork.AutoSize = true;
            this.lblNetwork.Location = new System.Drawing.Point(12, 12);
            this.lblNetwork.Name = "lblNetwork";
            this.lblNetwork.Size = new System.Drawing.Size(47, 13);
            this.lblNetwork.TabIndex = 0;
            this.lblNetwork.Text = "Network";
            // 
            // lblSimulation
            // 
            this.lblSimulation.AutoSize = true;
            this.lblSimulation.Location = new System.Drawing.Point(12, 57);
            this.lblSimulation.Name = "lblSimulation";
            this.lblSimulation.Size = new System.Drawing.Size(55, 13);
            this.lblSimulation.TabIndex = 0;
            this.lblSimulation.Text = "Simulation";
            // 
            // cbSims
            // 
            this.cbSims.FormattingEnabled = true;
            this.cbSims.Location = new System.Drawing.Point(12, 73);
            this.cbSims.Name = "cbSims";
            this.cbSims.Size = new System.Drawing.Size(360, 21);
            this.cbSims.TabIndex = 1;
            this.cbSims.SelectedIndexChanged += new System.EventHandler(this.cbSims_SelectedIndexChanged);
            // 
            // lblReportType
            // 
            this.lblReportType.AutoSize = true;
            this.lblReportType.Location = new System.Drawing.Point(12, 102);
            this.lblReportType.Name = "lblReportType";
            this.lblReportType.Size = new System.Drawing.Size(66, 13);
            this.lblReportType.TabIndex = 0;
            this.lblReportType.Text = "Report Type";
            // 
            // cbReps
            // 
            this.cbReps.FormattingEnabled = true;
            this.cbReps.Location = new System.Drawing.Point(12, 118);
            this.cbReps.Name = "cbReps";
            this.cbReps.Size = new System.Drawing.Size(360, 21);
            this.cbReps.TabIndex = 2;
            // 
            // btnGenRep
            // 
            this.btnGenRep.Location = new System.Drawing.Point(12, 153);
            this.btnGenRep.Name = "btnGenRep";
            this.btnGenRep.Size = new System.Drawing.Size(254, 23);
            this.btnGenRep.TabIndex = 3;
            this.btnGenRep.Text = "Generate Report";
            this.btnGenRep.UseVisualStyleBackColor = true;
            this.btnGenRep.Click += new System.EventHandler(this.btnGenRep_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(272, 153);
            this.progressBar.MarqueeAnimationSpeed = 0;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 0;
            // 
            // FormSpecialReportConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 188);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnGenRep);
            this.Controls.Add(this.lblReportType);
            this.Controls.Add(this.cbReps);
            this.Controls.Add(this.lblSimulation);
            this.Controls.Add(this.cbSims);
            this.Controls.Add(this.lblNetwork);
            this.Controls.Add(this.cbNets);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSpecialReportConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Special Report Generation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbNets;
        private System.Windows.Forms.Label lblNetwork;
        private System.Windows.Forms.Label lblSimulation;
        private System.Windows.Forms.ComboBox cbSims;
        private System.Windows.Forms.Label lblReportType;
        private System.Windows.Forms.ComboBox cbReps;
        private System.Windows.Forms.Button btnGenRep;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.FolderBrowserDialog fbdSimpleGen;
    }
}