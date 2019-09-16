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
	public partial class FormSecurityUserGroups : BaseForm
	{
		public FormSecurityUserGroups()
		{
			InitializeComponent();
			LoadUserGroups();
			ClearUsers();
			SecureForm();
		}

		protected void SecureForm()
		{
			LockTextBox( txtUserGroupName );
			LockCheckBoxList( cblUsers );
			LockButton( btnAddUserGroup );
			LockButton( btnChangeUserGroup );
			LockButton( btnRemoveUserGroup );

			if( Global.SecurityOperations.CanModifySecurityUserGroups() )
			{
				UnlockTextBox( txtUserGroupName );
				UnlockCheckBoxList( cblUsers );
				UnlockButton( btnChangeUserGroup );
				if( Global.SecurityOperations.CanCreateSecurityUserGroups() )
				{
					UnlockButton( btnAddUserGroup );
					if( Global.SecurityOperations.CanRemoveSecurityUserGroups() )
					{
						UnlockButton( btnRemoveUserGroup );
					}
				}
			}
		}

		private void LoadUserGroups()
		{
			lstUserGroups.Items.Clear();
			foreach( RoadCareUserGroup userGroup in Global.SecurityOperations.AllUserGroups )
			{
				lstUserGroups.Items.Add( userGroup.Name );
			}
		}

		private void ClearUsers()
		{
			cblUsers.ItemCheck -= new ItemCheckEventHandler( cblUsers_ItemCheck );
			cblUsers.Items.Clear();
			foreach( RoadCareUser user in Global.SecurityOperations.AllUsers )
			{
				cblUsers.Items.Add( user, CheckState.Unchecked );
			}
			cblUsers.ItemCheck += new ItemCheckEventHandler( cblUsers_ItemCheck );
		}

		private void lstUserGroups_SelectedIndexChanged( object sender, EventArgs e )
		{
			if( lstUserGroups.SelectedItem != null )
			{
				string selectedUserGroup = ( string )lstUserGroups.SelectedItem;
				LoadUsers( selectedUserGroup );
				LoadUserGroupSettings( selectedUserGroup );
			}
			else
			{
				ClearUsers();
			}
		}

		private void LoadUsers( string group )
		{
			cblUsers.ItemCheck -= new ItemCheckEventHandler( cblUsers_ItemCheck );
			List<RoadCareUser> members = Global.SecurityOperations.GetUserMembers( group );

			cblUsers.Items.Clear();

			foreach( RoadCareUser user in Global.SecurityOperations.AllUsers )
			{
				if( members.Contains( user ) )
				{
					cblUsers.Items.Add( user, CheckState.Checked );
				}
				else
				{
					cblUsers.Items.Add( user, CheckState.Unchecked );
				}
			}
			cblUsers.ItemCheck += new ItemCheckEventHandler( cblUsers_ItemCheck );
		}

		private void LoadUserGroupSettings( string group )
		{
			txtUserGroupName.Text = group;
		}

		private void cblUsers_ItemCheck( object sender, ItemCheckEventArgs e )
		{
			if( lstUserGroups.SelectedItem != null )
			{
				if( e.NewValue == CheckState.Checked )
				{
					AssignUserToGroup( ( RoadCareUser )cblUsers.Items[e.Index], lstUserGroups.SelectedItem.ToString() );
				}
				else
				{
					RemoveUserFromGroup( ( RoadCareUser )cblUsers.Items[e.Index], lstUserGroups.SelectedItem.ToString() );
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

		private void btnChangeUserGroup_Click( object sender, EventArgs e )
		{
			if( lstUserGroups.SelectedItem != null )
			{
				Global.SecurityOperations.UpdateUserGroupInformation( lstUserGroups.SelectedItem.ToString(), txtUserGroupName.Text );
				LoadUserGroups();
			}
		}

		private void btnAddUserGroup_Click( object sender, EventArgs e )
		{
			List<PropertyGridEx.CustomProperty> newUserGroupProperties = new List<PropertyGridEx.CustomProperty>();
			newUserGroupProperties.Add( new PropertyGridEx.CustomProperty( "Usergroup Name", "", false, "Usergroup Settings", "This is the name of the group.", true ) );
			FormSettings userGroupSettingsWindow = new FormSettings( "Users", newUserGroupProperties );
			if( userGroupSettingsWindow.ShowDialog() == DialogResult.OK )
			{
				if( SufficientUserGroupInformation( userGroupSettingsWindow.Settings ) )
				{
					AddUserGroup( userGroupSettingsWindow.Settings );
				}
				else
				{
					Global.WriteOutput( "All fields are required for creating a new usergroup." );
				}
			}
		}

		private bool SufficientUserGroupInformation( Dictionary<string, object> setting )
		{
			return setting["Usergroup Name"].ToString() != "";
		}

		private void AddUserGroup( Dictionary<string, object> dictionary )
		{
			if( Global.SecurityOperations.AddUserGroup( dictionary["Usergroup Name"].ToString() ) )
			{
				LoadUserGroups();
			}
		}

		private void btnRemoveUserGroup_Click( object sender, EventArgs e )
		{
			if( lstUserGroups.SelectedItem != null )
			{
				string selectedUserGroup = lstUserGroups.SelectedItem.ToString();
				lstUserGroups.Items.Remove( selectedUserGroup );
				Global.SecurityOperations.RemoveUserGroup( selectedUserGroup );
			}
		}

		private void FormSecurityUserGroups_FormClosed( object sender, FormClosedEventArgs e )
		{
			FormManager.RemoveFormSecurityUserGroups();
		}

	}
}
