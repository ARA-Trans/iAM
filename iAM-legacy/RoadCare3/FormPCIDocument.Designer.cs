namespace RoadCare3
{
	partial class FormPCIDocument
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPCIDocument));
            this.checkAllowEdit = new System.Windows.Forms.CheckBox();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelServer = new System.Windows.Forms.Label();
            this.contextMenuStripPCIDocument = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelPCI = new System.Windows.Forms.Label();
            this.lblAdvancedSearch = new System.Windows.Forms.Label();
            this.tbSearch = new System.Windows.Forms.TextBox();
            this.lblYears = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cbYear = new System.Windows.Forms.ComboBox();
            this.lblRouteFacility = new System.Windows.Forms.Label();
            this.cbRoutes = new System.Windows.Forms.ComboBox();
            this.rbLinearRef = new System.Windows.Forms.RadioButton();
            this.rbSectionRef = new System.Windows.Forms.RadioButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.dgvwPCI = new System.Windows.Forms.DataGridView();
            this.ID_ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ROUTES = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.BEGIN_STATION = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.END_STATION = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DIRECTION = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FACILITY = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.SECTION = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SAMPLE_ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DATE_ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.METHOD_ = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.SAMPLE_TYPE = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.AREA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PCI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvPCIDetail = new System.Windows.Forms.DataGridView();
            this.DETAIL_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DETAIL_PCI_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DISTRESS = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.SEVERITY = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.AMOUNT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EXTENT_ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DEDUCTVALUE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingNavigatorPCI = new System.Windows.Forms.BindingNavigator(this.components);
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
            this.toolStripButtonBulkLoad = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStripPCIDocument.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvwPCI)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPCIDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorPCI)).BeginInit();
            this.bindingNavigatorPCI.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkAllowEdit
            // 
            this.checkAllowEdit.AutoSize = true;
            this.checkAllowEdit.Checked = true;
            this.checkAllowEdit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAllowEdit.Location = new System.Drawing.Point(919, 39);
            this.checkAllowEdit.Margin = new System.Windows.Forms.Padding(2);
            this.checkAllowEdit.Name = "checkAllowEdit";
            this.checkAllowEdit.Size = new System.Drawing.Size(114, 17);
            this.checkAllowEdit.TabIndex = 31;
            this.checkAllowEdit.Text = "Allow Attribute Edit";
            this.checkAllowEdit.UseVisualStyleBackColor = true;
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            // 
            // labelServer
            // 
            this.labelServer.AutoSize = true;
            this.labelServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelServer.Location = new System.Drawing.Point(28, 26);
            this.labelServer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelServer.Name = "labelServer";
            this.labelServer.Size = new System.Drawing.Size(92, 13);
            this.labelServer.TabIndex = 29;
            this.labelServer.Text = "Server/Database:";
            // 
            // contextMenuStripPCIDocument
            // 
            this.contextMenuStripPCIDocument.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.contextMenuStripPCIDocument.Name = "contextMenuStripRawAttribute";
            this.contextMenuStripPCIDocument.Size = new System.Drawing.Size(103, 48);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            // 
            // labelPCI
            // 
            this.labelPCI.AutoSize = true;
            this.labelPCI.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPCI.Location = new System.Drawing.Point(26, 3);
            this.labelPCI.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPCI.Name = "labelPCI";
            this.labelPCI.Size = new System.Drawing.Size(49, 26);
            this.labelPCI.TabIndex = 27;
            this.labelPCI.Text = "PCI";
            // 
            // lblAdvancedSearch
            // 
            this.lblAdvancedSearch.AutoSize = true;
            this.lblAdvancedSearch.Location = new System.Drawing.Point(146, 60);
            this.lblAdvancedSearch.Name = "lblAdvancedSearch";
            this.lblAdvancedSearch.Size = new System.Drawing.Size(96, 13);
            this.lblAdvancedSearch.TabIndex = 26;
            this.lblAdvancedSearch.Text = "Advanced Search:";
            // 
            // tbSearch
            // 
            this.tbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearch.Enabled = false;
            this.tbSearch.Location = new System.Drawing.Point(244, 60);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Size = new System.Drawing.Size(753, 20);
            this.tbSearch.TabIndex = 25;
            // 
            // lblYears
            // 
            this.lblYears.AutoSize = true;
            this.lblYears.Location = new System.Drawing.Point(206, 39);
            this.lblYears.Name = "lblYears";
            this.lblYears.Size = new System.Drawing.Size(32, 13);
            this.lblYears.TabIndex = 23;
            this.lblYears.Text = "Year:";
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.Location = new System.Drawing.Point(1003, 61);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 24;
            this.btnSearch.Text = ". . .";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cbYear
            // 
            this.cbYear.FormattingEnabled = true;
            this.cbYear.Location = new System.Drawing.Point(244, 34);
            this.cbYear.Name = "cbYear";
            this.cbYear.Size = new System.Drawing.Size(121, 21);
            this.cbYear.TabIndex = 22;
            this.cbYear.SelectedIndexChanged += new System.EventHandler(this.cbYear_SelectedIndexChanged);
            // 
            // lblRouteFacility
            // 
            this.lblRouteFacility.AutoSize = true;
            this.lblRouteFacility.Location = new System.Drawing.Point(200, 13);
            this.lblRouteFacility.Name = "lblRouteFacility";
            this.lblRouteFacility.Size = new System.Drawing.Size(39, 13);
            this.lblRouteFacility.TabIndex = 21;
            this.lblRouteFacility.Text = "Route:";
            // 
            // cbRoutes
            // 
            this.cbRoutes.FormattingEnabled = true;
            this.cbRoutes.Location = new System.Drawing.Point(244, 8);
            this.cbRoutes.Name = "cbRoutes";
            this.cbRoutes.Size = new System.Drawing.Size(430, 21);
            this.cbRoutes.TabIndex = 20;
            this.cbRoutes.SelectedIndexChanged += new System.EventHandler(this.cbRoutes_SelectedIndexChanged);
            // 
            // rbLinearRef
            // 
            this.rbLinearRef.AutoSize = true;
            this.rbLinearRef.Location = new System.Drawing.Point(16, 41);
            this.rbLinearRef.Name = "rbLinearRef";
            this.rbLinearRef.Size = new System.Drawing.Size(107, 17);
            this.rbLinearRef.TabIndex = 19;
            this.rbLinearRef.TabStop = true;
            this.rbLinearRef.Text = "Linear Reference";
            this.rbLinearRef.UseVisualStyleBackColor = true;
            this.rbLinearRef.CheckedChanged += new System.EventHandler(this.rbLinearRef_CheckedChanged);
            // 
            // rbSectionRef
            // 
            this.rbSectionRef.AutoSize = true;
            this.rbSectionRef.Location = new System.Drawing.Point(16, 59);
            this.rbSectionRef.Name = "rbSectionRef";
            this.rbSectionRef.Size = new System.Drawing.Size(114, 17);
            this.rbSectionRef.TabIndex = 18;
            this.rbSectionRef.TabStop = true;
            this.rbSectionRef.Text = "Section Reference";
            this.rbSectionRef.UseVisualStyleBackColor = true;
            this.rbSectionRef.CheckedChanged += new System.EventHandler(this.rbSectionRef_CheckedChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(6, 3);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(22, 32);
            this.pictureBox1.TabIndex = 28;
            this.pictureBox1.TabStop = false;
            // 
            // dgvwPCI
            // 
            this.dgvwPCI.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvwPCI.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvwPCI.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID_,
            this.ROUTES,
            this.BEGIN_STATION,
            this.END_STATION,
            this.DIRECTION,
            this.FACILITY,
            this.SECTION,
            this.SAMPLE_,
            this.DATE_,
            this.METHOD_,
            this.SAMPLE_TYPE,
            this.AREA,
            this.PCI});
            this.dgvwPCI.Location = new System.Drawing.Point(2, 86);
            this.dgvwPCI.Name = "dgvwPCI";
            this.dgvwPCI.Size = new System.Drawing.Size(558, 633);
            this.dgvwPCI.TabIndex = 33;
            this.dgvwPCI.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvwPCI_CellBeginEdit);
            this.dgvwPCI.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvwPCI_CellEndEdit);
            this.dgvwPCI.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvwPCI_CellValidating);
            this.dgvwPCI.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvwPCI_DataError);
            this.dgvwPCI.SelectionChanged += new System.EventHandler(this.dgvwPCI_SelectionChanged);
            this.dgvwPCI.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgvwPCI_UserDeletingRow);
            // 
            // ID_
            // 
            this.ID_.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ID_.HeaderText = "ID";
            this.ID_.Name = "ID_";
            this.ID_.ReadOnly = true;
            this.ID_.Visible = false;
            // 
            // ROUTES
            // 
            this.ROUTES.HeaderText = "Routes";
            this.ROUTES.Name = "ROUTES";
            this.ROUTES.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ROUTES.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // BEGIN_STATION
            // 
            this.BEGIN_STATION.HeaderText = "Begin Station";
            this.BEGIN_STATION.Name = "BEGIN_STATION";
            this.BEGIN_STATION.Width = 50;
            // 
            // END_STATION
            // 
            this.END_STATION.HeaderText = "End Station";
            this.END_STATION.Name = "END_STATION";
            this.END_STATION.Width = 50;
            // 
            // DIRECTION
            // 
            this.DIRECTION.HeaderText = "Dir";
            this.DIRECTION.Name = "DIRECTION";
            this.DIRECTION.Width = 50;
            // 
            // FACILITY
            // 
            this.FACILITY.HeaderText = "Facility";
            this.FACILITY.Name = "FACILITY";
            this.FACILITY.Width = 150;
            // 
            // SECTION
            // 
            this.SECTION.HeaderText = "Section";
            this.SECTION.Name = "SECTION";
            this.SECTION.Width = 150;
            // 
            // SAMPLE_
            // 
            this.SAMPLE_.HeaderText = "Sample";
            this.SAMPLE_.Name = "SAMPLE_";
            this.SAMPLE_.Width = 150;
            // 
            // DATE_
            // 
            this.DATE_.HeaderText = "Date";
            this.DATE_.Name = "DATE_";
            this.DATE_.Width = 75;
            // 
            // METHOD_
            // 
            this.METHOD_.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.METHOD_.HeaderText = "Method";
            this.METHOD_.Name = "METHOD_";
            this.METHOD_.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.METHOD_.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // SAMPLE_TYPE
            // 
            this.SAMPLE_TYPE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SAMPLE_TYPE.HeaderText = "Type";
            this.SAMPLE_TYPE.Items.AddRange(new object[] {
            "Random",
            "Additional",
            "Ignore"});
            this.SAMPLE_TYPE.Name = "SAMPLE_TYPE";
            this.SAMPLE_TYPE.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SAMPLE_TYPE.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // AREA
            // 
            this.AREA.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.AREA.HeaderText = "Area";
            this.AREA.Name = "AREA";
            // 
            // PCI
            // 
            this.PCI.HeaderText = "PCI";
            this.PCI.Name = "PCI";
            this.PCI.ReadOnly = true;
            this.PCI.Width = 50;
            // 
            // dgvPCIDetail
            // 
            this.dgvPCIDetail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPCIDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPCIDetail.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DETAIL_ID,
            this.DETAIL_PCI_ID,
            this.DISTRESS,
            this.SEVERITY,
            this.AMOUNT,
            this.EXTENT_,
            this.DEDUCTVALUE});
            this.dgvPCIDetail.Location = new System.Drawing.Point(566, 86);
            this.dgvPCIDetail.Name = "dgvPCIDetail";
            this.dgvPCIDetail.Size = new System.Drawing.Size(467, 633);
            this.dgvPCIDetail.TabIndex = 36;
            this.dgvPCIDetail.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPCIDetail_CellEndEdit);
            this.dgvPCIDetail.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvPCIDetail_DataError);
            this.dgvPCIDetail.SelectionChanged += new System.EventHandler(this.dgvPCIDetail_SelectionChanged);
            this.dgvPCIDetail.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvPCIDetail_UserDeletedRow);
            this.dgvPCIDetail.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgvPCIDetail_UserDeletingRow);
            // 
            // DETAIL_ID
            // 
            this.DETAIL_ID.HeaderText = "ID";
            this.DETAIL_ID.Name = "DETAIL_ID";
            this.DETAIL_ID.Visible = false;
            // 
            // DETAIL_PCI_ID
            // 
            this.DETAIL_PCI_ID.HeaderText = "PCI_ID";
            this.DETAIL_PCI_ID.Name = "DETAIL_PCI_ID";
            this.DETAIL_PCI_ID.Visible = false;
            this.DETAIL_PCI_ID.Width = 120;
            // 
            // DISTRESS
            // 
            this.DISTRESS.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DISTRESS.HeaderText = "Distress";
            this.DISTRESS.Name = "DISTRESS";
            this.DISTRESS.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DISTRESS.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // SEVERITY
            // 
            this.SEVERITY.HeaderText = "Severity";
            this.SEVERITY.Items.AddRange(new object[] {
            "H",
            "M",
            "L"});
            this.SEVERITY.Name = "SEVERITY";
            this.SEVERITY.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SEVERITY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.SEVERITY.Width = 50;
            // 
            // AMOUNT
            // 
            this.AMOUNT.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.AMOUNT.HeaderText = "Amount";
            this.AMOUNT.Name = "AMOUNT";
            this.AMOUNT.Width = 50;
            // 
            // EXTENT_
            // 
            this.EXTENT_.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.EXTENT_.HeaderText = "Extent";
            this.EXTENT_.Name = "EXTENT_";
            this.EXTENT_.ReadOnly = true;
            this.EXTENT_.Width = 50;
            // 
            // DEDUCTVALUE
            // 
            this.DEDUCTVALUE.HeaderText = "Deduct";
            this.DEDUCTVALUE.Name = "DEDUCTVALUE";
            this.DEDUCTVALUE.ReadOnly = true;
            this.DEDUCTVALUE.Width = 50;
            // 
            // bindingNavigatorPCI
            // 
            this.bindingNavigatorPCI.AddNewItem = this.bindingNavigatorAddNewItem;
            this.bindingNavigatorPCI.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigatorPCI.DeleteItem = this.bindingNavigatorDeleteItem;
            this.bindingNavigatorPCI.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bindingNavigatorPCI.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.toolStripButtonBulkLoad});
            this.bindingNavigatorPCI.Location = new System.Drawing.Point(0, 722);
            this.bindingNavigatorPCI.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigatorPCI.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigatorPCI.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigatorPCI.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigatorPCI.Name = "bindingNavigatorPCI";
            this.bindingNavigatorPCI.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigatorPCI.Size = new System.Drawing.Size(1036, 25);
            this.bindingNavigatorPCI.TabIndex = 37;
            this.bindingNavigatorPCI.Text = "bindingNavigator1";
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorAddNewItem.Text = "Add new";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(35, 22);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorDeleteItem.Text = "Delete";
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
            // toolStripButtonBulkLoad
            // 
            this.toolStripButtonBulkLoad.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBulkLoad.Image")));
            this.toolStripButtonBulkLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBulkLoad.Name = "toolStripButtonBulkLoad";
            this.toolStripButtonBulkLoad.Size = new System.Drawing.Size(100, 22);
            this.toolStripButtonBulkLoad.Text = "PCI Bulk Load";
            this.toolStripButtonBulkLoad.Click += new System.EventHandler(this.toolStripButtonBulkLoad_Click);
            // 
            // FormPCIDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1036, 747);
            this.Controls.Add(this.bindingNavigatorPCI);
            this.Controls.Add(this.labelServer);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.dgvwPCI);
            this.Controls.Add(this.checkAllowEdit);
            this.Controls.Add(this.labelPCI);
            this.Controls.Add(this.lblAdvancedSearch);
            this.Controls.Add(this.dgvPCIDetail);
            this.Controls.Add(this.tbSearch);
            this.Controls.Add(this.lblYears);
            this.Controls.Add(this.cbYear);
            this.Controls.Add(this.lblRouteFacility);
            this.Controls.Add(this.cbRoutes);
            this.Controls.Add(this.rbLinearRef);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.rbSectionRef);
            this.Name = "FormPCIDocument";
            this.TabText = "FormPCIDocument";
            this.Text = "FormPCIDocument";
            this.Load += new System.EventHandler(this.FormPCIDocument_Load);
            this.contextMenuStripPCIDocument.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvwPCI)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPCIDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorPCI)).EndInit();
            this.bindingNavigatorPCI.ResumeLayout(false);
            this.bindingNavigatorPCI.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.CheckBox checkAllowEdit;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.Label labelServer;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripPCIDocument;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label labelPCI;
		private System.Windows.Forms.Label lblAdvancedSearch;
		private System.Windows.Forms.TextBox tbSearch;
		private System.Windows.Forms.Label lblYears;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.ComboBox cbYear;
		private System.Windows.Forms.Label lblRouteFacility;
		private System.Windows.Forms.ComboBox cbRoutes;
		private System.Windows.Forms.RadioButton rbLinearRef;
		private System.Windows.Forms.RadioButton rbSectionRef;
		private System.Windows.Forms.DataGridView dgvwPCI;
        private System.Windows.Forms.DataGridView dgvPCIDetail;
        private System.Windows.Forms.BindingNavigator bindingNavigatorPCI;
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
        private System.Windows.Forms.ToolStripButton toolStripButtonBulkLoad;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID_;
        private System.Windows.Forms.DataGridViewComboBoxColumn ROUTES;
        private System.Windows.Forms.DataGridViewTextBoxColumn BEGIN_STATION;
        private System.Windows.Forms.DataGridViewTextBoxColumn END_STATION;
        private System.Windows.Forms.DataGridViewTextBoxColumn DIRECTION;
        private System.Windows.Forms.DataGridViewComboBoxColumn FACILITY;
        private System.Windows.Forms.DataGridViewTextBoxColumn SECTION;
        private System.Windows.Forms.DataGridViewTextBoxColumn SAMPLE_;
        private System.Windows.Forms.DataGridViewTextBoxColumn DATE_;
        private System.Windows.Forms.DataGridViewComboBoxColumn METHOD_;
        private System.Windows.Forms.DataGridViewComboBoxColumn SAMPLE_TYPE;
        private System.Windows.Forms.DataGridViewTextBoxColumn AREA;
        private System.Windows.Forms.DataGridViewTextBoxColumn PCI;
        private System.Windows.Forms.DataGridViewTextBoxColumn DETAIL_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn DETAIL_PCI_ID;
        private System.Windows.Forms.DataGridViewComboBoxColumn DISTRESS;
        private System.Windows.Forms.DataGridViewComboBoxColumn SEVERITY;
        private System.Windows.Forms.DataGridViewTextBoxColumn AMOUNT;
        private System.Windows.Forms.DataGridViewTextBoxColumn EXTENT_;
        private System.Windows.Forms.DataGridViewTextBoxColumn DEDUCTVALUE;
		






	}
}
