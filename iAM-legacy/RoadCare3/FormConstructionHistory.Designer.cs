namespace RoadCare3
{
    partial class FormConstructionHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConstructionHistory));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblYears = new System.Windows.Forms.Label();
            this.cbYear = new System.Windows.Forms.ComboBox();
            this.lblRouteFacility = new System.Windows.Forms.Label();
            this.cbRoutes = new System.Windows.Forms.ComboBox();
            this.rbLinearRef = new System.Windows.Forms.RadioButton();
            this.rbSectionRef = new System.Windows.Forms.RadioButton();
            this.checkAllowEdit = new System.Windows.Forms.CheckBox();
            this.buttonExport = new System.Windows.Forms.Button();
            this.dgvConstructionHistory = new System.Windows.Forms.DataGridView();
            this.bindingNavigatorConstructionHistory = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConstructionHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorConstructionHistory)).BeginInit();
            this.bindingNavigatorConstructionHistory.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RoadCare3.Properties.Resources.construction;
            this.pictureBox1.Location = new System.Drawing.Point(10, 11);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 28);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(39, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(186, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "Construction Data";
            // 
            // lblYears
            // 
            this.lblYears.AutoSize = true;
            this.lblYears.Location = new System.Drawing.Point(254, 45);
            this.lblYears.Name = "lblYears";
            this.lblYears.Size = new System.Drawing.Size(32, 13);
            this.lblYears.TabIndex = 12;
            this.lblYears.Text = "Year:";
            // 
            // cbYear
            // 
            this.cbYear.FormattingEnabled = true;
            this.cbYear.Location = new System.Drawing.Point(292, 40);
            this.cbYear.Name = "cbYear";
            this.cbYear.Size = new System.Drawing.Size(121, 21);
            this.cbYear.TabIndex = 11;
            this.cbYear.SelectedIndexChanged += new System.EventHandler(this.cbYear_SelectedIndexChanged);
            // 
            // lblRouteFacility
            // 
            this.lblRouteFacility.AutoSize = true;
            this.lblRouteFacility.Location = new System.Drawing.Point(248, 17);
            this.lblRouteFacility.Name = "lblRouteFacility";
            this.lblRouteFacility.Size = new System.Drawing.Size(39, 13);
            this.lblRouteFacility.TabIndex = 10;
            this.lblRouteFacility.Text = "Route:";
            // 
            // cbRoutes
            // 
            this.cbRoutes.FormattingEnabled = true;
            this.cbRoutes.Location = new System.Drawing.Point(292, 12);
            this.cbRoutes.Name = "cbRoutes";
            this.cbRoutes.Size = new System.Drawing.Size(121, 21);
            this.cbRoutes.TabIndex = 9;
            this.cbRoutes.SelectedIndexChanged += new System.EventHandler(this.cbRoutes_SelectedIndexChanged);
            // 
            // rbLinearRef
            // 
            this.rbLinearRef.AutoSize = true;
            this.rbLinearRef.Location = new System.Drawing.Point(12, 45);
            this.rbLinearRef.Name = "rbLinearRef";
            this.rbLinearRef.Size = new System.Drawing.Size(107, 17);
            this.rbLinearRef.TabIndex = 8;
            this.rbLinearRef.TabStop = true;
            this.rbLinearRef.Text = "Linear Reference";
            this.rbLinearRef.UseVisualStyleBackColor = true;
            this.rbLinearRef.CheckedChanged += new System.EventHandler(this.rbLinearRef_CheckedChanged);
            // 
            // rbSectionRef
            // 
            this.rbSectionRef.AutoSize = true;
            this.rbSectionRef.Location = new System.Drawing.Point(131, 45);
            this.rbSectionRef.Name = "rbSectionRef";
            this.rbSectionRef.Size = new System.Drawing.Size(114, 17);
            this.rbSectionRef.TabIndex = 7;
            this.rbSectionRef.TabStop = true;
            this.rbSectionRef.Text = "Section Reference";
            this.rbSectionRef.UseVisualStyleBackColor = true;
            // 
            // checkAllowEdit
            // 
            this.checkAllowEdit.AutoSize = true;
            this.checkAllowEdit.Checked = true;
            this.checkAllowEdit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAllowEdit.Location = new System.Drawing.Point(429, 11);
            this.checkAllowEdit.Margin = new System.Windows.Forms.Padding(2);
            this.checkAllowEdit.Name = "checkAllowEdit";
            this.checkAllowEdit.Size = new System.Drawing.Size(169, 17);
            this.checkAllowEdit.TabIndex = 17;
            this.checkAllowEdit.Text = "Allow Construction History Edit";
            this.checkAllowEdit.UseVisualStyleBackColor = true;
            // 
            // buttonExport
            // 
            this.buttonExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExport.Location = new System.Drawing.Point(429, 35);
            this.buttonExport.Margin = new System.Windows.Forms.Padding(2);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(166, 25);
            this.buttonExport.TabIndex = 16;
            this.buttonExport.Text = "Export to file";
            this.buttonExport.UseVisualStyleBackColor = true;
            // 
            // dgvConstructionHistory
            // 
            this.dgvConstructionHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvConstructionHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConstructionHistory.Location = new System.Drawing.Point(0, 68);
            this.dgvConstructionHistory.Margin = new System.Windows.Forms.Padding(2);
            this.dgvConstructionHistory.Name = "dgvConstructionHistory";
            this.dgvConstructionHistory.RowTemplate.Height = 24;
            this.dgvConstructionHistory.Size = new System.Drawing.Size(751, 375);
            this.dgvConstructionHistory.TabIndex = 18;
            // 
            // bindingNavigatorConstructionHistory
            // 
            this.bindingNavigatorConstructionHistory.AddNewItem = this.bindingNavigatorAddNewItem;
            this.bindingNavigatorConstructionHistory.BackColor = System.Drawing.SystemColors.Control;
            this.bindingNavigatorConstructionHistory.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigatorConstructionHistory.DeleteItem = this.bindingNavigatorDeleteItem;
            this.bindingNavigatorConstructionHistory.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bindingNavigatorConstructionHistory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.bindingNavigatorConstructionHistory.Location = new System.Drawing.Point(0, 445);
            this.bindingNavigatorConstructionHistory.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigatorConstructionHistory.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigatorConstructionHistory.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigatorConstructionHistory.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigatorConstructionHistory.Name = "bindingNavigatorConstructionHistory";
            this.bindingNavigatorConstructionHistory.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigatorConstructionHistory.Size = new System.Drawing.Size(749, 25);
            this.bindingNavigatorConstructionHistory.TabIndex = 19;
            this.bindingNavigatorConstructionHistory.Text = "bindingNavigator1";
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
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(36, 22);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator";
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
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
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
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorDeleteItem.Text = "Delete";
            // 
            // FormConstructionHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 470);
            this.Controls.Add(this.bindingNavigatorConstructionHistory);
            this.Controls.Add(this.dgvConstructionHistory);
            this.Controls.Add(this.checkAllowEdit);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.lblYears);
            this.Controls.Add(this.cbYear);
            this.Controls.Add(this.lblRouteFacility);
            this.Controls.Add(this.cbRoutes);
            this.Controls.Add(this.rbLinearRef);
            this.Controls.Add(this.rbSectionRef);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormConstructionHistory";
            this.TabText = "FormConstructionHistory";
            this.Text = "FormConstructionHistory";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.Load += new System.EventHandler(this.FormConstructionHistory_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormConstructionHistory_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConstructionHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorConstructionHistory)).EndInit();
            this.bindingNavigatorConstructionHistory.ResumeLayout(false);
            this.bindingNavigatorConstructionHistory.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblYears;
        private System.Windows.Forms.ComboBox cbYear;
        private System.Windows.Forms.Label lblRouteFacility;
        private System.Windows.Forms.ComboBox cbRoutes;
        private System.Windows.Forms.RadioButton rbLinearRef;
        private System.Windows.Forms.RadioButton rbSectionRef;
        private System.Windows.Forms.CheckBox checkAllowEdit;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.DataGridView dgvConstructionHistory;
        private System.Windows.Forms.BindingNavigator bindingNavigatorConstructionHistory;
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

    }
}