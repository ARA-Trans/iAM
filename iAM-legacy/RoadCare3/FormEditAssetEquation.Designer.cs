namespace RoadCare3
{
    partial class FormEditAssetEquation
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
            this.listBoxAttribute = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxEquation = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCompile = new System.Windows.Forms.TextBox();
            this.labelCompile = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCheck = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.dgvDefault = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxReturn = new System.Windows.Forms.TextBox();
            this.buttonUpdate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDefault)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxAttribute
            // 
            this.listBoxAttribute.FormattingEnabled = true;
            this.listBoxAttribute.Location = new System.Drawing.Point(12, 22);
            this.listBoxAttribute.Name = "listBoxAttribute";
            this.listBoxAttribute.Size = new System.Drawing.Size(258, 186);
            this.listBoxAttribute.TabIndex = 0;
            this.listBoxAttribute.SelectedIndexChanged += new System.EventHandler(this.listBoxAttribute_SelectedIndexChanged);
            this.listBoxAttribute.DoubleClick += new System.EventHandler(this.listBoxAttribute_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Attributes";
            // 
            // textBoxEquation
            // 
            this.textBoxEquation.Location = new System.Drawing.Point(12, 228);
            this.textBoxEquation.Multiline = true;
            this.textBoxEquation.Name = "textBoxEquation";
            this.textBoxEquation.Size = new System.Drawing.Size(658, 268);
            this.textBoxEquation.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 213);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Code:";
            // 
            // textBoxCompile
            // 
            this.textBoxCompile.BackColor = System.Drawing.Color.White;
            this.textBoxCompile.Location = new System.Drawing.Point(12, 515);
            this.textBoxCompile.Multiline = true;
            this.textBoxCompile.Name = "textBoxCompile";
            this.textBoxCompile.ReadOnly = true;
            this.textBoxCompile.Size = new System.Drawing.Size(658, 107);
            this.textBoxCompile.TabIndex = 4;
            // 
            // labelCompile
            // 
            this.labelCompile.AutoSize = true;
            this.labelCompile.Location = new System.Drawing.Point(12, 499);
            this.labelCompile.Name = "labelCompile";
            this.labelCompile.Size = new System.Drawing.Size(124, 13);
            this.labelCompile.TabIndex = 5;
            this.labelCompile.Text = "Compile Output Window:";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(335, 630);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCheck
            // 
            this.buttonCheck.Location = new System.Drawing.Point(245, 630);
            this.buttonCheck.Name = "buttonCheck";
            this.buttonCheck.Size = new System.Drawing.Size(75, 23);
            this.buttonCheck.TabIndex = 7;
            this.buttonCheck.Text = "Check";
            this.buttonCheck.UseVisualStyleBackColor = true;
            this.buttonCheck.Click += new System.EventHandler(this.buttonCheck_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(425, 629);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // dgvDefault
            // 
            this.dgvDefault.AllowUserToAddRows = false;
            this.dgvDefault.AllowUserToDeleteRows = false;
            this.dgvDefault.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDefault.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dgvDefault.Location = new System.Drawing.Point(280, 20);
            this.dgvDefault.Name = "dgvDefault";
            this.dgvDefault.RowHeadersVisible = false;
            this.dgvDefault.Size = new System.Drawing.Size(390, 160);
            this.dgvDefault.TabIndex = 18;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.HeaderText = "Attribute";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.HeaderText = "Value";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(277, 191);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Result:";
            // 
            // textBoxReturn
            // 
            this.textBoxReturn.Location = new System.Drawing.Point(331, 187);
            this.textBoxReturn.Name = "textBoxReturn";
            this.textBoxReturn.Size = new System.Drawing.Size(258, 20);
            this.textBoxReturn.TabIndex = 20;
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.Location = new System.Drawing.Point(595, 184);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(75, 23);
            this.buttonUpdate.TabIndex = 21;
            this.buttonUpdate.Text = "Update";
            this.buttonUpdate.UseVisualStyleBackColor = true;
            this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
            // 
            // FormEditAssetEquation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 665);
            this.Controls.Add(this.buttonUpdate);
            this.Controls.Add(this.textBoxReturn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dgvDefault);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonCheck);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelCompile);
            this.Controls.Add(this.textBoxCompile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxEquation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxAttribute);
            this.Name = "FormEditAssetEquation";
            this.Text = "Edit Asset Functions";
            this.Load += new System.EventHandler(this.FormEditEquation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDefault)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxAttribute;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxEquation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxCompile;
        private System.Windows.Forms.Label labelCompile;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCheck;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.DataGridView dgvDefault;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxReturn;
        private System.Windows.Forms.Button buttonUpdate;
    }
}