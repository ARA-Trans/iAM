namespace RCImageView3
{
    partial class FormImageViewSegmentation
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
            this.dgvSegmentRollup = new System.Windows.Forms.DataGridView();
            this.Attribute = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RollupMethod = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.SegmentMethod = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.listBoxAttributeValues = new System.Windows.Forms.ListBox();
            this.checkBoxShowAvailable = new System.Windows.Forms.CheckBox();
            this.buttonSegmentRollup = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timerRollup = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSegmentRollup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSegmentRollup
            // 
            this.dgvSegmentRollup.AllowUserToAddRows = false;
            this.dgvSegmentRollup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvSegmentRollup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSegmentRollup.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Attribute,
            this.RollupMethod,
            this.SegmentMethod});
            this.dgvSegmentRollup.Location = new System.Drawing.Point(1, 1);
            this.dgvSegmentRollup.Name = "dgvSegmentRollup";
            this.dgvSegmentRollup.RowHeadersVisible = false;
            this.dgvSegmentRollup.Size = new System.Drawing.Size(445, 455);
            this.dgvSegmentRollup.TabIndex = 0;
            this.dgvSegmentRollup.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSegmentRollup_RowEnter);
            // 
            // Attribute
            // 
            this.Attribute.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Attribute.HeaderText = "Attribute";
            this.Attribute.Name = "Attribute";
            this.Attribute.Width = 71;
            // 
            // RollupMethod
            // 
            this.RollupMethod.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.RollupMethod.HeaderText = "Rollup Method";
            this.RollupMethod.Name = "RollupMethod";
            // 
            // SegmentMethod
            // 
            this.SegmentMethod.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SegmentMethod.HeaderText = "Segmentation Method";
            this.SegmentMethod.Items.AddRange(new object[] {
            "-",
            "Any Record",
            "Any Change"});
            this.SegmentMethod.Name = "SegmentMethod";
            // 
            // listBoxAttributeValues
            // 
            this.listBoxAttributeValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxAttributeValues.FormattingEnabled = true;
            this.listBoxAttributeValues.Location = new System.Drawing.Point(452, 3);
            this.listBoxAttributeValues.Name = "listBoxAttributeValues";
            this.listBoxAttributeValues.Size = new System.Drawing.Size(227, 420);
            this.listBoxAttributeValues.TabIndex = 1;
            // 
            // checkBoxShowAvailable
            // 
            this.checkBoxShowAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxShowAvailable.AutoSize = true;
            this.checkBoxShowAvailable.Checked = true;
            this.checkBoxShowAvailable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowAvailable.Location = new System.Drawing.Point(456, 438);
            this.checkBoxShowAvailable.Name = "checkBoxShowAvailable";
            this.checkBoxShowAvailable.Size = new System.Drawing.Size(138, 17);
            this.checkBoxShowAvailable.TabIndex = 2;
            this.checkBoxShowAvailable.Text = "Show all available data.";
            this.checkBoxShowAvailable.UseVisualStyleBackColor = true;
            this.checkBoxShowAvailable.CheckedChanged += new System.EventHandler(this.checkBoxShowAvailable_CheckedChanged);
            // 
            // buttonSegmentRollup
            // 
            this.buttonSegmentRollup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSegmentRollup.Location = new System.Drawing.Point(3, 462);
            this.buttonSegmentRollup.Name = "buttonSegmentRollup";
            this.buttonSegmentRollup.Size = new System.Drawing.Size(204, 23);
            this.buttonSegmentRollup.TabIndex = 3;
            this.buttonSegmentRollup.Text = "Segment/Rollup";
            this.buttonSegmentRollup.UseVisualStyleBackColor = true;
            this.buttonSegmentRollup.Click += new System.EventHandler(this.buttonSegmentRollup_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.Location = new System.Drawing.Point(213, 462);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(466, 22);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // timerRollup
            // 
            this.timerRollup.Tick += new System.EventHandler(this.timerRollup_Tick);
            // 
            // FormImageViewSegmentation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(681, 491);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonSegmentRollup);
            this.Controls.Add(this.checkBoxShowAvailable);
            this.Controls.Add(this.listBoxAttributeValues);
            this.Controls.Add(this.dgvSegmentRollup);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "FormImageViewSegmentation";
            this.TabText = "FormImageViewSegmentation";
            this.Text = "FormImageViewSegmentation";
            this.Load += new System.EventHandler(this.FormImageViewSegmentation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSegmentRollup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSegmentRollup;
        private System.Windows.Forms.ListBox listBoxAttributeValues;
        private System.Windows.Forms.CheckBox checkBoxShowAvailable;
        private System.Windows.Forms.Button buttonSegmentRollup;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Attribute;
        private System.Windows.Forms.DataGridViewComboBoxColumn RollupMethod;
        private System.Windows.Forms.DataGridViewComboBoxColumn SegmentMethod;
        private System.Windows.Forms.Timer timerRollup;
    }
}