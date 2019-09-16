namespace RoadCare3
{
	partial class FormMultiLineStringDefinition
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
			SharpMap.Map map1 = new SharpMap.Map();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMultiLineStringDefinition));
			System.Drawing.Drawing2D.Matrix matrix1 = new System.Drawing.Drawing2D.Matrix();
			this.dgvMultiLineString = new System.Windows.Forms.DataGridView();
			this.btnPan = new System.Windows.Forms.Button();
			this.btnInformation = new System.Windows.Forms.Button();
			this.btnZoomIn = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.mapImageDefinition = new SharpMap.Forms.MapImage();
			this.bindingNavigatorMLS = new System.Windows.Forms.BindingNavigator(this.components);
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
			((System.ComponentModel.ISupportInitialize)(this.dgvMultiLineString)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapImageDefinition)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorMLS)).BeginInit();
			this.bindingNavigatorMLS.SuspendLayout();
			this.SuspendLayout();
			// 
			// dgvMultiLineString
			// 
			this.dgvMultiLineString.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvMultiLineString.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvMultiLineString.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.dgvMultiLineString.Location = new System.Drawing.Point(0, 0);
			this.dgvMultiLineString.Name = "dgvMultiLineString";
			this.dgvMultiLineString.Size = new System.Drawing.Size(599, 126);
			this.dgvMultiLineString.TabIndex = 11;
			this.dgvMultiLineString.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvMultiLineString_MouseUp);
			this.dgvMultiLineString.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMultiLineString_CellEndEdit);
			this.dgvMultiLineString.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgvMultiLineString_KeyUp);
			// 
			// btnPan
			// 
			this.btnPan.Image = global::RoadCare3.Properties.Resources.pan_icon;
			this.btnPan.Location = new System.Drawing.Point(65, 3);
			this.btnPan.Name = "btnPan";
			this.btnPan.Size = new System.Drawing.Size(25, 23);
			this.btnPan.TabIndex = 14;
			this.btnPan.UseVisualStyleBackColor = true;
			this.btnPan.Click += new System.EventHandler(this.btnPan_Click);
			// 
			// btnInformation
			// 
			this.btnInformation.Image = global::RoadCare3.Properties.Resources.select;
			this.btnInformation.Location = new System.Drawing.Point(3, 3);
			this.btnInformation.Name = "btnInformation";
			this.btnInformation.Size = new System.Drawing.Size(25, 23);
			this.btnInformation.TabIndex = 13;
			this.btnInformation.UseVisualStyleBackColor = true;
			this.btnInformation.Click += new System.EventHandler(this.btnInformation_Click);
			// 
			// btnZoomIn
			// 
			this.btnZoomIn.Image = global::RoadCare3.Properties.Resources.zoomIn;
			this.btnZoomIn.Location = new System.Drawing.Point(34, 3);
			this.btnZoomIn.Name = "btnZoomIn";
			this.btnZoomIn.Size = new System.Drawing.Size(25, 23);
			this.btnZoomIn.TabIndex = 12;
			this.btnZoomIn.UseVisualStyleBackColor = true;
			this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.btnInformation);
			this.splitContainer1.Panel1.Controls.Add(this.btnPan);
			this.splitContainer1.Panel1.Controls.Add(this.btnZoomIn);
			this.splitContainer1.Panel1.Controls.Add(this.mapImageDefinition);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.bindingNavigatorMLS);
			this.splitContainer1.Panel2.Controls.Add(this.dgvMultiLineString);
			this.splitContainer1.Size = new System.Drawing.Size(599, 368);
			this.splitContainer1.SplitterDistance = 238;
			this.splitContainer1.TabIndex = 15;
			// 
			// mapImageDefinition
			// 
			this.mapImageDefinition.ActiveTool = SharpMap.Forms.MapImage.Tools.None;
			this.mapImageDefinition.BackColor = System.Drawing.Color.White;
			this.mapImageDefinition.Cursor = System.Windows.Forms.Cursors.Cross;
			this.mapImageDefinition.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mapImageDefinition.Location = new System.Drawing.Point(0, 0);
			map1.BackColor = System.Drawing.Color.Transparent;
			map1.Center = ((SharpMap.Geometries.Point)(resources.GetObject("map1.Center")));
			map1.Layers = ((System.Collections.Generic.List<SharpMap.Layers.ILayer>)(resources.GetObject("map1.Layers")));
			map1.MapTransform = matrix1;
			map1.MaximumZoom = 1.7976931348623157E+308;
			map1.MinimumZoom = 0;
			map1.PixelAspectRatio = 1;
			map1.Size = new System.Drawing.Size(100, 50);
			map1.Zoom = 1;
			this.mapImageDefinition.Map = map1;
			this.mapImageDefinition.Name = "mapImageDefinition";
			this.mapImageDefinition.QueryLayerIndex = 0;
			this.mapImageDefinition.Size = new System.Drawing.Size(599, 238);
			this.mapImageDefinition.TabIndex = 15;
			this.mapImageDefinition.TabStop = false;
			// 
			// bindingNavigatorMLS
			// 
			this.bindingNavigatorMLS.AddNewItem = this.bindingNavigatorAddNewItem;
			this.bindingNavigatorMLS.CountItem = this.bindingNavigatorCountItem;
			this.bindingNavigatorMLS.DeleteItem = this.bindingNavigatorDeleteItem;
			this.bindingNavigatorMLS.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bindingNavigatorMLS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
			this.bindingNavigatorMLS.Location = new System.Drawing.Point(0, 101);
			this.bindingNavigatorMLS.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bindingNavigatorMLS.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bindingNavigatorMLS.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bindingNavigatorMLS.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bindingNavigatorMLS.Name = "bindingNavigatorMLS";
			this.bindingNavigatorMLS.PositionItem = this.bindingNavigatorPositionItem;
			this.bindingNavigatorMLS.Size = new System.Drawing.Size(599, 25);
			this.bindingNavigatorMLS.TabIndex = 12;
			this.bindingNavigatorMLS.Text = "bindingNavigator1";
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
			// FormMultiLineStringDefinition
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(599, 368);
			this.Controls.Add(this.splitContainer1);
			this.Name = "FormMultiLineStringDefinition";
			this.TabText = "FormMultiLineStringDefinition";
			this.Text = "FormMultiLineStringDefinition";
			this.Load += new System.EventHandler(this.FormMultiLineStringDefinition_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgvMultiLineString)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.mapImageDefinition)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorMLS)).EndInit();
			this.bindingNavigatorMLS.ResumeLayout(false);
			this.bindingNavigatorMLS.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dgvMultiLineString;
		private System.Windows.Forms.Button btnPan;
		private System.Windows.Forms.Button btnInformation;
		private System.Windows.Forms.Button btnZoomIn;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.BindingNavigator bindingNavigatorMLS;
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
		private SharpMap.Forms.MapImage mapImageDefinition;
	}
}