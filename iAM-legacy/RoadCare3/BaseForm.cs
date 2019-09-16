using System;
using System.Collections.Generic;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using RoadCare3.Properties;
using DatabaseManager;
using System.Collections;
using System.Windows.Forms;
using DataObjects;
using RoadCareGlobalOperations;

namespace RoadCare3
{
	public class BaseForm : DockContent
	{
		protected String nodeMatchText;
		protected String inactiveIconKey;
		protected String inactiveSelectedIconKey;
        private bool m_bImageView = false;
        private NetworkObject m_networkObject;
        private bool m_bStop = false;
        public EventPublisher m_event;
        DockPanel m_DockPanel;

        public DockPanel DockingPanel
        {
            get { return m_DockPanel; }
            set { m_DockPanel = value; }
        }

        public NetworkObject Network
        {
            get { return m_networkObject; }
            set { m_networkObject = value; }
        }

        public bool ImageView
        {
            get { return m_bImageView; }
            set { m_bImageView = value; }
        }

        public bool Stop
        {
            get { return m_bStop; }
            set { m_bStop = value; }
        }

		/// <summary>
		/// Sets up generic form parameters.
		/// </summary>
		/// <param name="inactiveImage">This is the ImageKey the associated node should have</param>
		/// <param name="selectedInactiveImage">This is the SelectedImageKey the associated node should have</param>
		protected void FormLoad(String inactiveImage, String selectedInactiveImage)
		{
            if (FormManager.GetSolutionExplorer() != null)//ROADCARE
            {
                nodeMatchText = (string)FormManager.GetSolutionExplorer().GetTreeView().SelectedNode.Tag;
                inactiveIconKey = inactiveImage;
                inactiveSelectedIconKey = selectedInactiveImage;
                ChangeAssociatedIcon(nodeMatchText, Settings.Default.ACTIVE_IMAGE_KEY, Settings.Default.ACTIVE_IMAGE_KEY_SELECTED);
            }
            else //IMAGEVIEW
            {

            }
		}

		protected void FormUnload()
		{
			ChangeAssociatedIcon(nodeMatchText, inactiveIconKey, inactiveSelectedIconKey);
		}

		protected void ChangeAssociatedIcon( String matchText, String imageKey, String selectedImageKey )
		{
			try
			{
				FormManager.GetSolutionExplorer().ChangeIcon( matchText, imageKey, selectedImageKey);
			}
			catch
			{
			}
		}

		protected void LockContextMenuStrip( ContextMenuStrip cms )
		{
			foreach( ToolStripMenuItem tsi in cms.Items )
			{
				LockToolStripMenuItem( tsi );
			}
		}


		
		protected void LockDataGridView( DataGridView dgv )
		{
			dgv.ReadOnly = true;
			dgv.AllowUserToAddRows = false;
			dgv.AllowUserToDeleteRows = false;
		}

		protected void LockTextBox( TextBox txt )
		{
			txt.ReadOnly = true;
		}

		protected void LockComboBox( ComboBox cmb )
		{
			cmb.Enabled = false;
		}

		protected void LockButton( Button btn )
		{
			btn.Enabled = false;
		}

		protected void LockCheckBox( CheckBox chb )
		{
			chb.Enabled = false;
		}

		protected void LockToolStripButton( ToolStripButton tsb )
		{
			tsb.Enabled = false;
		}

		protected void LockToolStripMenuItem( ToolStripMenuItem tsmi )
		{
			tsmi.Enabled = false;
		}

		protected void LockBindingNavigator( BindingNavigator bn )
		{
			bn.AddNewItem.Enabled = false;
			bn.DeleteItem.Enabled = false;
			//( ( ToolStripButton )bn.AddNewItem ).Enabled = false;
			//( ( ToolStripButton )bn.DeleteItem ).Enabled = false;
			//bn.AddNewItem = null;
			//bn.DeleteItem = null;

			bn.AddNewItem.Visible = false;
			bn.DeleteItem.Visible = false;
		}

		protected void LockCheckBoxList( CheckedListBox cbl )
		{
			cbl.SelectionMode = SelectionMode.None;
		}

		protected void UnlockCheckBoxList( CheckedListBox cbl )
		{
			cbl.SelectionMode = SelectionMode.One;
		}		

		protected void UnlockBindingNavigatorForCreateDestroy( BindingNavigator bn )
		{
			UnlockBindingNavigatorForCreate( bn );
			bn.DeleteItem.Enabled = true;
			bn.DeleteItem.Visible = true;
		}

		protected void UnlockBindingNavigatorForCreate( BindingNavigator bn )
		{
			bn.AddNewItem.Enabled = true;
			bn.AddNewItem.Visible = true;
		}
		
		
		protected void UnlockToolStripMenuItem( ToolStripMenuItem tsmi )
		{
			tsmi.Enabled = true;
		}

		protected void UnlockToolStripButton( ToolStripButton tsb )
		{
			tsb.Enabled = true;
		}

		protected void UnlockButton( Button btn )
		{
			btn.Enabled = true;
		}
	
		protected void UnlockDataGridViewForCreateDestroy( DataGridView dgv )
		{
			UnlockDataGridViewForCreate( dgv );
			dgv.AllowUserToDeleteRows = true;
		}

		protected void UnlockDataGridViewForCreate( DataGridView dgv )
		{
			UnlockDataGridViewForModify( dgv );
			dgv.AllowUserToAddRows = true;
		}

		protected void UnlockDataGridViewForModify( DataGridView dgv )
		{
			dgv.ReadOnly = false;
		}

		protected void UnlockTextBox( TextBox txt )
		{
			txt.ReadOnly = false;
		}

		protected void UnlockComboBox( ComboBox cmb )
		{
			cmb.Enabled = true;
		}


		protected void UnlockCheckBox( CheckBox chb )
		{
			chb.Enabled = true;
		}

        public virtual void UpdateNode(DockPanel dockPanel, object ob)
        {
            if (!this.ImageView)
            {
                this.ImageView = true;
                this.HideOnClose = true;
            }
            this.m_DockPanel = dockPanel;
            this.Show(dockPanel); 
        }

        public virtual void NavigationTick(NavigationObject navigationObject)
        {
        }

        public virtual bool IsNavigationStop()
        {
            if (m_bStop)
            {
                m_bStop = false;
                return true;
            }
            else
            {
                return false;
            }
        }

	}
}
