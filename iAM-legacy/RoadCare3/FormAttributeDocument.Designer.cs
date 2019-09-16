namespace RoadCare3
{
    partial class FormAttributeDocument
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAttributeDocument));
            this.dgvAttribute = new System.Windows.Forms.DataGridView();
            this.contextMenuStripRawAttribute = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rbSectionRef = new System.Windows.Forms.RadioButton();
            this.rbLinearRef = new System.Windows.Forms.RadioButton();
            this.cbRoutes = new System.Windows.Forms.ComboBox();
            this.lblRouteFacility = new System.Windows.Forms.Label();
            this.cbYear = new System.Windows.Forms.ComboBox();
            this.lblYears = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.tbSearch = new System.Windows.Forms.TextBox();
            this.lblAdvancedSearch = new System.Windows.Forms.Label();
            this.labelAttribute = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelServer = new System.Windows.Forms.Label();
            this.checkAllowEdit = new System.Windows.Forms.CheckBox();
            this.bindingNavigatorAttributeRaw = new System.Windows.Forms.BindingNavigator(this.components);
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
            this.tsbImportFromDataSource = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbDeleteAll = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttribute)).BeginInit();
            this.contextMenuStripRawAttribute.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorAttributeRaw)).BeginInit();
            this.bindingNavigatorAttributeRaw.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvAttribute
            // 
            this.dgvAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAttribute.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAttribute.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAttribute.ContextMenuStrip = this.contextMenuStripRawAttribute;
            this.dgvAttribute.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dgvAttribute.Location = new System.Drawing.Point(1, 85);
            this.dgvAttribute.Name = "dgvAttribute";
            this.dgvAttribute.RowTemplate.Height = 24;
            this.dgvAttribute.Size = new System.Drawing.Size(789, 353);
            this.dgvAttribute.TabIndex = 0;
            this.dgvAttribute.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvAttribute_DataError);
            this.dgvAttribute.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvAttribute_RowHeaderMouseDoubleClick);
            this.dgvAttribute.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvAttribute_KeyDown);
            // 
            // contextMenuStripRawAttribute
            // 
            this.contextMenuStripRawAttribute.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.contextMenuStripRawAttribute.Name = "contextMenuStripRawAttribute";
            this.contextMenuStripRawAttribute.Size = new System.Drawing.Size(103, 48);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // rbSectionRef
            // 
            this.rbSectionRef.AutoSize = true;
            this.rbSectionRef.Checked = true;
            this.rbSectionRef.Location = new System.Drawing.Point(16, 62);
            this.rbSectionRef.Name = "rbSectionRef";
            this.rbSectionRef.Size = new System.Drawing.Size(114, 17);
            this.rbSectionRef.TabIndex = 1;
            this.rbSectionRef.TabStop = true;
            this.rbSectionRef.Text = "Section Reference";
            this.rbSectionRef.UseVisualStyleBackColor = true;
            this.rbSectionRef.CheckedChanged += new System.EventHandler(this.rbSectionRef_CheckedChanged);
            // 
            // rbLinearRef
            // 
            this.rbLinearRef.AutoSize = true;
            this.rbLinearRef.Location = new System.Drawing.Point(16, 44);
            this.rbLinearRef.Name = "rbLinearRef";
            this.rbLinearRef.Size = new System.Drawing.Size(107, 17);
            this.rbLinearRef.TabIndex = 2;
            this.rbLinearRef.Text = "Linear Reference";
            this.rbLinearRef.UseVisualStyleBackColor = true;
            // 
            // cbRoutes
            // 
            this.cbRoutes.FormattingEnabled = true;
            this.cbRoutes.Location = new System.Drawing.Point(244, 11);
            this.cbRoutes.Name = "cbRoutes";
            this.cbRoutes.Size = new System.Drawing.Size(313, 21);
            this.cbRoutes.TabIndex = 3;
            this.cbRoutes.SelectedIndexChanged += new System.EventHandler(this.cbRoutes_SelectedIndexChanged);
            // 
            // lblRouteFacility
            // 
            this.lblRouteFacility.AutoSize = true;
            this.lblRouteFacility.Location = new System.Drawing.Point(200, 16);
            this.lblRouteFacility.Name = "lblRouteFacility";
            this.lblRouteFacility.Size = new System.Drawing.Size(39, 13);
            this.lblRouteFacility.TabIndex = 4;
            this.lblRouteFacility.Text = "Route:";
            // 
            // cbYear
            // 
            this.cbYear.FormattingEnabled = true;
            this.cbYear.Location = new System.Drawing.Point(244, 37);
            this.cbYear.Name = "cbYear";
            this.cbYear.Size = new System.Drawing.Size(96, 21);
            this.cbYear.TabIndex = 5;
            this.cbYear.SelectedIndexChanged += new System.EventHandler(this.cbYear_SelectedIndexChanged);
            // 
            // lblYears
            // 
            this.lblYears.AutoSize = true;
            this.lblYears.Location = new System.Drawing.Point(206, 42);
            this.lblYears.Name = "lblYears";
            this.lblYears.Size = new System.Drawing.Size(32, 13);
            this.lblYears.TabIndex = 6;
            this.lblYears.Text = "Year:";
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.Location = new System.Drawing.Point(760, 63);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(30, 20);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = ". . .";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // tbSearch
            // 
            this.tbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearch.Location = new System.Drawing.Point(244, 63);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Size = new System.Drawing.Size(510, 20);
            this.tbSearch.TabIndex = 8;
            // 
            // lblAdvancedSearch
            // 
            this.lblAdvancedSearch.AutoSize = true;
            this.lblAdvancedSearch.Location = new System.Drawing.Point(146, 63);
            this.lblAdvancedSearch.Name = "lblAdvancedSearch";
            this.lblAdvancedSearch.Size = new System.Drawing.Size(96, 13);
            this.lblAdvancedSearch.TabIndex = 9;
            this.lblAdvancedSearch.Text = "Advanced Search:";
            // 
            // labelAttribute
            // 
            this.labelAttribute.AutoSize = true;
            this.labelAttribute.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAttribute.Location = new System.Drawing.Point(26, 6);
            this.labelAttribute.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAttribute.Name = "labelAttribute";
            this.labelAttribute.Size = new System.Drawing.Size(93, 26);
            this.labelAttribute.TabIndex = 11;
            this.labelAttribute.Text = "Attribute";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RoadCare3.Properties.Resources.database;
            this.pictureBox1.Location = new System.Drawing.Point(6, 6);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(22, 32);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // labelServer
            // 
            this.labelServer.AutoSize = true;
            this.labelServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelServer.Location = new System.Drawing.Point(28, 29);
            this.labelServer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelServer.Name = "labelServer";
            this.labelServer.Size = new System.Drawing.Size(92, 13);
            this.labelServer.TabIndex = 13;
            this.labelServer.Text = "Server/Database:";
            // 
            // checkAllowEdit
            // 
            this.checkAllowEdit.AutoSize = true;
            this.checkAllowEdit.Checked = true;
            this.checkAllowEdit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAllowEdit.Location = new System.Drawing.Point(443, 41);
            this.checkAllowEdit.Margin = new System.Windows.Forms.Padding(2);
            this.checkAllowEdit.Name = "checkAllowEdit";
            this.checkAllowEdit.Size = new System.Drawing.Size(114, 17);
            this.checkAllowEdit.TabIndex = 15;
            this.checkAllowEdit.Text = "Allow Attribute Edit";
            this.checkAllowEdit.UseVisualStyleBackColor = true;
            this.checkAllowEdit.CheckedChanged += new System.EventHandler(this.checkAllowEdit_CheckedChanged);
            // 
            // bindingNavigatorAttributeRaw
            // 
            this.bindingNavigatorAttributeRaw.AddNewItem = this.bindingNavigatorAddNewItem;
            this.bindingNavigatorAttributeRaw.BackColor = System.Drawing.SystemColors.Control;
            this.bindingNavigatorAttributeRaw.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigatorAttributeRaw.DeleteItem = this.bindingNavigatorDeleteItem;
            this.bindingNavigatorAttributeRaw.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bindingNavigatorAttributeRaw.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.tsbImportFromDataSource,
            this.toolStripSeparator1,
            this.tsbDeleteAll});
            this.bindingNavigatorAttributeRaw.Location = new System.Drawing.Point(0, 441);
            this.bindingNavigatorAttributeRaw.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigatorAttributeRaw.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigatorAttributeRaw.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigatorAttributeRaw.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigatorAttributeRaw.Name = "bindingNavigatorAttributeRaw";
            this.bindingNavigatorAttributeRaw.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigatorAttributeRaw.Size = new System.Drawing.Size(790, 25);
            this.bindingNavigatorAttributeRaw.TabIndex = 16;
            this.bindingNavigatorAttributeRaw.Text = "bindingNavigator1";
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
            // tsbImportFromDataSource
            // 
            this.tsbImportFromDataSource.Image = ((System.Drawing.Image)(resources.GetObject("tsbImportFromDataSource.Image")));
            this.tsbImportFromDataSource.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbImportFromDataSource.Name = "tsbImportFromDataSource";
            this.tsbImportFromDataSource.Size = new System.Drawing.Size(165, 22);
            this.tsbImportFromDataSource.Text = "Import from data source...";
            this.tsbImportFromDataSource.Click += new System.EventHandler(this.tsbImportFromDataSource_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbDeleteAll
            // 
            this.tsbDeleteAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteAll.Image = global::RoadCare3.Properties.Resources.PlusMinus;
            this.tsbDeleteAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeleteAll.Name = "tsbDeleteAll";
            this.tsbDeleteAll.Size = new System.Drawing.Size(23, 22);
            this.tsbDeleteAll.Click += new System.EventHandler(this.tsbDeleteAll_Click);
            // 
            // FormAttributeDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(790, 466);
            this.Controls.Add(this.bindingNavigatorAttributeRaw);
            this.Controls.Add(this.labelServer);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelAttribute);
            this.Controls.Add(this.checkAllowEdit);
            this.Controls.Add(this.lblAdvancedSearch);
            this.Controls.Add(this.tbSearch);
            this.Controls.Add(this.lblYears);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.cbYear);
            this.Controls.Add(this.lblRouteFacility);
            this.Controls.Add(this.cbRoutes);
            this.Controls.Add(this.rbLinearRef);
            this.Controls.Add(this.rbSectionRef);
            this.Controls.Add(this.dgvAttribute);
            this.Name = "FormAttributeDocument";
            this.TabText = "FormAttributeDocument";
            this.Text = "FormAttributeDocument";
            this.Deactivate += new System.EventHandler(this.FormAttributeDocument_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAttributeDocument_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAttributeDocument_FormClosed);
            this.Load += new System.EventHandler(this.FormAttributeDocument_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttribute)).EndInit();
            this.contextMenuStripRawAttribute.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorAttributeRaw)).EndInit();
            this.bindingNavigatorAttributeRaw.ResumeLayout(false);
            this.bindingNavigatorAttributeRaw.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAttribute;
        private System.Windows.Forms.RadioButton rbSectionRef;
        private System.Windows.Forms.RadioButton rbLinearRef;
        private System.Windows.Forms.ComboBox cbRoutes;
        private System.Windows.Forms.Label lblRouteFacility;
        private System.Windows.Forms.ComboBox cbYear;
        private System.Windows.Forms.Label lblYears;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox tbSearch;
        private System.Windows.Forms.Label lblAdvancedSearch;
        private System.Windows.Forms.Label labelAttribute;
        private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label labelServer;
        private System.Windows.Forms.CheckBox checkAllowEdit;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRawAttribute;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.BindingNavigator bindingNavigatorAttributeRaw;
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
		private System.Windows.Forms.ToolStripButton tsbImportFromDataSource;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton tsbDeleteAll;
    }
}
