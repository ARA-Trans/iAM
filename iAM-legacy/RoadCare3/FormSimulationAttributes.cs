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
using System.Data.OleDb;


namespace RoadCare3
{
    public partial class FormSimulationAttributes : ToolWindow
    {
        private List<CommitAttributeChange> m_listAttributes = new List<CommitAttributeChange>();
        private bool m_bCommitted;
        private bool m_bChange;
        private String m_strSimulationID;
        private String m_strNetworkID;
        private SimulationAttributes m_SimulationAttribute;
        private Object m_oLastCell;
        private Object m_oLastAttributeCell;
        private List<String> m_listAttributeSimulation;
        private List<String> m_listBudgets;
        private List<String> m_listTreatments;
        private String m_strSectionID;
        private String m_strYear;
        private bool m_bTreatments=false;
        private List<String> m_listFeasibleTreatment;
        private bool _isRSL = false;

        public String Year
        {
            get { return m_strYear; }
            set { m_strYear = value; }
        }

        public String SectionID
        {
            get { return m_strSectionID; }
            set { m_strSectionID = value; }
        }

        public List<CommitAttributeChange> Attributes
        {
            get { return m_listAttributes; }
            set { m_listAttributes = value; }
        }


        public bool IsCommitted
        {
            get { return m_bCommitted; }
            set { m_bCommitted = value; }
        }

        public String SimulationID
        {
            get { return m_strSimulationID; }
            set { m_strSimulationID = value; }
        }

        public String NetworkID
        {
            get { return m_strNetworkID; }
            set { m_strNetworkID = value; }
        }


        public FormSimulationAttributes()
        {
            InitializeComponent();
        }

        public FormSimulationAttributes(bool bTreatment,String strID, String strYear)
        {
            InitializeComponent();
            m_bTreatments = true;
            this.Text = "Run/Commit Feasible Treatments";
            buttonCancel.Visible = true;
            buttonRun.Visible = true;
            buttonCommit.Visible = true;
            this.Year = strYear;
            this.SectionID = strID;
        }

        private void FormSimulationAttributes_Load(object sender, EventArgs e)
        {
			SecureForm();
			LoadFormSimulationAttributes();

            if (m_bTreatments)
            {
                LoadFeasibleTreatment();
            }
		}

		protected void SecureForm()
		{
			LockDataGridView( dgvAttribute );
			LockDataGridView( dgvSummary );

			if( Global.SecurityOperations.CanModifySimulationAttributesData( m_strNetworkID, m_strSimulationID ) )
			{
				UnlockDataGridViewForCreateDestroy( dgvAttribute );
				UnlockDataGridViewForCreateDestroy( dgvSummary );
			}
		}
        private void LoadFeasibleTreatment()
        {
            m_bChange = false;
            String strBenefit = "BENEFITCOST_" + m_strNetworkID + "_" + m_strSimulationID;
            String strSection = "SECTION_" + m_strNetworkID;
            m_listFeasibleTreatment = new List<String>();
            String strSelect = "SELECT TREATMENT FROM " + strBenefit + " INNER JOIN " + strSection + " ON " + strSection + ".SECTIONID=" + strBenefit + ".SECTIONID WHERE " + strBenefit + ".SECTIONID=" + this.SectionID + " AND YEARS=" + this.Year;
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    m_listFeasibleTreatment.Add(dr["TREATMENT"].ToString());
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Retrieving feasibility information for tooltip." + exception.Message);
            }



            if (dgvAttribute == null) return;

            dgvAttribute.Columns.Clear();
            DataGridViewComboBoxColumn comboboxColumn;
            comboboxColumn = CreateComboBoxColumn();
            comboboxColumn.HeaderText = "Attribute";
            Global.LoadAttributes();
            foreach (String str in Global.Attributes)
            {
                comboboxColumn.Items.Add(str);
            }
            if(!comboboxColumn.Items.Contains("LENGTH"))
            {
                comboboxColumn.Items.Add("LENGTH");
            }

            if (!comboboxColumn.Items.Contains("AREA"))
            {
                comboboxColumn.Items.Add("AREA");
            }

            dgvAttribute.Columns.Insert(0, comboboxColumn);
            

            dgvAttribute.RowHeadersVisible = true;
            dgvAttribute.AllowUserToDeleteRows = true;
            dgvAttribute.ColumnCount = 2;
            dgvAttribute.Columns[1].Name = "Change";
            dgvAttribute.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvAttribute.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            foreach (CommitAttributeChange commitAttributeChange in Attributes)
            {
                int nRow = dgvAttribute.Rows.Add(commitAttributeChange.m_strAttribute, commitAttributeChange.m_strChange);
                dgvAttribute.Rows[nRow].Tag = commitAttributeChange.m_strID;
            }

            DataGridViewRow dataGridRow = new DataGridViewRow();
            DataGridViewCell[] cells = new DataGridViewCell[2];
            DataGridViewTextBoxCell txtCell = new DataGridViewTextBoxCell();
            DataGridViewComboBoxCell cbShowAll = new DataGridViewComboBoxCell();
            txtCell.Value = "Show Only Feasible";
            dataGridRow.Cells.Add(txtCell);
            txtCell.ReadOnly = true;
            dataGridRow.Cells.Add(cbShowAll);
            cbShowAll.Items.Add("True");
            cbShowAll.Items.Add("False");
            cbShowAll.Value = "True";
            dgvSummary.Rows.Add(dataGridRow);


            dataGridRow = new DataGridViewRow();
            cells = new DataGridViewCell[2];
            txtCell = new DataGridViewTextBoxCell();
            DataGridViewComboBoxCell cbTreatment = new DataGridViewComboBoxCell();

            txtCell.Value = "Treatment";
            dataGridRow.Cells.Add(txtCell);
            txtCell.ReadOnly = true;
            dataGridRow.Cells.Add(cbTreatment);
            cbTreatment.Items.Add("");
            cbTreatment.Value = "";
            cbTreatment.Items.Add("No Treatment");
            foreach (String str in m_listFeasibleTreatment)
            {
                cbTreatment.Items.Add(str);
            }
            dgvSummary.Rows.Add(dataGridRow);




            // ------------------------------------------------
            // Add a combobox cell and add the combobox items            
            dataGridRow = new DataGridViewRow();
            cells = new DataGridViewCell[2];
            txtCell = new DataGridViewTextBoxCell();
            DataGridViewComboBoxCell cbBudget = new DataGridViewComboBoxCell();

            foreach (String str in m_listBudgets)
            {
                cbBudget.Items.Add(str);
            }

            txtCell.Value = "Budget";
            dataGridRow.Cells.Add(txtCell);
            txtCell.ReadOnly = true;
            dataGridRow.Cells.Add(cbBudget);
//            cbBudget.Value = sa.m_strBudget;
            dgvSummary.Rows.Add(dataGridRow);

            float fCost = 0;
//            float.TryParse(sa.m_strCost, out fCost);
            int nCostRow = dgvSummary.Rows.Add("Cost", fCost);
            dgvSummary.Rows[nCostRow].Cells[1].Style.Format = "c";
            
            dgvSummary.Rows.Add("Years Before Any", "");
            dgvSummary.Rows.Add("Years Before Same", "");

            m_bChange = true;
        }

		private void LoadFormSimulationAttributes()
		{
			this.TabText = "Simulation Property";

			String strSelect = "SELECT SIMULATION_VARIABLES,ANALYSIS FROM SIMULATIONS WHERE SIMULATIONID='" + SimulationID + "'";
			try
			{
				DataSet ds = DBMgr.ExecuteQuery(strSelect);
				m_listAttributeSimulation = new List<String>();
				if (ds.Tables[0].Rows.Count == 1)
				{
					string[] attributes = ds.Tables[0].Rows[0].ItemArray[0].ToString().Split(new string[] { "\t" }, StringSplitOptions.None);
					for (int i = 0; i < attributes.Length; i++)
					{
						m_listAttributeSimulation.Add(attributes[i]);
					}

                    if( ds.Tables[0].Rows[0].ItemArray[1].ToString() == "Conditional RSL/Cost")
                    {
                        _isRSL = true;
                    }
				}
			}
			catch (Exception exception)
			{

				Global.WriteOutput("Error retrieving simulation attributes: " + exception.Message);

			}

            




			strSelect = "SELECT BUDGETORDER FROM INVESTMENTS WHERE SIMULATIONID='" + m_strSimulationID + "'";
			try
			{

				DataSet ds = DBMgr.ExecuteQuery(strSelect);
				m_listBudgets = new List<String>();
				if (ds.Tables[0].Rows.Count == 1)
				{
					string[] budgets = ds.Tables[0].Rows[0].ItemArray[0].ToString().Split(',');
					foreach (string str in budgets)
					{
						m_listBudgets.Add(str);
					}
				}
			}
			catch (Exception exception)
			{
				Global.WriteOutput("Error in initializing investments." + exception.Message);

			}


			strSelect = "SELECT TREATMENT FROM TREATMENTS WHERE SIMULATIONID='" + m_strSimulationID + "'";
			try
			{

				DataSet ds = DBMgr.ExecuteQuery(strSelect);
				m_listTreatments = new List<String>();
				foreach (DataRow row in ds.Tables[0].Rows)
				{
					m_listTreatments.Add(row[0].ToString());
				}
			}
			catch (Exception exception)
			{
				Global.WriteOutput("Error in initializing investments." + exception.Message);

			}
		}

        public void InitializeAttributes()
        {
			LoadFormSimulationAttributes();
            if (dgvAttribute == null) return;

            dgvAttribute.Columns.Clear();
            DataGridViewComboBoxColumn comboboxColumn;
            comboboxColumn = CreateComboBoxColumn();
            comboboxColumn.HeaderText = "Attribute";
            Global.LoadAttributes();
            foreach (String str in Global.Attributes)
            {
                comboboxColumn.Items.Add(str);
            }

            dgvAttribute.Columns.Insert(0, comboboxColumn);
            

            if (IsCommitted)
            {
                dgvAttribute.RowHeadersVisible = true;
                dgvAttribute.AllowUserToDeleteRows = true;
                dgvAttribute.ColumnCount = 2;
                dgvAttribute.Columns[1].Name = "Change";
                dgvAttribute.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvAttribute.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                foreach (CommitAttributeChange commitAttributeChange in Attributes)
                {
                    int nRow = dgvAttribute.Rows.Add(commitAttributeChange.m_strAttribute, commitAttributeChange.m_strChange);
                    dgvAttribute.Rows[nRow].Tag = commitAttributeChange.m_strID;
                }
 
            
            }
            else
            {
                dgvAttribute.AllowUserToDeleteRows = false;
                dgvAttribute.RowHeadersVisible = false;
                dgvAttribute.ColumnCount = 4;
                dgvAttribute.Columns[1].Name = "Change";
                dgvAttribute.Columns[2].Name = "Value";
                dgvAttribute.Columns[3].Name = "Remain\nLife";
//                dgvAttribute.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvAttribute.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvAttribute.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvAttribute.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvAttribute.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }


        }

        public void InitializeSummary(SimulationAttributes sa)
        {
            if (dgvSummary == null) return;
            m_SimulationAttribute = sa;
            m_bChange = false;
            dgvSummary.Rows.Clear();
            dgvSummary.Tag = "";
            int nRow;

            if (sa.m_strCommitID != null)
            {
                dgvSummary.Tag = sa.m_strCommitID;

            }

            nRow = dgvSummary.Rows.Add("Facility", sa.m_strFacility);
            dgvSummary.Rows[nRow].ReadOnly = true;
            dgvSummary.Rows[nRow].Tag = sa.m_strSectionID;

            nRow = dgvSummary.Rows.Add("Section", sa.m_strSection);
            dgvSummary.Rows[nRow].ReadOnly = true;

            if (sa.m_strYear != null)
            {
                nRow = dgvSummary.Rows.Add("Year", sa.m_strYear);
                dgvSummary.Rows[nRow].ReadOnly = true;
            }

            if (sa.m_strTreatment != null)
            {
                DataGridViewRow dataGridRow = new DataGridViewRow();
                DataGridViewCell[] cells = new DataGridViewCell[2];
                DataGridViewTextBoxCell txtCell = new DataGridViewTextBoxCell();
                DataGridViewComboBoxCell cbTreatment = new DataGridViewComboBoxCell();

                txtCell.Value = "Treatment";
                dataGridRow.Cells.Add(txtCell);
                txtCell.ReadOnly = true;
                dataGridRow.Cells.Add(cbTreatment);
                cbTreatment.Items.Add(sa.m_strTreatment);
                cbTreatment.Value = sa.m_strTreatment;
                foreach (String str in m_listTreatments)
                {
                    cbTreatment.Items.Add(str);
                }
                dgvSummary.Rows.Add(dataGridRow);           



            }
            if (sa.m_strBudget != null)
            {
                // ------------------------------------------------
                // Add a combobox cell and add the combobox items            
                DataGridViewRow dataGridRow = new DataGridViewRow();
                DataGridViewCell[] cells = new DataGridViewCell[2];
                DataGridViewTextBoxCell txtCell = new DataGridViewTextBoxCell();
                DataGridViewComboBoxCell cbBudget = new DataGridViewComboBoxCell();

                foreach (String str in m_listBudgets)
                {
                    cbBudget.Items.Add(str);
                }
                
                txtCell.Value = "Budget";
                dataGridRow.Cells.Add(txtCell);
                txtCell.ReadOnly = true;
                dataGridRow.Cells.Add(cbBudget);
                cbBudget.Value = sa.m_strBudget;
                dgvSummary.Rows.Add(dataGridRow);           
            }

            if (sa.m_strCost != null)
            {
                float fCost = 0;
                float.TryParse(sa.m_strCost, out fCost);
                int nCostRow = dgvSummary.Rows.Add("Cost", fCost);
                dgvSummary.Rows[nCostRow].Cells[1].Style.Format = "c";
            }
            if (sa.m_strBeforeAny != null) dgvSummary.Rows.Add("Years Before Any", sa.m_strBeforeAny);
            if (sa.m_strBeforeSame != null) dgvSummary.Rows.Add("Years Before Same", sa.m_strBeforeSame);
            UpdateGridView();
            m_bChange = true;
        }

        private DataGridViewComboBoxColumn CreateComboBoxColumn()
        {
            DataGridViewComboBoxColumn column =
                new DataGridViewComboBoxColumn();
            {
                //column.DataPropertyName = ColumnName.TitleOfCourtesy.ToString();
                //column.HeaderText = ColumnName.TitleOfCourtesy.ToString();
                column.DropDownWidth = 160;
 //               column.Width = 90;
 //               column.MaxDropDownItems = 3;
                column.FlatStyle = FlatStyle.Flat;
            }
            return column;
        }


        public void AddAttributeChange(CommitAttributeChange commitAttributeChange)
        {
            m_listAttributes.Add(commitAttributeChange);
        }

        public void UpdateGridView()
        {
            if (dgvAttribute == null) return;
            dgvAttribute.Rows.Clear();

            foreach (CommitAttributeChange commitAttributeChange in m_listAttributes)
            {
                int nRow = dgvAttribute.Rows.Add(commitAttributeChange.m_strAttribute,commitAttributeChange.m_strChange);
                dgvAttribute.Rows[nRow].Tag = commitAttributeChange.m_strID;

            }
            if (dgvAttribute.Columns.Count == 4)
            {
                if (_isRSL)
                {
                    dgvAttribute.Columns[3].HeaderText = "Life Extension";
                }
                else
                {
                    dgvAttribute.Columns[3].HeaderText = "Remaining_Life";
                }
            }


            dgvAttribute.Update();
        }

        private void FormSimulationAttributes_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!m_bTreatments)
            {
                FormManager.RemoveSimulationAttributeWindow();
            }
        }

        private void dgvSummary_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_bChange) return;
            int nCol = e.ColumnIndex;
            int nRow = e.RowIndex;
            CommittedProjectSummaryChange(nCol,nRow);
        }

        private void CommittedProjectSummaryChange(int nCol, int nRow)
        {
            String strTreatment = "";

            if (nCol == 1)
            {
                String strCommitID = dgvSummary.Tag.ToString();
                if (strCommitID.Length > 0)
                {
                    if (dgvSummary.Rows[nRow].Cells[nCol].Value == null) return;

                    String strValue = dgvSummary.Rows[nRow].Cells[nCol].Value.ToString();
                    String strUpdate = "";
                    //UPDATE
                    switch (nRow)
                    {

                        case 3://Treatment
                            if (m_listTreatments.Contains(strValue)) strTreatment = strValue;
                            strUpdate = "UPDATE COMMITTED_ SET TREATMENTNAME='" + strValue + "' WHERE COMMITID='" + strCommitID + "'";
                            m_SimulationAttribute.m_formResultCommit.GridViewCommitResult.Rows[m_SimulationAttribute.m_nRow].Cells[m_SimulationAttribute.m_nCol].Value = strValue;
                            break;
                        case 4://Budget
                            strUpdate = "UPDATE COMMITTED_ SET BUDGET='" + strValue + "' WHERE COMMITID='" + strCommitID + "'";
                            break;

                        case 5://Cost
                            strValue = strValue.Replace(",", "");
                            strValue = strValue.Replace("$", "");
                            strUpdate = "UPDATE COMMITTED_ SET COST_='" + strValue + "' WHERE COMMITID='" + strCommitID + "'";
                            break;

                        case 6://Year Same
                            strUpdate = "UPDATE COMMITTED_ SET YEARANY='" + strValue + "' WHERE COMMITID='" + strCommitID + "'";
                            break;

                        case 7://Year Any
                            strUpdate = "UPDATE COMMITTED_ SET YEARSAME='" + strValue + "' WHERE COMMITID='" + strCommitID + "'";
                            break;

                    }
                    if (strUpdate.Length > 0)
                    {
                        try
                        {
                            DBMgr.ExecuteNonQuery(strUpdate);
                        }
                        catch (Exception exception)
                        {
                            m_bChange = false;
                            dgvSummary.Rows[nRow].Cells[nCol].Value = m_oLastCell;
                            Global.WriteOutput("Error updating simualtion variables. " + exception.Message);
                            m_bChange = true;
                        }
                    }
                }
                else
                {
                    if (nRow > 1)
                    {
                        String strSectionID = m_SimulationAttribute.m_strSectionID;
                        String strYear = m_SimulationAttribute.m_strYear;
                        String strYearSame = "1";
                        String strYearAny = "1";
                        String strBudget = "";
                        String strTreatmentName = "";
                        String strCost = "0";

                        switch (nRow)
                        {
                            case 3://Treatment Name
                                if (dgvSummary.Rows[nRow].Cells[1].Value == null) return;
                                strTreatmentName = dgvSummary.Rows[nRow].Cells[1].Value.ToString();
                                if (m_listTreatments.Contains(strTreatmentName)) strTreatment = strTreatmentName;
                                break;
                            case 4://Budget
                                strBudget = dgvSummary.Rows[nRow].Cells[1].Value.ToString();
                                break;
                            case 5://Budget
                                strCost = dgvSummary.Rows[nRow].Cells[1].Value.ToString();
                                break;
                            case 6://Year Same
                                strYearAny = dgvSummary.Rows[nRow].Cells[1].Value.ToString();
                                break;
                            case 7:// 
                                strYearSame = dgvSummary.Rows[nRow].Cells[1].Value.ToString();
                                break;
                        }
                        String strInsert = "INSERT INTO COMMITTED_ (SIMULATIONID,SECTIONID,YEARS,TREATMENTNAME,YEARSAME,YEARANY,BUDGET,COST_) VALUES('" + SimulationID.ToString() + "','"
                            + strSectionID + "','"
                            + strYear + "','"
                            + strTreatmentName + "','"
                            + strYearSame + "','"
                            + strYearAny + "','"
                            + strBudget + "','"
                            + strCost + "')";

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
										strIdentity = "SELECT IDENT_CURRENT ('COMMITTED_') FROM COMMITTED_";
										break;
									case "ORACLE":
										//strIdentity = "SELECT COMMITTED_COMMITID_SEQ.CURRVAL FROM DUAL";
										//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'COMMITTED_COMMITID_SEQ'";
										strIdentity = "SELECT MAX(COMMITID) FROM COMMITTED_";
										break;
									default:
										throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
										//break;
								}

                                DataSet ds = DBMgr.ExecuteQuery(strIdentity);
                                strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                                dgvSummary.Tag = strIdentity;
                                m_SimulationAttribute.m_formResultCommit.GridViewCommitResult.Rows[m_SimulationAttribute.m_nRow].Cells[m_SimulationAttribute.m_nCol].Tag = strIdentity;
                                m_SimulationAttribute.m_formResultCommit.GridViewCommitResult.Rows[m_SimulationAttribute.m_nRow].Cells[m_SimulationAttribute.m_nCol].Value = strTreatmentName;


                            }
                            catch (Exception except)
                            {
                                Global.WriteOutput("Error inserting new Committed Project:" + except.Message.ToString());
                                dgvSummary.Rows[nRow].Cells[nCol].Value = m_oLastCell;
                                return;
                            }
                        }
                    }
                }
            }
            if (strTreatment.Length > 0)
            {
                LoadExistingTreatment(strTreatment);
            }
        }

        private void FeasibleTreatmentSummaryChange(int nCol,int nRow)
        {
            String strBenefit = "BENEFITCOST_" + m_strNetworkID + "_" + m_strSimulationID;
            String strSection = "SECTION_" + m_strNetworkID;
            if (nCol == 1)
            {
                m_bChange = false;
                if (nRow == 0)
                {
                    DataGridViewComboBoxCell cbTreatment = (DataGridViewComboBoxCell)dgvSummary.Rows[1].Cells[1];
                    cbTreatment.Items.Clear();
                    cbTreatment.Items.Add("");
                    if (dgvSummary.Rows[0].Cells[1].Value.ToString() == "False")//Load all treatments
                    {
                        foreach (String treatments in m_listTreatments)
                        {
                            cbTreatment.Items.Add(treatments);
                        }
                    }
                    else//Load only feasible
                    {
                        String strSelect = "SELECT TREATMENT FROM " + strBenefit + " INNER JOIN " + strSection + " ON " + strSection + ".SECTIONID=" + strBenefit + ".SECTIONID WHERE " + strBenefit + ".SECTIONID=" + SectionID + " AND YEARS=" + Year;
                        try
                        {
                            DataSet ds = DBMgr.ExecuteQuery(strSelect);
                            cbTreatment.Items.Add("No Treatment");
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                cbTreatment.Items.Add(dr["TREATMENT"].ToString());
                            }
                        }
                        catch (Exception exception)
                        {
                            Global.WriteOutput("Error: Retrieving feasibility information." + exception.Message);
                        }
                    }
                }
                else if(nRow == 1)
                {
                    String strSelect = "SELECT TREATMENT, BUDGET, COST_ *AREA AS TOTALCOST,YEARSANY,YEARSSAME FROM " + strBenefit + " INNER JOIN " + strSection + " ON " + strSection + ".SECTIONID=" + strBenefit + ".SECTIONID WHERE " + strBenefit + ".SECTIONID=" + SectionID + " AND YEARS=" + Year;
                    try
                    {
                        DataSet ds = DBMgr.ExecuteQuery(strSelect);
                        if(ds.Tables[0].Rows.Count == 1)
            
                        {
                            DataRow dr = ds.Tables[0].Rows[0];
                            string budget = dr["BUDGET"].ToString();
                            if(!budget.Contains("|"))dgvSummary[1, 2].Value = budget;
                            dgvSummary[1, 3].Value = dr["TOTALCOST"].ToString();
                            dgvSummary[1, 4].Value = dr["YEARSANY"].ToString();
                            dgvSummary[1, 5].Value = dr["YEARSSAME"].ToString();
                        }
                    }
                    catch (Exception exception)
                    {
                        Global.WriteOutput("Error: Retrieving feasibility information." + exception.Message);
                    }

                }
                m_bChange = true;
            }
        }


        private void LoadExistingTreatment(String strTreatment)
        {
            String strSelect = "SELECT BEFOREANY, BEFORESAME, BUDGET, TREATMENTID FROM TREATMENTS WHERE SIMULATIONID ='" + m_strSimulationID + "' AND TREATMENT='" + strTreatment + "'";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);

                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    String strAny = row[0].ToString();
                    String strSame = row[1].ToString();
                    String strBudget = row[2].ToString();
                    String strTreatmentID = row[3].ToString();

                    dgvSummary.Rows[4].Cells[1].Value = strBudget;
                    dgvSummary.Rows[6].Cells[1].Value = strAny;
                    dgvSummary.Rows[7].Cells[1].Value = strSame;

                    //Need to calculate the cost of this treatment.
					//strSelect = "SELECT COST_,UNIT FROM COSTS WHERE TREATMENTID='" + strTreatmentID + "' AND CRITERIA=''";
					//inserting '' in oracle inserts a null
					strSelect = "SELECT COST_,UNIT FROM COSTS WHERE TREATMENTID='" + strTreatmentID + "' AND (CRITERIA='' OR CRITERIA IS NULL)";
                    ds = DBMgr.ExecuteQuery(strSelect);
                    String strCost;
                    String strUnits;

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        row = ds.Tables[0].Rows[0];
                        strCost = row[0].ToString();
                        strUnits = row[1].ToString();
                        String strOutArea;
                        //String strOutUnit;

                        if (!GetSectionArea(SectionID, out strOutArea))
                        {
                            strOutArea = "1";
                            Global.WriteOutput("Error: Calculating error.  Default area of 1");
                        }

                        float fCost = 0;
                        float.TryParse(strCost, out fCost);
                        float fOutCost = 0;
                        Global.GetCost(fCost, "1", strOutArea,  out fOutCost);
                        dgvSummary.Rows[5].Cells[1].Value = fOutCost;
                        dgvSummary.Rows[5].Cells[1].Style.Format = "c";

                    }
                    else
                    {
                        dgvSummary.Rows[5].Cells[1].Value = "0";
                    }

                    //Delete any existing committIDs
                    foreach(DataGridViewRow dgvRow in dgvAttribute.Rows)
                    {
                        if (dgvRow.Tag != null)
                        {
                            DeleteAttributeRow(dgvRow.Tag.ToString());
                        }
                    }
                    dgvAttribute.Rows.Clear();


                    //Load new Consequences.  This is all consequences for this treatment ID with blank Criteria.

                    //strSelect = "SELECT ATTRIBUTE_,CHANGE_ FROM CONSEQUENCES WHERE TREATMENTID='" + strTreatmentID + "' AND CRITERIA=''";
					//inserting '' in oracle inserts a null
					strSelect = "SELECT ATTRIBUTE_,CHANGE_ FROM CONSEQUENCES WHERE TREATMENTID='" + strTreatmentID + "' AND (CRITERIA='' OR CRITERIA IS NULL)";

                    ds = DBMgr.ExecuteQuery(strSelect);
                    String strCommitID = dgvSummary.Tag.ToString();

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        String strAttribute = dr[0].ToString();
                        String strChange = dr[1].ToString();
                        String strInsert = "INSERT INTO COMMIT_CONSEQUENCES (COMMITID,ATTRIBUTE_,CHANGE_) VALUES('" + strCommitID + "','" + strAttribute + "','" + strChange + "')";
                        DBMgr.ExecuteNonQuery(strInsert);

                        int nRow = dgvAttribute.Rows.Add(strAttribute, strChange);


						String strIdentity = "";
							switch (DBMgr.NativeConnectionParameters.Provider)
							{
								case "MSSQL":
									strIdentity = "SELECT IDENT_CURRENT ('COMMIT_CONSEQUENCES') FROM COMMIT_CONSEQUENCES";
									break;
								case "ORACLE":
									//strIdentity = "SELECT COMMIT_CONSEQUENCES_ID_SEQ.CURRVAL FROM DUAL";
									//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'COMMIT_CONSEQUENCES_ID_SEQ'";
									strIdentity = "SELECT MAX(CONSEQUENCES_ID) FROM COMMIT_CONSEQUENCES";
									break;
								default:
									throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
									//break;
							}
                        DataSet dataset = DBMgr.ExecuteQuery(strIdentity);
                        strIdentity = dataset.Tables[0].Rows[0].ItemArray[0].ToString();
                        dgvAttribute.Rows[nRow].Tag = strIdentity;
                    }
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Loading treatment. " + exception.Message);
            }

        }

        private void dgvSummary_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if(m_bCommitted)
            {
                if (this.dgvSummary.CurrentCellAddress.X == 1 && this.dgvSummary.CurrentCellAddress.Y == 3)
                {
                    DataGridViewComboBoxCell cb = (DataGridViewComboBoxCell)dgvSummary.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    if (cb != null)
                    {
                        if (!cb.Items.Contains(e.FormattedValue))
                        {
                            cb.Items.Add(e.FormattedValue);

                            m_bChange = true;
                            cb.Value = e.FormattedValue;
                            cb.DisplayMember = "";
                            

                        }
                    }
                }
            }

            if (m_bTreatments)
            {
                if (!m_bChange) return;
                int nCol = e.ColumnIndex;
                int nRow = e.RowIndex;
                FeasibleTreatmentSummaryChange(nCol,nRow);
            }
        }

        public void UpdateResultSummaryFacilitySection(String strFacility, String strSection)
        {
            dgvSummary.Rows.Clear();
            dgvAttribute.Rows.Clear();
            dgvSummary.Rows.Add("Facility", strFacility);
            dgvSummary.Rows.Add("Section", strSection);

        }



        public void UpdateResultSummary(String strID, String strYear,String strFacility, String strSection, Dictionary<string, string> attributeColumns)
        {
            dgvSummary.Rows.Clear();
            dgvAttribute.Rows.Clear();
            DataSet ds = null;
            String strAnalysisType = "Benefit Cost Ratio";
            String strSelect = "SELECT ANALYSIS FROM SIMULATIONS WHERE SIMULATIONID='" + SimulationID.ToString() + "'";
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    switch (row["Analysis"].ToString())
                    {
                        case "Maximum Benefit":
                        case "Multi-year Maximum Benefit":
                            strAnalysisType = "Maximum Benefit";
                            break;

                        case "Remaining Life/Cost":
                        case "Multi-year Remaining Life/Cost":
                            strAnalysisType = "Remaining Life Cost Ratio";
                            break;

                        case "Maximum Remaining Life":
                        case "Multi-year Maximum Life":
                            strAnalysisType = "Maximum Remaining Life";
                            break;

                        case "Conditional RSL/Cost":
                            strAnalysisType = "Life Extension/Cost";
                            break;
                        case "Incremental Benefit/Cost":
                        case "Multi-year Incremental Benefit/Cost":
                        case "Prioritized Needs":
                        default:
                            strAnalysisType = "Benefit Cost Ratio";
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Loading simualtion summary analysis type." + exception.Message);
                return;
            }
            
            String strReportTable = "REPORT_" + NetworkID.ToString() + "_" + SimulationID.ToString();
            String strSimulationTable = "SIMULATION_" + NetworkID.ToString() + "_" + SimulationID.ToString() + "_0";

            strSelect = "SELECT TREATMENT,BUDGET,COST_,YEARSANY,YEARSSAME,REMAINING_LIFE,BENEFIT,BC_RATIO,CONSEQUENCEID,PRIORITY,RLHASH,CHANGEHASH,COMMITORDER,ISCOMMITTED FROM " + strReportTable;
            strSelect += " WHERE SECTIONID ='" + strID + "' AND YEARS='" + strYear + "'";

            String strConsequenceID="";
            String strChangeHash = "";
            String strRLHash = "";

            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    dgvSummary.Rows.Add("Facility",strFacility);
                    dgvSummary.Rows.Add("Section",strSection);
                    dgvSummary.Rows.Add("Years",strYear);
                    dgvSummary.Rows.Add("Treatment",row[0].ToString());
                    dgvSummary.Rows.Add("Budget",row[1].ToString());
                    float fCost = 0;
                    float.TryParse(row[2].ToString(), out fCost);
					//don't need this now that inflation is now correctly already taken into account by this point
					//fCost *= CalculateTotalInflationRateForYear( int.Parse( strYear ) );
                    int nCost = dgvSummary.Rows.Add("Cost",fCost);
                    dgvSummary.Rows[nCost].Cells[1].Style.Format = "c";
                    
                    dgvSummary.Rows.Add("Years Before Any",row[3].ToString());
                    dgvSummary.Rows.Add("Years Before Same",row[4].ToString());
                    if (strAnalysisType == "Life Extension/Cost")
                    {
                        dgvSummary.Rows.Add("Life Extension", row[5].ToString());
                    }
                    else
                    {
                        dgvSummary.Rows.Add("Remaining Life", row[5].ToString());
                    }
                    dgvSummary.Rows.Add("Total Benefit",row[6].ToString());
                    dgvSummary.Rows.Add("Delta " + strAnalysisType,row[7].ToString());
                    strConsequenceID = row[8].ToString();
                    dgvSummary.Rows.Add("Priority Level",row[9].ToString());
                    strRLHash = row[10].ToString();
                    strChangeHash = row[11].ToString();
                    dgvSummary.Rows.Add("Commit Order", row[12].ToString());
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Loading simualtion summary." + exception.Message);
                return;

            }
            if (dgvAttribute.Columns.Count > 3)
            {
                if (strAnalysisType == "Life Extension/Cost")
                {
                    dgvAttribute.Columns[3].HeaderText = "Life Extension";
                }
                else
                {
                    dgvAttribute.Columns[3].HeaderText = "Remaining Life";
                }
            }
            Hashtable hashAttributeChange = new Hashtable();
            string[] pairs = strChangeHash.Split(new string[] { "\n" }, StringSplitOptions.None);
            for (int i = 0; i < pairs.Length; i++)
            {
                if (pairs[i].Contains("\t"))
                {
                    string[] attributechange = pairs[i].Split(new string[] { "\t" }, StringSplitOptions.None);
                    hashAttributeChange.Add(attributechange[0], attributechange[1]);
                }
            }


            Hashtable hashAttributeRL = new Hashtable();
            string[] pairsRL = strRLHash.Split(new string[] { "\n" }, StringSplitOptions.None);
            for (int i = 0; i < pairsRL.Length; i++)
            {
                if(pairsRL[i].Contains("\t"))
                {
                    string[] attributechange = pairsRL[i].Split(new string[] { "\t" }, StringSplitOptions.None);
                    hashAttributeRL.Add(attributechange[0], attributechange[1]);
                }
            }

            var listTables = new List<string>();
            
            strSelect = "SELECT ";
            foreach (String key in attributeColumns.Keys)
            {
                
                if (key.Contains("_" + strYear))
                {
                    strSelect += attributeColumns[key] + "." + key + ",";
                    if(!listTables.Contains(attributeColumns[key]))
                    {
                        listTables.Add(attributeColumns[key]);
                    }
                }
            }
            strSelect = strSelect.Substring(0, strSelect.Length - 1);
            String strWhere = " FROM " + strSimulationTable + " WHERE " + strSimulationTable + ".SECTIONID = '" + strID + "'";

            for(var i = 1; i < listTables.Count; i++)
            {
                strWhere = " INNER JOIN " + listTables[i] + " ON " + strSimulationTable + ".SECTIONID = " + listTables[i] + ".SECTIONID";
            }


            strSelect += strWhere;


            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);

                if (ds.Tables[0].Rows.Count == 1)
                {
                    int i = 0;
                    foreach (String str in m_listAttributeSimulation)
                    {
                        object[] oRow = new object[4];

                        oRow[0] = str.ToString();
                        oRow[2] = ds.Tables[0].Rows[0].ItemArray[i].ToString();
                        if (hashAttributeChange.Contains(str))
                        {
                            oRow[1] = hashAttributeChange[str];
                        }
                        //else
                        //{
                        //    oRow[1] = "";
                        //}

                        if (hashAttributeRL.Contains(str))
                        {
                            oRow[3] = hashAttributeRL[str];
                        }
                        //else
                        //{
                        //    oRow[3] = "";
                        //}

                        dgvAttribute.Rows.Add(oRow);
                        i++;
                    }
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput(exception.Message);

            }

        }

		private float CalculateTotalInflationRateForYear( int currentYear )
		{
			float rate = -1;
			DataSet inflationData = null;
			string selectStatment = "SELECT FIRSTYEAR,INFLATIONRATE FROM INVESTMENTS WHERE SIMULATIONID='" + m_strSimulationID + "'";

			try
			{
				inflationData = DBMgr.ExecuteQuery( selectStatment );
				int startYear = int.Parse( inflationData.Tables[0].Rows[0]["FIRSTYEAR"].ToString() );
				rate = float.Parse( inflationData.Tables[0].Rows[0]["INFLATIONRATE"].ToString() );
				rate /= 100;
				rate += 1;
				rate = ( float )Math.Pow( rate, currentYear - startYear );
			}
			catch( Exception ex )
			{
				Global.WriteOutput( "ERROR [could not retrieve inflation information]: " + ex.Message );
			}

			return rate;
		}



        public void UpdateResultAttributes(String strID, String strYear)
        {




        }


        private void dgvSummary_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int nRow = e.RowIndex;
            int nCol = e.ColumnIndex;
            m_oLastCell = dgvSummary.Rows[nRow].Cells[nCol].Value;
        }

        private void dgvAttribute_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            m_oLastAttributeCell = dgvAttribute.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        }

        private void dgvAttribute_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_bChange) return;
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvAttribute.Rows[e.RowIndex];


            if (e.ColumnIndex == 0)
            {
                String strAttribute = dgvAttribute.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                for (int nRow = 0; nRow < dgvAttribute.Rows.Count; nRow++)
                {
                    if (nRow != e.RowIndex)
                    {
                        if (dgvAttribute.Rows[nRow].Cells[0].Value != null)
                        {
                            if (strAttribute == dgvAttribute.Rows[nRow].Cells[0].Value.ToString())
                            {
                                m_bChange = false;
                                Global.WriteOutput("Error: A single committed project may not have duplicate attribute consequences.");
                                dgvAttribute.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastAttributeCell;
                                m_bChange = true;
                                return;
                            }
                        }
                    }
                }
            }


            String strUpdate = "";

            if (row.Tag != null)
            {
                String strValue = dgvAttribute.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                String strID = row.Tag.ToString();
                if (e.ColumnIndex == 0)
                {
                    strUpdate = "UPDATE COMMIT_CONSEQUENCES SET ATTRIBUTE_='" + strValue + "' WHERE ID_='" + strID + "'";

                }
                else if (e.ColumnIndex == 1)
                {
                    strUpdate = "UPDATE COMMIT_CONSEQUENCES SET CHANGE_='" + strValue + "' WHERE ID_='" + strID + "'";
                }
                else
                {
                    return;
                }
                try
                {
                    DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch (Exception exception)
                {
                    m_bChange = false;
                    dgvAttribute.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastAttributeCell;
                    Global.WriteOutput("Error updating attribute change pairs... " + exception.Message);
                    m_bChange = true;
                }
            }
            else
            {
                if (dgvSummary.Tag == null)
                {
                    m_bChange = false;
                    dgvAttribute.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastAttributeCell;
                    Global.WriteOutput("Error:Treatment information must be entered above before assigning attribute changes.");
                    m_bChange = true;
                    return;

                }
                if (dgvSummary.Tag.ToString() == "")
                {
                    m_bChange = false;
                    dgvAttribute.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastAttributeCell;
                    Global.WriteOutput("Error:Treatment information must be entered above before assigning attribute changes.");
                    m_bChange = true;
                    return;

                }

                String strCommitID = dgvSummary.Tag.ToString();
                String strInsert = "";
                String strValue = dgvAttribute.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                
                if (e.ColumnIndex == 0)
                {
                    strInsert = "INSERT INTO COMMIT_CONSEQUENCES (COMMITID,ATTRIBUTE_) VALUES('" + strCommitID + "','" + strValue + "')";

                }
                else if (e.ColumnIndex == 1)
                {
                    m_bChange = false;
                    dgvAttribute.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastAttributeCell;
                    Global.WriteOutput("Error: Select an Attribute before entering an attribute change.");
                    m_bChange = true;
                    return;
                }
                else
                {
                    m_bChange = false;
                    dgvAttribute.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastAttributeCell;
                    Global.WriteOutput("Error: Simulation results cannot be editted.");
                    m_bChange = true;
                    return;
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
									strIdentity = "SELECT IDENT_CURRENT ('COMMIT_CONSEQUENCES') FROM COMMIT_CONSEQUENCES";
									break;
								case "ORACLE":
									//strIdentity = "SELECT COMMIT_CONSEQUENCES_ID_SEQ.CURRVAL FROM DUAL";
									//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'COMMIT_CONSEQUENCES_ID_SEQ'";
									strIdentity = "SELECT MAX(ID_) FROM COMMIT_CONSEQUENCES";
									break;
								default:
									throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
									//break;
							}
                        DataSet ds = DBMgr.ExecuteQuery(strIdentity);
                        strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        dgvAttribute.Rows[e.RowIndex].Tag = strIdentity;

                    }
                    catch (Exception except)
                    {
                        m_bChange = false;
                        dgvAttribute.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastAttributeCell;
                        Global.WriteOutput("Error inserting new attribute-change pairs." + except.Message);
                        m_bChange = true;
                        return;
                    }
                }
            }
        }

        private void dgvSummary_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if(m_bCommitted)
            {

                if (this.dgvSummary.CurrentCellAddress.X == 1 && this.dgvSummary.CurrentCellAddress.Y == 3)
                {

                    ComboBox cb = e.Control as ComboBox;

                    if (cb != null)
                    {

                        cb.DropDownStyle = ComboBoxStyle.DropDown;

                    }
                }
            }
        }


        public bool GetSectionArea(String strSectionID, out String strArea)
        {
            strArea = "0";

            String strSelect = "SELECT AREA FROM SECTION_" + m_strNetworkID.ToString() + " WHERE SECTIONID='" + strSectionID + "'";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    strArea = row["AREA"].ToString();
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error getting area: " + exception.Message);
                return false;
            }
            return true;

        }

        private void dgvAttribute_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if(e.Row.Tag == null) return;
            DeleteAttributeRow(e.Row.Tag.ToString());
        }
        private void DeleteAttributeRow(String strID)
        {
			String strDelete = "DELETE FROM COMMIT_CONSEQUENCES WHERE ID_ ='" + strID + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strDelete);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Deleting committed consequence" + exception.Message);

            }
        }

		private void dgvAttribute_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{

		}





    }
    public class SimulationAttributes
    {
        public String m_strSectionID;
        public String m_strFacility;
        public String m_strYear;
        public String m_strSection;
        public String m_strCommitID;
        public String m_strTreatment;
        public String m_strBeforeAny;
        public String m_strBeforeSame;
        public String m_strCost;
        public String m_strBudget;
        public String m_strBenefitCost;
        public String m_strRemainingLife;
        public FormSimulationResults m_formResultCommit;
        public int m_nCol;
        public int m_nRow;

    }

    public class CommitAttributeChange
    {
        public String m_strID;
        public String m_strAttribute;
        public String m_strChange;
    }

}
