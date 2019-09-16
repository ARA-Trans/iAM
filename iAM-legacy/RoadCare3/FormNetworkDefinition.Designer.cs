namespace RoadCare3
{
    partial class FormNetworkDefinition
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNetworkDefinition));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvNetworkDefinition = new System.Windows.Forms.DataGridView();
            this.cmsNetworkDefintion = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.labelNetworkDefinition = new System.Windows.Forms.Label();
            this.labelUnits = new System.Windows.Forms.Label();
            this.comboFacility = new System.Windows.Forms.ComboBox();
            this.labelFacility = new System.Windows.Forms.Label();
            this.checkBoxAllowEdit = new System.Windows.Forms.CheckBox();
            this.btnImportGeometries = new System.Windows.Forms.Button();
            this.toolTipImportGeometries = new System.Windows.Forms.ToolTip(this.components);
            this.btnImportShapefile = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.dgvShapeFileOnlyNDDs = new System.Windows.Forms.DataGridView();
            this.cmsAddShapeFileRows = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddAll = new System.Windows.Forms.ToolStripMenuItem();
            this.cbUnits = new System.Windows.Forms.ComboBox();
            this.bindingNavigatorNetworkDefinition = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonVerifyNetwork = new System.Windows.Forms.ToolStripButton();
            this.tsbJoinMultilinestring = new System.Windows.Forms.ToolStripButton();
            this.tsbDefineMultiLineStrings = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.tsbUpdate = new System.Windows.Forms.ToolStripLabel();
            this.comboBoxUpdateNetworkDefinition = new System.Windows.Forms.ToolStripComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNetworkDefinition)).BeginInit();
            this.cmsNetworkDefintion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvShapeFileOnlyNDDs)).BeginInit();
            this.cmsAddShapeFileRows.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorNetworkDefinition)).BeginInit();
            this.bindingNavigatorNetworkDefinition.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvNetworkDefinition
            // 
            this.dgvNetworkDefinition.AllowUserToDeleteRows = false;
            this.dgvNetworkDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvNetworkDefinition.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvNetworkDefinition.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNetworkDefinition.ContextMenuStrip = this.cmsNetworkDefintion;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvNetworkDefinition.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvNetworkDefinition.Location = new System.Drawing.Point(-1, 66);
            this.dgvNetworkDefinition.Margin = new System.Windows.Forms.Padding(2);
            this.dgvNetworkDefinition.Name = "dgvNetworkDefinition";
            this.dgvNetworkDefinition.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvNetworkDefinition.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvNetworkDefinition.RowTemplate.Height = 24;
            this.dgvNetworkDefinition.Size = new System.Drawing.Size(1011, 442);
            this.dgvNetworkDefinition.TabIndex = 0;
            this.dgvNetworkDefinition.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgvNetworkDefinition_KeyUp);
            this.dgvNetworkDefinition.Leave += new System.EventHandler(this.dgvNetworkDefinition_Leave);
            // 
            // cmsNetworkDefintion
            // 
            this.cmsNetworkDefintion.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopy,
            this.tsmiPaste,
            this.tsmiDelete});
            this.cmsNetworkDefintion.Name = "cmsNetworkDefintion";
            this.cmsNetworkDefintion.Size = new System.Drawing.Size(108, 70);
            // 
            // tsmiCopy
            // 
            this.tsmiCopy.Name = "tsmiCopy";
            this.tsmiCopy.Size = new System.Drawing.Size(107, 22);
            this.tsmiCopy.Text = "Copy";
            this.tsmiCopy.Click += new System.EventHandler(this.tsmiCopy_Click);
            // 
            // tsmiPaste
            // 
            this.tsmiPaste.Name = "tsmiPaste";
            this.tsmiPaste.Size = new System.Drawing.Size(107, 22);
            this.tsmiPaste.Text = "Paste";
            this.tsmiPaste.Click += new System.EventHandler(this.tsmiPaste_Click);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(107, 22);
            this.tsmiDelete.Text = "Delete";
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // labelNetworkDefinition
            // 
            this.labelNetworkDefinition.AutoSize = true;
            this.labelNetworkDefinition.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNetworkDefinition.Location = new System.Drawing.Point(54, 11);
            this.labelNetworkDefinition.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelNetworkDefinition.Name = "labelNetworkDefinition";
            this.labelNetworkDefinition.Size = new System.Drawing.Size(233, 26);
            this.labelNetworkDefinition.TabIndex = 2;
            this.labelNetworkDefinition.Text = "Linear Route Definition";
            // 
            // labelUnits
            // 
            this.labelUnits.AutoSize = true;
            this.labelUnits.Location = new System.Drawing.Point(9, 44);
            this.labelUnits.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelUnits.Name = "labelUnits";
            this.labelUnits.Size = new System.Drawing.Size(139, 13);
            this.labelUnits.TabIndex = 3;
            this.labelUnits.Text = "Linear route stationing units:";
            // 
            // comboFacility
            // 
            this.comboFacility.FormattingEnabled = true;
            this.comboFacility.Location = new System.Drawing.Point(332, 41);
            this.comboFacility.Margin = new System.Windows.Forms.Padding(2);
            this.comboFacility.Name = "comboFacility";
            this.comboFacility.Size = new System.Drawing.Size(222, 21);
            this.comboFacility.TabIndex = 4;
            this.comboFacility.Visible = false;
            this.comboFacility.SelectedIndexChanged += new System.EventHandler(this.comboFacility_SelectedIndexChanged);
            // 
            // labelFacility
            // 
            this.labelFacility.AutoSize = true;
            this.labelFacility.Location = new System.Drawing.Point(286, 43);
            this.labelFacility.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelFacility.Name = "labelFacility";
            this.labelFacility.Size = new System.Drawing.Size(42, 13);
            this.labelFacility.TabIndex = 5;
            this.labelFacility.Text = "Facility:";
            this.labelFacility.Visible = false;
            // 
            // checkBoxAllowEdit
            // 
            this.checkBoxAllowEdit.AutoSize = true;
            this.checkBoxAllowEdit.Location = new System.Drawing.Point(562, 13);
            this.checkBoxAllowEdit.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxAllowEdit.Name = "checkBoxAllowEdit";
            this.checkBoxAllowEdit.Size = new System.Drawing.Size(119, 17);
            this.checkBoxAllowEdit.TabIndex = 8;
            this.checkBoxAllowEdit.Text = "Allow Definition Edit";
            this.checkBoxAllowEdit.UseVisualStyleBackColor = true;
            this.checkBoxAllowEdit.CheckedChanged += new System.EventHandler(this.checkBoxAllowEdit_CheckedChanged);
            // 
            // btnImportGeometries
            // 
            this.btnImportGeometries.Enabled = false;
            this.btnImportGeometries.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportGeometries.Location = new System.Drawing.Point(562, 34);
            this.btnImportGeometries.Margin = new System.Windows.Forms.Padding(2);
            this.btnImportGeometries.Name = "btnImportGeometries";
            this.btnImportGeometries.Size = new System.Drawing.Size(156, 27);
            this.btnImportGeometries.TabIndex = 9;
            this.btnImportGeometries.Text = "Import Geometries...";
            this.toolTipImportGeometries.SetToolTip(this.btnImportGeometries, "Import geometries from external files such as ESRI .shp files.");
            this.btnImportGeometries.UseVisualStyleBackColor = true;
            this.btnImportGeometries.Click += new System.EventHandler(this.btnImportGeometries_Click);
            // 
            // btnImportShapefile
            // 
            this.btnImportShapefile.Enabled = false;
            this.btnImportShapefile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportShapefile.Location = new System.Drawing.Point(722, 34);
            this.btnImportShapefile.Margin = new System.Windows.Forms.Padding(2);
            this.btnImportShapefile.Name = "btnImportShapefile";
            this.btnImportShapefile.Size = new System.Drawing.Size(156, 27);
            this.btnImportShapefile.TabIndex = 10;
            this.btnImportShapefile.Text = "Import Shapefile...";
            this.toolTipImportGeometries.SetToolTip(this.btnImportShapefile, "Import geometries from external files such as ESRI .shp files.");
            this.btnImportShapefile.UseVisualStyleBackColor = true;
            this.btnImportShapefile.Click += new System.EventHandler(this.btnImportShapefile_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 11);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(28, 30);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // dgvShapeFileOnlyNDDs
            // 
            this.dgvShapeFileOnlyNDDs.AllowUserToAddRows = false;
            this.dgvShapeFileOnlyNDDs.AllowUserToDeleteRows = false;
            this.dgvShapeFileOnlyNDDs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvShapeFileOnlyNDDs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvShapeFileOnlyNDDs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvShapeFileOnlyNDDs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvShapeFileOnlyNDDs.ContextMenuStrip = this.cmsAddShapeFileRows;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvShapeFileOnlyNDDs.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvShapeFileOnlyNDDs.Location = new System.Drawing.Point(-1, 315);
            this.dgvShapeFileOnlyNDDs.Margin = new System.Windows.Forms.Padding(2);
            this.dgvShapeFileOnlyNDDs.Name = "dgvShapeFileOnlyNDDs";
            this.dgvShapeFileOnlyNDDs.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvShapeFileOnlyNDDs.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvShapeFileOnlyNDDs.RowTemplate.Height = 24;
            this.dgvShapeFileOnlyNDDs.Size = new System.Drawing.Size(1011, 193);
            this.dgvShapeFileOnlyNDDs.TabIndex = 11;
            this.dgvShapeFileOnlyNDDs.Visible = false;
            // 
            // cmsAddShapeFileRows
            // 
            this.cmsAddShapeFileRows.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddSelected,
            this.tsmiAddAll});
            this.cmsAddShapeFileRows.Name = "cmsAddShapeFileRows";
            this.cmsAddShapeFileRows.Size = new System.Drawing.Size(175, 48);
            this.cmsAddShapeFileRows.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsAddShapeFileRows_ItemClicked);
            // 
            // tsmiAddSelected
            // 
            this.tsmiAddSelected.Name = "tsmiAddSelected";
            this.tsmiAddSelected.Size = new System.Drawing.Size(174, 22);
            this.tsmiAddSelected.Text = "Add Selected Rows";
            // 
            // tsmiAddAll
            // 
            this.tsmiAddAll.Name = "tsmiAddAll";
            this.tsmiAddAll.Size = new System.Drawing.Size(174, 22);
            this.tsmiAddAll.Text = "Add All Rows";
            // 
            // cbUnits
            // 
            this.cbUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUnits.FormattingEnabled = true;
            this.cbUnits.Items.AddRange(new object[] {
            "Miles",
            "Feet",
            "Kilometers",
            "Meters",
            "Yards",
            "Rods"});
            this.cbUnits.Location = new System.Drawing.Point(165, 40);
            this.cbUnits.Margin = new System.Windows.Forms.Padding(2);
            this.cbUnits.Name = "cbUnits";
            this.cbUnits.Size = new System.Drawing.Size(116, 21);
            this.cbUnits.TabIndex = 12;
            this.cbUnits.SelectedIndexChanged += new System.EventHandler(this.cbUnits_SelectedIndexChanged);
            // 
            // bindingNavigatorNetworkDefinition
            // 
            this.bindingNavigatorNetworkDefinition.AddNewItem = this.bindingNavigatorAddNewItem;
            this.bindingNavigatorNetworkDefinition.BackColor = System.Drawing.SystemColors.Control;
            this.bindingNavigatorNetworkDefinition.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigatorNetworkDefinition.DeleteItem = this.bindingNavigatorDeleteItem;
            this.bindingNavigatorNetworkDefinition.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bindingNavigatorNetworkDefinition.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem,
            this.toolStripButtonVerifyNetwork,
            this.tsbJoinMultilinestring,
            this.tsbDefineMultiLineStrings,
            this.toolStripButton1,
            this.tsbUpdate,
            this.comboBoxUpdateNetworkDefinition});
            this.bindingNavigatorNetworkDefinition.Location = new System.Drawing.Point(0, 508);
            this.bindingNavigatorNetworkDefinition.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigatorNetworkDefinition.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigatorNetworkDefinition.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigatorNetworkDefinition.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigatorNetworkDefinition.Name = "bindingNavigatorNetworkDefinition";
            this.bindingNavigatorNetworkDefinition.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigatorNetworkDefinition.Size = new System.Drawing.Size(1012, 27);
            this.bindingNavigatorNetworkDefinition.TabIndex = 13;
            this.bindingNavigatorNetworkDefinition.Text = "bindingNavigator1";
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(23, 24);
            this.bindingNavigatorAddNewItem.Text = "Add new";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(35, 24);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(23, 24);
            this.bindingNavigatorDeleteItem.Text = "Delete";
            this.bindingNavigatorDeleteItem.Click += new System.EventHandler(this.dgvNetworkDefinition_Leave);
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 24);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 24);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 27);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 21);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 24);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 24);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButtonVerifyNetwork
            // 
            this.toolStripButtonVerifyNetwork.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonVerifyNetwork.Image")));
            this.toolStripButtonVerifyNetwork.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonVerifyNetwork.Name = "toolStripButtonVerifyNetwork";
            this.toolStripButtonVerifyNetwork.Size = new System.Drawing.Size(105, 24);
            this.toolStripButtonVerifyNetwork.Text = "Verify Network";
            this.toolStripButtonVerifyNetwork.Click += new System.EventHandler(this.toolStripButtonVerifyNetwork_Click);
            // 
            // tsbJoinMultilinestring
            // 
            this.tsbJoinMultilinestring.Image = ((System.Drawing.Image)(resources.GetObject("tsbJoinMultilinestring.Image")));
            this.tsbJoinMultilinestring.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbJoinMultilinestring.Name = "tsbJoinMultilinestring";
            this.tsbJoinMultilinestring.Size = new System.Drawing.Size(146, 24);
            this.tsbJoinMultilinestring.Text = "Join MultiLineStrings...";
            this.tsbJoinMultilinestring.Click += new System.EventHandler(this.tsbJoinMultilinestring_Click);
            // 
            // tsbDefineMultiLineStrings
            // 
            this.tsbDefineMultiLineStrings.Image = ((System.Drawing.Image)(resources.GetObject("tsbDefineMultiLineStrings.Image")));
            this.tsbDefineMultiLineStrings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDefineMultiLineStrings.Name = "tsbDefineMultiLineStrings";
            this.tsbDefineMultiLineStrings.Size = new System.Drawing.Size(159, 24);
            this.tsbDefineMultiLineStrings.Text = "Define MultiLineStrings...";
            this.tsbDefineMultiLineStrings.Click += new System.EventHandler(this.tsbDefineMultiLineStrings_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(121, 24);
            this.toolStripButton1.Text = "Create Shapefile...";
            // 
            // tsbUpdate
            // 
            this.tsbUpdate.Image = ((System.Drawing.Image)(resources.GetObject("tsbUpdate.Image")));
            this.tsbUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUpdate.Name = "tsbUpdate";
            this.tsbUpdate.Size = new System.Drawing.Size(96, 24);
            this.tsbUpdate.Text = "Update using:";
            // 
            // comboBoxUpdateNetworkDefinition
            // 
            this.comboBoxUpdateNetworkDefinition.MaxDropDownItems = 20;
            this.comboBoxUpdateNetworkDefinition.Name = "comboBoxUpdateNetworkDefinition";
            this.comboBoxUpdateNetworkDefinition.Size = new System.Drawing.Size(200, 23);
            this.comboBoxUpdateNetworkDefinition.SelectedIndexChanged += new System.EventHandler(this.comboBoxUpdateNetworkDefinition_SelectedIndexChanged);
            // 
            // FormNetworkDefinition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 535);
            this.Controls.Add(this.bindingNavigatorNetworkDefinition);
            this.Controls.Add(this.cbUnits);
            this.Controls.Add(this.dgvShapeFileOnlyNDDs);
            this.Controls.Add(this.btnImportShapefile);
            this.Controls.Add(this.btnImportGeometries);
            this.Controls.Add(this.checkBoxAllowEdit);
            this.Controls.Add(this.labelFacility);
            this.Controls.Add(this.comboFacility);
            this.Controls.Add(this.labelNetworkDefinition);
            this.Controls.Add(this.labelUnits);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.dgvNetworkDefinition);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormNetworkDefinition";
            this.TabText = "FormNetworkDefinition";
            this.Text = "FormNetworkDefinition";
            this.Deactivate += new System.EventHandler(this.FormNetworkDefinition_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormNetworkDefinition_FormClosing);
            this.Load += new System.EventHandler(this.FormNetworkDefinition_Load);
            this.VisibleChanged += new System.EventHandler(this.FormNetworkDefinition_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNetworkDefinition)).EndInit();
            this.cmsNetworkDefintion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvShapeFileOnlyNDDs)).EndInit();
            this.cmsAddShapeFileRows.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorNetworkDefinition)).EndInit();
            this.bindingNavigatorNetworkDefinition.ResumeLayout(false);
            this.bindingNavigatorNetworkDefinition.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvNetworkDefinition;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelNetworkDefinition;
        private System.Windows.Forms.Label labelUnits;
        private System.Windows.Forms.ComboBox comboFacility;
        private System.Windows.Forms.Label labelFacility;
        private System.Windows.Forms.CheckBox checkBoxAllowEdit;
        private System.Windows.Forms.Button btnImportGeometries;
        private System.Windows.Forms.ToolTip toolTipImportGeometries;
        private System.Windows.Forms.Button btnImportShapefile;
        private System.Windows.Forms.DataGridView dgvShapeFileOnlyNDDs;
        private System.Windows.Forms.ContextMenuStrip cmsAddShapeFileRows;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddSelected;
		private System.Windows.Forms.ToolStripMenuItem tsmiAddAll;
        private System.Windows.Forms.ComboBox cbUnits;
        private System.Windows.Forms.BindingNavigator bindingNavigatorNetworkDefinition;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ContextMenuStrip cmsNetworkDefintion;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopy;
        private System.Windows.Forms.ToolStripMenuItem tsmiPaste;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripButton toolStripButtonVerifyNetwork;
		private System.Windows.Forms.ToolStripButton tsbJoinMultilinestring;
		private System.Windows.Forms.ToolStripButton tsbDefineMultiLineStrings;
        private System.Windows.Forms.ToolStripComboBox comboBoxUpdateNetworkDefinition;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripLabel tsbUpdate;
    }
}
