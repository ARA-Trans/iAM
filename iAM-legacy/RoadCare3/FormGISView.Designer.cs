namespace RoadCare3
{
    partial class FormGISView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGISView));
			System.Drawing.Drawing2D.Matrix matrix1 = new System.Drawing.Drawing2D.Matrix();
			this.btnAdvancedSearch = new System.Windows.Forms.Button();
			this.tbAdvancedSearch = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnPan = new System.Windows.Forms.Button();
			this.btnInformation = new System.Windows.Forms.Button();
			this.btnZoomOut = new System.Windows.Forms.Button();
			this.btnZoomIn = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.MainMapImage = new SharpMap.Forms.MapImage();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripLabelGISView = new System.Windows.Forms.ToolStripLabel();
			this.toolStripComboBoxSimulations = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripButtonManager = new System.Windows.Forms.ToolStripButton();
			this.imageListDefaultSymbols = new System.Windows.Forms.ImageList(this.components);
			((System.ComponentModel.ISupportInitialize)(this.MainMapImage)).BeginInit();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnAdvancedSearch
			// 
			this.btnAdvancedSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAdvancedSearch.Location = new System.Drawing.Point(587, 4);
			this.btnAdvancedSearch.Name = "btnAdvancedSearch";
			this.btnAdvancedSearch.Size = new System.Drawing.Size(28, 21);
			this.btnAdvancedSearch.TabIndex = 9;
			this.btnAdvancedSearch.Text = "...";
			this.btnAdvancedSearch.UseVisualStyleBackColor = true;
			this.btnAdvancedSearch.Click += new System.EventHandler(this.btnAdvancedSearch_Click);
			// 
			// tbAdvancedSearch
			// 
			this.tbAdvancedSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbAdvancedSearch.Location = new System.Drawing.Point(265, 5);
			this.tbAdvancedSearch.Name = "tbAdvancedSearch";
			this.tbAdvancedSearch.ReadOnly = true;
			this.tbAdvancedSearch.Size = new System.Drawing.Size(321, 20);
			this.tbAdvancedSearch.TabIndex = 8;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(169, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "Advanced Search:";
			// 
			// btnPan
			// 
			this.btnPan.Image = global::RoadCare3.Properties.Resources.pan_icon;
			this.btnPan.Location = new System.Drawing.Point(128, 2);
			this.btnPan.Name = "btnPan";
			this.btnPan.Size = new System.Drawing.Size(25, 23);
			this.btnPan.TabIndex = 6;
			this.btnPan.UseVisualStyleBackColor = true;
			this.btnPan.Click += new System.EventHandler(this.btnPan_Click);
			// 
			// btnInformation
			// 
			this.btnInformation.Image = global::RoadCare3.Properties.Resources.select;
			this.btnInformation.Location = new System.Drawing.Point(35, 2);
			this.btnInformation.Name = "btnInformation";
			this.btnInformation.Size = new System.Drawing.Size(25, 23);
			this.btnInformation.TabIndex = 5;
			this.btnInformation.UseVisualStyleBackColor = true;
			this.btnInformation.Click += new System.EventHandler(this.btnInformation_Click);
			// 
			// btnZoomOut
			// 
			this.btnZoomOut.Image = global::RoadCare3.Properties.Resources.zoomOut;
			this.btnZoomOut.Location = new System.Drawing.Point(97, 2);
			this.btnZoomOut.Name = "btnZoomOut";
			this.btnZoomOut.Size = new System.Drawing.Size(25, 23);
			this.btnZoomOut.TabIndex = 4;
			this.btnZoomOut.UseVisualStyleBackColor = true;
			this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
			// 
			// btnZoomIn
			// 
			this.btnZoomIn.Image = global::RoadCare3.Properties.Resources.zoomIn;
			this.btnZoomIn.Location = new System.Drawing.Point(66, 2);
			this.btnZoomIn.Name = "btnZoomIn";
			this.btnZoomIn.Size = new System.Drawing.Size(25, 23);
			this.btnZoomIn.TabIndex = 3;
			this.btnZoomIn.UseVisualStyleBackColor = true;
			this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
			// 
			// button1
			// 
			this.button1.Image = global::RoadCare3.Properties.Resources.routeSmall;
			this.button1.Location = new System.Drawing.Point(4, 2);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(25, 23);
			this.button1.TabIndex = 1;
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// MainMapImage
			// 
			this.MainMapImage.ActiveTool = SharpMap.Forms.MapImage.Tools.None;
			this.MainMapImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.MainMapImage.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.MainMapImage.Cursor = System.Windows.Forms.Cursors.Cross;
			this.MainMapImage.Location = new System.Drawing.Point(0, 31);
			map1.BackColor = System.Drawing.Color.Transparent;
			map1.Center = ((SharpMap.Geometries.Point)(resources.GetObject("map1.Center")));
			map1.Layers = ((System.Collections.Generic.List<SharpMap.Layers.ILayer>)(resources.GetObject("map1.Layers")));
			map1.MapTransform = matrix1;
			map1.MaximumZoom = 1.7976931348623157E+308;
			map1.MinimumZoom = 0;
			map1.PixelAspectRatio = 1;
			map1.Size = new System.Drawing.Size(100, 50);
			map1.Zoom = 1;
			this.MainMapImage.Map = map1;
			this.MainMapImage.Name = "MainMapImage";
			this.MainMapImage.QueryLayerIndex = 0;
			this.MainMapImage.Size = new System.Drawing.Size(619, 270);
			this.MainMapImage.TabIndex = 11;
			this.MainMapImage.TabStop = false;
			this.MainMapImage.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MainMapImage_MouseDoubleClick);
			this.MainMapImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainMapImage_MouseClick);
			this.MainMapImage.MouseDrag += new SharpMap.Forms.MapImage.MouseEventHandler(this.MainMapImage_MouseDrag);
			this.MainMapImage.Click += new System.EventHandler(this.MainMapImage_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabelGISView,
            this.toolStripComboBoxSimulations,
            this.toolStripButtonManager});
			this.toolStrip1.Location = new System.Drawing.Point(0, 276);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip1.Size = new System.Drawing.Size(618, 25);
			this.toolStrip1.TabIndex = 12;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripLabelGISView
			// 
			this.toolStripLabelGISView.Name = "toolStripLabelGISView";
			this.toolStripLabelGISView.Size = new System.Drawing.Size(67, 22);
			this.toolStripLabelGISView.Text = "Simulation:";
			// 
			// toolStripComboBoxSimulations
			// 
			this.toolStripComboBoxSimulations.Name = "toolStripComboBoxSimulations";
			this.toolStripComboBoxSimulations.Size = new System.Drawing.Size(121, 25);
			this.toolStripComboBoxSimulations.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxSimulations_SelectedIndexChanged);
			// 
			// toolStripButtonManager
			// 
			this.toolStripButtonManager.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButtonManager.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonManager.Image")));
			this.toolStripButtonManager.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonManager.Name = "toolStripButtonManager";
			this.toolStripButtonManager.Size = new System.Drawing.Size(78, 22);
			this.toolStripButtonManager.Text = "GIS Manager";
			this.toolStripButtonManager.Click += new System.EventHandler(this.toolStripButtonManager_Click);
			// 
			// imageListDefaultSymbols
			// 
			this.imageListDefaultSymbols.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListDefaultSymbols.ImageStream")));
			this.imageListDefaultSymbols.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListDefaultSymbols.Images.SetKeyName(0, "redDiamond.png");
			this.imageListDefaultSymbols.Images.SetKeyName(1, "circle.png");
			this.imageListDefaultSymbols.Images.SetKeyName(2, "sign1.png");
			// 
			// FormGISView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(618, 301);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.MainMapImage);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnAdvancedSearch);
			this.Controls.Add(this.tbAdvancedSearch);
			this.Controls.Add(this.btnPan);
			this.Controls.Add(this.btnInformation);
			this.Controls.Add(this.btnZoomOut);
			this.Controls.Add(this.btnZoomIn);
			this.Controls.Add(this.button1);
			this.Name = "FormGISView";
			this.TabText = "FormGISView";
			this.Text = "FormGISView";
			this.Load += new System.EventHandler(this.FormGISView_Load);
			this.Activated += new System.EventHandler(this.FormGISView_Activated);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormGISView_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.MainMapImage)).EndInit();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }
		
		#endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnInformation;
        private System.Windows.Forms.Button btnPan;
        private System.Windows.Forms.Button btnAdvancedSearch;
        private System.Windows.Forms.TextBox tbAdvancedSearch;
		private System.Windows.Forms.Label label1;
		private SharpMap.Forms.MapImage MainMapImage;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripLabel toolStripLabelGISView;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBoxSimulations;
        private System.Windows.Forms.ImageList imageListDefaultSymbols;
        private System.Windows.Forms.ToolStripButton toolStripButtonManager;
    }
}