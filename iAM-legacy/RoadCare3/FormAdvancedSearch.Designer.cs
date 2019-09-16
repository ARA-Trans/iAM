namespace RoadCare3
{
    partial class FormAdvancedSearch
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
			this.label1 = new System.Windows.Forms.Label();
			this.checkBoxShowAll = new System.Windows.Forms.CheckBox();
			this.treeViewAttribute = new System.Windows.Forms.TreeView();
			this.listBoxValues = new System.Windows.Forms.ListBox();
			this.textBoxSearch = new System.Windows.Forms.TextBox();
			this.buttonCheck = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonNotEqual = new System.Windows.Forms.Button();
			this.buttonOR = new System.Windows.Forms.Button();
			this.buttonAnd = new System.Windows.Forms.Button();
			this.buttonGreaterThan = new System.Windows.Forms.Button();
			this.buttonGreaterEqual = new System.Windows.Forms.Button();
			this.buttonLessEqual = new System.Windows.Forms.Button();
			this.buttonLessThan = new System.Windows.Forms.Button();
			this.buttonEqual = new System.Windows.Forms.Button();
			this.labelResults = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 10);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Attributes:";
			// 
			// checkBoxShowAll
			// 
			this.checkBoxShowAll.AutoSize = true;
			this.checkBoxShowAll.Location = new System.Drawing.Point(301, 10);
			this.checkBoxShowAll.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxShowAll.Name = "checkBoxShowAll";
			this.checkBoxShowAll.Size = new System.Drawing.Size(156, 17);
			this.checkBoxShowAll.TabIndex = 2;
			this.checkBoxShowAll.Text = "Show all values for network";
			this.checkBoxShowAll.UseVisualStyleBackColor = true;
			this.checkBoxShowAll.CheckedChanged += new System.EventHandler(this.checkBoxShowAll_CheckedChanged);
			// 
			// treeViewAttribute
			// 
			this.treeViewAttribute.Location = new System.Drawing.Point(12, 34);
			this.treeViewAttribute.Margin = new System.Windows.Forms.Padding(2);
			this.treeViewAttribute.Name = "treeViewAttribute";
			this.treeViewAttribute.Size = new System.Drawing.Size(164, 310);
			this.treeViewAttribute.TabIndex = 3;
			this.treeViewAttribute.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeViewAttribute_MouseDoubleClick);
			this.treeViewAttribute.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewAttribute_AfterSelect);
			// 
			// listBoxValues
			// 
			this.listBoxValues.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxValues.FormattingEnabled = true;
			this.listBoxValues.Location = new System.Drawing.Point(302, 34);
			this.listBoxValues.Margin = new System.Windows.Forms.Padding(2);
			this.listBoxValues.Name = "listBoxValues";
			this.listBoxValues.Size = new System.Drawing.Size(185, 316);
			this.listBoxValues.Sorted = true;
			this.listBoxValues.TabIndex = 4;
			this.listBoxValues.DoubleClick += new System.EventHandler(this.listBoxValues_DoubleClick);
			// 
			// textBoxSearch
			// 
			this.textBoxSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxSearch.Location = new System.Drawing.Point(10, 403);
			this.textBoxSearch.Margin = new System.Windows.Forms.Padding(2);
			this.textBoxSearch.Multiline = true;
			this.textBoxSearch.Name = "textBoxSearch";
			this.textBoxSearch.Size = new System.Drawing.Size(476, 90);
			this.textBoxSearch.TabIndex = 5;
			// 
			// buttonCheck
			// 
			this.buttonCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonCheck.Location = new System.Drawing.Point(11, 357);
			this.buttonCheck.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCheck.Name = "buttonCheck";
			this.buttonCheck.Size = new System.Drawing.Size(80, 27);
			this.buttonCheck.TabIndex = 8;
			this.buttonCheck.Text = "Check";
			this.buttonCheck.UseVisualStyleBackColor = true;
			this.buttonCheck.Click += new System.EventHandler(this.buttonCheck_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonCancel.Location = new System.Drawing.Point(181, 357);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(80, 27);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonOK.Location = new System.Drawing.Point(96, 357);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(2);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(80, 27);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonNotEqual
			// 
			this.buttonNotEqual.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonNotEqual.Location = new System.Drawing.Point(241, 46);
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
			this.buttonOR.Location = new System.Drawing.Point(241, 178);
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
			this.buttonAnd.Location = new System.Drawing.Point(180, 178);
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
			this.buttonGreaterThan.Location = new System.Drawing.Point(241, 134);
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
			this.buttonGreaterEqual.Location = new System.Drawing.Point(180, 134);
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
			this.buttonLessEqual.Location = new System.Drawing.Point(180, 89);
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
			this.buttonLessThan.Location = new System.Drawing.Point(241, 89);
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
			this.buttonEqual.Location = new System.Drawing.Point(180, 46);
			this.buttonEqual.Margin = new System.Windows.Forms.Padding(2);
			this.buttonEqual.Name = "buttonEqual";
			this.buttonEqual.Size = new System.Drawing.Size(56, 28);
			this.buttonEqual.TabIndex = 17;
			this.buttonEqual.Text = "=";
			this.buttonEqual.UseVisualStyleBackColor = true;
			this.buttonEqual.Click += new System.EventHandler(this.buttonEqual_Click);
			// 
			// labelResults
			// 
			this.labelResults.AutoSize = true;
			this.labelResults.Location = new System.Drawing.Point(11, 386);
			this.labelResults.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelResults.Name = "labelResults";
			this.labelResults.Size = new System.Drawing.Size(124, 13);
			this.labelResults.TabIndex = 25;
			this.labelResults.Text = "xxxx results match query.";
			this.labelResults.Visible = false;
			// 
			// FormAdvancedSearch
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(494, 504);
			this.Controls.Add(this.labelResults);
			this.Controls.Add(this.buttonNotEqual);
			this.Controls.Add(this.buttonOR);
			this.Controls.Add(this.buttonAnd);
			this.Controls.Add(this.buttonGreaterThan);
			this.Controls.Add(this.buttonGreaterEqual);
			this.Controls.Add(this.buttonLessEqual);
			this.Controls.Add(this.buttonLessThan);
			this.Controls.Add(this.buttonEqual);
			this.Controls.Add(this.buttonCheck);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.textBoxSearch);
			this.Controls.Add(this.listBoxValues);
			this.Controls.Add(this.treeViewAttribute);
			this.Controls.Add(this.checkBoxShowAll);
			this.Controls.Add(this.label1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "FormAdvancedSearch";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FormAdvancedSearch";
			this.Load += new System.EventHandler(this.FormAdvancedSearch_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxShowAll;
        private System.Windows.Forms.TreeView treeViewAttribute;
        private System.Windows.Forms.ListBox listBoxValues;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.Button buttonCheck;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonNotEqual;
        private System.Windows.Forms.Button buttonOR;
        private System.Windows.Forms.Button buttonAnd;
        private System.Windows.Forms.Button buttonGreaterThan;
        private System.Windows.Forms.Button buttonGreaterEqual;
        private System.Windows.Forms.Button buttonLessEqual;
        private System.Windows.Forms.Button buttonLessThan;
        private System.Windows.Forms.Button buttonEqual;
        private System.Windows.Forms.Label labelResults;
    }
}