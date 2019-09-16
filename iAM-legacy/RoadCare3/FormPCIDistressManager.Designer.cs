namespace RoadCare3
{
	partial class FormPCIDistressManager
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
			this.dgvDistressMapping = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.dgvDistressMapping)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvDistressMapping
			// 
			this.dgvDistressMapping.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvDistressMapping.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvDistressMapping.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvDistressMapping.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.dgvDistressMapping.Location = new System.Drawing.Point(0, 0);
			this.dgvDistressMapping.Name = "dgvDistressMapping";
			this.dgvDistressMapping.Size = new System.Drawing.Size(292, 266);
			this.dgvDistressMapping.TabIndex = 0;
			this.dgvDistressMapping.Leave += new System.EventHandler(this.dgvDistressMapping_Leave);
			// 
			// FormPCIDistressManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.dgvDistressMapping);
			this.Name = "FormPCIDistressManager";
			this.TabText = "FormPCIDistressManager";
			this.Text = "FormPCIDistressManager";
			this.Load += new System.EventHandler(this.FormPCIDistressManager_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgvDistressMapping)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dgvDistressMapping;
	}
}