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
	public partial class FormSecurityPermissions : BaseForm
	{
		public FormSecurityPermissions()
		{
			InitializeComponent();
			SetUpLists();
			//for( AccessLevel level = AccessLevel.None; level <= AccessLevel.CreateDestroy; ++level )
			//{
			//    cmbPermission.Items.Add( level );
			//}
			//foreach( RoadCareUser user in Global.SecurityOperations.AllUsers )
			//{
			//    lstUsers.Items.Add( user );
			//}
			//foreach( RoadCareAction action in Global.SecurityOperations.AllActions )
			//{
			//    lstActions.Items.Add( action );
			//}
			//foreach( string userGroup in Global.SecurityOperations.AllUserGroups )
			//{
			//    lstUserGroups.Items.Add( userGroup );
			//}
			//foreach( string actionGroup in Global.SecurityOperations.AllActionGroups )
			//{
			//    lstActionGroups.Items.Add( actionGroup );
			//}
		}

		private void FormSecurityPermissions_Load( object sender, EventArgs e )
		{
			SecureForm();
		}

		private void lstUserGroups_SelectedIndexChanged( object sender, EventArgs e )
		{
			lstUsers.SelectedIndexChanged -= new EventHandler( lstUsers_SelectedIndexChanged );
			lstUsers.ClearSelected();
			SetPermissionsBox();
			lstUsers.SelectedIndexChanged += new EventHandler( lstUsers_SelectedIndexChanged );
		}

		private void lstUsers_SelectedIndexChanged( object sender, EventArgs e )
		{
			lstUserGroups.SelectedIndexChanged -= new EventHandler( lstUserGroups_SelectedIndexChanged );
			lstUserGroups.ClearSelected();
			SetPermissionsBox();
			lstUserGroups.SelectedIndexChanged += new EventHandler( lstUserGroups_SelectedIndexChanged );
		}

		private void lstActionGroups_SelectedIndexChanged( object sender, EventArgs e )
		{
			lstActions.SelectedIndexChanged -= new EventHandler( lstActions_SelectedIndexChanged );
			lstActions.ClearSelected();
			SetPermissionsBox();
			lstActions.SelectedIndexChanged += new EventHandler( lstActions_SelectedIndexChanged );
		}

		private void lstActions_SelectedIndexChanged( object sender, EventArgs e )
		{
			lstActionGroups.SelectedIndexChanged -= new EventHandler( lstActionGroups_SelectedIndexChanged );
			lstActionGroups.ClearSelected();
			SetPermissionsBox();
			lstActionGroups.SelectedIndexChanged += new EventHandler( lstActionGroups_SelectedIndexChanged );
		}

		private void SetPermissionsBox()
		{
			if( lstUsers.SelectedItem != null )
			{
				if( lstActions.SelectedItem != null )
				{
					cmbPermission.SelectedIndexChanged -= new EventHandler( cmbPermission_SelectedIndexChanged );
					cmbPermission.Text = Global.SecurityOperations.GetUserActionPermissions( ( RoadCareUser )lstUsers.SelectedItem, ( RoadCareAction )lstActions.SelectedItem );
					cmbPermission.SelectedIndexChanged += new EventHandler( cmbPermission_SelectedIndexChanged );
				}
				else if( lstActionGroups.SelectedItem != null )
				{
					cmbPermission.SelectedIndexChanged -= new EventHandler( cmbPermission_SelectedIndexChanged );
					cmbPermission.Text = Global.SecurityOperations.GetUserActionGroupPermissions( ( RoadCareUser )lstUsers.SelectedItem, lstActionGroups.SelectedItem.ToString() );
					cmbPermission.SelectedIndexChanged += new EventHandler( cmbPermission_SelectedIndexChanged );
				}
			}
			else if( lstUserGroups.SelectedItem != null )
			{
				if( lstActions.SelectedItem != null )
				{
					cmbPermission.SelectedIndexChanged -= new EventHandler( cmbPermission_SelectedIndexChanged );
					cmbPermission.Text = Global.SecurityOperations.GetUserGroupActionPermissions( lstUserGroups.SelectedItem.ToString(), ( RoadCareAction )lstActions.SelectedItem );
					cmbPermission.SelectedIndexChanged += new EventHandler( cmbPermission_SelectedIndexChanged );
				}
				else if( lstActionGroups.SelectedItem != null )
				{
					cmbPermission.SelectedIndexChanged -= new EventHandler( cmbPermission_SelectedIndexChanged );
					cmbPermission.Text = Global.SecurityOperations.GetUserGroupActionGroupPermissions( lstUserGroups.SelectedItem.ToString(), lstActionGroups.SelectedItem.ToString() );
					cmbPermission.SelectedIndexChanged += new EventHandler( cmbPermission_SelectedIndexChanged );
				}
			}
		}

		private void FormSecurityPermissions_FormClosed( object sender, FormClosedEventArgs e )
		{
			FormManager.RemoveFormSecurityPermissions();
		}

		protected void SecureForm()
		{
			LockComboBox( cmbPermission );
			if( Global.SecurityOperations.CanModifySecurityPermissions() )
			{
				UnlockComboBox( cmbPermission );
			}
		}

		private void cmbPermission_SelectedIndexChanged( object sender, EventArgs e )
		{
			if( lstUsers.SelectedItem != null )
			{
				if( lstActions.SelectedItem != null )
				{
					Global.SecurityOperations.SetUserActionPermissions( ( RoadCareUser )lstUsers.SelectedItem, ( RoadCareAction )lstActions.SelectedItem, cmbPermission.Text );
				}
				else if( lstActionGroups.SelectedItem != null )
				{
					Global.SecurityOperations.SetUserActionGroupPermissions( ( RoadCareUser )lstUsers.SelectedItem, lstActionGroups.SelectedItem.ToString(), cmbPermission.Text );
				}
			}
			else if( lstUserGroups.SelectedItem != null )
			{
				if( lstActions.SelectedItem != null )
				{
					Global.SecurityOperations.SetUserGroupActionPermissions( lstUserGroups.SelectedItem.ToString(), ( RoadCareAction )lstActions.SelectedItem, cmbPermission.Text );
				}
				else if( lstActionGroups.SelectedItem != null )
				{
					Global.SecurityOperations.SetUserGroupActionGroupPermissions( lstUserGroups.SelectedItem.ToString(), lstActionGroups.SelectedItem.ToString(), cmbPermission.Text );
				}
			}
		}

		private void FormSecurityPermissions_Enter( object sender, EventArgs e )
		{
			SetUpLists();
		}

		private void SetUpLists()
		{
			cmbPermission.Items.Clear();
			lstUsers.Items.Clear();
			lstActions.Items.Clear();
			lstUserGroups.Items.Clear();
			lstActionGroups.Items.Clear();
			for( AccessLevel level = AccessLevel.None; level <= AccessLevel.CreateDestroy; ++level )
			{
				cmbPermission.Items.Add( level );
			}
			foreach( RoadCareUser user in Global.SecurityOperations.AllUsers )
			{
				lstUsers.Items.Add( user );
			}
			foreach( RoadCareAction action in Global.SecurityOperations.AllActions )
			{
				lstActions.Items.Add( action );
			}
			foreach( RoadCareUserGroup userGroup in Global.SecurityOperations.AllUserGroups )
			{
				lstUserGroups.Items.Add( userGroup.Name );
			}
			foreach( RoadCareActionGroup actionGroup in Global.SecurityOperations.AllActionGroups )
			{
				lstActionGroups.Items.Add( actionGroup.Name );
			}
		}
	}
}
