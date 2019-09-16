namespace RoadCare3
{
    partial class FormImportGeometries
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
            this.dgvImportGeometries = new System.Windows.Forms.DataGridView();
            this.colRoute = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDirection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMilepost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLong = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGeo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblImportGeometriesTitle = new System.Windows.Forms.Label();
            this.ttCreateGeoms = new System.Windows.Forms.ToolTip(this.components);
            this.btnCreateGeometries = new System.Windows.Forms.Button();
            this.checkBoxOverwriteGeometries = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvImportGeometries)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvImportGeometries
            // 
            this.dgvImportGeometries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvImportGeometries.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvImportGeometries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvImportGeometries.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRoute,
            this.colDirection,
            this.colMilepost,
            this.colLat,
            this.colLong,
            this.colSection,
            this.colGeo});
            this.dgvImportGeometries.Location = new System.Drawing.Point(0, 40);
            this.dgvImportGeometries.Margin = new System.Windows.Forms.Padding(2);
            this.dgvImportGeometries.Name = "dgvImportGeometries";
            this.dgvImportGeometries.RowTemplate.Height = 24;
            this.dgvImportGeometries.Size = new System.Drawing.Size(805, 349);
            this.dgvImportGeometries.TabIndex = 1;
            this.dgvImportGeometries.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvImportGeometries_KeyDown);
            // 
            // colRoute
            // 
            this.colRoute.HeaderText = "ROUTE";
            this.colRoute.Name = "colRoute";
            this.colRoute.Visible = false;
            // 
            // colDirection
            // 
            this.colDirection.HeaderText = "DIRECTION";
            this.colDirection.Name = "colDirection";
            this.colDirection.Visible = false;
            // 
            // colMilepost
            // 
            this.colMilepost.HeaderText = "MILEPOST";
            this.colMilepost.Name = "colMilepost";
            this.colMilepost.Visible = false;
            // 
            // colLat
            // 
            this.colLat.HeaderText = "LATITUDE";
            this.colLat.Name = "colLat";
            this.colLat.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colLat.Visible = false;
            // 
            // colLong
            // 
            this.colLong.HeaderText = "LONGITUDE";
            this.colLong.Name = "colLong";
            this.colLong.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colLong.Visible = false;
            // 
            // colSection
            // 
            this.colSection.HeaderText = "SECTION";
            this.colSection.Name = "colSection";
            this.colSection.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colSection.Visible = false;
            // 
            // colGeo
            // 
            this.colGeo.HeaderText = "GEOMETRY";
            this.colGeo.Name = "colGeo";
            this.colGeo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colGeo.Visible = false;
            // 
            // lblImportGeometriesTitle
            // 
            this.lblImportGeometriesTitle.AutoSize = true;
            this.lblImportGeometriesTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImportGeometriesTitle.Location = new System.Drawing.Point(51, 9);
            this.lblImportGeometriesTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblImportGeometriesTitle.Name = "lblImportGeometriesTitle";
            this.lblImportGeometriesTitle.Size = new System.Drawing.Size(228, 26);
            this.lblImportGeometriesTitle.TabIndex = 4;
            this.lblImportGeometriesTitle.Text = "Geometry Data Import";
            // 
            // btnCreateGeometries
            // 
            this.btnCreateGeometries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateGeometries.Location = new System.Drawing.Point(699, 9);
            this.btnCreateGeometries.Name = "btnCreateGeometries";
            this.btnCreateGeometries.Size = new System.Drawing.Size(106, 26);
            this.btnCreateGeometries.TabIndex = 5;
            this.btnCreateGeometries.Text = "Create Geometries";
            this.ttCreateGeoms.SetToolTip(this.btnCreateGeometries, "Create geometries for road data in network definition table.");
            this.btnCreateGeometries.UseVisualStyleBackColor = true;
            this.btnCreateGeometries.Click += new System.EventHandler(this.btnCreateGeometries_Click);
            // 
            // checkBoxOverwriteGeometries
            // 
            this.checkBoxOverwriteGeometries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxOverwriteGeometries.AutoSize = true;
            this.checkBoxOverwriteGeometries.Location = new System.Drawing.Point(530, 15);
            this.checkBoxOverwriteGeometries.Name = "checkBoxOverwriteGeometries";
            this.checkBoxOverwriteGeometries.Size = new System.Drawing.Size(163, 17);
            this.checkBoxOverwriteGeometries.TabIndex = 6;
            this.checkBoxOverwriteGeometries.Text = "Overwrite existing geometries";
            this.checkBoxOverwriteGeometries.UseVisualStyleBackColor = true;
            // 
            // FormImportGeometries
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 385);
            this.Controls.Add(this.checkBoxOverwriteGeometries);
            this.Controls.Add(this.btnCreateGeometries);
            this.Controls.Add(this.lblImportGeometriesTitle);
            this.Controls.Add(this.dgvImportGeometries);
            this.Name = "FormImportGeometries";
            this.TabText = "Import Geometries";
            this.Text = "Import Geometries";
            this.Load += new System.EventHandler(this.FormImportGeometries_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormImportGeometries_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvImportGeometries)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvImportGeometries;
        private System.Windows.Forms.Label lblImportGeometriesTitle;
        private System.Windows.Forms.ToolTip ttCreateGeoms;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRoute;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDirection;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMilepost;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLat;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLong;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSection;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGeo;
        private System.Windows.Forms.Button btnCreateGeometries;
        private System.Windows.Forms.CheckBox checkBoxOverwriteGeometries;

    }
}