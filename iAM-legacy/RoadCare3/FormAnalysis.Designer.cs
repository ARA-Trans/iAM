namespace RoadCare3
{
    partial class FormAnalysis
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
            this.labelAnalysis = new System.Windows.Forms.Label();
            this.tabControlAnalysis = new System.Windows.Forms.TabControl();
            this.tabPagePriority = new System.Windows.Forms.TabPage();
            this.radioButtonAcrossBudget = new System.Windows.Forms.RadioButton();
            this.radioButtonWithinBudget = new System.Windows.Forms.RadioButton();
            this.dgvPriority = new System.Windows.Forms.DataGridView();
            this.tabPageTarget = new System.Windows.Forms.TabPage();
            this.dgvTarget = new System.Windows.Forms.DataGridView();
            this.Attribute = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.NameTarget = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Years = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Target = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Criteria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageDeficient = new System.Windows.Forms.TabPage();
            this.dgvDeficient = new System.Windows.Forms.DataGridView();
            this.DeficientAttribute = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.DeficientName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Deficient = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeficientPercent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeficientCriteria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabRemainingLife = new System.Windows.Forms.TabPage();
            this.dataGridViewRemainLife = new System.Windows.Forms.DataGridView();
            this.RemainingLifeAttribute = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.RemainingLifeLimit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RemainingLifeCriteria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.cbOptimization = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbBudget = new System.Windows.Forms.ComboBox();
            this.labelBenefit = new System.Windows.Forms.Label();
            this.cbBenefit = new System.Windows.Forms.ComboBox();
            this.labelWeighting = new System.Windows.Forms.Label();
            this.cbWeighting = new System.Windows.Forms.ComboBox();
            this.labelBenefitLimit = new System.Windows.Forms.Label();
            this.tbBenefitLimit = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbJurisdiction = new System.Windows.Forms.TextBox();
            this.buttonCriteria = new System.Windows.Forms.Button();
            this.buttonRunSimulation = new System.Windows.Forms.Button();
            this.timerSimulation = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkBoxMultipleCost = new System.Windows.Forms.CheckBox();
            this.tabControlAnalysis.SuspendLayout();
            this.tabPagePriority.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPriority)).BeginInit();
            this.tabPageTarget.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTarget)).BeginInit();
            this.tabPageDeficient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeficient)).BeginInit();
            this.tabRemainingLife.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRemainLife)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelAnalysis
            // 
            this.labelAnalysis.AutoSize = true;
            this.labelAnalysis.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAnalysis.Location = new System.Drawing.Point(52, 9);
            this.labelAnalysis.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAnalysis.Name = "labelAnalysis";
            this.labelAnalysis.Size = new System.Drawing.Size(324, 24);
            this.labelAnalysis.TabIndex = 1;
            this.labelAnalysis.Text = "Analysis Method Simulation : Network";
            // 
            // tabControlAnalysis
            // 
            this.tabControlAnalysis.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlAnalysis.Controls.Add(this.tabPagePriority);
            this.tabControlAnalysis.Controls.Add(this.tabPageTarget);
            this.tabControlAnalysis.Controls.Add(this.tabPageDeficient);
            this.tabControlAnalysis.Controls.Add(this.tabRemainingLife);
            this.tabControlAnalysis.Location = new System.Drawing.Point(-1, 107);
            this.tabControlAnalysis.Margin = new System.Windows.Forms.Padding(2);
            this.tabControlAnalysis.Name = "tabControlAnalysis";
            this.tabControlAnalysis.SelectedIndex = 0;
            this.tabControlAnalysis.Size = new System.Drawing.Size(842, 412);
            this.tabControlAnalysis.TabIndex = 2;
            // 
            // tabPagePriority
            // 
            this.tabPagePriority.Controls.Add(this.radioButtonAcrossBudget);
            this.tabPagePriority.Controls.Add(this.radioButtonWithinBudget);
            this.tabPagePriority.Controls.Add(this.dgvPriority);
            this.tabPagePriority.Location = new System.Drawing.Point(4, 22);
            this.tabPagePriority.Margin = new System.Windows.Forms.Padding(2);
            this.tabPagePriority.Name = "tabPagePriority";
            this.tabPagePriority.Padding = new System.Windows.Forms.Padding(2);
            this.tabPagePriority.Size = new System.Drawing.Size(834, 386);
            this.tabPagePriority.TabIndex = 0;
            this.tabPagePriority.Text = "Priority";
            this.tabPagePriority.UseVisualStyleBackColor = true;
            this.tabPagePriority.Click += new System.EventHandler(this.TabPagePriority_Click);
            // 
            // radioButtonAcrossBudget
            // 
            this.radioButtonAcrossBudget.AutoSize = true;
            this.radioButtonAcrossBudget.Location = new System.Drawing.Point(193, 9);
            this.radioButtonAcrossBudget.Name = "radioButtonAcrossBudget";
            this.radioButtonAcrossBudget.Size = new System.Drawing.Size(174, 17);
            this.radioButtonAcrossBudget.TabIndex = 27;
            this.radioButtonAcrossBudget.TabStop = true;
            this.radioButtonAcrossBudget.Text = "Use extra funds across budgets";
            this.radioButtonAcrossBudget.UseVisualStyleBackColor = true;
            this.radioButtonAcrossBudget.CheckedChanged += new System.EventHandler(this.RadioButtonAcrossBudget_CheckedChanged);
            // 
            // radioButtonWithinBudget
            // 
            this.radioButtonWithinBudget.AutoSize = true;
            this.radioButtonWithinBudget.Location = new System.Drawing.Point(7, 9);
            this.radioButtonWithinBudget.Name = "radioButtonWithinBudget";
            this.radioButtonWithinBudget.Size = new System.Drawing.Size(165, 17);
            this.radioButtonWithinBudget.TabIndex = 26;
            this.radioButtonWithinBudget.TabStop = true;
            this.radioButtonWithinBudget.Text = "Use extra funds within budget";
            this.radioButtonWithinBudget.UseVisualStyleBackColor = true;
            this.radioButtonWithinBudget.CheckedChanged += new System.EventHandler(this.RadioButtonWithinBudget_CheckedChanged);
            // 
            // dgvPriority
            // 
            this.dgvPriority.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPriority.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPriority.Location = new System.Drawing.Point(0, 33);
            this.dgvPriority.Margin = new System.Windows.Forms.Padding(2);
            this.dgvPriority.Name = "dgvPriority";
            this.dgvPriority.RowTemplate.Height = 24;
            this.dgvPriority.Size = new System.Drawing.Size(834, 353);
            this.dgvPriority.TabIndex = 0;
            this.dgvPriority.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPriority_CellDoubleClick);
            this.dgvPriority.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPriority_CellEnter);
            this.dgvPriority.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPriority_CellValueChanged);
            this.dgvPriority.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvPriority_UserDeletedRow);
            // 
            // tabPageTarget
            // 
            this.tabPageTarget.Controls.Add(this.dgvTarget);
            this.tabPageTarget.Location = new System.Drawing.Point(4, 22);
            this.tabPageTarget.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageTarget.Name = "tabPageTarget";
            this.tabPageTarget.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageTarget.Size = new System.Drawing.Size(834, 386);
            this.tabPageTarget.TabIndex = 1;
            this.tabPageTarget.Text = "Target";
            this.tabPageTarget.UseVisualStyleBackColor = true;
            // 
            // dgvTarget
            // 
            this.dgvTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTarget.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTarget.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Attribute,
            this.NameTarget,
            this.Years,
            this.Target,
            this.Criteria});
            this.dgvTarget.Location = new System.Drawing.Point(0, 0);
            this.dgvTarget.Margin = new System.Windows.Forms.Padding(2);
            this.dgvTarget.Name = "dgvTarget";
            this.dgvTarget.RowTemplate.Height = 24;
            this.dgvTarget.Size = new System.Drawing.Size(834, 388);
            this.dgvTarget.TabIndex = 0;
            this.dgvTarget.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTarget_CellDoubleClick);
            this.dgvTarget.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTarget_CellEnter);
            this.dgvTarget.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTarget_CellValueChanged);
            this.dgvTarget.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvTarget_UserDeletedRow);
            // 
            // Attribute
            // 
            this.Attribute.HeaderText = "Attribute";
            this.Attribute.Name = "Attribute";
            this.Attribute.Width = 150;
            // 
            // NameTarget
            // 
            this.NameTarget.HeaderText = "Name";
            this.NameTarget.Name = "NameTarget";
            // 
            // Years
            // 
            this.Years.HeaderText = "Year";
            this.Years.Name = "Years";
            // 
            // Target
            // 
            this.Target.HeaderText = "Target";
            this.Target.Name = "Target";
            // 
            // Criteria
            // 
            this.Criteria.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Criteria.HeaderText = "Criteria";
            this.Criteria.Name = "Criteria";
            // 
            // tabPageDeficient
            // 
            this.tabPageDeficient.Controls.Add(this.dgvDeficient);
            this.tabPageDeficient.Location = new System.Drawing.Point(4, 22);
            this.tabPageDeficient.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageDeficient.Name = "tabPageDeficient";
            this.tabPageDeficient.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageDeficient.Size = new System.Drawing.Size(834, 386);
            this.tabPageDeficient.TabIndex = 2;
            this.tabPageDeficient.Text = "Deficient";
            this.tabPageDeficient.UseVisualStyleBackColor = true;
            // 
            // dgvDeficient
            // 
            this.dgvDeficient.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDeficient.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDeficient.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DeficientAttribute,
            this.DeficientName,
            this.Deficient,
            this.DeficientPercent,
            this.DeficientCriteria});
            this.dgvDeficient.Location = new System.Drawing.Point(0, 0);
            this.dgvDeficient.Margin = new System.Windows.Forms.Padding(2);
            this.dgvDeficient.Name = "dgvDeficient";
            this.dgvDeficient.RowTemplate.Height = 24;
            this.dgvDeficient.Size = new System.Drawing.Size(836, 388);
            this.dgvDeficient.TabIndex = 0;
            this.dgvDeficient.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDeficient_CellDoubleClick);
            this.dgvDeficient.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDeficient_CellEnter);
            this.dgvDeficient.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDeficient_CellValueChanged);
            this.dgvDeficient.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvDeficient_DataError);
            this.dgvDeficient.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvDeficient_UserDeletedRow);
            // 
            // DeficientAttribute
            // 
            this.DeficientAttribute.HeaderText = "Attribute";
            this.DeficientAttribute.Name = "DeficientAttribute";
            this.DeficientAttribute.Width = 125;
            // 
            // DeficientName
            // 
            this.DeficientName.HeaderText = "Name";
            this.DeficientName.Name = "DeficientName";
            // 
            // Deficient
            // 
            this.Deficient.HeaderText = "Deficient Level";
            this.Deficient.Name = "Deficient";
            // 
            // DeficientPercent
            // 
            this.DeficientPercent.HeaderText = "Allowed Deficient(%)";
            this.DeficientPercent.Name = "DeficientPercent";
            // 
            // DeficientCriteria
            // 
            this.DeficientCriteria.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DeficientCriteria.HeaderText = "Criteria";
            this.DeficientCriteria.Name = "DeficientCriteria";
            // 
            // tabRemainingLife
            // 
            this.tabRemainingLife.Controls.Add(this.dataGridViewRemainLife);
            this.tabRemainingLife.Location = new System.Drawing.Point(4, 22);
            this.tabRemainingLife.Name = "tabRemainingLife";
            this.tabRemainingLife.Padding = new System.Windows.Forms.Padding(3);
            this.tabRemainingLife.Size = new System.Drawing.Size(834, 386);
            this.tabRemainingLife.TabIndex = 3;
            this.tabRemainingLife.Text = "Remaining Life Limit";
            this.tabRemainingLife.UseVisualStyleBackColor = true;
            // 
            // dataGridViewRemainLife
            // 
            this.dataGridViewRemainLife.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRemainLife.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RemainingLifeAttribute,
            this.RemainingLifeLimit,
            this.RemainingLifeCriteria});
            this.dataGridViewRemainLife.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRemainLife.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewRemainLife.Name = "dataGridViewRemainLife";
            this.dataGridViewRemainLife.Size = new System.Drawing.Size(828, 380);
            this.dataGridViewRemainLife.TabIndex = 0;
            this.dataGridViewRemainLife.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewRemainLife_CellMouseDoubleClick);
            this.dataGridViewRemainLife.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewRemainLife_CellValidated);
            this.dataGridViewRemainLife.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.DataGridViewRemainLife_UserDeletedRow);

            // 
            // RemainingLifeAttribute
            // 
            this.RemainingLifeAttribute.HeaderText = "Remaining Life Attribute";
            this.RemainingLifeAttribute.Name = "RemainingLifeAttribute";
            this.RemainingLifeAttribute.Width = 200;
            // 
            // RemainingLifeLimit
            // 
            this.RemainingLifeLimit.HeaderText = "Limit";
            this.RemainingLifeLimit.Name = "RemainingLifeLimit";
            // 
            // RemainingLifeCriteria
            // 
            this.RemainingLifeCriteria.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.RemainingLifeCriteria.HeaderText = "Criteria";
            this.RemainingLifeCriteria.Name = "RemainingLifeCriteria";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Optimization:";
            // 
            // cbOptimization
            // 
            this.cbOptimization.FormattingEnabled = true;
            this.cbOptimization.Items.AddRange(new object[] {
            "Incremental Benefit/Cost",
            "Maximum Benefit",
            "Remaining Life/Cost",
            "Conditional RSL/Cost",
            "Maximum Remaining Life",
            "Multi-year Incremental Benefit/Cost",
            "Multi-year Maximum Benefit",
            "Multi-year Remaining Life/Cost",
            "Multi-year Maximum Life"});
            this.cbOptimization.Location = new System.Drawing.Point(76, 34);
            this.cbOptimization.Margin = new System.Windows.Forms.Padding(2);
            this.cbOptimization.Name = "cbOptimization";
            this.cbOptimization.Size = new System.Drawing.Size(213, 21);
            this.cbOptimization.TabIndex = 4;
            this.cbOptimization.SelectedIndexChanged += new System.EventHandler(this.cbOptimization_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 59);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Description:";
            // 
            // tbDescription
            // 
            this.tbDescription.Location = new System.Drawing.Point(76, 58);
            this.tbDescription.Margin = new System.Windows.Forms.Padding(2);
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.Size = new System.Drawing.Size(380, 20);
            this.tbDescription.TabIndex = 6;
            this.tbDescription.Validated += new System.EventHandler(this.tbDescription_Validated);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(293, 36);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Budget:";
            // 
            // cbBudget
            // 
            this.cbBudget.FormattingEnabled = true;
            this.cbBudget.Items.AddRange(new object[] {
            "No Spending",
            "As Budget Permits",
            "Until Targets Met",
            "Until Deficient Met",
            "Targets/Deficient Met",
            "Unlimited"});
            this.cbBudget.Location = new System.Drawing.Point(338, 34);
            this.cbBudget.Margin = new System.Windows.Forms.Padding(2);
            this.cbBudget.Name = "cbBudget";
            this.cbBudget.Size = new System.Drawing.Size(118, 21);
            this.cbBudget.TabIndex = 8;
            this.cbBudget.SelectedIndexChanged += new System.EventHandler(this.cbBudget_SelectedIndexChanged);
            // 
            // labelBenefit
            // 
            this.labelBenefit.AutoSize = true;
            this.labelBenefit.Location = new System.Drawing.Point(653, 36);
            this.labelBenefit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelBenefit.Name = "labelBenefit";
            this.labelBenefit.Size = new System.Drawing.Size(43, 13);
            this.labelBenefit.TabIndex = 9;
            this.labelBenefit.Text = "Benefit:";
            // 
            // cbBenefit
            // 
            this.cbBenefit.FormattingEnabled = true;
            this.cbBenefit.Location = new System.Drawing.Point(695, 34);
            this.cbBenefit.Margin = new System.Windows.Forms.Padding(2);
            this.cbBenefit.Name = "cbBenefit";
            this.cbBenefit.Size = new System.Drawing.Size(118, 21);
            this.cbBenefit.TabIndex = 10;
            this.cbBenefit.SelectedIndexChanged += new System.EventHandler(this.cbBenefit_SelectedIndexChanged);
            // 
            // labelWeighting
            // 
            this.labelWeighting.AutoSize = true;
            this.labelWeighting.Location = new System.Drawing.Point(460, 36);
            this.labelWeighting.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelWeighting.Name = "labelWeighting";
            this.labelWeighting.Size = new System.Drawing.Size(58, 13);
            this.labelWeighting.TabIndex = 11;
            this.labelWeighting.Text = "Weighting:";
            // 
            // cbWeighting
            // 
            this.cbWeighting.FormattingEnabled = true;
            this.cbWeighting.Location = new System.Drawing.Point(520, 33);
            this.cbWeighting.Margin = new System.Windows.Forms.Padding(2);
            this.cbWeighting.Name = "cbWeighting";
            this.cbWeighting.Size = new System.Drawing.Size(118, 21);
            this.cbWeighting.TabIndex = 12;
            this.cbWeighting.SelectedIndexChanged += new System.EventHandler(this.cbWeighting_SelectedIndexChanged);
            // 
            // labelBenefitLimit
            // 
            this.labelBenefitLimit.AutoSize = true;
            this.labelBenefitLimit.Location = new System.Drawing.Point(461, 62);
            this.labelBenefitLimit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelBenefitLimit.Name = "labelBenefitLimit";
            this.labelBenefitLimit.Size = new System.Drawing.Size(67, 13);
            this.labelBenefitLimit.TabIndex = 13;
            this.labelBenefitLimit.Text = "Benefit Limit:";
            // 
            // tbBenefitLimit
            // 
            this.tbBenefitLimit.Location = new System.Drawing.Point(528, 58);
            this.tbBenefitLimit.Margin = new System.Windows.Forms.Padding(2);
            this.tbBenefitLimit.Name = "tbBenefitLimit";
            this.tbBenefitLimit.Size = new System.Drawing.Size(118, 20);
            this.tbBenefitLimit.TabIndex = 14;
            this.tbBenefitLimit.Enter += new System.EventHandler(this.tbBenefitLimit_Enter);
            this.tbBenefitLimit.Validated += new System.EventHandler(this.tbBenefitLimit_Validated);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(185, 87);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Jurisdiction Criteria:";
            // 
            // tbJurisdiction
            // 
            this.tbJurisdiction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbJurisdiction.Location = new System.Drawing.Point(286, 84);
            this.tbJurisdiction.Margin = new System.Windows.Forms.Padding(2);
            this.tbJurisdiction.Name = "tbJurisdiction";
            this.tbJurisdiction.ReadOnly = true;
            this.tbJurisdiction.Size = new System.Drawing.Size(497, 20);
            this.tbJurisdiction.TabIndex = 16;
            this.tbJurisdiction.TextChanged += new System.EventHandler(this.tbJurisdiction_TextChanged);
            this.tbJurisdiction.Validated += new System.EventHandler(this.tbJurisdiction_Validated);
            // 
            // buttonCriteria
            // 
            this.buttonCriteria.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCriteria.Location = new System.Drawing.Point(787, 84);
            this.buttonCriteria.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCriteria.Name = "buttonCriteria";
            this.buttonCriteria.Size = new System.Drawing.Size(26, 19);
            this.buttonCriteria.TabIndex = 17;
            this.buttonCriteria.Text = "...";
            this.buttonCriteria.UseVisualStyleBackColor = true;
            this.buttonCriteria.Click += new System.EventHandler(this.buttonCriteria_Click);
            // 
            // buttonRunSimulation
            // 
            this.buttonRunSimulation.Location = new System.Drawing.Point(12, 81);
            this.buttonRunSimulation.Name = "buttonRunSimulation";
            this.buttonRunSimulation.Size = new System.Drawing.Size(154, 23);
            this.buttonRunSimulation.TabIndex = 18;
            this.buttonRunSimulation.Text = "Run Simulation";
            this.buttonRunSimulation.UseVisualStyleBackColor = true;
            this.buttonRunSimulation.Click += new System.EventHandler(this.buttonRunSimulation_Click);
            // 
            // timerSimulation
            // 
            this.timerSimulation.Interval = 2000;
            this.timerSimulation.Tick += new System.EventHandler(this.timerSimulation_Tick);
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
            // checkBoxMultipleCost
            // 
            this.checkBoxMultipleCost.AutoSize = true;
            this.checkBoxMultipleCost.Location = new System.Drawing.Point(656, 60);
            this.checkBoxMultipleCost.Name = "checkBoxMultipleCost";
            this.checkBoxMultipleCost.Size = new System.Drawing.Size(157, 17);
            this.checkBoxMultipleCost.TabIndex = 24;
            this.checkBoxMultipleCost.Text = "Apply multiple feasible costs";
            this.checkBoxMultipleCost.UseVisualStyleBackColor = true;
            this.checkBoxMultipleCost.CheckedChanged += new System.EventHandler(this.checkBoxMultipleCost_CheckedChanged);
            // 
            // FormAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 518);
            this.Controls.Add(this.checkBoxMultipleCost);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonRunSimulation);
            this.Controls.Add(this.buttonCriteria);
            this.Controls.Add(this.tbJurisdiction);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbBenefitLimit);
            this.Controls.Add(this.labelBenefitLimit);
            this.Controls.Add(this.cbWeighting);
            this.Controls.Add(this.labelWeighting);
            this.Controls.Add(this.cbBenefit);
            this.Controls.Add(this.labelBenefit);
            this.Controls.Add(this.cbBudget);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbDescription);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbOptimization);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControlAnalysis);
            this.Controls.Add(this.labelAnalysis);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormAnalysis";
            this.TabText = "FormAnalysis";
            this.Text = "FormAnalysis";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAnalysis_FormClosed);
            this.Load += new System.EventHandler(this.FormAnalysis_Load);
            this.tabControlAnalysis.ResumeLayout(false);
            this.tabPagePriority.ResumeLayout(false);
            this.tabPagePriority.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPriority)).EndInit();
            this.tabPageTarget.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTarget)).EndInit();
            this.tabPageDeficient.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeficient)).EndInit();
            this.tabRemainingLife.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRemainLife)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelAnalysis;
        private System.Windows.Forms.TabControl tabControlAnalysis;
        private System.Windows.Forms.TabPage tabPagePriority;
        private System.Windows.Forms.DataGridView dgvPriority;
        private System.Windows.Forms.TabPage tabPageTarget;
        private System.Windows.Forms.TabPage tabPageDeficient;
        private System.Windows.Forms.DataGridView dgvTarget;
        private System.Windows.Forms.DataGridView dgvDeficient;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbOptimization;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbBudget;
        private System.Windows.Forms.Label labelBenefit;
        private System.Windows.Forms.ComboBox cbBenefit;
        private System.Windows.Forms.Label labelWeighting;
        private System.Windows.Forms.ComboBox cbWeighting;
        private System.Windows.Forms.Label labelBenefitLimit;
        private System.Windows.Forms.TextBox tbBenefitLimit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbJurisdiction;
        private System.Windows.Forms.Button buttonCriteria;
        private System.Windows.Forms.Button buttonRunSimulation;
        private System.Windows.Forms.Timer timerSimulation;
        private System.Windows.Forms.DataGridViewComboBoxColumn Attribute;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameTarget;
        private System.Windows.Forms.DataGridViewTextBoxColumn Years;
        private System.Windows.Forms.DataGridViewTextBoxColumn Target;
        private System.Windows.Forms.DataGridViewTextBoxColumn Criteria;
        private System.Windows.Forms.DataGridViewComboBoxColumn DeficientAttribute;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeficientName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Deficient;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeficientPercent;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeficientCriteria;
		private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox checkBoxMultipleCost;
        private System.Windows.Forms.RadioButton radioButtonAcrossBudget;
        private System.Windows.Forms.RadioButton radioButtonWithinBudget;
        private System.Windows.Forms.TabPage tabRemainingLife;
        private System.Windows.Forms.DataGridView dataGridViewRemainLife;
        private System.Windows.Forms.DataGridViewComboBoxColumn RemainingLifeAttribute;
        private System.Windows.Forms.DataGridViewTextBoxColumn RemainingLifeLimit;
        private System.Windows.Forms.DataGridViewTextBoxColumn RemainingLifeCriteria;
    }
}