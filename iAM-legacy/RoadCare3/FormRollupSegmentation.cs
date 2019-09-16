using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DatabaseManager;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using RollupSegmentation;
using System.Threading;
using RoadCare3.Properties;

namespace RoadCare3
{
    public partial class FormRollupSegmentation : BaseForm
    {
        String m_strNetwork;
        String m_strNetworkID;
        RollupSegmentation.RollupSegmentation m_rollup;
        Thread m_rollupThread;

		LockInformation rollupLock;
        
        
        
        /// <summary>
        /// Allows the user to rollup a segmented network and export the results of the rollup to a data file.
        /// </summary>
        /// <param name="strNetwork">Name of the segmented network to rollup.</param>
        public FormRollupSegmentation(String strNetwork)
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
                MessageBox.Show(strError);

            }
            this.TabText = "Rollup-" + m_strNetwork;


        }


		protected void SecureForm()
		{
			LockButton( buttonRollup );
			LockDataGridView( dgvRollup );

			if( Global.SecurityOperations.CanModifyRollup( m_strNetworkID ) )
			{
				UnlockButton( buttonRollup );
				dgvRollup.ReadOnly = false;
			}
		}

        private void FormRollupSegmentation_Load(object sender, EventArgs e)
        {
			SecureForm();

			FormLoad(Settings.Default.ROLLUP_IMAGE_KEY, Settings.Default.ROLLUP_IMAGE_KEY_SELECTED);

            Global.Attributes.Clear();
            Global.LoadAttributes();

            labelRollup.Text = "Segmentation Rollup - " + m_strNetwork;

            DataSet ds = DBMgr.ExecuteQuery("SELECT COUNT(*) FROM DYNAMIC_SEGMENTATION WHERE NETWORKID='" + m_strNetworkID  + "'");
            int nCount = 0;
            int.TryParse(ds.Tables[0].Rows[0].ItemArray[0].ToString(),out nCount);

            this.labelLRS.Text = "Linear Reference Sections: " + nCount.ToString();
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					ds = DBMgr.ExecuteQuery("SELECT COUNT(*) FROM NETWORK_DEFINITION WHERE FACILITY<>''");
					break;
				case "ORACLE":
					ds = DBMgr.ExecuteQuery("SELECT COUNT(*) FROM NETWORK_DEFINITION WHERE FACILITY LIKE '_%'");
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
					//break;
			}
            nCount = 0;
            int.TryParse(ds.Tables[0].Rows[0].ItemArray[0].ToString(), out nCount);
            this.labelSRS.Text = "Section Reference Sections: " + nCount.ToString();

            // Get list of ATTRIBUTE,TYPE from ATTRIBUTES
            // Get list of ATTRIBUTE from ROLLUP_CONTROL
            // Add missing to ROLLUP_CONTROL 

            List<String> listRollupAttribute = new List<String>();
            String strSelect = "SELECT ATTRIBUTE_ FROM ROLLUP_CONTROL WHERE NETWORKID='" + m_strNetworkID + "' ORDER BY ATTRIBUTE_";
            ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                listRollupAttribute.Add(row.ItemArray[0].ToString());
            }

            String strAttribute;
            String strType;
            String strInsert = "";

            strSelect = "SELECT ATTRIBUTE_,TYPE_ FROM ATTRIBUTES_ WHERE CALCULATED=0 OR CALCULATED IS NULL OR ATTRIBUTE_='PCI'";
            ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                strAttribute = row.ItemArray[0].ToString();
                strType = row.ItemArray[1].ToString();

                if (!listRollupAttribute.Contains(strAttribute))
                {
                    if (strType == "NUMBER")
                    {
                        strInsert = "INSERT INTO ROLLUP_CONTROL (NETWORKID,ATTRIBUTE_,ROLLUPTYPE) VALUES ('"
                                + m_strNetworkID + "','"
                                + strAttribute + "','Average')"; //Rollup type 2 is average
                    }
                    else
                    {
                        strInsert = "INSERT INTO ROLLUP_CONTROL (NETWORKID,ATTRIBUTE_,ROLLUPTYPE) VALUES ('"
                                + m_strNetworkID + "','"
                                + strAttribute + "','Predominant')"; //Rollup type 1 is predominant
                    }
                    DBMgr.ExecuteNonQuery(strInsert);
                }

            }

            strSelect = "SELECT ATTRIBUTE_, ROLLUPTYPE FROM ROLLUP_CONTROL WHERE NETWORKID ='" + m_strNetworkID + "' ORDER BY ATTRIBUTE_";
            ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string[] strDataRow = {row[0].ToString(), row[1].ToString() };
                int i = dgvRollup.Rows.Add(strDataRow);
                DataGridViewComboBoxCell dgvCBC = (DataGridViewComboBoxCell)dgvRollup[1, i];
                if (Global.GetAttributeType(row[0].ToString()) == "STRING")
                {
                    dgvCBC.Items.Add("None");
                    dgvCBC.Items.Add("Predominant");
                    dgvCBC.Items.Add("First");
                    dgvCBC.Items.Add("Last");
                    dgvCBC.Items.Add("Count");
                }
                else
                {
                    dgvCBC.Items.Add("None");
                    dgvCBC.Items.Add("Predominant");
                    dgvCBC.Items.Add("Average");
                    dgvCBC.Items.Add("Median");
                    dgvCBC.Items.Add("Maximum");
                    dgvCBC.Items.Add("Minimum");
                    dgvCBC.Items.Add("Absolute Minimum");
                    dgvCBC.Items.Add("Absolute Maximum");
                    dgvCBC.Items.Add("Sum");
                    dgvCBC.Items.Add("Standard Deviation");
                    dgvCBC.Items.Add("First");
                    dgvCBC.Items.Add("Last");
                    dgvCBC.Items.Add("Count");
  
                }
            }

            dgvRollup.Columns[0].AutoSizeMode = (DataGridViewAutoSizeColumnMode.Fill);
            dgvRollup.Columns[1].AutoSizeMode = (DataGridViewAutoSizeColumnMode.Fill);
        }

        public String NetworkID
        {
            get { return m_strNetworkID; }
        }

        private void dgvRollup_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRollup.CurrentRow.Cells[0].Value == null) return;

            String strAttribute = dgvRollup.CurrentRow.Cells[0].Value.ToString();
			ConnectionParameters cp = DBMgr.GetAttributeConnectionObject(strAttribute);
            DataSet ds = DBMgr.ExecuteQuery("SELECT COUNT(*) FROM " + strAttribute, cp);
            int nCount = 1;
            int nMode = 1;
            int.TryParse(ds.Tables[0].Rows[0].ItemArray[0].ToString(),out nCount);
           //Retrieve a reasonable number of selections.
            while ((float)nCount / (float)nMode > 100)
            {
                nMode = nMode * 2;
            }

            listBoxAttributes.Items.Clear();
			String strSelect;
			if( strAttribute != "PCI" && strAttribute != "CLIMATE_PCI" && strAttribute != "LOAD_PCI" && strAttribute != "OTHER_PCI" )
			{
				strSelect = "SELECT DISTINCT DATA_ FROM " + strAttribute;
			}
			else
			{
				strSelect = "SELECT DISTINCT " + strAttribute + " FROM PCI";
			}
			ds = DBMgr.ExecuteQuery(strSelect, cp);
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				listBoxAttributes.Items.Add(row[0].ToString());
			}
        }

        private void buttonRollup_Click(object sender, EventArgs e)
        {
			LockInformation rollupCheck = Global.GetLockInfo( m_strNetworkID, "" );
			if( !rollupCheck.Locked )
			{
				rollupLock = Global.LockNetwork( m_strNetworkID, false );

				this.Cursor = Cursors.WaitCursor;
				buttonRollup.Enabled = false;
				Global.ClearOutputWindow();
				m_rollup = new RollupSegmentation.RollupSegmentation();
				m_rollup.strNetwork = m_strNetwork;
				this.Cursor = Cursors.WaitCursor;

				// Close all tabs associated with this network
				FormManager.CloseMultipleSimulations( m_strNetworkID );
				FormManager.CloseNetworkViewers( m_strNetworkID );

				m_rollupThread = new Thread( new ThreadStart( m_rollup.DoRollup ) );
				//m_rollupThread.Priority = ThreadPriority.AboveNormal;
                try
                {
                    m_rollupThread.Start();
                    timerRollup.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
				//currentLock.UnlockNetwork();
			}
			else
			{
				Global.WriteOutput( rollupCheck.LockOwner + " started performing an action locking this network at " + rollupCheck.Start.ToString() + "." );
			}

        }

        private void FormRollupSegmentation_FormClosed(object sender, FormClosedEventArgs e)
		{
			FormUnload();
            FormManager.RemoveFormRollupSegmentation(this);
        }

        private void timerRollup_Tick(object sender, EventArgs e)
        {
            List<String> listRollup = RollupSegmentation.RollupMessaging.GetProgressList();
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
                    Global.WriteOutput(strOut);
                }
            }
            RollupSegmentation.RollupMessaging.ClearProgressList();
            if (!m_rollupThread.IsAlive)
            {
                timerRollup.Stop();

                // Call the soln explorer to add viewers.
                FormManager.AddSolutionExplorerNetworkViewers(m_strNetwork, m_strNetworkID);

				rollupLock.UnlockNetwork();
				buttonRollup.Enabled = true;
                
            }
            this.Cursor = Cursors.Arrow;

        }

        private void dgvRollup_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                try
                {
                    String strUpdate = "UPDATE ROLLUP_CONTROL SET ROLLUPTYPE = '" + dgvRollup[1, e.RowIndex].Value.ToString()
                                        + "' WHERE ATTRIBUTE_ = '" + dgvRollup[0, e.RowIndex].Value.ToString()
                                        + "' AND NETWORKID = '" + m_strNetworkID + "'";
                    DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch (Exception exc)
                {
                    Global.WriteOutput("Error: Could not update ROLLUP_CONTROL table with new value. " + exc.Message);
                    return;
                }
            }
        }

    }
}
