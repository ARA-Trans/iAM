namespace RoadCare3
{
    partial class FormResultsDeficient
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
			this.dgvDeficient = new System.Windows.Forms.DataGridView();
			this.textBoxDeficientCriteria = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			( ( System.ComponentModel.ISupportInitialize )( this.dgvDeficient ) ).BeginInit();
			this.SuspendLayout();
			// 
			// dgvDeficient
			// 
			this.dgvDeficient.AllowUserToAddRows = false;
			this.dgvDeficient.AllowUserToDeleteRows = false;
			this.dgvDeficient.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.dgvDeficient.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvDeficient.Location = new System.Drawing.Point( 0, 37 );
			this.dgvDeficient.Name = "dgvDeficient";
			this.dgvDeficient.RowHeadersVisible = false;
			this.dgvDeficient.Size = new System.Drawing.Size( 828, 408 );
			this.dgvDeficient.TabIndex = 0;
			this.dgvDeficient.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler( this.dgvDeficient_CellEnter );
			// 
			// textBoxDeficientCriteria
			// 
			this.textBoxDeficientCriteria.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.textBoxDeficientCriteria.Location = new System.Drawing.Point( 84, 8 );
			this.textBoxDeficientCriteria.Name = "textBoxDeficientCriteria";
			this.textBoxDeficientCriteria.Size = new System.Drawing.Size( 735, 20 );
			this.textBoxDeficientCriteria.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point( 2, 12 );
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size( 87, 13 );
			this.label1.TabIndex = 4;
			this.label1.Text = "Deficient Criteria:";
			// 
			// FormResultsDeficient
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 828, 445 );
			this.Controls.Add( this.textBoxDeficientCriteria );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.dgvDeficient );
			this.Name = "FormResultsDeficient";
			this.Text = "FormResultsDeficient";
			this.Load += new System.EventHandler( this.FormResultsDeficient_Load );
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.FormResultsDeficient_FormClosed );
			( ( System.ComponentModel.ISupportInitialize )( this.dgvDeficient ) ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDeficient;
        private System.Windows.Forms.TextBox textBoxDeficientCriteria;
        private System.Windows.Forms.Label label1;
    }
}