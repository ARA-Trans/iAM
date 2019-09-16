namespace RoadCare3
{
    partial class FormResultsBudget
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
            this.dgvBudget = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBudget)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvBudget
            // 
            this.dgvBudget.AllowUserToAddRows = false;
            this.dgvBudget.AllowUserToDeleteRows = false;
            this.dgvBudget.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBudget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBudget.Location = new System.Drawing.Point(0, 0);
            this.dgvBudget.Name = "dgvBudget";
            this.dgvBudget.RowHeadersVisible = false;
            this.dgvBudget.Size = new System.Drawing.Size(624, 395);
            this.dgvBudget.TabIndex = 1;
            // 
            // FormResultsBudget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 395);
            this.Controls.Add(this.dgvBudget);
            this.Name = "FormResultsBudget";
            this.Text = "FormResultsBudget";
            this.Load += new System.EventHandler(this.FormResultsBudget_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormResultsBudget_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBudget)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvBudget;
    }
}