namespace RoadCare3
{
	partial class FormWorkstationOrUnlimitedLicenseActivation
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
			this.tbVKey = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tbLCode = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnActivateRoadCare = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tbVKey
			// 
			this.tbVKey.Location = new System.Drawing.Point(12, 23);
			this.tbVKey.Name = "tbVKey";
			this.tbVKey.Size = new System.Drawing.Size(353, 20);
			this.tbVKey.TabIndex = 23;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(9, 7);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(74, 13);
			this.label3.TabIndex = 26;
			this.label3.Text = "Validation Key";
			// 
			// tbLCode
			// 
			this.tbLCode.Location = new System.Drawing.Point(12, 62);
			this.tbLCode.Name = "tbLCode";
			this.tbLCode.Size = new System.Drawing.Size(353, 20);
			this.tbLCode.TabIndex = 24;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(9, 46);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 13);
			this.label4.TabIndex = 25;
			this.label4.Text = "License Code";
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(218, 88);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(137, 23);
			this.btnCancel.TabIndex = 28;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnActivateRoadCare
			// 
			this.btnActivateRoadCare.Location = new System.Drawing.Point(21, 88);
			this.btnActivateRoadCare.Name = "btnActivateRoadCare";
			this.btnActivateRoadCare.Size = new System.Drawing.Size(137, 23);
			this.btnActivateRoadCare.TabIndex = 27;
			this.btnActivateRoadCare.Text = "Activate RoadCare";
			this.btnActivateRoadCare.UseVisualStyleBackColor = true;
			this.btnActivateRoadCare.Click += new System.EventHandler(this.btnActivateRoadCare_Click);
			// 
			// FormWorkstationOrUnlimitedLicenseActivation
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(377, 120);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnActivateRoadCare);
			this.Controls.Add(this.tbVKey);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.tbLCode);
			this.Controls.Add(this.label4);
			this.Name = "FormWorkstationOrUnlimitedLicenseActivation";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "License Activation (Workstation or Unlimited)";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbVKey;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbLCode;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnActivateRoadCare;
	}
}