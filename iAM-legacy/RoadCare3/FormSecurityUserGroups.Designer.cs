namespace RoadCare3
{
	partial class FormSecurityUserGroups
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
			this.btnRemoveUserGroup = new System.Windows.Forms.Button();
			this.btnChangeUserGroup = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.txtUserGroupName = new System.Windows.Forms.TextBox();
			this.btnAddUserGroup = new System.Windows.Forms.Button();
			this.cblUsers = new System.Windows.Forms.CheckedListBox();
			this.lstUserGroups = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// btnRemoveUserGroup
			// 
			this.btnRemoveUserGroup.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.btnRemoveUserGroup.Location = new System.Drawing.Point( 731, 797 );
			this.btnRemoveUserGroup.Name = "btnRemoveUserGroup";
			this.btnRemoveUserGroup.Size = new System.Drawing.Size( 133, 20 );
			this.btnRemoveUserGroup.TabIndex = 47;
			this.btnRemoveUserGroup.Text = "Remove Usergroup";
			this.btnRemoveUserGroup.UseVisualStyleBackColor = true;
			this.btnRemoveUserGroup.Click += new System.EventHandler( this.btnRemoveUserGroup_Click );
			// 
			// btnChangeUserGroup
			// 
			this.btnChangeUserGroup.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.btnChangeUserGroup.Location = new System.Drawing.Point( 14, 797 );
			this.btnChangeUserGroup.Name = "btnChangeUserGroup";
			this.btnChangeUserGroup.Size = new System.Drawing.Size( 120, 20 );
			this.btnChangeUserGroup.TabIndex = 46;
			this.btnChangeUserGroup.Text = "Update Usergroup";
			this.btnChangeUserGroup.UseVisualStyleBackColor = true;
			this.btnChangeUserGroup.Click += new System.EventHandler( this.btnChangeUserGroup_Click );
			// 
			// label1
			// 
			this.label1.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point( 12, 761 );
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size( 67, 13 );
			this.label1.TabIndex = 39;
			this.label1.Text = "Group Name";
			// 
			// txtUserGroupName
			// 
			this.txtUserGroupName.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.txtUserGroupName.Location = new System.Drawing.Point( 85, 758 );
			this.txtUserGroupName.Name = "txtUserGroupName";
			this.txtUserGroupName.Size = new System.Drawing.Size( 129, 20 );
			this.txtUserGroupName.TabIndex = 37;
			// 
			// btnAddUserGroup
			// 
			this.btnAddUserGroup.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.btnAddUserGroup.Location = new System.Drawing.Point( 186, 797 );
			this.btnAddUserGroup.Name = "btnAddUserGroup";
			this.btnAddUserGroup.Size = new System.Drawing.Size( 108, 20 );
			this.btnAddUserGroup.TabIndex = 38;
			this.btnAddUserGroup.Text = "Add Usergroup";
			this.btnAddUserGroup.UseVisualStyleBackColor = true;
			this.btnAddUserGroup.Click += new System.EventHandler( this.btnAddUserGroup_Click );
			// 
			// cblUsers
			// 
			this.cblUsers.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left ) ) );
			this.cblUsers.CheckOnClick = true;
			this.cblUsers.FormattingEnabled = true;
			this.cblUsers.Location = new System.Drawing.Point( 435, 12 );
			this.cblUsers.Name = "cblUsers";
			this.cblUsers.Size = new System.Drawing.Size( 429, 739 );
			this.cblUsers.TabIndex = 36;
			this.cblUsers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler( this.cblUsers_ItemCheck );
			// 
			// lstUserGroups
			// 
			this.lstUserGroups.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left ) ) );
			this.lstUserGroups.FormattingEnabled = true;
			this.lstUserGroups.Location = new System.Drawing.Point( 12, 12 );
			this.lstUserGroups.Name = "lstUserGroups";
			this.lstUserGroups.Size = new System.Drawing.Size( 417, 732 );
			this.lstUserGroups.TabIndex = 35;
			this.lstUserGroups.SelectedIndexChanged += new System.EventHandler( this.lstUserGroups_SelectedIndexChanged );
			// 
			// FormSecurityUserGroups
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 876, 829 );
			this.Controls.Add( this.btnRemoveUserGroup );
			this.Controls.Add( this.btnChangeUserGroup );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.txtUserGroupName );
			this.Controls.Add( this.btnAddUserGroup );
			this.Controls.Add( this.cblUsers );
			this.Controls.Add( this.lstUserGroups );
			this.Name = "FormSecurityUserGroups";
			this.TabText = "FormSecurityUserGroups";
			this.Text = "FormSecurityUserGroups";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.FormSecurityUserGroups_FormClosed );
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnRemoveUserGroup;
		private System.Windows.Forms.Button btnChangeUserGroup;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtUserGroupName;
		private System.Windows.Forms.Button btnAddUserGroup;
		private System.Windows.Forms.CheckedListBox cblUsers;
		private System.Windows.Forms.ListBox lstUserGroups;
	}
}