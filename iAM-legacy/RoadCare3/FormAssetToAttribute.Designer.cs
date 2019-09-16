namespace RoadCare3
{
    partial class FormAssetToAttribute
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
            this.dataGridViewAssetToAttribute = new System.Windows.Forms.DataGridView();
            this.Asset = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.AssetProperty = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Attribute = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Format = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Default = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Minimum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Maximum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblMaterial = new System.Windows.Forms.Label();
            this.buttonAddAttribute = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAssetToAttribute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewAssetToAttribute
            // 
            this.dataGridViewAssetToAttribute.AllowUserToAddRows = false;
            this.dataGridViewAssetToAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewAssetToAttribute.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAssetToAttribute.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Asset,
            this.AssetProperty,
            this.Attribute,
            this.Format,
            this.Default,
            this.Minimum,
            this.Maximum});
            this.dataGridViewAssetToAttribute.Location = new System.Drawing.Point(1, 42);
            this.dataGridViewAssetToAttribute.Name = "dataGridViewAssetToAttribute";
            this.dataGridViewAssetToAttribute.Size = new System.Drawing.Size(690, 406);
            this.dataGridViewAssetToAttribute.TabIndex = 0;
            this.dataGridViewAssetToAttribute.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridViewAssetToAttribute_UserDeletedRow);
            this.dataGridViewAssetToAttribute.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewAssetToAttribute_CellEndEdit);
            this.dataGridViewAssetToAttribute.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridViewAssetToAttribute_RowsRemoved);
            // 
            // Asset
            // 
            this.Asset.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Asset.HeaderText = "Asset";
            this.Asset.Name = "Asset";
            // 
            // AssetProperty
            // 
            this.AssetProperty.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.AssetProperty.HeaderText = "AssetProperty";
            this.AssetProperty.Name = "AssetProperty";
            // 
            // Attribute
            // 
            this.Attribute.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Attribute.HeaderText = "Attribute";
            this.Attribute.Name = "Attribute";
            // 
            // Format
            // 
            this.Format.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Format.HeaderText = "Format";
            this.Format.Name = "Format";
            // 
            // Default
            // 
            this.Default.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Default.HeaderText = "Default";
            this.Default.Name = "Default";
            // 
            // Minimum
            // 
            this.Minimum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Minimum.HeaderText = "Minimum";
            this.Minimum.Name = "Minimum";
            // 
            // Maximum
            // 
            this.Maximum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Maximum.HeaderText = "Maximum";
            this.Maximum.Name = "Maximum";
            // 
            // lblMaterial
            // 
            this.lblMaterial.AutoSize = true;
            this.lblMaterial.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaterial.Location = new System.Drawing.Point(32, 4);
            this.lblMaterial.Name = "lblMaterial";
            this.lblMaterial.Size = new System.Drawing.Size(189, 25);
            this.lblMaterial.TabIndex = 20;
            this.lblMaterial.Text = "Asset To Attribute:";
            // 
            // buttonAddAttribute
            // 
            this.buttonAddAttribute.Location = new System.Drawing.Point(539, 7);
            this.buttonAddAttribute.Name = "buttonAddAttribute";
            this.buttonAddAttribute.Size = new System.Drawing.Size(140, 23);
            this.buttonAddAttribute.TabIndex = 22;
            this.buttonAddAttribute.Text = "Add Attribute";
            this.buttonAddAttribute.UseVisualStyleBackColor = true;
            this.buttonAddAttribute.Click += new System.EventHandler(this.buttonAddAttribute_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RoadCare3.Properties.Resources.bluedatabase;
            this.pictureBox1.Location = new System.Drawing.Point(5, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(21, 29);
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // FormAssetToAttribute
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 445);
            this.Controls.Add(this.buttonAddAttribute);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblMaterial);
            this.Controls.Add(this.dataGridViewAssetToAttribute);
            this.Name = "FormAssetToAttribute";
            this.TabText = "Asset To Attribute";
            this.Text = "Asset To Attribute";
            this.Load += new System.EventHandler(this.FormAssetToAttribute_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAssetToAttribute_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAssetToAttribute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewAssetToAttribute;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblMaterial;
        private System.Windows.Forms.DataGridViewComboBoxColumn Asset;
        private System.Windows.Forms.DataGridViewComboBoxColumn AssetProperty;
        private System.Windows.Forms.DataGridViewTextBoxColumn Attribute;
        private System.Windows.Forms.DataGridViewTextBoxColumn Format;
        private System.Windows.Forms.DataGridViewTextBoxColumn Default;
        private System.Windows.Forms.DataGridViewTextBoxColumn Minimum;
        private System.Windows.Forms.DataGridViewTextBoxColumn Maximum;
        private System.Windows.Forms.Button buttonAddAttribute;
    }
}