namespace RoadCare3
{
	partial class FormHPMSFilter
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
			this.buttonNotEqual = new System.Windows.Forms.Button();
			this.buttonOR = new System.Windows.Forms.Button();
			this.buttonAnd = new System.Windows.Forms.Button();
			this.buttonGreaterThan = new System.Windows.Forms.Button();
			this.buttonGreaterEqual = new System.Windows.Forms.Button();
			this.buttonLessEqual = new System.Windows.Forms.Button();
			this.buttonLessThan = new System.Windows.Forms.Button();
			this.buttonEqual = new System.Windows.Forms.Button();
			this.labelReturn = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonCheck = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.listBoxValue = new System.Windows.Forms.ListBox();
			this.listBoxField = new System.Windows.Forms.ListBox();
			this.textBoxSearch = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// buttonNotEqual
			// 
			this.buttonNotEqual.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonNotEqual.Location = new System.Drawing.Point(219, 39);
			this.buttonNotEqual.Margin = new System.Windows.Forms.Padding(2);
			this.buttonNotEqual.Name = "buttonNotEqual";
			this.buttonNotEqual.Size = new System.Drawing.Size(56, 28);
			this.buttonNotEqual.TabIndex = 33;
			this.buttonNotEqual.Text = "<>";
			this.buttonNotEqual.UseVisualStyleBackColor = true;
			this.buttonNotEqual.Click += new System.EventHandler(this.buttonNotEqual_Click);
			// 
			// buttonOR
			// 
			this.buttonOR.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonOR.Location = new System.Drawing.Point(219, 170);
			this.buttonOR.Margin = new System.Windows.Forms.Padding(2);
			this.buttonOR.Name = "buttonOR";
			this.buttonOR.Size = new System.Drawing.Size(56, 28);
			this.buttonOR.TabIndex = 32;
			this.buttonOR.Text = "OR";
			this.buttonOR.UseVisualStyleBackColor = true;
			this.buttonOR.Click += new System.EventHandler(this.buttonOR_Click);
			// 
			// buttonAnd
			// 
			this.buttonAnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonAnd.Location = new System.Drawing.Point(158, 170);
			this.buttonAnd.Margin = new System.Windows.Forms.Padding(2);
			this.buttonAnd.Name = "buttonAnd";
			this.buttonAnd.Size = new System.Drawing.Size(56, 28);
			this.buttonAnd.TabIndex = 31;
			this.buttonAnd.Text = "AND";
			this.buttonAnd.UseVisualStyleBackColor = true;
			this.buttonAnd.Click += new System.EventHandler(this.buttonAnd_Click);
			// 
			// buttonGreaterThan
			// 
			this.buttonGreaterThan.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonGreaterThan.Location = new System.Drawing.Point(219, 126);
			this.buttonGreaterThan.Margin = new System.Windows.Forms.Padding(2);
			this.buttonGreaterThan.Name = "buttonGreaterThan";
			this.buttonGreaterThan.Size = new System.Drawing.Size(56, 28);
			this.buttonGreaterThan.TabIndex = 30;
			this.buttonGreaterThan.Text = "<";
			this.buttonGreaterThan.UseVisualStyleBackColor = true;
			this.buttonGreaterThan.Click += new System.EventHandler(this.buttonGreaterThan_Click);
			// 
			// buttonGreaterEqual
			// 
			this.buttonGreaterEqual.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonGreaterEqual.Location = new System.Drawing.Point(158, 126);
			this.buttonGreaterEqual.Margin = new System.Windows.Forms.Padding(2);
			this.buttonGreaterEqual.Name = "buttonGreaterEqual";
			this.buttonGreaterEqual.Size = new System.Drawing.Size(56, 28);
			this.buttonGreaterEqual.TabIndex = 29;
			this.buttonGreaterEqual.Text = "<=";
			this.buttonGreaterEqual.UseVisualStyleBackColor = true;
			this.buttonGreaterEqual.Click += new System.EventHandler(this.buttonGreaterEqual_Click);
			// 
			// buttonLessEqual
			// 
			this.buttonLessEqual.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonLessEqual.Location = new System.Drawing.Point(158, 82);
			this.buttonLessEqual.Margin = new System.Windows.Forms.Padding(2);
			this.buttonLessEqual.Name = "buttonLessEqual";
			this.buttonLessEqual.Size = new System.Drawing.Size(56, 28);
			this.buttonLessEqual.TabIndex = 28;
			this.buttonLessEqual.Text = ">=";
			this.buttonLessEqual.UseVisualStyleBackColor = true;
			this.buttonLessEqual.Click += new System.EventHandler(this.buttonLessEqual_Click);
			// 
			// buttonLessThan
			// 
			this.buttonLessThan.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonLessThan.Location = new System.Drawing.Point(219, 82);
			this.buttonLessThan.Margin = new System.Windows.Forms.Padding(2);
			this.buttonLessThan.Name = "buttonLessThan";
			this.buttonLessThan.Size = new System.Drawing.Size(56, 28);
			this.buttonLessThan.TabIndex = 27;
			this.buttonLessThan.Text = ">";
			this.buttonLessThan.UseVisualStyleBackColor = true;
			this.buttonLessThan.Click += new System.EventHandler(this.buttonLessThan_Click);
			// 
			// buttonEqual
			// 
			this.buttonEqual.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonEqual.Location = new System.Drawing.Point(158, 39);
			this.buttonEqual.Margin = new System.Windows.Forms.Padding(2);
			this.buttonEqual.Name = "buttonEqual";
			this.buttonEqual.Size = new System.Drawing.Size(56, 28);
			this.buttonEqual.TabIndex = 26;
			this.buttonEqual.Text = "=";
			this.buttonEqual.UseVisualStyleBackColor = true;
			this.buttonEqual.Click += new System.EventHandler(this.buttonEqual_Click);
			// 
			// labelReturn
			// 
			this.labelReturn.AutoSize = true;
			this.labelReturn.Location = new System.Drawing.Point(357, 283);
			this.labelReturn.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelReturn.Name = "labelReturn";
			this.labelReturn.Size = new System.Drawing.Size(107, 13);
			this.labelReturn.TabIndex = 25;
			this.labelReturn.Text = "3240 entries returned";
			this.labelReturn.Visible = false;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(11, 9);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(34, 13);
			this.label2.TabIndex = 24;
			this.label2.Text = "Fields";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(291, 9);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 13);
			this.label1.TabIndex = 23;
			this.label1.Text = "Attribute Value:";
			// 
			// buttonCheck
			// 
			this.buttonCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonCheck.Location = new System.Drawing.Point(33, 276);
			this.buttonCheck.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCheck.Name = "buttonCheck";
			this.buttonCheck.Size = new System.Drawing.Size(65, 27);
			this.buttonCheck.TabIndex = 22;
			this.buttonCheck.Text = "Check";
			this.buttonCheck.UseVisualStyleBackColor = true;
			this.buttonCheck.Click += new System.EventHandler(this.buttonCheck_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonCancel.Location = new System.Drawing.Point(203, 276);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(72, 27);
			this.buttonCancel.TabIndex = 21;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonOK.Location = new System.Drawing.Point(118, 276);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(2);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(65, 27);
			this.buttonOK.TabIndex = 20;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// listBoxValue
			// 
			this.listBoxValue.FormattingEnabled = true;
			this.listBoxValue.Location = new System.Drawing.Point(291, 26);
			this.listBoxValue.Margin = new System.Windows.Forms.Padding(2);
			this.listBoxValue.Name = "listBoxValue";
			this.listBoxValue.Size = new System.Drawing.Size(258, 238);
			this.listBoxValue.TabIndex = 19;
			this.listBoxValue.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxValue_MouseDoubleClick);
			// 
			// listBoxField
			// 
			this.listBoxField.FormattingEnabled = true;
			this.listBoxField.Location = new System.Drawing.Point(11, 26);
			this.listBoxField.Margin = new System.Windows.Forms.Padding(2);
			this.listBoxField.Name = "listBoxField";
			this.listBoxField.Size = new System.Drawing.Size(138, 238);
			this.listBoxField.TabIndex = 18;
			this.listBoxField.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxField_MouseDoubleClick);
			this.listBoxField.SelectedIndexChanged += new System.EventHandler(this.listBoxField_SelectedIndexChanged);
			// 
			// textBoxSearch
			// 
			this.textBoxSearch.Location = new System.Drawing.Point(11, 309);
			this.textBoxSearch.Margin = new System.Windows.Forms.Padding(2);
			this.textBoxSearch.Multiline = true;
			this.textBoxSearch.Name = "textBoxSearch";
			this.textBoxSearch.Size = new System.Drawing.Size(538, 66);
			this.textBoxSearch.TabIndex = 17;
			// 
			// FormHPMSFilter
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(558, 385);
			this.Controls.Add(this.buttonNotEqual);
			this.Controls.Add(this.buttonOR);
			this.Controls.Add(this.buttonAnd);
			this.Controls.Add(this.buttonGreaterThan);
			this.Controls.Add(this.buttonGreaterEqual);
			this.Controls.Add(this.buttonLessEqual);
			this.Controls.Add(this.buttonLessThan);
			this.Controls.Add(this.buttonEqual);
			this.Controls.Add(this.labelReturn);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonCheck);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.listBoxValue);
			this.Controls.Add(this.listBoxField);
			this.Controls.Add(this.textBoxSearch);
			this.Name = "FormHPMSFilter";
			this.Text = "FormHPMSFilter";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonNotEqual;
		private System.Windows.Forms.Button buttonOR;
		private System.Windows.Forms.Button buttonAnd;
		private System.Windows.Forms.Button buttonGreaterThan;
		private System.Windows.Forms.Button buttonGreaterEqual;
		private System.Windows.Forms.Button buttonLessEqual;
		private System.Windows.Forms.Button buttonLessThan;
		private System.Windows.Forms.Button buttonEqual;
		private System.Windows.Forms.Label labelReturn;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonCheck;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.ListBox listBoxValue;
		private System.Windows.Forms.ListBox listBoxField;
		private System.Windows.Forms.TextBox textBoxSearch;
	}
}