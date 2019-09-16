namespace RoadCare3
{
    partial class FormOutputWindow
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.cmsOutputWindow = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiClearOutputWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialogOutput = new System.Windows.Forms.SaveFileDialog();
            this.cmsOutputWindow.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.White;
            this.richTextBox1.ContextMenuStrip = this.cmsOutputWindow;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(1060, 170);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // cmsOutputWindow
            // 
            this.cmsOutputWindow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiClearOutputWindow,
            this.copyToolStripMenuItem,
            this.createLogFileToolStripMenuItem,
            this.closeLogFileToolStripMenuItem});
            this.cmsOutputWindow.Name = "contextMenuStrip1";
            this.cmsOutputWindow.Size = new System.Drawing.Size(190, 114);
            // 
            // tsmiClearOutputWindow
            // 
            this.tsmiClearOutputWindow.Name = "tsmiClearOutputWindow";
            this.tsmiClearOutputWindow.Size = new System.Drawing.Size(189, 22);
            this.tsmiClearOutputWindow.Text = "Clear Output Window";
            this.tsmiClearOutputWindow.Click += new System.EventHandler(this.tsmiClearOutputWindow_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // createLogFileToolStripMenuItem
            // 
            this.createLogFileToolStripMenuItem.Name = "createLogFileToolStripMenuItem";
            this.createLogFileToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.createLogFileToolStripMenuItem.Text = "Create Log File";
            this.createLogFileToolStripMenuItem.Click += new System.EventHandler(this.createLogFileToolStripMenuItem_Click);
            // 
            // closeLogFileToolStripMenuItem
            // 
            this.closeLogFileToolStripMenuItem.Name = "closeLogFileToolStripMenuItem";
            this.closeLogFileToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.closeLogFileToolStripMenuItem.Text = "Close Log File";
            this.closeLogFileToolStripMenuItem.Click += new System.EventHandler(this.closeLogFileToolStripMenuItem_Click);
            // 
            // saveFileDialogOutput
            // 
            this.saveFileDialogOutput.DefaultExt = "txt";
            this.saveFileDialogOutput.Filter = "Text files|*.txt|All files|*.*";
            // 
            // FormOutputWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 170);
            this.Controls.Add(this.richTextBox1);
            this.Name = "FormOutputWindow";
            this.TabText = "Output";
            this.Text = "Output";
            this.Load += new System.EventHandler(this.FormOutputWindow_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormOutputWindow_FormClosed);
            this.cmsOutputWindow.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ContextMenuStrip cmsOutputWindow;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearOutputWindow;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createLogFileToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialogOutput;
        private System.Windows.Forms.ToolStripMenuItem closeLogFileToolStripMenuItem;

    }
}