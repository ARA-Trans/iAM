namespace RoadCare3
{
    partial class FormSimulationAttributes
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
			this.dgvSummary = new System.Windows.Forms.DataGridView();
			this.Properties = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dgvAttribute = new System.Windows.Forms.DataGridView();
			this.buttonCommit = new System.Windows.Forms.Button();
			this.buttonRun = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dgvSummary)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvAttribute)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvSummary
			// 
			this.dgvSummary.AllowUserToAddRows = false;
			this.dgvSummary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dgvSummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSummary.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Properties,
            this.Value});
			this.dgvSummary.Location = new System.Drawing.Point(1, 1);
			this.dgvSummary.Name = "dgvSummary";
			this.dgvSummary.RowHeadersVisible = false;
			this.dgvSummary.Size = new System.Drawing.Size(401, 321);
			this.dgvSummary.TabIndex = 0;
			this.dgvSummary.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSummary_CellEnter);
			this.dgvSummary.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvSummary_CellValidating);
			this.dgvSummary.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSummary_CellValueChanged);
			this.dgvSummary.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvSummary_EditingControlShowing);
			// 
			// Properties
			// 
			this.Properties.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Properties.HeaderText = "Property";
			this.Properties.Name = "Properties";
			// 
			// Value
			// 
			this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.Value.HeaderText = "Value";
			this.Value.Name = "Value";
			// 
			// dgvAttribute
			// 
			this.dgvAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dgvAttribute.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAttribute.Location = new System.Drawing.Point(1, 328);
			this.dgvAttribute.Name = "dgvAttribute";
			this.dgvAttribute.RowHeadersVisible = false;
			this.dgvAttribute.Size = new System.Drawing.Size(401, 294);
			this.dgvAttribute.TabIndex = 1;
			this.dgvAttribute.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAttribute_CellEnter);
			this.dgvAttribute.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAttribute_CellValueChanged);
			this.dgvAttribute.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvAttribute_DataError);
			this.dgvAttribute.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvAttribute_UserDeletedRow);
			// 
			// buttonCommit
			// 
			this.buttonCommit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonCommit.Location = new System.Drawing.Point(81, 597);
			this.buttonCommit.Name = "buttonCommit";
			this.buttonCommit.Size = new System.Drawing.Size(75, 23);
			this.buttonCommit.TabIndex = 2;
			this.buttonCommit.Text = "Commit";
			this.buttonCommit.UseVisualStyleBackColor = true;
			this.buttonCommit.Visible = false;
			// 
			// buttonRun
			// 
			this.buttonRun.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonRun.Location = new System.Drawing.Point(162, 597);
			this.buttonRun.Name = "buttonRun";
			this.buttonRun.Size = new System.Drawing.Size(75, 23);
			this.buttonRun.TabIndex = 3;
			this.buttonRun.Text = "Run Section";
			this.buttonRun.UseVisualStyleBackColor = true;
			this.buttonRun.Visible = false;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(246, 596);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Visible = false;
			// 
			// FormSimulationAttributes
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(403, 623);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonRun);
			this.Controls.Add(this.buttonCommit);
			this.Controls.Add(this.dgvAttribute);
			this.Controls.Add(this.dgvSummary);
			this.Name = "FormSimulationAttributes";
			this.Text = "FormSimulationAttributes";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSimulationAttributes_FormClosed);
			this.Load += new System.EventHandler(this.FormSimulationAttributes_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgvSummary)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvAttribute)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSummary;
        private System.Windows.Forms.DataGridViewTextBoxColumn Properties;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridView dgvAttribute;
        private System.Windows.Forms.Button buttonCommit;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Button buttonCancel;
    }
}