namespace RoadCare3
{
    partial class FormSegmentation
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( FormSegmentation ) );
			this.listBoxSubset = new System.Windows.Forms.ListBox();
			this.tvNetwork = new System.Windows.Forms.TreeView();
			this.textBoxCriteria = new System.Windows.Forms.TextBox();
			this.buttonEditSubset = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonAddRoot = new System.Windows.Forms.Button();
			this.buttonAddChild = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.buttonSegment = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.labelDynamicSegmentation = new System.Windows.Forms.Label();
			this.buttonSegmentNetwork = new System.Windows.Forms.Button();
			this.checkBoxNetworkSegment = new System.Windows.Forms.CheckBox();
			this.dgvResultSegment = new System.Windows.Forms.DataGridView();
			this.buttonNew = new System.Windows.Forms.Button();
			this.bindingNavigatorSegmentation = new System.Windows.Forms.BindingNavigator( this.components );
			this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
			this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
			this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			( ( System.ComponentModel.ISupportInitialize )( this.pictureBox1 ) ).BeginInit();
			( ( System.ComponentModel.ISupportInitialize )( this.dgvResultSegment ) ).BeginInit();
			( ( System.ComponentModel.ISupportInitialize )( this.bindingNavigatorSegmentation ) ).BeginInit();
			this.bindingNavigatorSegmentation.SuspendLayout();
			( ( System.ComponentModel.ISupportInitialize )( this.pictureBox2 ) ).BeginInit();
			this.SuspendLayout();
			// 
			// listBoxSubset
			// 
			this.listBoxSubset.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left ) ) );
			this.listBoxSubset.FormattingEnabled = true;
			this.listBoxSubset.Location = new System.Drawing.Point( 9, 120 );
			this.listBoxSubset.Margin = new System.Windows.Forms.Padding( 2 );
			this.listBoxSubset.MinimumSize = new System.Drawing.Size( 4, 200 );
			this.listBoxSubset.Name = "listBoxSubset";
			this.listBoxSubset.Size = new System.Drawing.Size( 309, 225 );
			this.listBoxSubset.TabIndex = 0;
			this.listBoxSubset.SelectedIndexChanged += new System.EventHandler( this.listBoxSubset_SelectedIndexChanged );
			this.listBoxSubset.KeyUp += new System.Windows.Forms.KeyEventHandler( this.listBoxSubset_KeyUp );
			// 
			// tvNetwork
			// 
			this.tvNetwork.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.tvNetwork.Location = new System.Drawing.Point( 410, 120 );
			this.tvNetwork.Margin = new System.Windows.Forms.Padding( 2 );
			this.tvNetwork.Name = "tvNetwork";
			this.tvNetwork.Size = new System.Drawing.Size( 307, 190 );
			this.tvNetwork.TabIndex = 1;
			// 
			// textBoxCriteria
			// 
			this.textBoxCriteria.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.textBoxCriteria.Location = new System.Drawing.Point( 10, 76 );
			this.textBoxCriteria.Margin = new System.Windows.Forms.Padding( 2 );
			this.textBoxCriteria.Name = "textBoxCriteria";
			this.textBoxCriteria.ReadOnly = true;
			this.textBoxCriteria.Size = new System.Drawing.Size( 669, 20 );
			this.textBoxCriteria.TabIndex = 2;
			// 
			// buttonEditSubset
			// 
			this.buttonEditSubset.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.buttonEditSubset.Enabled = false;
			this.buttonEditSubset.Location = new System.Drawing.Point( 682, 74 );
			this.buttonEditSubset.Margin = new System.Windows.Forms.Padding( 2 );
			this.buttonEditSubset.Name = "buttonEditSubset";
			this.buttonEditSubset.Size = new System.Drawing.Size( 26, 25 );
			this.buttonEditSubset.TabIndex = 3;
			this.buttonEditSubset.Text = "...";
			this.buttonEditSubset.UseVisualStyleBackColor = true;
			this.buttonEditSubset.Click += new System.EventHandler( this.buttonEditSubset_Click );
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point( 9, 60 );
			this.label1.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 );
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size( 78, 13 );
			this.label1.TabIndex = 4;
			this.label1.Text = "Subset Criteria:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point( 7, 102 );
			this.label2.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 );
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size( 89, 13 );
			this.label2.TabIndex = 5;
			this.label2.Text = "Available Subset:";
			// 
			// buttonAddRoot
			// 
			this.buttonAddRoot.Enabled = false;
			this.buttonAddRoot.Font = new System.Drawing.Font( "Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
			this.buttonAddRoot.Location = new System.Drawing.Point( 322, 157 );
			this.buttonAddRoot.Margin = new System.Windows.Forms.Padding( 2 );
			this.buttonAddRoot.Name = "buttonAddRoot";
			this.buttonAddRoot.Size = new System.Drawing.Size( 80, 32 );
			this.buttonAddRoot.TabIndex = 7;
			this.buttonAddRoot.Text = "Add Root";
			this.buttonAddRoot.UseVisualStyleBackColor = true;
			this.buttonAddRoot.Click += new System.EventHandler( this.buttonAddRoot_Click );
			// 
			// buttonAddChild
			// 
			this.buttonAddChild.Enabled = false;
			this.buttonAddChild.Font = new System.Drawing.Font( "Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
			this.buttonAddChild.Location = new System.Drawing.Point( 322, 196 );
			this.buttonAddChild.Margin = new System.Windows.Forms.Padding( 2 );
			this.buttonAddChild.Name = "buttonAddChild";
			this.buttonAddChild.Size = new System.Drawing.Size( 80, 32 );
			this.buttonAddChild.TabIndex = 8;
			this.buttonAddChild.Text = "Add";
			this.buttonAddChild.UseVisualStyleBackColor = true;
			this.buttonAddChild.Click += new System.EventHandler( this.buttonAddChild_Click );
			// 
			// buttonRemove
			// 
			this.buttonRemove.Enabled = false;
			this.buttonRemove.Font = new System.Drawing.Font( "Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
			this.buttonRemove.Location = new System.Drawing.Point( 322, 233 );
			this.buttonRemove.Margin = new System.Windows.Forms.Padding( 2 );
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size( 80, 32 );
			this.buttonRemove.TabIndex = 9;
			this.buttonRemove.Text = "Remove";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.buttonRemove.Click += new System.EventHandler( this.buttonRemove_Click );
			// 
			// buttonSegment
			// 
			this.buttonSegment.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.buttonSegment.Location = new System.Drawing.Point( 27, 566 );
			this.buttonSegment.Margin = new System.Windows.Forms.Padding( 2 );
			this.buttonSegment.Name = "buttonSegment";
			this.buttonSegment.Size = new System.Drawing.Size( 183, 30 );
			this.buttonSegment.TabIndex = 10;
			this.buttonSegment.Text = "Segment Network";
			this.buttonSegment.UseVisualStyleBackColor = true;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::RoadCare3.Properties.Resources.route;
			this.pictureBox1.Location = new System.Drawing.Point( 9, 16 );
			this.pictureBox1.Margin = new System.Windows.Forms.Padding( 2 );
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size( 29, 31 );
			this.pictureBox1.TabIndex = 11;
			this.pictureBox1.TabStop = false;
			// 
			// labelDynamicSegmentation
			// 
			this.labelDynamicSegmentation.AutoSize = true;
			this.labelDynamicSegmentation.Font = new System.Drawing.Font( "Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
			this.labelDynamicSegmentation.Location = new System.Drawing.Point( 44, 16 );
			this.labelDynamicSegmentation.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 );
			this.labelDynamicSegmentation.Name = "labelDynamicSegmentation";
			this.labelDynamicSegmentation.Size = new System.Drawing.Size( 239, 26 );
			this.labelDynamicSegmentation.TabIndex = 12;
			this.labelDynamicSegmentation.Text = "Dynamic Segmentation";
			// 
			// buttonSegmentNetwork
			// 
			this.buttonSegmentNetwork.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.buttonSegmentNetwork.Enabled = false;
			this.buttonSegmentNetwork.Font = new System.Drawing.Font( "Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
			this.buttonSegmentNetwork.Location = new System.Drawing.Point( 410, 320 );
			this.buttonSegmentNetwork.Margin = new System.Windows.Forms.Padding( 2 );
			this.buttonSegmentNetwork.Name = "buttonSegmentNetwork";
			this.buttonSegmentNetwork.Size = new System.Drawing.Size( 158, 32 );
			this.buttonSegmentNetwork.TabIndex = 13;
			this.buttonSegmentNetwork.Text = "Segment Network";
			this.buttonSegmentNetwork.UseVisualStyleBackColor = true;
			this.buttonSegmentNetwork.Click += new System.EventHandler( this.buttonSegmentNetwork_Click );
			// 
			// checkBoxNetworkSegment
			// 
			this.checkBoxNetworkSegment.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.checkBoxNetworkSegment.AutoSize = true;
			this.checkBoxNetworkSegment.Location = new System.Drawing.Point( 546, 11 );
			this.checkBoxNetworkSegment.Margin = new System.Windows.Forms.Padding( 2 );
			this.checkBoxNetworkSegment.Name = "checkBoxNetworkSegment";
			this.checkBoxNetworkSegment.Size = new System.Drawing.Size( 162, 17 );
			this.checkBoxNetworkSegment.TabIndex = 14;
			this.checkBoxNetworkSegment.Text = "Allow Network Segmentation";
			this.checkBoxNetworkSegment.UseVisualStyleBackColor = true;
			this.checkBoxNetworkSegment.CheckedChanged += new System.EventHandler( this.checkBoxNetworkSegment_CheckedChanged );
			// 
			// dgvResultSegment
			// 
			this.dgvResultSegment.AllowUserToAddRows = false;
			this.dgvResultSegment.AllowUserToDeleteRows = false;
			this.dgvResultSegment.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.dgvResultSegment.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.dgvResultSegment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvResultSegment.Location = new System.Drawing.Point( 0, 358 );
			this.dgvResultSegment.Margin = new System.Windows.Forms.Padding( 2 );
			this.dgvResultSegment.Name = "dgvResultSegment";
			this.dgvResultSegment.RowHeadersVisible = false;
			this.dgvResultSegment.RowTemplate.Height = 24;
			this.dgvResultSegment.Size = new System.Drawing.Size( 718, 181 );
			this.dgvResultSegment.TabIndex = 16;
			// 
			// buttonNew
			// 
			this.buttonNew.Enabled = false;
			this.buttonNew.Font = new System.Drawing.Font( "Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
			this.buttonNew.Location = new System.Drawing.Point( 322, 119 );
			this.buttonNew.Margin = new System.Windows.Forms.Padding( 2 );
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.Size = new System.Drawing.Size( 80, 32 );
			this.buttonNew.TabIndex = 17;
			this.buttonNew.Text = "New";
			this.buttonNew.UseVisualStyleBackColor = true;
			this.buttonNew.Click += new System.EventHandler( this.buttonNew_Click );
			// 
			// bindingNavigatorSegmentation
			// 
			this.bindingNavigatorSegmentation.AddNewItem = null;
			this.bindingNavigatorSegmentation.BackColor = System.Drawing.SystemColors.Control;
			this.bindingNavigatorSegmentation.CountItem = this.bindingNavigatorCountItem;
			this.bindingNavigatorSegmentation.DeleteItem = null;
			this.bindingNavigatorSegmentation.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bindingNavigatorSegmentation.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2} );
			this.bindingNavigatorSegmentation.Location = new System.Drawing.Point( 0, 541 );
			this.bindingNavigatorSegmentation.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bindingNavigatorSegmentation.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bindingNavigatorSegmentation.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bindingNavigatorSegmentation.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bindingNavigatorSegmentation.Name = "bindingNavigatorSegmentation";
			this.bindingNavigatorSegmentation.PositionItem = this.bindingNavigatorPositionItem;
			this.bindingNavigatorSegmentation.Size = new System.Drawing.Size( 718, 25 );
			this.bindingNavigatorSegmentation.TabIndex = 18;
			this.bindingNavigatorSegmentation.Text = "bindingNavigator1";
			// 
			// bindingNavigatorCountItem
			// 
			this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
			this.bindingNavigatorCountItem.Size = new System.Drawing.Size( 36, 22 );
			this.bindingNavigatorCountItem.Text = "of {0}";
			this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
			// 
			// bindingNavigatorMoveFirstItem
			// 
			this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bindingNavigatorMoveFirstItem.Image = ( ( System.Drawing.Image )( resources.GetObject( "bindingNavigatorMoveFirstItem.Image" ) ) );
			this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
			this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
			this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size( 23, 22 );
			this.bindingNavigatorMoveFirstItem.Text = "Move first";
			// 
			// bindingNavigatorMovePreviousItem
			// 
			this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bindingNavigatorMovePreviousItem.Image = ( ( System.Drawing.Image )( resources.GetObject( "bindingNavigatorMovePreviousItem.Image" ) ) );
			this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
			this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
			this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size( 23, 22 );
			this.bindingNavigatorMovePreviousItem.Text = "Move previous";
			// 
			// bindingNavigatorSeparator
			// 
			this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
			this.bindingNavigatorSeparator.Size = new System.Drawing.Size( 6, 25 );
			// 
			// bindingNavigatorPositionItem
			// 
			this.bindingNavigatorPositionItem.AccessibleName = "Position";
			this.bindingNavigatorPositionItem.AutoSize = false;
			this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
			this.bindingNavigatorPositionItem.Size = new System.Drawing.Size( 50, 21 );
			this.bindingNavigatorPositionItem.Text = "0";
			this.bindingNavigatorPositionItem.ToolTipText = "Current position";
			// 
			// bindingNavigatorSeparator1
			// 
			this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
			this.bindingNavigatorSeparator1.Size = new System.Drawing.Size( 6, 25 );
			// 
			// bindingNavigatorMoveNextItem
			// 
			this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bindingNavigatorMoveNextItem.Image = ( ( System.Drawing.Image )( resources.GetObject( "bindingNavigatorMoveNextItem.Image" ) ) );
			this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
			this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
			this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size( 23, 22 );
			this.bindingNavigatorMoveNextItem.Text = "Move next";
			// 
			// bindingNavigatorMoveLastItem
			// 
			this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bindingNavigatorMoveLastItem.Image = ( ( System.Drawing.Image )( resources.GetObject( "bindingNavigatorMoveLastItem.Image" ) ) );
			this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
			this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
			this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size( 23, 22 );
			this.bindingNavigatorMoveLastItem.Text = "Move last";
			// 
			// bindingNavigatorSeparator2
			// 
			this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
			this.bindingNavigatorSeparator2.Size = new System.Drawing.Size( 6, 25 );
			// 
			// pictureBox2
			// 
			this.pictureBox2.Location = new System.Drawing.Point( 0, 0 );
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size( 10, 538 );
			this.pictureBox2.TabIndex = 19;
			this.pictureBox2.TabStop = false;
			// 
			// FormSegmentation
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size( 718, 566 );
			this.Controls.Add( this.buttonNew );
			this.Controls.Add( this.buttonRemove );
			this.Controls.Add( this.buttonAddChild );
			this.Controls.Add( this.buttonAddRoot );
			this.Controls.Add( this.bindingNavigatorSegmentation );
			this.Controls.Add( this.dgvResultSegment );
			this.Controls.Add( this.checkBoxNetworkSegment );
			this.Controls.Add( this.buttonSegmentNetwork );
			this.Controls.Add( this.labelDynamicSegmentation );
			this.Controls.Add( this.pictureBox1 );
			this.Controls.Add( this.buttonSegment );
			this.Controls.Add( this.label2 );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.buttonEditSubset );
			this.Controls.Add( this.textBoxCriteria );
			this.Controls.Add( this.tvNetwork );
			this.Controls.Add( this.listBoxSubset );
			this.Controls.Add( this.pictureBox2 );
			this.Margin = new System.Windows.Forms.Padding( 2 );
			this.Name = "FormSegmentation";
			this.TabText = "FormSegmentation";
			this.Text = "FormSegmentation";
			this.Load += new System.EventHandler( this.FormSegmentation_Load );
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.FormSegmentation_FormClosed );
			( ( System.ComponentModel.ISupportInitialize )( this.pictureBox1 ) ).EndInit();
			( ( System.ComponentModel.ISupportInitialize )( this.dgvResultSegment ) ).EndInit();
			( ( System.ComponentModel.ISupportInitialize )( this.bindingNavigatorSegmentation ) ).EndInit();
			this.bindingNavigatorSegmentation.ResumeLayout( false );
			this.bindingNavigatorSegmentation.PerformLayout();
			( ( System.ComponentModel.ISupportInitialize )( this.pictureBox2 ) ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxSubset;
        private System.Windows.Forms.TreeView tvNetwork;
        private System.Windows.Forms.TextBox textBoxCriteria;
        private System.Windows.Forms.Button buttonEditSubset;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonAddRoot;
        private System.Windows.Forms.Button buttonAddChild;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonSegment;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelDynamicSegmentation;
        private System.Windows.Forms.Button buttonSegmentNetwork;
		private System.Windows.Forms.CheckBox checkBoxNetworkSegment;
        private System.Windows.Forms.DataGridView dgvResultSegment;
        private System.Windows.Forms.Button buttonNew;
        private System.Windows.Forms.BindingNavigator bindingNavigatorSegmentation;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}