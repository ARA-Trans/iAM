using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RoadCareSecurityOperations;

namespace RoadCare3
{
	public partial class FormSecurityUsers : BaseForm
	{
		public FormSecurityUsers()
		{
			InitializeComponent();
			LoadUsers();
			ClearUserGroups();
		}

		private void LoadUsers()
		{
			lstUsers.Items.Clear();
			foreach( RoadCareUser user in Global.SecurityOperations.AllUsers )
			{
				lstUsers.Items.Add( user );
			}
		}

		private void ClearUserGroups()
		{
			cblUserGroups.ItemCheck -= new ItemCheckEventHandler( cblUserGroups_ItemCheck );
			cblUserGroups.Items.Clear();
			foreach( RoadCareUserGroup group in Global.SecurityOperations.AllUserGroups )
			{
				cblUserGroups.Items.Add( group.Name, CheckState.Unchecked );
			}
			cblUserGroups.ItemCheck += new ItemCheckEventHandler( cblUserGroups_ItemCheck );
		}

		private void lstUsers_SelectedIndexChanged( object sender, EventArgs e )
		{
			if( lstUsers.SelectedItem != null )
			{
				RoadCareUser selectedUser = ( RoadCareUser )lstUsers.SelectedItem;
				LoadUserGroups( selectedUser );
				LoadUserSettings( selectedUser );
			}
			else
			{
				ClearUserGroups();
			}
		}

		private void LoadUserGroups( RoadCareUser user )
		{
			cblUserGroups.ItemCheck -= new ItemCheckEventHandler( cblUserGroups_ItemCheck );
			List<string> memberGroups = Global.SecurityOperations.GetUserGroups( user );

			cblUserGroups.Items.Clear();

			foreach( RoadCareUserGroup group in Global.SecurityOperations.AllUserGroups )
			{
				if( memberGroups.Contains( group.Name ) )
				{
					cblUserGroups.Items.Add( group.Name, CheckState.Checked );
				}
				else
				{
					cblUserGroups.Items.Add( group.Name, CheckState.Unchecked );
				}
			}
			cblUserGroups.ItemCheck += new ItemCheckEventHandler( cblUserGroups_ItemCheck );
		}

		private void LoadUserSettings( RoadCareUser user )
		{
			txtUserName.Text = user.Descriptor["USER_LOGIN"];
			txtEmail.Text = user.Descriptor["EMAIL"];
			txtSecurityQuestion.Text = user.Descriptor["PASSWORD_QUESTION"];
			txtSecurityAnswer.Text = user.Descriptor["PASSWORD_ANSWER"];
		}

		private void cblUserGroups_ItemCheck( object sender, ItemCheckEventArgs e )
		{
			if( lstUsers.SelectedItem != null )
			{
				if( e.NewValue == CheckState.Checked )
				{
					AssignUserToGroup( ( RoadCareUser )lstUsers.SelectedItem, cblUserGroups.Items[e.Index].ToString() );
				}
				else
				{
					RemoveUserFromGroup( ( RoadCareUser )lstUsers.SelectedItem, cblUserGroups.Items[e.Index].ToString() );
				}
			}
		}

		private void AssignUserToGroup( RoadCareUser user, string groupName )
		{
			Global.SecurityOperations.AddUserToGroup( user, groupName );
		}

		private void RemoveUserFromGroup( RoadCareUser user, string groupName )
		{
			Global.SecurityOperations.RemoveUserFromGroup( user, groupName );
		}

		private void FormSecurityUsers_FormClosed( object sender, FormClosedEventArgs e )
		{
			FormManager.RemoveFormSecurityUsers();
		}

		private void btnChangeUser_Click( object sender, EventArgs e )
		{
			if( lstUsers.SelectedItem != null )
			{
				Global.SecurityOperations.UpdateUserInformation( ( RoadCareUser )lstUsers.SelectedItem, txtUserName.Text, txtEmail.Text, txtSecurityQuestion.Text, txtSecurityAnswer.Text );
			}
		}

		private void btnAddUser_Click( object sender, EventArgs e )
		{
			List<PropertyGridEx.CustomProperty> newUserProperties = new List<PropertyGridEx.CustomProperty>();
			newUserProperties.Add( new PropertyGridEx.CustomProperty( "User Name", "", false, "User Settings", "This is the username used to log into RoadCare.", true ));
			newUserProperties.Add( new PropertyGridEx.CustomProperty( "Password", "", false, "User Settings", "This is the password used to log into RoadCare.", true ));
			newUserProperties.Add( new PropertyGridEx.CustomProperty( "Email", "", false, "User Settings", "This is the email the confirmation email will be sent to.", true ));
			newUserProperties.Add( new PropertyGridEx.CustomProperty( "Security Question", "", false, "User Settings", "This is the security question used to retrieve the password.", true ));
			newUserProperties.Add( new PropertyGridEx.CustomProperty( "Security Answer", "", false, "User Settings", "This is the answer to the security question.", true ) );
			FormSettings userSettingsWindow = new FormSettings( "Users", newUserProperties );
			if( userSettingsWindow.ShowDialog() == DialogResult.OK )
			{
				if( SufficientUserInformation( userSettingsWindow.Settings ) )
				{
					AddUser( userSettingsWindow.Settings );
				}
				else
				{
					Global.WriteOutput( "All fields are required for creating a new user." );
				}
			}
		}

		private void btnRemoveUser_Click( object sender, EventArgs e )
		{
			if( lstUsers.SelectedItem != null )
			{
				RoadCareUser selectedUser = ( RoadCareUser )lstUsers.SelectedItem;
				lstUsers.Items.Remove( selectedUser );
				Global.SecurityOperations.RemoveUser( selectedUser );
			}
		}

		private bool SufficientUserInformation( Dictionary<string, object> userDescriptor )
		{
			return userDescriptor["User Name"].ToString() != "" &&
				userDescriptor["Password"].ToString() != "" &&
				userDescriptor["Email"].ToString() != "" &&
				userDescriptor["Security Question"].ToString() != "" &&
				userDescriptor["Security Answer"].ToString() != "";
		}

		private void AddUser( Dictionary<string, object> userDescriptor )
		{
			if( Global.SecurityOperations.AddUser(
				userDescriptor["User Name"].ToString(),
				userDescriptor["Password"].ToString(),
				userDescriptor["Email"].ToString(),
				userDescriptor["Security Question"].ToString(),
				userDescriptor["Security Answer"].ToString() ) )
			{
				LoadUsers();
			}
		}

		private void FormSecurityUsers_Load( object sender, EventArgs e )
		{
			SecureForm();
		}

		protected void SecureForm()
		{
			LockTextBox( txtEmail );
			LockTextBox( txtSecurityAnswer );
			LockTextBox( txtSecurityQuestion );
			LockTextBox( txtUserName );
			LockButton( btnChangeUser );
			LockCheckBoxList( cblUserGroups );

			LockButton( btnAddUser );

			LockButton( btnRemoveUser );

			if( Global.SecurityOperations.CanModifySecurityUsers() )
			{
				UnlockTextBox( txtEmail );
				UnlockTextBox( txtSecurityAnswer );
				UnlockTextBox( txtSecurityQuestion );
				UnlockTextBox( txtUserName );
				UnlockButton( btnChangeUser );
				UnlockCheckBoxList( cblUserGroups );

				if( Global.SecurityOperations.CanCreateSecurityUsers() )
				{
					UnlockButton( btnAddUser );
					if( Global.SecurityOperations.CanRemoveSecurityUsers() )
					{
						UnlockButton( btnRemoveUser );
					}
				}
			}
		}

	}
}
