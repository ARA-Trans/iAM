namespace RCImageView3
{
    partial class FormImageViewSolutionExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImageViewSolutionExplorer));
            this.treeViewSolutionExplorer = new System.Windows.Forms.TreeView();
            this.contextMenuStripSolutionExplorerTab = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageListTreeView = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStripRawAttribute = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripRawAttributeNode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteRawAttributeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesRawAttributeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripNetwork = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripNetworkNode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripViews = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.updateImagePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialogImagePath = new System.Windows.Forms.FolderBrowserDialog();
            this.contextMenuStripSolutionExplorerTab.SuspendLayout();
            this.contextMenuStripRawAttribute.SuspendLayout();
            this.contextMenuStripRawAttributeNode.SuspendLayout();
            this.contextMenuStripNetwork.SuspendLayout();
            this.contextMenuStripNetworkNode.SuspendLayout();
            this.contextMenuStripViews.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewSolutionExplorer
            // 
            this.treeViewSolutionExplorer.ContextMenuStrip = this.contextMenuStripSolutionExplorerTab;
            this.treeViewSolutionExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewSolutionExplorer.ImageIndex = 0;
            this.treeViewSolutionExplorer.ImageList = this.imageListTreeView;
            this.treeViewSolutionExplorer.Location = new System.Drawing.Point(0, 0);
            this.treeViewSolutionExplorer.Name = "treeViewSolutionExplorer";
            this.treeViewSolutionExplorer.SelectedImageIndex = 0;
            this.treeViewSolutionExplorer.Size = new System.Drawing.Size(290, 264);
            this.treeViewSolutionExplorer.TabIndex = 0;
            this.treeViewSolutionExplorer.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewSolutionExplorer_NodeMouseDoubleClick);
            this.treeViewSolutionExplorer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewSolutionExplorer_MouseDown);
            // 
            // contextMenuStripSolutionExplorerTab
            // 
            this.contextMenuStripSolutionExplorerTab.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem});
            this.contextMenuStripSolutionExplorerTab.Name = "contextMenuStripSolutionExplorerTab";
            this.contextMenuStripSolutionExplorerTab.Size = new System.Drawing.Size(124, 26);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            // 
            // imageListTreeView
            // 
            this.imageListTreeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTreeView.ImageStream")));
            this.imageListTreeView.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListTreeView.Images.SetKeyName(0, "folderopen.ico");
            this.imageListTreeView.Images.SetKeyName(1, "LIGHTON.ICO");
            this.imageListTreeView.Images.SetKeyName(2, "db.ico");
            this.imageListTreeView.Images.SetKeyName(3, "networks.png");
            this.imageListTreeView.Images.SetKeyName(4, "backhoe.ico");
            this.imageListTreeView.Images.SetKeyName(5, "calculator.ico");
            this.imageListTreeView.Images.SetKeyName(6, "sign.ico");
            this.imageListTreeView.Images.SetKeyName(7, "Users.ico");
            this.imageListTreeView.Images.SetKeyName(8, "dbs.ico");
            this.imageListTreeView.Images.SetKeyName(9, "network.ico");
            this.imageListTreeView.Images.SetKeyName(10, "class.ico");
            this.imageListTreeView.Images.SetKeyName(11, "bluedb.ico");
            this.imageListTreeView.Images.SetKeyName(12, "bluedbs.ico");
            this.imageListTreeView.Images.SetKeyName(13, "construction.ico");
            this.imageListTreeView.Images.SetKeyName(14, "roadnetwork.ico");
            this.imageListTreeView.Images.SetKeyName(15, "orange.ico");
            this.imageListTreeView.Images.SetKeyName(16, "blue.ico");
            this.imageListTreeView.Images.SetKeyName(17, "pink.ico");
            this.imageListTreeView.Images.SetKeyName(18, "map.ico");
            this.imageListTreeView.Images.SetKeyName(19, "Folder.ico");
            this.imageListTreeView.Images.SetKeyName(20, "Camera.ico");
            // 
            // contextMenuStripRawAttribute
            // 
            this.contextMenuStripRawAttribute.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewToolStripMenuItem});
            this.contextMenuStripRawAttribute.Name = "contextMenuStripRawAttribute";
            this.contextMenuStripRawAttribute.Size = new System.Drawing.Size(128, 26);
            // 
            // addNewToolStripMenuItem
            // 
            this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
            this.addNewToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.addNewToolStripMenuItem.Text = "&Add new";
            this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
            // 
            // contextMenuStripRawAttributeNode
            // 
            this.contextMenuStripRawAttributeNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteRawAttributeToolStripMenuItem,
            this.propertiesRawAttributeToolStripMenuItem});
            this.contextMenuStripRawAttributeNode.Name = "contextMenuStripRawAttributeNode";
            this.contextMenuStripRawAttributeNode.Size = new System.Drawing.Size(135, 48);
            // 
            // deleteRawAttributeToolStripMenuItem
            // 
            this.deleteRawAttributeToolStripMenuItem.Name = "deleteRawAttributeToolStripMenuItem";
            this.deleteRawAttributeToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.deleteRawAttributeToolStripMenuItem.Text = "&Delete";
            this.deleteRawAttributeToolStripMenuItem.Click += new System.EventHandler(this.deleteRawAttributeToolStripMenuItem_Click);
            // 
            // propertiesRawAttributeToolStripMenuItem
            // 
            this.propertiesRawAttributeToolStripMenuItem.Name = "propertiesRawAttributeToolStripMenuItem";
            this.propertiesRawAttributeToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.propertiesRawAttributeToolStripMenuItem.Text = "&Properties";
            // 
            // contextMenuStripNetwork
            // 
            this.contextMenuStripNetwork.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewToolStripMenuItem1});
            this.contextMenuStripNetwork.Name = "contextMenuStripNetwork";
            this.contextMenuStripNetwork.Size = new System.Drawing.Size(129, 26);
            // 
            // addNewToolStripMenuItem1
            // 
            this.addNewToolStripMenuItem1.Name = "addNewToolStripMenuItem1";
            this.addNewToolStripMenuItem1.Size = new System.Drawing.Size(128, 22);
            this.addNewToolStripMenuItem1.Text = "Add New";
            this.addNewToolStripMenuItem1.Click += new System.EventHandler(this.addNewToolStripMenuItem1_Click);
            // 
            // contextMenuStripNetworkNode
            // 
            this.contextMenuStripNetworkNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.contextMenuStripNetworkNode.Name = "contextMenuStripNetworkNode";
            this.contextMenuStripNetworkNode.Size = new System.Drawing.Size(160, 26);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.deleteToolStripMenuItem.Text = "Delete Network";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // contextMenuStripViews
            // 
            this.contextMenuStripViews.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateImagePathToolStripMenuItem});
            this.contextMenuStripViews.Name = "contextMenuStripViews";
            this.contextMenuStripViews.Size = new System.Drawing.Size(179, 26);
            // 
            // updateImagePathToolStripMenuItem
            // 
            this.updateImagePathToolStripMenuItem.Name = "updateImagePathToolStripMenuItem";
            this.updateImagePathToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.updateImagePathToolStripMenuItem.Text = "Update Image Path";
            this.updateImagePathToolStripMenuItem.Click += new System.EventHandler(this.updateImagePathToolStripMenuItem_Click);
            // 
            // FormImageViewSolutionExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 264);
            this.CloseButton = false;
            this.Controls.Add(this.treeViewSolutionExplorer);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormImageViewSolutionExplorer";
            this.TabPageContextMenuStrip = this.contextMenuStripSolutionExplorerTab;
            this.TabText = "Solution Explorer";
            this.Text = "Solution Explorer";
            this.Load += new System.EventHandler(this.FormImageViewSolutionExplorer_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FormImageViewSolutionExplorer_MouseClick);
            this.contextMenuStripSolutionExplorerTab.ResumeLayout(false);
            this.contextMenuStripRawAttribute.ResumeLayout(false);
            this.contextMenuStripRawAttributeNode.ResumeLayout(false);
            this.contextMenuStripNetwork.ResumeLayout(false);
            this.contextMenuStripNetworkNode.ResumeLayout(false);
            this.contextMenuStripViews.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewSolutionExplorer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripSolutionExplorerTab;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ImageList imageListTreeView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRawAttribute;
        private System.Windows.Forms.ToolStripMenuItem addNewToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRawAttributeNode;
        private System.Windows.Forms.ToolStripMenuItem deleteRawAttributeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesRawAttributeToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNetwork;
        private System.Windows.Forms.ToolStripMenuItem addNewToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNetworkNode;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripViews;
        private System.Windows.Forms.ToolStripMenuItem updateImagePathToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogImagePath;
    }
}