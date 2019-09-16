namespace RoadCare3
{
	partial class AssetTab
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
			this.cbActivityDate = new System.Windows.Forms.ComboBox();
			this.dgvAssetSelection = new System.Windows.Forms.DataGridView();
			this.colAssetAttribute = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.lblDate = new System.Windows.Forms.Label();
			this.pictureBoxAsset = new XPicbox.XtendPicBox();
			this.tbShapeFilePath = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.cbSelectedAsset = new System.Windows.Forms.ComboBox();
			this.chkBoxShowAssetPicture = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.dgvAssetSelection)).BeginInit();
			this.SuspendLayout();
			// 
			// cbActivityDate
			// 
			this.cbActivityDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cbActivityDate.FormattingEnabled = true;
			this.cbActivityDate.Location = new System.Drawing.Point(61, 270);
			this.cbActivityDate.Name = "cbActivityDate";
			this.cbActivityDate.Size = new System.Drawing.Size(312, 21);
			this.cbActivityDate.TabIndex = 20;
			this.cbActivityDate.SelectedIndexChanged += new System.EventHandler(this.cbActivityDate_SelectedIndexChanged);
			// 
			// dgvAssetSelection
			// 
			this.dgvAssetSelection.AllowUserToAddRows = false;
			this.dgvAssetSelection.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.dgvAssetSelection.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAssetSelection.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAssetAttribute,
            this.colValue});
			this.dgvAssetSelection.Location = new System.Drawing.Point(3, 178);
			this.dgvAssetSelection.Name = "dgvAssetSelection";
			this.dgvAssetSelection.RowHeadersVisible = false;
			this.dgvAssetSelection.Size = new System.Drawing.Size(370, 588);
			this.dgvAssetSelection.TabIndex = 14;
			this.dgvAssetSelection.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvAssetSelection_CellBeginEdit);
			this.dgvAssetSelection.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAssetSelection_CellEndEdit);
			// 
			// colAssetAttribute
			// 
			this.colAssetAttribute.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colAssetAttribute.HeaderText = "Attribute";
			this.colAssetAttribute.Name = "colAssetAttribute";
			// 
			// colValue
			// 
			this.colValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colValue.HeaderText = "Value";
			this.colValue.Name = "colValue";
			// 
			// lblDate
			// 
			this.lblDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblDate.AutoSize = true;
			this.lblDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDate.Location = new System.Drawing.Point(0, 271);
			this.lblDate.Name = "lblDate";
			this.lblDate.Size = new System.Drawing.Size(64, 16);
			this.lblDate.TabIndex = 19;
			this.lblDate.Text = "Activities:";
			// 
			// pictureBoxAsset
			// 
			this.pictureBoxAsset.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.pictureBoxAsset.AutoScroll = true;
			this.pictureBoxAsset.BackColor = System.Drawing.Color.White;
			this.pictureBoxAsset.Location = new System.Drawing.Point(3, 5);
			this.pictureBoxAsset.Name = "pictureBoxAsset";
			this.pictureBoxAsset.PictureFile = "";
			this.pictureBoxAsset.Size = new System.Drawing.Size(370, 167);
			this.pictureBoxAsset.TabIndex = 18;
			// 
			// tbShapeFilePath
			// 
			this.tbShapeFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbShapeFilePath.Location = new System.Drawing.Point(3, 30);
			this.tbShapeFilePath.Name = "tbShapeFilePath";
			this.tbShapeFilePath.ReadOnly = true;
			this.tbShapeFilePath.Size = new System.Drawing.Size(336, 20);
			this.tbShapeFilePath.TabIndex = 16;
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowse.Location = new System.Drawing.Point(345, 30);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(28, 21);
			this.btnBrowse.TabIndex = 17;
			this.btnBrowse.Text = "...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// cbSelectedAsset
			// 
			this.cbSelectedAsset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cbSelectedAsset.FormattingEnabled = true;
			this.cbSelectedAsset.Location = new System.Drawing.Point(3, 3);
			this.cbSelectedAsset.Name = "cbSelectedAsset";
			this.cbSelectedAsset.Size = new System.Drawing.Size(370, 21);
			this.cbSelectedAsset.TabIndex = 13;
			this.cbSelectedAsset.SelectedIndexChanged += new System.EventHandler(this.cbSelectedAsset_SelectedIndexChanged);
			// 
			// chkBoxShowAssetPicture
			// 
			this.chkBoxShowAssetPicture.AutoSize = true;
			this.chkBoxShowAssetPicture.Location = new System.Drawing.Point(3, 59);
			this.chkBoxShowAssetPicture.Name = "chkBoxShowAssetPicture";
			this.chkBoxShowAssetPicture.Size = new System.Drawing.Size(89, 17);
			this.chkBoxShowAssetPicture.TabIndex = 15;
			this.chkBoxShowAssetPicture.Text = "Show Picture";
			this.chkBoxShowAssetPicture.UseVisualStyleBackColor = true;
			this.chkBoxShowAssetPicture.CheckedChanged += new System.EventHandler(this.chkBoxShowAssetPicture_CheckedChanged);
			// 
			// AssetTab
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(376, 778);
			this.Controls.Add(this.cbActivityDate);
			this.Controls.Add(this.dgvAssetSelection);
			this.Controls.Add(this.lblDate);
			this.Controls.Add(this.pictureBoxAsset);
			this.Controls.Add(this.tbShapeFilePath);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.cbSelectedAsset);
			this.Controls.Add(this.chkBoxShowAssetPicture);
			this.Name = "AssetTab";
			this.TabText = "Asset Manager";
			this.Text = "Asset Manager";
			this.SizeChanged += new System.EventHandler(this.AssetTab_SizeChanged);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AssetTab_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.dgvAssetSelection)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cbActivityDate;
		private System.Windows.Forms.DataGridView dgvAssetSelection;
		private System.Windows.Forms.DataGridViewTextBoxColumn colAssetAttribute;
		private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
		private System.Windows.Forms.Label lblDate;
		private XPicbox.XtendPicBox pictureBoxAsset;
		private System.Windows.Forms.TextBox tbShapeFilePath;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.ComboBox cbSelectedAsset;
		private System.Windows.Forms.CheckBox chkBoxShowAssetPicture;
	}
}