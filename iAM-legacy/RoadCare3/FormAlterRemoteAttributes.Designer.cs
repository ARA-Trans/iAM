namespace RoadCare3
{
    partial class FormAlterRemoteAttributes
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
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxRemoteAttributes = new System.Windows.Forms.ListBox();
            this.textBoxOldTableName = new System.Windows.Forms.TextBox();
            this.textBoxNewTableName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxSQLView = new System.Windows.Forms.TextBox();
            this.menuStripTools = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStripTools.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter old table name:";
            // 
            // listBoxRemoteAttributes
            // 
            this.listBoxRemoteAttributes.FormattingEnabled = true;
            this.listBoxRemoteAttributes.Location = new System.Drawing.Point(12, 79);
            this.listBoxRemoteAttributes.Name = "listBoxRemoteAttributes";
            this.listBoxRemoteAttributes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxRemoteAttributes.Size = new System.Drawing.Size(303, 264);
            this.listBoxRemoteAttributes.TabIndex = 1;
            this.listBoxRemoteAttributes.SelectedValueChanged += new System.EventHandler(this.listBoxRemoteAttributes_SelectedValueChanged);
            // 
            // textBoxOldTableName
            // 
            this.textBoxOldTableName.Location = new System.Drawing.Point(125, 30);
            this.textBoxOldTableName.Name = "textBoxOldTableName";
            this.textBoxOldTableName.Size = new System.Drawing.Size(190, 20);
            this.textBoxOldTableName.TabIndex = 2;
            // 
            // textBoxNewTableName
            // 
            this.textBoxNewTableName.Location = new System.Drawing.Point(125, 56);
            this.textBoxNewTableName.Name = "textBoxNewTableName";
            this.textBoxNewTableName.Size = new System.Drawing.Size(190, 20);
            this.textBoxNewTableName.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Enter new table name:";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(240, 510);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 32);
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(12, 510);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 32);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxSQLView
            // 
            this.textBoxSQLView.Location = new System.Drawing.Point(12, 349);
            this.textBoxSQLView.Multiline = true;
            this.textBoxSQLView.Name = "textBoxSQLView";
            this.textBoxSQLView.Size = new System.Drawing.Size(303, 156);
            this.textBoxSQLView.TabIndex = 10;
            // 
            // menuStripTools
            // 
            this.menuStripTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem});
            this.menuStripTools.Location = new System.Drawing.Point(0, 0);
            this.menuStripTools.Name = "menuStripTools";
            this.menuStripTools.Size = new System.Drawing.Size(327, 24);
            this.menuStripTools.TabIndex = 13;
            this.menuStripTools.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.updateToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadToolStripMenuItem.Text = "Load...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.updateToolStripMenuItem.Text = "Update";
            this.updateToolStripMenuItem.Click += new System.EventHandler(this.updateToolStripMenuItem_Click);
            // 
            // FormAlterRemoteAttributes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 554);
            this.Controls.Add(this.textBoxSQLView);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxNewTableName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxOldTableName);
            this.Controls.Add(this.listBoxRemoteAttributes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStripTools);
            this.MainMenuStrip = this.menuStripTools;
            this.Name = "FormAlterRemoteAttributes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Alter Remote Attribute Query";
            this.menuStripTools.ResumeLayout(false);
            this.menuStripTools.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxRemoteAttributes;
        private System.Windows.Forms.TextBox textBoxOldTableName;
        private System.Windows.Forms.TextBox textBoxNewTableName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxSQLView;
        private System.Windows.Forms.MenuStrip menuStripTools;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
    }
}