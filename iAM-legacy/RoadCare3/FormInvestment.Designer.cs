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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dataGridViewSplitTreatmentLimit = new System.Windows.Forms.DataGridView();
            this.SPLIT_YEAR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SPLIT_AMOUNT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PERCENTAGE_SPLIT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewSplitTreatmentCriteria = new System.Windows.Forms.DataGridView();
            this.DESCRIPTION = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CRITERIA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBudget)).BeginInit();
            this.contextMenuStripInvesment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBudgetCriteria)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSplitTreatmentLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSplitTreatmentCriteria)).BeginInit();
            this.SuspendLayout();
            // 
            // labelInvestment
            // 
            this.labelInvestment.AutoSize = true;
            this.labelInvestment.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInvestment.Location = new System.Drawing.Point(58, 11);
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
            this.cbYears.Location = new System.Drawing.Point(206, 38);
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
            this.labelAnalysis.Location = new System.Drawing.Point(122, 40);
            this.labelAnalysis.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAnalysis.Name = "labelAnalysis";
            this.labelAnalysis.Size = new System.Drawing.Size(81, 13);
            this.labelAnalysis.TabIndex = 2;
            this.labelAnalysis.Text = "Analysis Period:";
            // 
            // textBoxInflation
            // 
            this.textBoxInflation.Location = new System.Drawing.Point(354, 38);
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
            this.label1.Location = new System.Drawing.Point(263, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Inflation Rate(%):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(410, 40);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Discount Rate(%):";
            // 
            // textBoxDiscount
            // 
            this.textBoxDiscount.Location = new System.Drawing.Point(502, 39);
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
            this.dgvBudget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvBudget.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBudget.ContextMenuStrip = this.contextMenuStripInvesment;
            this.dgvBudget.Location = new System.Drawing.Point(5, 113);
            this.dgvBudget.Margin = new System.Windows.Forms.Padding(2);
            this.dgvBudget.Name = "dgvBudget";
            this.dgvBudget.RowTemplate.Height = 24;
            this.dgvBudget.Size = new System.Drawing.Size(637, 295);
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
            this.textBoxBudgetOrder.Location = new System.Drawing.Point(98, 63);
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
            this.buttonEditOrder.Location = new System.Drawing.Point(555, 64);
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
            this.label3.Location = new System.Drawing.Point(9, 67);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Budget Order:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 38);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Start Year:";
            // 
            // textBoxStartYear
            // 
            this.textBoxStartYear.Location = new System.Drawing.Point(69, 38);
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
            this.pictureBox1.Location = new System.Drawing.Point(18, 6);
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
            this.dataGridViewBudgetCriteria.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewBudgetCriteria.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridViewBudgetCriteria.Name = "dataGridViewBudgetCriteria";
            this.dataGridViewBudgetCriteria.RowTemplate.Height = 24;
            this.dataGridViewBudgetCriteria.Size = new System.Drawing.Size(645, 403);
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
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(659, 435);
            this.tabControl1.TabIndex = 26;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dgvBudget);
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Controls.Add(this.labelInvestment);
            this.tabPage1.Controls.Add(this.cbYears);
            this.tabPage1.Controls.Add(this.textBoxStartYear);
            this.tabPage1.Controls.Add(this.labelAnalysis);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.textBoxInflation);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.buttonEditOrder);
            this.tabPage1.Controls.Add(this.textBoxDiscount);
            this.tabPage1.Controls.Add(this.textBoxBudgetOrder);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(651, 409);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Investment";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridViewBudgetCriteria);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(651, 409);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Budget Criteria";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dataGridViewSplitTreatmentLimit);
            this.tabPage3.Controls.Add(this.dataGridViewSplitTreatmentCriteria);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(651, 409);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Split Treatment";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dataGridViewSplitTreatmentLimit
            // 
            this.dataGridViewSplitTreatmentLimit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewSplitTreatmentLimit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSplitTreatmentLimit.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SPLIT_YEAR,
            this.SPLIT_AMOUNT,
            this.PERCENTAGE_SPLIT});
            this.dataGridViewSplitTreatmentLimit.Location = new System.Drawing.Point(261, 0);
            this.dataGridViewSplitTreatmentLimit.Name = "dataGridViewSplitTreatmentLimit";
            this.dataGridViewSplitTreatmentLimit.Size = new System.Drawing.Size(384, 409);
            this.dataGridViewSplitTreatmentLimit.TabIndex = 1;
            this.dataGridViewSplitTreatmentLimit.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewSplitTreatmentLimit_CellValidated);
            this.dataGridViewSplitTreatmentLimit.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.DataGridViewSplitTreatmentLimit_UserDeletedRow);
            // 
            // SPLIT_YEAR
            // 
            this.SPLIT_YEAR.HeaderText = "Split Year";
            this.SPLIT_YEAR.Name = "SPLIT_YEAR";
            this.SPLIT_YEAR.ReadOnly = true;
            this.SPLIT_YEAR.Width = 80;
            // 
            // SPLIT_AMOUNT
            // 
            this.SPLIT_AMOUNT.HeaderText = "Amount";
            this.SPLIT_AMOUNT.Name = "SPLIT_AMOUNT";
            this.SPLIT_AMOUNT.Width = 80;
            // 
            // PERCENTAGE_SPLIT
            // 
            this.PERCENTAGE_SPLIT.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PERCENTAGE_SPLIT.HeaderText = "Percentages";
            this.PERCENTAGE_SPLIT.Name = "PERCENTAGE_SPLIT";
            // 
            // dataGridViewSplitTreatmentCriteria
            // 
            this.dataGridViewSplitTreatmentCriteria.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewSplitTreatmentCriteria.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSplitTreatmentCriteria.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DESCRIPTION,
            this.CRITERIA});
            this.dataGridViewSplitTreatmentCriteria.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewSplitTreatmentCriteria.Name = "dataGridViewSplitTreatmentCriteria";
            this.dataGridViewSplitTreatmentCriteria.Size = new System.Drawing.Size(255, 409);
            this.dataGridViewSplitTreatmentCriteria.TabIndex = 0;
            this.dataGridViewSplitTreatmentCriteria.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewSplitTreatmentCriteria_CellDoubleClick);
            this.dataGridViewSplitTreatmentCriteria.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewSplitTreatmentCriteria_CellValidated);
            this.dataGridViewSplitTreatmentCriteria.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewSplitTreatmentCriteria_RowEnter);
            this.dataGridViewSplitTreatmentCriteria.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.DataGridViewSplitTreatmentCriteria_UserDeletedRow);
            // 
            // DESCRIPTION
            // 
            this.DESCRIPTION.HeaderText = "Description";
            this.DESCRIPTION.Name = "DESCRIPTION";
            this.DESCRIPTION.Width = 120;
            // 
            // CRITERIA
            // 
            this.CRITERIA.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CRITERIA.HeaderText = "Criteria";
            this.CRITERIA.Name = "CRITERIA";
            // 
            // FormInvestment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 435);
            this.Controls.Add(this.tabControl1);
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
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSplitTreatmentLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSplitTreatmentCriteria)).EndInit();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.DataGridViewComboBoxColumn Budget;
        private System.Windows.Forms.DataGridViewTextBoxColumn BudgetCriteria;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dataGridViewSplitTreatmentLimit;
        private System.Windows.Forms.DataGridView dataGridViewSplitTreatmentCriteria;
        private System.Windows.Forms.DataGridViewTextBoxColumn DESCRIPTION;
        private System.Windows.Forms.DataGridViewTextBoxColumn CRITERIA;
        private System.Windows.Forms.DataGridViewTextBoxColumn SPLIT_YEAR;
        private System.Windows.Forms.DataGridViewTextBoxColumn SPLIT_AMOUNT;
        private System.Windows.Forms.DataGridViewTextBoxColumn PERCENTAGE_SPLIT;
    }
}
