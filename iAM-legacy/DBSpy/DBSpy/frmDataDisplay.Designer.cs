namespace DBSpy
{
    partial class frmDataDisplay
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDataDisplay));
			this.GroupBox2 = new System.Windows.Forms.GroupBox();
			this.lstViews = new System.Windows.Forms.ListBox();
			this.GroupBox1 = new System.Windows.Forms.GroupBox();
			this.lstTables = new System.Windows.Forms.ListBox();
			this.ContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.GetFieldInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.GroupBox3 = new System.Windows.Forms.GroupBox();
			this.lstFields = new System.Windows.Forms.ListBox();
			this.CloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.ViewCurrentConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
			this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.gbSQLQuery = new System.Windows.Forms.GroupBox();
			this.tbDataDisplay = new System.Windows.Forms.TextBox();
			this.tbCreateSQLView = new System.Windows.Forms.TextBox();
			this.lblCreateSQLView = new System.Windows.Forms.Label();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnTest = new System.Windows.Forms.Button();
			this.GroupBox2.SuspendLayout();
			this.GroupBox1.SuspendLayout();
			this.ContextMenuStrip1.SuspendLayout();
			this.GroupBox3.SuspendLayout();
			this.MenuStrip1.SuspendLayout();
			this.gbSQLQuery.SuspendLayout();
			this.SuspendLayout();
			// 
			// GroupBox2
			// 
			this.GroupBox2.Controls.Add(this.lstViews);
			this.GroupBox2.Location = new System.Drawing.Point(12, 234);
			this.GroupBox2.Name = "GroupBox2";
			this.GroupBox2.Size = new System.Drawing.Size(260, 172);
			this.GroupBox2.TabIndex = 6;
			this.GroupBox2.TabStop = false;
			this.GroupBox2.Text = "Database Views";
			// 
			// lstViews
			// 
			this.lstViews.FormattingEnabled = true;
			this.lstViews.Location = new System.Drawing.Point(13, 19);
			this.lstViews.Name = "lstViews";
			this.lstViews.Size = new System.Drawing.Size(237, 134);
			this.lstViews.TabIndex = 1;
			this.lstViews.SelectedIndexChanged += new System.EventHandler(this.lstViews_SelectedIndexChanged);
			// 
			// GroupBox1
			// 
			this.GroupBox1.Controls.Add(this.lstTables);
			this.GroupBox1.Location = new System.Drawing.Point(12, 27);
			this.GroupBox1.Name = "GroupBox1";
			this.GroupBox1.Size = new System.Drawing.Size(260, 201);
			this.GroupBox1.TabIndex = 5;
			this.GroupBox1.TabStop = false;
			this.GroupBox1.Text = "Database Tables";
			// 
			// lstTables
			// 
			this.lstTables.FormattingEnabled = true;
			this.lstTables.Location = new System.Drawing.Point(13, 22);
			this.lstTables.Name = "lstTables";
			this.lstTables.Size = new System.Drawing.Size(237, 173);
			this.lstTables.TabIndex = 0;
			this.lstTables.SelectedIndexChanged += new System.EventHandler(this.lstTables_SelectedIndexChanged);
			// 
			// ContextMenuStrip1
			// 
			this.ContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GetFieldInformationToolStripMenuItem});
			this.ContextMenuStrip1.Name = "ContextMenuStrip1";
			this.ContextMenuStrip1.Size = new System.Drawing.Size(199, 26);
			// 
			// GetFieldInformationToolStripMenuItem
			// 
			this.GetFieldInformationToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("GetFieldInformationToolStripMenuItem.Image")));
			this.GetFieldInformationToolStripMenuItem.Name = "GetFieldInformationToolStripMenuItem";
			this.GetFieldInformationToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.GetFieldInformationToolStripMenuItem.Text = "Get Field Information...";
			this.GetFieldInformationToolStripMenuItem.Click += new System.EventHandler(this.GetFieldInformationToolStripMenuItem_Click);
			// 
			// GroupBox3
			// 
			this.GroupBox3.Controls.Add(this.lstFields);
			this.GroupBox3.Location = new System.Drawing.Point(278, 27);
			this.GroupBox3.Name = "GroupBox3";
			this.GroupBox3.Size = new System.Drawing.Size(260, 379);
			this.GroupBox3.TabIndex = 7;
			this.GroupBox3.TabStop = false;
			this.GroupBox3.Text = "Selected Fields";
			// 
			// lstFields
			// 
			this.lstFields.ContextMenuStrip = this.ContextMenuStrip1;
			this.lstFields.FormattingEnabled = true;
			this.lstFields.Location = new System.Drawing.Point(12, 21);
			this.lstFields.Name = "lstFields";
			this.lstFields.Size = new System.Drawing.Size(236, 342);
			this.lstFields.TabIndex = 0;
			this.lstFields.SelectedIndexChanged += new System.EventHandler(this.lstFields_SelectedIndexChanged);
			// 
			// CloseToolStripMenuItem
			// 
			this.CloseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("CloseToolStripMenuItem.Image")));
			this.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem";
			this.CloseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.CloseToolStripMenuItem.Text = "&Close";
			this.CloseToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
			// 
			// ConnectionToolStripMenuItem
			// 
			this.ConnectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem1,
            this.ViewCurrentConnectionToolStripMenuItem});
			this.ConnectionToolStripMenuItem.Name = "ConnectionToolStripMenuItem";
			this.ConnectionToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
			this.ConnectionToolStripMenuItem.Text = "Co&nnection";
			// 
			// ToolStripMenuItem1
			// 
			this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
			this.ToolStripMenuItem1.Size = new System.Drawing.Size(210, 6);
			// 
			// ViewCurrentConnectionToolStripMenuItem
			// 
			this.ViewCurrentConnectionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ViewCurrentConnectionToolStripMenuItem.Image")));
			this.ViewCurrentConnectionToolStripMenuItem.Name = "ViewCurrentConnectionToolStripMenuItem";
			this.ViewCurrentConnectionToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
			this.ViewCurrentConnectionToolStripMenuItem.Text = "&View Current Connection";
			this.ViewCurrentConnectionToolStripMenuItem.Click += new System.EventHandler(this.ViewCurrentConnectionToolStripMenuItem_Click);
			// 
			// MenuStrip1
			// 
			this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.ConnectionToolStripMenuItem});
			this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
			this.MenuStrip1.Name = "MenuStrip1";
			this.MenuStrip1.Size = new System.Drawing.Size(1235, 24);
			this.MenuStrip1.TabIndex = 4;
			this.MenuStrip1.Text = "MenuStrip1";
			// 
			// FileToolStripMenuItem
			// 
			this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CloseToolStripMenuItem});
			this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
			this.FileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.FileToolStripMenuItem.Text = "&File";
			// 
			// gbSQLQuery
			// 
			this.gbSQLQuery.Controls.Add(this.tbDataDisplay);
			this.gbSQLQuery.Controls.Add(this.tbCreateSQLView);
			this.gbSQLQuery.Controls.Add(this.lblCreateSQLView);
			this.gbSQLQuery.Controls.Add(this.btnOk);
			this.gbSQLQuery.Controls.Add(this.btnTest);
			this.gbSQLQuery.Location = new System.Drawing.Point(544, 27);
			this.gbSQLQuery.Name = "gbSQLQuery";
			this.gbSQLQuery.Size = new System.Drawing.Size(679, 379);
			this.gbSQLQuery.TabIndex = 8;
			this.gbSQLQuery.TabStop = false;
			this.gbSQLQuery.Text = "SQL Statement";
			// 
			// tbDataDisplay
			// 
			this.tbDataDisplay.Location = new System.Drawing.Point(12, 240);
			this.tbDataDisplay.Multiline = true;
			this.tbDataDisplay.Name = "tbDataDisplay";
			this.tbDataDisplay.ReadOnly = true;
			this.tbDataDisplay.Size = new System.Drawing.Size(661, 104);
			this.tbDataDisplay.TabIndex = 5;
			// 
			// tbCreateSQLView
			// 
			this.tbCreateSQLView.Location = new System.Drawing.Point(12, 53);
			this.tbCreateSQLView.Multiline = true;
			this.tbCreateSQLView.Name = "tbCreateSQLView";
			this.tbCreateSQLView.Size = new System.Drawing.Size(661, 181);
			this.tbCreateSQLView.TabIndex = 4;
			this.tbCreateSQLView.Text = "SELECT ID_, ROUTES, BEGIN_STATION, END_STATION, DIRECTION, FACILITY, SECTION, SAM" +
				"PLE_, DATE_, <attributecolmn> AS DATA_ FROM <tablename>";
			// 
			// lblCreateSQLView
			// 
			this.lblCreateSQLView.AutoSize = true;
			this.lblCreateSQLView.Location = new System.Drawing.Point(9, 36);
			this.lblCreateSQLView.Name = "lblCreateSQLView";
			this.lblCreateSQLView.Size = new System.Drawing.Size(0, 13);
			this.lblCreateSQLView.TabIndex = 3;
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(598, 350);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnTest
			// 
			this.btnTest.Location = new System.Drawing.Point(513, 350);
			this.btnTest.Name = "btnTest";
			this.btnTest.Size = new System.Drawing.Size(75, 23);
			this.btnTest.TabIndex = 1;
			this.btnTest.Text = "Test";
			this.btnTest.UseVisualStyleBackColor = true;
			this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
			// 
			// frmDataDisplay
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1235, 418);
			this.Controls.Add(this.MenuStrip1);
			this.Controls.Add(this.GroupBox1);
			this.Controls.Add(this.gbSQLQuery);
			this.Controls.Add(this.GroupBox2);
			this.Controls.Add(this.GroupBox3);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmDataDisplay";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Database Information";
			this.GroupBox2.ResumeLayout(false);
			this.GroupBox1.ResumeLayout(false);
			this.ContextMenuStrip1.ResumeLayout(false);
			this.GroupBox3.ResumeLayout(false);
			this.MenuStrip1.ResumeLayout(false);
			this.MenuStrip1.PerformLayout();
			this.gbSQLQuery.ResumeLayout(false);
			this.gbSQLQuery.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.GroupBox GroupBox2;
        internal System.Windows.Forms.ListBox lstViews;
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.ListBox lstTables;
        internal System.Windows.Forms.ContextMenuStrip ContextMenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem GetFieldInformationToolStripMenuItem;
        internal System.Windows.Forms.GroupBox GroupBox3;
		internal System.Windows.Forms.ListBox lstFields;
        internal System.Windows.Forms.ToolStripMenuItem CloseToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ConnectionToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem1;
        internal System.Windows.Forms.ToolStripMenuItem ViewCurrentConnectionToolStripMenuItem;
        internal System.Windows.Forms.MenuStrip MenuStrip1;
		internal System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        internal System.Windows.Forms.GroupBox gbSQLQuery;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label lblCreateSQLView;
        private System.Windows.Forms.TextBox tbCreateSQLView;
		private System.Windows.Forms.TextBox tbDataDisplay;
    }
}