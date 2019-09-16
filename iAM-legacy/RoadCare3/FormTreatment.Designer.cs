namespace RoadCare3
{
    partial class FormTreatment
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
            this.labelTreatment = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dgvFeasibility = new System.Windows.Forms.DataGridView();
            this.Feasibility = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuFeasibility = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuFeasibleCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuFeasiblePaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuFeasibleCriteria = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuCosts = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuCostCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuCostPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuCostsCriteria = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvTreatment = new System.Windows.Forms.DataGridView();
            this.Treatments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCost = new System.Windows.Forms.DataGridView();
            this.Cost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Criteria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxAny = new System.Windows.Forms.TextBox();
            this.textBoxSame = new System.Windows.Forms.TextBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.dgvConsequences = new System.Windows.Forms.DataGridView();
            this.Attribute = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Change = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CONSEQUENCE_EQUATION = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CriteriaS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuConsequence = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuConsequenceCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuConsequencePaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuConsequenceCriteria = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControlFeasibility = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPageCost = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPageScheduled = new System.Windows.Forms.TabPage();
            this.dataGridViewScheduled = new System.Windows.Forms.DataGridView();
            this.ScheduledTreatment = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ScheduledYears = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tabPageSupercedes = new System.Windows.Forms.TabPage();
            this.dataGridViewSupersede = new System.Windows.Forms.DataGridView();
            this.SUPERCEDES = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.SUPERCEDES_CRITERIA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStripCT = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editCompoundTreatmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkedComboBoxBudget = new RoadCare3.CheckedComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFeasibility)).BeginInit();
            this.contextMenuFeasibility.SuspendLayout();
            this.contextMenuCosts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTreatment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConsequences)).BeginInit();
            this.contextMenuConsequence.SuspendLayout();
            this.tabControlFeasibility.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPageCost.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPageScheduled.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewScheduled)).BeginInit();
            this.tabPageSupercedes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSupersede)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStripCT.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTreatment
            // 
            this.labelTreatment.AutoSize = true;
            this.labelTreatment.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTreatment.Location = new System.Drawing.Point(46, 9);
            this.labelTreatment.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTreatment.Name = "labelTreatment";
            this.labelTreatment.Size = new System.Drawing.Size(271, 24);
            this.labelTreatment.TabIndex = 0;
            this.labelTreatment.Text = "Treatment Simulation : Network";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(190, 42);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Budget:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(737, 42);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Years Before Same Treatment:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(190, 71);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Description:";
            // 
            // dgvFeasibility
            // 
            this.dgvFeasibility.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFeasibility.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFeasibility.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Feasibility});
            this.dgvFeasibility.ContextMenuStrip = this.contextMenuFeasibility;
            this.dgvFeasibility.Location = new System.Drawing.Point(2, 0);
            this.dgvFeasibility.Margin = new System.Windows.Forms.Padding(2);
            this.dgvFeasibility.Name = "dgvFeasibility";
            this.dgvFeasibility.RowHeadersWidth = 20;
            this.dgvFeasibility.RowTemplate.Height = 24;
            this.dgvFeasibility.Size = new System.Drawing.Size(753, 398);
            this.dgvFeasibility.TabIndex = 6;
            this.dgvFeasibility.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFeasibility_CellDoubleClick);
            this.dgvFeasibility.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvFeasibility_CellMouseDoubleClick);
            this.dgvFeasibility.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFeasibility_CellValueChanged);
            this.dgvFeasibility.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvFeasibility_UserDeletedRow);
            // 
            // Feasibility
            // 
            this.Feasibility.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Feasibility.HeaderText = "Feasibility";
            this.Feasibility.Name = "Feasibility";
            // 
            // contextMenuFeasibility
            // 
            this.contextMenuFeasibility.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuFeasibleCopy,
            this.toolStripMenuFeasiblePaste,
            this.toolStripMenuFeasibleCriteria});
            this.contextMenuFeasibility.Name = "contextMenuCosts";
            this.contextMenuFeasibility.Size = new System.Drawing.Size(136, 70);
            // 
            // toolStripMenuFeasibleCopy
            // 
            this.toolStripMenuFeasibleCopy.Name = "toolStripMenuFeasibleCopy";
            this.toolStripMenuFeasibleCopy.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuFeasibleCopy.Text = "Copy";
            this.toolStripMenuFeasibleCopy.Click += new System.EventHandler(this.toolStripMenuFeasibleCopy_Click);
            // 
            // toolStripMenuFeasiblePaste
            // 
            this.toolStripMenuFeasiblePaste.Name = "toolStripMenuFeasiblePaste";
            this.toolStripMenuFeasiblePaste.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuFeasiblePaste.Text = "Paste";
            this.toolStripMenuFeasiblePaste.Click += new System.EventHandler(this.toolStripMenuFeasiblePaste_Click);
            // 
            // toolStripMenuFeasibleCriteria
            // 
            this.toolStripMenuFeasibleCriteria.Name = "toolStripMenuFeasibleCriteria";
            this.toolStripMenuFeasibleCriteria.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuFeasibleCriteria.Text = "Edit Criteria";
            this.toolStripMenuFeasibleCriteria.Click += new System.EventHandler(this.toolStripMenuFeasibleCriteria_Click);
            // 
            // contextMenuCosts
            // 
            this.contextMenuCosts.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuCostCopy,
            this.toolStripMenuCostPaste,
            this.toolStripMenuCostsCriteria});
            this.contextMenuCosts.Name = "contextMenuCosts";
            this.contextMenuCosts.Size = new System.Drawing.Size(136, 70);
            // 
            // toolStripMenuCostCopy
            // 
            this.toolStripMenuCostCopy.Name = "toolStripMenuCostCopy";
            this.toolStripMenuCostCopy.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuCostCopy.Text = "Copy";
            this.toolStripMenuCostCopy.Click += new System.EventHandler(this.toolStripMenuCostCopy_Click);
            // 
            // toolStripMenuCostPaste
            // 
            this.toolStripMenuCostPaste.Name = "toolStripMenuCostPaste";
            this.toolStripMenuCostPaste.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuCostPaste.Text = "Paste";
            this.toolStripMenuCostPaste.Click += new System.EventHandler(this.toolStripMenuCostPaste_Click);
            // 
            // toolStripMenuCostsCriteria
            // 
            this.toolStripMenuCostsCriteria.Name = "toolStripMenuCostsCriteria";
            this.toolStripMenuCostsCriteria.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuCostsCriteria.Text = "Edit Criteria";
            this.toolStripMenuCostsCriteria.Click += new System.EventHandler(this.toolStripMenuCostsCriteria_Click);
            // 
            // dgvTreatment
            // 
            this.dgvTreatment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvTreatment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTreatment.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Treatments});
            this.dgvTreatment.Location = new System.Drawing.Point(4, 42);
            this.dgvTreatment.Margin = new System.Windows.Forms.Padding(2);
            this.dgvTreatment.MultiSelect = false;
            this.dgvTreatment.Name = "dgvTreatment";
            this.dgvTreatment.RowHeadersWidth = 20;
            this.dgvTreatment.RowTemplate.Height = 24;
            this.dgvTreatment.Size = new System.Drawing.Size(182, 475);
            this.dgvTreatment.TabIndex = 8;
            this.dgvTreatment.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTreatment_CellEnter);
            this.dgvTreatment.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTreatment_CellValueChanged);
            this.dgvTreatment.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTreatment_RowEnter);
            this.dgvTreatment.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvTreatment_RowsRemoved);
            // 
            // Treatments
            // 
            this.Treatments.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Treatments.HeaderText = "Treatments";
            this.Treatments.Name = "Treatments";
            // 
            // dgvCost
            // 
            this.dgvCost.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCost.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Cost,
            this.Criteria});
            this.dgvCost.ContextMenuStrip = this.contextMenuCosts;
            this.dgvCost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCost.Location = new System.Drawing.Point(2, 2);
            this.dgvCost.Margin = new System.Windows.Forms.Padding(2);
            this.dgvCost.Name = "dgvCost";
            this.dgvCost.RowHeadersWidth = 20;
            this.dgvCost.RowTemplate.Height = 24;
            this.dgvCost.Size = new System.Drawing.Size(754, 394);
            this.dgvCost.TabIndex = 11;
            this.dgvCost.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCost_CellDoubleClick);
            this.dgvCost.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCost_CellValueChanged);
            this.dgvCost.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvCost_UserDeletedRow);
            // 
            // Cost
            // 
            this.Cost.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Cost.HeaderText = "Cost";
            this.Cost.Name = "Cost";
            // 
            // Criteria
            // 
            this.Criteria.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Criteria.HeaderText = "Criteria";
            this.Criteria.Name = "Criteria";
            this.Criteria.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Criteria.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(530, 42);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(143, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Years Before Any Treatment:";
            // 
            // textBoxAny
            // 
            this.textBoxAny.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAny.Location = new System.Drawing.Point(674, 39);
            this.textBoxAny.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxAny.Name = "textBoxAny";
            this.textBoxAny.Size = new System.Drawing.Size(55, 20);
            this.textBoxAny.TabIndex = 14;
            this.textBoxAny.Validated += new System.EventHandler(this.textBoxAny_Validated);
            // 
            // textBoxSame
            // 
            this.textBoxSame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSame.Location = new System.Drawing.Point(890, 39);
            this.textBoxSame.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSame.Name = "textBoxSame";
            this.textBoxSame.Size = new System.Drawing.Size(55, 20);
            this.textBoxSame.TabIndex = 15;
            this.textBoxSame.TextChanged += new System.EventHandler(this.textBoxSame_TextChanged);
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(252, 71);
            this.textBoxDescription.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(945, 20);
            this.textBoxDescription.TabIndex = 16;
            this.textBoxDescription.Validated += new System.EventHandler(this.textBoxDescription_Validated);
            // 
            // dgvConsequences
            // 
            this.dgvConsequences.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConsequences.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Attribute,
            this.Change,
            this.CONSEQUENCE_EQUATION,
            this.CriteriaS});
            this.dgvConsequences.ContextMenuStrip = this.contextMenuConsequence;
            this.dgvConsequences.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvConsequences.Location = new System.Drawing.Point(2, 2);
            this.dgvConsequences.Margin = new System.Windows.Forms.Padding(2);
            this.dgvConsequences.Name = "dgvConsequences";
            this.dgvConsequences.RowHeadersWidth = 20;
            this.dgvConsequences.RowTemplate.Height = 24;
            this.dgvConsequences.Size = new System.Drawing.Size(754, 394);
            this.dgvConsequences.TabIndex = 19;
            this.dgvConsequences.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvConsequences_CellMouseDoubleClick);
            this.dgvConsequences.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvConsequences_CellMouseDown);
            this.dgvConsequences.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvConsequences_CellValueChanged);
            this.dgvConsequences.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvConsequences_UserDeletedRow);
            // 
            // Attribute
            // 
            this.Attribute.HeaderText = "Attribute";
            this.Attribute.Name = "Attribute";
            this.Attribute.Width = 150;
            // 
            // Change
            // 
            this.Change.HeaderText = "Change";
            this.Change.Name = "Change";
            this.Change.Width = 75;
            // 
            // CONSEQUENCE_EQUATION
            // 
            this.CONSEQUENCE_EQUATION.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CONSEQUENCE_EQUATION.HeaderText = "Equation";
            this.CONSEQUENCE_EQUATION.Name = "CONSEQUENCE_EQUATION";
            // 
            // CriteriaS
            // 
            this.CriteriaS.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CriteriaS.HeaderText = "Criteria";
            this.CriteriaS.Name = "CriteriaS";
            // 
            // contextMenuConsequence
            // 
            this.contextMenuConsequence.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuConsequenceCopy,
            this.toolStripMenuConsequencePaste,
            this.toolStripMenuConsequenceCriteria});
            this.contextMenuConsequence.Name = "contextMenuCosts";
            this.contextMenuConsequence.Size = new System.Drawing.Size(136, 70);
            // 
            // toolStripMenuConsequenceCopy
            // 
            this.toolStripMenuConsequenceCopy.Name = "toolStripMenuConsequenceCopy";
            this.toolStripMenuConsequenceCopy.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuConsequenceCopy.Text = "Copy";
            this.toolStripMenuConsequenceCopy.Click += new System.EventHandler(this.toolStripMenuConsequenceCopy_Click);
            // 
            // toolStripMenuConsequencePaste
            // 
            this.toolStripMenuConsequencePaste.Name = "toolStripMenuConsequencePaste";
            this.toolStripMenuConsequencePaste.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuConsequencePaste.Text = "Paste";
            this.toolStripMenuConsequencePaste.Click += new System.EventHandler(this.toolStripMenuConsequencePaste_Click);
            // 
            // toolStripMenuConsequenceCriteria
            // 
            this.toolStripMenuConsequenceCriteria.Name = "toolStripMenuConsequenceCriteria";
            this.toolStripMenuConsequenceCriteria.Size = new System.Drawing.Size(135, 22);
            this.toolStripMenuConsequenceCriteria.Text = "Edit Criteria";
            this.toolStripMenuConsequenceCriteria.Click += new System.EventHandler(this.toolStripMenuConsequenceCriteria_Click);
            // 
            // tabControlFeasibility
            // 
            this.tabControlFeasibility.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlFeasibility.Controls.Add(this.tabPage1);
            this.tabControlFeasibility.Controls.Add(this.tabPageCost);
            this.tabControlFeasibility.Controls.Add(this.tabPage3);
            this.tabControlFeasibility.Controls.Add(this.tabPageScheduled);
            this.tabControlFeasibility.Controls.Add(this.tabPageSupercedes);
            this.tabControlFeasibility.Location = new System.Drawing.Point(188, 93);
            this.tabControlFeasibility.Margin = new System.Windows.Forms.Padding(2);
            this.tabControlFeasibility.Name = "tabControlFeasibility";
            this.tabControlFeasibility.SelectedIndex = 0;
            this.tabControlFeasibility.Size = new System.Drawing.Size(766, 424);
            this.tabControlFeasibility.TabIndex = 20;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dgvFeasibility);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(758, 398);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Feasibility";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPageCost
            // 
            this.tabPageCost.Controls.Add(this.dgvCost);
            this.tabPageCost.Location = new System.Drawing.Point(4, 22);
            this.tabPageCost.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageCost.Name = "tabPageCost";
            this.tabPageCost.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageCost.Size = new System.Drawing.Size(758, 398);
            this.tabPageCost.TabIndex = 1;
            this.tabPageCost.Text = "Cost";
            this.tabPageCost.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dgvConsequences);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage3.Size = new System.Drawing.Size(758, 398);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Consequence";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPageScheduled
            // 
            this.tabPageScheduled.Controls.Add(this.dataGridViewScheduled);
            this.tabPageScheduled.Location = new System.Drawing.Point(4, 22);
            this.tabPageScheduled.Name = "tabPageScheduled";
            this.tabPageScheduled.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageScheduled.Size = new System.Drawing.Size(758, 398);
            this.tabPageScheduled.TabIndex = 3;
            this.tabPageScheduled.Text = "Scheduled";
            this.tabPageScheduled.UseVisualStyleBackColor = true;
            // 
            // dataGridViewScheduled
            // 
            this.dataGridViewScheduled.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewScheduled.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ScheduledTreatment,
            this.ScheduledYears});
            this.dataGridViewScheduled.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewScheduled.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewScheduled.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridViewScheduled.Name = "dataGridViewScheduled";
            this.dataGridViewScheduled.RowHeadersWidth = 20;
            this.dataGridViewScheduled.RowTemplate.Height = 24;
            this.dataGridViewScheduled.Size = new System.Drawing.Size(752, 392);
            this.dataGridViewScheduled.TabIndex = 12;
            this.dataGridViewScheduled.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewScheduled_CellValidated);
            this.dataGridViewScheduled.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewScheduled_CellValueChanged);
            this.dataGridViewScheduled.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridViewScheduled_RowsRemoved);
            this.dataGridViewScheduled.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridViewScheduled_UserDeletedRow);
            // 
            // ScheduledTreatment
            // 
            this.ScheduledTreatment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ScheduledTreatment.HeaderText = "Treatment";
            this.ScheduledTreatment.Name = "ScheduledTreatment";
            this.ScheduledTreatment.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ScheduledTreatment.Sorted = true;
            this.ScheduledTreatment.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ScheduledYears
            // 
            this.ScheduledYears.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ScheduledYears.HeaderText = "Scheduled Year";
            this.ScheduledYears.Name = "ScheduledYears";
            this.ScheduledYears.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ScheduledYears.Sorted = true;
            this.ScheduledYears.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ScheduledYears.ToolTipText = "Number of years before treatment is scheduled.";
            // 
            // tabPageSupercedes
            // 
            this.tabPageSupercedes.Controls.Add(this.dataGridViewSupersede);
            this.tabPageSupercedes.Location = new System.Drawing.Point(4, 22);
            this.tabPageSupercedes.Name = "tabPageSupercedes";
            this.tabPageSupercedes.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSupercedes.Size = new System.Drawing.Size(758, 398);
            this.tabPageSupercedes.TabIndex = 4;
            this.tabPageSupercedes.Text = "Supercedes";
            this.tabPageSupercedes.UseVisualStyleBackColor = true;
            // 
            // dataGridViewSupersede
            // 
            this.dataGridViewSupersede.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSupersede.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SUPERCEDES,
            this.SUPERCEDES_CRITERIA});
            this.dataGridViewSupersede.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewSupersede.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewSupersede.Name = "dataGridViewSupersede";
            this.dataGridViewSupersede.Size = new System.Drawing.Size(752, 392);
            this.dataGridViewSupersede.TabIndex = 0;
            this.dataGridViewSupersede.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewSupersede_CellMouseDoubleClick);
            this.dataGridViewSupersede.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewSupersede_CellValidated);
            this.dataGridViewSupersede.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.DataGridViewSupersede_UserDeletedRow);
            this.dataGridViewSupersede.DoubleClick += new System.EventHandler(this.DataGridViewSupersede_DoubleClick);
            // 
            // SUPERCEDES
            // 
            this.SUPERCEDES.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SUPERCEDES.HeaderText = "Supercedes";
            this.SUPERCEDES.Name = "SUPERCEDES";
            this.SUPERCEDES.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SUPERCEDES.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // SUPERCEDES_CRITERIA
            // 
            this.SUPERCEDES_CRITERIA.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SUPERCEDES_CRITERIA.HeaderText = "Criteria";
            this.SUPERCEDES_CRITERIA.Name = "SUPERCEDES_CRITERIA";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RoadCare3.Properties.Resources.bigpink;
            this.pictureBox1.Location = new System.Drawing.Point(6, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(35, 29);
            this.pictureBox1.TabIndex = 22;
            this.pictureBox1.TabStop = false;
            // 
            // contextMenuStripCT
            // 
            this.contextMenuStripCT.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editCompoundTreatmentToolStripMenuItem});
            this.contextMenuStripCT.Name = "contextMenuStripCT";
            this.contextMenuStripCT.Size = new System.Drawing.Size(225, 26);
            // 
            // editCompoundTreatmentToolStripMenuItem
            // 
            this.editCompoundTreatmentToolStripMenuItem.Name = "editCompoundTreatmentToolStripMenuItem";
            this.editCompoundTreatmentToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.editCompoundTreatmentToolStripMenuItem.Text = "Edit Compound Treatment...";
            // 
            // checkedComboBoxBudget
            // 
            this.checkedComboBoxBudget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedComboBoxBudget.CheckOnClick = true;
            this.checkedComboBoxBudget.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.checkedComboBoxBudget.DropDownHeight = 1;
            this.checkedComboBoxBudget.FormattingEnabled = true;
            this.checkedComboBoxBudget.IntegralHeight = false;
            this.checkedComboBoxBudget.Location = new System.Drawing.Point(239, 39);
            this.checkedComboBoxBudget.Name = "checkedComboBoxBudget";
            this.checkedComboBoxBudget.Size = new System.Drawing.Size(286, 21);
            this.checkedComboBoxBudget.TabIndex = 23;
            this.checkedComboBoxBudget.ValueSeparator = ", ";
            this.checkedComboBoxBudget.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedComboBoxBudget_ItemCheck);
            this.checkedComboBoxBudget.DropDownClosed += new System.EventHandler(this.checkedComboBoxBudget_DropDownClosed);
            // 
            // FormTreatment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(800, 500);
            this.ClientSize = new System.Drawing.Size(956, 518);
            this.Controls.Add(this.checkedComboBoxBudget);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tabControlFeasibility);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.textBoxSame);
            this.Controls.Add(this.textBoxAny);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.dgvTreatment);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelTreatment);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormTreatment";
            this.TabText = "FormTreatment";
            this.Text = "FormTreatment";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormTreatment_FormClosed);
            this.Load += new System.EventHandler(this.FormTreatment_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFeasibility)).EndInit();
            this.contextMenuFeasibility.ResumeLayout(false);
            this.contextMenuCosts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTreatment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConsequences)).EndInit();
            this.contextMenuConsequence.ResumeLayout(false);
            this.tabControlFeasibility.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPageCost.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPageScheduled.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewScheduled)).EndInit();
            this.tabPageSupercedes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSupersede)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStripCT.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTreatment;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dgvFeasibility;
        private System.Windows.Forms.DataGridView dgvTreatment;
        private System.Windows.Forms.DataGridView dgvCost;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxAny;
        private System.Windows.Forms.TextBox textBoxSame;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.DataGridView dgvConsequences;
        private System.Windows.Forms.DataGridViewTextBoxColumn Treatments;
        private System.Windows.Forms.TabControl tabControlFeasibility;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPageCost;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Feasibility;
        private System.Windows.Forms.ContextMenuStrip contextMenuCosts;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuCostCopy;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuCostPaste;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuCostsCriteria;
        private System.Windows.Forms.ContextMenuStrip contextMenuFeasibility;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuFeasibleCopy;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuFeasiblePaste;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuFeasibleCriteria;
        private System.Windows.Forms.ContextMenuStrip contextMenuConsequence;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuConsequenceCopy;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuConsequencePaste;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuConsequenceCriteria;
		private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cost;
        private System.Windows.Forms.DataGridViewTextBoxColumn Criteria;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripCT;
		private System.Windows.Forms.ToolStripMenuItem editCompoundTreatmentToolStripMenuItem;
        private CheckedComboBox checkedComboBoxBudget;
        private System.Windows.Forms.DataGridViewComboBoxColumn Attribute;
        private System.Windows.Forms.DataGridViewTextBoxColumn Change;
        private System.Windows.Forms.DataGridViewTextBoxColumn CONSEQUENCE_EQUATION;
        private System.Windows.Forms.DataGridViewTextBoxColumn CriteriaS;
        private System.Windows.Forms.TabPage tabPageScheduled;
        private System.Windows.Forms.DataGridView dataGridViewScheduled;
        private System.Windows.Forms.DataGridViewComboBoxColumn ScheduledTreatment;
        private System.Windows.Forms.DataGridViewComboBoxColumn ScheduledYears;
        private System.Windows.Forms.TabPage tabPageSupercedes;
        private System.Windows.Forms.DataGridView dataGridViewSupersede;
        private System.Windows.Forms.DataGridViewComboBoxColumn SUPERCEDES;
        private System.Windows.Forms.DataGridViewTextBoxColumn SUPERCEDES_CRITERIA;
    }
}