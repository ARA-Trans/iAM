namespace RoadCare3
{
    partial class FormAssetsCalculated
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblMaterial = new System.Windows.Forms.Label();
            this.dataGridViewCalculatedAssets = new System.Windows.Forms.DataGridView();
            this.ASSET = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Property = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Equation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Critera = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCalculatedAssets)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RoadCare3.Properties.Resources.bluedatabase;
            this.pictureBox1.Location = new System.Drawing.Point(4, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(21, 29);
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // lblMaterial
            // 
            this.lblMaterial.AutoSize = true;
            this.lblMaterial.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaterial.Location = new System.Drawing.Point(31, 6);
            this.lblMaterial.Name = "lblMaterial";
            this.lblMaterial.Size = new System.Drawing.Size(185, 25);
            this.lblMaterial.TabIndex = 20;
            this.lblMaterial.Text = "Calculated Assets";
            // 
            // dataGridViewCalculatedAssets
            // 
            this.dataGridViewCalculatedAssets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewCalculatedAssets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCalculatedAssets.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ASSET,
            this.Property,
            this.Type,
            this.Equation,
            this.Critera});
            this.dataGridViewCalculatedAssets.Location = new System.Drawing.Point(2, 44);
            this.dataGridViewCalculatedAssets.Name = "dataGridViewCalculatedAssets";
            this.dataGridViewCalculatedAssets.Size = new System.Drawing.Size(661, 432);
            this.dataGridViewCalculatedAssets.TabIndex = 22;
            this.dataGridViewCalculatedAssets.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewCalculatedAssets_CellDoubleClick);
            this.dataGridViewCalculatedAssets.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewCalculatedAssets_CellValidated);
            this.dataGridViewCalculatedAssets.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridViewCalculatedAssets_RowsAdded);

            // 
            // ASSET
            // 
            this.ASSET.HeaderText = "Asset";
            this.ASSET.Name = "ASSET";
            // 
            // Property
            // 
            this.Property.HeaderText = "Property";
            this.Property.Name = "Property";
            // 
            // Type
            // 
            this.Type.HeaderText = "Type";
            this.Type.Items.AddRange(new object[] {
            "STRING",
            "NUMBER"});
            this.Type.Name = "Type";
            this.Type.Width = 80;
            // 
            // Equation
            // 
            this.Equation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Equation.HeaderText = "Equation";
            this.Equation.Name = "Equation";
            this.Equation.ReadOnly = true;
            // 
            // Critera
            // 
            this.Critera.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Critera.HeaderText = "Criteria";
            this.Critera.Name = "Critera";
            this.Critera.ReadOnly = true;
            // 
            // FormAssetsCalculated
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 477);
            this.Controls.Add(this.dataGridViewCalculatedAssets);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblMaterial);
            this.Name = "FormAssetsCalculated";
            this.TabText = "Calculated Assets";
            this.Text = "Calculated Asset";
            this.Load += new System.EventHandler(this.FormAssetsCalculated_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAssetsCalculated_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCalculatedAssets)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblMaterial;
        private System.Windows.Forms.DataGridView dataGridViewCalculatedAssets;
        private System.Windows.Forms.DataGridViewComboBoxColumn ASSET;
        private System.Windows.Forms.DataGridViewTextBoxColumn Property;
        private System.Windows.Forms.DataGridViewComboBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Equation;
        private System.Windows.Forms.DataGridViewTextBoxColumn Critera;
    }
}