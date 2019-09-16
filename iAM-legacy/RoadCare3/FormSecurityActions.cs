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
	public partial class FormSecurityActions : BaseForm
	{
		public FormSecurityActions()
		{
			InitializeComponent();
			LoadActions();
			ClearActionGroups();
		}

		private void LoadActions()
		{
			lstActions.Items.Clear();
			foreach( RoadCareAction action in Global.SecurityOperations.AllActions )
			{
				lstActions.Items.Add( action );
			}
		}
		
		private void ClearActionGroups()
		{
			cblActionGroups.ItemCheck -= new ItemCheckEventHandler( cblActionGroups_ItemCheck );
			cblActionGroups.Items.Clear();
			foreach( RoadCareActionGroup actionGroup in Global.SecurityOperations.AllActionGroups )
			{
				cblActionGroups.Items.Add( actionGroup.Name, CheckState.Unchecked );
			}
			cblActionGroups.ItemCheck += new ItemCheckEventHandler( cblActionGroups_ItemCheck );
		}

		private void lstActions_SelectedIndexChanged( object sender, EventArgs e )
		{
			if( lstActions.SelectedItem != null )
			{
				RoadCareAction selectedAction = ( RoadCareAction )lstActions.SelectedItem;
				LoadActionGroups( selectedAction );
				LoadActionSettings( selectedAction );
			}
			else
			{
				ClearActionGroups();
			}
		}

		private void LoadActionGroups( RoadCareAction action )
		{
			cblActionGroups.ItemCheck -= new ItemCheckEventHandler( cblActionGroups_ItemCheck );
			List<string> memberGroups = Global.SecurityOperations.GetActionGroups( action );

			cblActionGroups.Items.Clear();

			foreach( RoadCareActionGroup group in Global.SecurityOperations.AllActionGroups )
			{
				if( memberGroups.Contains( group.Name ) )
				{
					cblActionGroups.Items.Add( group.Name, CheckState.Checked );
				}
				else
				{
					cblActionGroups.Items.Add( group.Name, CheckState.Unchecked );
				}
			}
			cblActionGroups.ItemCheck += new ItemCheckEventHandler( cblActionGroups_ItemCheck );
		}

		private void LoadActionSettings( RoadCareAction action )
		{
			txtActionType.Text = action.Descriptor["ACTION_TYPE"];
			txtDescription.Text = action.Descriptor["DESCRIPTION"];
			txtNetwork.Text = action.Descriptor["NETWORK_"];
			txtSimulation.Text = action.Descriptor["SIMULATION"];
		}

		private void cblActionGroups_ItemCheck( object sender, ItemCheckEventArgs e )
		{
			if( lstActions.SelectedItem != null )
			{
				if( e.NewValue == CheckState.Checked )
				{
					AssignActionToGroup( ( RoadCareAction )lstActions.SelectedItem, cblActionGroups.Items[e.Index].ToString() );
				}
				else
				{
					RemoveActionFromGroup( ( RoadCareAction )lstActions.SelectedItem, cblActionGroups.Items[e.Index].ToString() );
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

		private void FormSecurityAction_FormClosed( object sender, FormClosedEventArgs e )
		{
			FormManager.RemoveFormSecurityActions();
		}

		private void btnChangeAction_Click( object sender, EventArgs e )
		{
			if( lstActions.SelectedItem != null )
			{
				Global.SecurityOperations.UpdateActionInformation( ( RoadCareAction )lstActions.SelectedItem, txtActionType.Text, txtDescription.Text, txtNetwork.Text, txtSimulation.Text );
				LoadActions();
			}
		}

		private void btnAddAction_Click( object sender, EventArgs e )
		{
			List<PropertyGridEx.CustomProperty> newActionProperties = new List<PropertyGridEx.CustomProperty>();
			string actionType = "";
			string description = "";
			string network = "";
			string simulation = "";
			if( lstActions.SelectedItem != null )
			{
				actionType = ( ( RoadCareAction )lstActions.SelectedItem ).Descriptor["ACTION_TYPE"];
				description = ( ( RoadCareAction )lstActions.SelectedItem ).Descriptor["DESCRIPTION"];
				network = ( ( RoadCareAction )lstActions.SelectedItem ).Descriptor["NETWORK"];
				simulation = ( ( RoadCareAction )lstActions.SelectedItem ).Descriptor["SIMULATION"];
			}
			newActionProperties.Add( new PropertyGridEx.CustomProperty( "Action Type", actionType, false, "Action Settings", "This is the broader category the action falls into.", true ) );
			newActionProperties.Add( new PropertyGridEx.CustomProperty( "Description", description, false, "Action Settings", "This defines the action within the category more specifically.", true ) );
			newActionProperties.Add( new PropertyGridEx.CustomProperty( "Network", network, false, "Action Settings", "This is the networkID to which the action applies.", true ) );
			newActionProperties.Add( new PropertyGridEx.CustomProperty( "Simulation", simulation, false, "Action Settings", "This is the simulationID to which the action applies.", true ) );
			FormSettings actionSettingsWindow = new FormSettings( "Actions", newActionProperties );
			if( actionSettingsWindow.ShowDialog() == DialogResult.OK )
			{
				if( SufficientActionInformation( actionSettingsWindow.Settings ) )
				{
					if( AddAction( actionSettingsWindow.Settings ) )
					{
						lstActions.SelectedItem = lstActions.Items[lstActions.Items.Count - 1];
					}
				}
				else
				{
					Global.WriteOutput( "You must specify an action type when creating a new action." );
				}
			}
		}

		private void btnRemoveAction_Click( object sender, EventArgs e )
		{
			if( lstActions.SelectedItem != null )
			{
				RoadCareAction selectedAction = ( RoadCareAction )lstActions.SelectedItem;
				if( lstActions.SelectedIndex == 0 )
				{
					if( lstActions.Items.Count > 1 )
					{
						lstActions.SelectedIndex++;
					}
				}
				else if( lstActions.SelectedIndex < lstActions.Items.Count - 1 )
				{
					lstActions.SelectedIndex++;
				}
				else
				{
					lstActions.SelectedIndex--;
				}
				lstActions.Items.Remove( selectedAction );

				Global.SecurityOperations.RemoveAction( selectedAction );
			}
		}

		private bool SufficientActionInformation( Dictionary<string, object> dictionary )
		{
			return dictionary["Action Type"].ToString() != "";
		}

		private bool AddAction( Dictionary<string, object> dictionary )
		{
			bool added = false;
			if( Global.SecurityOperations.AddAction(
				dictionary["Action Type"].ToString(),
				dictionary["Description"].ToString(),
				dictionary["Network"].ToString(),
				dictionary["Simulation"].ToString() ) )
			{
				LoadActions();
				added = true;
			}

			return added;
		}

		private void FormSecurityActions_Load( object sender, EventArgs e )
		{
			SecureForm();
		}

		protected void SecureForm()
		{
			LockTextBox( txtActionType );
			LockTextBox( txtDescription );
			LockTextBox( txtNetwork );
			LockTextBox( txtSimulation );
			LockCheckBoxList( cblActionGroups );
			LockButton( btnChangeAction );

			LockButton( btnAddAction );

			LockButton( btnRemoveAction );
			if( Global.SecurityOperations.CanModifySecurityActions() )
			{
				UnlockTextBox( txtActionType );
				UnlockTextBox( txtDescription );
				UnlockTextBox( txtNetwork );
				UnlockTextBox( txtSimulation );
				UnlockButton( btnChangeAction );
				UnlockCheckBoxList( cblActionGroups );
				if( Global.SecurityOperations.CanCreateSecurityActions() )
				{
					UnlockButton( btnAddAction );
					if( Global.SecurityOperations.CanRemoveSecurityActions() )
					{
						UnlockButton( btnRemoveAction );
					}
				}
			}
		}
	}
}
