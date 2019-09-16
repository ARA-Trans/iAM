namespace RoadCare3
{
    partial class FormAttributeView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAttributeView));
            this.dgvAttributeView = new System.Windows.Forms.DataGridView();
            this.contextMenuStripAttibuteView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setLimitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topToggleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelAttribute = new System.Windows.Forms.Label();
            this.comboBoxFilterAttribute = new System.Windows.Forms.ComboBox();
            this.comboBoxRouteFacilty = new System.Windows.Forms.ComboBox();
            this.checkBoxCustomFilter = new System.Windows.Forms.CheckBox();
            this.comboBoxAttributeValue = new System.Windows.Forms.ComboBox();
            this.labelRouteFacility = new System.Windows.Forms.Label();
            this.labelAdvancedSearch = new System.Windows.Forms.Label();
            this.textBoxAdvanceSearch = new System.Windows.Forms.TextBox();
            this.buttonAdvancedSearch = new System.Windows.Forms.Button();
            this.checkMilepost = new System.Windows.Forms.CheckBox();
            this.bindingNavigatorAttributeView = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBoxSimulation = new System.Windows.Forms.ToolStripComboBox();
            this.btnARAN = new System.Windows.Forms.ToolStripButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.buttonEditColumns = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.menuStripViewerTools = new System.Windows.Forms.MenuStrip();
            this.viewerOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.validationLimitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.validationLimitsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fontsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttributeView)).BeginInit();
            this.contextMenuStripAttibuteView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorAttributeView)).BeginInit();
            this.bindingNavigatorAttributeView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStripViewerTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvAttributeView
            // 
            this.dgvAttributeView.AllowUserToAddRows = false;
            this.dgvAttributeView.AllowUserToDeleteRows = false;
            this.dgvAttributeView.AllowUserToOrderColumns = true;
            this.dgvAttributeView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAttributeView.ContextMenuStrip = this.contextMenuStripAttibuteView;
            this.dgvAttributeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAttributeView.Location = new System.Drawing.Point(0, 0);
            this.dgvAttributeView.Margin = new System.Windows.Forms.Padding(2);
            this.dgvAttributeView.Name = "dgvAttributeView";
            this.dgvAttributeView.RowTemplate.Height = 24;
            this.dgvAttributeView.Size = new System.Drawing.Size(985, 282);
            this.dgvAttributeView.TabIndex = 0;
            this.dgvAttributeView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvAttributeView_DataError);
            this.dgvAttributeView.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvAttributeView_RowHeaderMouseDoubleClick);
            // 
            // contextMenuStripAttibuteView
            // 
            this.contextMenuStripAttibuteView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setLimitsToolStripMenuItem,
            this.setFontToolStripMenuItem,
            this.editColumnsToolStripMenuItem,
            this.updateToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.topToggleToolStripMenuItem});
            this.contextMenuStripAttibuteView.Name = "contextMenuStripAttibuteView";
            this.contextMenuStripAttibuteView.Size = new System.Drawing.Size(182, 136);
            // 
            // setLimitsToolStripMenuItem
            // 
            this.setLimitsToolStripMenuItem.Name = "setLimitsToolStripMenuItem";
            this.setLimitsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.setLimitsToolStripMenuItem.Text = "Set Validation Limits";
            this.setLimitsToolStripMenuItem.Click += new System.EventHandler(this.setLimitsToolStripMenuItem_Click);
            // 
            // setFontToolStripMenuItem
            // 
            this.setFontToolStripMenuItem.Name = "setFontToolStripMenuItem";
            this.setFontToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.setFontToolStripMenuItem.Text = "Set Font";
            this.setFontToolStripMenuItem.Click += new System.EventHandler(this.setFontToolStripMenuItem_Click);
            // 
            // editColumnsToolStripMenuItem
            // 
            this.editColumnsToolStripMenuItem.Name = "editColumnsToolStripMenuItem";
            this.editColumnsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.editColumnsToolStripMenuItem.Text = "&Edit Columns";
            this.editColumnsToolStripMenuItem.Click += new System.EventHandler(this.editColumnsToolStripMenuItem_Click);
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.updateToolStripMenuItem.Text = "&Update";
            this.updateToolStripMenuItem.Click += new System.EventHandler(this.updateToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // topToggleToolStripMenuItem
            // 
            this.topToggleToolStripMenuItem.Name = "topToggleToolStripMenuItem";
            this.topToggleToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.topToggleToolStripMenuItem.Text = "Hide Top";
            this.topToggleToolStripMenuItem.Click += new System.EventHandler(this.topToggleToolStripMenuItem_Click);
            // 
            // labelAttribute
            // 
            this.labelAttribute.AutoSize = true;
            this.labelAttribute.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAttribute.Location = new System.Drawing.Point(53, 21);
            this.labelAttribute.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAttribute.Name = "labelAttribute";
            this.labelAttribute.Size = new System.Drawing.Size(246, 26);
            this.labelAttribute.TabIndex = 1;
            this.labelAttribute.Text = "Attribute View - Network";
            // 
            // comboBoxFilterAttribute
            // 
            this.comboBoxFilterAttribute.FormattingEnabled = true;
            this.comboBoxFilterAttribute.Location = new System.Drawing.Point(42, 54);
            this.comboBoxFilterAttribute.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxFilterAttribute.Name = "comboBoxFilterAttribute";
            this.comboBoxFilterAttribute.Size = new System.Drawing.Size(138, 21);
            this.comboBoxFilterAttribute.TabIndex = 3;
            this.comboBoxFilterAttribute.Visible = false;
            this.comboBoxFilterAttribute.SelectedIndexChanged += new System.EventHandler(this.comboBoxFilterAttribute_SelectedIndexChanged);
            // 
            // comboBoxRouteFacilty
            // 
            this.comboBoxRouteFacilty.FormattingEnabled = true;
            this.comboBoxRouteFacilty.Location = new System.Drawing.Point(384, 52);
            this.comboBoxRouteFacilty.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxRouteFacilty.Name = "comboBoxRouteFacilty";
            this.comboBoxRouteFacilty.Size = new System.Drawing.Size(157, 21);
            this.comboBoxRouteFacilty.Sorted = true;
            this.comboBoxRouteFacilty.TabIndex = 4;
            this.comboBoxRouteFacilty.SelectedIndexChanged += new System.EventHandler(this.comboBoxRouteFacilty_SelectedIndexChanged);
            // 
            // checkBoxCustomFilter
            // 
            this.checkBoxCustomFilter.AutoSize = true;
            this.checkBoxCustomFilter.Location = new System.Drawing.Point(26, 56);
            this.checkBoxCustomFilter.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxCustomFilter.Name = "checkBoxCustomFilter";
            this.checkBoxCustomFilter.Size = new System.Drawing.Size(118, 17);
            this.checkBoxCustomFilter.TabIndex = 2;
            this.checkBoxCustomFilter.Text = "Enable custom filter";
            this.checkBoxCustomFilter.UseVisualStyleBackColor = true;
            this.checkBoxCustomFilter.CheckedChanged += new System.EventHandler(this.checkBoxCustomFilter_CheckedChanged);
            // 
            // comboBoxAttributeValue
            // 
            this.comboBoxAttributeValue.FormattingEnabled = true;
            this.comboBoxAttributeValue.Location = new System.Drawing.Point(183, 53);
            this.comboBoxAttributeValue.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxAttributeValue.Name = "comboBoxAttributeValue";
            this.comboBoxAttributeValue.Size = new System.Drawing.Size(152, 21);
            this.comboBoxAttributeValue.TabIndex = 5;
            this.comboBoxAttributeValue.Visible = false;
            this.comboBoxAttributeValue.SelectedIndexChanged += new System.EventHandler(this.comboBoxAttributeValue_SelectedIndexChanged);
            // 
            // labelRouteFacility
            // 
            this.labelRouteFacility.AutoSize = true;
            this.labelRouteFacility.Location = new System.Drawing.Point(339, 55);
            this.labelRouteFacility.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelRouteFacility.Name = "labelRouteFacility";
            this.labelRouteFacility.Size = new System.Drawing.Size(42, 13);
            this.labelRouteFacility.TabIndex = 6;
            this.labelRouteFacility.Text = "Facility:";
            // 
            // labelAdvancedSearch
            // 
            this.labelAdvancedSearch.AutoSize = true;
            this.labelAdvancedSearch.Location = new System.Drawing.Point(10, 79);
            this.labelAdvancedSearch.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAdvancedSearch.Name = "labelAdvancedSearch";
            this.labelAdvancedSearch.Size = new System.Drawing.Size(96, 13);
            this.labelAdvancedSearch.TabIndex = 7;
            this.labelAdvancedSearch.Text = "Advanced Search:";
            // 
            // textBoxAdvanceSearch
            // 
            this.textBoxAdvanceSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAdvanceSearch.Location = new System.Drawing.Point(109, 76);
            this.textBoxAdvanceSearch.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxAdvanceSearch.Name = "textBoxAdvanceSearch";
            this.textBoxAdvanceSearch.Size = new System.Drawing.Size(833, 20);
            this.textBoxAdvanceSearch.TabIndex = 8;
            // 
            // buttonAdvancedSearch
            // 
            this.buttonAdvancedSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAdvancedSearch.Location = new System.Drawing.Point(946, 76);
            this.buttonAdvancedSearch.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAdvancedSearch.Name = "buttonAdvancedSearch";
            this.buttonAdvancedSearch.Size = new System.Drawing.Size(28, 19);
            this.buttonAdvancedSearch.TabIndex = 9;
            this.buttonAdvancedSearch.Text = "...";
            this.buttonAdvancedSearch.UseVisualStyleBackColor = true;
            this.buttonAdvancedSearch.Click += new System.EventHandler(this.buttonAdvancedSearch_Click);
            // 
            // checkMilepost
            // 
            this.checkMilepost.AutoSize = true;
            this.checkMilepost.Location = new System.Drawing.Point(545, 56);
            this.checkMilepost.Margin = new System.Windows.Forms.Padding(2);
            this.checkMilepost.Name = "checkMilepost";
            this.checkMilepost.Size = new System.Drawing.Size(154, 17);
            this.checkMilepost.TabIndex = 15;
            this.checkMilepost.Text = "Show Begin/End/Direction";
            this.checkMilepost.UseVisualStyleBackColor = true;
            this.checkMilepost.CheckedChanged += new System.EventHandler(this.checkMilepost_CheckedChanged);
            // 
            // bindingNavigatorAttributeView
            // 
            this.bindingNavigatorAttributeView.AddNewItem = null;
            this.bindingNavigatorAttributeView.BackColor = System.Drawing.SystemColors.Control;
            this.bindingNavigatorAttributeView.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigatorAttributeView.DeleteItem = null;
            this.bindingNavigatorAttributeView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bindingNavigatorAttributeView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.toolStripLabel1,
            this.toolStripComboBoxSimulation,
            this.btnARAN});
            this.bindingNavigatorAttributeView.Location = new System.Drawing.Point(0, 395);
            this.bindingNavigatorAttributeView.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigatorAttributeView.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigatorAttributeView.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigatorAttributeView.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigatorAttributeView.Name = "bindingNavigatorAttributeView";
            this.bindingNavigatorAttributeView.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigatorAttributeView.Size = new System.Drawing.Size(985, 25);
            this.bindingNavigatorAttributeView.TabIndex = 16;
            this.bindingNavigatorAttributeView.Text = "bindingNavigator1";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(35, 22);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
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
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(64, 22);
            this.toolStripLabel1.Text = "Simulation";
            // 
            // toolStripComboBoxSimulation
            // 
            this.toolStripComboBoxSimulation.Name = "toolStripComboBoxSimulation";
            this.toolStripComboBoxSimulation.Size = new System.Drawing.Size(121, 25);
            // 
            // btnARAN
            // 
            this.btnARAN.Image = global::RoadCare3.Properties.Resources.ModalPopup;
            this.btnARAN.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnARAN.Name = "btnARAN";
            this.btnARAN.Size = new System.Drawing.Size(87, 22);
            this.btnARAN.Text = "ARAN View";
            this.btnARAN.Visible = false;
            this.btnARAN.Click += new System.EventHandler(this.btnARAN_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RoadCare3.Properties.Resources.bigclass;
            this.pictureBox1.Location = new System.Drawing.Point(17, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(31, 35);
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUpdate.Image = global::RoadCare3.Properties.Resources.update;
            this.buttonUpdate.Location = new System.Drawing.Point(935, 10);
            this.buttonUpdate.Margin = new System.Windows.Forms.Padding(2);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(39, 37);
            this.buttonUpdate.TabIndex = 14;
            this.buttonUpdate.UseVisualStyleBackColor = true;
            this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
            // 
            // buttonEditColumns
            // 
            this.buttonEditColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEditColumns.Image = global::RoadCare3.Properties.Resources.selectColumns;
            this.buttonEditColumns.Location = new System.Drawing.Point(892, 11);
            this.buttonEditColumns.Margin = new System.Windows.Forms.Padding(2);
            this.buttonEditColumns.Name = "buttonEditColumns";
            this.buttonEditColumns.Size = new System.Drawing.Size(39, 37);
            this.buttonEditColumns.TabIndex = 12;
            this.buttonEditColumns.UseVisualStyleBackColor = true;
            this.buttonEditColumns.Click += new System.EventHandler(this.buttonEditColumns_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            this.splitContainer1.Panel1.Controls.Add(this.labelAttribute);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxCustomFilter);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxFilterAttribute);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxRouteFacilty);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxAttributeValue);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxAdvanceSearch);
            this.splitContainer1.Panel1.Controls.Add(this.labelAdvancedSearch);
            this.splitContainer1.Panel1.Controls.Add(this.checkMilepost);
            this.splitContainer1.Panel1.Controls.Add(this.labelRouteFacility);
            this.splitContainer1.Panel1.Controls.Add(this.buttonUpdate);
            this.splitContainer1.Panel1.Controls.Add(this.buttonEditColumns);
            this.splitContainer1.Panel1.Controls.Add(this.buttonAdvancedSearch);
            this.splitContainer1.Panel1MinSize = 109;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvAttributeView);
            this.splitContainer1.Panel2.Controls.Add(this.menuStripViewerTools);
            this.splitContainer1.Size = new System.Drawing.Size(985, 395);
            this.splitContainer1.SplitterDistance = 109;
            this.splitContainer1.TabIndex = 20;
            // 
            // menuStripViewerTools
            // 
            this.menuStripViewerTools.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStripViewerTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewerOptionsToolStripMenuItem});
            this.menuStripViewerTools.Location = new System.Drawing.Point(0, 0);
            this.menuStripViewerTools.Name = "menuStripViewerTools";
            this.menuStripViewerTools.Size = new System.Drawing.Size(107, 24);
            this.menuStripViewerTools.TabIndex = 1;
            this.menuStripViewerTools.Text = "Viewer Tools";
            // 
            // viewerOptionsToolStripMenuItem
            // 
            this.viewerOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.validationLimitsToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.validationLimitsToolStripMenuItem1,
            this.fontsToolStripMenuItem,
            this.propertiesToolStripMenuItem});
            this.viewerOptionsToolStripMenuItem.Name = "viewerOptionsToolStripMenuItem";
            this.viewerOptionsToolStripMenuItem.Size = new System.Drawing.Size(99, 20);
            this.viewerOptionsToolStripMenuItem.Text = "Viewer Options";
            // 
            // validationLimitsToolStripMenuItem
            // 
            this.validationLimitsToolStripMenuItem.Name = "validationLimitsToolStripMenuItem";
            this.validationLimitsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.validationLimitsToolStripMenuItem.Text = "Save...";
            this.validationLimitsToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.loadToolStripMenuItem.Text = "Load...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // validationLimitsToolStripMenuItem1
            // 
            this.validationLimitsToolStripMenuItem1.Name = "validationLimitsToolStripMenuItem1";
            this.validationLimitsToolStripMenuItem1.Size = new System.Drawing.Size(171, 22);
            this.validationLimitsToolStripMenuItem1.Text = "Validation Limits...";
            this.validationLimitsToolStripMenuItem1.Click += new System.EventHandler(this.validationLimitsToolStripMenuItem1_Click);
            // 
            // fontsToolStripMenuItem
            // 
            this.fontsToolStripMenuItem.Name = "fontsToolStripMenuItem";
            this.fontsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.fontsToolStripMenuItem.Text = "Fonts...";
            this.fontsToolStripMenuItem.Click += new System.EventHandler(this.fontsToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.propertiesToolStripMenuItem.Text = "Properties...";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // FormAttributeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(985, 420);
            this.ContextMenuStrip = this.contextMenuStripAttibuteView;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.bindingNavigatorAttributeView);
            this.MainMenuStrip = this.menuStripViewerTools;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormAttributeView";
            this.TabText = "FormAttributeView";
            this.Text = "FormAttributeView";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAttributeView_FormClosed);
            this.Load += new System.EventHandler(this.FormAttributeView_Load);
            this.LocationChanged += new System.EventHandler(this.FormAttributeView_LocationChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttributeView)).EndInit();
            this.contextMenuStripAttibuteView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorAttributeView)).EndInit();
            this.bindingNavigatorAttributeView.ResumeLayout(false);
            this.bindingNavigatorAttributeView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.menuStripViewerTools.ResumeLayout(false);
            this.menuStripViewerTools.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAttributeView;
        private System.Windows.Forms.Label labelAttribute;
        private System.Windows.Forms.ComboBox comboBoxFilterAttribute;
        private System.Windows.Forms.ComboBox comboBoxRouteFacilty;
        private System.Windows.Forms.CheckBox checkBoxCustomFilter;
        private System.Windows.Forms.ComboBox comboBoxAttributeValue;
        private System.Windows.Forms.Label labelRouteFacility;
        private System.Windows.Forms.Label labelAdvancedSearch;
        private System.Windows.Forms.TextBox textBoxAdvanceSearch;
		private System.Windows.Forms.Button buttonAdvancedSearch;
		private System.Windows.Forms.Button buttonEditColumns;
        private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAttibuteView;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editColumnsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkMilepost;
        private System.Windows.Forms.BindingNavigator bindingNavigatorAttributeView;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
		private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBoxSimulation;
        private System.Windows.Forms.ToolStripButton btnARAN;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolStripMenuItem setLimitsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setFontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topToggleToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStripViewerTools;
        private System.Windows.Forms.ToolStripMenuItem viewerOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem validationLimitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem validationLimitsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fontsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
    }
}