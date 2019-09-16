namespace RoadCare3
{
    partial class FormProperties
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProperties));
			this.pgProperties = new PropertyGridEx.PropertyGridEx();
			this.SuspendLayout();
			// 
			// pgProperties
			// 
			// 
			// 
			// 
			this.pgProperties.DocCommentDescription.AccessibleName = "";
			this.pgProperties.DocCommentDescription.AutoEllipsis = true;
			this.pgProperties.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
			this.pgProperties.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
			this.pgProperties.DocCommentDescription.Name = "";
			this.pgProperties.DocCommentDescription.Size = new System.Drawing.Size(275, 37);
			this.pgProperties.DocCommentDescription.TabIndex = 1;
			this.pgProperties.DocCommentImage = null;
			// 
			// 
			// 
			this.pgProperties.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
			this.pgProperties.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
			this.pgProperties.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
			this.pgProperties.DocCommentTitle.Name = "";
			this.pgProperties.DocCommentTitle.Size = new System.Drawing.Size(275, 15);
			this.pgProperties.DocCommentTitle.TabIndex = 0;
			this.pgProperties.DocCommentTitle.UseMnemonic = false;
			this.pgProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgProperties.Location = new System.Drawing.Point(0, 0);
			this.pgProperties.Name = "pgProperties";
			this.pgProperties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.pgProperties.SelectedObject = ((object)(resources.GetObject("pgProperties.SelectedObject")));
			this.pgProperties.ShowCustomProperties = true;
			this.pgProperties.Size = new System.Drawing.Size(281, 542);
			this.pgProperties.TabIndex = 0;
			// 
			// 
			// 
			this.pgProperties.ToolStrip.AccessibleName = "ToolBar";
			this.pgProperties.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
			this.pgProperties.ToolStrip.AllowMerge = false;
			this.pgProperties.ToolStrip.AutoSize = false;
			this.pgProperties.ToolStrip.CanOverflow = false;
			this.pgProperties.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.pgProperties.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.pgProperties.ToolStrip.Location = new System.Drawing.Point(0, 1);
			this.pgProperties.ToolStrip.Name = "";
			this.pgProperties.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
			this.pgProperties.ToolStrip.Size = new System.Drawing.Size(281, 25);
			this.pgProperties.ToolStrip.TabIndex = 1;
			this.pgProperties.ToolStrip.TabStop = true;
			this.pgProperties.ToolStrip.Text = "PropertyGridToolBar";
			this.pgProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgProperties_PropertyValueChanged);
			// 
			// FormProperties
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(281, 542);
			this.Controls.Add(this.pgProperties);
			this.Name = "FormProperties";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "AddAttributeDialoge";
			this.Load += new System.EventHandler(this.FormProperties_Load);
			this.ResumeLayout(false);

        }

        #endregion

        private PropertyGridEx.PropertyGridEx pgProperties;

    }
}