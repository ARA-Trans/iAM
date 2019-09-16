namespace RoadCare3
{
    partial class FormImportAsset
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
            this.tbShapeFilePath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lbFieldNames = new System.Windows.Forms.ListBox();
            this.lbAssetFields = new System.Windows.Forms.ListBox();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.tbSelect = new System.Windows.Forms.TextBox();
            this.tbNewField = new System.Windows.Forms.TextBox();
            this.btnAddNewAttribute = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.chkBoxImportData = new System.Windows.Forms.CheckBox();
            this.cbDataType = new System.Windows.Forms.ComboBox();
            this.gbNewAttribute = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tbAssetName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSQL = new System.Windows.Forms.Button();
            this.gbNewAttribute.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbShapeFilePath
            // 
            this.tbShapeFilePath.Location = new System.Drawing.Point(12, 50);
            this.tbShapeFilePath.Name = "tbShapeFilePath";
            this.tbShapeFilePath.ReadOnly = true;
            this.tbShapeFilePath.Size = new System.Drawing.Size(339, 20);
            this.tbShapeFilePath.TabIndex = 0;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(352, 49);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(28, 21);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lbFieldNames
            // 
            this.lbFieldNames.FormattingEnabled = true;
            this.lbFieldNames.Location = new System.Drawing.Point(12, 87);
            this.lbFieldNames.Name = "lbFieldNames";
            this.lbFieldNames.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFieldNames.Size = new System.Drawing.Size(160, 238);
            this.lbFieldNames.TabIndex = 2;
            // 
            // lbAssetFields
            // 
            this.lbAssetFields.FormattingEnabled = true;
            this.lbAssetFields.Location = new System.Drawing.Point(220, 89);
            this.lbAssetFields.Name = "lbAssetFields";
            this.lbAssetFields.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbAssetFields.Size = new System.Drawing.Size(160, 316);
            this.lbAssetFields.TabIndex = 3;
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Image = global::RoadCare3.Properties.Resources.ARW04UP;
            this.btnMoveUp.Location = new System.Drawing.Point(178, 137);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(38, 24);
            this.btnMoveUp.TabIndex = 4;
            this.btnMoveUp.Text = "...";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Image = global::RoadCare3.Properties.Resources.ARW04DN;
            this.btnMoveDown.Location = new System.Drawing.Point(178, 162);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(38, 24);
            this.btnMoveDown.TabIndex = 5;
            this.btnMoveDown.Text = "...";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // tbSelect
            // 
            this.tbSelect.Location = new System.Drawing.Point(15, 425);
            this.tbSelect.Multiline = true;
            this.tbSelect.Name = "tbSelect";
            this.tbSelect.Size = new System.Drawing.Size(368, 74);
            this.tbSelect.TabIndex = 6;
            // 
            // tbNewField
            // 
            this.tbNewField.Location = new System.Drawing.Point(6, 19);
            this.tbNewField.Name = "tbNewField";
            this.tbNewField.Size = new System.Drawing.Size(148, 20);
            this.tbNewField.TabIndex = 7;
            // 
            // btnAddNewAttribute
            // 
            this.btnAddNewAttribute.Image = global::RoadCare3.Properties.Resources.addLayer;
            this.btnAddNewAttribute.Location = new System.Drawing.Point(178, 340);
            this.btnAddNewAttribute.Name = "btnAddNewAttribute";
            this.btnAddNewAttribute.Size = new System.Drawing.Size(38, 24);
            this.btnAddNewAttribute.TabIndex = 8;
            this.btnAddNewAttribute.Text = "...";
            this.btnAddNewAttribute.UseVisualStyleBackColor = true;
            this.btnAddNewAttribute.Click += new System.EventHandler(this.btnAddNewAttribute_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemove.Location = new System.Drawing.Point(176, 250);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(38, 24);
            this.btnRemove.TabIndex = 10;
            this.btnRemove.Text = "<";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.Location = new System.Drawing.Point(176, 226);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(38, 24);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.Text = ">";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // chkBoxImportData
            // 
            this.chkBoxImportData.AutoSize = true;
            this.chkBoxImportData.Location = new System.Drawing.Point(204, 501);
            this.chkBoxImportData.Name = "chkBoxImportData";
            this.chkBoxImportData.Size = new System.Drawing.Size(179, 17);
            this.chkBoxImportData.TabIndex = 11;
            this.chkBoxImportData.Text = "Import data after Asset Add New";
            this.chkBoxImportData.UseVisualStyleBackColor = true;
            // 
            // cbDataType
            // 
            this.cbDataType.FormattingEnabled = true;
            this.cbDataType.Items.AddRange(new object[] {
            "bigint",
            "binary(50)",
            "bit",
            "char(10)",
            "datetime",
            "decimal(18, 0)",
            "float",
            "image",
            "int",
            "money",
            "nchar(10)",
            "ntext",
            "numeric(18, 0)",
            "nvarchar(50)",
            "nvarchar(MAX)",
            "real",
            "smalldatetime",
            "smallint",
            "smallmoney",
            "text",
            "timestamp",
            "tinyint",
            "uniqueidentifier",
            "varbinary(50)",
            "varbinary(MAX)",
            "varchar(50)",
            "varchar(MAX)"});
            this.cbDataType.Location = new System.Drawing.Point(6, 46);
            this.cbDataType.Name = "cbDataType";
            this.cbDataType.Size = new System.Drawing.Size(148, 21);
            this.cbDataType.TabIndex = 12;
            // 
            // gbNewAttribute
            // 
            this.gbNewAttribute.Controls.Add(this.cbDataType);
            this.gbNewAttribute.Controls.Add(this.tbNewField);
            this.gbNewAttribute.Location = new System.Drawing.Point(12, 325);
            this.gbNewAttribute.Name = "gbNewAttribute";
            this.gbNewAttribute.Size = new System.Drawing.Size(160, 80);
            this.gbNewAttribute.TabIndex = 13;
            this.gbNewAttribute.TabStop = false;
            this.gbNewAttribute.Text = "New Attribute";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Shapefile path:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Shapefile fields:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(217, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Asset Attributes:";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(15, 501);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 17;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(91, 501);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 409);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Select statement:";
            // 
            // tbAssetName
            // 
            this.tbAssetName.Location = new System.Drawing.Point(130, 6);
            this.tbAssetName.Name = "tbAssetName";
            this.tbAssetName.Size = new System.Drawing.Size(112, 20);
            this.tbAssetName.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(115, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "New asset table name:";
            // 
            // btnSQL
            // 
            this.btnSQL.Location = new System.Drawing.Point(178, 368);
            this.btnSQL.Name = "btnSQL";
            this.btnSQL.Size = new System.Drawing.Size(38, 24);
            this.btnSQL.TabIndex = 22;
            this.btnSQL.Text = "SQL";
            this.btnSQL.UseVisualStyleBackColor = true;
            this.btnSQL.Click += new System.EventHandler(this.btnSQL_Click);
            // 
            // FormImportAsset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 526);
            this.Controls.Add(this.btnSQL);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbAssetName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gbNewAttribute);
            this.Controls.Add(this.btnAddNewAttribute);
            this.Controls.Add(this.chkBoxImportData);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.tbSelect);
            this.Controls.Add(this.btnMoveDown);
            this.Controls.Add(this.btnMoveUp);
            this.Controls.Add(this.lbAssetFields);
            this.Controls.Add(this.lbFieldNames);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.tbShapeFilePath);
            this.Name = "FormImportAsset";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create asset table from shapefile";
            this.Load += new System.EventHandler(this.FormImportAsset_Load);
            this.gbNewAttribute.ResumeLayout(false);
            this.gbNewAttribute.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbShapeFilePath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.ListBox lbFieldNames;
        private System.Windows.Forms.ListBox lbAssetFields;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.TextBox tbSelect;
        private System.Windows.Forms.TextBox tbNewField;
        private System.Windows.Forms.Button btnAddNewAttribute;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.CheckBox chkBoxImportData;
        private System.Windows.Forms.ComboBox cbDataType;
        private System.Windows.Forms.GroupBox gbNewAttribute;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbAssetName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSQL;
    }
}
