namespace RoadCare3
{
	partial class FormCompoundTreatment
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
			this.dgvCompoundTreatments = new System.Windows.Forms.DataGridView();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.textBoxDescription = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.dataGridViewCompoundTreatmentElements = new System.Windows.Forms.DataGridView();
			this.colCompoundTreatmentElement = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colAttributeFrom = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.colAttributeTo = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.colExtent = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colCriteria = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colCompoundTreatments = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colAffectedAttribute = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colCompoundTreatmentName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dgvCompoundTreatments)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewCompoundTreatmentElements)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvCompoundTreatments
			// 
			this.dgvCompoundTreatments.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvCompoundTreatments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvCompoundTreatments.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCompoundTreatments,
            this.colAffectedAttribute,
            this.colCompoundTreatmentName});
			this.dgvCompoundTreatments.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvCompoundTreatments.Location = new System.Drawing.Point(0, 0);
			this.dgvCompoundTreatments.Margin = new System.Windows.Forms.Padding(2);
			this.dgvCompoundTreatments.MultiSelect = false;
			this.dgvCompoundTreatments.Name = "dgvCompoundTreatments";
			this.dgvCompoundTreatments.RowHeadersWidth = 20;
			this.dgvCompoundTreatments.RowTemplate.Height = 24;
			this.dgvCompoundTreatments.Size = new System.Drawing.Size(296, 674);
			this.dgvCompoundTreatments.TabIndex = 9;
			this.dgvCompoundTreatments.SelectionChanged += new System.EventHandler(this.dgvCompoundTreatments_SelectionChanged);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.dgvCompoundTreatments);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.textBoxDescription);
			this.splitContainer1.Panel2.Controls.Add(this.label5);
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(987, 674);
			this.splitContainer1.SplitterDistance = 296;
			this.splitContainer1.TabIndex = 10;
			// 
			// textBoxDescription
			// 
			this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxDescription.Location = new System.Drawing.Point(69, 7);
			this.textBoxDescription.Margin = new System.Windows.Forms.Padding(2);
			this.textBoxDescription.Name = "textBoxDescription";
			this.textBoxDescription.Size = new System.Drawing.Size(607, 20);
			this.textBoxDescription.TabIndex = 28;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(2, 10);
			this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(63, 13);
			this.label5.TabIndex = 24;
			this.label5.Text = "Description:";
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.dataGridViewCompoundTreatmentElements);
			this.splitContainer2.Size = new System.Drawing.Size(687, 674);
			this.splitContainer2.SplitterDistance = 34;
			this.splitContainer2.TabIndex = 30;
			// 
			// dataGridViewCompoundTreatmentElements
			// 
			this.dataGridViewCompoundTreatmentElements.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dataGridViewCompoundTreatmentElements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridViewCompoundTreatmentElements.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCompoundTreatmentElement,
            this.colAttributeFrom,
            this.colAttributeTo,
            this.colExtent,
            this.colQuantity,
            this.colCriteria,
            this.colCost});
			this.dataGridViewCompoundTreatmentElements.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewCompoundTreatmentElements.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewCompoundTreatmentElements.Name = "dataGridViewCompoundTreatmentElements";
			this.dataGridViewCompoundTreatmentElements.Size = new System.Drawing.Size(687, 636);
			this.dataGridViewCompoundTreatmentElements.TabIndex = 0;
			this.dataGridViewCompoundTreatmentElements.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridViewCompoundTreatmentElements_UserAddedRow);
			this.dataGridViewCompoundTreatmentElements.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewCompoundTreatmentElements_CellDoubleClick);
			this.dataGridViewCompoundTreatmentElements.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridViewCompoundTreatmentElements_RowsAdded);
			this.dataGridViewCompoundTreatmentElements.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridViewCompoundTreatmentElements_DataError);
			this.dataGridViewCompoundTreatmentElements.SelectionChanged += new System.EventHandler(this.dataGridViewCompoundTreatmentElements_SelectionChanged);
			// 
			// colCompoundTreatmentElement
			// 
			this.colCompoundTreatmentElement.HeaderText = "Compound Treatment Element";
			this.colCompoundTreatmentElement.Name = "colCompoundTreatmentElement";
			this.colCompoundTreatmentElement.ReadOnly = true;
			this.colCompoundTreatmentElement.Visible = false;
			this.colCompoundTreatmentElement.Width = 175;
			// 
			// colAttributeFrom
			// 
			this.colAttributeFrom.HeaderText = "Attribute From";
			this.colAttributeFrom.Name = "colAttributeFrom";
			this.colAttributeFrom.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colAttributeFrom.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.colAttributeFrom.Width = 97;
			// 
			// colAttributeTo
			// 
			this.colAttributeTo.HeaderText = "Attribute To";
			this.colAttributeTo.Name = "colAttributeTo";
			this.colAttributeTo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colAttributeTo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.colAttributeTo.Width = 87;
			// 
			// colExtent
			// 
			this.colExtent.HeaderText = "Extent";
			this.colExtent.Name = "colExtent";
			this.colExtent.Width = 62;
			// 
			// colQuantity
			// 
			this.colQuantity.HeaderText = "Quantity";
			this.colQuantity.Name = "colQuantity";
			this.colQuantity.Width = 71;
			// 
			// colCriteria
			// 
			this.colCriteria.HeaderText = "Criteria";
			this.colCriteria.Name = "colCriteria";
			this.colCriteria.Width = 64;
			// 
			// colCost
			// 
			this.colCost.HeaderText = "Cost";
			this.colCost.Name = "colCost";
			this.colCost.Width = 53;
			// 
			// colCompoundTreatments
			// 
			this.colCompoundTreatments.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colCompoundTreatments.HeaderText = "Compound Treatments";
			this.colCompoundTreatments.Name = "colCompoundTreatments";
			this.colCompoundTreatments.Visible = false;
			// 
			// colAffectedAttribute
			// 
			this.colAffectedAttribute.HeaderText = "Effected Attribute";
			this.colAffectedAttribute.Name = "colAffectedAttribute";
			this.colAffectedAttribute.ReadOnly = true;
			this.colAffectedAttribute.Width = 105;
			// 
			// colCompoundTreatmentName
			// 
			this.colCompoundTreatmentName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colCompoundTreatmentName.HeaderText = "Compound Treatment Name";
			this.colCompoundTreatmentName.Name = "colCompoundTreatmentName";
			// 
			// FormCompoundTreatment
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(987, 674);
			this.Controls.Add(this.splitContainer1);
			this.Name = "FormCompoundTreatment";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TabText = "Compound Treatments";
			this.Text = "Compound Treatments";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCompoundTreatment_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.dgvCompoundTreatments)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewCompoundTreatmentElements)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dgvCompoundTreatments;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TextBox textBoxDescription;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.DataGridView dataGridViewCompoundTreatmentElements;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCompoundTreatmentElement;
		private System.Windows.Forms.DataGridViewComboBoxColumn colAttributeFrom;
		private System.Windows.Forms.DataGridViewComboBoxColumn colAttributeTo;
		private System.Windows.Forms.DataGridViewTextBoxColumn colExtent;
		private System.Windows.Forms.DataGridViewTextBoxColumn colQuantity;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCriteria;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCost;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCompoundTreatments;
		private System.Windows.Forms.DataGridViewTextBoxColumn colAffectedAttribute;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCompoundTreatmentName;
	}
}