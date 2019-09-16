namespace RoadCare3
{
	partial class FormAttributeValidation
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.ColumnAttribute = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColumnMin = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColumnMax = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			( ( System.ComponentModel.ISupportInitialize )( this.dataGridView1 ) ).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point( 0, 0 );
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add( this.dataGridView1 );
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add( this.buttonOK );
			this.splitContainer1.Panel2.Controls.Add( this.buttonCancel );
			this.splitContainer1.Size = new System.Drawing.Size( 779, 537 );
			this.splitContainer1.SplitterDistance = 475;
			this.splitContainer1.TabIndex = 0;
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange( new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnAttribute,
            this.ColumnMin,
            this.ColumnMax} );
			this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView1.Location = new System.Drawing.Point( 0, 0 );
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.Size = new System.Drawing.Size( 779, 475 );
			this.dataGridView1.TabIndex = 0;
			this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler( this.dataGridView1_CellValueChanged );
			// 
			// ColumnAttribute
			// 
			this.ColumnAttribute.HeaderText = "Attribute";
			this.ColumnAttribute.Name = "ColumnAttribute";
			this.ColumnAttribute.ReadOnly = true;
			// 
			// ColumnMin
			// 
			this.ColumnMin.HeaderText = "Min";
			this.ColumnMin.Name = "ColumnMin";
			// 
			// ColumnMax
			// 
			this.ColumnMax.HeaderText = "Max";
			this.ColumnMax.Name = "ColumnMax";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point( 12, 22 );
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size( 75, 23 );
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler( this.buttonOK_Click );
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point( 692, 22 );
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size( 75, 23 );
			this.buttonCancel.TabIndex = 0;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler( this.buttonCancel_Click );
			// 
			// FormAttributeValidation
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 779, 537 );
			this.Controls.Add( this.splitContainer1 );
			this.Name = "FormAttributeValidation";
			this.Text = "Attribute Validation";
			this.Load += new System.EventHandler( this.FormAttributeValidation_Load );
			this.splitContainer1.Panel1.ResumeLayout( false );
			this.splitContainer1.Panel2.ResumeLayout( false );
			this.splitContainer1.ResumeLayout( false );
			( ( System.ComponentModel.ISupportInitialize )( this.dataGridView1 ) ).EndInit();
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnAttribute;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnMin;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColumnMax;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
	}
}