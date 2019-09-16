namespace RCImageView3
{
    partial class FormGoogleMap
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGoogleMap));
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.openFileDialogkml = new System.Windows.Forms.OpenFileDialog();
            this.toolStripGoogle = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonKML = new System.Windows.Forms.ToolStripButton();
            this.toolStripGoogle.SuspendLayout();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(292, 240);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.Url = new System.Uri("http://www.edhellen.com/test.htm", System.UriKind.Absolute);
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // openFileDialogkml
            // 
            this.openFileDialogkml.FileName = "openFileDialog1";
            this.openFileDialogkml.Filter = "KML Files|*.kml";
            // 
            // toolStripGoogle
            // 
            this.toolStripGoogle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStripGoogle.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonKML});
            this.toolStripGoogle.Location = new System.Drawing.Point(0, 241);
            this.toolStripGoogle.Name = "toolStripGoogle";
            this.toolStripGoogle.Size = new System.Drawing.Size(292, 25);
            this.toolStripGoogle.TabIndex = 1;
            this.toolStripGoogle.Text = "toolStrip1";
            // 
            // toolStripButtonKML
            // 
            this.toolStripButtonKML.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonKML.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonKML.Image")));
            this.toolStripButtonKML.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonKML.Name = "toolStripButtonKML";
            this.toolStripButtonKML.Size = new System.Drawing.Size(107, 22);
            this.toolStripButtonKML.Text = "Create KML Overlay";
            this.toolStripButtonKML.Click += new System.EventHandler(this.toolStripButtonKML_Click);
            // 
            // FormGoogleMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.toolStripGoogle);
            this.Controls.Add(this.webBrowser1);
            this.Name = "FormGoogleMap";
            this.TabText = "FormGoogleMap";
            this.Text = "FormGoogleMap";
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FormGoogleMap_MouseClick);
            this.toolStripGoogle.ResumeLayout(false);
            this.toolStripGoogle.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.OpenFileDialog openFileDialogkml;
        private System.Windows.Forms.ToolStrip toolStripGoogle;
        private System.Windows.Forms.ToolStripButton toolStripButtonKML;

    }
}