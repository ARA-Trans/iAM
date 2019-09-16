namespace RoadCare3
{
	partial class AttributeTab
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
			this.comboBoxAttributeYears = new System.Windows.Forms.ComboBox();
			this.lblYear = new System.Windows.Forms.Label();
			this.dgvAttributeValues = new System.Windows.Forms.DataGridView();
			this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dgvAttributeValues)).BeginInit();
			this.SuspendLayout();
			// 
			// comboBoxAttributeYears
			// 
			this.comboBoxAttributeYears.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxAttributeYears.FormattingEnabled = true;
			this.comboBoxAttributeYears.Location = new System.Drawing.Point(50, 14);
			this.comboBoxAttributeYears.Name = "comboBoxAttributeYears";
			this.comboBoxAttributeYears.Size = new System.Drawing.Size(253, 21);
			this.comboBoxAttributeYears.TabIndex = 18;
			this.comboBoxAttributeYears.SelectedIndexChanged += new System.EventHandler(this.comboBoxAttributeYears_SelectedIndexChanged);
			// 
			// lblYear
			// 
			this.lblYear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblYear.AutoSize = true;
			this.lblYear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblYear.Location = new System.Drawing.Point(4, 14);
			this.lblYear.Name = "lblYear";
			this.lblYear.Size = new System.Drawing.Size(40, 16);
			this.lblYear.TabIndex = 17;
			this.lblYear.Text = "Year:";
			// 
			// dgvAttributeValues
			// 
			this.dgvAttributeValues.AllowUserToAddRows = false;
			this.dgvAttributeValues.AllowUserToDeleteRows = false;
			this.dgvAttributeValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dgvAttributeValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAttributeValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
			this.dgvAttributeValues.Location = new System.Drawing.Point(1, 39);
			this.dgvAttributeValues.Name = "dgvAttributeValues";
			this.dgvAttributeValues.ReadOnly = true;
			this.dgvAttributeValues.RowHeadersVisible = false;
			this.dgvAttributeValues.Size = new System.Drawing.Size(302, 735);
			this.dgvAttributeValues.TabIndex = 16;
			// 
			// dataGridViewTextBoxColumn1
			// 
			this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.dataGridViewTextBoxColumn1.HeaderText = "Attribute";
			this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
			this.dataGridViewTextBoxColumn1.ReadOnly = true;
			// 
			// dataGridViewTextBoxColumn2
			// 
			this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.dataGridViewTextBoxColumn2.HeaderText = "Value";
			this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
			this.dataGridViewTextBoxColumn2.ReadOnly = true;
			// 
			// AttributeTab
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(305, 778);
			this.Controls.Add(this.comboBoxAttributeYears);
			this.Controls.Add(this.lblYear);
			this.Controls.Add(this.dgvAttributeValues);
			this.Name = "AttributeTab";
			this.TabText = "Attribute Manager";
			this.Text = "Attribute Manager";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AttributeTab_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.dgvAttributeValues)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBoxAttributeYears;
		private System.Windows.Forms.Label lblYear;
		private System.Windows.Forms.DataGridView dgvAttributeValues;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
	}
}