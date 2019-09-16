namespace RoadCare3
{
	partial class FormMultiLineStringJoin
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMultiLineStringJoin));
			System.Drawing.Drawing2D.Matrix matrix1 = new System.Drawing.Drawing2D.Matrix();
			SharpMap.Map map2 = new SharpMap.Map();
			System.Drawing.Drawing2D.Matrix matrix2 = new System.Drawing.Drawing2D.Matrix();
			this.dgvMultiLineString = new System.Windows.Forms.DataGridView();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.dgvJoinedLineStrings = new System.Windows.Forms.DataGridView();
			this.FACILITY_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.LINESTRING = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.btnPan = new System.Windows.Forms.Button();
			this.btnInformation = new System.Windows.Forms.Button();
			this.btnZoomIn = new System.Windows.Forms.Button();
			this.mapImageTemp = new SharpMap.Forms.MapImage();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.buttonResultPan = new System.Windows.Forms.Button();
			this.buttonResultInfo = new System.Windows.Forms.Button();
			this.buttonResultZoom = new System.Windows.Forms.Button();
			this.bindingNavigatorMLS = new System.Windows.Forms.BindingNavigator(this.components);
			this.tsbJoin = new System.Windows.Forms.ToolStripButton();
			this.tsbUpdateJoinedLineStrings = new System.Windows.Forms.ToolStripButton();
			this.mapImageResults = new SharpMap.Forms.MapImage();
			((System.ComponentModel.ISupportInitialize)(this.dgvMultiLineString)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvJoinedLineStrings)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapImageTemp)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorMLS)).BeginInit();
			this.bindingNavigatorMLS.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapImageResults)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvMultiLineString
			// 
			this.dgvMultiLineString.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvMultiLineString.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvMultiLineString.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.dgvMultiLineString.Location = new System.Drawing.Point(0, 0);
			this.dgvMultiLineString.Name = "dgvMultiLineString";
			this.dgvMultiLineString.Size = new System.Drawing.Size(520, 159);
			this.dgvMultiLineString.TabIndex = 0;
			this.dgvMultiLineString.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvMultiLineString_MouseUp);
			this.dgvMultiLineString.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgvMultiLineString_KeyUp);
			// 
			// button1
			// 
			this.button1.Image = global::RoadCare3.Properties.Resources.pan_icon;
			this.button1.Location = new System.Drawing.Point(65, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(25, 23);
			this.button1.TabIndex = 9;
			this.button1.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this.button2.Image = global::RoadCare3.Properties.Resources.select;
			this.button2.Location = new System.Drawing.Point(3, 3);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(25, 23);
			this.button2.TabIndex = 8;
			this.button2.UseVisualStyleBackColor = true;
			// 
			// button3
			// 
			this.button3.Image = global::RoadCare3.Properties.Resources.zoomIn;
			this.button3.Location = new System.Drawing.Point(34, 3);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(25, 23);
			this.button3.TabIndex = 7;
			this.button3.UseVisualStyleBackColor = true;
			// 
			// dataGridView1
			// 
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView1.Location = new System.Drawing.Point(0, 0);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.Size = new System.Drawing.Size(532, 168);
			this.dataGridView1.TabIndex = 0;
			// 
			// dgvJoinedLineStrings
			// 
			this.dgvJoinedLineStrings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvJoinedLineStrings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FACILITY_ID,
            this.LINESTRING});
			this.dgvJoinedLineStrings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvJoinedLineStrings.Location = new System.Drawing.Point(0, 0);
			this.dgvJoinedLineStrings.Name = "dgvJoinedLineStrings";
			this.dgvJoinedLineStrings.Size = new System.Drawing.Size(542, 159);
			this.dgvJoinedLineStrings.TabIndex = 5;
			this.dgvJoinedLineStrings.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvJoinedLineStrings_MouseUp);
			this.dgvJoinedLineStrings.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgvJoinedLineStrings_KeyUp);
			// 
			// FACILITY_ID
			// 
			this.FACILITY_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.FACILITY_ID.HeaderText = "FACILITY_ID";
			this.FACILITY_ID.Name = "FACILITY_ID";
			this.FACILITY_ID.ReadOnly = true;
			// 
			// LINESTRING
			// 
			this.LINESTRING.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.LINESTRING.HeaderText = "LINESTRING";
			this.LINESTRING.MaxInputLength = 99999999;
			this.LINESTRING.Name = "LINESTRING";
			this.LINESTRING.ReadOnly = true;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.AutoScroll = true;
			this.splitContainer3.Panel1.Controls.Add(this.splitContainer1);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.AutoScroll = true;
			this.splitContainer3.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer3.Size = new System.Drawing.Size(1072, 452);
			this.splitContainer3.SplitterDistance = 520;
			this.splitContainer3.SplitterWidth = 10;
			this.splitContainer3.TabIndex = 7;
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
			this.splitContainer1.Panel1.Controls.Add(this.btnPan);
			this.splitContainer1.Panel1.Controls.Add(this.btnInformation);
			this.splitContainer1.Panel1.Controls.Add(this.btnZoomIn);
			this.splitContainer1.Panel1.Controls.Add(this.mapImageTemp);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.AutoScroll = true;
			this.splitContainer1.Panel2.Controls.Add(this.dgvMultiLineString);
			this.splitContainer1.Size = new System.Drawing.Size(520, 452);
			this.splitContainer1.SplitterDistance = 289;
			this.splitContainer1.TabIndex = 0;
			// 
			// btnPan
			// 
			this.btnPan.Image = global::RoadCare3.Properties.Resources.pan_icon;
			this.btnPan.Location = new System.Drawing.Point(65, 3);
			this.btnPan.Name = "btnPan";
			this.btnPan.Size = new System.Drawing.Size(25, 23);
			this.btnPan.TabIndex = 9;
			this.btnPan.UseVisualStyleBackColor = true;
			this.btnPan.Click += new System.EventHandler(this.btnPan_Click);
			// 
			// btnInformation
			// 
			this.btnInformation.Image = global::RoadCare3.Properties.Resources.select;
			this.btnInformation.Location = new System.Drawing.Point(3, 3);
			this.btnInformation.Name = "btnInformation";
			this.btnInformation.Size = new System.Drawing.Size(25, 23);
			this.btnInformation.TabIndex = 8;
			this.btnInformation.UseVisualStyleBackColor = true;
			this.btnInformation.Click += new System.EventHandler(this.btnInformation_Click);
			// 
			// btnZoomIn
			// 
			this.btnZoomIn.Image = global::RoadCare3.Properties.Resources.zoomIn;
			this.btnZoomIn.Location = new System.Drawing.Point(34, 3);
			this.btnZoomIn.Name = "btnZoomIn";
			this.btnZoomIn.Size = new System.Drawing.Size(25, 23);
			this.btnZoomIn.TabIndex = 7;
			this.btnZoomIn.UseVisualStyleBackColor = true;
			this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
			// 
			// mapImageTemp
			// 
			this.mapImageTemp.ActiveTool = SharpMap.Forms.MapImage.Tools.None;
			this.mapImageTemp.BackColor = System.Drawing.Color.White;
			this.mapImageTemp.Cursor = System.Windows.Forms.Cursors.Cross;
			this.mapImageTemp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mapImageTemp.Location = new System.Drawing.Point(0, 0);
			map1.BackColor = System.Drawing.Color.Transparent;
			map1.Center = ((SharpMap.Geometries.Point)(resources.GetObject("map1.Center")));
			map1.Layers = ((System.Collections.Generic.List<SharpMap.Layers.ILayer>)(resources.GetObject("map1.Layers")));
			map1.MapTransform = matrix1;
			map1.MaximumZoom = 1.7976931348623157E+308;
			map1.MinimumZoom = 0;
			map1.PixelAspectRatio = 1;
			map1.Size = new System.Drawing.Size(100, 50);
			map1.Zoom = 1;
			this.mapImageTemp.Map = map1;
			this.mapImageTemp.Name = "mapImageTemp";
			this.mapImageTemp.QueryLayerIndex = 0;
			this.mapImageTemp.Size = new System.Drawing.Size(520, 289);
			this.mapImageTemp.TabIndex = 10;
			this.mapImageTemp.TabStop = false;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.buttonResultPan);
			this.splitContainer2.Panel1.Controls.Add(this.buttonResultInfo);
			this.splitContainer2.Panel1.Controls.Add(this.buttonResultZoom);
			this.splitContainer2.Panel1.Controls.Add(this.mapImageResults);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.dgvJoinedLineStrings);
			this.splitContainer2.Size = new System.Drawing.Size(542, 452);
			this.splitContainer2.SplitterDistance = 289;
			this.splitContainer2.TabIndex = 0;
			// 
			// buttonResultPan
			// 
			this.buttonResultPan.Image = global::RoadCare3.Properties.Resources.pan_icon;
			this.buttonResultPan.Location = new System.Drawing.Point(65, 3);
			this.buttonResultPan.Name = "buttonResultPan";
			this.buttonResultPan.Size = new System.Drawing.Size(25, 23);
			this.buttonResultPan.TabIndex = 14;
			this.buttonResultPan.UseVisualStyleBackColor = true;
			this.buttonResultPan.Click += new System.EventHandler(this.button4_Click);
			// 
			// buttonResultInfo
			// 
			this.buttonResultInfo.Image = global::RoadCare3.Properties.Resources.select;
			this.buttonResultInfo.Location = new System.Drawing.Point(3, 3);
			this.buttonResultInfo.Name = "buttonResultInfo";
			this.buttonResultInfo.Size = new System.Drawing.Size(25, 23);
			this.buttonResultInfo.TabIndex = 13;
			this.buttonResultInfo.UseVisualStyleBackColor = true;
			this.buttonResultInfo.Click += new System.EventHandler(this.button5_Click);
			// 
			// buttonResultZoom
			// 
			this.buttonResultZoom.Image = global::RoadCare3.Properties.Resources.zoomIn;
			this.buttonResultZoom.Location = new System.Drawing.Point(34, 3);
			this.buttonResultZoom.Name = "buttonResultZoom";
			this.buttonResultZoom.Size = new System.Drawing.Size(25, 23);
			this.buttonResultZoom.TabIndex = 12;
			this.buttonResultZoom.UseVisualStyleBackColor = true;
			this.buttonResultZoom.Click += new System.EventHandler(this.button6_Click);
			// 
			// bindingNavigatorMLS
			// 
			this.bindingNavigatorMLS.AddNewItem = null;
			this.bindingNavigatorMLS.CountItem = null;
			this.bindingNavigatorMLS.DeleteItem = null;
			this.bindingNavigatorMLS.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bindingNavigatorMLS.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbJoin,
            this.tsbUpdateJoinedLineStrings});
			this.bindingNavigatorMLS.Location = new System.Drawing.Point(0, 455);
			this.bindingNavigatorMLS.MoveFirstItem = null;
			this.bindingNavigatorMLS.MoveLastItem = null;
			this.bindingNavigatorMLS.MoveNextItem = null;
			this.bindingNavigatorMLS.MovePreviousItem = null;
			this.bindingNavigatorMLS.Name = "bindingNavigatorMLS";
			this.bindingNavigatorMLS.PositionItem = null;
			this.bindingNavigatorMLS.Size = new System.Drawing.Size(1072, 25);
			this.bindingNavigatorMLS.TabIndex = 2;
			this.bindingNavigatorMLS.Text = "bindingNavigator1";
			// 
			// tsbJoin
			// 
			this.tsbJoin.Image = ((System.Drawing.Image)(resources.GetObject("tsbJoin.Image")));
			this.tsbJoin.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbJoin.Name = "tsbJoin";
			this.tsbJoin.Size = new System.Drawing.Size(90, 22);
			this.tsbJoin.Text = "Join Selected";
			this.tsbJoin.ToolTipText = "Join together selected  LINESTRING segments based on priority.";
			this.tsbJoin.Click += new System.EventHandler(this.tsbJoin_Click);
			// 
			// tsbUpdateJoinedLineStrings
			// 
			this.tsbUpdateJoinedLineStrings.Image = ((System.Drawing.Image)(resources.GetObject("tsbUpdateJoinedLineStrings.Image")));
			this.tsbUpdateJoinedLineStrings.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbUpdateJoinedLineStrings.Name = "tsbUpdateJoinedLineStrings";
			this.tsbUpdateJoinedLineStrings.Size = new System.Drawing.Size(62, 22);
			this.tsbUpdateJoinedLineStrings.Text = "Update";
			this.tsbUpdateJoinedLineStrings.ToolTipText = "Updates the NETWORK_DEFINITION table with the new linestrings in the right DataGr" +
				"idView.";
			this.tsbUpdateJoinedLineStrings.Click += new System.EventHandler(this.tsbUpdateJoinedLineStrings_Click);
			// 
			// mapImageResults
			// 
			this.mapImageResults.ActiveTool = SharpMap.Forms.MapImage.Tools.None;
			this.mapImageResults.BackColor = System.Drawing.Color.White;
			this.mapImageResults.Cursor = System.Windows.Forms.Cursors.Cross;
			this.mapImageResults.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mapImageResults.Location = new System.Drawing.Point(0, 0);
			map2.BackColor = System.Drawing.Color.Transparent;
			map2.Center = ((SharpMap.Geometries.Point)(resources.GetObject("map2.Center")));
			map2.Layers = ((System.Collections.Generic.List<SharpMap.Layers.ILayer>)(resources.GetObject("map2.Layers")));
			map2.MapTransform = matrix2;
			map2.MaximumZoom = 1.7976931348623157E+308;
			map2.MinimumZoom = 0;
			map2.PixelAspectRatio = 1;
			map2.Size = new System.Drawing.Size(100, 50);
			map2.Zoom = 1;
			this.mapImageResults.Map = map2;
			this.mapImageResults.Name = "mapImageResults";
			this.mapImageResults.QueryLayerIndex = 0;
			this.mapImageResults.Size = new System.Drawing.Size(542, 289);
			this.mapImageResults.TabIndex = 15;
			this.mapImageResults.TabStop = false;
			// 
			// FormMultiLineStringJoin
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1072, 480);
			this.Controls.Add(this.bindingNavigatorMLS);
			this.Controls.Add(this.splitContainer3);
			this.Name = "FormMultiLineStringJoin";
			this.TabText = "MultiLineString Definition";
			this.Text = "MultiLineString Definition";
			this.Load += new System.EventHandler(this.FormMultiLineStringDefinition_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgvMultiLineString)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvJoinedLineStrings)).EndInit();
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.mapImageTemp)).EndInit();
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigatorMLS)).EndInit();
			this.bindingNavigatorMLS.ResumeLayout(false);
			this.bindingNavigatorMLS.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapImageResults)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dgvMultiLineString;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridView dgvJoinedLineStrings;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button btnPan;
		private System.Windows.Forms.Button btnInformation;
		private System.Windows.Forms.Button btnZoomIn;
		private System.Windows.Forms.Button buttonResultPan;
		private System.Windows.Forms.Button buttonResultInfo;
		private System.Windows.Forms.Button buttonResultZoom;
		private System.Windows.Forms.DataGridViewTextBoxColumn FACILITY_ID;
		private System.Windows.Forms.DataGridViewTextBoxColumn LINESTRING;
		private System.Windows.Forms.BindingNavigator bindingNavigatorMLS;
		private System.Windows.Forms.ToolStripButton tsbJoin;
		private System.Windows.Forms.ToolStripButton tsbUpdateJoinedLineStrings;
		private SharpMap.Forms.MapImage mapImageTemp;
		private SharpMap.Forms.MapImage mapImageResults;
	}
}