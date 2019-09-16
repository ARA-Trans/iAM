using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;
using System.Data.SqlClient;
using System.Collections;
using WeifenLuo.WinFormsUI.Docking;
using DynamicSegmentation;
using RoadCare3.Properties;
namespace RoadCare3
{
    public partial class FormSegmentation : BaseForm
    {
        private String m_strNetwork;
        Hashtable m_hashFamily = new Hashtable();
        private String m_strNetworkID;
        BindingSource binding;
        DataAdapter dataAdapter;
        DataTable table;
        private bool m_bEditSegmentation = false;
#region "Icon Switching"
        //dsmelser 2008.07.29
        //prerequisites for handling icon changing on form-opening
        public TreeNode associatedNode;//so far only used for setting the lightbulb on the selected node

#endregion
        
        /// <summary>
        /// Opens the interface to allow dynamic segmentation of the network.
        /// </summary>
        /// <param name="strNetwork">Name of the network on which segmentation is to occur.</param>
        public FormSegmentation(String strNetwork)
        {
            InitializeComponent();
            m_strNetwork = strNetwork;

            String strSelect = "SELECT NETWORKID FROM NETWORKS WHERE NETWORK_NAME='" + m_strNetwork + "'";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                m_strNetworkID = ds.Tables[0].Rows[0].ItemArray[0].ToString();

            }
            catch (Exception e)
            {
                String strError = "Error in getting NETWORKID for NETWORK_NAME = " + m_strNetwork + ". SQL error = " + e.Message;
                Global.WriteOutput(strError);
            }

        }

		protected void SecureForm()
		{
			checkBoxNetworkSegment.Checked = false;
			LockCheckBox( checkBoxNetworkSegment );
			LockButton( buttonAddChild );
			LockButton( buttonAddRoot );
			LockButton( buttonSegment );
			LockButton( buttonRemove );
			LockButton( buttonNew );
			LockButton( buttonEditSubset );
			LockButton( buttonSegmentNetwork );
			if( Global.SecurityOperations.CanModifySegmentationLogic( m_strNetworkID ) )
			{
				UnlockCheckBox( checkBoxNetworkSegment );
			}
		}


        private void FormSegmentation_Load(object sender, EventArgs e)
        {
			SecureForm();

			FormLoad(Settings.Default.SEGMENTATION_LOGIC_IMAGE_KEY, Settings.Default.SEGMENTATION_LOGIC_IMAGE_KEY_SELECTED);

            this.TabText = "Logic-" + m_strNetwork;
            this.labelDynamicSegmentation.Text = "Segmentation Logic: " + m_strNetwork;

        
            //Load criteria.
            String strQuery = "SELECT FAMILY_NAME, FAMILY_EXPRESSION FROM CRITERIA_SEGMENT ORDER BY FAMILY_NAME";
            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(strQuery);
                String strName;
                String strCriteria;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    strName = row.ItemArray[0].ToString();
                    strCriteria = row.ItemArray[1].ToString();
                    strCriteria = strCriteria.Replace("|", "'");
                    m_hashFamily.Add(strName, strCriteria);
                    listBoxSubset.Items.Add(strName);
                }
            }
            catch (Exception sqlE)
            {
                MessageBox.Show(sqlE.ToString());
            }


            // Load tree view from table NETWORK_TREE
            tvNetwork.Nodes.Clear();
            String strNode;
            strQuery = "SELECT NODES FROM NETWORK_TREE WHERE NETWORKID='" + m_strNetworkID +"'";
            try { ds = DBMgr.ExecuteQuery(strQuery); }
            catch (Exception exception) { Global.WriteOutput("Error: Filling network tree. " + exception.Message); return; }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                strNode = row[0].ToString();
                String[] split = strNode.Split('|');
                List<String> list = new List<String>();

                for (int i = 0; i < split.Length; i++)
                {
                    list.Add(split[i].ToString());
                }



                BuildRecursive(list, tvNetwork.Nodes);
            }
            LoadSegmentationResults();
        }

        public String NetworkID
        {
            get { return m_strNetworkID; }
        }

        private void BuildRecursive(List<String> split, TreeNodeCollection nodes)
        {
            //Stop when done parsing
            if (split.Count == 0) return;
            foreach (TreeNode tn in nodes)
            {
                if (split[0] == tn.Text)
                {
                    split.RemoveRange(0,1);
                    if(split.Count > 0)
                    {
                        BuildRecursive(split, tn.Nodes);
                    }
                    return;
                }
            }    
            TreeNode tnNew = nodes.Add(split[0].ToString());
            split.RemoveRange(0, 1);

            if(split.Count > 0)
            {
                BuildRecursive(split,tnNew.Nodes);
            }
        
        }

        private void buttonEditSubset_Click(object sender, EventArgs e)
        {
            int nBegin = -1;
            int nEnd = -1 ;
            String strCriteria = textBoxCriteria.Text;

            if (listBoxSubset.SelectedItem != null)
            {
                String strName = listBoxSubset.SelectedItem.ToString();
                if (strCriteria != "" && strName != "")
                {
                    nBegin = strCriteria.IndexOf("[");
                    nEnd = strCriteria.IndexOf("]");
                    //if (nEnd >= 0 && nBegin >= 0)
                    {
						String strAttribute = "";
						if (nEnd >= 0 && nBegin >= 0)
						{
							strAttribute = strCriteria.Substring(nBegin + 1, nEnd - nBegin - 1);
						}
                        
                        FormSegmentationCriteria form = new FormSegmentationCriteria(strName, strCriteria, strAttribute);
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            m_hashFamily.Clear();
                            listBoxSubset.Items.Clear();
                            String strQuery = "SELECT FAMILY_NAME, FAMILY_EXPRESSION FROM CRITERIA_SEGMENT ORDER BY FAMILY_NAME";
                            DataSet ds;
                            try
                            {
                                ds = DBMgr.ExecuteQuery(strQuery);
                                foreach (DataRow row in ds.Tables[0].Rows)
                                {
                                    strName = row.ItemArray[0].ToString();
                                    strCriteria = row.ItemArray[1].ToString();
                                    strCriteria = strCriteria.Replace("|", "'");
                                    m_hashFamily.Add(strName, strCriteria);
                                    listBoxSubset.Items.Add(strName);
                                }
                                listBoxSubset.Text = strName;
                                strCriteria = (String)m_hashFamily[strName].ToString();
                                textBoxCriteria.Text = strCriteria;

                            }
                            catch (Exception sqlE)
                            {
                                MessageBox.Show(sqlE.ToString());
                            }

                        }
                    }
                }
            }
            else
            {
                String strName;


                FormSegmentationCriteria form = new FormSegmentationCriteria("", "", "");
                if (form.ShowDialog() == DialogResult.OK)
                {
                    m_hashFamily.Clear();
                    listBoxSubset.Items.Clear();
                    String strQuery = "SELECT FAMILY_NAME, FAMILY_EXPRESSION FROM CRITERIA_SEGMENT ORDER BY FAMILY_NAME";
                    DataSet ds;
                    try
                    {
                        ds = DBMgr.ExecuteQuery(strQuery);
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            strName = row.ItemArray[0].ToString();
                            strCriteria = row.ItemArray[1].ToString();
                            strCriteria = strCriteria.Replace("|", "'");
                            m_hashFamily.Add(strName, strCriteria);
                            listBoxSubset.Items.Add(strName);
                        }
                        listBoxSubset.Text = form.m_strName;
                        strCriteria = (String)m_hashFamily[form.m_strName].ToString();
                        textBoxCriteria.Text = strCriteria;
                    }
                    catch (Exception sqlE)
                    {
                        MessageBox.Show(sqlE.ToString());
                    }
                }
            }
       }

        private void buttonExportSegmentSummary_Click(object sender, EventArgs e)
        {

        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (checkBoxNetworkSegment.Checked)
            {
                if (tvNetwork.SelectedNode != null)
                {
                    tvNetwork.SelectedNode.Remove();
                    m_bEditSegmentation = true;
                }
            }
            else
            {
                Global.WriteOutput("Note: Allow network segmentation must be checked to remove a node from the network tree.  If allow network segmentation box disabled, your permissions do not allow network editing.");
            }
        }

        private void buttonAddChild_Click(object sender, EventArgs e)
        {
            if (listBoxSubset.SelectedItem == null) return;
            
            if (listBoxSubset.SelectedItem.ToString() != "")
            {
                if (checkBoxNetworkSegment.Checked)
                {
                    if (tvNetwork.SelectedNode != null)
                    {
						if(m_hashFamily.Contains(listBoxSubset.SelectedItem.ToString()))
						{
							if (m_hashFamily[listBoxSubset.SelectedItem.ToString()].ToString().ToUpper().Contains("ANYRECORD") ||
								m_hashFamily[listBoxSubset.SelectedItem.ToString()].ToString().ToUpper().Contains("ANYCHANGE"))
							{
								Global.WriteOutput("Error: Cannot have ANYCHANGE, ANYRECORD in a child node.");
								return;
							}
							tvNetwork.SelectedNode.Nodes.Add(listBoxSubset.SelectedItem.ToString());
                            m_bEditSegmentation = true;
						}
                    }
                    else
                    {
                        Global.WriteOutput("Error: A node must be selected to a child element.");
                    }
                }
                else
                {
                    Global.WriteOutput("Note: Allow network segmentation must be checked to edit a network tree.  If allow network segmentation box disabled, your permissions do not allow network editing.");
                }
            }
        }

        private void buttonAddRoot_Click(object sender, EventArgs e)
        {
            if (listBoxSubset.SelectedItem != null)
            {
                if (checkBoxNetworkSegment.Checked)
                {
                    tvNetwork.Nodes.Add(listBoxSubset.SelectedItem.ToString());
                    m_bEditSegmentation = true;
                }
                else
                {
                    Global.WriteOutput("Note: Allow network segmentation must be checked to edit a network tree.  If allow network segmentation box disabled, your permissions do not allow network editing.");
                }
            }
        }

        private void buttonSegmentNetwork_Click(object sender, EventArgs e)
        {
            m_bEditSegmentation = false;
            // Close Segmentation related forms
            FormManager.CloseFormSegmentationCriteria(m_strNetworkID);
            FormManager.CloseFormSegmentedConstruction(m_strNetworkID);
            FormManager.CloseFormRollup(m_strNetworkID);

            // Start the segmentation process
            Global.WriteOutput("Segmentation Started: " + DateTime.Now.ToString());
            this.Cursor = Cursors.WaitCursor;
            SaveNetworkTree();
            DynamicSegmentation.DynamicSegmentation dynamic = new DynamicSegmentation.DynamicSegmentation();
            dynamic.DoSegmentation(m_strNetwork);
			if (SegmentationMessaging.GetProgressList().Count == 0)
			{
				LoadSegmentationResults();
				Global.WriteOutput("Segmentation Completed: " + DateTime.Now.ToString());
			}
			else
			{
				foreach (String str in SegmentationMessaging.GetProgressList())
				{
					Global.WriteOutput(str);
				}
			}
            this.Cursor = Cursors.Default;
			SegmentationMessaging.GetProgressList().Clear();
        }

        private void listBoxSubset_SelectedIndexChanged(object sender, EventArgs e)
        {
            String strName = listBoxSubset.Text.ToString();

            if(m_hashFamily.Contains(strName))
            {
                String strCriteria = (String) m_hashFamily[strName];
                textBoxCriteria.Text = strCriteria;
            }


        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            String strName;
            String strCriteria;

            FormSegmentationCriteria form = new FormSegmentationCriteria("", "", "");
            if (form.ShowDialog() == DialogResult.OK)
            {
                m_hashFamily.Clear();
                listBoxSubset.Items.Clear();
                String strQuery = "SELECT FAMILY_NAME, FAMILY_EXPRESSION FROM CRITERIA_SEGMENT ORDER BY FAMILY_NAME";
                DataSet ds;
                try
                {
                    ds = DBMgr.ExecuteQuery(strQuery);
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        strName = row.ItemArray[0].ToString();
                        strCriteria = row.ItemArray[1].ToString();
                        strCriteria = strCriteria.Replace("|", "'");
                        m_hashFamily.Add(strName, strCriteria);
                        listBoxSubset.Items.Add(strName);
                    }
                    listBoxSubset.Text = form.m_strName;
                    strCriteria = (String)m_hashFamily[form.m_strName].ToString();
                    textBoxCriteria.Text = strCriteria;
                }
                catch (Exception sqlE)
                {
                    MessageBox.Show(sqlE.ToString());
                }
            }


        }

        private void checkBoxNetworkSegment_CheckedChanged(object sender, EventArgs e)
        {
			if( checkBoxNetworkSegment.Checked )
			{
				if( Global.SecurityOperations.CanModifySegmentationLogic( m_strNetworkID ) )
				{
					UnlockButton( buttonAddChild );
					UnlockButton( buttonAddRoot );
					UnlockButton( buttonSegment );
					UnlockButton( buttonRemove );
					UnlockButton( buttonNew );
					UnlockButton( buttonEditSubset );
					UnlockButton( buttonSegmentNetwork );
				}
			}
			else
			{
				SecureForm();
			}
        }

        private void listBoxSubset_KeyUp(object sender, KeyEventArgs e)
        {
              if ( e.KeyCode == Keys.Delete && Global.SecurityOperations.CanModifySegmentationLogic( m_strNetworkID ) )
              {
                  String strSubset = listBoxSubset.SelectedItem.ToString();
                  if(strSubset != null)
                  {
                      try
                      {
                          String strDelete = "DELETE FROM CRITERIA_SEGMENT WHERE FAMILY_NAME ='" + strSubset + "'";
                          DBMgr.ExecuteNonQuery(strDelete);
                      }
                      catch (Exception exception)
                      {

                          Global.WriteOutput("Error: Deleting subset - " + exception.ToString());
                          MessageBox.Show("Error: Deleting subset -" + exception.ToString());
                          return;
                      }

                    listBoxSubset.Items.Remove(listBoxSubset.SelectedItem);
                  }
             }
        }

        private void SaveNetworkTree()
        {
            this.Cursor = Cursors.WaitCursor;
            //Delete existing information;
            String strDelete = "DELETE FROM NETWORK_TREE WHERE NETWORKID='"+ m_strNetworkID + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strDelete);
            }
            catch (Exception exception)
            {
                String strError = "Error: Deleting existing network tree on save with SQL exception -" + exception.Message;
                Global.WriteOutput(strError);
                MessageBox.Show(strError);
                return;
            }

            //Save tree information.
            //Iterate through tree.


            foreach (TreeNode tn in tvNetwork.Nodes)
            {
                GetRecursive(tn);
            }
            this.Cursor = Cursors.Default;
        }

        private void GetRecursive(TreeNode treeNode)
        {
            if (treeNode.Nodes.Count > 0)
            {
                foreach (TreeNode tn in treeNode.Nodes)
                {
                    GetRecursive(tn);
                }
            }
            else
            {
                // For nodes iterate until no parents.
                String strNodes = treeNode.Text;
                TreeNode parent = treeNode.Parent;
                while (parent != null)
                {
                    strNodes = strNodes.Insert(0, parent.Text + "|");
                    parent = parent.Parent;
                }
                String strInsert = "INSERT INTO NETWORK_TREE (NETWORKID,NODES) VALUES ('" + m_strNetworkID + "','" + strNodes + "')";
                try
                {
                    DBMgr.ExecuteNonQuery(strInsert);
                }
                catch (Exception exception)
                {
                    String strError = "Error: Inserting network tree on edit tree with SQL exception -" + exception.Message;
                    Global.WriteOutput(strError);
                    MessageBox.Show(strError);
                    return;
                }
            }
        }

		private void LoadSegmentationResults()
		{
			String strQuery;
			switch( DBMgr.NativeConnectionParameters.Provider )
			{
				case "MSSQL":
					strQuery = "SELECT BREAKCAUSE AS 'Reason for Break',SHORTEST AS 'Shortest(" + Global.LinearUnits + ")',LONGEST AS 'Longest(" + Global.LinearUnits + ")',AVERAGE as 'Average(" + Global.LinearUnits + ")',NUMBER_ AS Count FROM DYNAMIC_SEGMENTATION_RESULT WHERE NETWORKID='" + m_strNetworkID + "'";
					break;
				case "ORACLE":
					strQuery = "SELECT BREAKCAUSE AS \"Reason for Break\",SHORTEST AS \"Shortest(" + Global.LinearUnits + ")\",LONGEST AS \"Longest(" + Global.LinearUnits + ")\",AVERAGE as \"Average(" + Global.LinearUnits + ")\",NUMBER_ AS \"Count\" FROM DYNAMIC_SEGMENTATION_RESULT WHERE NETWORKID='" + m_strNetworkID + "'";
					break;
				default:
					throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
				//break;
			}

			try
			{

				binding = new BindingSource();
				dataAdapter = new DataAdapter( strQuery );

				// Populate a new data table and bind it to the BindingSource.
				table = new DataTable();
				table.Locale = System.Globalization.CultureInfo.InvariantCulture;

				dataAdapter.Fill( table );
				binding.DataSource = table;
				dgvResultSegment.DataSource = binding;
				bindingNavigatorSegmentation.BindingSource = binding;
				dgvResultSegment.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
				dgvResultSegment.ReadOnly = true;

				dgvResultSegment.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				dgvResultSegment.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				dgvResultSegment.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				dgvResultSegment.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				dgvResultSegment.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;





			}
			catch( Exception exception )
			{
				Global.WriteOutput( "Error: Connecting contruction history table. SQL message is " + exception.Message );
			}

		}

        private void FormSegmentation_FormClosed(object sender, FormClosedEventArgs e)
        {
			FormUnload();
            FormManager.RemoveFormSegmentation(this);
            if (m_bEditSegmentation)
            {
                MessageBox.Show("Closing the Dynamic Segmentation page before selecting the Segmentation button caused edits to be discarded.");
            }
        }




    }
}