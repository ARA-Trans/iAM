namespace RoadCare3
{
    partial class FormSegmentationCriteria
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
			this.listBoxValue = new System.Windows.Forms.ListBox();
			this.buttonNotEqual = new System.Windows.Forms.Button();
			this.buttonOR = new System.Windows.Forms.Button();
			this.buttonAnd = new System.Windows.Forms.Button();
			this.buttonGreaterThan = new System.Windows.Forms.Button();
			this.buttonGreaterEqual = new System.Windows.Forms.Button();
			this.buttonLessEqual = new System.Windows.Forms.Button();
			this.buttonLessThan = new System.Windows.Forms.Button();
			this.buttonEqual = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxSubsetName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxAttribute = new System.Windows.Forms.ComboBox();
			this.listBoxField = new System.Windows.Forms.ListBox();
			this.buttonAnychange = new System.Windows.Forms.Button();
			this.buttonCheck = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxSearch = new System.Windows.Forms.TextBox();
			this.labelResult = new System.Windows.Forms.Label();
			this.buttonAnyRecord = new System.Windows.Forms.Button();
			this.btnAnyYear = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listBoxValue
			// 
			this.listBoxValue.FormattingEnabled = true;
			this.listBoxValue.Location = new System.Drawing.Point(361, 40);
			this.listBoxValue.Margin = new System.Windows.Forms.Padding(2);
			this.listBoxValue.Name = "listBoxValue";
			this.listBoxValue.Size = new System.Drawing.Size(258, 290);
			this.listBoxValue.TabIndex = 3;
			this.listBoxValue.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxValue_MouseDoubleClick);
			// 
			// buttonNotEqual
			// 
			this.buttonNotEqual.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonNotEqual.Location = new System.Drawing.Point(278, 119);
			this.buttonNotEqual.Margin = new System.Windows.Forms.Padding(2);
			this.buttonNotEqual.Name = "buttonNotEqual";
			this.buttonNotEqual.Size = new System.Drawing.Size(56, 28);
			this.buttonNotEqual.TabIndex = 24;
			this.buttonNotEqual.Text = "<>";
			this.buttonNotEqual.UseVisualStyleBackColor = true;
			this.buttonNotEqual.Click += new System.EventHandler(this.buttonNotEqual_Click);
			// 
			// buttonOR
			// 
			this.buttonOR.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonOR.Location = new System.Drawing.Point(278, 251);
			this.buttonOR.Margin = new System.Windows.Forms.Padding(2);
			this.buttonOR.Name = "buttonOR";
			this.buttonOR.Size = new System.Drawing.Size(56, 28);
			this.buttonOR.TabIndex = 23;
			this.buttonOR.Text = "OR";
			this.buttonOR.UseVisualStyleBackColor = true;
			this.buttonOR.Click += new System.EventHandler(this.buttonOR_Click);
			// 
			// buttonAnd
			// 
			this.buttonAnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonAnd.Location = new System.Drawing.Point(217, 251);
			this.buttonAnd.Margin = new System.Windows.Forms.Padding(2);
			this.buttonAnd.Name = "buttonAnd";
			this.buttonAnd.Size = new System.Drawing.Size(56, 28);
			this.buttonAnd.TabIndex = 22;
			this.buttonAnd.Text = "AND";
			this.buttonAnd.UseVisualStyleBackColor = true;
			this.buttonAnd.Click += new System.EventHandler(this.buttonAnd_Click);
			// 
			// buttonGreaterThan
			// 
			this.buttonGreaterThan.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonGreaterThan.Location = new System.Drawing.Point(278, 207);
			this.buttonGreaterThan.Margin = new System.Windows.Forms.Padding(2);
			this.buttonGreaterThan.Name = "buttonGreaterThan";
			this.buttonGreaterThan.Size = new System.Drawing.Size(56, 28);
			this.buttonGreaterThan.TabIndex = 21;
			this.buttonGreaterThan.Text = "<";
			this.buttonGreaterThan.UseVisualStyleBackColor = true;
			this.buttonGreaterThan.Click += new System.EventHandler(this.buttonGreaterThan_Click);
			// 
			// buttonGreaterEqual
			// 
			this.buttonGreaterEqual.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonGreaterEqual.Location = new System.Drawing.Point(217, 207);
			this.buttonGreaterEqual.Margin = new System.Windows.Forms.Padding(2);
			this.buttonGreaterEqual.Name = "buttonGreaterEqual";
			this.buttonGreaterEqual.Size = new System.Drawing.Size(56, 28);
			this.buttonGreaterEqual.TabIndex = 20;
			this.buttonGreaterEqual.Text = "<=";
			this.buttonGreaterEqual.UseVisualStyleBackColor = true;
			this.buttonGreaterEqual.Click += new System.EventHandler(this.buttonGreaterEqual_Click);
			// 
			// buttonLessEqual
			// 
			this.buttonLessEqual.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonLessEqual.Location = new System.Drawing.Point(217, 162);
			this.buttonLessEqual.Margin = new System.Windows.Forms.Padding(2);
			this.buttonLessEqual.Name = "buttonLessEqual";
			this.buttonLessEqual.Size = new System.Drawing.Size(56, 28);
			this.buttonLessEqual.TabIndex = 19;
			this.buttonLessEqual.Text = ">=";
			this.buttonLessEqual.UseVisualStyleBackColor = true;
			this.buttonLessEqual.Click += new System.EventHandler(this.buttonLessEqual_Click);
			// 
			// buttonLessThan
			// 
			this.buttonLessThan.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonLessThan.Location = new System.Drawing.Point(278, 162);
			this.buttonLessThan.Margin = new System.Windows.Forms.Padding(2);
			this.buttonLessThan.Name = "buttonLessThan";
			this.buttonLessThan.Size = new System.Drawing.Size(56, 28);
			this.buttonLessThan.TabIndex = 18;
			this.buttonLessThan.Text = ">";
			this.buttonLessThan.UseVisualStyleBackColor = true;
			this.buttonLessThan.Click += new System.EventHandler(this.buttonLessThan_Click);
			// 
			// buttonEqual
			// 
			this.buttonEqual.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonEqual.Location = new System.Drawing.Point(217, 119);
			this.buttonEqual.Margin = new System.Windows.Forms.Padding(2);
			this.buttonEqual.Name = "buttonEqual";
			this.buttonEqual.Size = new System.Drawing.Size(56, 28);
			this.buttonEqual.TabIndex = 17;
			this.buttonEqual.Text = "=";
			this.buttonEqual.UseVisualStyleBackColor = true;
			this.buttonEqual.Click += new System.EventHandler(this.buttonEqual_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 11);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(74, 13);
			this.label1.TabIndex = 25;
			this.label1.Text = "Subset Name:";
			// 
			// textBoxSubsetName
			// 
			this.textBoxSubsetName.Location = new System.Drawing.Point(12, 32);
			this.textBoxSubsetName.Margin = new System.Windows.Forms.Padding(2);
			this.textBoxSubsetName.Name = "textBoxSubsetName";
			this.textBoxSubsetName.Size = new System.Drawing.Size(176, 20);
			this.textBoxSubsetName.TabIndex = 26;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 58);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(49, 13);
			this.label2.TabIndex = 27;
			this.label2.Text = "Attribute:";
			// 
			// comboBoxAttribute
			// 
			this.comboBoxAttribute.FormattingEnabled = true;
			this.comboBoxAttribute.Location = new System.Drawing.Point(12, 76);
			this.comboBoxAttribute.Margin = new System.Windows.Forms.Padding(2);
			this.comboBoxAttribute.Name = "comboBoxAttribute";
			this.comboBoxAttribute.Size = new System.Drawing.Size(174, 21);
			this.comboBoxAttribute.TabIndex = 28;
			this.comboBoxAttribute.SelectedIndexChanged += new System.EventHandler(this.comboBoxAttribute_SelectedIndexChanged);
			// 
			// listBoxField
			// 
			this.listBoxField.FormattingEnabled = true;
			this.listBoxField.Location = new System.Drawing.Point(14, 126);
			this.listBoxField.Margin = new System.Windows.Forms.Padding(2);
			this.listBoxField.Name = "listBoxField";
			this.listBoxField.Size = new System.Drawing.Size(174, 199);
			this.listBoxField.TabIndex = 29;
			this.listBoxField.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxField_MouseDoubleClick);
			this.listBoxField.SelectedIndexChanged += new System.EventHandler(this.listBoxField_SelectedIndexChanged);
			// 
			// buttonAnychange
			// 
			this.buttonAnychange.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonAnychange.Location = new System.Drawing.Point(217, 53);
			this.buttonAnychange.Margin = new System.Windows.Forms.Padding(2);
			this.buttonAnychange.Name = "buttonAnychange";
			this.buttonAnychange.Size = new System.Drawing.Size(117, 28);
			this.buttonAnychange.TabIndex = 30;
			this.buttonAnychange.Text = "Anychange";
			this.buttonAnychange.UseVisualStyleBackColor = true;
			this.buttonAnychange.Click += new System.EventHandler(this.buttonAnychange_Click);
			// 
			// buttonCheck
			// 
			this.buttonCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonCheck.Location = new System.Drawing.Point(17, 344);
			this.buttonCheck.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCheck.Name = "buttonCheck";
			this.buttonCheck.Size = new System.Drawing.Size(65, 27);
			this.buttonCheck.TabIndex = 33;
			this.buttonCheck.Text = "Check";
			this.buttonCheck.UseVisualStyleBackColor = true;
			this.buttonCheck.Click += new System.EventHandler(this.buttonCheck_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonCancel.Location = new System.Drawing.Point(187, 344);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(76, 27);
			this.buttonCancel.TabIndex = 32;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonOK.Location = new System.Drawing.Point(101, 344);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(2);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(65, 27);
			this.buttonOK.TabIndex = 31;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 105);
			this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(37, 13);
			this.label3.TabIndex = 34;
			this.label3.Text = "Fields:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(361, 21);
			this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(42, 13);
			this.label4.TabIndex = 35;
			this.label4.Text = "Values:";
			// 
			// textBoxSearch
			// 
			this.textBoxSearch.Location = new System.Drawing.Point(9, 388);
			this.textBoxSearch.Margin = new System.Windows.Forms.Padding(2);
			this.textBoxSearch.Multiline = true;
			this.textBoxSearch.Name = "textBoxSearch";
			this.textBoxSearch.Size = new System.Drawing.Size(623, 62);
			this.textBoxSearch.TabIndex = 36;
			// 
			// labelResult
			// 
			this.labelResult.AutoSize = true;
			this.labelResult.Location = new System.Drawing.Point(361, 353);
			this.labelResult.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelResult.Name = "labelResult";
			this.labelResult.Size = new System.Drawing.Size(35, 13);
			this.labelResult.TabIndex = 37;
			this.labelResult.Text = "label5";
			this.labelResult.Visible = false;
			// 
			// buttonAnyRecord
			// 
			this.buttonAnyRecord.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonAnyRecord.Location = new System.Drawing.Point(217, 21);
			this.buttonAnyRecord.Margin = new System.Windows.Forms.Padding(2);
			this.buttonAnyRecord.Name = "buttonAnyRecord";
			this.buttonAnyRecord.Size = new System.Drawing.Size(117, 28);
			this.buttonAnyRecord.TabIndex = 38;
			this.buttonAnyRecord.Text = "Anyrecord";
			this.buttonAnyRecord.UseVisualStyleBackColor = true;
			this.buttonAnyRecord.Click += new System.EventHandler(this.buttonAnyRecord_Click);
			// 
			// btnAnyYear
			// 
			this.btnAnyYear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnAnyYear.Location = new System.Drawing.Point(217, 85);
			this.btnAnyYear.Margin = new System.Windows.Forms.Padding(2);
			this.btnAnyYear.Name = "btnAnyYear";
			this.btnAnyYear.Size = new System.Drawing.Size(117, 28);
			this.btnAnyYear.TabIndex = 39;
			this.btnAnyYear.Text = "Any Year";
			this.btnAnyYear.UseVisualStyleBackColor = true;
			this.btnAnyYear.Click += new System.EventHandler(this.btnAnyYear_Click);
			// 
			// FormSegmentationCriteria
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(640, 460);
			this.Controls.Add(this.btnAnyYear);
			this.Controls.Add(this.buttonAnyRecord);
			this.Controls.Add(this.labelResult);
			this.Controls.Add(this.textBoxSearch);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.buttonCheck);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonAnychange);
			this.Controls.Add(this.listBoxField);
			this.Controls.Add(this.comboBoxAttribute);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxSubsetName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonNotEqual);
			this.Controls.Add(this.buttonOR);
			this.Controls.Add(this.buttonAnd);
			this.Controls.Add(this.buttonGreaterThan);
			this.Controls.Add(this.buttonGreaterEqual);
			this.Controls.Add(this.buttonLessEqual);
			this.Controls.Add(this.buttonLessThan);
			this.Controls.Add(this.buttonEqual);
			this.Controls.Add(this.listBoxValue);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "FormSegmentationCriteria";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Segmentation Criteria";
			this.Load += new System.EventHandler(this.FormSegmentationCriteria_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxValue;
        private System.Windows.Forms.Button buttonNotEqual;
        private System.Windows.Forms.Button buttonOR;
        private System.Windows.Forms.Button buttonAnd;
        private System.Windows.Forms.Button buttonGreaterThan;
        private System.Windows.Forms.Button buttonGreaterEqual;
        private System.Windows.Forms.Button buttonLessEqual;
        private System.Windows.Forms.Button buttonLessThan;
        private System.Windows.Forms.Button buttonEqual;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSubsetName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxAttribute;
        private System.Windows.Forms.ListBox listBoxField;
        private System.Windows.Forms.Button buttonAnychange;
        private System.Windows.Forms.Button buttonCheck;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.Button buttonAnyRecord;
		private System.Windows.Forms.Button btnAnyYear;
    }
}