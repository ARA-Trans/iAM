namespace RoadCare3
{
    partial class FormSummaryCheck
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonCalculate = new System.Windows.Forms.Button();
			this.textBoxYear = new System.Windows.Forms.TextBox();
			this.textBoxCriteria = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.comboBoxSummaryType = new System.Windows.Forms.ComboBox();
			this.comboBoxAttribute = new System.Windows.Forms.ComboBox();
			this.comboBoxMethod = new System.Windows.Forms.ComboBox();
			this.comboBoxNetwork = new System.Windows.Forms.ComboBox();
			this.comboBoxSimulation = new System.Windows.Forms.ComboBox();
			this.dgvSolution = new System.Windows.Forms.DataGridView();
			this.Attribute = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgvLevelSummary = new System.Windows.Forms.DataGridView();
			this.Range = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Area = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dgvSolution)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvLevelSummary)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 86);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(58, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "NetworkID";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 131);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "SimulationID (optional)";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 174);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(46, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Attribute";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(13, 211);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(43, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Method";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(13, 256);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(133, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "Year (optional for Network)";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(13, 294);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(85, 13);
			this.label6.TabIndex = 5;
			this.label6.Text = "Criteria (optional)";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(296, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(45, 13);
			this.label7.TabIndex = 6;
			this.label7.Text = "Solution";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(158, 574);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonCalculate
			// 
			this.buttonCalculate.Location = new System.Drawing.Point(49, 574);
			this.buttonCalculate.Name = "buttonCalculate";
			this.buttonCalculate.Size = new System.Drawing.Size(75, 23);
			this.buttonCalculate.TabIndex = 8;
			this.buttonCalculate.Text = "Calculate";
			this.buttonCalculate.UseVisualStyleBackColor = true;
			this.buttonCalculate.Click += new System.EventHandler(this.buttonCalculate_Click);
			// 
			// textBoxYear
			// 
			this.textBoxYear.Location = new System.Drawing.Point(180, 256);
			this.textBoxYear.Name = "textBoxYear";
			this.textBoxYear.Size = new System.Drawing.Size(100, 20);
			this.textBoxYear.TabIndex = 11;
			// 
			// textBoxCriteria
			// 
			this.textBoxCriteria.Location = new System.Drawing.Point(12, 310);
			this.textBoxCriteria.Multiline = true;
			this.textBoxCriteria.Name = "textBoxCriteria";
			this.textBoxCriteria.Size = new System.Drawing.Size(268, 116);
			this.textBoxCriteria.TabIndex = 12;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(13, 49);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(77, 13);
			this.label8.TabIndex = 14;
			this.label8.Text = "Summary Type";
			// 
			// comboBoxSummaryType
			// 
			this.comboBoxSummaryType.FormattingEnabled = true;
			this.comboBoxSummaryType.Items.AddRange(new object[] {
            "Number Summary",
            "Area Summary"});
			this.comboBoxSummaryType.Location = new System.Drawing.Point(137, 40);
			this.comboBoxSummaryType.Name = "comboBoxSummaryType";
			this.comboBoxSummaryType.Size = new System.Drawing.Size(143, 21);
			this.comboBoxSummaryType.TabIndex = 15;
			this.comboBoxSummaryType.SelectedIndexChanged += new System.EventHandler(this.comboBoxSummaryType_SelectedIndexChanged);
			// 
			// comboBoxAttribute
			// 
			this.comboBoxAttribute.FormattingEnabled = true;
			this.comboBoxAttribute.Location = new System.Drawing.Point(137, 172);
			this.comboBoxAttribute.Name = "comboBoxAttribute";
			this.comboBoxAttribute.Size = new System.Drawing.Size(143, 21);
			this.comboBoxAttribute.TabIndex = 16;
			// 
			// comboBoxMethod
			// 
			this.comboBoxMethod.FormattingEnabled = true;
			this.comboBoxMethod.Items.AddRange(new object[] {
            "SUM",
            "AVG",
            "MIN",
            "MAX"});
			this.comboBoxMethod.Location = new System.Drawing.Point(137, 205);
			this.comboBoxMethod.Name = "comboBoxMethod";
			this.comboBoxMethod.Size = new System.Drawing.Size(143, 21);
			this.comboBoxMethod.TabIndex = 17;
			// 
			// comboBoxNetwork
			// 
			this.comboBoxNetwork.FormattingEnabled = true;
			this.comboBoxNetwork.Location = new System.Drawing.Point(136, 83);
			this.comboBoxNetwork.Name = "comboBoxNetwork";
			this.comboBoxNetwork.Size = new System.Drawing.Size(143, 21);
			this.comboBoxNetwork.TabIndex = 18;
			this.comboBoxNetwork.SelectedIndexChanged += new System.EventHandler(this.comboBoxNetwork_SelectedIndexChanged);
			// 
			// comboBoxSimulation
			// 
			this.comboBoxSimulation.FormattingEnabled = true;
			this.comboBoxSimulation.Location = new System.Drawing.Point(137, 128);
			this.comboBoxSimulation.Name = "comboBoxSimulation";
			this.comboBoxSimulation.Size = new System.Drawing.Size(143, 21);
			this.comboBoxSimulation.TabIndex = 19;
			// 
			// dgvSolution
			// 
			this.dgvSolution.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSolution.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Attribute,
            this.Value});
			this.dgvSolution.Location = new System.Drawing.Point(299, 37);
			this.dgvSolution.Name = "dgvSolution";
			this.dgvSolution.RowHeadersVisible = false;
			this.dgvSolution.Size = new System.Drawing.Size(268, 560);
			this.dgvSolution.TabIndex = 20;
			// 
			// Attribute
			// 
			this.Attribute.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Attribute.HeaderText = "Attribute";
			this.Attribute.Name = "Attribute";
			// 
			// Value
			// 
			this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Value.HeaderText = "Value";
			this.Value.Name = "Value";
			// 
			// dgvLevelSummary
			// 
			this.dgvLevelSummary.AllowUserToAddRows = false;
			this.dgvLevelSummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvLevelSummary.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Range,
            this.Area});
			this.dgvLevelSummary.Location = new System.Drawing.Point(592, 40);
			this.dgvLevelSummary.Name = "dgvLevelSummary";
			this.dgvLevelSummary.RowHeadersVisible = false;
			this.dgvLevelSummary.Size = new System.Drawing.Size(360, 557);
			this.dgvLevelSummary.TabIndex = 21;
			// 
			// Range
			// 
			this.Range.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Range.HeaderText = "Range";
			this.Range.Name = "Range";
			// 
			// Area
			// 
			this.Area.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Area.HeaderText = "Area/Percent";
			this.Area.Name = "Area";
			// 
			// FormSummaryCheck
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(964, 648);
			this.Controls.Add(this.dgvLevelSummary);
			this.Controls.Add(this.dgvSolution);
			this.Controls.Add(this.comboBoxSimulation);
			this.Controls.Add(this.comboBoxNetwork);
			this.Controls.Add(this.comboBoxMethod);
			this.Controls.Add(this.comboBoxAttribute);
			this.Controls.Add(this.comboBoxSummaryType);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textBoxCriteria);
			this.Controls.Add(this.textBoxYear);
			this.Controls.Add(this.buttonCalculate);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "FormSummaryCheck";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FormSummaryCheck";
			this.Load += new System.EventHandler(this.FormSummaryCheck_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgvSolution)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvLevelSummary)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonCalculate;
        private System.Windows.Forms.TextBox textBoxYear;
        private System.Windows.Forms.TextBox textBoxCriteria;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBoxSummaryType;
        private System.Windows.Forms.ComboBox comboBoxAttribute;
        private System.Windows.Forms.ComboBox comboBoxMethod;
        private System.Windows.Forms.ComboBox comboBoxNetwork;
        private System.Windows.Forms.ComboBox comboBoxSimulation;
        private System.Windows.Forms.DataGridView dgvSolution;
        private System.Windows.Forms.DataGridViewTextBoxColumn Attribute;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridView dgvLevelSummary;
        private System.Windows.Forms.DataGridViewTextBoxColumn Range;
        private System.Windows.Forms.DataGridViewTextBoxColumn Area;
    }
}