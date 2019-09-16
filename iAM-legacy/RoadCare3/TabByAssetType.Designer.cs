namespace RoadCare3
{
	partial class TabByAssetType
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( TabByAssetType ) );
			this.dgvAssetsByType = new System.Windows.Forms.DataGridView();
			this.bindingNavigatorAssetsByType = new System.Windows.Forms.BindingNavigator( this.components );
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
			this.cbAssetTypes = new System.Windows.Forms.ComboBox();
			( ( System.ComponentModel.ISupportInitialize )( this.dgvAssetsByType ) ).BeginInit();
			( ( System.ComponentModel.ISupportInitialize )( this.bindingNavigatorAssetsByType ) ).BeginInit();
			this.bindingNavigatorAssetsByType.SuspendLayout();
			this.SuspendLayout();
			// 
			// dgvAssetsByType
			// 
			this.dgvAssetsByType.AllowUserToAddRows = false;
			this.dgvAssetsByType.AllowUserToDeleteRows = false;
			this.dgvAssetsByType.AllowUserToOrderColumns = true;
			this.dgvAssetsByType.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.dgvAssetsByType.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvAssetsByType.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAssetsByType.Location = new System.Drawing.Point( -1, 23 );
			this.dgvAssetsByType.Name = "dgvAssetsByType";
			this.dgvAssetsByType.ReadOnly = true;
			this.dgvAssetsByType.RowHeadersVisible = false;
			this.dgvAssetsByType.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvAssetsByType.Size = new System.Drawing.Size( 421, 355 );
			this.dgvAssetsByType.TabIndex = 0;
			// 
			// bindingNavigatorAssetsByType
			// 
			this.bindingNavigatorAssetsByType.AddNewItem = this.bindingNavigatorAddNewItem;
			this.bindingNavigatorAssetsByType.CountItem = this.bindingNavigatorCountItem;
			this.bindingNavigatorAssetsByType.DeleteItem = this.bindingNavigatorDeleteItem;
			this.bindingNavigatorAssetsByType.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bindingNavigatorAssetsByType.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
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
            this.bindingNavigatorDeleteItem} );
			this.bindingNavigatorAssetsByType.Location = new System.Drawing.Point( 0, 381 );
			this.bindingNavigatorAssetsByType.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bindingNavigatorAssetsByType.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bindingNavigatorAssetsByType.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bindingNavigatorAssetsByType.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bindingNavigatorAssetsByType.Name = "bindingNavigatorAssetsByType";
			this.bindingNavigatorAssetsByType.PositionItem = this.bindingNavigatorPositionItem;
			this.bindingNavigatorAssetsByType.Size = new System.Drawing.Size( 419, 25 );
			this.bindingNavigatorAssetsByType.TabIndex = 1;
			this.bindingNavigatorAssetsByType.Text = "bindingNavigator1";
			// 
			// bindingNavigatorAddNewItem
			// 
			this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bindingNavigatorAddNewItem.Image = ( ( System.Drawing.Image )( resources.GetObject( "bindingNavigatorAddNewItem.Image" ) ) );
			this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
			this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
			this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size( 23, 22 );
			this.bindingNavigatorAddNewItem.Text = "Add new";
			// 
			// bindingNavigatorCountItem
			// 
			this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
			this.bindingNavigatorCountItem.Size = new System.Drawing.Size( 36, 22 );
			this.bindingNavigatorCountItem.Text = "of {0}";
			this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
			// 
			// bindingNavigatorDeleteItem
			// 
			this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bindingNavigatorDeleteItem.Image = ( ( System.Drawing.Image )( resources.GetObject( "bindingNavigatorDeleteItem.Image" ) ) );
			this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
			this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
			this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size( 23, 22 );
			this.bindingNavigatorDeleteItem.Text = "Delete";
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
			// cbAssetTypes
			// 
			this.cbAssetTypes.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.cbAssetTypes.FormattingEnabled = true;
			this.cbAssetTypes.Location = new System.Drawing.Point( 0, 1 );
			this.cbAssetTypes.Name = "cbAssetTypes";
			this.cbAssetTypes.Size = new System.Drawing.Size( 419, 21 );
			this.cbAssetTypes.TabIndex = 2;
			this.cbAssetTypes.SelectedIndexChanged += new System.EventHandler( this.cbAssetTypes_SelectedIndexChanged );
			// 
			// TabByAssetType
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 419, 406 );
			this.Controls.Add( this.cbAssetTypes );
			this.Controls.Add( this.bindingNavigatorAssetsByType );
			this.Controls.Add( this.dgvAssetsByType );
			this.Name = "TabByAssetType";
			this.TabText = "Assets By Type";
			this.Text = "Assets By Type";
			this.Load += new System.EventHandler( this.TabByAssetType_Load );
			this.Enter += new System.EventHandler( this.TabByAssetType_Enter );
			( ( System.ComponentModel.ISupportInitialize )( this.dgvAssetsByType ) ).EndInit();
			( ( System.ComponentModel.ISupportInitialize )( this.bindingNavigatorAssetsByType ) ).EndInit();
			this.bindingNavigatorAssetsByType.ResumeLayout( false );
			this.bindingNavigatorAssetsByType.PerformLayout();
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dgvAssetsByType;
		private System.Windows.Forms.BindingNavigator bindingNavigatorAssetsByType;
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
		private System.Windows.Forms.ComboBox cbAssetTypes;
	}
}