using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DatabaseManager;
using System.Collections;
using System.CodeDom.Compiler;
using Simulation;
namespace RoadCare3
{
    public partial class FormSimulationRunSingleSection : Form
    {
        public bool m_bCommit = false;
        private bool m_bChange;
        //private SimulationAttributes m_SimulationAttribute;
        public String m_strSimulation;
        public String m_strNetwork;
        
        private String m_strSimulationID;
        private String m_strNetworkID;

        private List<String> m_listAttributeSimulation;
        private List<String> m_listBudgets;
        private List<String> m_listTreatments;
        private List<String> m_listFeasibleTreatment;

        private String m_strSectionID;
        private String m_strYear;
        private String m_strBenefit;
        private String m_strSection;
        private String m_strSimulationTable;
        private String m_strReportTable;
        private Hashtable m_hashAttributeValue;

		private double inflationRate;


        public Simulation.Committed m_singleSection;

        /// <summary>
        /// Current values for given year and section        /// </summary>
        private String m_strCurrentTreatment;
        private String m_strCurrentAny;
        private String m_strCurrentSame;
        private String m_strCurrentCost;
        private String m_strCurrentBudget;
        private Hashtable m_hashCurrentChange;

        public String m_strSingleTreatment;
        public String m_strSingleAny;
        public String m_strSingleSame;
        public String m_strSingleCost;
        public String m_strSingleBudget;
        public Hashtable m_hashSingleChange;

        public String BenefitCostTable
        {
            get { return m_strBenefit; }
            set { m_strBenefit = value; }
        }

        public String SimulationTable
        {
            get { return m_strSimulationTable; }
            set { m_strSimulationTable = value; }
        }

        public String SectionTable
        {
            get { return m_strSection; }
            set { m_strSection = value; }
        }
        public String ReportTable
        {
            get { return m_strReportTable; }
            set { m_strReportTable = value; }
        }
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



        public FormSimulationRunSingleSection(String strSectionID, String strYear,String strNetworkID, String strSimulationID, double inflation)
        {
            InitializeComponent();
            this.NetworkID = strNetworkID;
            this.SimulationID = strSimulationID;
            this.SectionID = strSectionID;
            this.Year = strYear;

			inflationRate = inflation;

            buttonCancel.Visible = true;
            buttonRun.Visible = true;
            buttonCommit.Visible = false;
            m_strBenefit = "BENEFITCOST_" + m_strNetworkID + "_" + m_strSimulationID;
            m_strSection = "SECTION_" + m_strNetworkID;
            m_strSimulationTable = "SIMULATION_" + m_strNetworkID + "_" + m_strSimulationID;
            m_strReportTable = "REPORT_" + m_strNetworkID + "_" + m_strSimulationID;
            FillAttributeValueHash();
        }
        /// <summary>
        /// Loads existing data for this SECTION and YEAR
        /// </summary>
        private void FillAttributeValueHash()
        {
            // Fill list of variables available in Simulation.  May not add new for single section run.
            m_listAttributeSimulation = new List<String>();
            String strSelect = "SELECT SIMULATION_VARIABLES FROM SIMULATIONS WHERE SIMULATIONID='" + SimulationID + "'";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    string[] attributes = ds.Tables[0].Rows[0].ItemArray[0].ToString().Split(new string[] { "\t" }, StringSplitOptions.None);
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        m_listAttributeSimulation.Add(attributes[i]);
                    }
                }
            }
            catch (Exception exception)
            {
                labelError.Visible = true;
                labelError.Text = "Error: Loading and parsing SIMULATION variables." + exception.Message;
            }

            //Fill list of current values starting with LENGTH and AREA for COST calculations
            m_hashAttributeValue = new Hashtable();
            //Get Length and Area for COST calculations
            String strArea = "0";
            double dLength = 0;
            strSelect = "SELECT END_STATION,BEGIN_STATION,AREA FROM SECTION_" + NetworkID + " WHERE SECTIONID='" + SectionID + "'";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    if (!String.IsNullOrEmpty(dr["END_STATION"].ToString()) || !String.IsNullOrEmpty(dr["BEGIN_STATION"].ToString()))
                    {
                        dLength = (double)dr["END_STATION"] - (double)dr["BEGIN_STATION"];
                        
                        m_hashAttributeValue.Add("LENGTH", dLength);
                    }
                    strArea = dr["AREA"].ToString();
                    double dArea = Convert.ToDouble(strArea);
                    m_hashAttributeValue.Add("AREA", dArea);
                }
            }
            catch (Exception exception)
            {
                labelError.Visible = true;
                labelError.Text = "Error: Loading AREA information for cost calculation. " + exception.Message;
            }
            
            //Get values for each simulation value
            strSelect = "SELECT ";
            foreach (String str in m_listAttributeSimulation)
            {
                if (strSelect.Length != 7)
                {
                    strSelect += ",";//Does not add comma first pass
                }
                String strAttributeYear = str + "_" + this.Year;
                strSelect += strAttributeYear;
            }

            String strWhere = " FROM " + this.SimulationTable + " WHERE SECTIONID ='" + this.SectionID + "'";
            strSelect += strWhere;

            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    foreach (String attribute in m_listAttributeSimulation)
                    {
                        m_hashAttributeValue.Add(attribute, dr[attribute+"_" +this.Year]);
                    }
                }
            }
            catch (Exception exception)
            {
                labelError.Visible = true;
                labelError.Text = "Error: Loading and parsing SIMULATION RESULTS table." + exception.Message;

            }

            //Load current treatment
            strSelect = "SELECT TREATMENT,COST_,BUDGET,YEARSANY,YEARSSAME,CHANGEHASH,AREA FROM " + this.ReportTable + " WHERE YEARS='" + this.Year + "' AND SECTIONID='" + this.SectionID + "'";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    m_strCurrentTreatment = dr["TREATMENT"].ToString();

                    double dCost = double.Parse(dr["COST_"].ToString()); //inflation already factored in in the report table
                    double dArea = double.Parse(dr["AREA"].ToString());
                    //dCost *= dArea;
                    m_strCurrentCost = dCost.ToString("c"); 
                    m_strCurrentBudget = dr["BUDGET"].ToString();
                    m_strCurrentAny = dr["YEARSANY"].ToString();
                    m_strCurrentSame = dr["YEARSSAME"].ToString();
                    String strHashChange = dr["CHANGEHASH"].ToString();
                    m_hashCurrentChange = new Hashtable();
                    string[] pairs = strHashChange.Split(new string[] { "\n" }, StringSplitOptions.None);
                    for (int i = 0; i < pairs.Length; i++)
                    {
                        if (pairs[i].Contains("\t"))
                        {
                            string[] attributechange = pairs[i].Split(new string[] { "\t" }, StringSplitOptions.None);
                            m_hashCurrentChange.Add(attributechange[0], attributechange[1]);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                labelError.Visible = true;
                labelError.Text = "Error: Loading and parsing REPORT table." + exception.Message;

            }            
        }

        private void FormSimulationRunSingleSection_Load(object sender, EventArgs e)
        {
            LoadFormSimulationAttributes();
            SetupDataGridViews();
        }
        /// <summary>
        /// Load All data for filling combo boxes.
        /// </summary>
        private void LoadFormSimulationAttributes()
        {
            String strSelect = "SELECT SIMULATION_VARIABLES FROM SIMULATIONS WHERE SIMULATIONID='" + SimulationID + "'";
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


        /// <summary>
        /// Setup datagridviews for form.
        /// </summary>
        private void SetupDataGridViews()
        {
            m_bChange = false;
            m_listFeasibleTreatment = new List<String>();
            String strSelect = "SELECT TREATMENT FROM " + this.BenefitCostTable + " INNER JOIN " + this.SectionTable + " ON " + this.SectionTable + ".SECTIONID=" + this.BenefitCostTable + ".SECTIONID WHERE " + this.BenefitCostTable + ".SECTIONID=" + this.SectionID + " AND YEARS=" + this.Year;
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
            foreach (String str in m_listAttributeSimulation)
            {
                comboboxColumn.Items.Add(str);
            }
            dgvAttribute.Columns.Insert(0, comboboxColumn);


            dgvAttribute.RowHeadersVisible = true;
            dgvAttribute.AllowUserToDeleteRows = true;
            dgvAttribute.ColumnCount = 2;
            dgvAttribute.Columns[1].Name = "Change";
            dgvAttribute.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvAttribute.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            foreach (String key in m_hashCurrentChange.Keys)
            {
                int nRow = dgvAttribute.Rows.Add(key, m_hashCurrentChange[key]);
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
            foreach (String str in m_listFeasibleTreatment)
            {
                cbTreatment.Items.Add(str);
            }
            dgvSummary.Rows.Add(dataGridRow);

            if (!cbTreatment.Items.Contains("No Treatment"))
            {
                cbTreatment.Items.Add("No Treatment");
            }

            if(!cbTreatment.Items.Contains(m_strCurrentTreatment))
            {
                cbTreatment.Items.Add(m_strCurrentTreatment);
            }
            cbTreatment.Value = m_strCurrentTreatment;

            // ------------------------------------------------
            // Add a combobox cell and add the combobox items            
            dataGridRow = new DataGridViewRow();
            cells = new DataGridViewCell[2];
            txtCell = new DataGridViewTextBoxCell();
            DataGridViewComboBoxCell cbBudget = new DataGridViewComboBoxCell();
            cbBudget.Items.Add("");
            foreach (String str in m_listBudgets)
            {
                cbBudget.Items.Add(str);
            }

            if (!m_listBudgets.Contains(m_strCurrentBudget))
            {
                cbBudget.Items.Add(m_strCurrentBudget);
            }
            cbBudget.Value = m_strCurrentBudget;
            
            txtCell.Value = "Budget";
            dataGridRow.Cells.Add(txtCell);
            txtCell.ReadOnly = true;
            dataGridRow.Cells.Add(cbBudget);
            dgvSummary.Rows.Add(dataGridRow);


            
            
            float fCost = 0;
            int nCostRow = dgvSummary.Rows.Add("Cost", fCost);
            dgvSummary.Rows[nCostRow].Cells[1].Style.Format = "c";
            dgvSummary[1, nCostRow].Value = m_strCurrentCost;
            
            dgvSummary.Rows.Add("Years Before Any", m_strCurrentAny);
            dgvSummary.Rows.Add("Years Before Same", m_strCurrentSame);

            m_bChange = true;
        }

        /// <summary>
        /// Programatically sets combo box properties for attribute DataGridView
        /// </summary>
        /// <returns></returns>
        private DataGridViewComboBoxColumn CreateComboBoxColumn()
        {
            DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
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
        /// <summary>
        /// Allows user to add new Treatments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvSummary_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.dgvSummary.CurrentCellAddress.X == 1 && this.dgvSummary.CurrentCellAddress.Y == 1)
            {

                ComboBox cb = e.Control as ComboBox;
                if (cb != null)
                {

                    cb.DropDownStyle = ComboBoxStyle.DropDown;

                }
            }

        }
        /// <summary>
        /// Adds new treatment to comboBox list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvSummary_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (this.dgvSummary.CurrentCellAddress.X == 1 && this.dgvSummary.CurrentCellAddress.Y == 1)
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

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.dgvSummary.CurrentCellAddress.X == 1 && this.dgvSummary.CurrentCellAddress.Y == 1)
            {
                UpdateSummaryValues();
            }
            else if (this.dgvSummary.CurrentCellAddress.X == 1 && this.dgvSummary.CurrentCellAddress.Y == 0)
            {
                UpdateAvailableTreatments();
            }
        }
        /// <summary>
        /// Updates summary when necessary
        /// </summary>
        private void UpdateSummaryValues()
        {
            if (!m_bChange) return;
            labelError.Visible = false;
            String strTreatment = dgvSummary[1, 1].Value.ToString();


            if (strTreatment == m_strCurrentTreatment)
            {
                dgvSummary[1, 2].Value = m_strCurrentBudget;
                dgvSummary[1, 3].Value = m_strCurrentCost;
                dgvSummary[1, 4].Value = m_strCurrentAny;
                dgvSummary[1, 5].Value = m_strCurrentSame;

                dgvAttribute.Rows.Clear();
                foreach (String key in m_hashCurrentChange.Keys)
                {
                    int nRow = dgvAttribute.Rows.Add(key, m_hashCurrentChange[key]);
                }

            }
            else if (m_listFeasibleTreatment.Contains(strTreatment))
            {
                double dArea = 0;
                String strSelect = "SELECT AREA FROM " + this.ReportTable + " WHERE SECTIONID ='" + this.SectionID +"' AND YEARS='" + this.Year + "'" ;
                try
                {
                    DataSet ds = DBMgr.ExecuteQuery(strSelect);
                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        dArea = double.Parse(dr["AREA"].ToString());

                    }

                }
                catch (Exception exception)
                {
                    labelError.Visible = true;
                    labelError.Text = "Error retrieving AREA." + exception.Message;

                }


                strSelect = "SELECT BUDGET,YEARSANY,YEARSSAME,COST_,CHANGEHASH FROM " + this.BenefitCostTable + " WHERE SECTIONID ='" + this.SectionID +"' AND YEARS='" + this.Year + "' AND TREATMENT='" + strTreatment + "'" ;
                try
                {
                    DataSet ds = DBMgr.ExecuteQuery(strSelect);
                    if (ds.Tables[0].Rows.Count > 0)
                    {

						

                        DataRow dr = ds.Tables[0].Rows[0];
						double dCost = double.Parse( dr["COST_"].ToString() ) * dArea * inflationRate;
                        string budget = dr["BUDGET"].ToString();
                        if(!budget.Contains("|")) dgvSummary[1, 2].Value = budget;
						
                        dgvSummary[1, 3].Value = Math.Round( dCost, 2 ).ToString( "c" );
                        dgvSummary[1, 4].Value = dr["YEARSANY"].ToString();
                        dgvSummary[1, 5].Value = dr["YEARSSAME"].ToString();
                        String strChangeHash = dr["CHANGEHASH"].ToString();
                        
                        dgvAttribute.Rows.Clear();
                        string[] pairs = strChangeHash.Split(new string[] { "\n" }, StringSplitOptions.None);
                        for (int i = 0; i < pairs.Length; i++)
                        {
                            if (pairs[i].Contains("\t"))
                            {
                                string[] attributechange = pairs[i].Split(new string[] { "\t" }, StringSplitOptions.None);
                                dgvAttribute.Rows.Add(attributechange[0], attributechange[1]);      
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    labelError.Visible = true;
                    labelError.Text = "Error: Retrieving default information from the BENEFIT_COST table." + exception.Message;
                }
            }
            else if(m_listTreatments.Contains(strTreatment))
            {
                //Fill with default from Treatment table
                String strSelect = "SELECT BUDGET, BEFOREANY, BEFORESAME, TREATMENTID FROM TREATMENTS WHERE SIMULATIONID ='" + this.SimulationID + "' AND TREATMENT='" + strTreatment + "'";
                String strTreatmentID = "";
                try
                {
                    DataSet ds = DBMgr.ExecuteQuery(strSelect);
                    if(ds.Tables[0].Rows.Count == 1)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        dgvSummary[1,2].Value = dr["BUDGET"].ToString();
                        dgvSummary[1,4].Value = dr["BEFOREANY"].ToString();
                        dgvSummary[1,5].Value = dr["BEFORESAME"].ToString();
                        strTreatmentID = dr["TREATMENTID"].ToString();
                    }

                }
                catch(Exception exception)
                {
                    labelError.Visible = true;
                    labelError.Text = "Error: Retrieving default information from the TREATMENTS table." + exception.Message;
                }

                //Get Cost information
                String strCost = "0";
                //strSelect = "SELECT COST_ FROM COSTS WHERE TREATMENTID='" + strTreatmentID + "' AND CRITERIA=''"; 
				//inserting '' in oracle inserts a null
                // TODO: Why not pull the criteria for each cost equation found, and calculate the cost for each and show them in a cost dropdown.
                strSelect = "SELECT COST_,CRITERIA FROM COSTS WHERE TREATMENTID='" + strTreatmentID + "'"; 
                try
                {
                    DataSet ds = DBMgr.ExecuteQuery(strSelect);
                    if(ds.Tables[0].Rows.Count == 1)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        var cost = dr["COST_"];
                        var criteria = dr["CRITERIA"];


                        if (criteria == DBNull.Value)
                        {
                            strCost = dr["COST_"].ToString();
                        }
                        else
                        {
                            var criteriaString = criteria.ToString();
                            if (String.IsNullOrWhiteSpace(criteriaString))
                            {
                                strCost = dr["COST_"].ToString();
                            }
                            else
                            {
                                var criteriaToEvaluate = new Criterias();
                                criteriaToEvaluate.Criteria = criteriaString;
                                if (criteriaToEvaluate.IsCriteriaMet(m_hashAttributeValue))
                                {
                                    strCost = dr["COST_"].ToString();
                                }
                            }
                        }
                        strCost = dr["COST_"].ToString();
                    }
                }
                catch(Exception exception)
                {
                    labelError.Visible = true;
                    labelError.Text = "Error: Retrieving default COST information from the COSTS table." + exception.Message;
                }
                List<String> listError;
                List<String> listAttributesEquation = Global.TryParseAttribute(strCost, out listError);            // See if listAttributeEquations is included in dgvDefault
                CalculateEvaluate.CalculateEvaluate calculate = new CalculateEvaluate.CalculateEvaluate();
                calculate.BuildTemporaryClass(strCost, true);
                CompilerResults m_crEquation = calculate.CompileAssembly();

                object[] input = new object[listAttributesEquation.Count];
                int i = 0;
                foreach (String attribute in listAttributesEquation)
                {

                    input[i] = m_hashAttributeValue[attribute];
                    i++;
                }
				try
				{
					object result = calculate.RunMethod(input);
					dgvSummary[1, 3].Value = result.ToString();
				}
				catch(Exception exc)
				{
					Global.WriteOutput("Error running single section. " + exc.Message);
				}


                //strSelect = "SELECT ATTRIBUTE_,CHANGE_ FROM CONSEQUENCES WHERE TREATMENTID='" + strTreatmentID + "' AND CRITERIA=''";
				//inserting '' in oracle inserts a null
				strSelect = "SELECT ATTRIBUTE_,CHANGE_, CRITERIA FROM CONSEQUENCES WHERE TREATMENTID='" + strTreatmentID + "'";
                dgvAttribute.Rows.Clear();
                try
                {
                    DataSet ds = DBMgr.ExecuteQuery(strSelect);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var attribute = dr["ATTRIBUTE_"];
                        var change = dr["CHANGE_"];
                        var criteria = dr["CRITERIA"];

                        if (criteria == DBNull.Value)
                        {
                            dgvAttribute.Rows.Add(dr["ATTRIBUTE_"].ToString(), dr["CHANGE_"].ToString());
                        }
                        else
                        {
                            var criteriaString = criteria.ToString();
                            if (String.IsNullOrWhiteSpace(criteriaString))
                            {
                                dgvAttribute.Rows.Add(dr["ATTRIBUTE_"].ToString(), dr["CHANGE_"].ToString());
                            }
                            else
                            {
                                var criteriaToEvaluate = new Criterias();
                                criteriaToEvaluate.Criteria = criteriaString;
                                if (criteriaToEvaluate.IsCriteriaMet(m_hashAttributeValue))
                                {
                                    dgvAttribute.Rows.Add(dr["ATTRIBUTE_"].ToString(), dr["CHANGE_"].ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    labelError.Visible = true;
                    labelError.Text = "Error: Loading Consequences for selected TREATMENT. " + exception.Message;
                }
            }
            else
            {
                //User defined treatment.  Leave existing as is.

            }
            dgvSummary.Update();
            dgvAttribute.Update();
        }


        /// <summary>
        /// Fills in Feasible or All treatments in the treatment drop down.
        /// </summary>
        private void UpdateAvailableTreatments()
        {
            if (!m_bChange) return;
            m_bChange = false;
            DataGridViewComboBoxCell cbTreatment = (DataGridViewComboBoxCell)dgvSummary.Rows[1].Cells[1];
            String strCurrent = cbTreatment.Value.ToString();
            cbTreatment.Items.Clear();
            
            if (dgvSummary[1, 0].Value.ToString() == "True")
            {
                foreach (String treatment in m_listFeasibleTreatment)
                {
                    cbTreatment.Items.Add(treatment);
                }
            }
            else
            {

                foreach (String treatment in m_listTreatments)
                {
                    cbTreatment.Items.Add(treatment);
                }
            }
            if (!cbTreatment.Items.Contains(strCurrent))
            {
                cbTreatment.Items.Add(strCurrent);
            }
            cbTreatment.Value = strCurrent;
            m_bChange = true;
        }

        private void dgvSummary_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == 1 && e.ColumnIndex == 1)
            {
                UpdateSummaryValues();
            }
            else if(e.RowIndex == 0 && e.ColumnIndex == 1)
            {
                UpdateAvailableTreatments();

            }
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            if (CheckInput())
            {
                m_bCommit = false;
                m_singleSection.IsCommitted = false;
                this.DialogResult = DialogResult.OK;
                this.Close();

				//Simulation.Simulation simulation = new Simulation.Simulation(m_strSimulation, m_strNetwork);
				//Simulation.Simulation simulation = new Simulation.Simulation(m_strSimulation, m_strNetwork, m_strSimulationID, m_strNetworkID);
				//simulation.SingleSectionSimulation(m_strSectionID, m_strYear);
				//simulation.SingleSectionSimulation();
				//simulation.RunSingleSection(m_strSectionID, m_strYear, m_singleSection);
				//simulation.RunSingleSection(m_strSectionID, m_strYear, m_singleSection, m_bCommit);
            }
        }

        private void buttonCommit_Click(object sender, EventArgs e)
        {
            if (CheckInput())
            {
                m_bCommit = true;
                m_singleSection.IsCommitted = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            
        }

        private bool CheckInput()
        {
            //Modified by adding simulationID so can run multiple runs for Cartegeraph.
            SimulationMessaging.LoadAttributes(m_strSimulationID);
            m_singleSection = new Simulation.Committed();

            labelError.Visible = false;
            //Check for Treatment Name
            m_strSingleTreatment = dgvSummary[1, 1].Value.ToString();
            if (m_strSingleTreatment.Trim().Length == 0)
            {
                labelError.Visible = true;
                labelError.Text = "Error:Non-blank treatment name must be entered.";
                return false;
            }
            m_singleSection.Treatment = m_strSingleTreatment;

            //Check for budget
            m_strSingleBudget = dgvSummary[1, 2].Value.ToString();
            if (m_strSingleTreatment != "No Treatment")
            {
                if (!m_listBudgets.Contains(m_strSingleBudget))
                {
                    labelError.Visible = true;
                    labelError.Text = "Error: Budget category must be selected";
                    return false;
                }
                m_singleSection.Budget = m_strSingleBudget;
            }
            //Check for Cost (is positive number)

            m_strSingleCost = dgvSummary[1, 3].Value.ToString().Replace("$","");
            try
            {
                double dCost = double.Parse(m_strSingleCost);
                if (dCost < 0)
                {
                    labelError.Visible = true;
                    labelError.Text = "Error: Cost must be greater than or equal to 0.";
                    return false;
                }
            }
            catch
            {
                labelError.Visible = true;
                labelError.Text = "Error: Cost must be a number.";
                return false;
            }
            m_singleSection.Cost = float.Parse(m_strSingleCost);
            
            //Check for YearsAny (is positive number)
            m_strSingleAny = dgvSummary[1, 4].Value.ToString();
            try
            {
                int nAny = int.Parse(m_strSingleAny);
                if (nAny < 0)
                {
                    labelError.Visible = true;
                    labelError.Text = "Error: Year any must be an integer must be greater than or equal to 0.";
                    return false;
                }
            }
            catch
            {
                labelError.Visible = true;
                labelError.Text = "Error: Year Any must be a positive integer.";
                return false;
            }
            m_singleSection.Any = int.Parse(m_strSingleAny);

            //Check for Years Same (is positive number)
            m_strSingleSame = dgvSummary[1, 5].Value.ToString();
            try
            {
                int nSame = int.Parse(m_strSingleSame);
                if (nSame < 0)
                {
                    labelError.Visible = true;
                    labelError.Text = "Error: Year same must be an integer must be greater than or equal to 0.";
                    return false;
                }
            }
            catch
            {
                labelError.Visible = true;
                labelError.Text = "Error: Year Same must be a positive integer.";
                return false;
            }
            m_singleSection.Same = int.Parse(m_strSingleSame);

            //Check to make sure there are not repeats of attributes.
            List<String> listConsequence = new List<String>();
            Simulation.Consequences consequence = new Simulation.Consequences();
            foreach (DataGridViewRow row in dgvAttribute.Rows)
            {
                if (row.Cells[0].Value == null)
                {
                    continue;
                }
                if (listConsequence.Contains(row.Cells[0].Value.ToString()))
                {
                    labelError.Visible = true;
                    labelError.Text = "Error: Multiple consequences for the same attribute not allowed.";
                    return false;
                }
                listConsequence.Add(row.Cells[0].Value.ToString());
                consequence.LoadAttributeChange(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());
            }
            m_singleSection.Consequence = consequence;

            return true;
        }







    }
}
