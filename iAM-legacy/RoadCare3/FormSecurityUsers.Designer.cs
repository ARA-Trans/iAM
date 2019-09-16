namespace RoadCare3
{
	partial class FormSecurityUsers
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
			this.lstUsers = new System.Windows.Forms.ListBox();
			this.cblUserGroups = new System.Windows.Forms.CheckedListBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtSecurityAnswer = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtSecurityQuestion = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtEmail = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtUserName = new System.Windows.Forms.TextBox();
			this.btnAddUser = new System.Windows.Forms.Button();
			this.btnChangeUser = new System.Windows.Forms.Button();
			this.btnRemoveUser = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lstUsers
			// 
			this.lstUsers.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left ) ) );
			this.lstUsers.FormattingEnabled = true;
			this.lstUsers.Location = new System.Drawing.Point( 8, 7 );
			this.lstUsers.Name = "lstUsers";
			this.lstUsers.Size = new System.Drawing.Size( 344, 602 );
			this.lstUsers.TabIndex = 0;
			this.lstUsers.SelectedIndexChanged += new System.EventHandler( this.lstUsers_SelectedIndexChanged );
			// 
			// cblUserGroups
			// 
			this.cblUserGroups.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
						| System.Windows.Forms.AnchorStyles.Left ) ) );
			this.cblUserGroups.CheckOnClick = true;
			this.cblUserGroups.FormattingEnabled = true;
			this.cblUserGroups.Location = new System.Drawing.Point( 358, 7 );
			this.cblUserGroups.Name = "cblUserGroups";
			this.cblUserGroups.Size = new System.Drawing.Size( 434, 634 );
			this.cblUserGroups.TabIndex = 1;
			this.cblUserGroups.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler( this.cblUserGroups_ItemCheck );
			// 
			// label5
			// 
			this.label5.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point( 7, 713 );
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size( 83, 13 );
			this.label5.TabIndex = 32;
			this.label5.Text = "Security Answer";
			// 
			// txtSecurityAnswer
			// 
			this.txtSecurityAnswer.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.txtSecurityAnswer.Location = new System.Drawing.Point( 103, 710 );
			this.txtSecurityAnswer.Name = "txtSecurityAnswer";
			this.txtSecurityAnswer.Size = new System.Drawing.Size( 323, 20 );
			this.txtSecurityAnswer.TabIndex = 31;
			// 
			// label4
			// 
			this.label4.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point( 7, 687 );
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size( 90, 13 );
			this.label4.TabIndex = 30;
			this.label4.Text = "Security Question";
			// 
			// txtSecurityQuestion
			// 
			this.txtSecurityQuestion.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.txtSecurityQuestion.Location = new System.Drawing.Point( 103, 684 );
			this.txtSecurityQuestion.Name = "txtSecurityQuestion";
			this.txtSecurityQuestion.Size = new System.Drawing.Size( 323, 20 );
			this.txtSecurityQuestion.TabIndex = 29;
			// 
			// label3
			// 
			this.label3.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point( 7, 661 );
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size( 32, 13 );
			this.label3.TabIndex = 28;
			this.label3.Text = "Email";
			// 
			// txtEmail
			// 
			this.txtEmail.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.txtEmail.Location = new System.Drawing.Point( 66, 658 );
			this.txtEmail.Name = "txtEmail";
			this.txtEmail.Size = new System.Drawing.Size( 129, 20 );
			this.txtEmail.TabIndex = 27;
			// 
			// label1
			// 
			this.label1.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point( 5, 634 );
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size( 55, 13 );
			this.label1.TabIndex = 25;
			this.label1.Text = "Username";
			// 
			// txtUserName
			// 
			this.txtUserName.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.txtUserName.Location = new System.Drawing.Point( 66, 632 );
			this.txtUserName.Name = "txtUserName";
			this.txtUserName.Size = new System.Drawing.Size( 129, 20 );
			this.txtUserName.TabIndex = 22;
			// 
			// btnAddUser
			// 
			this.btnAddUser.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.btnAddUser.Location = new System.Drawing.Point( 113, 748 );
			this.btnAddUser.Name = "btnAddUser";
			this.btnAddUser.Size = new System.Drawing.Size( 82, 20 );
			this.btnAddUser.TabIndex = 23;
			this.btnAddUser.Text = "Add User";
			this.btnAddUser.UseVisualStyleBackColor = true;
			this.btnAddUser.Click += new System.EventHandler( this.btnAddUser_Click );
			// 
			// btnChangeUser
			// 
			this.btnChangeUser.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
			this.btnChangeUser.Location = new System.Drawing.Point( 8, 748 );
			this.btnChangeUser.Name = "btnChangeUser";
			this.btnChangeUser.Size = new System.Drawing.Size( 82, 20 );
			this.btnChangeUser.TabIndex = 33;
			this.btnChangeUser.Text = "Update User";
			this.btnChangeUser.UseVisualStyleBackColor = true;
			this.btnChangeUser.Click += new System.EventHandler( this.btnChangeUser_Click );
			// 
			// btnRemoveUser
			// 
			this.btnRemoveUser.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.btnRemoveUser.Location = new System.Drawing.Point( 710, 745 );
			this.btnRemoveUser.Name = "btnRemoveUser";
			this.btnRemoveUser.Size = new System.Drawing.Size( 82, 20 );
			this.btnRemoveUser.TabIndex = 34;
			this.btnRemoveUser.Text = "Remove User";
			this.btnRemoveUser.UseVisualStyleBackColor = true;
			this.btnRemoveUser.Click += new System.EventHandler( this.btnRemoveUser_Click );
			// 
			// FormSecurityUsers
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 804, 777 );
			this.Controls.Add( this.btnRemoveUser );
			this.Controls.Add( this.btnChangeUser );
			this.Controls.Add( this.label5 );
			this.Controls.Add( this.txtSecurityAnswer );
			this.Controls.Add( this.label4 );
			this.Controls.Add( this.txtSecurityQuestion );
			this.Controls.Add( this.label3 );
			this.Controls.Add( this.txtEmail );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.txtUserName );
			this.Controls.Add( this.btnAddUser );
			this.Controls.Add( this.cblUserGroups );
			this.Controls.Add( this.lstUsers );
			this.Name = "FormSecurityUsers";
			this.TabText = "Users";
			this.Text = "Users";
			this.Load += new System.EventHandler( this.FormSecurityUsers_Load );
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.FormSecurityUsers_FormClosed );
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox lstUsers;
		private System.Windows.Forms.CheckedListBox cblUserGroups;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtSecurityAnswer;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtSecurityQuestion;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtEmail;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtUserName;
		private System.Windows.Forms.Button btnAddUser;
		private System.Windows.Forms.Button btnChangeUser;
		private System.Windows.Forms.Button btnRemoveUser;
	}
}