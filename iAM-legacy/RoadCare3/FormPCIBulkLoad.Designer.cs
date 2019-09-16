namespace RoadCare3
{
    partial class FormPCIBulkLoad
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPCIBulkLoad));
			this.dgvPCIBulkLoad = new System.Windows.Forms.DataGridView();
			this.contextMenuStripPCIBulkLoad = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripPCIBulkLoad = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonValidate = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonImport = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorPCIValidation = new System.Windows.Forms.BindingNavigator(this.components);
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
			this.tsbPCIDistress = new System.Windows.Forms.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(this.dgvPCIBulkLoad)).BeginInit();
			this.contextMenuStripPCIBulkLoad.SuspendLayout();
			this.toolStripPCIBulkLoad.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorPCIValidation)).BeginInit();
			this.bindingNavigatorPCIValidation.SuspendLayout();
			this.SuspendLayout();
			// 
			// dgvPCIBulkLoad
			// 
			this.dgvPCIBulkLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dgvPCIBulkLoad.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvPCIBulkLoad.ContextMenuStrip = this.contextMenuStripPCIBulkLoad;
			this.dgvPCIBulkLoad.Location = new System.Drawing.Point(1, 0);
			this.dgvPCIBulkLoad.Name = "dgvPCIBulkLoad";
			this.dgvPCIBulkLoad.Size = new System.Drawing.Size(920, 583);
			this.dgvPCIBulkLoad.TabIndex = 0;
			// 
			// contextMenuStripPCIBulkLoad
			// 
			this.contextMenuStripPCIBulkLoad.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pasteToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.deleteToolStripMenuItem});
			this.contextMenuStripPCIBulkLoad.Name = "contextMenuStripPCIBulkLoad";
			this.contextMenuStripPCIBulkLoad.Size = new System.Drawing.Size(117, 70);
			this.contextMenuStripPCIBulkLoad.Text = "Actions";
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.pasteToolStripMenuItem.Text = "Paste";
			this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			// 
			// toolStripPCIBulkLoad
			// 
			this.toolStripPCIBulkLoad.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.toolStripPCIBulkLoad.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonValidate,
            this.toolStripButtonImport,
            this.tsbPCIDistress});
			this.toolStripPCIBulkLoad.Location = new System.Drawing.Point(0, 585);
			this.toolStripPCIBulkLoad.Name = "toolStripPCIBulkLoad";
			this.toolStripPCIBulkLoad.Size = new System.Drawing.Size(922, 25);
			this.toolStripPCIBulkLoad.TabIndex = 1;
			this.toolStripPCIBulkLoad.Text = "toolStrip1";
			// 
			// toolStripButtonValidate
			// 
			this.toolStripButtonValidate.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonValidate.Image")));
			this.toolStripButtonValidate.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonValidate.Name = "toolStripButtonValidate";
			this.toolStripButtonValidate.Size = new System.Drawing.Size(65, 22);
			this.toolStripButtonValidate.Text = "Validate";
			this.toolStripButtonValidate.Click += new System.EventHandler(this.toolStripButtonValidate_Click);
			// 
			// toolStripButtonImport
			// 
			this.toolStripButtonImport.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonImport.Image")));
			this.toolStripButtonImport.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonImport.Name = "toolStripButtonImport";
			this.toolStripButtonImport.Size = new System.Drawing.Size(59, 22);
			this.toolStripButtonImport.Text = "Import";
			this.toolStripButtonImport.Click += new System.EventHandler(this.toolStripButtonImport_Click);
			// 
			// bindingNavigatorPCIValidation
			// 
			this.bindingNavigatorPCIValidation.AddNewItem = this.bindingNavigatorAddNewItem;
			this.bindingNavigatorPCIValidation.CountItem = this.bindingNavigatorCountItem;
			this.bindingNavigatorPCIValidation.DeleteItem = this.bindingNavigatorDeleteItem;
			this.bindingNavigatorPCIValidation.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bindingNavigatorPCIValidation.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.bindingNavigatorDeleteItem});
			this.bindingNavigatorPCIValidation.Location = new System.Drawing.Point(0, 560);
			this.bindingNavigatorPCIValidation.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bindingNavigatorPCIValidation.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bindingNavigatorPCIValidation.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bindingNavigatorPCIValidation.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bindingNavigatorPCIValidation.Name = "bindingNavigatorPCIValidation";
			this.bindingNavigatorPCIValidation.PositionItem = this.bindingNavigatorPositionItem;
			this.bindingNavigatorPCIValidation.Size = new System.Drawing.Size(922, 25);
			this.bindingNavigatorPCIValidation.TabIndex = 2;
			this.bindingNavigatorPCIValidation.Text = "bindingNavigator1";
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
			this.bindingNavigatorCountItem.Size = new System.Drawing.Size(36, 22);
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
			// tsbPCIDistress
			// 
			this.tsbPCIDistress.Image = ((System.Drawing.Image)(resources.GetObject("tsbPCIDistress.Image")));
			this.tsbPCIDistress.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbPCIDistress.Name = "tsbPCIDistress";
			this.tsbPCIDistress.Size = new System.Drawing.Size(142, 22);
			this.tsbPCIDistress.Text = "Define PCI Distresses...";
			this.tsbPCIDistress.Click += new System.EventHandler(this.tsbPCIDistress_Click);
			// 
			// FormPCIBulkLoad
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(922, 610);
			this.Controls.Add(this.bindingNavigatorPCIValidation);
			this.Controls.Add(this.toolStripPCIBulkLoad);
			this.Controls.Add(this.dgvPCIBulkLoad);
			this.Name = "FormPCIBulkLoad";
			this.TabText = "FormPCIBulkLoad";
			this.Text = "PCI Bulk Loader";
			this.Load += new System.EventHandler(this.FormPCIBulkLoad_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgvPCIBulkLoad)).EndInit();
			this.contextMenuStripPCIBulkLoad.ResumeLayout(false);
			this.toolStripPCIBulkLoad.ResumeLayout(false);
			this.toolStripPCIBulkLoad.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorPCIValidation)).EndInit();
			this.bindingNavigatorPCIValidation.ResumeLayout(false);
			this.bindingNavigatorPCIValidation.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPCIBulkLoad;
        private System.Windows.Forms.ToolStrip toolStripPCIBulkLoad;
        private System.Windows.Forms.ToolStripButton toolStripButtonValidate;
        private System.Windows.Forms.ToolStripButton toolStripButtonImport;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripPCIBulkLoad;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.BindingNavigator bindingNavigatorPCIValidation;
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
		private System.Windows.Forms.ToolStripButton tsbPCIDistress;
    }
}