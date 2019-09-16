namespace RoadCare3
{
    partial class DialogImportData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogImportData));
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.lstData = new System.Windows.Forms.ListBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.lstTables = new System.Windows.Forms.ListBox();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.lstFields = new System.Windows.Forms.ListBox();
            this.pgConnection = new PropertyGridEx.PropertyGridEx();
            this.btnOk = new System.Windows.Forms.Button();
            this.tbSQLQuery = new System.Windows.Forms.TextBox();
            this.tbDataDisplay = new System.Windows.Forms.TextBox();
            this.lblExample = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbSQLQuery = new System.Windows.Forms.GroupBox();
            this.GroupBox2.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.gbSQLQuery.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.lstData);
            this.GroupBox2.Location = new System.Drawing.Point(736, 14);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(219, 200);
            this.GroupBox2.TabIndex = 10;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Field Data";
            // 
            // lstData
            // 
            this.lstData.FormattingEnabled = true;
            this.lstData.HorizontalScrollbar = true;
            this.lstData.Location = new System.Drawing.Point(6, 19);
            this.lstData.Name = "lstData";
            this.lstData.Size = new System.Drawing.Size(206, 173);
            this.lstData.TabIndex = 1;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(264, 102);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "Connect";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.lstTables);
            this.GroupBox1.Location = new System.Drawing.Point(345, 14);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(219, 201);
            this.GroupBox1.TabIndex = 9;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Database Tables";
            // 
            // lstTables
            // 
            this.lstTables.FormattingEnabled = true;
            this.lstTables.HorizontalScrollbar = true;
            this.lstTables.Location = new System.Drawing.Point(13, 22);
            this.lstTables.Name = "lstTables";
            this.lstTables.Size = new System.Drawing.Size(200, 173);
            this.lstTables.TabIndex = 0;
            this.lstTables.SelectedIndexChanged += new System.EventHandler(this.lstTables_SelectedIndexChanged);
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.lstFields);
            this.GroupBox3.Location = new System.Drawing.Point(570, 13);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(160, 201);
            this.GroupBox3.TabIndex = 11;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Fields";
            // 
            // lstFields
            // 
            this.lstFields.FormattingEnabled = true;
            this.lstFields.HorizontalScrollbar = true;
            this.lstFields.Location = new System.Drawing.Point(12, 21);
            this.lstFields.Name = "lstFields";
            this.lstFields.Size = new System.Drawing.Size(140, 173);
            this.lstFields.TabIndex = 0;
            this.lstFields.SelectedIndexChanged += new System.EventHandler(this.lstFields_SelectedIndexChanged);
            // 
            // pgConnection
            // 
            // 
            // 
            // 
            this.pgConnection.DocCommentDescription.AccessibleName = "";
            this.pgConnection.DocCommentDescription.AutoEllipsis = true;
            this.pgConnection.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgConnection.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
            this.pgConnection.DocCommentDescription.Name = "";
            this.pgConnection.DocCommentDescription.Size = new System.Drawing.Size(240, 37);
            this.pgConnection.DocCommentDescription.TabIndex = 1;
            this.pgConnection.DocCommentImage = null;
            // 
            // 
            // 
            this.pgConnection.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgConnection.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.pgConnection.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.pgConnection.DocCommentTitle.Name = "";
            this.pgConnection.DocCommentTitle.Size = new System.Drawing.Size(240, 15);
            this.pgConnection.DocCommentTitle.TabIndex = 0;
            this.pgConnection.DocCommentTitle.UseMnemonic = false;
            this.pgConnection.Location = new System.Drawing.Point(12, 13);
            this.pgConnection.Name = "pgConnection";
            this.pgConnection.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgConnection.SelectedObject = ((object)(resources.GetObject("pgConnection.SelectedObject")));
            this.pgConnection.ShowCustomProperties = true;
            this.pgConnection.Size = new System.Drawing.Size(246, 364);
            this.pgConnection.TabIndex = 13;
            // 
            // 
            // 
            this.pgConnection.ToolStrip.AccessibleName = "ToolBar";
            this.pgConnection.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.pgConnection.ToolStrip.AllowMerge = false;
            this.pgConnection.ToolStrip.AutoSize = false;
            this.pgConnection.ToolStrip.CanOverflow = false;
            this.pgConnection.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.pgConnection.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.pgConnection.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this.pgConnection.ToolStrip.Name = "";
            this.pgConnection.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.pgConnection.ToolStrip.Size = new System.Drawing.Size(246, 25);
            this.pgConnection.ToolStrip.TabIndex = 1;
            this.pgConnection.ToolStrip.TabStop = true;
            this.pgConnection.ToolStrip.Text = "PropertyGridToolBar";
            this.pgConnection.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgConnection_PropertyValueChanged);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(609, 99);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // tbSQLQuery
            // 
            this.tbSQLQuery.Location = new System.Drawing.Point(9, 19);
            this.tbSQLQuery.Multiline = true;
            this.tbSQLQuery.Name = "tbSQLQuery";
            this.tbSQLQuery.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbSQLQuery.Size = new System.Drawing.Size(364, 103);
            this.tbSQLQuery.TabIndex = 4;
            this.tbSQLQuery.Text = "SELECT ROUTES, DIRECTION, BEGIN_STATION, END_STATION, DATE_, DATA_ FROM ";
            // 
            // tbDataDisplay
            // 
            this.tbDataDisplay.Location = new System.Drawing.Point(379, 19);
            this.tbDataDisplay.Multiline = true;
            this.tbDataDisplay.Name = "tbDataDisplay";
            this.tbDataDisplay.ReadOnly = true;
            this.tbDataDisplay.Size = new System.Drawing.Size(305, 79);
            this.tbDataDisplay.TabIndex = 5;
            // 
            // lblExample
            // 
            this.lblExample.Location = new System.Drawing.Point(6, 128);
            this.lblExample.Name = "lblExample";
            this.lblExample.Size = new System.Drawing.Size(678, 29);
            this.lblExample.TabIndex = 6;
            this.lblExample.Text = "Example:  SELECT ROAD AS ROUTES, DIR AS DIRECTION, BMP AS BEGIN_STATION, EMP AS E" +
    "ND_STATION, ADT AS DATA, DATE FROM ROAD_INFORMATION";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(379, 99);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbSQLQuery
            // 
            this.gbSQLQuery.Controls.Add(this.btnCancel);
            this.gbSQLQuery.Controls.Add(this.lblExample);
            this.gbSQLQuery.Controls.Add(this.tbSQLQuery);
            this.gbSQLQuery.Controls.Add(this.tbDataDisplay);
            this.gbSQLQuery.Controls.Add(this.btnOk);
            this.gbSQLQuery.Location = new System.Drawing.Point(264, 220);
            this.gbSQLQuery.Name = "gbSQLQuery";
            this.gbSQLQuery.Size = new System.Drawing.Size(691, 158);
            this.gbSQLQuery.TabIndex = 12;
            this.gbSQLQuery.TabStop = false;
            this.gbSQLQuery.Text = "SQL Statement";
            // 
            // DialogImportData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 387);
            this.Controls.Add(this.pgConnection);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.gbSQLQuery);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.GroupBox3);
            this.Name = "DialogImportData";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import Data...";
            this.Load += new System.EventHandler(this.DialogImportData_Load);
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox3.ResumeLayout(false);
            this.gbSQLQuery.ResumeLayout(false);
            this.gbSQLQuery.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox GroupBox2;
        internal System.Windows.Forms.ListBox lstData;
        private System.Windows.Forms.Button btnTest;
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.ListBox lstTables;
        internal System.Windows.Forms.GroupBox GroupBox3;
        internal System.Windows.Forms.ListBox lstFields;
        private PropertyGridEx.PropertyGridEx pgConnection;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox tbSQLQuery;
        private System.Windows.Forms.TextBox tbDataDisplay;
        private System.Windows.Forms.Label lblExample;
        private System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.GroupBox gbSQLQuery;
    }
}