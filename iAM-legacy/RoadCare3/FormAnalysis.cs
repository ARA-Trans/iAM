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
using System.Threading;
using RoadCare3.Properties;
using RoadCareDatabaseOperations;
using DataAccessLayer;
using SimulationDataAccess;

namespace RoadCare3
{
	public partial class FormAnalysis : BaseForm
    {
        private String m_strNetwork;

        private String m_strSimulation;
        private String m_strSimulationID;
        private String m_strNetworkID;
        private Hashtable m_hashAttributeYear;

        private Thread m_simulationThread;
        private Simulation.Simulation m_simulation;
        private bool m_bChange = false;
        private object m_oLastCellPriority;
        private object m_oLastCellTarget;
        private object m_oLastCellDeficient;
        private String m_strLastLimit;

		private LockInformation simulationLock;

        public FormAnalysis(String strNetwork, String strSimulation, String strSimulationID, Hashtable hashAttributeYear)
        {
            InitializeComponent();

            m_strNetwork = strNetwork;
			m_strNetworkID = DBOp.GetNetworkIDFromName( strNetwork );
            m_strSimulation = strSimulation;
            m_strSimulationID = strSimulationID;
            m_hashAttributeYear = hashAttributeYear;
        }

		protected void SecureForm()
		{
			LockDataGridView( dgvPriority );
			LockDataGridView( dgvTarget );
			LockDataGridView( dgvDeficient );

			if( !Global.SecurityOperations.CanViewSimulationAnalysisPriority( m_strNetworkID, m_strSimulationID ) )
			{
				//tabPagePriority.CanSelect = false;
				tabControlAnalysis.Controls.Remove( tabPagePriority );
			}
			else
			{
				if( Global.SecurityOperations.CanModifySimulationAnalysisPriority( m_strNetworkID, m_strSimulationID ) )
				{
					if( Global.SecurityOperations.CanCreateSimulationAnalysisPriority( m_strNetworkID, m_strSimulationID ) )
					{
						if( Global.SecurityOperations.CanRemoveSimulationAnalysisPriority( m_strNetworkID, m_strSimulationID ) )
						{
							UnlockDataGridViewForCreateDestroy( dgvPriority );
						}
						else
						{
							UnlockDataGridViewForCreate( dgvPriority );
						}
					}
					else
					{
						UnlockDataGridViewForModify( dgvPriority );
					}
				}
			}
			
			if( !Global.SecurityOperations.CanViewSimulationAnalysisTargets( m_strNetworkID, m_strSimulationID ) )
			{
				//tabPageTarget.CanSelect = false;
				tabControlAnalysis.Controls.Remove( tabPageTarget );
			}
			else
			{
				if( Global.SecurityOperations.CanModifySimulationAnalysisTargets( m_strNetworkID, m_strSimulationID ) )
				{
					if( Global.SecurityOperations.CanCreateSimulationAnalysisTargets( m_strNetworkID, m_strSimulationID ) )
					{
						if( Global.SecurityOperations.CanRemoveSimulationAnalysisTargets( m_strNetworkID, m_strSimulationID ) )
						{
							UnlockDataGridViewForCreateDestroy( dgvTarget );
						}
						else
						{
							UnlockDataGridViewForCreate( dgvTarget );
						}
					}
					else
					{
						UnlockDataGridViewForModify( dgvTarget );
					}
				}
			}

			if( !Global.SecurityOperations.CanViewSimulationAnalysisDeficients( m_strNetworkID, m_strSimulationID ) )
			{
				//tabPageDeficient.CanSelect = false;
				tabControlAnalysis.Controls.Remove( tabPageDeficient );
			}
			else
			{
				if( Global.SecurityOperations.CanModifySimulationAnalysisDeficients( m_strNetworkID, m_strSimulationID ) )
				{
					if( Global.SecurityOperations.CanCreateSimulationAnalysisDeficients( m_strNetworkID, m_strSimulationID ) )
					{
						if( Global.SecurityOperations.CanRemoveSimulationAnalysisDeficients( m_strNetworkID, m_strSimulationID ) )
						{
							UnlockDataGridViewForCreateDestroy( dgvDeficient );
						}
						else
						{
							UnlockDataGridViewForCreate( dgvDeficient );
						}
					}
					else
					{
						UnlockDataGridViewForModify( dgvDeficient );
					}
				}
			}


			LockButton( buttonRunSimulation );
			LockButton( buttonCriteria );

			LockComboBox( cbBenefit );
			LockComboBox( cbBudget );
			LockComboBox( cbOptimization );
			LockComboBox( cbWeighting );

			LockTextBox( tbBenefitLimit );
			LockTextBox( tbDescription );
			LockTextBox( tbJurisdiction );
            LockCheckBox(checkBoxMultipleCost);

			if( Global.SecurityOperations.CanModifySimulationAnalysisData( m_strNetworkID, m_strSimulationID ) )
			{
				UnlockButton( buttonRunSimulation );

				UnlockComboBox( cbBenefit );
				UnlockComboBox( cbBudget );
				UnlockComboBox( cbOptimization );
				UnlockComboBox( cbWeighting );

				UnlockTextBox( tbBenefitLimit );
				UnlockTextBox( tbDescription );
			    UnlockCheckBox(checkBoxMultipleCost);
                if (Global.SecurityOperations.CanModifySimulationAnalysisJurisdiction(m_strNetworkID, m_strSimulationID))
				{
					UnlockTextBox(tbJurisdiction);
					UnlockButton( buttonCriteria );
				}

			}
		}

        private void FormAnalysis_Load(object sender, EventArgs e)
        {
			SecureForm();
			FormLoad( Settings.Default.SIMULATION_IMAGE_KEY, Settings.Default.SIMULATION_IMAGE_KEY_SELECTED );
			
			// Get the networkID for this simulation
            DataSet ds = null;
            try
            {
                ds = DBMgr.ExecuteQuery("SELECT NETWORKID FROM SIMULATIONS WHERE SIMULATIONID = '" + m_strSimulationID + "'");
                m_strNetworkID = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: Could not get NETWORKID from SIMULATIONS. " + exc.Message);
            }

            // Load the attribute hash.
            Global.LoadAttributes();
            this.labelAnalysis.Text = "Analysis Methods for " + m_strSimulation + " : " + m_strNetwork;

            cbWeighting.Items.Add("none");

            List<string> attributes = new List<string>();
            foreach (string attribute in Global.Attributes)
            {
                attributes.Add(attribute);
            }

            attributes.Sort();

            foreach (String strAttribute in attributes)
            {
                if ("NUMBER" == Global.GetAttributeType(strAttribute))
                {
                    cbWeighting.Items.Add(strAttribute);
                    cbBenefit.Items.Add(strAttribute);
                    this.Attribute.Items.Add(strAttribute);
                    this.DeficientAttribute.Items.Add(strAttribute);
                    RemainingLifeAttribute.Items.Add(strAttribute);

                }
            }

            String strSelect = "SELECT COMMENTS,JURISDICTION,ANALYSIS,BUDGET_CONSTRAINT,WEIGHTING,BENEFIT_VARIABLE,BENEFIT_LIMIT,USE_CUMULATIVE_COST,USE_ACROSS_BUDGET,USE_REASONS FROM SIMULATIONS WHERE SIMULATIONID='" + m_strSimulationID + "'";
            ds = new DataSet();

            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);

            }
            catch (Exception except)
            {
                Global.WriteOutput("Error: " + except.Message);
                return;
            }

            String strDescription = ds.Tables[0].Rows[0].ItemArray[0].ToString().Replace("|","'");
            String strJurisdiction = ds.Tables[0].Rows[0].ItemArray[1].ToString().Replace("|","'");
            String strAnalysis = ds.Tables[0].Rows[0].ItemArray[2].ToString();
            String strBudgetConstraint = ds.Tables[0].Rows[0].ItemArray[3].ToString();
            String strWeighting = ds.Tables[0].Rows[0].ItemArray[4].ToString();
            String strBenefitVariable = ds.Tables[0].Rows[0].ItemArray[5].ToString();
            String strBenefitLimit = ds.Tables[0].Rows[0].ItemArray[6].ToString();
            var useCumulativeCost = false;

            if (ds.Tables[0].Rows[0]["USE_CUMULATIVE_COST"] != DBNull.Value)
            {
                useCumulativeCost = Convert.ToBoolean(ds.Tables[0].Rows[0]["USE_CUMULATIVE_COST"]);
            }

            var useAcrossBudget = false;

            if (ds.Tables[0].Rows[0]["USE_ACROSS_BUDGET"] != DBNull.Value)
            {
                useAcrossBudget = Convert.ToBoolean(ds.Tables[0].Rows[0]["USE_ACROSS_BUDGET"]);
            }


            checkBoxMultipleCost.Checked = useCumulativeCost;

            if (useAcrossBudget)
            {
                radioButtonAcrossBudget.Checked = true;
            }
            else
            {
                radioButtonWithinBudget.Checked = true;
            }

            var useReasons = true;
            if (ds.Tables[0].Rows[0]["USE_REASONS"] != DBNull.Value)
            {
                useReasons = Convert.ToBoolean(ds.Tables[0].Rows[0]["USE_REASONS"]);
            }
            checkBoxReasons.Checked = useReasons;


            cbBenefit.Text = strBenefitVariable.ToString();
            cbBudget.Text = strBudgetConstraint.ToString();
            cbOptimization.Text = strAnalysis.ToString();
            cbWeighting.Text = strWeighting.ToString();
            tbJurisdiction.Text = strJurisdiction.ToString();
            tbBenefitLimit.Text = strBenefitLimit.ToString();
            tbDescription.Text = strDescription.ToString();


			strSelect = "SELECT ID_,ATTRIBUTE_,TARGETNAME,YEARS,TARGETMEAN,CRITERIA FROM TARGETS WHERE SIMULATIONID='" + m_strSimulationID + "'";
			try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception except)
            {
                Global.WriteOutput("Error: " + except.Message);
                return;
            }

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string[] row =  { dr[1].ToString(),dr[2].ToString(),dr[3].ToString(),dr[4].ToString(),dr[5].ToString().Replace("|","'") };
                int nIndex = dgvTarget.Rows.Add(row);
                dgvTarget.Rows[nIndex].Tag = dr[0].ToString();
            }

			strSelect = "SELECT ID_,ATTRIBUTE_,DEFICIENTNAME,DEFICIENT,PERCENTDEFICIENT,CRITERIA FROM DEFICIENTS WHERE SIMULATIONID='" + m_strSimulationID + "'";
			try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception except)
            {
                Global.WriteOutput("Error: " + except.Message);
                return;
            }

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string[] row =  { dr[1].ToString(),  dr[2].ToString(),dr[3].ToString(), dr[4].ToString(),dr[5].ToString().Replace("|","'") };
                int nIndex = dgvDeficient.Rows.Add(row);
                dgvDeficient.Rows[nIndex].Tag = dr[0].ToString();
            }


            strSelect = "SELECT BUDGETORDER FROM INVESTMENTS WHERE SIMULATIONID='" + m_strSimulationID + "'";
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception except)
            {
                Global.WriteOutput("Error: " + except.Message);
                return;
            }
            DataGridViewTextBoxColumn column;

            column = new DataGridViewTextBoxColumn();
            column.HeaderCell.Value = "Priority";
            column.Tag = "Priority";
            column.Name = "Priority";
            dgvPriority.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.HeaderCell.Value = "Year";
            column.Tag = "Year";
            column.Name = "Year";
            dgvPriority.Columns.Add(column);
            
            column = new DataGridViewTextBoxColumn();
            column.HeaderCell.Value = "Criteria";
            column.Tag = "Criteria";
            column.Name = "Criteria";
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            
            dgvPriority.Columns.Add(column);

			string[] listBudgets = null;

			if( ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0].ItemArray.Length > 0 )
			{

				listBudgets = ds.Tables[0].Rows[0].ItemArray[0].ToString().Split( ',' );
				foreach( string str in listBudgets )
				{
					column = new DataGridViewTextBoxColumn();
					column.HeaderCell.Value = str;
					column.Tag = str;
					column.Name = str;
					dgvPriority.Columns.Add( column );
				}
			}


            strSelect = "SELECT PRIORITYLEVEL,YEARS,CRITERIA,PRIORITYID FROM PRIORITY WHERE SIMULATIONID='" + m_strSimulationID + "' ORDER BY PRIORITYLEVEL";
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception except)
            {
                Global.WriteOutput("Error: " + except.Message);
                return;
            }

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                String strLevel = dr[0].ToString();
                String strYears = dr[1].ToString();
                String strCriteria = dr[2].ToString().Replace("|","'");
                String strPriorityID = dr[3].ToString();

                string[] row =  { strLevel, strYears, strCriteria };
                int nIndex = dgvPriority.Rows.Add(row);
                dgvPriority.Rows[nIndex].Tag = strPriorityID;

                int nCount = 3;
                foreach (string str in listBudgets)
                {
                    strSelect = "SELECT FUNDING,PRIORITYFUNDID FROM PRIORITYFUND WHERE PRIORITYID='" + strPriorityID + "' AND BUDGET='" + str + "'";
                    DataSet dsFunding = DBMgr.ExecuteQuery(strSelect);
                    if (dsFunding.Tables[0].Rows.Count == 1)
                    {
                        dgvPriority.Rows[nIndex].Cells[nCount].Value = dsFunding.Tables[0].Rows[0].ItemArray[0].ToString();
                        dgvPriority.Rows[nIndex].Cells[nCount].Tag = dsFunding.Tables[0].Rows[0].ItemArray[1].ToString();
                    }
                    else
                    {


                    }
                    nCount++;
                }
            }



            //Add Remaining Life Limit
            var select =
                "SELECT REMAINING_LIFE_ID, ATTRIBUTE_,REMAINING_LIFE_LIMIT, CRITERIA FROM REMAINING_LIFE_LIMITS WHERE SIMULATION_ID='" +
                m_strSimulationID + "'";


                var datasetRemainingLife = DBMgr.ExecuteQuery(select);

                dataGridViewRemainLife.Rows.Clear();
                dataGridViewRemainLife.Columns[2].ReadOnly = true;

                foreach (DataRow dr in datasetRemainingLife.Tables[0].Rows)
                {
                    var attribute = dr["ATTRIBUTE_"].ToString();
                    float remainingLifeLimit = 0;
                    if (dr["REMAINING_LIFE_LIMIT"] != DBNull.Value)
                        remainingLifeLimit = Convert.ToSingle(dr["REMAINING_LIFE_LIMIT"]);
                    var criteria = "";
                    if (dr["CRITERIA"] != DBNull.Value) criteria = dr["CRITERIA"].ToString();

                    int nIndex = dataGridViewRemainLife.Rows.Add(attribute, remainingLifeLimit, criteria.Replace("|","'"));
                    dataGridViewRemainLife.Rows[nIndex].Tag = dr["REMAINING_LIFE_ID"].ToString();
                }


            m_bChange = true;


        }
        /// <summary>
        /// Update Optimization method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbOptimization_SelectedIndexChanged(object sender, EventArgs e)
        {
            String strOptimization = cbOptimization.Text.ToString();
            String strUpdate = "UPDATE SIMULATIONS SET ANALYSIS='" + strOptimization + "' WHERE SIMULATIONID='" + m_strSimulationID + "'";

            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error updating analysis: " + exception.Message.ToString());
                return;
            }


        }
        /// <summary>
        /// Update budget spending method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbBudget_SelectedIndexChanged(object sender, EventArgs e)
        {
            String strBudget = cbBudget.Text.ToString();
            String strUpdate = "UPDATE SIMULATIONS SET BUDGET_CONSTRAINT='" + strBudget + "' WHERE SIMULATIONID='" + m_strSimulationID + "'";

            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error updating budget constraint: " + exception.Message.ToString());
                return;
            }

        }

        /// <summary>
        /// Update weighting variable.  The variable which will be multiplied by Benefit, to get a weighted benefit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbWeighting_SelectedIndexChanged(object sender, EventArgs e)
        {
            String strWeighting = cbWeighting.Text.ToString();
            String strUpdate = "UPDATE SIMULATIONS SET WEIGHTING='" + strWeighting + "' WHERE SIMULATIONID='" + m_strSimulationID + "'";

            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error updating weighting variable: " + exception.Message.ToString());
                return;
            }
        }
        /// <summary>
        /// Update the selected benefit variable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbBenefit_SelectedIndexChanged(object sender, EventArgs e)
        {
            String strBenefit = cbBenefit.Text.ToString();
            String strUpdate = "UPDATE SIMULATIONS SET BENEFIT_VARIABLE='" + strBenefit + "' WHERE SIMULATIONID='" + m_strSimulationID + "'";

            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error updating Benefit variable: " + exception.Message.ToString());
                return;
            }

        }

        private void tbDescription_Validated(object sender, EventArgs e)
        {
            String strDescription = tbDescription.Text.ToString();
            strDescription = strDescription.Replace("'", "|");
            String strUpdate = "UPDATE SIMULATIONS SET COMMENTS='" + strDescription + "' WHERE SIMULATIONID='" + m_strSimulationID + "'";

            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error updating Description: " + exception.Message.ToString());
                return;
            }
        }


        private void tbBenefitLimit_Validated(object sender, EventArgs e)
      
        {
            if (!m_bChange) return;
            String strBenefitLimit = tbBenefitLimit.Text.ToString();
            String strUpdate = "UPDATE SIMULATIONS SET BENEFIT_LIMIT='" + strBenefitLimit + "' WHERE SIMULATIONID='" + m_strSimulationID + "'";

            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);
            }
            catch (Exception exception)
            {
                m_bChange = false;
                tbBenefitLimit.Text = m_strLastLimit;
                Global.WriteOutput("Error updating Benefit Limit: " + exception.Message.ToString());
                m_bChange = true;
                return;
            }
        }


        private void tbJurisdiction_Validated(object sender, EventArgs e)
        {
            String strJurisdiction = tbJurisdiction.Text.ToString();
            strJurisdiction = strJurisdiction.Replace("'", "|");
            String strUpdate = "UPDATE SIMULATIONS SET JURISDICTION='" + strJurisdiction + "' WHERE SIMULATIONID='" + m_strSimulationID + "'";

            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error updating Jurisdiction: " + exception.Message.ToString());
                return;
            }
        }

        private void buttonCriteria_Click(object sender, EventArgs e)
        {
            FormAdvancedSearch form = new FormAdvancedSearch(m_strNetwork, m_hashAttributeYear, tbJurisdiction.Text, false);
            if (form.ShowDialog() == DialogResult.OK)
            {
                tbJurisdiction.Text = form.Query;
                String strUpdate = "UPDATE SIMULATIONS SET JURISDICTION='" + form.Query.Replace("'","|") + "' WHERE SIMULATIONID=" + m_strSimulationID;
                try
                {
                    DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch(Exception exception)
                {
                    Global.WriteOutput("Error: Updating JURISDICTION." + exception.Message);

                }

            }
        }

		private void buttonRunSimulation_Click( object sender, EventArgs e )
		{
            RunSimulation();
        }
        

		public void RunSimulation()
		{
            String omsConnectionString = DataAccessLayer.ImportOMS.GetOMSConnectionString(DBMgr.GetNativeConnection().ConnectionString);
            if(!String.IsNullOrWhiteSpace(omsConnectionString))
            {
                string networkArea = SimulationData.GetNetworkSpecificArea(m_strNetworkID);
                PrepareAnalysis.SetSimulationArea(m_strSimulationID, networkArea);
                PrepareAnalysis.Attributes(m_strSimulationID);
            }
            LockInformation simulationCheck = Global.GetLockInfo(m_strNetworkID, m_strSimulationID);
            if (!simulationCheck.Locked)
            {
                LockInformation networkCheck = Global.GetLockInfo(m_strNetworkID, "");
                if (!networkCheck.Locked || networkCheck.NetworkReadable)
                {
                    simulationLock = Global.LockSimulation(m_strNetworkID, m_strSimulationID, false);
                    CloseButton = false;
                    buttonRunSimulation.Enabled = false;
                    if (cbOptimization.Text == "Genetic Algorithm")
                    {
                        CallGeneticAlgorithm();
                    }
                    FormSimulationResults formSimulationResults;
                    if (FormManager.IsFormSimulationResultsOpen(m_strSimulationID, out formSimulationResults))
                    {
                        formSimulationResults.Close();
                    }
                    Global.ClearOutputWindow();
                    m_simulation = new Simulation.Simulation(m_strSimulation, m_strNetwork, m_strSimulationID, m_strNetworkID);

                    timerSimulation.Start();
                    this.Cursor = Cursors.WaitCursor;
                    m_simulationThread = new Thread(new ParameterizedThreadStart(m_simulation.CompileSimulation));
                    //m_simulationThread.Priority = ThreadPriority.Highest;
                    m_simulationThread.Start(false);
                }
                else
                {
                    Global.WriteOutput(networkCheck.LockOwner + " started performing an action locking this network at " + networkCheck.Start.ToString() + ".");
                }
            }
            else
            {
                Global.WriteOutput(simulationCheck.LockOwner + " started performing an action locking this simulation at " + simulationCheck.Start.ToString() + ".");
            }

		}

        private void CallGeneticAlgorithm()
        {
            throw new NotImplementedException();
        }

        private void timerSimulation_Tick(object sender, EventArgs e)
        {
            List<Simulation.SimulationMessage> listSimulation = Simulation.SimulationMessaging.GetProgressList();
            lock (listSimulation)
            {
				String strOut = "";
                var isProgress = false;
                foreach (Simulation.SimulationMessage message in listSimulation)
                {
                    if(strOut.Length == 0 || !message.IsProgress)
                    {
                        strOut += message.Message + "\n";
                        if (message.IsProgress) isProgress = true;
                    }
                }
                if (isProgress)
                {
                    Global.ReplaceOutput(strOut);
                }
                else
                {
                    Global.WriteOutput(strOut);
                }
            }
            Simulation.SimulationMessaging.ClearProgressList();

            


            if (!m_simulationThread.IsAlive)
            {
                timerSimulation.Stop();
                this.Cursor = Cursors.Default;
				CloseButton = true;

				simulationLock.UnlockNetwork();

				buttonRunSimulation.Enabled = true;
				Refresh();
                m_simulation = null;
                GC.Collect();
            }
        }

        private void dgvPriority_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (!m_bChange) return;
            int nRow = e.RowIndex;
            int nCol = e.ColumnIndex;

            if (dgvPriority.Rows[nRow].Tag != null)
            {
                DataGridViewRow row = dgvPriority.Rows[nRow];
                String strValue = "";
                if (row.Cells[nCol].Value != null)
                {
                    strValue = row.Cells[nCol].Value.ToString();
                }

                //strSelect = "SELECT PRIORITYLEVEL,CRITERIA FROM PRIORITY WHERE SIMULATIONID='" + m_strSimulationID + "'";
                
                String strUpdate = "";
                if (nCol == 0)
                {
                    strUpdate = "UPDATE PRIORITY SET PRIORITYLEVEL='" + strValue + "' WHERE PRIORITYID='" + dgvPriority.Rows[nRow].Tag.ToString() + "'";
                }

                if (nCol == 1)
                {
                    string years = strValue;
                    try
                    {
                        if (String.IsNullOrWhiteSpace(years))
                        {
                            years = null;
                        }
                        else
                        {
                            int nYears = int.Parse(years);
                            if (nYears > 10000 && nYears < 1900) years = null;
                        }
                    }
                    catch
                    {
                        years = null;
                    }
                    if (years == null) strUpdate = "UPDATE PRIORITY SET YEARS=NULL WHERE PRIORITYID='" + dgvPriority.Rows[nRow].Tag.ToString() + "'";
                    else strUpdate = "UPDATE PRIORITY SET YEARS='" + strValue + "' WHERE PRIORITYID='" + dgvPriority.Rows[nRow].Tag.ToString() + "'";
                }

                else if (nCol == 2)
                {
                    strValue = strValue.Replace("'", "|");
                    strUpdate = "UPDATE PRIORITY SET CRITERIA='" + strValue + "' WHERE PRIORITYID='" + dgvPriority.Rows[nRow].Tag.ToString() + "'";
                }
                if (strUpdate.Length > 0)
                {
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                    }
                    catch (Exception except)
                    {
                        m_bChange = false;
                        dgvPriority.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastCellPriority;
                        Global.WriteOutput("Error Updating level/criteria:" + except.Message.ToString());
                        m_bChange = true;
                        return;
                    }
                    return;
                }


                //If execution reaches here... Updating or Insert Funding levels.

                if (row.Cells[nCol].Tag != null)//Update
                {
                    strUpdate = "UPDATE PRIORITYFUND SET FUNDING='" + strValue + "' WHERE PRIORITYFUNDID='" + row.Cells[nCol].Tag.ToString() + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                    }
                    catch (Exception except)
                    {
                        m_bChange = false;
                        dgvPriority.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastCellPriority;
                        Global.WriteOutput("Error updating Funding level:" + except.Message.ToString());
                        m_bChange = false;
                        return;
                    }
                    return;
                }
                else
                {
                    String strBudget = dgvPriority.Columns[nCol].Name.ToString();
                    String strInsert = "INSERT INTO PRIORITYFUND (PRIORITYID,BUDGET,FUNDING) VALUES ('" + dgvPriority.Rows[nRow].Tag.ToString() + "','" + strBudget + "','" + row.Cells[nCol].Value.ToString() +"')";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strInsert);
						String strIdentity = "";
						switch (DBMgr.NativeConnectionParameters.Provider)
						{
							case "MSSQL":
								strIdentity = "SELECT IDENT_CURRENT ('PRIORITYFUND') FROM PRIORITYFUND";
								break;
							case "ORACLE":
								//strIdentity = "SELECT PRIORITYFUND_PRIORITYFUNDID_SE.CURRVAL FROM DUAL";
								//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'PRIORITYFUND_PRIORITYFUNDID_SE'";
								strIdentity = "SELECT MAX(PRIORITYFUNDID) FROM PRIORITYFUND";
								break;
							default:
								throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
								//break;
						}
						DataSet ds = DBMgr.ExecuteQuery( strIdentity );
                        strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        row.Cells[nCol].Tag = strIdentity;
                    }
                    catch (Exception except)
                    {
                        m_bChange = false;
                        dgvPriority.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastCellPriority;
                        Global.WriteOutput("Error inserting new Priority:" + except.Message.ToString());
                        m_bChange = true;
                        return;
                    }
                }
            }//Insert
            else
            {
                String strPriority = "";
                String strCriteria = "";


                DataGridViewRow row = dgvPriority.Rows[nRow];

                if (nCol == 0)
                {
                    if(row.Cells[nCol].Value != null)
                        strPriority = row.Cells[nCol].Value.ToString();
                }
                else if (nCol == 1)
                {
                    if (row.Cells[nCol].Value != null)
                        strCriteria = row.Cells[nCol].Value.ToString();
                }
                //If new row, always insert this.  Even if strPriority and strCriteria are blank.
                strCriteria = strCriteria.Replace("'", "|");
                String strInsert = "INSERT INTO PRIORITY (SIMULATIONID,PRIORITYLEVEL,CRITERIA) VALUES ('" + m_strSimulationID + "','" + strPriority + "','" + strCriteria + "')";
                String strPriorityTag = "";
                try
                {
                    DBMgr.ExecuteNonQuery(strInsert);
					String strIdentity = "";
					switch (DBMgr.NativeConnectionParameters.Provider)
					{
						case "MSSQL":
							strIdentity = "SELECT IDENT_CURRENT ('PRIORITY') FROM PRIORITY";
							break;
						case "ORACLE":
							//strIdentity = "SELECT PRIORITY_PRIORITYID_SEQ.CURRVAL FROM DUAL";
							//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'PRIORITY_PRIORITYID_SEQ'";
							strIdentity = "SELECT MAX(PRIORITYID) FROM PRIORITY";
							break;
						default:
							throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
							//break;

					}
					DataSet ds = DBMgr.ExecuteQuery( strIdentity );
                    strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    dgvPriority.Rows[e.RowIndex].Tag = strIdentity;
                    strPriorityTag = strIdentity;
                }
                catch (Exception except)
                {
                    m_bChange = false;
                    dgvPriority.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastCellPriority;
                    Global.WriteOutput("Error inserting new Priority:" + except.Message.ToString());
                    m_bChange = true;
                    return;
                }


                if (nCol > 1)
                {

                    String strBudget = dgvPriority.Columns[nCol].Name.ToString();
                    strInsert = "INSERT INTO PRIORITYFUND (PRIORITYID,BUDGET,FUNDING) VALUES ('" + strPriorityTag + "','" + strBudget + "','" + row.Cells[nCol].Value.ToString() + "')";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strInsert);
						String strIdentity = "";

						switch (DBMgr.NativeConnectionParameters.Provider)
						{
							case "MSSQL":
								strIdentity = "SELECT IDENT_CURRENT ('PRIORITYFUND') FROM PRIORITYFUND";
								break;
							case "ORACLE":
								//strIdentity = "SELECT PRIORITYFUND_PRIORITYFUNDID_SE.CURRVAL FROM DUAL";
								//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'PRIORITYFUND_PRIORITYFUNDID_SE'";
								strIdentity = "SELECT MAX(PRIORITYFUNDID) FROM PRIORITYFUND";
								break;
							default:
								throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
								//break;
						}
						DataSet ds = DBMgr.ExecuteQuery( strIdentity );
                        strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        row.Cells[nCol].Tag = strIdentity;
                    }
                    catch (Exception except)
                    {
                        m_bChange = false;
                        dgvPriority.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastCellPriority;
                        Global.WriteOutput("Error inserting new Priority:" + except.Message.ToString());
                        m_bChange = true;
                        return;
                    }
                }
            }
        }

        private void dgvPriority_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {

            if (e.Row.Tag != null)
            {
                String strDelete = "DELETE FROM PRIORITY WHERE PRIORITYID='" + e.Row.Tag.ToString() + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strDelete);
                }
                catch (Exception except)
                {
                    Global.WriteOutput("Error Deleting Priority:" + except.Message.ToString());
                }
            }
        }

        private void dgvTarget_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_bChange) return;
            int nRow = e.RowIndex;
            int nCol = e.ColumnIndex;

            if (dgvTarget.Rows[nRow].Tag != null)//Update
            {
                DataGridViewRow row = dgvTarget.Rows[nRow];
                String strValue = "";
                if (row.Cells[nCol].Value != null)
                {
                    strValue = row.Cells[nCol].Value.ToString();
                }

                strValue = strValue.Replace("'", "|");
                String strTag = row.Tag.ToString();
                String strUpdate = "";

                if (nCol == 0)
                {
					strUpdate = "UPDATE TARGETS SET ATTRIBUTE_='" + strValue + "' WHERE ID_='" + strTag + "'";
                }
                else if (nCol == 1)
                {
                    if (strValue.Length == 0)
						strUpdate = "UPDATE TARGETS SET TARGETNAME=NULL WHERE ID_='" + strTag + "'";
                    else
						strUpdate = "UPDATE TARGETS SET TARGETNAME='" + strValue + "' WHERE ID_='" + strTag + "'";
                }
                else if (nCol == 2)
                {
                    if(strValue.Length == 0)
						strUpdate = "UPDATE TARGETS SET YEARS=NULL WHERE ID_='" + strTag + "'";
                    else
						strUpdate = "UPDATE TARGETS SET YEARS='" + strValue + "' WHERE ID_='" + strTag + "'";
                }
                else if (nCol == 3)
                {
                    if (strValue.Length == 0)
						strUpdate = "UPDATE TARGETS SET TARGETMEAN=NULL WHERE ID_='" + strTag + "'";
                    else
                        strUpdate = "UPDATE TARGETS SET TARGETMEAN='" + strValue + "' WHERE ID_='" + strTag + "'";
                }
                else if (nCol == 4)
                {
                    strUpdate = "UPDATE TARGETS SET CRITERIA='" + strValue + "' WHERE ID_='" + strTag + "'";
                }

                if (strUpdate.Length > 0)
                {
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                    }
                    catch (Exception except)
                    {
                        m_bChange = false;
                        dgvTarget.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastCellTarget;
                        Global.WriteOutput("Error updating Targets:" + except.Message.ToString());
                        m_bChange = true;
                    }
                    return;
                }
            }
            else
            {

                String strAttribute = "";
                String strYears = "";
                String strTargetMean = "";
                String strCriteria = "";
                String strTargetName = "";


                DataGridViewRow row = dgvTarget.Rows[nRow];
                String strInsert = ""; ;
                if (row.Cells[nCol].Value != null)
                {
                    if (nCol == 0)
                    {
                        strAttribute = row.Cells[nCol].Value.ToString();
						strInsert = "INSERT INTO TARGETS (SIMULATIONID,ATTRIBUTE_,CRITERIA) VALUES ('" + m_strSimulationID + "','" + strAttribute + "','" + strCriteria + "')";
                    }

                    if (nCol == 1)
                    {
                        strTargetName = row.Cells[nCol].Value.ToString();
						strInsert = "INSERT INTO TARGETS (SIMULATIONID,ATTRIBUTE_,TARGETNAME) VALUES ('" + m_strSimulationID + "','" + strAttribute + "','" + strTargetName + "')";
                    }


                    else if (nCol == 2)
                    {
                        strYears = row.Cells[nCol].Value.ToString();
                        strInsert = "INSERT INTO TARGETS (SIMULATIONID,ATTRIBUTE_,YEARS,CRITERIA) VALUES ('" + m_strSimulationID + "','" + strAttribute + "','" + strYears + "','" + strCriteria + "')";
                    }
                    else if (nCol == 3)
                    {
                        strTargetMean = row.Cells[nCol].Value.ToString();
                        strInsert = "INSERT INTO TARGETS (SIMULATIONID,TARGETMEAN,CRITERIA) VALUES ('" + m_strSimulationID + "','" + strTargetMean + "','" + strCriteria + "')";
                    }
                    else if (nCol == 4)
                    {
                        strCriteria = row.Cells[nCol].Value.ToString();
                        strInsert = "INSERT INTO TARGETS (SIMULATIONID,CRITERIA) VALUES ('" + m_strSimulationID + "','" + strCriteria + "')";
                    }
                }
                

                //If new row, always insert this.  Even if all blank
                if (strInsert.Length > 0)
                {
                    try
                    {
                        DBMgr.ExecuteNonQuery(strInsert);
						String strIdentity = "";
						switch (DBMgr.NativeConnectionParameters.Provider)
						{
							case "MSSQL":
								strIdentity = "SELECT IDENT_CURRENT ('TARGETS') FROM TARGETS";
								break;
							case "ORACLE":
								//strIdentity = "SELECT TARGETS_ID_SEQ.CURRVAL FROM DUAL";
								//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'TARGETS_ID_SEQ'";
								strIdentity = "SELECT MAX(ID_) FROM TARGETS";
								break;
							default:
								throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
								//break;
						}
						DataSet ds = DBMgr.ExecuteQuery( strIdentity );
                        strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        dgvTarget.Rows[e.RowIndex].Tag = strIdentity;
                    }
                    catch (Exception except)
                    {
                        m_bChange = false;
                        dgvTarget.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastCellTarget;
                        Global.WriteOutput("Error inserting new Target:" + except.Message.ToString());
                        m_bChange = true;
                        return;
                    }
                }
            }
        }

        private void dgvTarget_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (e.Row.Tag != null)
            {
                String strDelete = "DELETE FROM TARGETS WHERE ID_='" + e.Row.Tag.ToString() + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strDelete);
                }
                catch (Exception except)
                {
                    Global.WriteOutput("Error Deleting TARGETS:" + except.Message.ToString());
                }
            }
        }

        private void dgvDeficient_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (e.Row.Tag != null)
            {
                String strDelete = "DELETE FROM DEFICIENTS WHERE ID_='" + e.Row.Tag.ToString() + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strDelete);
                }
                catch (Exception except)
                {
                    Global.WriteOutput("Error Deleting DEFICIENTS:" + except.Message.ToString());
                }
            }
        }

        private void dgvDeficient_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (!m_bChange) return;
            int nRow = e.RowIndex;
            int nCol = e.ColumnIndex;

            if (dgvDeficient.Rows[nRow].Tag != null)//Update
            {
                DataGridViewRow row = dgvDeficient.Rows[nRow];
                String strValue = "";
                if (row.Cells[nCol].Value != null)
                {
                    strValue = row.Cells[nCol].Value.ToString();
                }
                String strTag = row.Tag.ToString();
                String strUpdate = "";

                if (nCol == 0)
                {
					strUpdate = "UPDATE DEFICIENTS SET ATTRIBUTE_='" + strValue + "' WHERE ID_='" + strTag + "'";
                }

                
                else if (nCol == 1)
                {
					strUpdate = "UPDATE DEFICIENTS SET DEFICIENTNAME='" + strValue.Replace("'", "|") + "' WHERE ID_='" + strTag + "'";
                }
                else if (nCol == 2)
                {
                    if (strValue.Length == 0)
						strUpdate = "UPDATE DEFICIENTS SET DEFICIENT=NULL WHERE ID_='" + strTag + "'";
                    else
						strUpdate = "UPDATE DEFICIENTS SET DEFICIENT='" + strValue + "' WHERE ID_='" + strTag + "'";
                }
                else if (nCol == 3)
                {
                    if (strValue.Length == 0)
						strUpdate = "UPDATE DEFICIENTS SET PERCENTDEFICIENT=NULL WHERE ID_='" + strTag + "'";
                    else
						strUpdate = "UPDATE DEFICIENTS SET PERCENTDEFICIENT='" + strValue + "' WHERE ID_='" + strTag + "'";
                }
                else if (nCol == 4)
                {
                    strUpdate = "UPDATE DEFICIENTS SET CRITERIA='" + strValue.Replace("'", "|") + "' WHERE ID_='" + strTag + "'";
                }

                if (strUpdate.Length > 0)
                {
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                    }
                    catch (Exception except)
                    {
                        m_bChange = false;
                        dgvDeficient.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastCellDeficient;
                        Global.WriteOutput("Error updating Deficient:" + except.Message.ToString());
                        m_bChange = true;
                    }
                    return;
                }
            }
            else
            {

                String strAttribute = "";
                String strDeficient = "";
                String strTargetDeficient = "";
                String strCriteria = "";
                String strDeficientName = "";

                DataGridViewRow row = dgvDeficient.Rows[nRow];
                String strInsert = ""; ;
                if (row.Cells[nCol].Value != null)
                {
                    if (nCol == 0)
                    {
                        strAttribute = row.Cells[nCol].Value.ToString();
                        strInsert = "INSERT INTO DEFICIENTS (SIMULATIONID,ATTRIBUTE_) VALUES ('" + m_strSimulationID + "','" + strAttribute + "')";
                    }
                    else if (nCol == 1)
                    {
                        strDeficientName = row.Cells[nCol].Value.ToString();
                        strInsert = "INSERT INTO DEFICIENTS (SIMULATIONID,ATTRIBUTE_,DEFICIENTNAME,CRITERIA) VALUES ('" + m_strSimulationID + "','" + strAttribute + "','" + strDeficientName + "','" + strCriteria + "')";
                    }

                    else if (nCol == 2)
                    {
                        strDeficient = row.Cells[nCol].Value.ToString();
                        strInsert = "INSERT INTO DEFICIENTS (SIMULATIONID,DEFICIENT) VALUES ('" + m_strSimulationID + "','" + strDeficient + "')";
                    }
                    else if (nCol == 3)
                    {
                        strTargetDeficient = row.Cells[nCol].Value.ToString();
                        strInsert = "INSERT INTO DEFICIENTS (SIMULATIONID,PERCENTDEFICIENT) VALUES ('" + m_strSimulationID + "','" + strTargetDeficient + "')";
                    }
                    else if (nCol == 4)
                    {
                        strCriteria = row.Cells[nCol].Value.ToString().Replace("'","|");
                        strInsert = "INSERT INTO DEFICIENTS (SIMULATIONID,CRITERIA) VALUES ('" + m_strSimulationID + "','" + strCriteria + "')";
                    }
                }


                //If new row, always insert this.  Even if all blank
                if (strInsert.Length > 0)
                {
                    try
                    {
                        DBMgr.ExecuteNonQuery(strInsert);
						String strIdentity = "";
						switch (DBMgr.NativeConnectionParameters.Provider)
						{
							case "MSSQL":
								strIdentity = "SELECT IDENT_CURRENT ('DEFICIENTS') FROM DEFICIENTS";
								break;
							case "ORACLE":
								//strIdentity = "SELECT DEFICIENTS_ID_SEQ.CURRVAL FROM DUAL";
								//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'DEFICIENTS_ID_SEQ'";
								strIdentity = "SELECT MAX(ID_) FROM DEFICIENTS";
								break;
							default:
								throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
								//break;
						}
						DataSet ds = DBMgr.ExecuteQuery( strIdentity );
                        strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        dgvDeficient.Rows[e.RowIndex].Tag = strIdentity;
                    }
                    catch (Exception except)
                    {
                        m_bChange = false;
                        dgvDeficient.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastCellDeficient;
                        Global.WriteOutput("Error inserting new Deficient:" + except.Message.ToString());
                        m_bChange = true;
                        return;
                    }
                }
            }
        }

        private void FormAnalysis_FormClosed(object sender, FormClosedEventArgs e)
        {
			FormUnload();
            FormManager.RemoveFormAnalysis(this);
        }

        private void dgvPriority_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            m_oLastCellPriority = dgvPriority.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        }

        private void dgvTarget_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            m_oLastCellTarget = dgvTarget.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        }

        private void dgvDeficient_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            m_oLastCellDeficient = dgvDeficient.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        }

        private void tbBenefitLimit_Enter(object sender, EventArgs e)
        {
            m_strLastLimit = tbBenefitLimit.Text;
        }

        private void dgvPriority_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
			if( e.ColumnIndex == 2 )
			{
                if (e.RowIndex == -1) return;
				if( Global.SecurityOperations.CanModifySimulationAnalysisData( m_strNetworkID, m_strSimulationID ) )
				{
					String strCriteria = "";
					if( dgvPriority.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null )
					{
						strCriteria = dgvPriority.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
					}
					FormAdvancedSearch formAdvancedSearch = new FormAdvancedSearch( m_strNetwork, m_hashAttributeYear, strCriteria, true );
					if( formAdvancedSearch.ShowDialog() == DialogResult.OK )
					{
						dgvPriority[e.ColumnIndex, e.RowIndex].Value = formAdvancedSearch.GetWhereClause();
					}

					dgvPriority.Update();
				}
				else
				{
					Global.WriteOutput( "Error: insufficient permissions to alter simulation data." );
				}
			}
        }

        private void dgvTarget_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
			if( e.ColumnIndex == 4 )
			{
				if( Global.SecurityOperations.CanModifySimulationAnalysisData( m_strNetworkID, m_strSimulationID ) )
				{
					if( dgvTarget[0, e.RowIndex].Value == null )
					{
						Global.WriteOutput( "An attribute must be selected before editing target criteria" );
						return;
					}
					String strCriteria = "";
					if( dgvTarget.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null )
					{
						strCriteria = dgvTarget.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
					}
					FormAdvancedSearch formAdvancedSearch = new FormAdvancedSearch( m_strNetwork, m_hashAttributeYear, strCriteria, true );
					if( formAdvancedSearch.ShowDialog() == DialogResult.OK )
					{
						dgvTarget[e.ColumnIndex, e.RowIndex].Value = formAdvancedSearch.GetWhereClause();
					}

                    dgvTarget.Update();
				}
				else
				{
					Global.WriteOutput( "Error: insufficient permissions to alter simulation data." );
				}
			}
        }

        private void dgvDeficient_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
			if( e.ColumnIndex == 4 )
			{
				if( Global.SecurityOperations.CanModifySimulationAnalysisData( m_strNetworkID, m_strSimulationID ) )
				{
					if( dgvDeficient[0, e.RowIndex].Value == null )
					{
						Global.WriteOutput( "A attribute must be selected before editing deficiency criteria" );
						return;


					}
					String strCriteria = "";
					if( dgvDeficient.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null )
					{
						strCriteria = dgvDeficient.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
					}
					FormAdvancedSearch formAdvancedSearch = new FormAdvancedSearch( m_strNetwork, m_hashAttributeYear, strCriteria, true );
					if( formAdvancedSearch.ShowDialog() == DialogResult.OK )
					{
						dgvDeficient[e.ColumnIndex, e.RowIndex].Value = formAdvancedSearch.GetWhereClause();
					}

					dgvDeficient.Update();
				}
				else
				{
					Global.WriteOutput( "Error: insufficient permissions to alter simulation data." );
				}
			}
        }

        private void tbJurisdiction_TextChanged(object sender, EventArgs e)
        {

        }

		private void dgvDeficient_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{

		}

        private void checkBoxMultipleCost_CheckedChanged(object sender, EventArgs e)
        {
            var update = "";
            if (checkBoxMultipleCost.Checked)
            {
                update = "UPDATE SIMULATIONS SET USE_CUMULATIVE_COST='1' WHERE SIMULATIONID='" + m_strSimulationID + "'";
            }
            else
            {
                update = "UPDATE SIMULATIONS SET USE_CUMULATIVE_COST='0' WHERE SIMULATIONID='" + m_strSimulationID + "'";
            }
            

            try
            {
                DBMgr.ExecuteNonQuery(update);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error updating Apply multiple feasible costs: " + exception.Message.ToString());
                return;
            }

        }

        private void TabPagePriority_Click(object sender, EventArgs e)
        {

        }

        private void RadioButtonWithinBudget_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void RadioButtonAcrossBudget_CheckedChanged(object sender, EventArgs e)
        {
            var update = "";
            if (radioButtonAcrossBudget.Checked)
            {
                update = "UPDATE SIMULATIONS SET USE_ACROSS_BUDGET='1' WHERE SIMULATIONID='" + m_strSimulationID + "'";
            }
            else
            {
                update = "UPDATE SIMULATIONS SET USE_ACROSS_BUDGET='0' WHERE SIMULATIONID='" + m_strSimulationID + "'";
            }
            try
            {
                DBMgr.ExecuteNonQuery(update);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error updating apply budget: " + exception.Message.ToString());
                return;
            }
        }

        private void DataGridViewRemainLife_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                var strValue = "";
                if (dataGridViewRemainLife.Rows[e.RowIndex].Tag == null)
                {
                    if (dataGridViewRemainLife.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        strValue = dataGridViewRemainLife.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    }
                    else
                    {
                        return;
                    }


                    var strInsert = "";
                    switch (e.ColumnIndex)
                    {
                        case 0:
                            strInsert = "INSERT INTO REMAINING_LIFE_LIMITS (SIMULATION_ID, ATTRIBUTE_,REMAINING_LIFE_LIMIT) VALUES ('" +
                                        m_strSimulationID + "','" + strValue + "','0')";
                            break;
                        default:
                            MessageBox.Show("Select attribute before entering a limit or criteria.");
                            return;
                    }



                    try
                    {
                        String strIdentity;
                        DBMgr.ExecuteNonQuery(strInsert);
                        switch (DBMgr.NativeConnectionParameters.Provider)
                        {
                            case "MSSQL":
                                strIdentity = "SELECT IDENT_CURRENT ('REMAINING_LIFE_LIMITS') FROM REMAINING_LIFE_LIMITS";
                                break;
                            case "ORACLE":
                                strIdentity = "SELECT MAX(REMAINING_LIFE_ID) FROM REMAINING_LIFE_LIMITS";
                                break;
                            default:
                                throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                                //break;
                        }

                        var ds = DBMgr.ExecuteQuery(strIdentity);
                        dataGridViewRemainLife.Rows[e.RowIndex].Tag = ds.Tables[0].Rows[0].ItemArray[0].ToString();

                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error:" + except.Message.ToString());
                    }
                }
                else
                {
                    String strTag = dataGridViewRemainLife.Rows[e.RowIndex].Tag.ToString();
                    String strUpdate = "";


                    if (dataGridViewRemainLife.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        strValue = dataGridViewRemainLife.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    }
                    else
                    {
                        return;
                    }

                    switch (e.ColumnIndex)
                    {
                        case 0:

                            strUpdate = "UPDATE REMAINING_LIFE_LIMITS SET ATTRIBUTE_='" + strValue + "' WHERE REMAINING_LIFE_ID='" + strTag + "'";
                            break;
                        case 1:
                            try
                            {
                                strValue = Convert.ToSingle(strValue).ToString();

                            }
                            catch (Exception exception)
                            {
                                MessageBox.Show("Remaining Life Limit must be a number. " + exception.Message);
                                return;
                            }

                            strUpdate = "UPDATE REMAINING_LIFE_LIMITS SET REMAINING_LIFE_LIMIT ='" + strValue + "' WHERE REMAINING_LIFE_ID='" + strTag + "'";
                            break;

                        case 2:
                            strValue = strValue.Trim();
                            strValue = strValue.Replace("'", "|");
                            strUpdate = "UPDATE REMAINING_LIFE_LIMITS SET CRITERIA ='" + strValue + "' WHERE REMAINING_LIFE_ID='" + strTag + "'";
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

        private void DataGridViewRemainLife_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (e.Row.Tag != null)
            {
                try
                {
                    DBMgr.ExecuteNonQuery("DELETE FROM REMAINING_LIFE_LIMITS WHERE REMAINING_LIFE_ID='" + e.Row.Tag + "'");
                }
                catch (Exception except)
                {
                    Global.WriteOutput("Error Deleting Remaining Life Limits:" + except.Message.ToString());
                }
            }
        }


        private void DataGridViewRemainLife_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.RowIndex != -1)
            {
                if (Global.SecurityOperations.CanModifySimulationTreatment(m_strNetworkID, m_strSimulationID))
                {
                    String strCriteria = "";
                    if (dataGridViewRemainLife.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        strCriteria = dataGridViewRemainLife.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    }
                    FormAdvancedSearch formAdvancedSearch = new FormAdvancedSearch(m_strNetwork, m_hashAttributeYear, strCriteria, true);
                    if (formAdvancedSearch.ShowDialog() == DialogResult.OK)
                    {
                        dataGridViewRemainLife[e.ColumnIndex, e.RowIndex].Value = formAdvancedSearch.GetWhereClause();
                        try
                        {
                            string strID = dataGridViewRemainLife.Rows[e.RowIndex].Tag.ToString();
                        }
                        catch (Exception except)
                        {
                            Global.WriteOutput("Error: Updating Consequence Criteria." + except.Message);
                        }

                    }
                    dataGridViewRemainLife.Update();
                }
            }
        }

        private void CheckBoxReasons_CheckedChanged(object sender, EventArgs e)
        {
            var update = "";
            if (checkBoxReasons.Checked)
            {
                update = "UPDATE SIMULATIONS SET USE_REASONS='1' WHERE SIMULATIONID='" + m_strSimulationID + "'";
            }
            else
            {
                update = "UPDATE SIMULATIONS SET USE_REASONS='0' WHERE SIMULATIONID='" + m_strSimulationID + "'";
            }
            try
            {
                DBMgr.ExecuteNonQuery(update);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error output reason data to database: " + exception.Message.ToString());
                return;
            }
        }
    }
}
