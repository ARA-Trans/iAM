namespace RoadCare3
{
    partial class FormImportShapeFile
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
			this.tbShapeFileLocation = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lbColumns = new System.Windows.Forms.ListBox();
			this.tbSelect = new System.Windows.Forms.TextBox();
			this.lblShapeFile = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lblProgress = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tbShapeFileLocation
			// 
			this.tbShapeFileLocation.Location = new System.Drawing.Point(10, 29);
			this.tbShapeFileLocation.Name = "tbShapeFileLocation";
			this.tbShapeFileLocation.ReadOnly = true;
			this.tbShapeFileLocation.Size = new System.Drawing.Size(375, 20);
			this.tbShapeFileLocation.TabIndex = 1;
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(391, 29);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(24, 20);
			this.btnBrowse.TabIndex = 2;
			this.btnBrowse.Text = "...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(7, 412);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 4;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(88, 412);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// lbColumns
			// 
			this.lbColumns.FormattingEnabled = true;
			this.lbColumns.Location = new System.Drawing.Point(10, 79);
			this.lbColumns.Name = "lbColumns";
			this.lbColumns.Size = new System.Drawing.Size(405, 147);
			this.lbColumns.TabIndex = 7;
			// 
			// tbSelect
			// 
			this.tbSelect.Location = new System.Drawing.Point(10, 258);
			this.tbSelect.Multiline = true;
			this.tbSelect.Name = "tbSelect";
			this.tbSelect.Size = new System.Drawing.Size(405, 108);
			this.tbSelect.TabIndex = 8;
			// 
			// lblShapeFile
			// 
			this.lblShapeFile.AutoSize = true;
			this.lblShapeFile.Location = new System.Drawing.Point(7, 13);
			this.lblShapeFile.Name = "lblShapeFile";
			this.lblShapeFile.Size = new System.Drawing.Size(88, 13);
			this.lblShapeFile.TabIndex = 9;
			this.lblShapeFile.Text = "Select shape file:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 63);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(108, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "Columns in Shapefile:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 242);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(408, 13);
			this.label2.TabIndex = 11;
			this.label2.Text = "Create SELECT (Required fields FACILITY, SECTION, GEOMETRY; AREA optional):";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 373);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(221, 13);
			this.label3.TabIndex = 12;
			this.label3.Text = "FROM TEMP_SHAPEFILE must be included.";
			// 
			// lblProgress
			// 
			this.lblProgress.AutoSize = true;
			this.lblProgress.Location = new System.Drawing.Point(198, 412);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new System.Drawing.Size(0, 13);
			this.lblProgress.TabIndex = 13;
			// 
			// FormImportShapeFile
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(421, 436);
			this.Controls.Add(this.lblProgress);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblShapeFile);
			this.Controls.Add(this.tbSelect);
			this.Controls.Add(this.lbColumns);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.tbShapeFileLocation);
			this.Name = "FormImportShapeFile";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Import Shape File...";
			this.Load += new System.EventHandler(this.FormImportShapeFile_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormImportShapeFile_FormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbShapeFileLocation;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListBox lbColumns;
        private System.Windows.Forms.TextBox tbSelect;
        private System.Windows.Forms.Label lblShapeFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblProgress;
    }
}