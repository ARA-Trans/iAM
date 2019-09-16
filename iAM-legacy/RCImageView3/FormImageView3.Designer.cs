namespace RCImageView3
{
    partial class FormImageView3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImageView3));
            this.dockPanelMain = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.folderBrowserDialogImagePath = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // dockPanelMain
            // 
            this.dockPanelMain.ActiveAutoHideContent = null;
            this.dockPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanelMain.Location = new System.Drawing.Point(0, 0);
            this.dockPanelMain.Name = "dockPanelMain";
            this.dockPanelMain.Size = new System.Drawing.Size(998, 579);
            this.dockPanelMain.TabIndex = 2;
            // 
            // folderBrowserDialogImagePath
            // 
            this.folderBrowserDialogImagePath.Description = "Select root directory for Image Location";
            this.folderBrowserDialogImagePath.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // FormImageView3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(998, 579);
            this.Controls.Add(this.dockPanelMain);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::RCImageView3.Properties.Settings.Default, "APP_SIZE", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Location = global::RCImageView3.Properties.Settings.Default.APP_SIZE;
            this.Name = "FormImageView3";
            this.Text = "RoadCare Image Viewer";
            this.Load += new System.EventHandler(this.FormImageView3_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormImageView3_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanelMain;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogImagePath;
    }
}

