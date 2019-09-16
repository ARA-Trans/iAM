namespace RoadCare3
{
    partial class FormSimulationRunSingleSection
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
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonRun = new System.Windows.Forms.Button();
			this.buttonCommit = new System.Windows.Forms.Button();
			this.dgvAttribute = new System.Windows.Forms.DataGridView();
			this.dgvSummary = new System.Windows.Forms.DataGridView();
			this.Properties = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.labelError = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dgvAttribute)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSummary)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(241, 486);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 9;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Visible = false;
			// 
			// buttonRun
			// 
			this.buttonRun.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonRun.Location = new System.Drawing.Point(79, 487);
			this.buttonRun.Name = "buttonRun";
			this.buttonRun.Size = new System.Drawing.Size(75, 23);
			this.buttonRun.TabIndex = 8;
			this.buttonRun.Text = "Run Section";
			this.buttonRun.UseVisualStyleBackColor = true;
			this.buttonRun.Visible = false;
			this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
			// 
			// buttonCommit
			// 
			this.buttonCommit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.buttonCommit.Location = new System.Drawing.Point(160, 486);
			this.buttonCommit.Name = "buttonCommit";
			this.buttonCommit.Size = new System.Drawing.Size(75, 23);
			this.buttonCommit.TabIndex = 7;
			this.buttonCommit.Text = "Commit";
			this.buttonCommit.UseVisualStyleBackColor = true;
			this.buttonCommit.Visible = false;
			this.buttonCommit.Click += new System.EventHandler(this.buttonCommit_Click);
			// 
			// dgvAttribute
			// 
			this.dgvAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dgvAttribute.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAttribute.Location = new System.Drawing.Point(1, 219);
			this.dgvAttribute.Name = "dgvAttribute";
			this.dgvAttribute.RowHeadersVisible = false;
			this.dgvAttribute.Size = new System.Drawing.Size(390, 261);
			this.dgvAttribute.TabIndex = 6;
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
			this.dgvSummary.Location = new System.Drawing.Point(1, 6);
			this.dgvSummary.Name = "dgvSummary";
			this.dgvSummary.RowHeadersVisible = false;
			this.dgvSummary.Size = new System.Drawing.Size(390, 204);
			this.dgvSummary.TabIndex = 5;
			this.dgvSummary.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSummary_CellValueChanged);
			this.dgvSummary.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvSummary_CellValidating);
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
			// labelError
			// 
			this.labelError.AutoSize = true;
			this.labelError.ForeColor = System.Drawing.Color.Red;
			this.labelError.Location = new System.Drawing.Point(13, 512);
			this.labelError.Name = "labelError";
			this.labelError.Size = new System.Drawing.Size(32, 13);
			this.labelError.TabIndex = 10;
			this.labelError.Text = "Error:";
			this.labelError.Visible = false;
			// 
			// FormSimulationRunSingleSection
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(394, 541);
			this.Controls.Add(this.labelError);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonRun);
			this.Controls.Add(this.buttonCommit);
			this.Controls.Add(this.dgvAttribute);
			this.Controls.Add(this.dgvSummary);
			this.Name = "FormSimulationRunSingleSection";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Commit/Run Single Section";
			this.Load += new System.EventHandler(this.FormSimulationRunSingleSection_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgvAttribute)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSummary)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Button buttonCommit;
        private System.Windows.Forms.DataGridView dgvAttribute;
        private System.Windows.Forms.DataGridView dgvSummary;
        private System.Windows.Forms.DataGridViewTextBoxColumn Properties;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.Label labelError;
    }
}