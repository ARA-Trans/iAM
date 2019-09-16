namespace RoadCare3
{
    partial class FormSectionView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSectionView));
			this.textBoxAdvanceSearch = new System.Windows.Forms.TextBox();
			this.buttonAdvancedSearch = new System.Windows.Forms.Button();
			this.labelRouteFacility = new System.Windows.Forms.Label();
			this.labelAdvancedSearch = new System.Windows.Forms.Label();
			this.comboBoxAttributeValue = new System.Windows.Forms.ComboBox();
			this.comboBoxRouteFacilty = new System.Windows.Forms.ComboBox();
			this.comboBoxFilterAttribute = new System.Windows.Forms.ComboBox();
			this.checkBoxCustomFilter = new System.Windows.Forms.CheckBox();
			this.labelAttribute = new System.Windows.Forms.Label();
			this.dgvSection = new System.Windows.Forms.DataGridView();
			this.tabControlSection = new System.Windows.Forms.TabControl();
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
			this.toolStripButtonARAN = new System.Windows.Forms.ToolStripButton();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.buttonUpdate = new System.Windows.Forms.Button();
			this.checkMilepost = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.dgvSection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorSectionView)).BeginInit();
			this.bindingNavigatorSectionView.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// textBoxAdvanceSearch
			// 
			this.textBoxAdvanceSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxAdvanceSearch.Location = new System.Drawing.Point(104, 70);
			this.textBoxAdvanceSearch.Margin = new System.Windows.Forms.Padding(2);
			this.textBoxAdvanceSearch.Name = "textBoxAdvanceSearch";
			this.textBoxAdvanceSearch.Size = new System.Drawing.Size(825, 20);
			this.textBoxAdvanceSearch.TabIndex = 23;
			// 
			// buttonAdvancedSearch
			// 
			this.buttonAdvancedSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAdvancedSearch.Location = new System.Drawing.Point(933, 70);
			this.buttonAdvancedSearch.Margin = new System.Windows.Forms.Padding(2);
			this.buttonAdvancedSearch.Name = "buttonAdvancedSearch";
			this.buttonAdvancedSearch.Size = new System.Drawing.Size(28, 19);
			this.buttonAdvancedSearch.TabIndex = 24;
			this.buttonAdvancedSearch.Text = "...";
			this.buttonAdvancedSearch.UseVisualStyleBackColor = true;
			this.buttonAdvancedSearch.Click += new System.EventHandler(this.buttonAdvancedSearch_Click);
			// 
			// labelRouteFacility
			// 
			this.labelRouteFacility.AutoSize = true;
			this.labelRouteFacility.Location = new System.Drawing.Point(334, 49);
			this.labelRouteFacility.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelRouteFacility.Name = "labelRouteFacility";
			this.labelRouteFacility.Size = new System.Drawing.Size(42, 13);
			this.labelRouteFacility.TabIndex = 21;
			this.labelRouteFacility.Text = "Facility:";
			// 
			// labelAdvancedSearch
			// 
			this.labelAdvancedSearch.AutoSize = true;
			this.labelAdvancedSearch.Location = new System.Drawing.Point(5, 73);
			this.labelAdvancedSearch.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelAdvancedSearch.Name = "labelAdvancedSearch";
			this.labelAdvancedSearch.Size = new System.Drawing.Size(96, 13);
			this.labelAdvancedSearch.TabIndex = 22;
			this.labelAdvancedSearch.Text = "Advanced Search:";
			// 
			// comboBoxAttributeValue
			// 
			this.comboBoxAttributeValue.FormattingEnabled = true;
			this.comboBoxAttributeValue.Location = new System.Drawing.Point(178, 47);
			this.comboBoxAttributeValue.Margin = new System.Windows.Forms.Padding(2);
			this.comboBoxAttributeValue.Name = "comboBoxAttributeValue";
			this.comboBoxAttributeValue.Size = new System.Drawing.Size(152, 21);
			this.comboBoxAttributeValue.TabIndex = 20;
			this.comboBoxAttributeValue.Visible = false;
			this.comboBoxAttributeValue.SelectedIndexChanged += new System.EventHandler(this.comboBoxAttributeValue_SelectedIndexChanged);
			// 
			// comboBoxRouteFacilty
			// 
			this.comboBoxRouteFacilty.FormattingEnabled = true;
			this.comboBoxRouteFacilty.Location = new System.Drawing.Point(379, 46);
			this.comboBoxRouteFacilty.Margin = new System.Windows.Forms.Padding(2);
			this.comboBoxRouteFacilty.Name = "comboBoxRouteFacilty";
			this.comboBoxRouteFacilty.Size = new System.Drawing.Size(157, 21);
			this.comboBoxRouteFacilty.TabIndex = 19;
			// 
			// comboBoxFilterAttribute
			// 
			this.comboBoxFilterAttribute.FormattingEnabled = true;
			this.comboBoxFilterAttribute.Location = new System.Drawing.Point(37, 48);
			this.comboBoxFilterAttribute.Margin = new System.Windows.Forms.Padding(2);
			this.comboBoxFilterAttribute.Name = "comboBoxFilterAttribute";
			this.comboBoxFilterAttribute.Size = new System.Drawing.Size(138, 21);
			this.comboBoxFilterAttribute.TabIndex = 18;
			this.comboBoxFilterAttribute.Visible = false;
			this.comboBoxFilterAttribute.SelectedIndexChanged += new System.EventHandler(this.comboBoxFilterAttribute_SelectedIndexChanged);
			// 
			// checkBoxCustomFilter
			// 
			this.checkBoxCustomFilter.AutoSize = true;
			this.checkBoxCustomFilter.Location = new System.Drawing.Point(22, 50);
			this.checkBoxCustomFilter.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxCustomFilter.Name = "checkBoxCustomFilter";
			this.checkBoxCustomFilter.Size = new System.Drawing.Size(118, 17);
			this.checkBoxCustomFilter.TabIndex = 17;
			this.checkBoxCustomFilter.Text = "Enable custom filter";
			this.checkBoxCustomFilter.UseVisualStyleBackColor = true;
			this.checkBoxCustomFilter.CheckedChanged += new System.EventHandler(this.checkBoxCustomFilter_CheckedChanged);
			// 
			// labelAttribute
			// 
			this.labelAttribute.AutoSize = true;
			this.labelAttribute.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAttribute.Location = new System.Drawing.Point(45, 9);
			this.labelAttribute.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelAttribute.Name = "labelAttribute";
			this.labelAttribute.Size = new System.Drawing.Size(238, 26);
			this.labelAttribute.TabIndex = 16;
			this.labelAttribute.Text = "Section View - Network";
			// 
			// dgvSection
			// 
			this.dgvSection.AllowUserToAddRows = false;
			this.dgvSection.AllowUserToDeleteRows = false;
			this.dgvSection.AllowUserToResizeRows = false;
			this.dgvSection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.dgvSection.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSection.Location = new System.Drawing.Point(-1, 96);
			this.dgvSection.Name = "dgvSection";
			this.dgvSection.ReadOnly = true;
			this.dgvSection.RowHeadersWidth = 25;
			this.dgvSection.Size = new System.Drawing.Size(331, 477);
			this.dgvSection.TabIndex = 1;
			this.dgvSection.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvSection_RowHeaderMouseDoubleClick);
			// 
			// tabControlSection
			// 
			this.tabControlSection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlSection.Location = new System.Drawing.Point(337, 96);
			this.tabControlSection.Name = "tabControlSection";
			this.tabControlSection.SelectedIndex = 0;
			this.tabControlSection.Size = new System.Drawing.Size(635, 503);
			this.tabControlSection.TabIndex = 32;
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
            this.bindingNavigatorSeparator2,
            this.toolStripButtonARAN});
			this.bindingNavigatorSectionView.Location = new System.Drawing.Point(-1, 575);
			this.bindingNavigatorSectionView.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bindingNavigatorSectionView.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bindingNavigatorSectionView.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bindingNavigatorSectionView.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bindingNavigatorSectionView.Name = "bindingNavigatorSectionView";
			this.bindingNavigatorSectionView.PositionItem = this.bindingNavigatorPositionItem;
			this.bindingNavigatorSectionView.Size = new System.Drawing.Size(327, 25);
			this.bindingNavigatorSectionView.Stretch = true;
			this.bindingNavigatorSectionView.TabIndex = 33;
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
			// toolStripButtonARAN
			// 
			this.toolStripButtonARAN.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonARAN.Image")));
			this.toolStripButtonARAN.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonARAN.Name = "toolStripButtonARAN";
			this.toolStripButtonARAN.Size = new System.Drawing.Size(87, 22);
			this.toolStripButtonARAN.Text = "ARAN View";
			this.toolStripButtonARAN.Visible = false;
			this.toolStripButtonARAN.Click += new System.EventHandler(this.toolStripButtonARAN_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::RoadCare3.Properties.Resources.bigorange1;
			this.pictureBox1.Location = new System.Drawing.Point(6, 7);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(34, 36);
			this.pictureBox1.TabIndex = 34;
			this.pictureBox1.TabStop = false;
			// 
			// buttonUpdate
			// 
			this.buttonUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonUpdate.Image = global::RoadCare3.Properties.Resources.update;
			this.buttonUpdate.Location = new System.Drawing.Point(922, 7);
			this.buttonUpdate.Margin = new System.Windows.Forms.Padding(2);
			this.buttonUpdate.Name = "buttonUpdate";
			this.buttonUpdate.Size = new System.Drawing.Size(39, 37);
			this.buttonUpdate.TabIndex = 29;
			this.buttonUpdate.UseVisualStyleBackColor = true;
			this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
			// 
			// checkMilepost
			// 
			this.checkMilepost.AutoSize = true;
			this.checkMilepost.Enabled = false;
			this.checkMilepost.Location = new System.Drawing.Point(568, 49);
			this.checkMilepost.Margin = new System.Windows.Forms.Padding(2);
			this.checkMilepost.Name = "checkMilepost";
			this.checkMilepost.Size = new System.Drawing.Size(154, 17);
			this.checkMilepost.TabIndex = 30;
			this.checkMilepost.Text = "Show Begin/End/Direction";
			this.checkMilepost.UseVisualStyleBackColor = true;
			this.checkMilepost.Visible = false;
			// 
			// FormSectionView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(969, 602);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.bindingNavigatorSectionView);
			this.Controls.Add(this.tabControlSection);
			this.Controls.Add(this.dgvSection);
			this.Controls.Add(this.checkMilepost);
			this.Controls.Add(this.buttonUpdate);
			this.Controls.Add(this.textBoxAdvanceSearch);
			this.Controls.Add(this.buttonAdvancedSearch);
			this.Controls.Add(this.labelRouteFacility);
			this.Controls.Add(this.labelAdvancedSearch);
			this.Controls.Add(this.comboBoxAttributeValue);
			this.Controls.Add(this.comboBoxRouteFacilty);
			this.Controls.Add(this.comboBoxFilterAttribute);
			this.Controls.Add(this.checkBoxCustomFilter);
			this.Controls.Add(this.labelAttribute);
			this.Name = "FormSectionView";
			this.TabText = "Section View";
			this.Text = "Section View";
			this.Load += new System.EventHandler(this.FormSectionView_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSectionView_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.dgvSection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorSectionView)).EndInit();
			this.bindingNavigatorSectionView.ResumeLayout(false);
			this.bindingNavigatorSectionView.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.TextBox textBoxAdvanceSearch;
        private System.Windows.Forms.Button buttonAdvancedSearch;
        private System.Windows.Forms.Label labelRouteFacility;
        private System.Windows.Forms.Label labelAdvancedSearch;
        private System.Windows.Forms.ComboBox comboBoxAttributeValue;
        private System.Windows.Forms.ComboBox comboBoxRouteFacilty;
        private System.Windows.Forms.ComboBox comboBoxFilterAttribute;
        private System.Windows.Forms.CheckBox checkBoxCustomFilter;
        private System.Windows.Forms.Label labelAttribute;
        private System.Windows.Forms.DataGridView dgvSection;
        private System.Windows.Forms.TabControl tabControlSection;
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
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.CheckBox checkMilepost;
		private System.Windows.Forms.ToolStripButton toolStripButtonARAN;
    }
}