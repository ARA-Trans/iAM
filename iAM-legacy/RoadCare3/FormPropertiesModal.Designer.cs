namespace RoadCare3
{
    partial class FormPropertiesModal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPropertiesModal));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pgProperties = new PropertyGridEx.PropertyGridEx();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Location = new System.Drawing.Point(12, 466);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(103, 50);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(204, 466);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(103, 50);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pgProperties
            // 
            this.pgProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.pgProperties.DocCommentDescription.AccessibleName = "";
            this.pgProperties.DocCommentDescription.AutoEllipsis = true;
            this.pgProperties.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgProperties.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
            this.pgProperties.DocCommentDescription.Name = "";
            this.pgProperties.DocCommentDescription.Size = new System.Drawing.Size(289, 37);
            this.pgProperties.DocCommentDescription.TabIndex = 1;
            this.pgProperties.DocCommentImage = null;
            // 
            // 
            // 
            this.pgProperties.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.pgProperties.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.pgProperties.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.pgProperties.DocCommentTitle.Name = "";
            this.pgProperties.DocCommentTitle.Size = new System.Drawing.Size(289, 15);
            this.pgProperties.DocCommentTitle.TabIndex = 0;
            this.pgProperties.DocCommentTitle.UseMnemonic = false;
            this.pgProperties.DrawFlatToolbar = true;
            this.pgProperties.Location = new System.Drawing.Point(12, 12);
            this.pgProperties.Name = "pgProperties";
            this.pgProperties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgProperties.SelectedObject = ((object)(resources.GetObject("pgProperties.SelectedObject")));
            this.pgProperties.ShowCustomProperties = true;
            this.pgProperties.Size = new System.Drawing.Size(295, 451);
            this.pgProperties.TabIndex = 3;
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
            this.pgProperties.ToolStrip.Size = new System.Drawing.Size(295, 25);
            this.pgProperties.ToolStrip.TabIndex = 1;
            this.pgProperties.ToolStrip.TabStop = true;
            this.pgProperties.ToolStrip.Text = "PropertyGridToolBar";
            this.pgProperties.ViewForeColor = System.Drawing.SystemColors.ControlText;
            this.pgProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgProperties_PropertyValueChanged);
            // 
            // FormPropertiesModal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 523);
            this.Controls.Add(this.pgProperties);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Name = "FormPropertiesModal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormPropertiesModal";
            this.Load += new System.EventHandler(this.FormPropertiesModal_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private PropertyGridEx.PropertyGridEx pgProperties;
    }
}
