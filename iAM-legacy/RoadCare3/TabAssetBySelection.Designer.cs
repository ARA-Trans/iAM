namespace RoadCare3
{
	partial class TabAssetBySelection
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
			this.dgvAssetsBySelection = new System.Windows.Forms.DataGridView();
			this.GEO_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SECTIONID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ASSET_TYPE = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FACILITY = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SECTION = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.BEGIN_STATION = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.END_STATION = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dgvAssetsBySelection)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvAssetsBySelection
			// 
			this.dgvAssetsBySelection.AllowUserToAddRows = false;
			this.dgvAssetsBySelection.AllowUserToDeleteRows = false;
			this.dgvAssetsBySelection.AllowUserToOrderColumns = true;
			this.dgvAssetsBySelection.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvAssetsBySelection.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAssetsBySelection.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.GEO_ID,
            this.SECTIONID,
            this.ASSET_TYPE,
            this.FACILITY,
            this.SECTION,
            this.BEGIN_STATION,
            this.END_STATION});
			this.dgvAssetsBySelection.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvAssetsBySelection.Location = new System.Drawing.Point(0, 0);
			this.dgvAssetsBySelection.MultiSelect = false;
			this.dgvAssetsBySelection.Name = "dgvAssetsBySelection";
			this.dgvAssetsBySelection.ReadOnly = true;
			this.dgvAssetsBySelection.RowHeadersVisible = false;
			this.dgvAssetsBySelection.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvAssetsBySelection.Size = new System.Drawing.Size(481, 422);
			this.dgvAssetsBySelection.TabIndex = 0;
			this.dgvAssetsBySelection.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvAssetsBySelection_CellMouseClick);
			this.dgvAssetsBySelection.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAssetsBySelection_RowEnter);
			this.dgvAssetsBySelection.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgvAssetsBySelection_KeyUp);
			// 
			// GEO_ID
			// 
			this.GEO_ID.HeaderText = "GEO_ID";
			this.GEO_ID.Name = "GEO_ID";
			this.GEO_ID.ReadOnly = true;
			this.GEO_ID.Visible = false;
			// 
			// SECTIONID
			// 
			this.SECTIONID.HeaderText = "SECTIONID";
			this.SECTIONID.Name = "SECTIONID";
			this.SECTIONID.ReadOnly = true;
			this.SECTIONID.Visible = false;
			// 
			// ASSET_TYPE
			// 
			this.ASSET_TYPE.HeaderText = "Asset Type";
			this.ASSET_TYPE.Name = "ASSET_TYPE";
			this.ASSET_TYPE.ReadOnly = true;
			// 
			// FACILITY
			// 
			this.FACILITY.HeaderText = "Facility";
			this.FACILITY.Name = "FACILITY";
			this.FACILITY.ReadOnly = true;
			// 
			// SECTION
			// 
			this.SECTION.HeaderText = "Section";
			this.SECTION.Name = "SECTION";
			this.SECTION.ReadOnly = true;
			// 
			// BEGIN_STATION
			// 
			this.BEGIN_STATION.HeaderText = "Begin Station";
			this.BEGIN_STATION.Name = "BEGIN_STATION";
			this.BEGIN_STATION.ReadOnly = true;
			// 
			// END_STATION
			// 
			this.END_STATION.HeaderText = "End Station";
			this.END_STATION.Name = "END_STATION";
			this.END_STATION.ReadOnly = true;
			// 
			// TabAssetBySelection
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(481, 422);
			this.Controls.Add(this.dgvAssetsBySelection);
			this.Name = "TabAssetBySelection";
			this.TabText = "Assets By Selection";
			this.Text = "Assets By Selection";
			this.Load += new System.EventHandler(this.TabAssetBySelection_Load);
			this.Enter += new System.EventHandler(this.TabAssetBySelection_Enter);
			((System.ComponentModel.ISupportInitialize)(this.dgvAssetsBySelection)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dgvAssetsBySelection;
		private System.Windows.Forms.DataGridViewTextBoxColumn GEO_ID;
		private System.Windows.Forms.DataGridViewTextBoxColumn SECTIONID;
		private System.Windows.Forms.DataGridViewTextBoxColumn ASSET_TYPE;
		private System.Windows.Forms.DataGridViewTextBoxColumn FACILITY;
		private System.Windows.Forms.DataGridViewTextBoxColumn SECTION;
		private System.Windows.Forms.DataGridViewTextBoxColumn BEGIN_STATION;
		private System.Windows.Forms.DataGridViewTextBoxColumn END_STATION;

	}
}