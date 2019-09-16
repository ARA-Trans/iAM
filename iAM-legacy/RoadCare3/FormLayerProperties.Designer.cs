namespace RoadCare3
{
    partial class FormLayerProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLayerProperties));
            ImageComboBox.ImageComboBoxItem imageComboBoxItem1 = new ImageComboBox.ImageComboBoxItem();
            ImageComboBox.ImageComboBoxItem imageComboBoxItem2 = new ImageComboBox.ImageComboBoxItem();
            ImageComboBox.ImageComboBoxItem imageComboBoxItem3 = new ImageComboBox.ImageComboBoxItem();
            ImageComboBox.ImageComboBoxItem imageComboBoxItem4 = new ImageComboBox.ImageComboBoxItem();
            ImageComboBox.ImageComboBoxItem imageComboBoxItem5 = new ImageComboBox.ImageComboBoxItem();
            ImageComboBox.ImageComboBoxItem imageComboBoxItem6 = new ImageComboBox.ImageComboBoxItem();
            ImageComboBox.ImageComboBoxItem imageComboBoxItem7 = new ImageComboBox.ImageComboBoxItem();
            this.chkboxLabels = new System.Windows.Forms.CheckBox();
            this.lblName = new System.Windows.Forms.Label();
            this.tbLayerName = new System.Windows.Forms.TextBox();
            this.lblLabels = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSymbol = new System.Windows.Forms.Button();
            this.lblLayerSymbol = new System.Windows.Forms.Label();
            this.imageListDefaultSymbols = new System.Windows.Forms.ImageList(this.components);
            this.imageListLines = new System.Windows.Forms.ImageList(this.components);
            this.imageComboBoxSymbols = new ImageComboBox.ImageComboBox();
            this.imageComboBoxLineThickness = new ImageComboBox.ImageComboBox();
            this.lblSymbol = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnColor = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkboxLabels
            // 
            this.chkboxLabels.AutoSize = true;
            this.chkboxLabels.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkboxLabels.Location = new System.Drawing.Point(211, 28);
            this.chkboxLabels.Name = "chkboxLabels";
            this.chkboxLabels.Size = new System.Drawing.Size(15, 14);
            this.chkboxLabels.TabIndex = 10;
            this.chkboxLabels.UseVisualStyleBackColor = true;
            this.chkboxLabels.CheckedChanged += new System.EventHandler(this.chkboxLabels_CheckedChanged);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(0, 9);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(67, 13);
            this.lblName.TabIndex = 13;
            this.lblName.Text = "Layer Name:";
            // 
            // tbLayerName
            // 
            this.tbLayerName.Location = new System.Drawing.Point(64, 6);
            this.tbLayerName.Name = "tbLayerName";
            this.tbLayerName.ReadOnly = true;
            this.tbLayerName.Size = new System.Drawing.Size(163, 20);
            this.tbLayerName.TabIndex = 14;
            // 
            // lblLabels
            // 
            this.lblLabels.AutoSize = true;
            this.lblLabels.Location = new System.Drawing.Point(128, 28);
            this.lblLabels.Name = "lblLabels";
            this.lblLabels.Size = new System.Drawing.Size(77, 13);
            this.lblLabels.TabIndex = 15;
            this.lblLabels.Text = "Labels On/Off:";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(152, 192);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 16;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(153, 221);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSymbol
            // 
            this.btnSymbol.BackColor = System.Drawing.Color.Transparent;
            this.btnSymbol.Location = new System.Drawing.Point(105, 191);
            this.btnSymbol.Name = "btnSymbol";
            this.btnSymbol.Size = new System.Drawing.Size(28, 24);
            this.btnSymbol.TabIndex = 19;
            this.btnSymbol.Text = "...";
            this.btnSymbol.UseVisualStyleBackColor = false;
            this.btnSymbol.Click += new System.EventHandler(this.btnSymbol_Click);
            // 
            // lblLayerSymbol
            // 
            this.lblLayerSymbol.AutoSize = true;
            this.lblLayerSymbol.Location = new System.Drawing.Point(1, 197);
            this.lblLayerSymbol.Name = "lblLayerSymbol";
            this.lblLayerSymbol.Size = new System.Drawing.Size(98, 13);
            this.lblLayerSymbol.TabIndex = 18;
            this.lblLayerSymbol.Text = "Set custom symbol:";
            // 
            // imageListDefaultSymbols
            // 
            this.imageListDefaultSymbols.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListDefaultSymbols.ImageStream")));
            this.imageListDefaultSymbols.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListDefaultSymbols.Images.SetKeyName(0, "redDiamond.png");
            this.imageListDefaultSymbols.Images.SetKeyName(1, "circle.png");
            this.imageListDefaultSymbols.Images.SetKeyName(2, "sign1.png");
            // 
            // imageListLines
            // 
            this.imageListLines.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListLines.ImageStream")));
            this.imageListLines.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListLines.Images.SetKeyName(0, "lineSmall.png");
            this.imageListLines.Images.SetKeyName(1, "lineMedium.png");
            this.imageListLines.Images.SetKeyName(2, "lineLarge.png");
            this.imageListLines.Images.SetKeyName(3, "currentLine.png");
            // 
            // imageComboBoxSymbols
            // 
            this.imageComboBoxSymbols.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.imageComboBoxSymbols.ImageList = this.imageListDefaultSymbols;
            this.imageComboBoxSymbols.Indent = 0;
            this.imageComboBoxSymbols.ItemHeight = 15;
            imageComboBoxItem1.Font = null;
            imageComboBoxItem1.Image = "0";
            imageComboBoxItem1.ImageIndex = 0;
            imageComboBoxItem1.IndentLevel = 0;
            imageComboBoxItem1.Item = null;
            imageComboBoxItem1.Text = "Diamond";
            imageComboBoxItem2.Font = null;
            imageComboBoxItem2.Image = "1";
            imageComboBoxItem2.ImageIndex = 1;
            imageComboBoxItem2.IndentLevel = 0;
            imageComboBoxItem2.Item = null;
            imageComboBoxItem2.Text = "Circle";
            imageComboBoxItem3.Font = null;
            imageComboBoxItem3.Image = "2";
            imageComboBoxItem3.ImageIndex = 2;
            imageComboBoxItem3.IndentLevel = 0;
            imageComboBoxItem3.Item = null;
            imageComboBoxItem3.Text = "Tree";
            this.imageComboBoxSymbols.Items.AddRange(new ImageComboBox.ImageComboBoxItem[] {
            imageComboBoxItem1,
            imageComboBoxItem2,
            imageComboBoxItem3});
            this.imageComboBoxSymbols.Location = new System.Drawing.Point(65, 62);
            this.imageComboBoxSymbols.Name = "imageComboBoxSymbols";
            this.imageComboBoxSymbols.Size = new System.Drawing.Size(162, 21);
            this.imageComboBoxSymbols.TabIndex = 20;
            this.imageComboBoxSymbols.SelectedIndexChanged += new System.EventHandler(this.imageComboBoxSymbols_SelectedIndexChanged);
            // 
            // imageComboBoxLineThickness
            // 
            this.imageComboBoxLineThickness.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.imageComboBoxLineThickness.ImageList = this.imageListLines;
            this.imageComboBoxLineThickness.Indent = 0;
            this.imageComboBoxLineThickness.ItemHeight = 15;
            imageComboBoxItem4.Font = null;
            imageComboBoxItem4.Image = "0";
            imageComboBoxItem4.ImageIndex = 0;
            imageComboBoxItem4.IndentLevel = 0;
            imageComboBoxItem4.Item = null;
            imageComboBoxItem4.Text = "1";
            imageComboBoxItem5.Font = null;
            imageComboBoxItem5.Image = "1";
            imageComboBoxItem5.ImageIndex = 1;
            imageComboBoxItem5.IndentLevel = 0;
            imageComboBoxItem5.Item = null;
            imageComboBoxItem5.Text = "2";
            imageComboBoxItem6.Font = null;
            imageComboBoxItem6.Image = "2";
            imageComboBoxItem6.ImageIndex = 2;
            imageComboBoxItem6.IndentLevel = 0;
            imageComboBoxItem6.Item = null;
            imageComboBoxItem6.Text = "3";
            imageComboBoxItem7.Font = null;
            imageComboBoxItem7.Image = "(none)";
            imageComboBoxItem7.ImageIndex = -1;
            imageComboBoxItem7.IndentLevel = 0;
            imageComboBoxItem7.Item = null;
            imageComboBoxItem7.Text = "Default";
            this.imageComboBoxLineThickness.Items.AddRange(new ImageComboBox.ImageComboBoxItem[] {
            imageComboBoxItem4,
            imageComboBoxItem5,
            imageComboBoxItem6,
            imageComboBoxItem7});
            this.imageComboBoxLineThickness.Location = new System.Drawing.Point(66, 89);
            this.imageComboBoxLineThickness.Name = "imageComboBoxLineThickness";
            this.imageComboBoxLineThickness.Size = new System.Drawing.Size(162, 21);
            this.imageComboBoxLineThickness.TabIndex = 21;
            this.imageComboBoxLineThickness.SelectedIndexChanged += new System.EventHandler(this.imageComboBoxLineThickness_SelectedIndexChanged);
            // 
            // lblSymbol
            // 
            this.lblSymbol.AutoSize = true;
            this.lblSymbol.Location = new System.Drawing.Point(0, 70);
            this.lblSymbol.Name = "lblSymbol";
            this.lblSymbol.Size = new System.Drawing.Size(44, 13);
            this.lblSymbol.TabIndex = 22;
            this.lblSymbol.Text = "Symbol:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Size:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Color:";
            // 
            // btnColor
            // 
            this.btnColor.BackColor = System.Drawing.Color.Transparent;
            this.btnColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnColor.Location = new System.Drawing.Point(65, 116);
            this.btnColor.Name = "btnColor";
            this.btnColor.Size = new System.Drawing.Size(163, 24);
            this.btnColor.TabIndex = 26;
            this.btnColor.UseVisualStyleBackColor = false;
            this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
            // 
            // FormLayerProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(229, 252);
            this.Controls.Add(this.btnColor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblSymbol);
            this.Controls.Add(this.imageComboBoxLineThickness);
            this.Controls.Add(this.imageComboBoxSymbols);
            this.Controls.Add(this.btnSymbol);
            this.Controls.Add(this.lblLayerSymbol);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblLabels);
            this.Controls.Add(this.tbLayerName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.chkboxLabels);
            this.Name = "FormLayerProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Layer Properties";
            this.Load += new System.EventHandler(this.FormLayerProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.CheckBox chkboxLabels;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox tbLayerName;
        private System.Windows.Forms.Label lblLabels;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnSymbol;
		private System.Windows.Forms.Label lblLayerSymbol;
		private System.Windows.Forms.ImageList imageListDefaultSymbols;
		private System.Windows.Forms.ImageList imageListLines;
		private ImageComboBox.ImageComboBox imageComboBoxSymbols;
		private ImageComboBox.ImageComboBox imageComboBoxLineThickness;
		private System.Windows.Forms.Label lblSymbol;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnColor;
    }
}