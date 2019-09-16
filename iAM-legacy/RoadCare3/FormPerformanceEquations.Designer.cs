namespace RoadCare3
{
    partial class FormPerformanceEquations
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
            this.components = new System.ComponentModel.Container();
            this.labelPerformance = new System.Windows.Forms.Label();
            this.dgvPerfomance = new System.Windows.Forms.DataGridView();
            this.contextMenuPerformance = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editEquationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editCriteriaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Attribute = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.EquationName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Equation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Criteria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Shift = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPerfomance)).BeginInit();
            this.contextMenuPerformance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelPerformance
            // 
            this.labelPerformance.AutoSize = true;
            this.labelPerformance.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPerformance.Location = new System.Drawing.Point(52, 8);
            this.labelPerformance.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPerformance.Name = "labelPerformance";
            this.labelPerformance.Size = new System.Drawing.Size(118, 24);
            this.labelPerformance.TabIndex = 1;
            this.labelPerformance.Text = "Performance";
            // 
            // dgvPerfomance
            // 
            this.dgvPerfomance.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPerfomance.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPerfomance.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Attribute,
            this.EquationName,
            this.Equation,
            this.Criteria,
            this.Shift});
            this.dgvPerfomance.ContextMenuStrip = this.contextMenuPerformance;
            this.dgvPerfomance.Location = new System.Drawing.Point(-2, 37);
            this.dgvPerfomance.Margin = new System.Windows.Forms.Padding(2);
            this.dgvPerfomance.Name = "dgvPerfomance";
            this.dgvPerfomance.RowTemplate.Height = 24;
            this.dgvPerfomance.Size = new System.Drawing.Size(660, 362);
            this.dgvPerfomance.TabIndex = 2;
            this.dgvPerfomance.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPerfomance_CellValueChanged);
            this.dgvPerfomance.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPerfomance_CellDoubleClick);
            this.dgvPerfomance.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvPerfomance_UserDeletedRow);
            this.dgvPerfomance.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvPerfomance_KeyDown);
            this.dgvPerfomance.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPerfomance_CellEnter);
            // 
            // contextMenuPerformance
            // 
            this.contextMenuPerformance.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.editEquationToolStripMenuItem,
            this.editCriteriaToolStripMenuItem});
            this.contextMenuPerformance.Name = "contextMenuPerformance";
            this.contextMenuPerformance.Size = new System.Drawing.Size(149, 92);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // editEquationToolStripMenuItem
            // 
            this.editEquationToolStripMenuItem.Name = "editEquationToolStripMenuItem";
            this.editEquationToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.editEquationToolStripMenuItem.Text = "Edit Equation";
            this.editEquationToolStripMenuItem.Click += new System.EventHandler(this.editEquationToolStripMenuItem_Click);
            // 
            // editCriteriaToolStripMenuItem
            // 
            this.editCriteriaToolStripMenuItem.Name = "editCriteriaToolStripMenuItem";
            this.editCriteriaToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.editCriteriaToolStripMenuItem.Text = "Edit Criteria";
            this.editCriteriaToolStripMenuItem.Click += new System.EventHandler(this.editCriteriaToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RoadCare3.Properties.Resources.bigpink;
            this.pictureBox1.Location = new System.Drawing.Point(12, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(35, 29);
            this.pictureBox1.TabIndex = 23;
            this.pictureBox1.TabStop = false;
            // 
            // Attribute
            // 
            this.Attribute.HeaderText = "Attribute";
            this.Attribute.Name = "Attribute";
            this.Attribute.Width = 150;
            // 
            // EquationName
            // 
            this.EquationName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.EquationName.HeaderText = "Equation Name";
            this.EquationName.Name = "EquationName";
            this.EquationName.Width = 97;
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
            // Shift
            // 
            this.Shift.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Shift.HeaderText = "Shift";
            this.Shift.Name = "Shift";
            this.Shift.Width = 34;
            // 
            // FormPerformanceEquations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 398);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.dgvPerfomance);
            this.Controls.Add(this.labelPerformance);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormPerformanceEquations";
            this.TabText = "FormPerformanceEquations";
            this.Text = "FormPerformanceEquations";
            this.Load += new System.EventHandler(this.FormPerformanceEquations_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormPerformanceEquations_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPerfomance)).EndInit();
            this.contextMenuPerformance.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelPerformance;
        private System.Windows.Forms.DataGridView dgvPerfomance;
        private System.Windows.Forms.ContextMenuStrip contextMenuPerformance;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editEquationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editCriteriaToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewComboBoxColumn Attribute;
        private System.Windows.Forms.DataGridViewTextBoxColumn EquationName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Equation;
        private System.Windows.Forms.DataGridViewTextBoxColumn Criteria;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Shift;
    }
}