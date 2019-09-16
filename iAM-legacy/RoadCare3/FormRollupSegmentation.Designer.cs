namespace RoadCare3
{
    partial class FormRollupSegmentation
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( FormRollupSegmentation ) );
			this.dgvRollup = new System.Windows.Forms.DataGridView();
			this.ATTRIBUTE = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.METHOD = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.labelRollup = new System.Windows.Forms.Label();
			this.listBoxAttributes = new System.Windows.Forms.ListBox();
			this.buttonRollup = new System.Windows.Forms.Button();
			this.labelLRS = new System.Windows.Forms.Label();
			this.labelSRS = new System.Windows.Forms.Label();
			this.timerRollup = new System.Windows.Forms.Timer( this.components );
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.panelRollup = new System.Windows.Forms.Panel();
			( ( System.ComponentModel.ISupportInitialize )( this.dgvRollup ) ).BeginInit();
			( ( System.ComponentModel.ISupportInitialize )( this.pictureBox2 ) ).BeginInit();
			this.panelRollup.SuspendLayout();
			this.SuspendLayout();
			// 
			// dgvRollup
			// 
			this.dgvRollup.AllowUserToAddRows = false;
			this.dgvRollup.AllowUserToDeleteRows = false;
			this.dgvRollup.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left ) ) );
			this.dgvRollup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvRollup.Columns.AddRange( new System.Windows.Forms.DataGridViewColumn[] {
            this.ATTRIBUTE,
            this.METHOD} );
			this.dgvRollup.Location = new System.Drawing.Point( 8, 13 );
			this.dgvRollup.Margin = new System.Windows.Forms.Padding( 2 );
			this.dgvRollup.Name = "dgvRollup";
			this.dgvRollup.RowHeadersVisible = false;
			this.dgvRollup.RowTemplate.Height = 24;
			this.dgvRollup.Size = new System.Drawing.Size( 352, 253 );
			this.dgvRollup.TabIndex = 0;
			this.dgvRollup.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler( this.dgvRollup_CellValidated );
			this.dgvRollup.SelectionChanged += new System.EventHandler( this.dgvRollup_SelectionChanged );
			// 
			// ATTRIBUTE
			// 
			this.ATTRIBUTE.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.ATTRIBUTE.HeaderText = "ATTRIBUTE";
			this.ATTRIBUTE.Name = "ATTRIBUTE";
			// 
			// METHOD
			// 
			this.METHOD.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.METHOD.HeaderText = "METHOD";
			this.METHOD.Name = "METHOD";
			// 
			// labelRollup
			// 
			this.labelRollup.AutoSize = true;
			this.labelRollup.Font = new System.Drawing.Font( "Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
			this.labelRollup.Location = new System.Drawing.Point( 46, 11 );
			this.labelRollup.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 );
			this.labelRollup.Name = "labelRollup";
			this.labelRollup.Size = new System.Drawing.Size( 228, 26 );
			this.labelRollup.TabIndex = 2;
			this.labelRollup.Text = "Segmentation Rollup -";
			// 
			// listBoxAttributes
			// 
			this.listBoxAttributes.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.listBoxAttributes.FormattingEnabled = true;
			this.listBoxAttributes.Location = new System.Drawing.Point( 374, 86 );
			this.listBoxAttributes.Margin = new System.Windows.Forms.Padding( 2 );
			this.listBoxAttributes.Name = "listBoxAttributes";
			this.listBoxAttributes.Size = new System.Drawing.Size( 376, 264 );
			this.listBoxAttributes.TabIndex = 3;
			// 
			// buttonRollup
			// 
			this.buttonRollup.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.buttonRollup.Enabled = false;
			this.buttonRollup.Font = new System.Drawing.Font( "Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
			this.buttonRollup.Location = new System.Drawing.Point( 73, 368 );
			this.buttonRollup.Margin = new System.Windows.Forms.Padding( 2 );
			this.buttonRollup.Name = "buttonRollup";
			this.buttonRollup.Size = new System.Drawing.Size( 217, 32 );
			this.buttonRollup.TabIndex = 4;
			this.buttonRollup.Text = "Rollup Network";
			this.buttonRollup.UseVisualStyleBackColor = true;
			this.buttonRollup.Click += new System.EventHandler( this.buttonRollup_Click );
			// 
			// labelLRS
			// 
			this.labelLRS.AutoSize = true;
			this.labelLRS.Location = new System.Drawing.Point( 51, 40 );
			this.labelLRS.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 );
			this.labelLRS.Name = "labelLRS";
			this.labelLRS.Size = new System.Drawing.Size( 136, 13 );
			this.labelLRS.TabIndex = 20;
			this.labelLRS.Text = "Linear Reference Sections:";
			// 
			// labelSRS
			// 
			this.labelSRS.AutoSize = true;
			this.labelSRS.Location = new System.Drawing.Point( 51, 57 );
			this.labelSRS.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 );
			this.labelSRS.Name = "labelSRS";
			this.labelSRS.Size = new System.Drawing.Size( 149, 13 );
			this.labelSRS.TabIndex = 21;
			this.labelSRS.Text = "Section Referenced Sections:";
			// 
			// timerRollup
			// 
			this.timerRollup.Tick += new System.EventHandler( this.timerRollup_Tick );
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = ( ( System.Drawing.Image )( resources.GetObject( "pictureBox2.Image" ) ) );
			this.pictureBox2.Location = new System.Drawing.Point( 10, 11 );
			this.pictureBox2.Margin = new System.Windows.Forms.Padding( 2 );
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size( 28, 30 );
			this.pictureBox2.TabIndex = 34;
			this.pictureBox2.TabStop = false;
			// 
			// panelRollup
			// 
			this.panelRollup.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left )
						| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.panelRollup.Controls.Add( this.dgvRollup );
			this.panelRollup.Location = new System.Drawing.Point( 10, 73 );
			this.panelRollup.Name = "panelRollup";
			this.panelRollup.Size = new System.Drawing.Size( 740, 290 );
			this.panelRollup.TabIndex = 35;
			// 
			// FormRollupSegmentation
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 758, 415 );
			this.Controls.Add( this.pictureBox2 );
			this.Controls.Add( this.labelSRS );
			this.Controls.Add( this.labelLRS );
			this.Controls.Add( this.buttonRollup );
			this.Controls.Add( this.listBoxAttributes );
			this.Controls.Add( this.labelRollup );
			this.Controls.Add( this.panelRollup );
			this.Margin = new System.Windows.Forms.Padding( 2 );
			this.Name = "FormRollupSegmentation";
			this.TabText = "FormRollupSegmentation";
			this.Text = "FormRollupSegmentation";
			this.Load += new System.EventHandler( this.FormRollupSegmentation_Load );
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.FormRollupSegmentation_FormClosed );
			( ( System.ComponentModel.ISupportInitialize )( this.dgvRollup ) ).EndInit();
			( ( System.ComponentModel.ISupportInitialize )( this.pictureBox2 ) ).EndInit();
			this.panelRollup.ResumeLayout( false );
			this.ResumeLayout( false );
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.DataGridView dgvRollup;
        private System.Windows.Forms.Label labelRollup;
        private System.Windows.Forms.ListBox listBoxAttributes;
		private System.Windows.Forms.Button buttonRollup;
        private System.Windows.Forms.Label labelLRS;
        private System.Windows.Forms.Label labelSRS;
        private System.Windows.Forms.Timer timerRollup;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ATTRIBUTE;
        private System.Windows.Forms.DataGridViewComboBoxColumn METHOD;
		private System.Windows.Forms.Panel panelRollup;
    }
}