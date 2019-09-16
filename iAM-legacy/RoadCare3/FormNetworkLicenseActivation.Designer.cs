namespace RoadCare3
{
	partial class FormNetworkLicenseActivation
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
			this.label6 = new System.Windows.Forms.Label();
			this.tbVKey = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tbLURL = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnActivate = new System.Windows.Forms.Button();
			this.tbLCode = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(12, 9);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(509, 13);
			this.label6.TabIndex = 37;
			this.label6.Text = "Please obtain Validation Key/License Code/License URL from your system administra" +
				"tor!";
			// 
			// tbVKey
			// 
			this.tbVKey.Location = new System.Drawing.Point(92, 28);
			this.tbVKey.Name = "tbVKey";
			this.tbVKey.Size = new System.Drawing.Size(463, 20);
			this.tbVKey.TabIndex = 27;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 31);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(77, 13);
			this.label3.TabIndex = 36;
			this.label3.Text = "Validation Key:";
			// 
			// tbLURL
			// 
			this.tbLURL.Location = new System.Drawing.Point(92, 79);
			this.tbLURL.Name = "tbLURL";
			this.tbLURL.Size = new System.Drawing.Size(463, 20);
			this.tbLURL.TabIndex = 29;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 82);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(72, 13);
			this.label5.TabIndex = 35;
			this.label5.Text = "License URL:";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(418, 105);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(137, 23);
			this.btnCancel.TabIndex = 31;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnActivate
			// 
			this.btnActivate.Location = new System.Drawing.Point(275, 105);
			this.btnActivate.Name = "btnActivate";
			this.btnActivate.Size = new System.Drawing.Size(137, 23);
			this.btnActivate.TabIndex = 30;
			this.btnActivate.Text = "Activate RoadCare";
			this.btnActivate.UseVisualStyleBackColor = true;
			this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
			// 
			// tbLCode
			// 
			this.tbLCode.Location = new System.Drawing.Point(92, 54);
			this.tbLCode.Name = "tbLCode";
			this.tbLCode.Size = new System.Drawing.Size(463, 20);
			this.tbLCode.TabIndex = 28;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 57);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(75, 13);
			this.label4.TabIndex = 34;
			this.label4.Text = "License Code:";
			// 
			// FormNetworkLicenseActivation
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(563, 137);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.tbVKey);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.tbLURL);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnActivate);
			this.Controls.Add(this.tbLCode);
			this.Controls.Add(this.label4);
			this.Name = "FormNetworkLicenseActivation";
			this.Text = "Network/Concurrent License Activation";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbVKey;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbLURL;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnActivate;
		private System.Windows.Forms.TextBox tbLCode;
		private System.Windows.Forms.Label label4;
	}
}