using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;
using WeifenLuo.WinFormsUI.Docking;
using System.Data.SqlClient;
using System.Collections;
using RoadCare3.Properties;
using RoadCareDatabaseOperations;
using RoadCareGlobalOperations;
using Simulation;
using RollupSegmentation;

namespace RoadCare3
{
    public partial class FormTreatment : BaseForm
    {
        private String m_strNetwork;
		//dsmelser
		//need network ID for security
		private String m_strNetworkID;
        private String m_strSimulation;
        private String m_strSimID;
        private Hashtable m_hashTreatment = new Hashtable();
        private Hashtable m_hashAttributeYear;
        private bool m_bDeleteFeasible = false;
        private String m_strLastTag;

        public FormTreatment(String strNetwork, String strSimulation,String strSimID,Hashtable hashAttributeYear)
        {
            m_strNetwork = strNetwork;
			m_strNetworkID = DBOp.GetNetworkIDFromName( strNetwork );
            m_strSimulation = strSimulation;
            m_strSimID = strSimID;
            m_hashAttributeYear = hashAttributeYear;

            InitializeComponent();
        }

        private void FormTreatment_Load(object sender, EventArgs e)
        {
			SecureForm();

			FormLoad(Settings.Default.SIMULATION_IMAGE_KEY, Settings.Default.SIMULATION_IMAGE_KEY_SELECTED);

            Global.LoadAttributes();
			dgvConsequences.Columns["Change"].ContextMenuStrip = contextMenuStripCT;
            this.labelTreatment.Text = "Treatments " + m_strSimulation + " : " + m_strNetwork;

            DataSet ds = null;
            String strSelect = "SELECT BUDGETORDER FROM INVESTMENTS WHERE SIMULATIONID='" + m_strSimID + "'";
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    string[] budgets = ds.Tables[0].Rows[0].ItemArray[0].ToString().Split(',');
                    foreach (string str in budgets)
                    {
                        checkedComboBoxBudget.Items.Add(new CCBoxItem(str, 0));
                    }
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error in initializing investments." + exception.Message);

            }
            
            
            String strQuery = "SELECT TREATMENTID,TREATMENT,BEFOREANY,BEFORESAME,BUDGET,DESCRIPTION FROM TREATMENTS WHERE SIMULATIONID='" + m_strSimID + "'";
            try
            {
                ds = DBMgr.ExecuteQuery(strQuery);
            }
            catch (Exception sqlE)
            {
                Global.WriteOutput("Error: Loading treatments. SQL Message - " + sqlE.Message);
                return;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Treatment treatment = new Treatment();
                
                treatment.m_strID = row[0].ToString();               
                treatment.m_strTreatment = row[1].ToString();
                treatment.m_strAny = row[2].ToString();
                treatment.m_strSame = row[3].ToString();
                treatment.m_strBudget = row[4].ToString();
                treatment.m_strDescription = row[5].ToString();

                m_hashTreatment.Add(treatment.m_strID, treatment);

                string[] strDataRow = { row[1].ToString() };
                int nAddedRow = dgvTreatment.Rows.Add(strDataRow);
                dgvTreatment.Rows[nAddedRow].Tag = treatment.m_strID;
                
            }

            if (dgvTreatment.Rows[0].Tag != null)
            {
                String strTag = dgvTreatment.Rows[0].Tag.ToString();
                if (!m_hashTreatment.Contains(strTag)) return;
                Treatment treatment = (Treatment)m_hashTreatment[strTag];
                textBoxAny.Text = treatment.m_strAny;
                textBoxSame.Text = treatment.m_strSame;
                //comboBoxBudget.Text = treatment.m_strBudget;
                UpdateBudgetControl(treatment.m_strBudget);
                textBoxDescription.Text = treatment.m_strDescription;
            }
            else
            {
                textBoxAny.Text = "1";
                textBoxSame.Text = "1";
                textBoxDescription.Text = "";

            }
           // Disabled for MDSHA
               for (var i = 1; i < 100; i++)
                {
                    ScheduledYears.Items.Add(i.ToString());
                }

            UpdateTreatments();
            //    tabControlFeasibility.TabPages.Remove(tabPageScheduled);

        }

        private void UpdateTreatments()
        {
            ScheduledTreatment.Items.Clear();
            SUPERCEDES.Items.Clear();
            foreach (var key in m_hashTreatment.Keys)
            {
                var treatment = (Treatment)m_hashTreatment[key];
                SUPERCEDES.Items.Add(treatment.m_strTreatment);
                ScheduledTreatment.Items.Add(treatment.m_strTreatment);
            }


        }

        protected void SecureForm()
		{
			dgvTreatment.RowsRemoved -= new DataGridViewRowsRemovedEventHandler(dgvTreatment_RowsRemoved);
			LockDataGridView( dgvTreatment );
			dgvTreatment.RowsRemoved += new DataGridViewRowsRemovedEventHandler( dgvTreatment_RowsRemoved );
			LockDataGridView( dgvFeasibility );
			//LockDataGridView( dgvCost );
			LockDataGridView( dgvConsequences );
			LockTextBox( textBoxAny );
			LockTextBox( textBoxSame );
			LockTextBox( textBoxDescription );
            LockComboBox(checkedComboBoxBudget);

			if( Global.SecurityOperations.CanModifySimulationTreatment( m_strNetworkID, m_strSimID ) )
			{
				UnlockDataGridViewForCreateDestroy( dgvTreatment );
				UnlockDataGridViewForCreateDestroy( dgvFeasibility );
				UnlockDataGridViewForCreateDestroy( dgvCost );
				UnlockDataGridViewForCreateDestroy( dgvConsequences );
				UnlockTextBox( textBoxAny );
				UnlockTextBox( textBoxSame );
				UnlockTextBox( textBoxDescription );
                UnlockComboBox(checkedComboBoxBudget);
			}
		}



		private void dgvTreatment_CellEnter( object sender, DataGridViewCellEventArgs e )
        {
            if (dgvTreatment.SelectedCells.Count == 0) return;
            if (dgvTreatment.Rows[e.RowIndex].Tag != null)
            {
                String strTag = dgvTreatment.Rows[e.RowIndex].Tag.ToString();
                if (!m_hashTreatment.Contains(strTag)) return;
                Treatment treatment = (Treatment)m_hashTreatment[strTag];
                textBoxAny.Text = treatment.m_strAny;
                textBoxSame.Text = treatment.m_strSame;
                //comboBoxBudget.Text = treatment.m_strBudget;
                UpdateBudgetControl(treatment.m_strBudget);

                textBoxDescription.Text = treatment.m_strDescription;
                LoadFeasibility(strTag);
            }
            else
            {
                textBoxAny.Text = "1";
                textBoxSame.Text = "1";
                textBoxDescription.Text = "";

                m_bDeleteFeasible = false;
                dgvFeasibility.Rows.Clear();
                dgvCost.Rows.Clear();
                dgvConsequences.Rows.Clear();
                m_bDeleteFeasible = true;
            }
        }

        private void dgvTreatment_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvTreatment.SelectedRows.Count == 0) return;
            if (dgvTreatment.Rows[e.RowIndex].Tag != null)
            {
                String strTag = dgvTreatment.Rows[e.RowIndex].Tag.ToString();
                if (!m_hashTreatment.Contains(strTag)) return;
                Treatment treatment = (Treatment)m_hashTreatment[strTag];
                textBoxAny.Text = treatment.m_strAny;
                textBoxSame.Text = treatment.m_strSame;

                UpdateBudgetControl(treatment.m_strBudget);
                //comboBoxBudget.Text = treatment.m_strBudget;
                
                textBoxDescription.Text = treatment.m_strDescription;
                LoadFeasibility(strTag);
            }
            else
            {
                textBoxAny.Text = "1";
                textBoxSame.Text = "1";
                textBoxDescription.Text = "";

                m_bDeleteFeasible = false;
                dgvFeasibility.Rows.Clear();
                dgvCost.Rows.Clear();
                dgvConsequences.Rows.Clear();
                m_bDeleteFeasible = true;
            }
        }


        private void UpdateBudgetControl(string budget)
        {
            List<string> listBudgets = new List<string>();
            string[] budgets = budget.Split(',');
            foreach (string b in budgets) { listBudgets.Add(b.Trim()); }
            for (int i = 0; i < checkedComboBoxBudget.Items.Count; i++)
            {
                CCBoxItem item = (CCBoxItem)checkedComboBoxBudget.Items[i];
                string name = item.Name.Trim();

                if (listBudgets.Contains(name)) checkedComboBoxBudget.SetItemChecked(i, true);
                else checkedComboBoxBudget.SetItemChecked(i, false);
            }
        }



        private void LoadFeasibility(String strTag)
        {
            if (strTag == null) return;
            m_strLastTag = strTag;
            m_bDeleteFeasible = false;
            dgvFeasibility.Rows.Clear();
            dgvCost.Rows.Clear();
            dgvConsequences.Rows.Clear();
            dataGridViewScheduled.Rows.Clear();
            dataGridViewSupersede.Rows.Clear();

            dgvFeasibility.Columns[0].ReadOnly = true;
            dgvCost.Columns[0].ReadOnly = true;
            dgvCost.Columns[1].ReadOnly = true;
            dgvConsequences.Columns[2].ReadOnly = true;
            dgvConsequences.Columns[3].ReadOnly = true;

            dataGridViewSupersede.Columns[1].ReadOnly = true;
            m_bDeleteFeasible = true;
            this.Attribute.Items.Clear();
            foreach (String strAttribute in Global.Attributes)
            {
                this.Attribute.Items.Add(strAttribute);
            }

            String strSelect = "SELECT FEASIBILITYID,CRITERIA FROM FEASIBILITY WHERE TREATMENTID='" + strTag + "'";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr[0].ToString().Trim() != "")
                {
                    String strCriteria = dr[1].ToString().Replace("|", "'");
                    int nRowIndex = dgvFeasibility.Rows.Add(strCriteria);
                    dgvFeasibility.Rows[nRowIndex].Tag = dr[0].ToString();

                }
            }


            strSelect = "SELECT COSTID,COST_,CRITERIA,ISFUNCTION FROM COSTS WHERE TREATMENTID='" + strTag + "'";
            if (SimulationMessaging.IsOMS)
            {
                strSelect = "SELECT COSTID,COST_,CRITERIA,NULL AS ISFUNCTION FROM COSTS WHERE TREATMENTID='" + strTag +
                            "'";
            }




            ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr[0].ToString().Trim() != "")
                {
                    String strCriteria = dr["CRITERIA"].ToString().Replace("|", "'");

                    string[] strDataRow = {dr["COST_"].ToString(), strCriteria};
                    int nRowIndex = dgvCost.Rows.Add(strDataRow);
                    dgvCost.Rows[nRowIndex].Tag = dr["COSTID"].ToString();

                    bool isFunction = false;
                    if (dr["ISFUNCTION"] != DBNull.Value)
                    {
                        isFunction = Convert.ToBoolean(dr["ISFUNCTION"]);
                    }

                    if (isFunction)
                    {
                        dgvCost.Rows[nRowIndex].Cells[0].Tag = "1";
                    }
                    else
                    {
                        dgvCost.Rows[nRowIndex].Cells[0].Tag = "0";
                    }
                }
            }


            strSelect =
                "SELECT CONSEQUENCEID,ATTRIBUTE_,CHANGE_,CRITERIA,EQUATION,ISFUNCTION FROM CONSEQUENCES WHERE TREATMENTID='" +
                strTag + "'";
            ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr["CONSEQUENCEID"].ToString().Trim() != "")
                {
                    String strAttribute = dr["ATTRIBUTE_"].ToString();
                    if (this.Attribute.Items.Contains(strAttribute))
                    {
                        String strEquation = dr["EQUATION"].ToString().Replace("|", "'");
                        String strCriteria = dr["CRITERIA"].ToString().Replace("|", "'");
                        string[] strDataRow = {strAttribute, dr["CHANGE_"].ToString(), strEquation, strCriteria};
                        int nRowIndex = dgvConsequences.Rows.Add(strDataRow);
                        dgvConsequences.Rows[nRowIndex].Tag = dr["CONSEQUENCEID"].ToString();

                        bool isFunction = false;
                        if (dr["ISFUNCTION"] != DBNull.Value) isFunction = Convert.ToBoolean(dr["ISFUNCTION"]);
                        if (isFunction) dgvConsequences.Rows[nRowIndex].Cells[2].Tag = "1";
                        else dgvConsequences.Rows[nRowIndex].Cells[2].Tag = "0";
                    }
                    else
                    {
                        Global.WriteOutput("Error: Consequence attribute does not exist " + strAttribute);
                    }
                }
            }

            strSelect =
                "SELECT SCHEDULEDID, SCHEDULEDYEAR, SCHEDULEDTREATMENTID FROM SCHEDULED WHERE TREATMENTID='" +
                strTag + "'";
            ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                var scheduledId = dr["SCHEDULEDID"].ToString();
                var scheduledTreatmentId = dr["SCHEDULEDTREATMENTID"].ToString();
                var scheduledYear = dr["SCHEDULEDYEAR"].ToString();

                if (m_hashTreatment.Contains(scheduledTreatmentId))
                {
                    var treatment = (Treatment)m_hashTreatment[scheduledTreatmentId];
                    var rowIndex = dataGridViewScheduled.Rows.Add(treatment.m_strTreatment, scheduledYear);
                    dataGridViewScheduled.Rows[rowIndex].Tag = scheduledId;
                }
            }

            var select ="SELECT SUPERSEDE_ID,  SUPERSEDE_TREATMENT_ID, CRITERIA FROM SUPERSEDES WHERE TREATMENT_ID='" +
                strTag + "'";
            ds = DBMgr.ExecuteQuery(select);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                var supersedeId = dr["SUPERSEDE_ID"].ToString();
                var supersedeTreatmentId = dr["SUPERSEDE_TREATMENT_ID"].ToString();
                var criteria = dr["CRITERIA"].ToString();

                if (m_hashTreatment.Contains(supersedeTreatmentId))
                {
                    var treatment = (Treatment)m_hashTreatment[supersedeTreatmentId];
                    var rowIndex = dataGridViewSupersede.Rows.Add(treatment.m_strTreatment, criteria.Replace("|","'"));
                    dataGridViewSupersede.Rows[rowIndex].Tag = supersedeId;
                }
            }
        }


        /// <summary>
        /// Delete rows from Treatment list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvTreatment_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            List<String> listRemoved = new List<String>();

            foreach(String strTag in m_hashTreatment.Keys)
            {
                bool bTagPresent = false;
                foreach (DataGridViewRow row in dgvTreatment.Rows)
                {
                    if (row.Tag == null) continue;
                    if (row.Tag.ToString() == strTag)
                    {
                        bTagPresent = true;
                    }
                }

                if (!bTagPresent)
                {
                    Treatment treatment = (Treatment)m_hashTreatment[strTag];
                    listRemoved.Add(strTag);
                    String strDelete = "DELETE FROM TREATMENTS WHERE TREATMENTID='" + treatment.m_strID + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strDelete);
                    }
                    catch (Exception except)
                    {

                        Global.WriteOutput("Error:" + except.Message.ToString());
                    }
                }

            }


            foreach (String str in listRemoved)
            {
                m_hashTreatment.Remove(str);
            }

            if (dgvTreatment.Rows[e.RowIndex].Tag != null)
            {
                String strTag = dgvTreatment.Rows[e.RowIndex].Tag.ToString();
                LoadFeasibility(strTag);

                if (m_hashTreatment.Contains(strTag))
                {
                    Treatment treatment = (Treatment)m_hashTreatment[strTag];
                    textBoxAny.Text = treatment.m_strAny;
                    textBoxSame.Text = treatment.m_strSame;
                    textBoxDescription.Text = treatment.m_strDescription;
                    //comboBoxBudget.Text = treatment.m_strBudget;
                    UpdateBudgetControl(treatment.m_strBudget);
                }
            }
            else
            {
                int nRow = dgvTreatment.Rows.Count-2;
                if (nRow > -1)
                {
                    String strTag = dgvTreatment.Rows[nRow].Tag.ToString();

                    LoadFeasibility(strTag);

                    if (m_hashTreatment.Contains(strTag))
                    {
                        Treatment treatment = (Treatment)m_hashTreatment[strTag];
                        textBoxAny.Text = treatment.m_strAny;
                        textBoxSame.Text = treatment.m_strSame;
                        textBoxDescription.Text = treatment.m_strDescription;
                        //comboBoxBudget.Text = treatment.m_strBudget;
                        UpdateBudgetControl(treatment.m_strBudget);
                    }
                }
            }
            UpdateTreatments();
        }


        private void dgvTreatment_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if (dgvTreatment.Rows[e.RowIndex].Tag != null)//If tag exist... need to update Treatment
                {
                    String strTreatment = dgvTreatment.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();
                    String strTag = dgvTreatment.Rows[e.RowIndex].Tag.ToString();
                    if (strTreatment != "")
                    {
                        String strUpdate = "UPDATE TREATMENTS SET TREATMENT='" + strTreatment + "' WHERE TREATMENTID='" + strTag + "'";
                        try
                        {
                            DBMgr.ExecuteNonQuery(strUpdate);
                        }
                        catch (Exception except)
                        {
                            Global.WriteOutput("Error: " + except.Message.ToString());

                        }
                    }
                    var treatment = (Treatment) m_hashTreatment[strTag];
                    treatment.m_strTreatment = strTreatment;
                }
                else// New insert
                {
                    String strTreatment = dgvTreatment.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();
                    String strBudget = checkedComboBoxBudget.Text;
                    String strDescription = textBoxDescription.Text;
                    int nAny = 1;
                    int nSame = 1;
                    int.TryParse(textBoxAny.Text,out nAny);
                    int.TryParse(textBoxSame.Text, out nSame);

                    String strInsert = "INSERT INTO TREATMENTS (SIMULATIONID,TREATMENT,BEFOREANY,BEFORESAME,BUDGET,DESCRIPTION) VALUES ('" + m_strSimID + "','" + strTreatment + "','" + nAny.ToString() + "','" + nSame.ToString() + "','" + strBudget + "','" + strDescription + "')";
                    try
                    {
						String strIdentity;
                        DBMgr.ExecuteNonQuery(strInsert);
						switch (DBMgr.NativeConnectionParameters.Provider)
						{
							case "MSSQL":
								strIdentity = "SELECT IDENT_CURRENT ('TREATMENTS') FROM TREATMENTS";
								break;
							case "ORACLE":
								//strIdentity = "SELECT TREATMENTS_TREATMENTID_SEQ.CURRVAL FROM DUAL";
								//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'TREATMENTS_TREATMENTID_SEQ'";
								strIdentity = "SELECT MAX(TREATMENTID) FROM TREATMENTS";
								break;
							default:
								throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
								//break;
						}
                        DataSet ds = DBMgr.ExecuteQuery(strIdentity);
                        strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        dgvTreatment.Rows[e.RowIndex].Tag = strIdentity;
                        Treatment treatment = new Treatment();
                        treatment.m_strID = strIdentity;
                        treatment.m_strTreatment = strTreatment;
                        treatment.m_strSame = nSame.ToString();
                        treatment.m_strAny = nAny.ToString();
                        treatment.m_strBudget = strBudget;
                        treatment.m_strDescription = strDescription;
                        m_hashTreatment.Add(strIdentity, treatment);
                        


                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error:" + except.Message.ToString());
                    }
                }
            }

            UpdateTreatments();
            LoadFeasibility(m_strLastTag);
 

        }




        private void textBoxAny_Validated(object sender, EventArgs e)
        {
            if (dgvTreatment.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvTreatment.SelectedRows)
                {
                    int nAny = 1;
                    String strAny = textBoxAny.Text.ToString();
                    int.TryParse(strAny, out nAny);
                    if (row.Tag == null) continue;
                    String strTag = row.Tag.ToString();
                    String strUpdate = "UPDATE TREATMENTS SET BEFOREANY='" + nAny.ToString() + "' WHERE TREATMENTID='" + strTag + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                        Treatment treatment = (Treatment)m_hashTreatment[strTag];
                        treatment.m_strAny = nAny.ToString();

                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error: " + except.Message.ToString());

                    }
                }
            }
            if (dgvTreatment.SelectedCells.Count > 0)
            {
                foreach (DataGridViewCell cell in dgvTreatment.SelectedCells)
                {
                    int nAny = 1;
                    String strAny = textBoxAny.Text.ToString();
                    int.TryParse(strAny, out nAny);

                    if (dgvTreatment.Rows[cell.RowIndex].Tag == null) continue;
                    String strTag = dgvTreatment.Rows[cell.RowIndex].Tag.ToString();
                    String strUpdate = "UPDATE TREATMENTS SET BEFOREANY='" + nAny.ToString() + "' WHERE TREATMENTID='" + strTag + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                        Treatment treatment = (Treatment)m_hashTreatment[strTag];
                        treatment.m_strAny = nAny.ToString();
                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error: " + except.Message.ToString());

                    }
                }
            }
        }

        private void textBoxSame_TextChanged(object sender, EventArgs e)
        {
            if (dgvTreatment.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvTreatment.SelectedRows)
                {
                    int nSame = 1;
                    String strSame = textBoxSame.Text.ToString();
                    int.TryParse(strSame, out nSame);
                    if (row.Tag == null) continue;
                    String strTag = row.Tag.ToString();
                    String strUpdate = "UPDATE TREATMENTS SET BEFORESAME='" + nSame.ToString() + "' WHERE TREATMENTID='" + strTag + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                        Treatment treatment = (Treatment)m_hashTreatment[strTag];
                        treatment.m_strSame = nSame.ToString();

                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error: " + except.Message.ToString());

                    }
                }
            }
            if (dgvTreatment.SelectedCells.Count > 0)
            {
                foreach (DataGridViewCell cell in dgvTreatment.SelectedCells)
                {
                    int nSame = 1;
                    String strSame = textBoxSame.Text.ToString();
                    int.TryParse(strSame, out nSame);

                    if (dgvTreatment.Rows[cell.RowIndex].Tag == null) continue;
                    String strTag = dgvTreatment.Rows[cell.RowIndex].Tag.ToString();
                    String strUpdate = "UPDATE TREATMENTS SET BEFORESAME='" + nSame.ToString() + "' WHERE TREATMENTID='" + strTag + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                        Treatment treatment = (Treatment)m_hashTreatment[strTag];
                        treatment.m_strSame = nSame.ToString();
                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error: " + except.Message.ToString());

                    }
                }
            }
        }

        private void textBoxDescription_Validated(object sender, EventArgs e)
        {
            if (dgvTreatment.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvTreatment.SelectedRows)
                {
                    String strDescription = textBoxDescription.Text.ToString();

                    if (row.Tag == null) continue;
                    String strTag = row.Tag.ToString();
                    String strUpdate = "UPDATE TREATMENTS SET DESCRIPTION='" + strDescription + "' WHERE TREATMENTID='" + strTag + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                        Treatment treatment = (Treatment)m_hashTreatment[strTag];
                        treatment.m_strDescription = strDescription;

                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error: " + except.Message.ToString());

                    }
                }
            }
            if (dgvTreatment.SelectedCells.Count > 0)
            {
                foreach (DataGridViewCell cell in dgvTreatment.SelectedCells)
                {
                    String strDecription = textBoxDescription.Text.ToString();

                    if (dgvTreatment.Rows[cell.RowIndex].Tag == null) continue;
                    String strTag = dgvTreatment.Rows[cell.RowIndex].Tag.ToString();
                    String strUpdate = "UPDATE TREATMENTS SET DESCRIPTION='" + strDecription + "' WHERE TREATMENTID='" + strTag + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                        Treatment treatment = (Treatment)m_hashTreatment[strTag];
                        treatment.m_strDescription = strDecription;
                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error: " + except.Message.ToString());

                    }
                }
            }
        }

        private void dgvFeasibility_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if (dgvFeasibility.Rows[e.RowIndex].Tag != null)//If tag exist... need to update Treatment
                {
                    String strValue = "";
                    if (dgvFeasibility.Rows[e.RowIndex].Cells[0].Value != null)
                    {
                        strValue = dgvFeasibility.Rows[e.RowIndex].Cells[0].Value.ToString();
                    }

                    String strCriteria = strValue.Trim();
                    strCriteria = strCriteria.Replace("'", "|");
                    String strTag = dgvFeasibility.Rows[e.RowIndex].Tag.ToString();
                    if (strCriteria != "")
                    {
                        String strUpdate = "UPDATE FEASIBILITY SET CRITERIA='" + strCriteria + "' WHERE FEASIBILITYID='" + strTag + "'";
                        try
                        {
                            DBMgr.ExecuteNonQuery(strUpdate);
                        }
                        catch (Exception except)
                        {
                            Global.WriteOutput("Error: " + except.Message.ToString());

                        }

                        dgvFeasibility.Update();
                    }
                }
                else// New insert
                {
                    String strCriteria = dgvFeasibility.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();
                    strCriteria = strCriteria.Replace("'", "|");
                    String strTreatmentID;
                    if (dgvTreatment.SelectedCells.Count > 0)
                    {
                        strTreatmentID = dgvTreatment.Rows[dgvTreatment.SelectedCells[0].RowIndex].Tag.ToString();
                    }
                    else if (dgvTreatment.SelectedRows.Count > 0)
                    {
                        strTreatmentID = dgvTreatment.SelectedRows[0].Tag.ToString();
                    }
                    else
                    {
                        return;
                    }

                    String strInsert = "INSERT INTO FEASIBILITY (TREATMENTID,CRITERIA) VALUES ('" + strTreatmentID + "','" + strCriteria + "')";
                    try
                    {
						String strIdentity;
                        DBMgr.ExecuteNonQuery(strInsert);

							switch (DBMgr.NativeConnectionParameters.Provider)
							{
								case "MSSQL":
									strIdentity = "SELECT IDENT_CURRENT ('FEASIBILITY') FROM FEASIBILITY";
									break;
								case "ORACLE":
									//strIdentity = "SELECT FEASIBILITY_FEASIBILITYID_SEQ.CURRVAL FROM DUAL";
									//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'FEASIBILITY_FEASIBILITYID_SEQ'";
									strIdentity = "SELECT MAX(FEASIBILITYID) FROM FEASIBILITY";
									break;
								default:
									throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
									//break;
							}

                        DataSet ds = DBMgr.ExecuteQuery(strIdentity);
                        strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        dgvFeasibility.Rows[e.RowIndex].Tag = strIdentity;

                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error: Updating Feasibility" + except.Message.ToString());
                    }
                }
            }
        }


        private void dgvFeasibility_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (e.Row.Tag != null)
            {
                String strDelete = "DELETE FROM FEASIBILITY WHERE FEASIBILITYID='" + e.Row.Tag.ToString() + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strDelete);
                }
                catch (Exception except)
                {
                    Global.WriteOutput("Error Deleting Feasibility:" + except.Message.ToString());
                }
            }
        }

 


        private void dgvFeasibility_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void dgvCost_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_bDeleteFeasible) return;
            if (e.RowIndex > -1)
            {
                if (dgvCost.Rows[e.RowIndex].Tag != null)//If tag exist... need to update Treatment
                {
                    String strTag = dgvCost.Rows[e.RowIndex].Tag.ToString();
                    String strUpdate = "";
                    String strValue = "";
                    String isFunctionTag = "0";
                    if (dgvCost.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        strValue = dgvCost.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                        if(dgvCost.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag != null)
                        {
                            isFunctionTag = dgvCost.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString();
                        }
                    }
                    switch (e.ColumnIndex)
                    {
                        case 0:
                            strUpdate = "UPDATE COSTS SET COST_='" + strValue + "', ISFUNCTION='" + isFunctionTag + "' WHERE COSTID='" + strTag + "'";
                            break;
                        case 1:
                            String strCriteria = strValue.Replace("'", "|");
                            strUpdate = "UPDATE COSTS SET CRITERIA='" + strCriteria + "' WHERE COSTID='" + strTag + "'";
                            break;
                        default:
                            return;
                    }

                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error: " + except.Message.ToString());

                    }
                    
                }
                else// New insert
                {
                     String strCost = "";
                    //String strUnit = "";
                    String strCriteria = "";

                    if (dgvCost.Rows[e.RowIndex].Cells[0].Value != null)
                    {
                        strCost = dgvCost.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();
                    }

                    if (dgvCost.Rows[e.RowIndex].Cells[1].Value != null)
                    {
                        strCriteria = dgvCost.Rows[e.RowIndex].Cells[2].Value.ToString().Trim();
                    }
                    String strTreatmentID;
                    if (dgvTreatment.SelectedCells.Count > 0)
                    {
                        strTreatmentID = dgvTreatment.Rows[dgvTreatment.SelectedCells[0].RowIndex].Tag.ToString();
                    }
                    else if (dgvTreatment.SelectedRows.Count > 0)
                    {
                        strTreatmentID = dgvTreatment.SelectedRows[0].Tag.ToString();
                    }
                    else
                    {
                        return;
                    }
                    strCriteria = strCriteria.Replace("'", "|");
                    String strInsert = "INSERT INTO COSTS (TREATMENTID,COST_,CRITERIA) VALUES ('" + strTreatmentID + "','" + strCost + "','" + strCriteria +"')";
                    try
                    {
						String strIdentity;
                        DBMgr.ExecuteNonQuery(strInsert);

						switch (DBMgr.NativeConnectionParameters.Provider)
						{
							case "MSSQL":
								strIdentity = "SELECT IDENT_CURRENT ('COSTS') FROM COSTS";
								break;
							case "ORACLE":
								//strIdentity = "SELECT COSTS_COSTID_SEQ.CURRVAL FROM DUAL";
								//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'COSTS_COSTID_SEQ'";
								strIdentity = "SELECT MAX(COSTID) FROM COSTS";
								break;
							default:
								throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
								//break;
						}
                        DataSet ds = DBMgr.ExecuteQuery(strIdentity);
                        strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        dgvCost.Rows[e.RowIndex].Tag = strIdentity;

                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error: " + except.Message.ToString());
                    }
                }
            }
        }


        private void dgvCost_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (e.Row.Tag != null)
            {
                String strDelete = "DELETE FROM COSTS WHERE COSTID='" + e.Row.Tag.ToString() + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strDelete);
                }
                catch (Exception except)
                {
                    Global.WriteOutput("Error Deleting Costs:" + except.Message.ToString());
                }
            }
        }


        private void dgvConsequences_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (e.Row.Tag != null)
            {
                String strDelete = "DELETE FROM CONSEQUENCES WHERE CONSEQUENCEID='" + e.Row.Tag.ToString() + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strDelete);
                }
                catch (Exception except)
                {
                    Global.WriteOutput("Error Deleting Consequences:" + except.Message.ToString());
                }
            }
        }



        private void toolStripMenuFeasibleCopy_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuFeasiblePaste_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuFeasibleCriteria_Click(object sender, EventArgs e)
        {
            int nRow = 0;
            if (dgvFeasibility.SelectedRows.Count > 0 || dgvFeasibility.SelectedCells.Count > 0)
            {
                if (dgvFeasibility.SelectedRows.Count > 0)
                {
                    nRow = dgvFeasibility.SelectedRows[0].Index;
                }
                else if (dgvFeasibility.SelectedCells.Count > 0)
                {
                    nRow = dgvFeasibility.SelectedCells[0].RowIndex;
                }
            }
            else
            {
                return;
            }
            DataGridViewRow row = dgvFeasibility.Rows[nRow];

            String strValue = "";
            if (row.Cells[0].Value != null)
            {
                strValue = row.Cells[0].Value.ToString();
            }



            FormAdvancedSearch advancedSearch = new FormAdvancedSearch(m_strNetwork, m_hashAttributeYear, strValue, true);
            if (advancedSearch.ShowDialog() == DialogResult.OK)
            {
                row.Cells[0].Value = advancedSearch.GetWhereClause();
                dgvFeasibility.Update();

            }
        }

        private void toolStripMenuCostCopy_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuCostPaste_Click(object sender, EventArgs e)
        {
			PasteIntoCost();
        }


		private void PasteIntoCost()
		{
			string pasteText = Clipboard.GetText();
			pasteText = pasteText.Replace( "\r\n", "\n" );
			string[] pasteLines = pasteText.Split( '\n' );

			if( dgvCost.SelectedCells.Count > 0 )
			{
				int startRow = int.MaxValue;
				int startColumn = int.MaxValue;
				foreach( DataGridViewCell selectedCell in dgvCost.SelectedCells )
				{
					if( selectedCell.RowIndex < startRow )
					{
						startRow = selectedCell.RowIndex;
					}
					if( selectedCell.ColumnIndex < startColumn )
					{
						startColumn = selectedCell.ColumnIndex;
					}
				}

				int currentRow = startRow;
				int currentColumn = startColumn;

				//bool validCell = false;

				List<string> pasteSQLStatements = new List<string>();

				if( dgvCost.Rows.Count < startRow + pasteLines.Length )
				{
					dgvCost.Rows.Add( startRow + pasteLines.Length - dgvCost.Rows.Count );
				}

				foreach( string line in pasteLines )
				{
					string[] cells = line.Split( '\t' );
					foreach( string cell in cells )
					{
						//validCell = true;
						switch( dgvCost.Columns[currentColumn].Name.Trim().ToUpper() )
						{
							case "COST":
                                
								FormEditEquation equationValidator = new FormEditEquation( cell,false );
								equationValidator.Show();
								equationValidator.buttonOK_Click( null, null );
								dgvCost[currentColumn, currentRow].Value = equationValidator.Equation;
                                if (equationValidator.Equation.Contains("COMPOUND_TREATMENT("))
                                {
                                    try
                                    {
                                        string strID = dgvCost.Rows[currentRow].Tag.ToString();
                                        byte[] assembly = RoadCareGlobalOperations.AssemblySerialize.SerializeObjectToByteArray(equationValidator.calculate);
                                        RoadCareGlobalOperations.GlobalDatabaseOperations.SaveBinaryObjectToDatabase(strID, "COSTID", "COSTS", "BINARY_COST", assembly);
                                    }
                                    catch (Exception except)
                                    {
                                        Global.WriteOutput("Error: Updating Costs equation." + except.Message);
                                    }
                                }
								equationValidator.Close();

								break;
							case "CRITERIA":
								FormAdvancedSearch formAdvancedSearch = new FormAdvancedSearch( m_strNetwork, m_hashAttributeYear, cell, true );
								formAdvancedSearch.Show();
								formAdvancedSearch.buttonOK_Click( null, null );
								dgvCost[currentColumn, currentRow].Value = formAdvancedSearch.GetWhereClause();
								formAdvancedSearch.Close();
								break;
							default:
								//validCell = false;
								throw new Exception( "Unknown column encountered in dgvCost" );
								//break;
						}
						++currentColumn;
					}
					currentColumn = startColumn;
					++currentRow;
				}
			}

			dgvCost.Update();
		}


        private void toolStripMenuCostsCriteria_Click(object sender, EventArgs e)
        {
            int nRow = 0;
            if (dgvCost.SelectedRows.Count > 0 || dgvCost.SelectedCells.Count > 0)
            {
                if (dgvCost.SelectedRows.Count > 0)
                {
                    nRow = dgvCost.SelectedRows[0].Index;
                }
                else if (dgvCost.SelectedCells.Count > 0)
                {
                    nRow = dgvCost.SelectedCells[0].RowIndex;
                }
            }
            else
            {
                return;
            }
            DataGridViewRow row = dgvCost.Rows[nRow];
            String strValue = "";
            if (row.Cells[1].Value != null)
            {
                strValue = row.Cells[1].Value.ToString();
            }

            FormAdvancedSearch advancedSearch = new FormAdvancedSearch(m_strNetwork, m_hashAttributeYear, strValue, true);
            if (advancedSearch.ShowDialog() == DialogResult.OK)
            {
                row.Cells[1].Value = advancedSearch.GetWhereClause();
                dgvCost.Update();
            }
        }

        private void toolStripMenuConsequenceCopy_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuConsequencePaste_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuConsequenceCriteria_Click(object sender, EventArgs e)
        {
            int nRow = 0;
            if (dgvConsequences.SelectedRows.Count > 0 || dgvConsequences.SelectedCells.Count > 0)
            {
                if (dgvConsequences.SelectedRows.Count > 0)
                {
                    nRow = dgvConsequences.SelectedRows[0].Index;
                }
                else if (dgvConsequences.SelectedCells.Count > 0)
                {
                    nRow = dgvConsequences.SelectedCells[0].RowIndex;
                }
            }
            else
            {
                return;
            }
            DataGridViewRow row = dgvConsequences.Rows[nRow];
            String strValue = "";
            if (row.Cells[3].Value != null)
            {
                strValue = row.Cells[3].Value.ToString();
            }

            FormAdvancedSearch advancedSearch = new FormAdvancedSearch(m_strNetwork, m_hashAttributeYear, strValue, true);
            if (advancedSearch.ShowDialog() == DialogResult.OK)
            {
                row.Cells[3].Value = advancedSearch.GetWhereClause();
            }
        }

        private void dgvConsequences_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_bDeleteFeasible) return;
            if (e.RowIndex > -1)
            {
                if (dgvConsequences.Rows[e.RowIndex].Tag != null)//If tag exist... need to update Treatment
                {
                    String strTag = dgvConsequences.Rows[e.RowIndex].Tag.ToString();
                    String strUpdate = "";

                    String strValue = "";
                    String isFunctionTag = "0";
                    if (dgvConsequences.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        strValue = dgvConsequences.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                        if (dgvConsequences.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag != null)
                        {
                            isFunctionTag = dgvConsequences.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString();
                        }
                    }


                    switch (e.ColumnIndex)
                    {
                        case 0:
                            strUpdate = "UPDATE CONSEQUENCES SET ATTRIBUTE_='" + strValue + "' WHERE CONSEQUENCEID='" + strTag + "'";
                            break;
                        case 1:
                            strUpdate = "UPDATE CONSEQUENCES SET CHANGE_='" + strValue + "' WHERE CONSEQUENCEID='" + strTag + "'";
                            break;
                        
                        case 2:
                            strUpdate = "UPDATE CONSEQUENCES SET EQUATION='" + strValue + "', ISFUNCTION='" + isFunctionTag.ToString() + "' WHERE CONSEQUENCEID='" + strTag + "'";
                            break;
                        case 3:
                            String strCriteria = strValue.Replace("'", "|");
                            strUpdate = "UPDATE CONSEQUENCES SET CRITERIA='" + strCriteria + "' WHERE CONSEQUENCEID='" + strTag + "'";
                            break;
                        default:
                            return;
                    }

                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error: " + except.Message.ToString());

                    }

                }
                else// New insert
                {
                    String strAttribute = "";
                    String strChange = "";
                    String strCriteria = "";
                    String strEquation = "";

                    if (dgvConsequences.Rows[e.RowIndex].Cells[0].Value != null)
                    {
                        strAttribute = dgvConsequences.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();
                    }

                    if (dgvConsequences.Rows[e.RowIndex].Cells[1].Value != null)
                    {
                        strChange = dgvConsequences.Rows[e.RowIndex].Cells[1].Value.ToString().Trim();
                    }
                    if (dgvConsequences.Rows[e.RowIndex].Cells[2].Value != null)
                    {
                        strEquation = dgvConsequences.Rows[e.RowIndex].Cells[2].Value.ToString().Trim();
                    }
                    if (dgvConsequences.Rows[e.RowIndex].Cells[3].Value != null)
                    {
                        strCriteria = dgvConsequences.Rows[e.RowIndex].Cells[3].Value.ToString().Trim();
                    }


                    String strTreatmentID;
                    if (dgvTreatment.SelectedCells.Count > 0)
                    {
                        strTreatmentID = dgvTreatment.Rows[dgvTreatment.SelectedCells[0].RowIndex].Tag.ToString();
                    }
                    else if (dgvTreatment.SelectedRows.Count > 0)
                    {
                        strTreatmentID = dgvTreatment.SelectedRows[0].Tag.ToString();
                    }
                    else
                    {
                        return;
                    }

                    strCriteria = strCriteria.Replace("'", "|");
                    String strInsert = "INSERT INTO CONSEQUENCES (TREATMENTID,ATTRIBUTE_,CHANGE_,EQUATION,CRITERIA) VALUES ('" + strTreatmentID + "','" + strAttribute + "','" + strChange + "','" + strEquation + "','" + strCriteria + "')";
                    try
                    {
						String strIdentity;
                        DBMgr.ExecuteNonQuery(strInsert);
						switch (DBMgr.NativeConnectionParameters.Provider)
						{
							case "MSSQL":
								strIdentity = "SELECT IDENT_CURRENT ('CONSEQUENCES') FROM CONSEQUENCES";
								break;
							case "ORACLE":
								//strIdentity = "SELECT CONSEQUENCES_CONSEQUENCEID_SEQ.CURRVAL FROM DUAL";
								//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'CONSEQUENCES_CONSEQUENCEID_SEQ'";
								strIdentity = "SELECT MAX(CONSEQUENCEID) FROM CONSEQUENCES";
								break;
							default:
								throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
								//break;
						}
                        DataSet ds = DBMgr.ExecuteQuery(strIdentity);
                        strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        dgvConsequences.Rows[e.RowIndex].Tag = strIdentity;
                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error: " + except.Message.ToString());
                    }
                }
            }
        }

        private void FormTreatment_FormClosed(object sender, FormClosedEventArgs e)
        {
			FormUnload();
            FormManager.RemoveFormTreatment(this);
        }


        private void dgvFeasibility_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
			if( Global.SecurityOperations.CanModifySimulationTreatment( m_strNetworkID, m_strSimID ) )
			{
				String strCriteria = "";
				if( dgvFeasibility.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null )
				{
					strCriteria = dgvFeasibility.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
				}
				FormAdvancedSearch formAdvancedSearch = new FormAdvancedSearch( m_strNetwork, m_hashAttributeYear, strCriteria, true );
				if( formAdvancedSearch.ShowDialog() == DialogResult.OK )
				{
					dgvFeasibility[e.ColumnIndex, e.RowIndex].Value = formAdvancedSearch.GetWhereClause();
                    dgvFeasibility.Update();

				}
				if( dgvFeasibility.Rows.Count - 1 == e.RowIndex )
				{
					LoadFeasibility( m_strLastTag );
				}

				dgvFeasibility.Update();
			}
        }

        private void dgvCost_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
			if( e.ColumnIndex == 0 || e.ColumnIndex == 1 )
			{
				if( Global.SecurityOperations.CanModifySimulationTreatment( m_strNetworkID, m_strSimID ) )
				{
					String strCriteria = "";
					if( e.ColumnIndex == 0 )
					{
						String strEquation = "";
                        bool isFunction = false;
						if( dgvCost.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null )
						{
							strEquation = dgvCost.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                            if (dgvCost.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag != null)
                            {
                                if (dgvCost.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString() == "1")
                                {
                                    isFunction = true;
                                }
                                else
                                {
                                    isFunction = false;
                                }
                            }
						}

						FormEditEquation formEditEquation = new FormEditEquation( strEquation, isFunction);
						if( formEditEquation.ShowDialog() == DialogResult.OK )
						{
                            if (formEditEquation.IsFunction)
                            {
                                dgvCost[e.ColumnIndex, e.RowIndex].Tag = "1";
                            }
                            else
                            {
                                dgvCost[e.ColumnIndex, e.RowIndex].Tag = "0";
                            }
							dgvCost[e.ColumnIndex, e.RowIndex].Value = formEditEquation.Equation;

                            try
                            {
                                string strID = dgvCost.Rows[e.RowIndex].Tag.ToString();
                                byte[] assembly = RoadCareGlobalOperations.AssemblySerialize.SerializeObjectToByteArray(formEditEquation.calculate);
                                RoadCareGlobalOperations.GlobalDatabaseOperations.SaveBinaryObjectToDatabase(strID, "COSTID", "COSTS", "BINARY_COST", assembly);
                            }
                            catch (Exception except)
                            {
                                Global.WriteOutput("Error: Updating Costs Criteria." + except.Message);
                            }


                            if (dgvCost.Rows.Count - 1 == e.RowIndex)
                            {
                                LoadFeasibility(m_strLastTag);
                            }
                            dgvCost.Update();
						}
					}
					else if( e.ColumnIndex == 1 )
					{
						if( dgvCost.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null )
						{
							strCriteria = dgvCost.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
						}
						FormAdvancedSearch formAdvancedSearch = new FormAdvancedSearch( m_strNetwork, m_hashAttributeYear, strCriteria, true );
						if( formAdvancedSearch.ShowDialog() == DialogResult.OK )
						{
							dgvCost[e.ColumnIndex, e.RowIndex].Value = formAdvancedSearch.GetWhereClause();
                            dgvCost.Update();


                            if (dgvCost.Rows.Count - 1 == e.RowIndex)
                            {
                                LoadFeasibility(m_strLastTag);
                            }
                        }

                    }
				}

			}
        }

        private void dgvConsequences_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
			if( e.ColumnIndex == 3 && e.RowIndex != -1)
			{
				if( Global.SecurityOperations.CanModifySimulationTreatment( m_strNetworkID, m_strSimID ) )
				{
					String strCriteria = "";
					if( dgvConsequences.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null )
					{
						strCriteria = dgvConsequences.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
					}
					FormAdvancedSearch formAdvancedSearch = new FormAdvancedSearch( m_strNetwork, m_hashAttributeYear, strCriteria, true );
					if( formAdvancedSearch.ShowDialog() == DialogResult.OK )
					{
						dgvConsequences[e.ColumnIndex, e.RowIndex].Value = formAdvancedSearch.GetWhereClause();
                        try
                        {
                            string strID = dgvConsequences.Rows[e.RowIndex].Tag.ToString();
                        }
                        catch (Exception except)
                        {
                            Global.WriteOutput("Error: Updating Consequence Criteria." + except.Message);
                        }

					}
					dgvConsequences.Update();
				}
			}
            if (e.ColumnIndex == 2 && e.RowIndex != -1)
            {
				if( Global.SecurityOperations.CanModifySimulationTreatment( m_strNetworkID, m_strSimID ) )
				{
					String strEquation = "";
                    bool isFunction = false;
					if( dgvConsequences.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null )
					{
                        if (dgvConsequences.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag != null)
                        {
                            if (dgvConsequences.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString() == "1")
                            {
                                isFunction = true;
                            }
                            else
                            {
                                isFunction = false;
                            }
                        }
						strEquation = dgvConsequences.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
					}
				    FormEditEquation formEditEquation = new FormEditEquation( strEquation, isFunction );
                    formEditEquation.IsConsequence = true;
                    if (formEditEquation.ShowDialog() == DialogResult.OK)
                    {
                        if (formEditEquation.IsFunction)
                        {
                            dgvConsequences[e.ColumnIndex, e.RowIndex].Tag = "1";
                        }
                        else
                        {
                            dgvConsequences[e.ColumnIndex, e.RowIndex].Tag = "0";
                        }
                        dgvConsequences[e.ColumnIndex, e.RowIndex].Value = formEditEquation.Equation;
                    }
                    dgvConsequences.Update();
                }
            }
        }


		private void dgvConsequences_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right)
			{
				// Change column index is one.
				if (dgvConsequences.Columns["Change"].Index == e.ColumnIndex)
				{
					if (dgvConsequences["Attribute", e.RowIndex].Value != null)
					{
						// Check for a compound treatment strategy
						string changeText = "";
						FormCompoundTreatment compoundTreatmentEditor = null;
						string affectedAttribute = dgvConsequences["Attribute", e.RowIndex].Value.ToString();

						// Change column index is 1 but the value is blank means create a new compound treatment.
						if (dgvConsequences[e.ColumnIndex, e.RowIndex].Value != null)
						{
							// Value was not blank, check to see if we already have a compound treatment statement.
							changeText = dgvConsequences[e.ColumnIndex, e.RowIndex].Value.ToString();
							if (changeText.Contains("COMPOUND_TREATMENT"))
							{
								// Get teh compound treatment name from the change text box.
								string compoundTreatmentName = changeText.Replace("COMPOUND_TREATMENT(", "");
								compoundTreatmentName = compoundTreatmentName.Replace(")", "");

								// Finally, make sure we have a compound treatment called that, otherwise we need to create a new one.
								CompoundTreatment selectedCompoundTreatment = new CompoundTreatment(compoundTreatmentName);
								if (selectedCompoundTreatment != null)
								{
									compoundTreatmentEditor = new FormCompoundTreatment(selectedCompoundTreatment, affectedAttribute);
								}
								else
								{
									compoundTreatmentEditor = new FormCompoundTreatment(affectedAttribute);
								}
							}
							else
							{
								compoundTreatmentEditor = new FormCompoundTreatment(affectedAttribute);
							}
						}
						else
						{
							compoundTreatmentEditor = new FormCompoundTreatment(affectedAttribute);
						}
						compoundTreatmentEditor.ShowDialog();
					}
					else
					{
						Global.WriteOutput("No attribute selected.  Must select an attribute before creating a compound treatment!");
					}
				}
			}
		}


        private void checkedComboBoxBudget_DropDownClosed(object sender, EventArgs e)
        {
            String strBudget = checkedComboBoxBudget.Text.ToString();
            if (dgvTreatment.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvTreatment.SelectedRows)
                {
                    if (row.Tag == null) continue;
                    String strTag = row.Tag.ToString();
                    String strUpdate = "UPDATE TREATMENTS SET BUDGET='" + strBudget + "' WHERE TREATMENTID='" + strTag + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                        Treatment treatment = (Treatment)m_hashTreatment[strTag];
                        treatment.m_strBudget = strBudget;

                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error: " + except.Message.ToString());

                    }
                }
            }
            if (dgvTreatment.SelectedCells.Count > 0)
            {
                foreach (DataGridViewCell cell in dgvTreatment.SelectedCells)
                {
                    if (dgvTreatment.Rows[cell.RowIndex].Tag == null) continue;
                    String strTag = dgvTreatment.Rows[cell.RowIndex].Tag.ToString();
                    String strUpdate = "UPDATE TREATMENTS SET BUDGET='" + strBudget + "' WHERE TREATMENTID='" + strTag + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                        Treatment treatment = (Treatment)m_hashTreatment[strTag];
                        treatment.m_strBudget = strBudget;
                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error:" + except.Message.ToString());

                    }
                }
            }
        }

        private void checkedComboBoxBudget_ItemCheck(object sender, ItemCheckEventArgs e)
        {

        }

        private void dataGridViewScheduled_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridViewScheduled_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
 
        }

        private void dataGridViewScheduled_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                var strValue = "";
                if (dataGridViewScheduled.Rows[e.RowIndex].Tag == null)
                {
                    var strTreatmentId = "";
                    if (dgvTreatment.SelectedCells.Count > 0)
                    {
                        strTreatmentId = dgvTreatment.Rows[dgvTreatment.SelectedCells[0].RowIndex].Tag.ToString();
                    }
                    else if (dgvTreatment.SelectedRows.Count > 0)
                    {
                        strTreatmentId = dgvTreatment.SelectedRows[0].Tag.ToString();
                    }
                    else
                    {
                        return;
                    }

                    if (dataGridViewScheduled.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        strValue = dataGridViewScheduled.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    }
                    else
                    {
                        return;
                    }


                    var strInsert = "";
                    switch (e.ColumnIndex)
                    {
                        case 0:

                            foreach (var key in m_hashTreatment.Keys)
                            {
                                var treatment = (Treatment)m_hashTreatment[key];
                                if (treatment.m_strTreatment == strValue)
                                {
                                    strInsert =
                                        "INSERT INTO SCHEDULED (TREATMENTID,SCHEDULEDTREATMENTID,SCHEDULEDYEAR) VALUES ('" +
                                        strTreatmentId + "','" +key +"','" + 1 + "')";
                                }
                            }
                            break;
                        case 1:
                            foreach (var key in m_hashTreatment.Keys)
                            {
                                var treatment = (Treatment)m_hashTreatment[key];
                                if (treatment.m_strTreatment == "No Treatment")
                                {
                                    strInsert =
                                        "INSERT INTO SCHEDULED (TREATMENTID,SCHEDULEDTREATMENTID,SCHEDULEDYEAR) VALUES ('" +
                                        strTreatmentId + "','" + key + "','" + strValue + "')";
                                }
                            }
                            break;
                        default:
                            return;
                    }

                   

                    try
                    {
                        String strIdentity;
                        DBMgr.ExecuteNonQuery(strInsert);
                        switch (DBMgr.NativeConnectionParameters.Provider)
                        {
                            case "MSSQL":
                                strIdentity = "SELECT IDENT_CURRENT ('SCHEDULED') FROM SCHEDULED";
                                break;
                            case "ORACLE":
                                strIdentity = "SELECT MAX(SCHEDULEDID) FROM SCHEDULED";
                                break;
                            default:
                                throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                            //break;
                        }

                        DataSet ds = DBMgr.ExecuteQuery(strIdentity);
                        dataGridViewScheduled.Rows[e.RowIndex].Tag = ds.Tables[0].Rows[0].ItemArray[0].ToString();

                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error:" + except.Message.ToString());
                    }
                }
                else
                {
                    String strTag = dataGridViewScheduled.Rows[e.RowIndex].Tag.ToString();
                    String strUpdate = "";


                    if (dataGridViewScheduled.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        strValue = dataGridViewScheduled.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    }
                    else
                    {
                        return;
                    }

                    switch (e.ColumnIndex)
                    {
                        case 0:

                            foreach (var key in m_hashTreatment.Keys)
                            {
                                var treatment = (Treatment)m_hashTreatment[key];
                                if (treatment.m_strTreatment == strValue)
                                {
                                    strUpdate = "UPDATE SCHEDULED SET SCHEDULEDTREATMENTID='" + key + "' WHERE SCHEDULEDID='" + strTag + "'";
                                }
                            }
                            break;
                        case 1:
                            strUpdate = "UPDATE SCHEDULED SET SCHEDULEDYEAR='" + strValue + "' WHERE SCHEDULEDID='" + strTag + "'";
                            break;
                        default:
                            return;
                    }

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(strUpdate))
                        {
                            DBMgr.ExecuteNonQuery(strUpdate);
                        }
                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error: " + except.Message.ToString());

                    }
                }
            }
        }

        private void dataGridViewScheduled_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (e.Row.Tag != null)
            {
                String strDelete = "DELETE FROM SCHEDULED WHERE SCHEDULEDID='" + e.Row.Tag + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strDelete);
                }
                catch (Exception except)
                {
                    Global.WriteOutput("Error Deleting Scheduled Treatment:" + except.Message.ToString());
                }
            }
        }

        private void DataGridViewSupersede_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (e.Row.Tag == null) return;
            var delete = "DELETE FROM SUPERSEDES WHERE SUPERSEDE_ID='" + e.Row.Tag + "'";
            try
            {
                DBMgr.ExecuteNonQuery(delete);
            }
            catch (Exception except)
            {
                Global.WriteOutput("Error Deleting Supersedes Treatment:" + except.Message.ToString());
            }
        }

        private void DataGridViewSupersede_CellValidated(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex > -1)
            {
                var strValue = "";
                if (dataGridViewSupersede.Rows[e.RowIndex].Tag == null)
                {
                    var strTreatmentId = "";
                    if (dgvTreatment.SelectedCells.Count > 0)
                    {
                        strTreatmentId = dgvTreatment.Rows[dgvTreatment.SelectedCells[0].RowIndex].Tag.ToString();
                    }
                    else if (dgvTreatment.SelectedRows.Count > 0)
                    {
                        strTreatmentId = dgvTreatment.SelectedRows[0].Tag.ToString();
                    }
                    else
                    {
                        return;
                    }

                    if (dataGridViewSupersede.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        strValue = dataGridViewSupersede.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    }
                    else
                    {
                        return;
                    }


                    var strInsert = "";
                    switch (e.ColumnIndex)
                    {
                        case 0:

                            foreach (var key in m_hashTreatment.Keys)
                            {
                                var treatment = (Treatment)m_hashTreatment[key];
                                if (treatment.m_strTreatment == strValue)
                                {
                                    strInsert =
                                        "INSERT INTO SUPERSEDES (TREATMENT_ID,SUPERSEDE_TREATMENT_ID) VALUES ('" +
                                        strTreatmentId + "','" + treatment.m_strID + "')";
                                }
                            }
                            break;
                        case 1:
                            strValue = strValue.Trim();
                            strValue = strValue.Replace("'", "|");
                            strInsert = "INSERT INTO SUPERSEDES (TREATMENT_ID,CRITERIA) VALUES ('" + strTreatmentId + "','" + strValue + "')";
                            break;
                        default:
                            return;
                    }



                    try
                    {
                        String strIdentity;
                        DBMgr.ExecuteNonQuery(strInsert);
                        switch (DBMgr.NativeConnectionParameters.Provider)
                        {
                            case "MSSQL":
                                strIdentity = "SELECT IDENT_CURRENT ('SUPERSEDES') FROM SUPERSEDES";
                                break;
                            case "ORACLE":
                                strIdentity = "SELECT MAX(SUPERSEDE_ID) FROM SUPERSEDES";
                                break;
                            default:
                                throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                                //break;
                        }

                        var ds = DBMgr.ExecuteQuery(strIdentity);
                        dataGridViewSupersede.Rows[e.RowIndex].Tag = ds.Tables[0].Rows[0].ItemArray[0].ToString();

                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error:" + except.Message.ToString());
                    }
                }
                else
                {
                    String strTag = dataGridViewSupersede.Rows[e.RowIndex].Tag.ToString();
                    String strUpdate = "";


                    if (dataGridViewSupersede.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        strValue = dataGridViewSupersede.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    }
                    else
                    {
                        return;
                    }

                    switch (e.ColumnIndex)
                    {
                        case 0:

                            foreach (var key in m_hashTreatment.Keys)
                            {
                                var treatment = (Treatment)m_hashTreatment[key];
                                if (treatment.m_strTreatment == strValue)
                                {
                                    strUpdate = "UPDATE SUPERSEDES SET SUPERSEDE_TREATMENT_ID='" + treatment.m_strID + "' WHERE SUPERSEDE_ID='" + strTag + "'";
                                }
                            }
                            break;
                        case 1:
                            strValue = strValue.Trim();
                            strValue = strValue.Replace("'", "|");
                            strUpdate = "UPDATE SUPERSEDES SET CRITERIA ='" + strValue + "' WHERE SUPERSEDE_ID='" + strTag + "'";
                            break;
                        default:
                            return;
                    }

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(strUpdate))
                        {
                            DBMgr.ExecuteNonQuery(strUpdate);
                        }
                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error: " + except.Message.ToString());

                    }
                }
            }




        }

        private void DataGridViewSupersede_DoubleClick(object sender, EventArgs e)
        {

        }

        private void DataGridViewSupersede_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex != -1)
            {
                if (Global.SecurityOperations.CanModifySimulationTreatment(m_strNetworkID, m_strSimID))
                {
                    String strCriteria = "";
                    if (dataGridViewSupersede.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        strCriteria = dataGridViewSupersede.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    }
                    FormAdvancedSearch formAdvancedSearch = new FormAdvancedSearch(m_strNetwork, m_hashAttributeYear, strCriteria, true);
                    if (formAdvancedSearch.ShowDialog() == DialogResult.OK)
                    {
                        dataGridViewSupersede[e.ColumnIndex, e.RowIndex].Value = formAdvancedSearch.GetWhereClause();
                        try
                        {
                            string strID = dataGridViewSupersede.Rows[e.RowIndex].Tag.ToString();
                        }
                        catch (Exception except)
                        {
                            Global.WriteOutput("Error: Updating Consequence Criteria." + except.Message);
                        }

                    }
                    dataGridViewSupersede.Update();
                }
            }
        }
    }

    public class Treatment
    {
        public String m_strID;
        public String m_strTreatment;
        public String m_strDescription;
        public String m_strAny;
        public String m_strSame;
        public String m_strBudget;
    }














}