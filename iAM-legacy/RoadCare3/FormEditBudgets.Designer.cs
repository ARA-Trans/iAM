namespace RoadCare3
{
    partial class FormEditBudgets
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
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonUp = new System.Windows.Forms.Button();
			this.buttonDown = new System.Windows.Forms.Button();
			this.textBoxAdd = new System.Windows.Forms.TextBox();
			this.listBoxBudgets = new System.Windows.Forms.ListBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonAdd
			// 
			this.buttonAdd.Location = new System.Drawing.Point(247, 12);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(75, 23);
			this.buttonAdd.TabIndex = 0;
			this.buttonAdd.Text = "Add";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Location = new System.Drawing.Point(247, 50);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(75, 23);
			this.buttonDelete.TabIndex = 1;
			this.buttonDelete.Text = "Delete";
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonUp
			// 
			this.buttonUp.Location = new System.Drawing.Point(247, 89);
			this.buttonUp.Name = "buttonUp";
			this.buttonUp.Size = new System.Drawing.Size(75, 23);
			this.buttonUp.TabIndex = 2;
			this.buttonUp.Text = "Up";
			this.buttonUp.UseVisualStyleBackColor = true;
			this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
			// 
			// buttonDown
			// 
			this.buttonDown.Location = new System.Drawing.Point(247, 132);
			this.buttonDown.Name = "buttonDown";
			this.buttonDown.Size = new System.Drawing.Size(75, 23);
			this.buttonDown.TabIndex = 3;
			this.buttonDown.Text = "Down";
			this.buttonDown.UseVisualStyleBackColor = true;
			this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
			// 
			// textBoxAdd
			// 
			this.textBoxAdd.Location = new System.Drawing.Point(13, 12);
			this.textBoxAdd.Name = "textBoxAdd";
			this.textBoxAdd.Size = new System.Drawing.Size(228, 20);
			this.textBoxAdd.TabIndex = 4;
			// 
			// listBoxBudgets
			// 
			this.listBoxBudgets.FormattingEnabled = true;
			this.listBoxBudgets.Location = new System.Drawing.Point(13, 50);
			this.listBoxBudgets.Name = "listBoxBudgets";
			this.listBoxBudgets.Size = new System.Drawing.Size(228, 264);
			this.listBoxBudgets.TabIndex = 5;
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(248, 173);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// FormEditBudgets
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(325, 346);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.listBoxBudgets);
			this.Controls.Add(this.textBoxAdd);
			this.Controls.Add(this.buttonDown);
			this.Controls.Add(this.buttonUp);
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.buttonAdd);
			this.Name = "FormEditBudgets";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Budget Categories";
			this.Load += new System.EventHandler(this.FormEditBudgets_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.TextBox textBoxAdd;
        private System.Windows.Forms.ListBox listBoxBudgets;
        private System.Windows.Forms.Button buttonOK;
    }
}