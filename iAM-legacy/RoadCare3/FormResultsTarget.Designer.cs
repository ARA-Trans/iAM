namespace RoadCare3
{
    partial class FormResultsTarget
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
            this.dgvTarget = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTargetCriteria = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTarget)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvTarget
            // 
            this.dgvTarget.AllowUserToAddRows = false;
            this.dgvTarget.AllowUserToDeleteRows = false;
            this.dgvTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTarget.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTarget.Location = new System.Drawing.Point(0, 29);
            this.dgvTarget.Name = "dgvTarget";
            this.dgvTarget.RowHeadersVisible = false;
            this.dgvTarget.Size = new System.Drawing.Size(834, 411);
            this.dgvTarget.TabIndex = 1;
            this.dgvTarget.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTarget_CellEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Target Criteria:";
            // 
            // textBoxTargetCriteria
            // 
            this.textBoxTargetCriteria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTargetCriteria.Location = new System.Drawing.Point(87, 6);
            this.textBoxTargetCriteria.Name = "textBoxTargetCriteria";
            this.textBoxTargetCriteria.Size = new System.Drawing.Size(735, 20);
            this.textBoxTargetCriteria.TabIndex = 3;
            // 
            // FormResultsTarget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 440);
            this.Controls.Add(this.textBoxTargetCriteria);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvTarget);
            this.Name = "FormResultsTarget";
            this.Text = "FormResultsTarget";
            this.Load += new System.EventHandler(this.FormResultsTarget_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormResultsTarget_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTarget)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvTarget;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTargetCriteria;
    }
}