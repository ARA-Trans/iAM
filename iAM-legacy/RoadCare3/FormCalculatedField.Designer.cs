namespace RoadCare3
{
    partial class FormCalculatedField
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
            this.labelAttribute = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.dgvCalculated = new System.Windows.Forms.DataGridView();
            this.Equation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Criteria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCalculated)).BeginInit();
            this.SuspendLayout();
            // 
            // labelAttribute
            // 
            this.labelAttribute.AutoSize = true;
            this.labelAttribute.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAttribute.Location = new System.Drawing.Point(29, 6);
            this.labelAttribute.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAttribute.Name = "labelAttribute";
            this.labelAttribute.Size = new System.Drawing.Size(93, 26);
            this.labelAttribute.TabIndex = 12;
            this.labelAttribute.Text = "Attribute";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RoadCare3.Properties.Resources.database;
            this.pictureBox1.Location = new System.Drawing.Point(2, 3);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(22, 32);
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // dgvCalculated
            // 
            this.dgvCalculated.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCalculated.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCalculated.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Equation,
            this.Criteria});
            this.dgvCalculated.Location = new System.Drawing.Point(3, 40);
            this.dgvCalculated.Name = "dgvCalculated";
            this.dgvCalculated.Size = new System.Drawing.Size(734, 502);
            this.dgvCalculated.TabIndex = 14;
            this.dgvCalculated.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCalculated_CellDoubleClick);
            this.dgvCalculated.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgvCalculated_UserDeletingRow);
            // 
            // Equation
            // 
            this.Equation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Equation.HeaderText = "Equation";
            this.Equation.Name = "Equation";
            // 
            // Criteria
            // 
            this.Criteria.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Criteria.HeaderText = "Criteria";
            this.Criteria.Name = "Criteria";
            // 
            // FormCalculatedField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 543);
            this.Controls.Add(this.dgvCalculated);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelAttribute);
            this.Name = "FormCalculatedField";
            this.TabText = "FormCalculatedField";
            this.Text = "FormCalculatedField";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormCalculatedField_FormClosed);
            this.Load += new System.EventHandler(this.FormCalculatedField_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCalculated)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelAttribute;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridView dgvCalculated;
        private System.Windows.Forms.DataGridViewTextBoxColumn Equation;
        private System.Windows.Forms.DataGridViewTextBoxColumn Criteria;
    }
}