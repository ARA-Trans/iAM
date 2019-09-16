namespace RoadCare3
{
    partial class FormCreateSubnetwork
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
			this.textBoxNetworkName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxFilter = new System.Windows.Forms.TextBox();
			this.buttonCriteria = new System.Windows.Forms.Button();
			this.buttonCreate = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Subnetwork name:";
			// 
			// textBoxNetworkName
			// 
			this.textBoxNetworkName.Location = new System.Drawing.Point(111, 10);
			this.textBoxNetworkName.Name = "textBoxNetworkName";
			this.textBoxNetworkName.Size = new System.Drawing.Size(196, 20);
			this.textBoxNetworkName.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(89, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Subnetwork filter:";
			// 
			// textBoxFilter
			// 
			this.textBoxFilter.Location = new System.Drawing.Point(111, 41);
			this.textBoxFilter.Multiline = true;
			this.textBoxFilter.Name = "textBoxFilter";
			this.textBoxFilter.Size = new System.Drawing.Size(507, 48);
			this.textBoxFilter.TabIndex = 3;
			// 
			// buttonCriteria
			// 
			this.buttonCriteria.Location = new System.Drawing.Point(624, 54);
			this.buttonCriteria.Name = "buttonCriteria";
			this.buttonCriteria.Size = new System.Drawing.Size(34, 28);
			this.buttonCriteria.TabIndex = 4;
			this.buttonCriteria.Text = "...";
			this.buttonCriteria.UseVisualStyleBackColor = true;
			this.buttonCriteria.Click += new System.EventHandler(this.buttonCriteria_Click);
			// 
			// buttonCreate
			// 
			this.buttonCreate.Location = new System.Drawing.Point(442, 95);
			this.buttonCreate.Name = "buttonCreate";
			this.buttonCreate.Size = new System.Drawing.Size(75, 23);
			this.buttonCreate.TabIndex = 5;
			this.buttonCreate.Text = "Create";
			this.buttonCreate.UseVisualStyleBackColor = true;
			this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(543, 95);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// FormCreateSubnetwork
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(660, 125);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonCreate);
			this.Controls.Add(this.buttonCriteria);
			this.Controls.Add(this.textBoxFilter);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxNetworkName);
			this.Controls.Add(this.label1);
			this.Name = "FormCreateSubnetwork";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Create Subnetwork";
			this.Load += new System.EventHandler(this.FormCreateSubnetwork_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxNetworkName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Button buttonCriteria;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonCancel;
    }
}