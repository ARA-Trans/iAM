namespace RoadCare3
{
    partial class FormGISLayerManager
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGISLayerManager));
            this.dgvLegend = new System.Windows.Forms.DataGridView();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColorPicker = new System.Windows.Forms.DataGridViewButtonColumn();
            this.cbAttribute = new System.Windows.Forms.ComboBox();
            this.cbYear = new System.Windows.Forms.ComboBox();
            this.tvLayers = new System.Windows.Forms.TreeView();
            this.imageListSymbols = new System.Windows.Forms.ImageList(this.components);
            this.cbAssets = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveLayerUp = new System.Windows.Forms.Button();
            this.btnProperties = new System.Windows.Forms.Button();
            this.btnRemoveLayer = new System.Windows.Forms.Button();
            this.btnAddLayer = new System.Windows.Forms.Button();
            this.buttonAddAttribute = new System.Windows.Forms.Button();
            this.cbAssetProperty = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxNumberLevels = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLegend)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLegend
            // 
            this.dgvLegend.AllowUserToAddRows = false;
            this.dgvLegend.AllowUserToDeleteRows = false;
            this.dgvLegend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLegend.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvLegend.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLegend.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colValue,
            this.colColor,
            this.colColorPicker});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLegend.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvLegend.Location = new System.Drawing.Point(3, 360);
            this.dgvLegend.Name = "dgvLegend";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLegend.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvLegend.RowHeadersVisible = false;
            this.dgvLegend.Size = new System.Drawing.Size(191, 176);
            this.dgvLegend.TabIndex = 12;
            this.dgvLegend.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLegend_CellEndEdit);
            this.dgvLegend.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLevelDefinitions_CellClick);
            // 
            // colValue
            // 
            this.colValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            this.colValue.HeaderText = "";
            this.colValue.Name = "colValue";
            this.colValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colValue.Width = 5;
            // 
            // colColor
            // 
            this.colColor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colColor.HeaderText = "Color";
            this.colColor.Name = "colColor";
            this.colColor.ReadOnly = true;
            this.colColor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colColorPicker
            // 
            this.colColorPicker.HeaderText = "";
            this.colColorPicker.Name = "colColorPicker";
            this.colColorPicker.ReadOnly = true;
            this.colColorPicker.Width = 25;
            // 
            // cbAttribute
            // 
            this.cbAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAttribute.FormattingEnabled = true;
            this.cbAttribute.Location = new System.Drawing.Point(75, 90);
            this.cbAttribute.Name = "cbAttribute";
            this.cbAttribute.Size = new System.Drawing.Size(119, 21);
            this.cbAttribute.TabIndex = 13;
            this.cbAttribute.SelectedIndexChanged += new System.EventHandler(this.cbAttribute_SelectedIndexChanged);
            // 
            // cbYear
            // 
            this.cbYear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbYear.FormattingEnabled = true;
            this.cbYear.Location = new System.Drawing.Point(75, 117);
            this.cbYear.Name = "cbYear";
            this.cbYear.Size = new System.Drawing.Size(119, 21);
            this.cbYear.TabIndex = 14;
            this.cbYear.SelectedIndexChanged += new System.EventHandler(this.cbYear_SelectedIndexChanged);
            // 
            // tvLayers
            // 
            this.tvLayers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvLayers.CheckBoxes = true;
            this.tvLayers.HideSelection = false;
            this.tvLayers.ImageIndex = 0;
            this.tvLayers.ImageList = this.imageListSymbols;
            this.tvLayers.Location = new System.Drawing.Point(3, 144);
            this.tvLayers.Name = "tvLayers";
            this.tvLayers.SelectedImageIndex = 0;
            this.tvLayers.Size = new System.Drawing.Size(191, 183);
            this.tvLayers.TabIndex = 15;
            this.tvLayers.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvLayers_AfterCheck);
            // 
            // imageListSymbols
            // 
            this.imageListSymbols.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListSymbols.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListSymbols.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // cbAssets
            // 
            this.cbAssets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAssets.FormattingEnabled = true;
            this.cbAssets.Location = new System.Drawing.Point(75, 34);
            this.cbAssets.Name = "cbAssets";
            this.cbAssets.Size = new System.Drawing.Size(119, 21);
            this.cbAssets.TabIndex = 16;
            this.cbAssets.SelectedIndexChanged += new System.EventHandler(this.cbAssets_SelectedIndexChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(3, 34);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(66, 44);
            this.btnAdd.TabIndex = 17;
            this.btnAdd.Text = "Add Asset";
            this.toolTip1.SetToolTip(this.btnAdd, "Add Layer from NETWORK_DEFINITION table");
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Image = global::RoadCare3.Properties.Resources.ARW04DN;
            this.btnMoveDown.Location = new System.Drawing.Point(169, 8);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(25, 22);
            this.btnMoveDown.TabIndex = 11;
            this.toolTip1.SetToolTip(this.btnMoveDown, "Move Layer Down");
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveLayerUp
            // 
            this.btnMoveLayerUp.Image = global::RoadCare3.Properties.Resources.ARW04UP;
            this.btnMoveLayerUp.Location = new System.Drawing.Point(138, 8);
            this.btnMoveLayerUp.Name = "btnMoveLayerUp";
            this.btnMoveLayerUp.Size = new System.Drawing.Size(25, 22);
            this.btnMoveLayerUp.TabIndex = 10;
            this.toolTip1.SetToolTip(this.btnMoveLayerUp, "Move Layer Up");
            this.btnMoveLayerUp.UseVisualStyleBackColor = true;
            this.btnMoveLayerUp.Click += new System.EventHandler(this.btnMoveLayerUp_Click);
            // 
            // btnProperties
            // 
            this.btnProperties.Image = ((System.Drawing.Image)(resources.GetObject("btnProperties.Image")));
            this.btnProperties.Location = new System.Drawing.Point(63, 6);
            this.btnProperties.Name = "btnProperties";
            this.btnProperties.Size = new System.Drawing.Size(25, 22);
            this.btnProperties.TabIndex = 9;
            this.toolTip1.SetToolTip(this.btnProperties, "Layer Properties");
            this.btnProperties.UseVisualStyleBackColor = true;
            this.btnProperties.Click += new System.EventHandler(this.btnProperties_Click);
            // 
            // btnRemoveLayer
            // 
            this.btnRemoveLayer.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveLayer.Image")));
            this.btnRemoveLayer.Location = new System.Drawing.Point(33, 6);
            this.btnRemoveLayer.Name = "btnRemoveLayer";
            this.btnRemoveLayer.Size = new System.Drawing.Size(25, 22);
            this.btnRemoveLayer.TabIndex = 8;
            this.toolTip1.SetToolTip(this.btnRemoveLayer, "Remove Layer");
            this.btnRemoveLayer.UseVisualStyleBackColor = true;
            this.btnRemoveLayer.Click += new System.EventHandler(this.btnRemoveLayer_Click);
            // 
            // btnAddLayer
            // 
            this.btnAddLayer.Image = global::RoadCare3.Properties.Resources.addLayer;
            this.btnAddLayer.Location = new System.Drawing.Point(3, 6);
            this.btnAddLayer.Name = "btnAddLayer";
            this.btnAddLayer.Size = new System.Drawing.Size(25, 22);
            this.btnAddLayer.TabIndex = 7;
            this.toolTip1.SetToolTip(this.btnAddLayer, "Add Layer");
            this.btnAddLayer.UseVisualStyleBackColor = true;
            this.btnAddLayer.Click += new System.EventHandler(this.btnAddLayer_Click);
            // 
            // buttonAddAttribute
            // 
            this.buttonAddAttribute.Location = new System.Drawing.Point(3, 90);
            this.buttonAddAttribute.Name = "buttonAddAttribute";
            this.buttonAddAttribute.Size = new System.Drawing.Size(66, 48);
            this.buttonAddAttribute.TabIndex = 19;
            this.buttonAddAttribute.Text = "Add Attribute";
            this.toolTip1.SetToolTip(this.buttonAddAttribute, "Add Layer from NETWORK_DEFINITION table");
            this.buttonAddAttribute.UseVisualStyleBackColor = true;
            this.buttonAddAttribute.Click += new System.EventHandler(this.buttonAddAttribute_Click);
            // 
            // cbAssetProperty
            // 
            this.cbAssetProperty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAssetProperty.FormattingEnabled = true;
            this.cbAssetProperty.Location = new System.Drawing.Point(75, 57);
            this.cbAssetProperty.Name = "cbAssetProperty";
            this.cbAssetProperty.Size = new System.Drawing.Size(119, 21);
            this.cbAssetProperty.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 336);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Number of Levels:";
            // 
            // comboBoxNumberLevels
            // 
            this.comboBoxNumberLevels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxNumberLevels.FormattingEnabled = true;
            this.comboBoxNumberLevels.Location = new System.Drawing.Point(102, 333);
            this.comboBoxNumberLevels.Name = "comboBoxNumberLevels";
            this.comboBoxNumberLevels.Size = new System.Drawing.Size(92, 21);
            this.comboBoxNumberLevels.TabIndex = 21;
            // 
            // FormGISLayerManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(197, 537);
            this.Controls.Add(this.comboBoxNumberLevels);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonAddAttribute);
            this.Controls.Add(this.cbAssetProperty);
            this.Controls.Add(this.tvLayers);
            this.Controls.Add(this.cbYear);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.cbAttribute);
            this.Controls.Add(this.cbAssets);
            this.Controls.Add(this.dgvLegend);
            this.Controls.Add(this.btnMoveDown);
            this.Controls.Add(this.btnMoveLayerUp);
            this.Controls.Add(this.btnProperties);
            this.Controls.Add(this.btnRemoveLayer);
            this.Controls.Add(this.btnAddLayer);
            this.HideOnClose = true;
            this.Name = "FormGISLayerManager";
            this.TabText = "Layer Manager";
            this.Text = "Layer Manager";
            this.Load += new System.EventHandler(this.FormGISLayerManager_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormGISLayerManager_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLegend)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveLayerUp;
        private System.Windows.Forms.Button btnProperties;
        private System.Windows.Forms.Button btnRemoveLayer;
        private System.Windows.Forms.Button btnAddLayer;
        private System.Windows.Forms.DataGridView dgvLegend;
        private System.Windows.Forms.ComboBox cbAttribute;
        private System.Windows.Forms.ComboBox cbYear;
        private System.Windows.Forms.TreeView tvLayers;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colColor;
        private System.Windows.Forms.DataGridViewButtonColumn colColorPicker;
        private System.Windows.Forms.ComboBox cbAssets;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.ImageList imageListSymbols;
		private System.Windows.Forms.ComboBox cbAssetProperty;
		private System.Windows.Forms.Button buttonAddAttribute;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxNumberLevels;

    }
}