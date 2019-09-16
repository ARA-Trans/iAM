using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RoadCare3;
using DataObjects;
using RoadCareGlobalOperations;
using WeifenLuo.WinFormsUI.Docking;
using DynamicSegmentation;
using RollupSegmentation;
using System.Threading;
using System.IO;

namespace RCImageView3
{
    public partial class FormImageViewSegmentation : BaseForm
    {
        RollupSegmentation.RollupSegmentation m_rollup;
        Thread m_rollupThread;
        bool m_bUpdate = false;
        public FormImageViewSegmentation(NetworkObject networkObject)
        {
            InitializeComponent();
            this.Network = networkObject;
            this.TabText = "Segment:" + networkObject.Network;
        }

        private void FormImageViewSegmentation_Load(object sender, EventArgs e)
        {

        }

        private void LoadGridViewAttributes()
        {
            m_bUpdate = false;
            dgvSegmentRollup.Rows.Clear();
            List<AttributeObject> listAttribute = null;
            List<RollupSegmentObject> listRollup = null;
            try
            {
                listAttribute = GlobalDatabaseOperations.GetAttributes();
            }
            catch(Exception except)
            {
                OutputWindow.WriteOutput("Error: Loading ATTRIBUTE table. " + except.Message);
                return;
            }

            try
            {
                listRollup = GlobalDatabaseOperations.GetSegmentRollup(this.Network.NetworkID);
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Loading ROLLUP_CONTROL table. " + except.Message);
                return;
            }





            foreach (AttributeObject attribute in listAttribute)
            {
                if (attribute.Calculated == false)
                {
                    int nRow = dgvSegmentRollup.Rows.Add(attribute.Attribute);
                    DataGridViewComboBoxCell combo = (DataGridViewComboBoxCell)dgvSegmentRollup[1, nRow];
                    dgvSegmentRollup.Rows[nRow].Cells[0].ReadOnly = true;
                    if (attribute.Type == "NUMBER")
                    {
                        combo.Items.Add("None");
                        combo.Items.Add("Average");
                        combo.Items.Add("Predominant");
                        combo.Items.Add("Median");
                        combo.Items.Add("Maximum");
                        combo.Items.Add("Minimum");
                        combo.Items.Add("Median");
                        combo.Items.Add("Absolute Maximum");
                        combo.Items.Add("Absolute Minimum");
                        combo.Items.Add("Standard Deviation");
                        combo.Items.Add("Sum");
                    }
                    else
                    {
                        combo.Items.Add("None");
                        combo.Items.Add("Average");
                        combo.Items.Add("Predominant");
                    }

                    RollupSegmentObject rollup = listRollup.Find(delegate(RollupSegmentObject rso) {return rso.Attribute == attribute.Attribute; });
                    if (rollup != null)
                    {
                        dgvSegmentRollup.Rows[nRow].Cells[1].Value = rollup.RollupMethod;
                        dgvSegmentRollup.Rows[nRow].Cells[2].Value = rollup.SegmentMethod;
                    }
                    else
                    {
                        if (attribute.Type == "STRING")
                        {
                            dgvSegmentRollup.Rows[nRow].Cells[1].Value = "Predominant";
                        }
                        else
                        {
                            dgvSegmentRollup.Rows[nRow].Cells[1].Value = "Average";
                        }
                        dgvSegmentRollup.Rows[nRow].Cells[2].Value = "-";
                    }
                }
            }
            m_bUpdate = true;

        }

        /// <summary>
        /// Update for ImageView
        /// </summary>
        public override void UpdateNode(DockPanel dockPanel, object ob)
        {
            if (!this.ImageView)
            {
                this.ImageView = true;
                this.HideOnClose = true;
            }
            this.LoadGridViewAttributes();
            this.Show(dockPanel);
        }

        private void dgvSegmentRollup_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            int nIndex = e.RowIndex;
            UpdateValueListBox(nIndex);
        }

        private void UpdateValueListBox(int nIndex)
        {
            if (!m_bUpdate) return;//Does this if RowEnter fired do to initialization.
            DataGridViewRow row = dgvSegmentRollup.Rows[nIndex];
            String strAttribute = row.Cells["Attribute"].Value.ToString();
            listBoxAttributeValues.Items.Clear();

            try
            {
                List<String> listValues = GlobalDatabaseOperations.GetRawAttributeValue(strAttribute, checkBoxShowAvailable.Checked);
                foreach (String strValue in listValues)
                {
                    listBoxAttributeValues.Items.Add(strValue);
                }
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Retrieving values from Raw Attribute table: " + strAttribute + ". " + except.Message);
            }

        }

        private void checkBoxShowAvailable_CheckedChanged(object sender, EventArgs e)
        {
            if(dgvSegmentRollup.CurrentRow != null)
            {
                int nIndex = dgvSegmentRollup.CurrentRow.Index;
                UpdateValueListBox(nIndex);
            }
        }

        private void buttonSegmentRollup_Click(object sender, EventArgs e)
        {
            if (!SaveSegmentRollup()) return;


            RollupSegmentation();
        }

        private void RollupSegmentation()
        {
            //Directory.CreateDirectory("..\\TempDLL");
			//string specialFolder = Directory.GetCurrentDirectory() + "\\TempDLL";
			//Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);
			//string specialFolder = ".\\TempDLL";

			//Directory.CreateDirectory(specialFolder);

            this.Cursor = Cursors.WaitCursor;

            m_rollup = new RollupSegmentation.RollupSegmentation();
            m_rollup.strNetwork = Network.Network;
            this.Cursor = Cursors.WaitCursor;

            // Close all tabs associated with this network
            m_rollupThread = new Thread(new ThreadStart(m_rollup.DoRollup));
			m_rollupThread.Priority = ThreadPriority.AboveNormal;
            m_rollupThread.Start();
            timerRollup.Start();           
        }



        /// <summary>
        /// Save data to ROLLUP_CONTROL
        /// </summary>
        /// <returns></returns>
        private bool SaveSegmentRollup()
        {
            List<RollupSegmentObject> listRollupSegment = new List<RollupSegmentObject>();
            foreach (DataGridViewRow row in dgvSegmentRollup.Rows)
            {
                RollupSegmentObject rollupSegment = new RollupSegmentObject();
                rollupSegment.Attribute = row.Cells[0].Value.ToString();
                rollupSegment.RollupMethod = row.Cells[1].Value.ToString();
                rollupSegment.SegmentMethod = row.Cells[2].Value.ToString();
                listRollupSegment.Add(rollupSegment);
            }

            try
            {
                GlobalDatabaseOperations.SetSegmentRollup(listRollupSegment, Network.NetworkID);
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Updating ROLLUP_CONTROL table. " + except.Message);
                return false;
            }
            if(!CreateDynamicSegmentation(listRollupSegment))return false;

            return true;
        }

        private bool CreateDynamicSegmentation(List<RollupSegmentObject> listRollupSegment)
        {
            //Delete all information for a given network.
            try
            {
                GlobalDatabaseOperations.DeleteCriteriaSegmentForNetwork(Network.NetworkID);
            }
            catch (Exception except)
            {
                OutputWindow.WriteOutput("Error: Deleting CRITERIA_SEGMENT. " + except.Message);
                return false;
            }

            foreach (RollupSegmentObject rollupSegment in listRollupSegment)
            {
                if (rollupSegment.SegmentMethod != "-")
                {
                    try
                    {
                        GlobalDatabaseOperations.SaveCriteriaSegmentForNetwork(Network.NetworkID, rollupSegment);

                    }
                    catch (Exception except)
                    {
                        OutputWindow.WriteOutput("Error: Saving CRITERIA_SEGMENT. " + except.Message);
                    }
                }
            }

            // Start the segmentation process
            Global.WriteOutput("Segmentation Started: " + DateTime.Now.ToString());
            this.Cursor = Cursors.WaitCursor;
            DynamicSegmentation.DynamicSegmentation dynamic = new DynamicSegmentation.DynamicSegmentation();
            dynamic.DoSegmentation(Network.Network);
            if (SegmentationMessaging.GetProgressList().Count == 0)
            {
                OutputWindow.WriteOutput("Segmentation Completed: " + DateTime.Now.ToString());
            }
            else
            {
                foreach (String str in SegmentationMessaging.GetProgressList())
                {
                    OutputWindow.WriteOutput(str);
                }
            }
            this.Cursor = Cursors.Default;
            SegmentationMessaging.GetProgressList().Clear();
            return true;
        }

        private void timerRollup_Tick(object sender, EventArgs e)
        {
            List<String> listRollup =RollupMessaging.GetProgressList();
            lock (listRollup)
            {
                string strOut = "";
                foreach (String str in listRollup)
                {
                    if (strOut.Length > 0) strOut += "\r\n";
                    strOut += str;
                }
                if (strOut != "")
                {
                    OutputWindow.WriteOutput(strOut);
                }
            }
            RollupMessaging.ClearProgressList();
            if (!m_rollupThread.IsAlive)
            {
                timerRollup.Stop();
                this.Cursor = Cursors.Default;
            // Call the soln explorer to add viewers.
            //    FormManager.AddSolutionExplorerNetworkViewers(m_strNetwork, m_strNetworkID);
            }
        }
    }
}
