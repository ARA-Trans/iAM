namespace RoadCare3
{
    partial class FormSegmentedConstruction
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
			this.dgvSection = new System.Windows.Forms.DataGridView();
			this.comboBoxRouteFacility = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxAttributeSearch = new System.Windows.Forms.TextBox();
			this.textBoxHistorySearch = new System.Windows.Forms.TextBox();
			this.buttonAdvancedSearch = new System.Windows.Forms.Button();
			this.buttonSearchHistory = new System.Windows.Forms.Button();
			this.dgvHistory = new System.Windows.Forms.DataGridView();
			this.pgProperties = new System.Windows.Forms.PropertyGrid();
			this.labelHistory = new System.Windows.Forms.Label();
			this.buttonUpdate = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.dgvSection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvSection
			// 
			this.dgvSection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.dgvSection.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSection.Location = new System.Drawing.Point(10, 71);
			this.dgvSection.Margin = new System.Windows.Forms.Padding(2);
			this.dgvSection.Name = "dgvSection";
			this.dgvSection.RowTemplate.Height = 24;
			this.dgvSection.Size = new System.Drawing.Size(206, 448);
			this.dgvSection.TabIndex = 0;
			// 
			// comboBoxRouteFacility
			// 
			this.comboBoxRouteFacility.FormattingEnabled = true;
			this.comboBoxRouteFacility.Location = new System.Drawing.Point(47, 46);
			this.comboBoxRouteFacility.Margin = new System.Windows.Forms.Padding(2);
			this.comboBoxRouteFacility.Name = "comboBoxRouteFacility";
			this.comboBoxRouteFacility.Size = new System.Drawing.Size(169, 21);
			this.comboBoxRouteFacility.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 48);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(42, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Facility:";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 529);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(86, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Attribute Search:";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 553);
			this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(141, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Construction History Search:";
			// 
			// textBoxAttributeSearch
			// 
			this.textBoxAttributeSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxAttributeSearch.Location = new System.Drawing.Point(102, 528);
			this.textBoxAttributeSearch.Margin = new System.Windows.Forms.Padding(2);
			this.textBoxAttributeSearch.Name = "textBoxAttributeSearch";
			this.textBoxAttributeSearch.Size = new System.Drawing.Size(680, 20);
			this.textBoxAttributeSearch.TabIndex = 5;
			// 
			// textBoxHistorySearch
			// 
			this.textBoxHistorySearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxHistorySearch.Location = new System.Drawing.Point(153, 552);
			this.textBoxHistorySearch.Margin = new System.Windows.Forms.Padding(2);
			this.textBoxHistorySearch.Name = "textBoxHistorySearch";
			this.textBoxHistorySearch.Size = new System.Drawing.Size(630, 20);
			this.textBoxHistorySearch.TabIndex = 6;
			// 
			// buttonAdvancedSearch
			// 
			this.buttonAdvancedSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAdvancedSearch.Location = new System.Drawing.Point(786, 528);
			this.buttonAdvancedSearch.Margin = new System.Windows.Forms.Padding(2);
			this.buttonAdvancedSearch.Name = "buttonAdvancedSearch";
			this.buttonAdvancedSearch.Size = new System.Drawing.Size(28, 19);
			this.buttonAdvancedSearch.TabIndex = 7;
			this.buttonAdvancedSearch.Text = "...";
			this.buttonAdvancedSearch.UseVisualStyleBackColor = true;
			// 
			// buttonSearchHistory
			// 
			this.buttonSearchHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSearchHistory.Location = new System.Drawing.Point(787, 551);
			this.buttonSearchHistory.Margin = new System.Windows.Forms.Padding(2);
			this.buttonSearchHistory.Name = "buttonSearchHistory";
			this.buttonSearchHistory.Size = new System.Drawing.Size(28, 19);
			this.buttonSearchHistory.TabIndex = 8;
			this.buttonSearchHistory.Text = "...";
			this.buttonSearchHistory.UseVisualStyleBackColor = true;
			// 
			// dgvHistory
			// 
			this.dgvHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvHistory.Location = new System.Drawing.Point(220, 71);
			this.dgvHistory.Margin = new System.Windows.Forms.Padding(2);
			this.dgvHistory.Name = "dgvHistory";
			this.dgvHistory.RowTemplate.Height = 24;
			this.dgvHistory.Size = new System.Drawing.Size(382, 448);
			this.dgvHistory.TabIndex = 9;
			// 
			// pgProperties
			// 
			this.pgProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pgProperties.Location = new System.Drawing.Point(607, 46);
			this.pgProperties.Name = "pgProperties";
			this.pgProperties.Size = new System.Drawing.Size(214, 473);
			this.pgProperties.TabIndex = 10;
			// 
			// labelHistory
			// 
			this.labelHistory.AutoSize = true;
			this.labelHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelHistory.Location = new System.Drawing.Point(42, 11);
			this.labelHistory.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelHistory.Name = "labelHistory";
			this.labelHistory.Size = new System.Drawing.Size(300, 26);
			this.labelHistory.TabIndex = 11;
			this.labelHistory.Text = "Construction History: Network";
			// 
			// buttonUpdate
			// 
			this.buttonUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonUpdate.Image = global::RoadCare3.Properties.Resources.update;
			this.buttonUpdate.Location = new System.Drawing.Point(590, 4);
			this.buttonUpdate.Margin = new System.Windows.Forms.Padding(2);
			this.buttonUpdate.Name = "buttonUpdate";
			this.buttonUpdate.Size = new System.Drawing.Size(39, 37);
			this.buttonUpdate.TabIndex = 15;
			this.buttonUpdate.UseVisualStyleBackColor = true;
			this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::RoadCare3.Properties.Resources.construction;
			this.pictureBox1.Location = new System.Drawing.Point(11, 11);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(24, 28);
			this.pictureBox1.TabIndex = 16;
			this.pictureBox1.TabStop = false;
			// 
			// FormSegmentedConstruction
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(824, 577);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.buttonUpdate);
			this.Controls.Add(this.labelHistory);
			this.Controls.Add(this.pgProperties);
			this.Controls.Add(this.dgvHistory);
			this.Controls.Add(this.buttonSearchHistory);
			this.Controls.Add(this.buttonAdvancedSearch);
			this.Controls.Add(this.textBoxHistorySearch);
			this.Controls.Add(this.textBoxAttributeSearch);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBoxRouteFacility);
			this.Controls.Add(this.dgvSection);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "FormSegmentedConstruction";
			this.TabText = "FormSegmentedConstruction";
			this.Text = "FormSegmentedConstruction";
			this.Load += new System.EventHandler(this.FormSegmentedConstruction_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSegmentedConstruction_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.dgvSection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSection;
        private System.Windows.Forms.ComboBox comboBoxRouteFacility;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxAttributeSearch;
        private System.Windows.Forms.TextBox textBoxHistorySearch;
        private System.Windows.Forms.Button buttonAdvancedSearch;
        private System.Windows.Forms.Button buttonSearchHistory;
        private System.Windows.Forms.DataGridView dgvHistory;
        private System.Windows.Forms.PropertyGrid pgProperties;
        private System.Windows.Forms.Label labelHistory;
        private System.Windows.Forms.Button buttonUpdate;
		private System.Windows.Forms.PictureBox pictureBox1;
    }
}