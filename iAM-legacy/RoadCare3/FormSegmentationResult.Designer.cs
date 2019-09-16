namespace RoadCare3
{
    partial class FormSegmentationResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSegmentationResult));
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.labelResults = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboRoute = new System.Windows.Forms.ComboBox();
            this.checkBoxNetworkSegment = new System.Windows.Forms.CheckBox();
            this.buttonJoin = new System.Windows.Forms.Button();
            this.buttonSplit = new System.Windows.Forms.Button();
            this.labelINTO = new System.Windows.Forms.Label();
            this.textBoxSplit = new System.Windows.Forms.TextBox();
            this.labelSECTIONS = new System.Windows.Forms.Label();
            this.bindingNavigatorSegmentationResult = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tslblMinimum = new System.Windows.Forms.ToolStripLabel();
            this.tstbMinimum = new System.Windows.Forms.ToolStripTextBox();
            this.tslblMaximum = new System.Windows.Forms.ToolStripLabel();
            this.tstbMaximum = new System.Windows.Forms.ToolStripTextBox();
            this.tslblIncrements = new System.Windows.Forms.ToolStripLabel();
            this.tscbIncrements = new System.Windows.Forms.ToolStripComboBox();
            this.tsbtnApplyToDisplay = new System.Windows.Forms.ToolStripButton();
            this.tsbtn = new System.Windows.Forms.ToolStripButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnCommit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorSegmentationResult)).BeginInit();
            this.bindingNavigatorSegmentationResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvResults
            // 
            this.dgvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Location = new System.Drawing.Point(-2, 93);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.RowTemplate.Height = 24;
            this.dgvResults.Size = new System.Drawing.Size(1151, 484);
            this.dgvResults.TabIndex = 0;
            this.dgvResults.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvResults_CellValueChanged);
            // 
            // labelResults
            // 
            this.labelResults.AutoSize = true;
            this.labelResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResults.Location = new System.Drawing.Point(44, 12);
            this.labelResults.Name = "labelResults";
            this.labelResults.Size = new System.Drawing.Size(228, 26);
            this.labelResults.TabIndex = 1;
            this.labelResults.Text = "Segmentation Result -";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Route:";
            // 
            // comboRoute
            // 
            this.comboRoute.FormattingEnabled = true;
            this.comboRoute.Location = new System.Drawing.Point(79, 57);
            this.comboRoute.Name = "comboRoute";
            this.comboRoute.Size = new System.Drawing.Size(234, 23);
            this.comboRoute.TabIndex = 3;
            this.comboRoute.SelectedIndexChanged += new System.EventHandler(this.comboRoute_SelectedIndexChanged);
            // 
            // checkBoxNetworkSegment
            // 
            this.checkBoxNetworkSegment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxNetworkSegment.AutoSize = true;
            this.checkBoxNetworkSegment.Location = new System.Drawing.Point(950, 10);
            this.checkBoxNetworkSegment.Name = "checkBoxNetworkSegment";
            this.checkBoxNetworkSegment.Size = new System.Drawing.Size(183, 19);
            this.checkBoxNetworkSegment.TabIndex = 17;
            this.checkBoxNetworkSegment.Text = "Allow Network Segmentation";
            this.checkBoxNetworkSegment.UseVisualStyleBackColor = true;
            // 
            // buttonJoin
            // 
            this.buttonJoin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonJoin.Location = new System.Drawing.Point(345, 50);
            this.buttonJoin.Name = "buttonJoin";
            this.buttonJoin.Size = new System.Drawing.Size(76, 36);
            this.buttonJoin.TabIndex = 18;
            this.buttonJoin.Text = "Join";
            this.buttonJoin.UseVisualStyleBackColor = true;
            this.buttonJoin.Click += new System.EventHandler(this.buttonJoin_Click);
            // 
            // buttonSplit
            // 
            this.buttonSplit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSplit.Location = new System.Drawing.Point(427, 50);
            this.buttonSplit.Name = "buttonSplit";
            this.buttonSplit.Size = new System.Drawing.Size(76, 36);
            this.buttonSplit.TabIndex = 19;
            this.buttonSplit.Text = "Split";
            this.buttonSplit.UseVisualStyleBackColor = true;
            this.buttonSplit.Click += new System.EventHandler(this.buttonSplit_Click);
            // 
            // labelINTO
            // 
            this.labelINTO.AutoSize = true;
            this.labelINTO.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelINTO.Location = new System.Drawing.Point(509, 57);
            this.labelINTO.Name = "labelINTO";
            this.labelINTO.Size = new System.Drawing.Size(35, 15);
            this.labelINTO.TabIndex = 20;
            this.labelINTO.Text = "INTO";
            // 
            // textBoxSplit
            // 
            this.textBoxSplit.Location = new System.Drawing.Point(550, 55);
            this.textBoxSplit.Name = "textBoxSplit";
            this.textBoxSplit.Size = new System.Drawing.Size(61, 21);
            this.textBoxSplit.TabIndex = 21;
            this.textBoxSplit.Text = "2";
            // 
            // labelSECTIONS
            // 
            this.labelSECTIONS.AutoSize = true;
            this.labelSECTIONS.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSECTIONS.Location = new System.Drawing.Point(617, 57);
            this.labelSECTIONS.Name = "labelSECTIONS";
            this.labelSECTIONS.Size = new System.Drawing.Size(67, 15);
            this.labelSECTIONS.TabIndex = 22;
            this.labelSECTIONS.Text = "SECTIONS";
            // 
            // bindingNavigatorSegmentationResult
            // 
            this.bindingNavigatorSegmentationResult.AddNewItem = null;
            this.bindingNavigatorSegmentationResult.BackColor = System.Drawing.SystemColors.Control;
            this.bindingNavigatorSegmentationResult.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigatorSegmentationResult.DeleteItem = null;
            this.bindingNavigatorSegmentationResult.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bindingNavigatorSegmentationResult.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.tslblMinimum,
            this.tstbMinimum,
            this.tslblMaximum,
            this.tstbMaximum,
            this.tslblIncrements,
            this.tscbIncrements,
            this.tsbtnApplyToDisplay,
            this.tsbtn});
            this.bindingNavigatorSegmentationResult.Location = new System.Drawing.Point(0, 580);
            this.bindingNavigatorSegmentationResult.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigatorSegmentationResult.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigatorSegmentationResult.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigatorSegmentationResult.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigatorSegmentationResult.Name = "bindingNavigatorSegmentationResult";
            this.bindingNavigatorSegmentationResult.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigatorSegmentationResult.Size = new System.Drawing.Size(1146, 25);
            this.bindingNavigatorSegmentationResult.TabIndex = 31;
            this.bindingNavigatorSegmentationResult.Text = "bindingNavigator1";
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
            // tslblMinimum
            // 
            this.tslblMinimum.Name = "tslblMinimum";
            this.tslblMinimum.Size = new System.Drawing.Size(63, 22);
            this.tslblMinimum.Text = "Minimum:";
            // 
            // tstbMinimum
            // 
            this.tstbMinimum.Name = "tstbMinimum";
            this.tstbMinimum.Size = new System.Drawing.Size(50, 25);
            this.tstbMinimum.Text = "0.1";
            // 
            // tslblMaximum
            // 
            this.tslblMaximum.Name = "tslblMaximum";
            this.tslblMaximum.Size = new System.Drawing.Size(64, 22);
            this.tslblMaximum.Text = "Maximum:";
            // 
            // tstbMaximum
            // 
            this.tstbMaximum.Name = "tstbMaximum";
            this.tstbMaximum.Size = new System.Drawing.Size(50, 25);
            this.tstbMaximum.Text = "1.0";
            // 
            // tslblIncrements
            // 
            this.tslblIncrements.Name = "tslblIncrements";
            this.tslblIncrements.Size = new System.Drawing.Size(66, 22);
            this.tslblIncrements.Text = "Increments";
            // 
            // tscbIncrements
            // 
            this.tscbIncrements.Items.AddRange(new object[] {
            "Even",
            "Exact",
            "Minimum Only"});
            this.tscbIncrements.Name = "tscbIncrements";
            this.tscbIncrements.Size = new System.Drawing.Size(100, 25);
            this.tscbIncrements.Text = "Even";
            // 
            // tsbtnApplyToDisplay
            // 
            this.tsbtnApplyToDisplay.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnApplyToDisplay.Image")));
            this.tsbtnApplyToDisplay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnApplyToDisplay.Name = "tsbtnApplyToDisplay";
            this.tsbtnApplyToDisplay.Size = new System.Drawing.Size(116, 22);
            this.tsbtnApplyToDisplay.Text = "Apply To Display";
            this.tsbtnApplyToDisplay.Click += new System.EventHandler(this.tsbtnApplyToDisplay_Click);
            // 
            // tsbtn
            // 
            this.tsbtn.Image = ((System.Drawing.Image)(resources.GetObject("tsbtn.Image")));
            this.tsbtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtn.Name = "tsbtn";
            this.tsbtn.Size = new System.Drawing.Size(92, 22);
            this.tsbtn.Text = "Apply To All";
            this.tsbtn.Click += new System.EventHandler(this.tsbtn_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(11, 11);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(28, 30);
            this.pictureBox2.TabIndex = 33;
            this.pictureBox2.TabStop = false;
            // 
            // btnCommit
            // 
            this.btnCommit.Enabled = false;
            this.btnCommit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCommit.Location = new System.Drawing.Point(746, 50);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(130, 36);
            this.btnCommit.TabIndex = 34;
            this.btnCommit.Text = "Commit Changes";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // FormSegmentationResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1146, 605);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.bindingNavigatorSegmentationResult);
            this.Controls.Add(this.labelSECTIONS);
            this.Controls.Add(this.textBoxSplit);
            this.Controls.Add(this.labelINTO);
            this.Controls.Add(this.buttonSplit);
            this.Controls.Add(this.buttonJoin);
            this.Controls.Add(this.checkBoxNetworkSegment);
            this.Controls.Add(this.comboRoute);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelResults);
            this.Controls.Add(this.dgvResults);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FormSegmentationResult";
            this.TabText = "FormSegmentationResult";
            this.Text = "FormSegmentationResult";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSegmentationResult_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSegmentationResult_FormClosed);
            this.Load += new System.EventHandler(this.FormSegmentationResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorSegmentationResult)).EndInit();
            this.bindingNavigatorSegmentationResult.ResumeLayout(false);
            this.bindingNavigatorSegmentationResult.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.Label labelResults;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboRoute;
        private System.Windows.Forms.CheckBox checkBoxNetworkSegment;
        private System.Windows.Forms.Button buttonJoin;
        private System.Windows.Forms.Button buttonSplit;
        private System.Windows.Forms.Label labelINTO;
        private System.Windows.Forms.TextBox textBoxSplit;
        private System.Windows.Forms.Label labelSECTIONS;
        private System.Windows.Forms.BindingNavigator bindingNavigatorSegmentationResult;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripLabel tslblMinimum;
        private System.Windows.Forms.ToolStripTextBox tstbMinimum;
        private System.Windows.Forms.ToolStripLabel tslblMaximum;
        private System.Windows.Forms.ToolStripTextBox tstbMaximum;
        private System.Windows.Forms.ToolStripComboBox tscbIncrements;
        private System.Windows.Forms.ToolStripLabel tslblIncrements;
        private System.Windows.Forms.ToolStripButton tsbtn;
		private System.Windows.Forms.ToolStripButton tsbtnApplyToDisplay;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Button btnCommit;
    }
}