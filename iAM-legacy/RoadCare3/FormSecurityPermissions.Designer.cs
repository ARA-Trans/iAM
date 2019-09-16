namespace RoadCare3
{
	partial class FormSecurityPermissions
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
			this.lstUserGroups = new System.Windows.Forms.ListBox();
			this.lstUsers = new System.Windows.Forms.ListBox();
			this.lstActions = new System.Windows.Forms.ListBox();
			this.lstActionGroups = new System.Windows.Forms.ListBox();
			this.cmbPermission = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// lstUserGroups
			// 
			this.lstUserGroups.FormattingEnabled = true;
			this.lstUserGroups.Location = new System.Drawing.Point( 12, 12 );
			this.lstUserGroups.Name = "lstUserGroups";
			this.lstUserGroups.Size = new System.Drawing.Size( 253, 147 );
			this.lstUserGroups.TabIndex = 0;
			this.lstUserGroups.SelectedIndexChanged += new System.EventHandler( this.lstUserGroups_SelectedIndexChanged );
			// 
			// lstUsers
			// 
			this.lstUsers.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left ) ) );
			this.lstUsers.FormattingEnabled = true;
			this.lstUsers.Location = new System.Drawing.Point( 12, 170 );
			this.lstUsers.Name = "lstUsers";
			this.lstUsers.Size = new System.Drawing.Size( 253, 251 );
			this.lstUsers.TabIndex = 1;
			this.lstUsers.SelectedIndexChanged += new System.EventHandler( this.lstUsers_SelectedIndexChanged );
			// 
			// lstActions
			// 
			this.lstActions.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left ) ) );
			this.lstActions.FormattingEnabled = true;
			this.lstActions.Location = new System.Drawing.Point( 280, 170 );
			this.lstActions.Name = "lstActions";
			this.lstActions.Size = new System.Drawing.Size( 253, 251 );
			this.lstActions.TabIndex = 3;
			this.lstActions.SelectedIndexChanged += new System.EventHandler( this.lstActions_SelectedIndexChanged );
			// 
			// lstActionGroups
			// 
			this.lstActionGroups.FormattingEnabled = true;
			this.lstActionGroups.Location = new System.Drawing.Point( 280, 12 );
			this.lstActionGroups.Name = "lstActionGroups";
			this.lstActionGroups.Size = new System.Drawing.Size( 253, 147 );
			this.lstActionGroups.TabIndex = 2;
			this.lstActionGroups.SelectedIndexChanged += new System.EventHandler( this.lstActionGroups_SelectedIndexChanged );
			// 
			// cmbPermission
			// 
			this.cmbPermission.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.cmbPermission.FormattingEnabled = true;
			this.cmbPermission.Location = new System.Drawing.Point( 12, 427 );
			this.cmbPermission.Name = "cmbPermission";
			this.cmbPermission.Size = new System.Drawing.Size( 521, 21 );
			this.cmbPermission.TabIndex = 4;
			this.cmbPermission.SelectedIndexChanged += new System.EventHandler( this.cmbPermission_SelectedIndexChanged );
			// 
			// FormSecurityPermissions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 545, 459 );
			this.Controls.Add( this.cmbPermission );
			this.Controls.Add( this.lstActions );
			this.Controls.Add( this.lstActionGroups );
			this.Controls.Add( this.lstUsers );
			this.Controls.Add( this.lstUserGroups );
			this.Name = "FormSecurityPermissions";
			this.TabText = "FormSecurityPermissions";
			this.Text = "FormSecurityPermissions";
			this.Load += new System.EventHandler( this.FormSecurityPermissions_Load );
			this.Enter += new System.EventHandler( this.FormSecurityPermissions_Enter );
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.FormSecurityPermissions_FormClosed );
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.ListBox lstUserGroups;
		private System.Windows.Forms.ListBox lstUsers;
		private System.Windows.Forms.ListBox lstActions;
		private System.Windows.Forms.ListBox lstActionGroups;
		private System.Windows.Forms.ComboBox cmbPermission;
	}
}