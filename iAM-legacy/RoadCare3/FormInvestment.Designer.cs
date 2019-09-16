namespace RoadCare3
{
    partial class FormInvestment
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
            this.labelInvestment = new System.Windows.Forms.Label();
            this.cbYears = new System.Windows.Forms.ComboBox();
            this.labelAnalysis = new System.Windows.Forms.Label();
            this.textBoxInflation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxDiscount = new System.Windows.Forms.TextBox();
            this.dgvBudget = new System.Windows.Forms.DataGridView();
            this.contextMenuStripInvesment = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxBudgetOrder = new System.Windows.Forms.TextBox();
            this.buttonEditOrder = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxStartYear = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.dataGridViewBudgetCriteria = new System.Windows.Forms.DataGridView();
            this.Budget = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.BudgetCriteria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainerInvestments = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBudget)).BeginInit();
            this.contextMenuStripInvesment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBudgetCriteria)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerInvestments)).BeginInit();
            this.splitContainerInvestments.Panel1.SuspendLayout();
            this.splitContainerInvestments.Panel2.SuspendLayout();
            this.splitContainerInvestments.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelInvestment
            // 
            this.labelInvestment.AutoSize = true;
            this.labelInvestment.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInvestment.Location = new System.Drawing.Point(52, 9);
            this.labelInvestment.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelInvestment.Name = "labelInvestment";
            this.labelInvestment.Size = new System.Drawing.Size(100, 24);
            this.labelInvestment.TabIndex = 0;
            this.labelInvestment.Text = "Investment";
            // 
            // cbYears
            // 
            this.cbYears.FormattingEnabled = true;
            this.cbYears.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50",
            "51",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59",
            "60"});
            this.cbYears.Location = new System.Drawing.Point(200, 36);
            this.cbYears.Margin = new System.Windows.Forms.Padding(2);
            this.cbYears.Name = "cbYears";
            this.cbYears.Size = new System.Drawing.Size(52, 21);
            this.cbYears.TabIndex = 1;
            this.cbYears.SelectedIndexChanged += new System.EventHandler(this.cbYears_SelectedIndexChanged);
            this.cbYears.Validated += new System.EventHandler(this.cbYears_Validated);
            // 
            // labelAnalysis
            // 
            this.labelAnalysis.AutoSize = true;
            this.labelAnalysis.Location = new System.Drawing.Point(116, 38);
            this.labelAnalysis.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAnalysis.Name = "labelAnalysis";
            this.labelAnalysis.Size = new System.Drawing.Size(81, 13);
            this.labelAnalysis.TabIndex = 2;
            this.labelAnalysis.Text = "Analysis Period:";
            // 
            // textBoxInflation
            // 
            this.textBoxInflation.Location = new System.Drawing.Point(348, 36);
            this.textBoxInflation.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxInflation.Name = "textBoxInflation";
            this.textBoxInflation.Size = new System.Drawing.Size(42, 20);
            this.textBoxInflation.TabIndex = 3;
            this.textBoxInflation.Enter += new System.EventHandler(this.textBoxInflation_Enter);
            this.textBoxInflation.Validated += new System.EventHandler(this.textBoxInflation_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(257, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Inflation Rate(%):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(404, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Discount Rate(%):";
            // 
            // textBoxDiscount
            // 
            this.textBoxDiscount.Location = new System.Drawing.Point(496, 37);
            this.textBoxDiscount.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxDiscount.Name = "textBoxDiscount";
            this.textBoxDiscount.Size = new System.Drawing.Size(42, 20);
            this.textBoxDiscount.TabIndex = 5;
            this.textBoxDiscount.Enter += new System.EventHandler(this.textBoxDiscount_Enter);
            this.textBoxDiscount.Validated += new System.EventHandler(this.textBoxDiscount_Validated);
            // 
            // dgvBudget
            // 
            this.dgvBudget.AllowUserToAddRows = false;
            this.dgvBudget.AllowUserToDeleteRows = false;
            this.dgvBudget.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBudget.ContextMenuStrip = this.contextMenuStripInvesment;
            this.dgvBudget.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBudget.Location = new System.Drawing.Point(0, 0);
            this.dgvBudget.Margin = new System.Windows.Forms.Padding(2);
            this.dgvBudget.Name = "dgvBudget";
            this.dgvBudget.RowTemplate.Height = 24;
            this.dgvBudget.Size = new System.Drawing.Size(647, 168);
            this.dgvBudget.TabIndex = 7;
            this.dgvBudget.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBudget_CellEnter);
            this.dgvBudget.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBudget_CellValueChanged);
            this.dgvBudget.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvBudget_KeyDown);
            // 
            // contextMenuStripInvesment
            // 
            this.contextMenuStripInvesment.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.contextMenuStripInvesment.Name = "contextMenuStripInvesment";
            this.contextMenuStripInvesment.Size = new System.Drawing.Size(108, 70);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // textBoxBudgetOrder
            // 
            this.textBoxBudgetOrder.Location = new System.Drawing.Point(92, 61);
            this.textBoxBudgetOrder.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxBudgetOrder.Name = "textBoxBudgetOrder";
            this.textBoxBudgetOrder.ReadOnly = true;
            this.textBoxBudgetOrder.Size = new System.Drawing.Size(453, 20);
            this.textBoxBudgetOrder.TabIndex = 8;
            this.textBoxBudgetOrder.TextChanged += new System.EventHandler(this.textBoxBudgetOrder_TextChanged);
            this.textBoxBudgetOrder.Validated += new System.EventHandler(this.textBoxBudgetOrder_Validated);
            // 
            // buttonEditOrder
            // 
            this.buttonEditOrder.Location = new System.Drawing.Point(549, 62);
            this.buttonEditOrder.Margin = new System.Windows.Forms.Padding(2);
            this.buttonEditOrder.Name = "buttonEditOrder";
            this.buttonEditOrder.Size = new System.Drawing.Size(27, 19);
            this.buttonEditOrder.TabIndex = 9;
            this.buttonEditOrder.Text = "...";
            this.buttonEditOrder.UseVisualStyleBackColor = true;
            this.buttonEditOrder.Click += new System.EventHandler(this.buttonEditOrder_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 65);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Budget Order:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 36);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Start Year:";
            // 
            // textBoxStartYear
            // 
            this.textBoxStartYear.Location = new System.Drawing.Point(63, 36);
            this.textBoxStartYear.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxStartYear.Name = "textBoxStartYear";
            this.textBoxStartYear.Size = new System.Drawing.Size(42, 20);
            this.textBoxStartYear.TabIndex = 12;
            this.textBoxStartYear.Enter += new System.EventHandler(this.textBoxStartYear_Enter);
            this.textBoxStartYear.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxStartYear_Validating);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RoadCare3.Properties.Resources.bigpink;
            this.pictureBox1.Location = new System.Drawing.Point(12, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(35, 29);
            this.pictureBox1.TabIndex = 23;
            this.pictureBox1.TabStop = false;
            // 
            // dataGridViewBudgetCriteria
            // 
            this.dataGridViewBudgetCriteria.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBudgetCriteria.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Budget,
            this.BudgetCriteria});
            this.dataGridViewBudgetCriteria.ContextMenuStrip = this.contextMenuStripInvesment;
            this.dataGridViewBudgetCriteria.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewBudgetCriteria.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewBudgetCriteria.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridViewBudgetCriteria.Name = "dataGridViewBudgetCriteria";
            this.dataGridViewBudgetCriteria.RowTemplate.Height = 24;
            this.dataGridViewBudgetCriteria.Size = new System.Drawing.Size(647, 165);
            this.dataGridViewBudgetCriteria.TabIndex = 24;
            this.dataGridViewBudgetCriteria.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewBudgetCriteria_CellMouseDoubleClick);
            this.dataGridViewBudgetCriteria.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewBudgetCriteria_CellValidated);
            this.dataGridViewBudgetCriteria.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.DataGridViewBudgetCriteria_UserDeletedRow);
            // 
            // Budget
            // 
            this.Budget.HeaderText = "Budget";
            this.Budget.Name = "Budget";
            this.Budget.Width = 200;
            // 
            // BudgetCriteria
            // 
            this.BudgetCriteria.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.BudgetCriteria.HeaderText = "Budget Critiera";
            this.BudgetCriteria.Name = "BudgetCriteria";
            // 
            // splitContainerInvestments
            // 
            this.splitContainerInvestments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerInvestments.Location = new System.Drawing.Point(6, 86);
            this.splitContainerInvestments.Name = "splitContainerInvestments";
            this.splitContainerInvestments.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerInvestments.Panel1
            // 
            this.splitContainerInvestments.Panel1.Controls.Add(this.dgvBudget);
            // 
            // splitContainerInvestments.Panel2
            // 
            this.splitContainerInvestments.Panel2.Controls.Add(this.dataGridViewBudgetCriteria);
            this.splitContainerInvestments.Size = new System.Drawing.Size(647, 337);
            this.splitContainerInvestments.SplitterDistance = 168;
            this.splitContainerInvestments.TabIndex = 25;
            // 
            // FormInvestment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 435);
            this.Controls.Add(this.splitContainerInvestments);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBoxStartYear);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonEditOrder);
            this.Controls.Add(this.textBoxBudgetOrder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxDiscount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxInflation);
            this.Controls.Add(this.labelAnalysis);
            this.Controls.Add(this.cbYears);
            this.Controls.Add(this.labelInvestment);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormInvestment";
            this.TabText = "FormInvestment";
            this.Text = "FormInvestment";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormInvestment_FormClosed);
            this.Load += new System.EventHandler(this.FormInvestment_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBudget)).EndInit();
            this.contextMenuStripInvesment.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBudgetCriteria)).EndInit();
            this.splitContainerInvestments.Panel1.ResumeLayout(false);
            this.splitContainerInvestments.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerInvestments)).EndInit();
            this.splitContainerInvestments.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelInvestment;
        private System.Windows.Forms.ComboBox cbYears;
        private System.Windows.Forms.Label labelAnalysis;
        private System.Windows.Forms.TextBox textBoxInflation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxDiscount;
        private System.Windows.Forms.DataGridView dgvBudget;
        private System.Windows.Forms.TextBox textBoxBudgetOrder;
        private System.Windows.Forms.Button buttonEditOrder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxStartYear;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripInvesment;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridView dataGridViewBudgetCriteria;
        private System.Windows.Forms.SplitContainer splitContainerInvestments;
        private System.Windows.Forms.DataGridViewComboBoxColumn Budget;
        private System.Windows.Forms.DataGridViewTextBoxColumn BudgetCriteria;
    }
}
