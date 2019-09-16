namespace RoadCare3
{
    partial class FormSimulationResults
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
            this.dgvResultCommit = new System.Windows.Forms.DataGridView();
            this.contextMenuCommitResult = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.Cut = new System.Windows.Forms.ToolStripMenuItem();
            this.Paste = new System.Windows.Forms.ToolStripMenuItem();
            this.Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.commitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxAdvanceSearch = new System.Windows.Forms.TextBox();
            this.buttonAdvancedSearch = new System.Windows.Forms.Button();
            this.labelRouteFacility = new System.Windows.Forms.Label();
            this.labelAdvancedSearch = new System.Windows.Forms.Label();
            this.comboBoxAttributeValue = new System.Windows.Forms.ComboBox();
            this.comboBoxRouteFacilty = new System.Windows.Forms.ComboBox();
            this.comboBoxFilterAttribute = new System.Windows.Forms.ComboBox();
            this.checkBoxCustomFilter = new System.Windows.Forms.CheckBox();
            this.labelResult = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelAnalysis = new System.Windows.Forms.Label();
            this.cbYears = new System.Windows.Forms.ComboBox();
            this.cbStartYear = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timerSimulation = new System.Windows.Forms.Timer(this.components);
            this.btnCommitExport = new System.Windows.Forms.Button();
            this.btnCommitImport = new System.Windows.Forms.Button();
            this.btnARAN = new System.Windows.Forms.Button();
            this.menuStripResultsOptions = new System.Windows.Forms.MenuStrip();
            this.resultsOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayAttributesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResultCommit)).BeginInit();
            this.contextMenuCommitResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStripResultsOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvResultCommit
            // 
            this.dgvResultCommit.AllowUserToAddRows = false;
            this.dgvResultCommit.AllowUserToDeleteRows = false;
            this.dgvResultCommit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvResultCommit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResultCommit.ContextMenuStrip = this.contextMenuCommitResult;
            this.dgvResultCommit.Location = new System.Drawing.Point(2, 95);
            this.dgvResultCommit.Name = "dgvResultCommit";
            this.dgvResultCommit.RowHeadersVisible = false;
            this.dgvResultCommit.Size = new System.Drawing.Size(839, 438);
            this.dgvResultCommit.TabIndex = 0;
            this.dgvResultCommit.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvResultCommit_CellDoubleClick);
            this.dgvResultCommit.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvResultCommit_CellEnter);
            this.dgvResultCommit.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvResultCommit_CellMouseEnter);
            this.dgvResultCommit.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.dgvResultCommit_CellToolTipTextNeeded);
            this.dgvResultCommit.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgvResultCommit_KeyUp);
            // 
            // contextMenuCommitResult
            // 
            this.contextMenuCommitResult.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Copy,
            this.Cut,
            this.Paste,
            this.Delete,
            this.commitToolStripMenuItem});
            this.contextMenuCommitResult.Name = "contextMenuCommitResult";
            this.contextMenuCommitResult.Size = new System.Drawing.Size(119, 114);
            // 
            // Copy
            // 
            this.Copy.Name = "Copy";
            this.Copy.Size = new System.Drawing.Size(118, 22);
            this.Copy.Text = "&Copy";
            this.Copy.Click += new System.EventHandler(this.Copy_Click);
            // 
            // Cut
            // 
            this.Cut.Enabled = false;
            this.Cut.Name = "Cut";
            this.Cut.Size = new System.Drawing.Size(118, 22);
            this.Cut.Text = "Cut";
            this.Cut.Click += new System.EventHandler(this.Cut_Click);
            // 
            // Paste
            // 
            this.Paste.Name = "Paste";
            this.Paste.Size = new System.Drawing.Size(118, 22);
            this.Paste.Text = "Paste";
            this.Paste.Click += new System.EventHandler(this.Paste_Click);
            // 
            // Delete
            // 
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(118, 22);
            this.Delete.Text = "Delete";
            this.Delete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // commitToolStripMenuItem
            // 
            this.commitToolStripMenuItem.Name = "commitToolStripMenuItem";
            this.commitToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.commitToolStripMenuItem.Text = "Commit";
            this.commitToolStripMenuItem.Click += new System.EventHandler(this.commitToolStripMenuItem_Click);
            // 
            // textBoxAdvanceSearch
            // 
            this.textBoxAdvanceSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAdvanceSearch.Location = new System.Drawing.Point(378, 70);
            this.textBoxAdvanceSearch.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxAdvanceSearch.Name = "textBoxAdvanceSearch";
            this.textBoxAdvanceSearch.Size = new System.Drawing.Size(429, 20);
            this.textBoxAdvanceSearch.TabIndex = 23;
            // 
            // buttonAdvancedSearch
            // 
            this.buttonAdvancedSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAdvancedSearch.Location = new System.Drawing.Point(811, 71);
            this.buttonAdvancedSearch.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAdvancedSearch.Name = "buttonAdvancedSearch";
            this.buttonAdvancedSearch.Size = new System.Drawing.Size(28, 19);
            this.buttonAdvancedSearch.TabIndex = 24;
            this.buttonAdvancedSearch.Text = "...";
            this.buttonAdvancedSearch.UseVisualStyleBackColor = true;
            this.buttonAdvancedSearch.Click += new System.EventHandler(this.buttonAdvancedSearch_Click);
            // 
            // labelRouteFacility
            // 
            this.labelRouteFacility.AutoSize = true;
            this.labelRouteFacility.Location = new System.Drawing.Point(333, 47);
            this.labelRouteFacility.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelRouteFacility.Name = "labelRouteFacility";
            this.labelRouteFacility.Size = new System.Drawing.Size(42, 13);
            this.labelRouteFacility.TabIndex = 21;
            this.labelRouteFacility.Text = "Facility:";
            // 
            // labelAdvancedSearch
            // 
            this.labelAdvancedSearch.AutoSize = true;
            this.labelAdvancedSearch.Location = new System.Drawing.Point(278, 73);
            this.labelAdvancedSearch.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAdvancedSearch.Name = "labelAdvancedSearch";
            this.labelAdvancedSearch.Size = new System.Drawing.Size(96, 13);
            this.labelAdvancedSearch.TabIndex = 22;
            this.labelAdvancedSearch.Text = "Advanced Search:";
            // 
            // comboBoxAttributeValue
            // 
            this.comboBoxAttributeValue.FormattingEnabled = true;
            this.comboBoxAttributeValue.Location = new System.Drawing.Point(177, 45);
            this.comboBoxAttributeValue.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxAttributeValue.Name = "comboBoxAttributeValue";
            this.comboBoxAttributeValue.Size = new System.Drawing.Size(152, 21);
            this.comboBoxAttributeValue.TabIndex = 20;
            this.comboBoxAttributeValue.Visible = false;
            // 
            // comboBoxRouteFacilty
            // 
            this.comboBoxRouteFacilty.FormattingEnabled = true;
            this.comboBoxRouteFacilty.Location = new System.Drawing.Point(378, 44);
            this.comboBoxRouteFacilty.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxRouteFacilty.Name = "comboBoxRouteFacilty";
            this.comboBoxRouteFacilty.Size = new System.Drawing.Size(157, 21);
            this.comboBoxRouteFacilty.TabIndex = 19;
            this.comboBoxRouteFacilty.SelectedIndexChanged += new System.EventHandler(this.comboBoxRouteFacilty_SelectedIndexChanged);
            // 
            // comboBoxFilterAttribute
            // 
            this.comboBoxFilterAttribute.FormattingEnabled = true;
            this.comboBoxFilterAttribute.Location = new System.Drawing.Point(31, 46);
            this.comboBoxFilterAttribute.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxFilterAttribute.Name = "comboBoxFilterAttribute";
            this.comboBoxFilterAttribute.Size = new System.Drawing.Size(143, 21);
            this.comboBoxFilterAttribute.TabIndex = 18;
            this.comboBoxFilterAttribute.Visible = false;
            // 
            // checkBoxCustomFilter
            // 
            this.checkBoxCustomFilter.AutoSize = true;
            this.checkBoxCustomFilter.Location = new System.Drawing.Point(11, 48);
            this.checkBoxCustomFilter.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxCustomFilter.Name = "checkBoxCustomFilter";
            this.checkBoxCustomFilter.Size = new System.Drawing.Size(118, 17);
            this.checkBoxCustomFilter.TabIndex = 17;
            this.checkBoxCustomFilter.Text = "Enable custom filter";
            this.checkBoxCustomFilter.UseVisualStyleBackColor = true;
            this.checkBoxCustomFilter.Visible = false;
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResult.Location = new System.Drawing.Point(52, 9);
            this.labelResult.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(227, 26);
            this.labelResult.TabIndex = 16;
            this.labelResult.Text = "Result View - Network";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 73);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Start Year:";
            // 
            // labelAnalysis
            // 
            this.labelAnalysis.AutoSize = true;
            this.labelAnalysis.Location = new System.Drawing.Point(132, 73);
            this.labelAnalysis.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAnalysis.Name = "labelAnalysis";
            this.labelAnalysis.Size = new System.Drawing.Size(81, 13);
            this.labelAnalysis.TabIndex = 27;
            this.labelAnalysis.Text = "Analysis Period:";
            // 
            // cbYears
            // 
            this.cbYears.FormattingEnabled = true;
            this.cbYears.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40"});
            this.cbYears.Location = new System.Drawing.Point(216, 70);
            this.cbYears.Margin = new System.Windows.Forms.Padding(2);
            this.cbYears.Name = "cbYears";
            this.cbYears.Size = new System.Drawing.Size(52, 21);
            this.cbYears.TabIndex = 26;
            this.cbYears.SelectedIndexChanged += new System.EventHandler(this.cbYears_SelectedIndexChanged);
            // 
            // cbStartYear
            // 
            this.cbStartYear.FormattingEnabled = true;
            this.cbStartYear.Items.AddRange(new object[] {
            "2000",
            "2001",
            "2002",
            "2003",
            "2004",
            "2005",
            "2006",
            "2007",
            "2008",
            "2009",
            "2010",
            "2011",
            "2012",
            "2013",
            "2014",
            "2015",
            "2016",
            "2017",
            "2018",
            "2019",
            "2020",
            "2021",
            "2022",
            "2023",
            "2024",
            "2025",
            "2026",
            "2027",
            "2028",
            "2029",
            "2030",
            "2031",
            "2032",
            "2033",
            "2034",
            "2035",
            "2036",
            "2037",
            "2038",
            "2039",
            "2040",
            "2041",
            "2042",
            "2043",
            "2044",
            "2045",
            "2046",
            "2047",
            "2048",
            "2049",
            "2050"});
            this.cbStartYear.Location = new System.Drawing.Point(65, 70);
            this.cbStartYear.Name = "cbStartYear";
            this.cbStartYear.Size = new System.Drawing.Size(60, 21);
            this.cbStartYear.TabIndex = 29;
            this.cbStartYear.SelectedIndexChanged += new System.EventHandler(this.cbStartYear_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RoadCare3.Properties.Resources.bigpink;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(35, 29);
            this.pictureBox1.TabIndex = 30;
            this.pictureBox1.TabStop = false;
            // 
            // timerSimulation
            // 
            this.timerSimulation.Tick += new System.EventHandler(this.timerSimulation_Tick);
            // 
            // btnCommitExport
            // 
            this.btnCommitExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCommitExport.Location = new System.Drawing.Point(660, 11);
            this.btnCommitExport.Name = "btnCommitExport";
            this.btnCommitExport.Size = new System.Drawing.Size(169, 24);
            this.btnCommitExport.TabIndex = 31;
            this.btnCommitExport.Text = "Export Committed Projects";
            this.btnCommitExport.UseVisualStyleBackColor = true;
            this.btnCommitExport.Visible = false;
            this.btnCommitExport.Click += new System.EventHandler(this.btnCommitExport_Click);
            // 
            // btnCommitImport
            // 
            this.btnCommitImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCommitImport.Location = new System.Drawing.Point(660, 36);
            this.btnCommitImport.Name = "btnCommitImport";
            this.btnCommitImport.Size = new System.Drawing.Size(169, 24);
            this.btnCommitImport.TabIndex = 32;
            this.btnCommitImport.Text = "Import Committed Projects";
            this.btnCommitImport.UseVisualStyleBackColor = true;
            this.btnCommitImport.Visible = false;
            this.btnCommitImport.Click += new System.EventHandler(this.btnCommitImport_Click);
            // 
            // btnARAN
            // 
            this.btnARAN.Location = new System.Drawing.Point(540, 42);
            this.btnARAN.Name = "btnARAN";
            this.btnARAN.Size = new System.Drawing.Size(75, 23);
            this.btnARAN.TabIndex = 33;
            this.btnARAN.Text = "ARAN View";
            this.btnARAN.UseVisualStyleBackColor = true;
            this.btnARAN.Visible = false;
            this.btnARAN.Click += new System.EventHandler(this.btnARAN_Click);
            // 
            // menuStripResultsOptions
            // 
            this.menuStripResultsOptions.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStripResultsOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resultsOptionsToolStripMenuItem});
            this.menuStripResultsOptions.Location = new System.Drawing.Point(2, 95);
            this.menuStripResultsOptions.Name = "menuStripResultsOptions";
            this.menuStripResultsOptions.Size = new System.Drawing.Size(109, 24);
            this.menuStripResultsOptions.TabIndex = 34;
            this.menuStripResultsOptions.Text = "Results Options";
            // 
            // resultsOptionsToolStripMenuItem
            // 
            this.resultsOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayAttributesToolStripMenuItem});
            this.resultsOptionsToolStripMenuItem.Name = "resultsOptionsToolStripMenuItem";
            this.resultsOptionsToolStripMenuItem.Size = new System.Drawing.Size(101, 20);
            this.resultsOptionsToolStripMenuItem.Text = "Results Options";
            // 
            // displayAttributesToolStripMenuItem
            // 
            this.displayAttributesToolStripMenuItem.Name = "displayAttributesToolStripMenuItem";
            this.displayAttributesToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.displayAttributesToolStripMenuItem.Text = "Display Attributes...";
            this.displayAttributesToolStripMenuItem.Click += new System.EventHandler(this.displayAttributesToolStripMenuItem_Click);
            // 
            // FormSimulationResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 533);
            this.Controls.Add(this.btnARAN);
            this.Controls.Add(this.btnCommitImport);
            this.Controls.Add(this.btnCommitExport);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.cbStartYear);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelAnalysis);
            this.Controls.Add(this.cbYears);
            this.Controls.Add(this.textBoxAdvanceSearch);
            this.Controls.Add(this.labelRouteFacility);
            this.Controls.Add(this.buttonAdvancedSearch);
            this.Controls.Add(this.labelAdvancedSearch);
            this.Controls.Add(this.comboBoxAttributeValue);
            this.Controls.Add(this.comboBoxRouteFacilty);
            this.Controls.Add(this.comboBoxFilterAttribute);
            this.Controls.Add(this.checkBoxCustomFilter);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.dgvResultCommit);
            this.Controls.Add(this.menuStripResultsOptions);
            this.MainMenuStrip = this.menuStripResultsOptions;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormSimulationResults";
            this.TabText = "FormSimulation";
            this.Text = "FormSimulation";
            this.Activated += new System.EventHandler(this.FormSimulationResults_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSimulationResults_FormClosed);
            this.Load += new System.EventHandler(this.FormSimulation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResultCommit)).EndInit();
            this.contextMenuCommitResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStripResultsOptions.ResumeLayout(false);
            this.menuStripResultsOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvResultCommit;
        private System.Windows.Forms.TextBox textBoxAdvanceSearch;
        private System.Windows.Forms.Button buttonAdvancedSearch;
        private System.Windows.Forms.Label labelRouteFacility;
        private System.Windows.Forms.Label labelAdvancedSearch;
        private System.Windows.Forms.ComboBox comboBoxAttributeValue;
        private System.Windows.Forms.ComboBox comboBoxRouteFacilty;
        private System.Windows.Forms.ComboBox comboBoxFilterAttribute;
        private System.Windows.Forms.CheckBox checkBoxCustomFilter;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelAnalysis;
        private System.Windows.Forms.ComboBox cbYears;
        private System.Windows.Forms.ComboBox cbStartYear;
        private System.Windows.Forms.ContextMenuStrip contextMenuCommitResult;
        private System.Windows.Forms.ToolStripMenuItem Copy;
        private System.Windows.Forms.ToolStripMenuItem Cut;
        private System.Windows.Forms.ToolStripMenuItem Paste;
        private System.Windows.Forms.ToolStripMenuItem Delete;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem commitToolStripMenuItem;
        private System.Windows.Forms.Timer timerSimulation;
		private System.Windows.Forms.Button btnCommitExport;
		private System.Windows.Forms.Button btnCommitImport;
		private System.Windows.Forms.Button btnARAN;
        private System.Windows.Forms.MenuStrip menuStripResultsOptions;
        private System.Windows.Forms.ToolStripMenuItem resultsOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayAttributesToolStripMenuItem;
    }
}