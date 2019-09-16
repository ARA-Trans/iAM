namespace RoadCare3
{
	partial class FormAssetView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAssetView));
			this.bindingNavigatorSectionView = new System.Windows.Forms.BindingNavigator(this.components);
			this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
			this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
			this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.dgvSection = new System.Windows.Forms.DataGridView();
			this.FACILITY = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SECTION = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SECTIONID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.labelAttribute = new System.Windows.Forms.Label();
			this.txtAssetFilter = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonAssetFilter = new System.Windows.Forms.Button();
			this.dpAssetDisplayContainer = new WeifenLuo.WinFormsUI.Docking.DockPanel();
			this.labelRouteFacility = new System.Windows.Forms.Label();
			this.comboBoxRouteFacilty = new System.Windows.Forms.ComboBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorSectionView)).BeginInit();
			this.bindingNavigatorSectionView.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvSection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// bindingNavigatorSectionView
			// 
			this.bindingNavigatorSectionView.AddNewItem = null;
			this.bindingNavigatorSectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.bindingNavigatorSectionView.BackColor = System.Drawing.SystemColors.Control;
			this.bindingNavigatorSectionView.CountItem = this.bindingNavigatorCountItem;
			this.bindingNavigatorSectionView.DeleteItem = null;
			this.bindingNavigatorSectionView.Dock = System.Windows.Forms.DockStyle.None;
			this.bindingNavigatorSectionView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2});
			this.bindingNavigatorSectionView.Location = new System.Drawing.Point(12, 612);
			this.bindingNavigatorSectionView.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bindingNavigatorSectionView.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bindingNavigatorSectionView.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bindingNavigatorSectionView.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bindingNavigatorSectionView.Name = "bindingNavigatorSectionView";
			this.bindingNavigatorSectionView.PositionItem = this.bindingNavigatorPositionItem;
			this.bindingNavigatorSectionView.Size = new System.Drawing.Size(209, 25);
			this.bindingNavigatorSectionView.Stretch = true;
			this.bindingNavigatorSectionView.TabIndex = 43;
			this.bindingNavigatorSectionView.Text = "bindingNavigator1";
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
			// dgvSection
			// 
			this.dgvSection.AllowUserToAddRows = false;
			this.dgvSection.AllowUserToDeleteRows = false;
			this.dgvSection.AllowUserToResizeRows = false;
			this.dgvSection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.dgvSection.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSection.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FACILITY,
            this.SECTION,
            this.SECTIONID});
			this.dgvSection.Location = new System.Drawing.Point(12, 82);
			this.dgvSection.Name = "dgvSection";
			this.dgvSection.ReadOnly = true;
			this.dgvSection.RowHeadersVisible = false;
			this.dgvSection.RowHeadersWidth = 25;
			this.dgvSection.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvSection.Size = new System.Drawing.Size(331, 527);
			this.dgvSection.TabIndex = 35;
			this.dgvSection.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSection_RowEnter);
			this.dgvSection.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvSection_MouseUp);
			this.dgvSection.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgvSection_KeyUp);
			// 
			// FACILITY
			// 
			this.FACILITY.HeaderText = "Facility";
			this.FACILITY.Name = "FACILITY";
			this.FACILITY.ReadOnly = true;
			// 
			// SECTION
			// 
			this.SECTION.HeaderText = "Section";
			this.SECTION.Name = "SECTION";
			this.SECTION.ReadOnly = true;
			// 
			// SECTIONID
			// 
			this.SECTIONID.HeaderText = "SectionID";
			this.SECTIONID.Name = "SECTIONID";
			this.SECTIONID.ReadOnly = true;
			this.SECTIONID.Visible = false;
			// 
			// labelAttribute
			// 
			this.labelAttribute.AutoSize = true;
			this.labelAttribute.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAttribute.Location = new System.Drawing.Point(31, 9);
			this.labelAttribute.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelAttribute.Name = "labelAttribute";
			this.labelAttribute.Size = new System.Drawing.Size(140, 26);
			this.labelAttribute.TabIndex = 36;
			this.labelAttribute.Text = "Asset View - ";
			// 
			// txtAssetFilter
			// 
			this.txtAssetFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtAssetFilter.Location = new System.Drawing.Point(280, 56);
			this.txtAssetFilter.Margin = new System.Windows.Forms.Padding(2);
			this.txtAssetFilter.Name = "txtAssetFilter";
			this.txtAssetFilter.Size = new System.Drawing.Size(651, 20);
			this.txtAssetFilter.TabIndex = 46;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(218, 59);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(58, 13);
			this.label1.TabIndex = 45;
			this.label1.Text = "Asset Filter";
			// 
			// buttonAssetFilter
			// 
			this.buttonAssetFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAssetFilter.Location = new System.Drawing.Point(938, 57);
			this.buttonAssetFilter.Margin = new System.Windows.Forms.Padding(2);
			this.buttonAssetFilter.Name = "buttonAssetFilter";
			this.buttonAssetFilter.Size = new System.Drawing.Size(28, 19);
			this.buttonAssetFilter.TabIndex = 47;
			this.buttonAssetFilter.Text = "...";
			this.buttonAssetFilter.UseVisualStyleBackColor = true;
			this.buttonAssetFilter.Click += new System.EventHandler(this.buttonAssetFilter_Click);
			// 
			// dpAssetDisplayContainer
			// 
			this.dpAssetDisplayContainer.ActiveAutoHideContent = null;
			this.dpAssetDisplayContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dpAssetDisplayContainer.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
			this.dpAssetDisplayContainer.Location = new System.Drawing.Point(349, 82);
			this.dpAssetDisplayContainer.Name = "dpAssetDisplayContainer";
			this.dpAssetDisplayContainer.Size = new System.Drawing.Size(617, 555);
			this.dpAssetDisplayContainer.TabIndex = 48;
			// 
			// labelRouteFacility
			// 
			this.labelRouteFacility.AutoSize = true;
			this.labelRouteFacility.Location = new System.Drawing.Point(11, 59);
			this.labelRouteFacility.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelRouteFacility.Name = "labelRouteFacility";
			this.labelRouteFacility.Size = new System.Drawing.Size(42, 13);
			this.labelRouteFacility.TabIndex = 51;
			this.labelRouteFacility.Text = "Facility:";
			// 
			// comboBoxRouteFacilty
			// 
			this.comboBoxRouteFacilty.FormattingEnabled = true;
			this.comboBoxRouteFacilty.Location = new System.Drawing.Point(57, 56);
			this.comboBoxRouteFacilty.Margin = new System.Windows.Forms.Padding(2);
			this.comboBoxRouteFacilty.Name = "comboBoxRouteFacilty";
			this.comboBoxRouteFacilty.Size = new System.Drawing.Size(157, 21);
			this.comboBoxRouteFacilty.TabIndex = 50;
			this.comboBoxRouteFacilty.SelectedIndexChanged += new System.EventHandler(this.comboBoxRouteFacilty_SelectedIndexChanged);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::RoadCare3.Properties.Resources.bigblue;
			this.pictureBox1.Location = new System.Drawing.Point(1, 4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(34, 36);
			this.pictureBox1.TabIndex = 44;
			this.pictureBox1.TabStop = false;
			// 
			// FormAssetView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(975, 646);
			this.Controls.Add(this.labelRouteFacility);
			this.Controls.Add(this.dpAssetDisplayContainer);
			this.Controls.Add(this.buttonAssetFilter);
			this.Controls.Add(this.comboBoxRouteFacilty);
			this.Controls.Add(this.txtAssetFilter);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.bindingNavigatorSectionView);
			this.Controls.Add(this.dgvSection);
			this.Controls.Add(this.labelAttribute);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAssetView";
			this.TabText = "FormAssetView";
			this.Text = "FormAssetView";
			this.Load += new System.EventHandler(this.FormAssetView_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAssetView_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorSectionView)).EndInit();
			this.bindingNavigatorSectionView.ResumeLayout(false);
			this.bindingNavigatorSectionView.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvSection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.BindingNavigator bindingNavigatorSectionView;
		private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
		private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
		private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
		private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
		private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
		private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
		private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
		private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
		private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
		private System.Windows.Forms.DataGridView dgvSection;
		private System.Windows.Forms.Label labelAttribute;
		private System.Windows.Forms.TextBox txtAssetFilter;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonAssetFilter;
		private WeifenLuo.WinFormsUI.Docking.DockPanel dpAssetDisplayContainer;
		private System.Windows.Forms.Label labelRouteFacility;
		private System.Windows.Forms.ComboBox comboBoxRouteFacilty;
		private System.Windows.Forms.DataGridViewTextBoxColumn FACILITY;
		private System.Windows.Forms.DataGridViewTextBoxColumn SECTION;
		private System.Windows.Forms.DataGridViewTextBoxColumn SECTIONID;
	}
}