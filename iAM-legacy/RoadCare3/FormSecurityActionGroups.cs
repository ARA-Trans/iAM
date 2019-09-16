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
	public partial class FormSecurityActionGroups : BaseForm
	{
		public FormSecurityActionGroups()
		{
			InitializeComponent();
			LoadActionGroups();
			ClearActions();
		}

		protected void SecureForm()
		{
			LockTextBox( txtActionGroupName );
			LockCheckBoxList( cblActions );
			LockButton( btnChangeActionGroup );

			LockButton( btnAddActionGroup );
	
			LockButton( btnRemoveActionGroup );
			if( Global.SecurityOperations.CanModifySecurityActionGroups() )
			{
				UnlockTextBox( txtActionGroupName );
				UnlockCheckBoxList( cblActions );
				UnlockButton( btnChangeActionGroup );
				if( Global.SecurityOperations.CanCreateSecurityActionGroups() )
				{
					UnlockButton( btnAddActionGroup );
					if( Global.SecurityOperations.CanRemoveSecurityActionGroups() )
					{
						UnlockButton( btnRemoveActionGroup );
					}
				}
			}
		}

		private void LoadActionGroups()
		{
			lstActionGroups.Items.Clear();
			foreach( RoadCareActionGroup actionGroup in Global.SecurityOperations.AllActionGroups )
			{
				lstActionGroups.Items.Add( actionGroup.Name );
			}
		}

		private void ClearActions()
		{
			cblActions.ItemCheck -= new ItemCheckEventHandler( cblActions_ItemCheck );
			cblActions.Items.Clear();
			foreach( RoadCareAction action in Global.SecurityOperations.AllActions )
			{
				cblActions.Items.Add( action, CheckState.Unchecked );
			}
			cblActions.ItemCheck += new ItemCheckEventHandler( cblActions_ItemCheck );
		}

		private void lstActionGroups_SelectedIndexChanged( object sender, EventArgs e )
		{
			if( lstActionGroups.SelectedItem != null )
			{
				string selectedActionGroup = ( string )lstActionGroups.SelectedItem;
				LoadActions( selectedActionGroup );
				LoadActionGroupSettings( selectedActionGroup );
			}
			else
			{
				ClearActions();
			}
		}

		private void LoadActions( string group )
		{
			cblActions.ItemCheck -= new ItemCheckEventHandler( cblActions_ItemCheck );
			List<RoadCareAction> members = Global.SecurityOperations.GetActionMembers( group );

			cblActions.Items.Clear();

			foreach( RoadCareAction action in Global.SecurityOperations.AllActions )
			{
				if( members.Contains( action ) )
				{
					cblActions.Items.Add( action, CheckState.Checked );
				}
				else
				{
					cblActions.Items.Add( action, CheckState.Unchecked );
				}
			}
			cblActions.ItemCheck += new ItemCheckEventHandler( cblActions_ItemCheck );
		}

		private void LoadActionGroupSettings( string group )
		{
			txtActionGroupName.Text = group;
		}

		private void cblActions_ItemCheck( object sender, ItemCheckEventArgs e )
		{
			if( lstActionGroups.SelectedItem != null )
			{
				if( e.NewValue == CheckState.Checked )
				{
					AssignActionToGroup( ( RoadCareAction )cblActions.Items[e.Index], lstActionGroups.SelectedItem.ToString() );
				}
				else
				{
					RemoveActionFromGroup( ( RoadCareAction )cblActions.Items[e.Index], lstActionGroups.SelectedItem.ToString() );
				}
			}
		}

		private void AssignActionToGroup( RoadCareAction action, string groupName )
		{
			Global.SecurityOperations.AddActionToGroup( action, groupName );
		}

		private void RemoveActionFromGroup( RoadCareAction action, string groupName )
		{
			Global.SecurityOperations.RemoveActionFromGroup( action, groupName );
		}

		private void btnChangeActionGroup_Click( object sender, EventArgs e )
		{
			if( lstActionGroups.SelectedItem != null )
			{
				Global.SecurityOperations.UpdateActionGroupInformation( lstActionGroups.SelectedItem.ToString(), txtActionGroupName.Text );
				LoadActionGroups();
			}
		}

		private void btnAddActionGroup_Click( object sender, EventArgs e )
		{
			List<PropertyGridEx.CustomProperty> newActionGroupProperties = new List<PropertyGridEx.CustomProperty>();
			newActionGroupProperties.Add( new PropertyGridEx.CustomProperty( "Actiongroup Name", "", false, "Actiongroup Settings", "This is the name of the group.", true ) );
			FormSettings actionGroupSettingsWindow = new FormSettings( "Actions", newActionGroupProperties );
			if( actionGroupSettingsWindow.ShowDialog() == DialogResult.OK )
			{
				if( SufficientActionGroupInformation( actionGroupSettingsWindow.Settings ) )
				{
					AddActionGroup( actionGroupSettingsWindow.Settings );
				}
				else
				{
					Global.WriteOutput( "All fields are required for creating a new actiongroup." );
				}
			}
		}

		private bool SufficientActionGroupInformation( Dictionary<string, object> setting )
		{
			return setting["Actiongroup Name"].ToString() != "";
		}

		private void AddActionGroup( Dictionary<string, object> dictionary )
		{
			if( Global.SecurityOperations.AddActionGroup( dictionary["Actiongroup Name"].ToString() ) )
			{
				LoadActionGroups();
			}
		}

		private void btnRemoveActionGroup_Click( object sender, EventArgs e )
		{
			if( lstActionGroups.SelectedItem != null )
			{
				string selectedActionGroup = lstActionGroups.SelectedItem.ToString();
				lstActionGroups.Items.Remove( selectedActionGroup );
				Global.SecurityOperations.RemoveActionGroup( selectedActionGroup );
			}
		}

		private void FormSecurityActionGroups_FormClosed( object sender, FormClosedEventArgs e )
		{
			FormManager.RemoveFormSecurityActionGroups();
		}

		private void FormSecurityActionGroups_Load( object sender, EventArgs e )
		{
			SecureForm();
		}


	}
}
