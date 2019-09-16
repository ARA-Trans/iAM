namespace RoadCare3
{
	partial class FormSettings
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            this.pgeSettings = new PropertyGridEx.PropertyGridEx();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pgeSettings
            // 
            this.pgeSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.pgeSettings.DocCommentDescription.AccessibleName = "";
            this.pgeSettings.DocCommentDescription.AutoEllipsis = true;
            this.pgeSettings.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgeSettings.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
            this.pgeSettings.DocCommentDescription.Name = "";
            this.pgeSettings.DocCommentDescription.Size = new System.Drawing.Size(295, 37);
            this.pgeSettings.DocCommentDescription.TabIndex = 1;
            this.pgeSettings.DocCommentImage = null;
            // 
            // 
            // 
            this.pgeSettings.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgeSettings.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.pgeSettings.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.pgeSettings.DocCommentTitle.Name = "";
            this.pgeSettings.DocCommentTitle.Size = new System.Drawing.Size(295, 15);
            this.pgeSettings.DocCommentTitle.TabIndex = 0;
            this.pgeSettings.DocCommentTitle.UseMnemonic = false;
            this.pgeSettings.Location = new System.Drawing.Point(12, 12);
            this.pgeSettings.Name = "pgeSettings";
            this.pgeSettings.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pgeSettings.SelectedObject = ((object)(resources.GetObject("pgeSettings.SelectedObject")));
            this.pgeSettings.ShowCustomProperties = true;
            this.pgeSettings.Size = new System.Drawing.Size(301, 263);
            this.pgeSettings.TabIndex = 0;
            // 
            // 
            // 
            this.pgeSettings.ToolStrip.AccessibleName = "ToolBar";
            this.pgeSettings.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.pgeSettings.ToolStrip.AllowMerge = false;
            this.pgeSettings.ToolStrip.AutoSize = false;
            this.pgeSettings.ToolStrip.CanOverflow = false;
            this.pgeSettings.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.pgeSettings.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.pgeSettings.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this.pgeSettings.ToolStrip.Name = "";
            this.pgeSettings.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.pgeSettings.ToolStrip.Size = new System.Drawing.Size(301, 25);
            this.pgeSettings.ToolStrip.TabIndex = 1;
            this.pgeSettings.ToolStrip.TabStop = true;
            this.pgeSettings.ToolStrip.Text = "PropertyGridToolBar";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOk.Location = new System.Drawing.Point(21, 291);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(105, 30);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(208, 291);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(105, 30);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 330);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.pgeSettings);
            this.Name = "FormSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TabText = "FormSettings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSettings_FormClosed);
            this.ResumeLayout(false);

		}

		#endregion

		private PropertyGridEx.PropertyGridEx pgeSettings;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
	}
}