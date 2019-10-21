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
using System.Threading;
using Simulation;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using DataAccessLayer;

namespace RoadCare3
{
    public partial class FormSimulationResults : BaseForm
    {
        private String m_strNetwork;
        private String m_strSimulation;
        private String m_strSimulationID;
        private String m_strNetworkID;
        private bool m_bCommitted = false;
        private bool m_bChange = false;

        //private String m_strTreatmentName;
       // private String m_strYearAny;
        //private String m_strYearSame;
        //private String m_strCost;
        //private String m_strBudget;

        //private String m_strArea;
        //private String m_strUnit;
        Simulation.Simulation m_simulation = null;
        private Thread m_simulationThread;

        public Hashtable m_hashBudgetYearView = new Hashtable();//  strKey = budget   value= hashtable with strKey = year  value = sum cost.

        private List<CommitAttributeChange> m_listAttributeChange = new List<CommitAttributeChange>();
        private bool m_bNoToolTip = false;
        private int m_nStart = 0;
        private int m_nPeriod = 0;
        private int m_nRowSingle = 0;
        private int m_nColSingle = 0;
        //private bool bCommit = false;        

        Hashtable _hashIDRow = new Hashtable();
        List<string> _simulationYearAttribute = new List<string>();
        Dictionary<string, string> m_dictionaryColumns;
        
        public DataGridView GridViewCommitResult
        {
            get { return dgvResultCommit; }
        }

        public FormSimulationResults(String strNetwork, String strSimulation, String strSimulationID, String strNetworkID,bool bCommitted)
        {
            InitializeComponent();
            m_strNetwork = strNetwork;
            m_strSimulation = strSimulation;
            m_strSimulationID = strSimulationID;
            m_strNetworkID = strNetworkID;
            m_bCommitted = bCommitted;

            m_dictionaryColumns = new Dictionary<string, string>();
            for(var i = 0; i < 10; i++)
            {
                var simulationTable = "SIMULATION_" + m_strNetworkID + "_" + m_strSimulationID + "_" + i;
                if (DBMgr.CheckIfTableExists(simulationTable))
                {
                    var columns = DBMgr.GetTableColumns(simulationTable);
                    foreach(var column in columns)
                    {
                        if (column.ToUpper() == "SECTIONID") continue;
                        m_dictionaryColumns.Add(column, simulationTable);
                    }
                }
            }



            String strReportTable = "REPORT_" + m_strNetworkID.ToString() + "_" + m_strSimulationID.ToString();
            String strSelect = "SELECT FIRSTYEAR FROM INVESTMENTS WHERE SIMULATIONID='" + m_strSimulationID + "'";
            DataSet ds = null;

            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    Global.WriteOutput("Error: Run Simulation before viewing Simulation Results");
                    return;
                }

                if (ds.Tables[0].Rows[0].ItemArray[0].ToString() == "")
                {

                    Global.WriteOutput("Error: Run Simulation before viewing Simulation Results");
                    return;
                }
                m_nStart = int.Parse(ds.Tables[0].Rows[0].ItemArray[0].ToString());
                
            }

            catch// (Exception exception)
            {
                Global.WriteOutput("Error: Run Simulation before viewing Simulation Results.");// + exception.Message);
                return;
            }


            strSelect = "SELECT NUMBERYEARS FROM INVESTMENTS WHERE SIMULATIONID='" + m_strSimulationID + "'";
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    Global.WriteOutput("Error: Run Simulation before viewing Simulation Results");
                    return;
                }
                m_nPeriod = int.Parse(ds.Tables[0].Rows[0].ItemArray[0].ToString());
            }

            catch (Exception exception)
            {
                Global.WriteOutput("Error: Run Simulation before viewing Simulation Results." + exception.Message);
                return;
            }


            if (bCommitted)
            {
                commitToolStripMenuItem.Enabled = false;
            }
            else
            {
                label4.Visible = false;
                labelAnalysis.Visible = false;
                cbStartYear.Visible = false;
                cbYears.Visible = false;
            }
        }

        private void FormSimulation_Load(object sender, EventArgs e)
        {
			SecureForm();

			FormLoad(Settings.Default.SIMULATION_IMAGE_KEY, Settings.Default.SIMULATION_IMAGE_KEY_SELECTED);

            if (this.TabText.Contains("Committed"))
            {
                labelResult.Text = "Committed " + m_strNetwork + " - " + m_strSimulation;
            }
            else
            {
                labelResult.Text = "Results " + m_strNetwork + " - " + m_strSimulation;
            }

            String strSelect = "";
            DataSet ds = null;
            comboBoxRouteFacilty.Items.Add("All");

            strSelect = "SELECT DISTINCT FACILITY FROM SECTION_" + m_strNetworkID.ToString() + " ORDER BY FACILITY";
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
                foreach(DataRow row in ds.Tables[0].Rows)
                {
                    comboBoxRouteFacilty.Items.Add(row[0].ToString());
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Loading Facility filter. " + exception.Message);
            }

            strSelect = "SELECT COUNT(*) FROM SECTION_" + m_strNetworkID.ToString();
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
                int nCount = int.Parse(ds.Tables[0].Rows[0].ItemArray[0].ToString());
                String strFirst = comboBoxRouteFacilty.Items[1].ToString();
				//String strFirst = "";
				//if (nCount >= 0)
				//{
				//     strFirst = comboBoxRouteFacilty.Items[0].ToString();
				//}
                if (nCount > 15000) comboBoxRouteFacilty.Text = strFirst;
                else comboBoxRouteFacilty.Text = "All";

            }
            catch (Exception exception)
            {
                 Global.WriteOutput("Error: Loading Facility filter. " + exception.Message);
            }

           


            if (m_bCommitted)
            {
                strSelect = "SELECT COMMITTED_START, COMMITTED_PERIOD FROM SIMULATIONS WHERE SIMULATIONID='" + m_strSimulationID + "'";
                try
                {
                    ds = DBMgr.ExecuteQuery(strSelect);
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error initalizing Committed Project page. " + exception.Message);
                    return;
                }

                String strStart = "2000";
                String strPeriod = "10";
                int nStartYear = 2000;
                int nPeriod = 10;

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    strStart = row[0].ToString();
                    strPeriod = row[1].ToString();

                    if (strStart.Trim().Length == 0)
                    {
                        ds = null;
                        strSelect = "SELECT FIRSTYEAR,NUMBERYEARS FROM INVESTMENTS WHERE SIMULATIONID='" + m_strSimulationID + "'";
                        try
                        {
                            ds = DBMgr.ExecuteQuery(strSelect);
                        }
                        catch (Exception exception)
                        {
                            Global.WriteOutput("Error initalizing get StartYear and Period from table INVESTMENTS page. " + exception.Message);
                            return;
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            row = ds.Tables[0].Rows[0];
                            strStart = row[0].ToString();
                            strPeriod = row[1].ToString();

                            String strUpdate = "UPDATE SIMULATIONS SET COMMITTED_START='" + strStart + "' WHERE SIMULATIONID='" + m_strSimulationID + "'";
                            try
                            {
                                DBMgr.ExecuteNonQuery(strUpdate);
                            }
                            catch (Exception exception)
                            {
                                Global.WriteOutput("Error initalizing get StartYear and Period from table INVESTMENTS page. " + exception.Message);
                            }

                            strUpdate = "UPDATE SIMULATIONS SET COMMITTED_PERIOD='" + strPeriod + "' WHERE SIMULATIONID='" + m_strSimulationID + "'";
                            try
                            {
                                DBMgr.ExecuteNonQuery(strUpdate);
                            }
                            catch (Exception exception)
                            {
                                Global.WriteOutput("Error initalizing get StartYear and Period from table INVESTMENTS page. " + exception.Message);
                            }
                        }
                    }
                }
                int.TryParse(strStart, out nStartYear);
                int.TryParse(strPeriod, out nPeriod);
                cbStartYear.Text = strStart;
                cbYears.Text = strPeriod;

                dgvResultCommit.ColumnCount = nPeriod + 2;
                dgvResultCommit.Columns[0].Name = "FACILITY";
                dgvResultCommit.Columns[1].Name = "SECTION";

                UpdateView();
                dgvResultCommit.ReadOnly = true;
                m_bChange = true;


            }
            else
            {
                m_bChange = false;
                UpdateView();
                dgvResultCommit.ReadOnly = true;

                contextMenuCommitResult.Items["Copy"].Enabled = false;
                contextMenuCommitResult.Items["Paste"].Enabled = false;
                contextMenuCommitResult.Items["Delete"].Enabled = false;
                contextMenuCommitResult.Items["Cut"].Enabled = false;
                m_bChange = true;
            }


        }

		protected void SecureForm()
		{
			LockDataGridView( dgvResultCommit );
			LockContextMenuStrip( contextMenuCommitResult );
			LockButton( btnCommitExport );
			LockButton( btnCommitImport );

			if( m_bCommitted )
			{
				btnCommitExport.Visible = true;
				btnCommitImport.Visible = true;
				if( Global.SecurityOperations.CanModifySimulationCommitted( m_strNetworkID, m_strSimulationID ) )
				{
					UnlockButton( btnCommitExport );
					UnlockButton( btnCommitImport );

					contextMenuCommitResult.Items["Copy"].Enabled = true;
					contextMenuCommitResult.Items["Paste"].Enabled = true;
					if( Global.SecurityOperations.CanRemoveSimulationCommitted( m_strNetworkID, m_strSimulationID ) )
					{
						contextMenuCommitResult.Items["Delete"].Enabled = true;
                        contextMenuCommitResult.Items["Cut"].Enabled = true;
					}
				}
			}
			else
			{
				if( Global.SecurityOperations.CanCreateSimulationCommitted( m_strNetworkID, m_strSimulationID ) )
				{
					contextMenuCommitResult.Items["commitToolStripMenuItem"].Enabled = true;
				}
				if( Global.SecurityOperations.CanModifySimulationResults( m_strNetworkID, m_strSimulationID ) )
				{
					UnlockDataGridViewForModify( dgvResultCommit );
				}
			}

		}

		private void cbStartYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_bChange) return;
            if (m_bCommitted)
            {
                String strStartYear = cbStartYear.Text;
                String strUpdate = "UPDATE SIMULATIONS SET COMMITTED_START='" + strStartYear + "' WHERE SIMULATIONID='" + m_strSimulationID + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error updating StartYear and Period. " + exception.Message);
                }
                UpdateView();
            }

        }

        private void cbYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_bChange) return;
            if (m_bCommitted)
            {
                String strPeriod = cbYears.Text;
                String strUpdate = "UPDATE SIMULATIONS SET COMMITTED_PERIOD='" + strPeriod + "' WHERE SIMULATIONID='" + m_strSimulationID + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error updating StartYear and Period. " + exception.Message);
                }

                UpdateView();
            }
        }

        private void UpdateView()
        {
            if (m_bCommitted)
            {
                UpdateCommitted();
                return;
            }

            dgvResultCommit.Columns.Clear();
            dgvResultCommit.Rows.Clear();

            dgvResultCommit.ColumnCount = 2;
            dgvResultCommit.Columns[0].Name = "FACILITY";
            dgvResultCommit.Columns[1].Name = "SECTION";



            int nMinimum = -1;
            int nMaximum = -1;
            int nLastSection = -1;
            int nLastRow = -1;

            String strReportTable = "REPORT_" + m_strNetworkID.ToString() + "_" + m_strSimulationID.ToString();
            String strSectionTable = "SECTION_" + m_strNetworkID.ToString();
            String strBenefitTable = "BENEFITCOST_" + m_strNetworkID.ToString() + "_" + m_strSimulationID.ToString();
            String strSimulationTable = "SIMULATION_" + m_strNetworkID + "_" + m_strSimulationID;

            String strSelect = "SELECT MIN(YEARS) FROM " + strReportTable;
            DataSet ds = null;

            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    Global.WriteOutput("Error: Run Simulation before viewing Simulation Results");
                    return;
                }

                if (ds.Tables[0].Rows[0].ItemArray[0].ToString() == "")
                {

                    Global.WriteOutput("Error: Run Simulation before viewing Simulation Results");
                    return;
                }
                nMinimum = int.Parse(ds.Tables[0].Rows[0].ItemArray[0].ToString());
            }

            catch (Exception exception)
            {
                Global.WriteOutput("Error: Run Simulation before viewing Simulation Results." + exception.Message);
                return;
            }


            strSelect = "SELECT MAX(YEARS) FROM " + strReportTable;
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    Global.WriteOutput("Error: Run Simulation before viewing Simulation Results");
                    return;
                }
                nMaximum = int.Parse(ds.Tables[0].Rows[0].ItemArray[0].ToString());
            }

            catch (Exception exception)
            {
                Global.WriteOutput("Error: Run Simulation before viewing Simulation Results." + exception.Message);
                return;
            }

            dgvResultCommit.ColumnCount = 2 + (nMaximum - nMinimum + 1);
            object[] oRow = new object[2+nMaximum-nMinimum+1];

            for (int i = nMinimum; i <= nMaximum; i++)
            {
                dgvResultCommit.Columns[2 + i - nMinimum].HeaderText = i.ToString();
                dgvResultCommit.Columns[2 + i - nMinimum].Name = i.ToString();
                oRow[2+i-nMinimum] = "-";
            }
            _hashIDRow = new Hashtable();

            String strFrom = DBOp.BuildFromStatement(m_strNetworkID, m_strSimulationID, true);

			strSelect = "SELECT DISTINCT SECTION_" + m_strNetworkID + ".FACILITY, SECTION_" + m_strNetworkID + ".SECTION,SECTION_" + m_strNetworkID + ".SECTIONID" + strFrom;
			

			// Add Where Clause
			strSelect += GenerateWhereClause();

			try
			{
                //SqlDataReader dr =  DBMgr.CreateDataReader(strSelect);
                DataReader drWhere = new DataReader(strSelect);
				while( drWhere.Read() )
                {
					oRow[0] = drWhere[0].ToString();
					oRow[1] = drWhere[1].ToString();
 
                    int nRow = dgvResultCommit.Rows.Add(oRow);
					_hashIDRow.Add( drWhere[2].ToString(), nRow );
					dgvResultCommit.Rows[nRow].Tag = drWhere[2].ToString();
                }
				drWhere.Close();
			}
			catch (Exception exception)
			{
			    Global.WriteOutput("Error:Run Simulation before viewing Simulation Results." + exception.Message);
			    return;
			}

			//Mark Benefit/Costs
           
            m_hashBudgetYearView.Clear();
            strSelect = "SELECT SECTIONID,YEARS,TREATMENT,CONSEQUENCEID,COMMITORDER,ISCOMMITTED,NUMBERTREATMENT,BUDGET,COST_ FROM " + strReportTable;
            try
            {
                //SqlDataReader dr = DBMgr.CreateDataReader(strSelect);
                DataReader dr = new DataReader(strSelect);

                while (dr.Read())
                {
                    String strSectionID = dr["SECTIONID"].ToString();
                    String strYears = dr["YEARS"].ToString();
                    String strTreatment = dr["TREATMENT"].ToString();
                    String strConsequenceID = dr["CONSEQUENCEID"].ToString();
                    String strCommitOrder = dr["COMMITORDER"].ToString();
                    String strBudget = dr["BUDGET"].ToString();

                    float fCost = 0;
                    float.TryParse(dr["COST_"].ToString(), out fCost);

					bool bCommitted;
					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
							bCommitted = bool.Parse( dr["ISCOMMITTED"].ToString() );
							break;
						case "ORACLE":
							bCommitted = dr["ISCOMMITTED"].ToString() == "1";
							break;
						default:
							throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
						//break;
					}
                    int nNumberTreatment = int.Parse(dr["NUMBERTREATMENT"].ToString());

                    int nRow = -1;
                    if (nLastSection.ToString() == strSectionID)
                    {
                        nRow = nLastRow;
                    }
                    else
                    {
                        if (_hashIDRow.Contains(strSectionID))
                        {
                            nRow = (int)_hashIDRow[strSectionID];
                        }
                        else
                        {
                            continue;
                        }

                    }

                    if (!strTreatment.ToUpper().Contains("NO TREATMENT"))
                    {
                        dgvResultCommit.Rows[nRow].Cells[strYears].Style.BackColor = Color.YellowGreen;
                        dgvResultCommit.Rows[nRow].Cells[strYears].Value = strTreatment;
                    }

                    dgvResultCommit.Rows[nRow].Cells[strYears].Tag = strConsequenceID;
                    if (bCommitted)
                    {
                        dgvResultCommit.Rows[nRow].Cells[strYears].Style.BackColor = Color.LightCoral;
                        
                    }

                    if (nNumberTreatment > 0 && !bCommitted)
                    {
                        if (dgvResultCommit.Rows[nRow].Cells[strYears].Style.BackColor != Color.YellowGreen)
                        {
                            dgvResultCommit.Rows[nRow].Cells[strYears].Style.BackColor = Color.LightBlue;
                            dgvResultCommit.Rows[nRow].Cells[strYears].ToolTipText = "Feasible";
                        }

                    }


                    //Add to view total
                    Hashtable hashYearView;
                    float fSum = 0;
                    if (strBudget == "") continue;
                    if (!m_hashBudgetYearView.Contains(strBudget))
                    {
                        hashYearView = new Hashtable();
                        m_hashBudgetYearView.Add(strBudget, hashYearView);
                        
                    }
                    else
                    {
                        hashYearView = (Hashtable)m_hashBudgetYearView[strBudget];
                    }

                    if (hashYearView.Contains(strYears))
                    {
                        fSum = (float) hashYearView[strYears];
                        hashYearView.Remove(strYears);
                    }

                    fSum += fCost;
                    hashYearView.Add(strYears, fSum);
                }
                dr.Close();
            }
            catch (Exception exception)
            {
                Global.WriteOutput(exception.Message);
            }

            if (FormManager.GetSimulationAttributeWindow() != null)
            {
                FormManager.GetSimulationAttributeWindow().UpdateResultSummaryFacilitySection("", "");
            }

            if (FormManager.GetResultsBudgetWindow() != null)
            {
                FormManager.GetResultsBudgetWindow().UpdateBudgetGrid(m_hashBudgetYearView);
            }


            return;
     
        }

		private string GenerateWhereClause()
		{
			//if there's no facility filtering or advanced search filtering, we don't want to even have the WHERE keyword
			string whereClause = "";
			if( comboBoxRouteFacilty.Text != "All" )
			{
				//at this point we know we're at least filtering on facility
				if( textBoxAdvanceSearch.Text.Trim() != "" )
				{
					//at this point we know we're also filtring on an advanced search query, so we'll need the AND

					//dsmelser 2009.11.30
					//need to add parenthesis or this produces incorrect results
					//strSelect += " AND ";
					//strSelect += textBoxAdvanceSearch.Text.Trim();
					whereClause = " WHERE FACILITY='" + comboBoxRouteFacilty.Text + "' AND (" + textBoxAdvanceSearch.Text.Trim() + ")";
				}
				else
				{
					//at this point we know we're just doing facility filtering
					whereClause = " WHERE FACILITY='" + comboBoxRouteFacilty.Text + "'";
				}
			}
			else
			{
				if( textBoxAdvanceSearch.Text.Trim() != "" )
				{
					//at this point we know we're just doing advanced search filtering
					whereClause = " WHERE (" + textBoxAdvanceSearch.Text.Trim() + ")";
				}
			}

			return whereClause;
		}

        private void UpdateCommitted()
        {
            int nStartYear;
            int nPeriod;
            String strStart = cbStartYear.Text;
            String strPeriod = cbYears.Text;



            int.TryParse(strStart, out nStartYear);
            int.TryParse(strPeriod, out nPeriod);
            cbStartYear.Text = strStart;
            cbYears.Text = strPeriod;

            int nCount = dgvResultCommit.Columns.Count;

            for (int i = nCount - 1; i > 1; i--)
            {
                dgvResultCommit.Columns.RemoveAt(i);
            }

            dgvResultCommit.Rows.Clear();

            dgvResultCommit.ColumnCount = nPeriod + 2;
            dgvResultCommit.Columns[0].Name = "FACILITY";
            dgvResultCommit.Columns[0].ReadOnly = true;
            dgvResultCommit.Columns[1].Name = "SECTION";
            dgvResultCommit.Columns[1].ReadOnly = true;

            for (int n = 0; n < nPeriod; n++)
            {
                int nCurrentYear = nStartYear + n;
                dgvResultCommit.Columns[n + 2].Name = nCurrentYear.ToString();
            }

            String strSectionTable = "SECTION_" + m_strNetworkID;

            //this from is insufficient to handle advanced search
			//String strSelect = "SELECT FACILITY,SECTION,SECTIONID FROM " + strSectionTable;

			string selectClause = "SELECT SECTION_" + m_strNetworkID + ".FACILITY, SECTION_" + m_strNetworkID + ".SECTION, SECTION_" + m_strNetworkID + ".SECTIONID ";
			string fromClause = DBOp.BuildFromStatement( m_strNetworkID, m_strSimulationID, true );
			string whereClause = GenerateWhereClause();
			string query = selectClause + fromClause + whereClause;

			//String strJurisdiction = "";
            //if (strJurisdiction.Trim().Length > 0)
            //{
            //    strJurisdiction = " WHERE " + textBoxAdvanceSearch.Text;
            //}
            //strSelect += strJurisdiction;

			DataSet ds = null;

            try
            {
                ds = DBMgr.ExecuteQuery(query);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Advance search error: " + exception.Message);
                return;
            }

            Hashtable hashRowID = new Hashtable();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                String strFacility = row[0].ToString();
                String strSection = row[1].ToString();
                String strID = row[2].ToString();

                int nRow = dgvResultCommit.Rows.Add();
                DataGridViewRow dr = dgvResultCommit.Rows[nRow];
                dr.Tag = strID;

                hashRowID.Add(int.Parse(strID), nRow);

                dr.Cells[0].Value = strFacility;
                dr.Cells[1].Value = strSection;
            }


            int nLastYear = nStartYear + nPeriod - 1;
            string strSelect = "SELECT COMMITID,SECTIONID,YEARS,TREATMENTNAME FROM COMMITTED_ WHERE SIMULATIONID='" + m_strSimulationID + "' AND YEARS>='" + strStart.ToString() + "' AND YEARS <='" + nLastYear.ToString() + "' ";
            //TODO: Add JOIN for JURISDICTION

            ds = null;
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Committed Project error: " + exception.Message);
                return;
            }

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                String strCommitID = row[0].ToString();
                String strSectionID = row[1].ToString();
                String strYear = row[2].ToString();
                String strTreatment = row[3].ToString();
                int nID = int.Parse(strSectionID);
                int nRow;
                if (hashRowID.Contains(nID))
                {
                    nRow = (int)hashRowID[nID];
                }
                else
                {
                    continue;
                }



                DataGridViewRow dr = dgvResultCommit.Rows[nRow];
                dr.Cells[strYear].Tag = strCommitID;
                dr.Cells[strYear].Value = strTreatment;
                dr.Cells[strYear].Style.BackColor = Color.LightCoral;

            }
        }

        private void dgvResultCommit_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_bChange) return;
            if (m_bCommitted)
            {
                LoadCommittedGrid(e);
            }
            else
            {
                String strFacility = dgvResultCommit.Rows[e.RowIndex].Cells[0].Value.ToString();
                String strSection = dgvResultCommit.Rows[e.RowIndex].Cells[1].Value.ToString();

                String strID;
                if (dgvResultCommit.Rows[e.RowIndex].Tag == null)
                {
                    FormManager.GetSimulationAttributeWindow().UpdateResultSummaryFacilitySection(strFacility, strSection);
                    return;
                }
                strID = dgvResultCommit.Rows[e.RowIndex].Tag.ToString();

                if (FormManager.GetSimulationAttributeWindow() != null)
                {
                    if (e.ColumnIndex < 2)
                    {
                        FormManager.GetSimulationAttributeWindow().UpdateResultSummaryFacilitySection(strFacility, strSection);
                        return;
                    }
                    else
                    {
                        String strYear = dgvResultCommit.Columns[e.ColumnIndex].HeaderText;
                        int result = 0;
                        bool isYear = Int32.TryParse(strYear, out result);
                        if(isYear)
                        {
                            FormManager.GetSimulationAttributeWindow().UpdateResultSummary(strID, strYear, strFacility, strSection, m_dictionaryColumns);
                        }
                    }
                }
            }
        }

        private void LoadCommittedGrid(DataGridViewCellEventArgs e)
        {
            int nCol = e.ColumnIndex;
            int nRow = e.RowIndex;
            SimulationAttributes sa = new SimulationAttributes();

            DataGridViewRow row = dgvResultCommit.Rows[nRow];
            if (row.Cells[0].Value == null) return;

            sa.m_strFacility = row.Cells[0].Value.ToString();
            sa.m_strSection = row.Cells[1].Value.ToString();

            String strCommitID = "";
            if (row.Cells[nCol].Tag != null)
            {
                strCommitID = row.Cells[nCol].Tag.ToString();
            }
            if (FormManager.GetSimulationAttributeWindow() != null)
            {
                if (FormManager.GetSimulationAttributeWindow().Attributes != null)
                {
                    FormManager.GetSimulationAttributeWindow().Attributes.Clear();
                }
            }

            if (nCol > 1)
            {
                sa.m_strYear = dgvResultCommit.Columns[nCol].Name.ToString();
                if (strCommitID.Length > 0)
                {
                    if (row.Cells[nCol].Value != null)
                    {
                        sa.m_strTreatment = row.Cells[nCol].Value.ToString();
                        sa.m_strCommitID = strCommitID;

                        String strSelect = "SELECT YEARANY,YEARSAME,BUDGET,COST_ FROM COMMITTED_ WHERE COMMITID='" + strCommitID + "'";
                        DataSet ds = null;
                        try
                        {
                            ds = DBMgr.ExecuteQuery(strSelect);
                        }
                        catch (Exception exception)
                        {
                            Global.WriteOutput("Commit information load error:" + exception.Message);
                            sa.m_strTreatment = "";
                            sa.m_strBeforeAny = "";
                            sa.m_strBeforeSame = "";
                            sa.m_strBudget = "";
                            sa.m_strCost = "";
                        }

                        if (ds.Tables[0].Rows.Count == 1)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];
                            sa.m_strBeforeAny = dr[0].ToString();
                            sa.m_strBeforeSame = dr[1].ToString();
                            sa.m_strBudget = dr[2].ToString();
                            sa.m_strCost = dr[3].ToString();
                        }
                        else
                        {
                            sa.m_strTreatment = "";
                            sa.m_strBeforeAny = "";
                            sa.m_strBeforeSame = "";
                            sa.m_strBudget = "";
                            sa.m_strCost = "";
                        }


                        ds = null;

						strSelect = "SELECT ID_,ATTRIBUTE_,CHANGE_ FROM COMMIT_CONSEQUENCES WHERE COMMITID='" + strCommitID + "'";
                        try
                        {
                            ds = DBMgr.ExecuteQuery(strSelect);
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                CommitAttributeChange commitAttributeChange = new CommitAttributeChange();
                                commitAttributeChange.m_strID = dr[0].ToString();
                                commitAttributeChange.m_strAttribute = dr[1].ToString();
                                commitAttributeChange.m_strChange = dr[2].ToString();
                                FormManager.GetSimulationAttributeWindow().AddAttributeChange(commitAttributeChange);
                            }


                        }
                        catch (Exception exception)
                        {
                            Global.WriteOutput("Commit attribute change load error:" + exception.Message);
                        }
                    }
                }
                else
                {
                    sa.m_strTreatment = "";
                    sa.m_strBeforeAny = "";
                    sa.m_strBeforeSame = "";
                    sa.m_strBudget = "";
                    sa.m_strCost = "";
                }
            }
            sa.m_strSectionID = dgvResultCommit.Rows[nRow].Tag.ToString();
            sa.m_nCol = nCol;
            sa.m_nRow = nRow;
            sa.m_formResultCommit = this;
            if (FormManager.GetSimulationAttributeWindow() != null)
            {
                FormManager.GetSimulationAttributeWindow().InitializeSummary(sa);
                FormManager.GetSimulationAttributeWindow().InitializeAttributes();
                FormManager.GetSimulationAttributeWindow().SectionID = dgvResultCommit.Rows[e.RowIndex].Tag.ToString();
            }
        }

        private void dgvResultCommit_KeyUp(object sender, KeyEventArgs e)
        {
            if (!m_bCommitted) return;

            if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewCell cell in dgvResultCommit.SelectedCells)
                {
                    if (cell.Tag != null)
                    {
                        String strDelete = "DELETE FROM COMMITTED_ WHERE COMMITID='" + cell.Tag.ToString() + "'";
                        try
                        {
                            DBMgr.ExecuteNonQuery(strDelete);
                            cell.Tag = null;
                            cell.Value = "";
                            cell.Style.BackColor = Color.White;
                        }
                        catch (Exception exception)
                        {
                            Global.WriteOutput("Error deleting committed project. " + exception.Message);

                        }
                    }
                }
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in dgvResultCommit.SelectedCells)
            {
                if (cell.Tag != null)
                {
                    String strDelete = "DELETE FROM COMMITTED_ WHERE COMMITID='" + cell.Tag.ToString() + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strDelete);
                        cell.Tag = null;
                        cell.Value = "";
                        cell.Style.BackColor = Color.White;
                    }
                    catch (Exception exception)
                    {
                        Global.WriteOutput("Error deleting committed project. " + exception.Message);

                    }
                }
            }
        }

        private void Copy_Click(object sender, EventArgs e)
        {
			List<CommitCopy> committedToCopy = GetSelectedTreatments();
			Global.PushDataToClipboard( "CommittedList", committedToCopy );
        }

		private void btnCommitExport_Click( object sender, EventArgs e )
		{
			List<CommitCopy> committedToCopy = GetAllTreatments();
			SaveCommittedToFile( committedToCopy );
		}

		private void SaveCommittedToFile( List<CommitCopy> committedToCopy )
		{
			List<string> attributeColumns = GenerateCommitConsequnceColumns( committedToCopy );
			SaveFileDialog commitedSaveDialog = new SaveFileDialog();
			commitedSaveDialog.Filter = "Tab Seperated Text|*.txt";
			commitedSaveDialog.ValidateNames = true;
			commitedSaveDialog.DefaultExt = "txt";
			commitedSaveDialog.AddExtension = true;


			if( commitedSaveDialog.ShowDialog() == DialogResult.OK )
			{
				try
				{
					TextWriter committedSaveFileWriter = new StreamWriter( commitedSaveDialog.OpenFile() );

					string headerRow = "ROUTE\tSECTION\tDIRECTION\tBMP\tEMP\tTREATMENT\tYEAR\tYEARANY\tYEARSAME\tBUDGET\tCOST\tAREA\t";

					foreach( string attributeColumn in attributeColumns )
					{
						headerRow += attributeColumn + "\t";
					}

					committedSaveFileWriter.WriteLine( headerRow );

					foreach( CommitCopy committedProjectToSave in committedToCopy )
					{
						committedSaveFileWriter.WriteLine( GenerateCommittedString( committedProjectToSave, attributeColumns ));
					}

					//foreach( CommitCopy committedProjectToSave in committedToCopy )
					//{
					//    committedSaveFileWriter.WriteLine( GenerateCommittedString( committedProjectToSave, attributeColumns );
					//}

					committedSaveFileWriter.Flush();
					committedSaveFileWriter.Close();
				}
				catch( Exception ex )
				{
					Global.WriteOutput( "ERROR: could not save file: " + ex.Message );
				}
			}
		}

		private string GenerateCommittedString( CommitCopy committedProjectToSave, List<string> attributeColumns )
		{

			//ROUTE\tSECTION\tDIRECTION\tBMP\tEMP\tTREATMENT\tYEAR\tYEARANY\tYEARSAME\tBUDGET\tCOST\tAREA

			string committedString = "";

			committedString += GenerateLocationString( committedProjectToSave.Treatment.OriginalLocation );
			committedString += committedProjectToSave.Treatment.TreatmentName;
			committedString += "\t";
			committedString += committedProjectToSave.Treatment.TreatmentYear;
			committedString += "\t";
			committedString += committedProjectToSave.YearAny;
			committedString += "\t";
			committedString += committedProjectToSave.YearSame;
			committedString += "\t";
			committedString += committedProjectToSave.Budget;
			committedString += "\t";
			committedString += committedProjectToSave.OriginalCost;
			committedString += "\t";
			committedString += committedProjectToSave.Treatment.OriginalLocation.Area;
			committedString += "\t";
			
			foreach( string attributeColumn in attributeColumns )
			{
				foreach( AttributeChange consequence in committedProjectToSave.CommitConsequences )
				{
					if( consequence.Attribute.ToUpper() == attributeColumn.ToUpper() )
					{
						committedString += consequence.Change;
						break;
					}
				}
				committedString += "\t";
			}

			return committedString;
		}

		private string GenerateLocationString( LocationCopy locationToSerialize )
		{
			//ROUTE\tSECTION\tDIRECTION\tBMP\tEMP\tTREATMENT\tYEAR\tYEARANY\tYEARSAME\tBUDGET\tCOST
			string locationString = "";

			locationString += locationToSerialize.Route + "\t";
			locationString += locationToSerialize.Section + "\t";
			locationString += locationToSerialize.Direction + "\t";
			locationString += locationToSerialize.Start + "\t";
			locationString += locationToSerialize.End + "\t";

			return locationString;
		}

		private List<string> GenerateCommitConsequnceColumns( List<CommitCopy> committedToCopy )
		{
			List<string> attributeColumns = new List<string>();
			foreach( CommitCopy committedProject in committedToCopy )
			{
				foreach( AttributeChange consequence in committedProject.CommitConsequences )
				{
					if( !attributeColumns.Contains( consequence.Attribute ))
					{
						attributeColumns.Add( consequence.Attribute );
					}
				}
			}

			return attributeColumns;
		}

		private List<CommitCopy> GetSelectedTreatments()
		{
			return GetTreatments( true );
		}

		private List<CommitCopy> GetAllTreatments()
		{
			return GetTreatments( false );
		}

		private List<CommitCopy> GetTreatments( bool selected )
		{
			Regex linearTester = new Regex( @"^[0-9]+(\.[0-9]+)?-[0-9]+(\.[0-9]+)?\([A-Za-z]+\)$" );
			Regex numberExtractor = new Regex( @"[0-9]+(\.[0-9]+)?" );
			Regex directionExtractor = new Regex( @"(?<=\()[A-Za-z]+" );
			List<CommitCopy> selectedTreatments = new List<CommitCopy>();
			List<DataGridViewCell> cellsToSearch = new List<DataGridViewCell>();
			if( selected )
			{
				foreach( DataGridViewCell cellToAdd in dgvResultCommit.SelectedCells )
				{
					cellsToSearch.Add( cellToAdd );
				}
			}
			else
			{
				foreach( DataGridViewRow rowToSearch in dgvResultCommit.Rows )
				{
					foreach( DataGridViewCell cellToAdd in rowToSearch.Cells )
					{
						cellsToSearch.Add( cellToAdd );
					}
				}
			}
			foreach( DataGridViewCell cell in cellsToSearch )
			{
				if( cell.Value != null )
				{
					string treatment = cell.Value.ToString();
					if( treatment != "" )
					{
						string columnName = cell.DataGridView.Columns[cell.ColumnIndex].Name;

						if( columnName.ToUpper() != "FACILITY" && columnName.ToUpper() != "SECTION" )
						{
							int year;
							if( int.TryParse( columnName, out year ) )
							{
                                double cost = -1.0;
                                int yearSame = -1;
                                int yearAny = -1;
                                string budget = "";
                                string commitID = "";

								string route = cell.DataGridView.Rows[cell.RowIndex].Cells["FACILITY"].Value.ToString();
								string segment = cell.DataGridView.Rows[cell.RowIndex].Cells["SECTION"].Value.ToString();
								if( linearTester.IsMatch( segment ) )
								{


									//here we assume we're in a linear based system
									Match bmpMatch = numberExtractor.Match( segment, 0 );
									double bmp = double.Parse( bmpMatch.Value );
									Match empMatch = bmpMatch.NextMatch();
									double emp = double.Parse( empMatch.Value );
									string dir = directionExtractor.Match( segment ).Value;

									double area = GetArea( route, segment, dir, bmp, emp );

									LocationCopy treatmentLoc = new LocationCopy( route, dir, bmp, emp, area );
									TreatmentCopy treatmentToCopy = new TreatmentCopy( treatment, year, treatmentLoc );

									LookUpCommitData( treatmentLoc, year, out cost, out yearSame, out yearAny, out budget, out commitID );
									List<AttributeChange> commitConsequences = GetCommitConsequences( commitID );
									CommitCopy commitedTreatmentToCopy = new CommitCopy( treatmentToCopy, yearSame, yearAny, budget, commitConsequences, cost );

									selectedTreatments.Add( commitedTreatmentToCopy );
								}
								else
								{
									//here we assume we're in a section based system
                                    double area = GetArea(route, segment);
                                    LocationCopy treatmentLocation = new LocationCopy(route, segment, area);
                                    TreatmentCopy treatmentToCopy = new TreatmentCopy(treatment, year, treatmentLocation);
                                    LookUpCommitData(treatmentLocation, year, out cost, out yearSame, out yearAny, out budget, out commitID);
                                    List<AttributeChange> commitConsequences = GetCommitConsequences(commitID);
                                    CommitCopy commitedTreatmentToCopy = new CommitCopy(treatmentToCopy, yearSame, yearAny, budget, commitConsequences, cost);
                                    selectedTreatments.Add(commitedTreatmentToCopy);
								}
							}
							else
							{
								throw new Exception( "ERROR: could not parse [\"" + columnName + "\"] as integer." );
							}
						}
					}
				}
			}

			return selectedTreatments;
		}

		private double GetArea( string route, string section, string dir, double bmp, double emp )
		{
			double area = double.NaN;
			string areaQuery = "SELECT AREA FROM SECTION_" + m_strNetworkID + " WHERE FACILITY = '" + route + "' AND BEGIN_STATION = '" + bmp + "' AND END_STATION = '" + emp + "' AND DIRECTION = '" + dir + "'";
			if( !String.IsNullOrEmpty( section ) )
			{
				areaQuery += " AND SECTION = '" + section + "'";
			}

			DataSet areaData = DBMgr.ExecuteQuery( areaQuery );
			if( areaData.Tables.Count > 0 )
			{
				if( areaData.Tables[0].Rows.Count > 0 )
				{
					if( !double.TryParse( areaData.Tables[0].Rows[0]["AREA"].ToString(), out area ))
					{
						area = double.NaN;
					}
				}
			}

			return area;
		}

        /// <summary>
        /// Get Area for SRS sections.
        /// </summary>
        /// <param name="facility">Name of the facility</param>
        /// <param name="section">Name of section</param>
        /// <returns>Returns the area field.</returns>
        public double GetArea(String facility, String section)
        {
            double area = double.NaN;
            String query = "SELECT AREA FROM SECTION_" + m_strNetworkID  + " WHERE FACILITY = '" + facility + "' AND SECTION='" + section + "'";
            DataSet areaData = DBMgr.ExecuteQuery(query);
            if (areaData.Tables.Count > 0)
            {
                if (areaData.Tables[0].Rows.Count > 0)
                {
                    if (!double.TryParse(areaData.Tables[0].Rows[0]["AREA"].ToString(), out area))
                    {
                        area = double.NaN;
                    }
                }
            }
            return area;
        }


		private List<AttributeChange> GetCommitConsequences( string commitID )
		{
			List<AttributeChange> commitConsequences = new List<AttributeChange>();
			string commitConsequenceQuery = "SELECT ATTRIBUTE_, CHANGE_ FROM COMMIT_CONSEQUENCES WHERE COMMITID = '" + commitID + "'";
			DataSet commitConsequnceData = DBMgr.ExecuteQuery( commitConsequenceQuery );
			if( commitConsequnceData.Tables.Count > 0 )
			{
				foreach( DataRow commitConsequenceRow in commitConsequnceData.Tables[0].Rows )
				{
					AttributeChange commitConsequnce = new AttributeChange();
					commitConsequnce.Attribute = commitConsequenceRow["ATTRIBUTE_"].ToString();
					commitConsequnce.Change = commitConsequenceRow["CHANGE_"].ToString();
					commitConsequences.Add( commitConsequnce );
				}
			}

			return commitConsequences;
		}

		private void LookUpCommitData( LocationCopy treatmentLoc, int year, out double cost, out int yearSame, out int yearAny, out string budget, out string commitID )
		{
			cost = -1.0;
			yearSame = -1;
			yearAny = -1;
			budget = "";
			commitID = "";

			string sectionID = GetSectionID( treatmentLoc );
			string costQuery = "SELECT COST_, YEARSAME, YEARANY, BUDGET, COMMITID FROM COMMITTED_ WHERE SIMULATIONID = '" + m_strSimulationID + "' AND SECTIONID = '" + sectionID + "' AND YEARS = '" + year.ToString() + "'";
			DataSet costData = DBMgr.ExecuteQuery( costQuery );
			if( costData.Tables.Count > 0 )
			{
				if( costData.Tables[0].Rows.Count > 0 )
				{
					cost = double.Parse( costData.Tables[0].Rows[0]["COST_"].ToString() );
					yearSame = int.Parse( costData.Tables[0].Rows[0]["YEARSAME"].ToString() );
					yearAny = int.Parse( costData.Tables[0].Rows[0]["YEARANY"].ToString() );
					budget = costData.Tables[0].Rows[0]["BUDGET"].ToString();
					commitID = costData.Tables[0].Rows[0]["COMMITID"].ToString();
				}
			}
		}

 		private string GetSectionID( LocationCopy treatmentLoc )
		{
			string sectionID;
			string sectionIDQuery = "";

			if( treatmentLoc.Linear )
			{
				sectionIDQuery = "SELECT SECTIONID FROM SECTION_" + m_strNetworkID + " WHERE FACILITY = '" + treatmentLoc.Route + "' AND DIRECTION = '" + treatmentLoc.Direction + "' AND BEGIN_STATION = '" + treatmentLoc.Start.ToString() + "' AND END_STATION = '" + treatmentLoc.End.ToString() + "'";
			}
			else
			{
                sectionIDQuery = "SELECT SECTIONID FROM SECTION_" + m_strNetworkID + " WHERE FACILITY = '" + treatmentLoc.Route + "' AND SECTION ='" + treatmentLoc.Section + "'";
			}

			DataSet sectionIDData = DBMgr.ExecuteQuery( sectionIDQuery );
			if( sectionIDData.Tables.Count > 0 )
			{
				if( sectionIDData.Tables[0].Rows.Count > 0 )
				{
					if( sectionIDData.Tables[0].Rows.Count == 1 )
					{
						sectionID = sectionIDData.Tables[0].Rows[0]["SECTIONID"].ToString();
					}
					else
					{
						throw new Exception( "ERROR: Ambiguous section selection. [" + treatmentLoc.ToString() + "] resolved to multiple section IDs" );
					}
				}
				else
				{
					throw new Exception( "ERROR: [" + treatmentLoc.ToString() + "] resolved to no section IDs" );
				}
			}
			else
			{
				throw new Exception( "ERROR with query [" + sectionIDQuery + "]." );
			}

			return sectionID;
		}

        private void Paste_Click(object sender, EventArgs e)
        {
			
			if( Global.ClipboardHasData( "CommittedList" ) )
			{
				List<CommitCopy> committedToPaste = (List<CommitCopy>) Global.PopDataFromClipboard( "CommittedList" );
				if( committedToPaste != null )
				{
					//PasteCommittted( committedToPaste ); Not working with screen paste
                    PasteFromScreen(committedToPaste);
				}
				else
				{
					throw new Exception( "ERROR: could not retrieve list of committed projects." );
				}
			}

			UpdateView();
        }


        private void PasteFromScreen(List<CommitCopy> committedToPaste)
        {
            if (committedToPaste.Count != 1) return;

            List<CommitAttributeChange> listConsequences = new List<CommitAttributeChange>();
            foreach (AttributeChange ac in committedToPaste[0].CommitConsequences)
            {
                CommitAttributeChange cac = new CommitAttributeChange();
                cac.m_strAttribute = ac.Attribute;
                cac.m_strChange = ac.Change;
                listConsequences.Add(cac);
            }

            Global.CopyCommittedProject(committedToPaste[0].Treatment.TreatmentName,
                committedToPaste[0].YearAny.ToString(),
                committedToPaste[0].YearSame.ToString(),
                committedToPaste[0].OriginalCost.ToString(),
                committedToPaste[0].Budget,
                committedToPaste[0].Treatment.OriginalLocation.Area.ToString(),
                listConsequences);

            if (Global.CommitTreatmentName != null)
            {
                String strCommitID = "";
                foreach (DataGridViewCell cell in dgvResultCommit.SelectedCells)
                {
                    if (cell.ColumnIndex < 2) continue;

                    if (cell.Tag != null) //Update
                    {
                        String strDelete = "DELETE FROM COMMITTED_ WHERE COMMITID='" + cell.Tag.ToString() + "'";
                        try
                        {
                            DBMgr.ExecuteNonQuery(strDelete);
                            cell.Tag = null;
                            cell.Value = "";
                        }
                        catch (SqlException exception)
                        {
                            Global.WriteOutput("Error overwriting committed treatment: " + exception.Message);
                            return;
                        }
                    }

                    DataGridViewRow row = dgvResultCommit.Rows[cell.RowIndex];
                    String strSectionID = row.Tag.ToString();
                    String strYear = dgvResultCommit.Columns[cell.ColumnIndex].Name.ToString();

                    String strOutArea;
                    //String strOutUnit;
                    if (!GetSectionArea(strSectionID, out strOutArea))
                    {
                        Global.WriteOutput("Error: Calculating section area. Default area of 1 used");
                        strOutArea = "1";
                    }

                    float fInCost = 0;
                    float fOutCost = 0;
                    float.TryParse(Global.CommitCost, out fInCost);

                    Global.GetCost(fInCost, Global.CommitArea, strOutArea, out fOutCost);


                    String strInsert = "INSERT INTO COMMITTED_ (SIMULATIONID,SECTIONID,YEARS,TREATMENTNAME,YEARSAME,YEARANY,BUDGET,COST_) VALUES('" + m_strSimulationID + "','"
                                                + strSectionID + "','"
                                                + strYear + "','"
                                                + Global.CommitTreatmentName + "','"
                                                + Global.CommitYearSame + "','"
                                                + Global.CommitYearAny + "','"
                                                + Global.CommitBudget + "','"
                                                + fOutCost.ToString() + "')";

                    try
                    {
                        DBMgr.ExecuteNonQuery(strInsert);
                        String strIdentity;
                        switch (DBMgr.NativeConnectionParameters.Provider)
                        {
                            case "MSSQL":
                                strIdentity = "SELECT IDENT_CURRENT ('COMMITTED_') FROM COMMITTED_";
                                break;
                            case "ORACLE":
                                strIdentity = "SELECT COMMITTED_COMMITID_SEQ.CURRVAL FROM DUAL";
                                break;
                            default:
                                throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                                //break;
                        }
                        DataSet ds = DBMgr.ExecuteQuery(strIdentity);
                        strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        strCommitID = strIdentity;
                        dgvResultCommit.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Tag = strIdentity;
                        dgvResultCommit.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = Global.CommitTreatmentName;


                    }
                    catch (SqlException except)
                    {
                        Global.WriteOutput("Error copying new Committed Project:" + except.Message.ToString());
                        return;
                    }



                    foreach (CommitAttributeChange commitAttributeChange in Global.CommitAttributeChange)
                    {
                        strInsert = "INSERT INTO COMMIT_CONSEQUENCES (COMMITID,ATTRIBUTE_,CHANGE_) VALUES ('" + strCommitID + "','" + commitAttributeChange.m_strAttribute + "','" + commitAttributeChange.m_strChange + "')";
                        try
                        {
                            DBMgr.ExecuteNonQuery(strInsert);
                        }
                        catch (SqlException except)
                        {
                            Global.WriteOutput("Error copying new Attribute change pairs for committed Project:" + except.Message.ToString());
                            return;
                        }
                    }
                }
            }


        }






		private void btnCommitImport_Click( object sender, EventArgs e )
		{
            Global.WriteOutput("Begin committed project import.");
			List<CommitCopy> committedToPaste = GetSavedTreatments();
            Global.WriteOutput("Begin saving committed project to database.");
			PasteCommittted( committedToPaste );
            Global.WriteOutput("End saving committed project to database.");
		    Global.WriteOutput("Updating committed project view.");
            UpdateView();
		    Global.WriteOutput("Import committed project is complete.");
        }

		private List<CommitCopy> GetSavedTreatments()
		{
			List<string> attributeColumns = new List<string>();
			List<CommitCopy> savedTreatments = new List<CommitCopy>();
			OpenFileDialog commitedOpenDialog = new OpenFileDialog();
			commitedOpenDialog.Filter = "Tab Seperated Text|*.txt";
			commitedOpenDialog.ValidateNames = true;
            string lineToParse = null;
            if ( commitedOpenDialog.ShowDialog() == DialogResult.OK )
			{
				try
				{
					TextReader committedSaveFileReader = new StreamReader( commitedOpenDialog.OpenFile() );
					string headerRow = committedSaveFileReader.ReadLine();
					string[] columnHeaders = headerRow.Split( '\t' );

					//"ROUTE\tSECTION\tDIRECTION\tBMP\tEMP\tTREATMENT\tYEAR\tYEARANY\tYEARSAME\tBUDGET\tCOST\tAREA

					//12 entries before the commit columns start

					for( int i = 12; i < columnHeaders.Length; ++i )
					{
						if( !String.IsNullOrEmpty( columnHeaders[i] ))
						{
						attributeColumns.Add( columnHeaders[i] );
						}
					}


                    for ( lineToParse = committedSaveFileReader.ReadLine(); lineToParse != null; lineToParse = committedSaveFileReader.ReadLine() )
					{
						string[] entries = lineToParse.Split( '\t' );
						LocationCopy loc = new LocationCopy( entries[0], entries[1], entries[2], double.Parse(entries[3]), double.Parse(entries[4]), double.Parse(entries[11]) );
						TreatmentCopy treat = new TreatmentCopy( entries[5], int.Parse(entries[6]), loc );
						List<AttributeChange> commitConsequnces = new List<AttributeChange>();
						for( int i = 12; i < entries.Length; ++i )
						{
							if( !String.IsNullOrEmpty( entries[i] ))
							{
							AttributeChange consequence = new AttributeChange();
							consequence.Attribute = attributeColumns[i-12];
							consequence.Change = entries[i];

							commitConsequnces.Add( consequence );
							}
						}

						CommitCopy commit = new CommitCopy( treat, int.Parse(entries[8]), int.Parse(entries[7]), entries[9], commitConsequnces, double.Parse(entries[10]) );

						savedTreatments.Add( commit );						
					}


					committedSaveFileReader.Close();
				}
				catch( Exception ex )
				{
					Global.WriteOutput( "ERROR: could not read from file: " + ex.Message + " " + lineToParse);
					savedTreatments.Clear();
				}
			}

			return savedTreatments;

		}


		private void PasteCommittted( List<CommitCopy> committedToPaste )
		{
			//sectionid, year, project
			Dictionary<string, Dictionary<int, List<CommitCopy>>> candidateProjects = new Dictionary<string, Dictionary<int, List<CommitCopy>>>();
			Dictionary<string, Dictionary<int, CommitCopy>> decidedProjects = new Dictionary<string, Dictionary<int, CommitCopy>>();

			foreach( CommitCopy projectToPaste in committedToPaste )
			{
				int year = projectToPaste.Treatment.TreatmentYear;
				//1. Identifiy all the appropriate sections to which the project could apply in this network
				List<string> candidateSectionIDs = GetMatchingSectionIDs( projectToPaste.Treatment.OriginalLocation );

				//2. Add the project being pasted to a list of candidate projects for that section
				foreach( string sectionID in candidateSectionIDs )
				{
					if( !candidateProjects.ContainsKey( sectionID ) )
					{
						//candidateProjects[sectionID] = new List<CommitCopy>();
						candidateProjects.Add( sectionID, new Dictionary<int, List<CommitCopy>>() );
						candidateProjects[sectionID].Add( year, new List<CommitCopy>() );
					}
					else
					{
						if( !candidateProjects[sectionID].ContainsKey( year ) )
						{
							candidateProjects[sectionID].Add( year, new List<CommitCopy>() );
						}
					}

					candidateProjects[sectionID][year].Add( projectToPaste );
				}
			}

			//3. eliminate any sections that already have a committed project
			//List<string> sectionIDsToRemove = new List<string>();
			Dictionary<string, List<int>> sectionYearsToRemove = new Dictionary<string, List<int>>();
			foreach( string sectionID in candidateProjects.Keys )
			{
				foreach( int year in candidateProjects[sectionID].Keys )
				{
					if( HasCommittedProject( sectionID, year ) )
					{
						if( !sectionYearsToRemove.ContainsKey( sectionID ))
						{
							sectionYearsToRemove.Add( sectionID, new List<int>() );
						}
						sectionYearsToRemove[sectionID].Add( year );
						//sectionYearsToRemove.Add( sectionID, year );
					}
				}
			}

			//we need this loop because you can't alter IEnumerable
			//collections within a foreach loop
			foreach( string sectionIDtoRemove in sectionYearsToRemove.Keys )
			{
				foreach( int year in sectionYearsToRemove[sectionIDtoRemove] )
				{
					candidateProjects[sectionIDtoRemove].Remove( year );
				}				
			}


			//4. Choose the correct project among the candidates for each section and interpolate the cost
			//5. Create the committed projects in this simulation
			foreach( string sectionID in candidateProjects.Keys )
			{
				foreach( int year in candidateProjects[sectionID].Keys )
				{
                    if (candidateProjects[sectionID][year].Count == 1)
                    {
                        candidateProjects[sectionID][year][0].NewCost =
                            candidateProjects[sectionID][year][0].OriginalCost;
                        CreateCommittedProject(sectionID, candidateProjects[sectionID][year][0]);
                    }

                    else
                    {
                        CommitCopy bestProject = ChooseAndAdjustBestProject(sectionID, candidateProjects[sectionID][year]);
                        CreateCommittedProject(sectionID, bestProject);
                    }
				}
			}
		}

		private List<string> GetMatchingSectionIDs( LocationCopy locationCopy )
		{
			List<string> matchingSectionIDs = new List<string>();
			string sectionIDQuery = "SELECT SECTIONID FROM SECTION_" + m_strNetworkID + " WHERE FACILITY = '" + locationCopy.Route + "' AND DIRECTION = '" + locationCopy.Direction + "' AND " + locationCopy.End + " > BEGIN_STATION AND " + locationCopy.Start + " < END_STATION";

		    if (string.IsNullOrWhiteSpace(locationCopy.Direction))
		    {
		        sectionIDQuery = "SELECT SECTIONID FROM SECTION_" + m_strNetworkID + " WHERE FACILITY = '" +
		                         locationCopy.Route + "' AND SECTION ='" + locationCopy.Section + "'";
		    }


		    DataSet matchingSectionData = DBMgr.ExecuteQuery( sectionIDQuery );
			if( matchingSectionData.Tables.Count > 0 )
			{
				foreach( DataRow matchingSectionRow in matchingSectionData.Tables[0].Rows )
				{
					matchingSectionIDs.Add( matchingSectionRow["SECTIONID"].ToString() );
				}
			}
            
			return matchingSectionIDs;
		}

		private bool HasCommittedProject( string sectionID, int year )
		{
			bool hasCommitted = false;

			string committedQuery = "SELECT COUNT(COMMITID) FROM COMMITTED_ WHERE SIMULATIONID = '" + m_strSimulationID + "' AND SECTIONID = '" + sectionID + "' AND YEARS = '" + year.ToString() + "'";
			hasCommitted = DBMgr.ExecuteScalar( committedQuery ) > 0;

			return hasCommitted;
		}

		private CommitCopy ChooseAndAdjustBestProject( string sectionID, List<CommitCopy> candidateProjects )
		{
			CommitCopy bestProject = null;
			double longestProjectLength = -1.0;
			double sectionStart = -1.0;
			double sectionEnd = -1.0;
			double newArea = -1.0;

			string sectionQuery = "SELECT BEGIN_STATION, END_STATION, AREA FROM SECTION_" + m_strNetworkID + " WHERE SECTIONID = '" + sectionID + "'";
			DataSet sectionData = DBMgr.ExecuteQuery( sectionQuery );
			if( sectionData.Tables.Count > 0 )
			{
				if( sectionData.Tables[0].Rows.Count > 0 )
				{
					if( double.TryParse( sectionData.Tables[0].Rows[0]["AREA"].ToString(), out newArea ) )
					{
						if( double.TryParse( sectionData.Tables[0].Rows[0]["BEGIN_STATION"].ToString(), out sectionStart ) )
						{
							if( double.TryParse( sectionData.Tables[0].Rows[0]["END_STATION"].ToString(), out sectionEnd ) )
							{
								if( candidateProjects.Count > 1 )
								{

									foreach( CommitCopy candidateProject in candidateProjects )
									{
										double projectStart = candidateProject.Treatment.OriginalLocation.Start;
										double projectEnd = candidateProject.Treatment.OriginalLocation.End;
										double testLength = Math.Min( sectionEnd, projectEnd ) - Math.Max( sectionStart, projectStart );
										if( testLength > longestProjectLength )
										{
											longestProjectLength = testLength;
											bestProject = candidateProject;
										}
									}
								}
								else
								{
									if( candidateProjects.Count > 0 )
									{
										bestProject = candidateProjects[0];
										double projectStart = candidateProjects[0].Treatment.OriginalLocation.Start;
										double projectEnd = candidateProjects[0].Treatment.OriginalLocation.End;
										longestProjectLength = Math.Min( sectionEnd, projectEnd ) - Math.Max( sectionStart, projectStart );

									}
									else
									{
										throw new Exception( "ERROR: cannot choose best project among empty candidate list." );
									}

								}

								//bestProject.NewCost = bestProject.OriginalCost / longestProjectLength * ( sectionEnd - sectionStart );
								bestProject.NewCost = bestProject.OriginalCost / bestProject.Treatment.OriginalLocation.Area * newArea;

							}
						}
					}
				}
			}

			return bestProject;
		}

		private void CreateCommittedProject( string sectionID, CommitCopy projectToConvert )
		{
		    if (projectToConvert == null) return;

			//1. Insert into COMMITTED_
			string committedInsert = "INSERT INTO COMMITTED_ (SIMULATIONID, SECTIONID, YEARS, TREATMENTNAME, YEARSAME, YEARANY, BUDGET, COST_)" +
										"VALUES ('" + m_strSimulationID +
										"', '" + sectionID +
										"', '" + projectToConvert.Treatment.TreatmentYear +
										"', '" + projectToConvert.Treatment.TreatmentName +
										"', '" + projectToConvert.YearSame.ToString() +
										"', '" + projectToConvert.YearAny.ToString() +
										"', '" + projectToConvert.Budget +
										"', '" + projectToConvert.NewCost + "')";
			//DBMgr.ExecuteNonQuery( committedInsert );
			//committedInsert = "INSERT INTO COMMITTED_ (SIMULATIONID, SECTIONID, YEARS, TREATMENTNAME, YEARSAME, YEARANY, BUDGET, COST_)" +
			//                            "VALUES ('" + m_strSimulationID +
			//                            "', '" + sectionID +
			//                            "', '" + projectToConvert.Treatment.TreatmentYear +
			//                            "', '" + projectToConvert.Treatment.TreatmentName +
			//                            "', '" + projectToConvert.YearSame.ToString() +
			//                            "', '" + projectToConvert.YearAny.ToString() +
			//                            "', '" + projectToConvert.Budget +
			//                            "', '" + projectToConvert.NewCost + "')";
			DBMgr.ExecuteNonQuery( committedInsert );

			//2. Get the COMMITID of what we just inserted
			string commitID = "";
			string committedQuery = "SELECT COMMITID FROM COMMITTED_ WHERE " +
									"SIMULATIONID = '" + m_strSimulationID + "' AND " +
									"SECTIONID = '" + sectionID + "' AND " +
									"YEARS = '" + projectToConvert.Treatment.TreatmentYear + "'";
			//DataSet committedData = DBMgr.ExecuteQuery( committedQuery );
			//committedQuery = "SELECT COMMITID FROM COMMITTED_ WHERE " +
			//                        "SIMULATIONID = '" + m_strSimulationID + "' AND " +
			//                        "SECTIONID = '" + sectionID + "' AND " +
			//                        "YEARS = '" + projectToConvert.Treatment.TreatmentYear + "'";
			DataSet committedData = DBMgr.ExecuteQuery( committedQuery );
			if( committedData.Tables.Count > 0 )
			{
				if( committedData.Tables[0].Rows.Count > 0 )
				{
					commitID = committedData.Tables[0].Rows[0]["COMMITID"].ToString();
				}
			}

			//2. Insert into COMMIT_CONSEQUENCES as needed
			List<string> commitConsequnceInserts = new List<string>();
			foreach( AttributeChange commitConsequnce in projectToConvert.CommitConsequences )
			{
				commitConsequnceInserts.Add( "INSERT INTO COMMIT_CONSEQUENCES (COMMITID, ATTRIBUTE_, CHANGE_) " +
												"VALUES ('" +
												commitID +
												"', '" + commitConsequnce.Attribute +
												"', '" + commitConsequnce.Change + "')" );
			}

			DBMgr.ExecuteBatchNonQuery( commitConsequnceInserts );
		}


        private void FormSimulationResults_FormClosed(object sender, FormClosedEventArgs e)
        {
			FormUnload();

            if (m_bCommitted)
            {
                FormManager.RemoveFormSimulationCommitted(this);
            }
            else
            {
                FormManager.RemoveFormSimulationResults(this);
            }
        }

        private void comboBoxRouteFacilty_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_bChange = false;
            UpdateView();
            m_bChange = true;
        }

        private void FormSimulationResults_Activated(object sender, EventArgs e)
        {
            FormSimulationAttributes formSimulationAttributes;
            if (!FormManager.IsSimulationAttributeOpen(out formSimulationAttributes))
            {
				//dsmelser 2008.10.22
				//If formSimulationAttributes requires a simulation ID,
				//it should be given to it in it's constructor.
				formSimulationAttributes = new FormSimulationAttributes();
				formSimulationAttributes.SimulationID = m_strSimulationID;

				FormManager.AddSimulationAttributeWindow(formSimulationAttributes);

				formSimulationAttributes.IsCommitted = m_bCommitted;
				formSimulationAttributes.NetworkID = m_strNetworkID;
				formSimulationAttributes.SimulationID = m_strSimulationID;
				formSimulationAttributes.InitializeAttributes();
				formSimulationAttributes.Activate();
				
				formSimulationAttributes.Show( FormManager.GetDockPanel(), DockState.DockLeft );
            }

            formSimulationAttributes.IsCommitted = m_bCommitted;
            formSimulationAttributes.InitializeAttributes();

            if (!m_bCommitted)
            {

                FormResultsTarget formResultsTarget;
                if (!FormManager.IsResultsTargetOpen(out formResultsTarget))
                {
                    formResultsTarget = new FormResultsTarget();
                    FormManager.AddResultsTargetWindow(formResultsTarget);
                    formResultsTarget.TabText = "Target Results";
                    formResultsTarget.Text = "Target Results";
				}

                if (m_strSimulationID != formResultsTarget.SimulationID || m_strNetworkID != formResultsTarget.NetworkID)
                {
                    formResultsTarget.SimulationID = m_strSimulationID;
                    formResultsTarget.NetworkID = m_strNetworkID;
                    formResultsTarget.StartYear = m_nStart;
                    formResultsTarget.Period = m_nPeriod;
                    formResultsTarget.UpdateTargetGrid();
                }
				formResultsTarget.Show( FormManager.GetDockPanel(), DockState.DockBottom );

                




                FormResultsDeficient formResultsDeficient;
                if (!FormManager.IsResultsDeficientOpen(out formResultsDeficient))
                {
                    formResultsDeficient = new FormResultsDeficient();
                    FormManager.AddResultsDeficientWindow(formResultsDeficient);
                    formResultsDeficient.TabText = "Deficient Results";
                    formResultsDeficient.Text = "Deficient Results";
                }

                if (m_strSimulationID != formResultsDeficient.SimulationID || m_strNetworkID != formResultsDeficient.NetworkID)
                {
                    formResultsDeficient.SimulationID = m_strSimulationID;
                    formResultsDeficient.NetworkID = m_strNetworkID;
                    formResultsDeficient.StartYear = m_nStart;
                    formResultsDeficient.Period = m_nPeriod;
                    formResultsDeficient.UpdateDeficientGrid();
                }
				formResultsDeficient.Show( FormManager.GetDockPanel(), DockState.DockBottom );




                FormResultsBudget formResultsBudget;
                if (!FormManager.IsResultsBudgetOpen(out formResultsBudget))
                {
                    formResultsBudget = new FormResultsBudget();
                    FormManager.AddResultsBudgetWindow(formResultsBudget);
                    formResultsBudget.TabText = "Budget Results";
                    formResultsBudget.Text = "Budget Results";
                }
                if (m_strSimulationID != formResultsBudget.SimulationID || m_strNetworkID != formResultsBudget.NetworkID)
                {
                    formResultsBudget.SimulationID = m_strSimulationID;
                    formResultsBudget.NetworkID = m_strNetworkID;
                    formResultsBudget.StartYear = m_nStart;
                    formResultsBudget.Period = m_nPeriod;
                    formResultsBudget.UpdateBudgetGrid(m_hashBudgetYearView);
                }
				formResultsBudget.Show( FormManager.GetDockPanel(), DockState.DockBottom );
				formResultsBudget.Activate();

            }
 










        }

        public bool GetSectionArea(String strSectionID, out String strArea)
        {
            strArea = "0";
            String strSelect = "SELECT AREA FROM SECTION_" + m_strNetworkID + " WHERE SECTIONID='" + strSectionID + "'";
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

        private void buttonAdvancedSearch_Click(object sender, EventArgs e)
        {
            String strQuery = textBoxAdvanceSearch.Text;

            Hashtable hashSimulationAttributeYear = GetSimulationAttributeYear(m_strNetworkID, m_strSimulationID);
            FormAdvancedSearch formAdvancedSearch = new FormAdvancedSearch(m_strNetwork, m_strSimulationID, hashSimulationAttributeYear, strQuery);
            if (formAdvancedSearch.ShowDialog() == DialogResult.OK)
            {
                textBoxAdvanceSearch.Text = formAdvancedSearch.Query;
                UpdateView();
            }
        }

        /// <summary>
        /// Returns attributes and year hash from simulation.
        /// </summary>
        /// <param name="strNetworkID"></param>
        /// <returns></returns>

		private Hashtable GetSimulationAttributeYear( String strNetworkID, String strSimulationID )
		{
			Hashtable hashAttributeYear = new Hashtable();

			//String strSelect = "";
			//switch( DBMgr.NativeConnectionParameters.Provider )
			//{
			//    case "MSSQL":
			//        strSelect = "SELECT TOP 1 * FROM SIMULATION_" + strNetworkID + "_" + strSimulationID;
			//        break;
			//    case "ORACLE":
			//        strSelect = "SELECT * FROM SIMULATION_" + strNetworkID + "_" + strSimulationID + " WHERE ROWNUM = 1";
			//        break;
			//    default:
			//        throw new NotImplementedException( "TODO: Implement ANSI version of GetSimulationAttributeYear()" );
			//        break;
			//}

			List<string> simulationColumns = DBMgr.GetTableColumns( "SIMULATION_" + strNetworkID + "_" + strSimulationID );

			//try
			//{
			//    DataSet ds = DBMgr.ExecuteQuery(strSelect);
			//    foreach (DataColumn dc in ds.Tables[0].Columns)
			foreach( string strColumn in simulationColumns )
			{
				//String strColumn = dc.ToString();
				if( strColumn != "SECTIONID" )
				{
					string[] columns = strColumn.Split( '_' );
					int nYear = 0;                  //WTF IS THIS? ->{
					int.TryParse( columns[columns.Length - 1], out nYear );
					if( nYear == 0 )
						continue;


					String strAttribute = strColumn.Substring( 0, strColumn.Length - columns[columns.Length - 1].Length - 1 );
					List<String> listYear;
					if( !hashAttributeYear.Contains( strAttribute ) )
					{
						listYear = new List<String>();
						hashAttributeYear.Add( strAttribute, listYear );
					}
					else
					{
						listYear = ( List<String> )hashAttributeYear[strAttribute];
					}
					listYear.Add( nYear.ToString() );
				}
			}
			//}
			//catch(Exception exception)
			//{
			//    //this is a bad way to handle this
			//    //returning null here severely screws up other code
			//    //Global.WriteOutput("Error in retrieving simulation results for Network:" + m_strNetwork + " Simulation:" + m_strSimulation + "." + exception.Message);
			//    //return null;

			//    throw new ArgumentException( "Error in retrieving simulation results for Network:" + m_strNetwork + " Simulation:" + m_strSimulation + ".  " + exception.Message );
			//}


			return hashAttributeYear;
		}

        private void commitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //bool bUpdate = false;//Tells the Committed page to update.  Initiallly false.
            foreach (DataGridViewCell cell in dgvResultCommit.SelectedCells)
            {
                int nRow = cell.RowIndex;//Get the highlighted cell.
                int nColumn = cell.ColumnIndex;// Get the highlighted row
                if (nColumn > 1)//Don't commit first two columns
                {
                    String strTreatmentID = cell.Tag.ToString();//Get the tag of the cell which is the TreatmentID
                    if (strTreatmentID != "")
                    {
                        CommitCell(nRow, nColumn);
                    }
                }
            }
        }

        private bool CommitCell(int nRow, int nColumn)
        {

            String strSectionID = dgvResultCommit.Rows[nRow].Tag.ToString();
            String strYear = dgvResultCommit.Columns[nColumn].Name.ToString();
            String strSelect = "SELECT TREATMENT,YEARSSAME,YEARSANY,BUDGET,COST_,CHANGEHASH FROM REPORT_" + m_strNetworkID + "_" + m_strSimulationID +
                                " WHERE SECTIONID='" + strSectionID + "' AND YEARS='" + strYear + "'";

            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                DataRow row = ds.Tables[0].Rows[0];
                String strTreatment = row["TREATMENT"].ToString();
                String strYearsSame = row["YEARSSAME"].ToString();
                String strYearsAny = row["YEARSANY"].ToString();
                String strBudget = row["BUDGET"].ToString();
                String strCost = row["COST_"].ToString();
                String strChangeHash = row["CHANGEHASH"].ToString();

                String strDelete = "DELETE FROM COMMITTED_ WHERE SIMULATIONID='" + m_strSimulationID + "' AND SECTIONID='" + strSectionID + "' AND YEARS='" + strYear + "'";
                DBMgr.ExecuteNonQuery(strDelete);
                
                String strInsert = "INSERT INTO COMMITTED_ (SIMULATIONID,SECTIONID,YEARS,TREATMENTNAME,YEARSAME,YEARANY,BUDGET,COST_)"
                                    + " VALUES ('" + m_strSimulationID + "','" + strSectionID + "','" + strYear + "','" + strTreatment + "','" + strYearsSame + "','" + strYearsAny + "','" + strBudget + "','" + strCost + "')";

                DBMgr.ExecuteNonQuery(strInsert);
				String strIdentity;
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
                ds = DBMgr.ExecuteQuery(strIdentity);
                strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();

                string[] pairs = strChangeHash.Split(new string[] { "\n" }, StringSplitOptions.None);
                for (int i = 0; i < pairs.Length; i++)
                {
                    if (pairs[i].Contains("\t"))
                    {
                        string[] attributechange = pairs[i].Split(new string[] { "\t" }, StringSplitOptions.None);
                        strInsert = "INSERT INTO COMMIT_CONSEQUENCES (COMMITID,ATTRIBUTE_,CHANGE_)"
                            + " VALUES ('" + strIdentity + "','" + attributechange[0] + "','" + attributechange[1] + "')";

                        DBMgr.ExecuteNonQuery(strInsert);
                    }
                }

                dgvResultCommit[nColumn, nRow].Style.BackColor = Color.LightCoral;
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error committing section: " + dgvResultCommit[1, nRow].Value.ToString() + "-" + dgvResultCommit[2, nRow].Value.ToString() + " for year: " + strYear + ". " + exception.Message);
                return false;
            }


            return true;
        }



        private void dgvResultCommit_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            int i = 0;
            i++;
        }

        private void dgvResultCommit_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (m_bNoToolTip) return;
            if (dgvResultCommit[e.ColumnIndex, e.RowIndex].Style.BackColor != Color.LightBlue) return;
            String strID = dgvResultCommit.Rows[e.RowIndex].Tag.ToString();
            String strYear = dgvResultCommit.Columns[e.ColumnIndex].Name;
            String strBenefit = "BENEFITCOST_" + m_strNetworkID + "_" + m_strSimulationID;
            String strSection = "SECTION_" + m_strNetworkID;


            String strSelect = "SELECT TREATMENT, BUDGET, COST_ *AREA AS TOTALCOST FROM " + strBenefit + " INNER JOIN " + strSection + " ON " + strSection + ".SECTIONID=" +strBenefit + ".SECTIONID WHERE "+strBenefit + ".SECTIONID=" + strID + " AND YEARS=" + strYear;

			float inflationRate = CalculateTotalInflationRateForYear( int.Parse( strYear ) );

            try
            {

                String strToolTip = "";
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
					String strRow = dr["TREATMENT"].ToString() + " (" + dr["BUDGET"].ToString() + ") " + (float.Parse( dr["TOTALCOST"].ToString() ) * inflationRate).ToString( "C" ) + "\n";
                    strToolTip += strRow;
                }
                if (strToolTip.Length > 0) strToolTip = strToolTip.Substring(0, strToolTip.Length - 1);
                dgvResultCommit[e.ColumnIndex, e.RowIndex].ToolTipText = strToolTip;
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Retrieving feasibility information for tooltip." + exception.Message);

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
				rate = (float)Math.Pow( rate, currentYear - startYear );
			}
			catch( Exception ex )
			{
				Global.WriteOutput( "ERROR [could not retrieve inflation information]: " + ex.Message );
			}

			return rate;
		}

		private void dgvResultCommit_CellDoubleClick( object sender, DataGridViewCellEventArgs e )
		{

			if( !m_bCommitted )
			{
				if( Global.SecurityOperations.CanModifySimulationResults( m_strNetworkID, m_strSimulationID ) )
				{
					if( e.ColumnIndex < 2 || e.RowIndex < 0 )
						return;

					String strID = dgvResultCommit.Rows[e.RowIndex].Tag.ToString();
					String strYear = dgvResultCommit.Columns[e.ColumnIndex].Name;
					double inflation = CalculateTotalInflationRateForYear( int.Parse( strYear ) );
					FormSimulationRunSingleSection form = new FormSimulationRunSingleSection( strID, strYear, m_strNetworkID, m_strSimulationID, inflation );
					form.m_strSimulation = m_strSimulation;
					form.m_strNetwork = m_strNetwork;

					form.StartPosition = FormStartPosition.CenterParent;
					if( form.ShowDialog() == DialogResult.OK )
					{
						dgvResultCommit.Enabled = false;
						m_bNoToolTip = true;
						m_nRowSingle = e.RowIndex;
						m_nColSingle = e.ColumnIndex;
						m_bCommitted = form.m_bCommit;
						this.Cursor = Cursors.WaitCursor;

						Global.ClearOutputWindow();
						if( m_simulation == null )
						{
							m_simulation = new Simulation.Simulation( m_strSimulation, m_strNetwork, m_strSimulationID, m_strNetworkID );
						}
						m_simulation.SingleSectionID = strID;
						m_simulation.SingleSectionYear = strYear;
						m_simulation.SingleSection = form.m_singleSection;

						timerSimulation.Start();
						m_simulationThread = new Thread( new ThreadStart( m_simulation.SingleSectionSimulation ) );
						m_simulationThread.Start();

					}
				}
			}
		}

        private void timerSimulation_Tick(object sender, EventArgs e)
        {
            List<Simulation.SimulationMessage> listSimulation = Simulation.SimulationMessaging.GetProgressList();
            lock (listSimulation)
            {
                Simulation.SimulationMessage endMessage = listSimulation.Find(delegate(Simulation.SimulationMessage sm) { return sm.Message.Contains("End Single Section Run."); });

				if (endMessage != null)
				{
					dgvResultCommit.Enabled = true;
				}
                String strOut = "";
                foreach (Simulation.SimulationMessage message in listSimulation)
                {
                    strOut += message.Message + "\r\n";
                }
                if (strOut.Length > 0)
                {
                    strOut = strOut.Substring(0, strOut.Length - 2);
                    Global.WriteOutput(strOut);
                }
            }
            Simulation.SimulationMessaging.ClearProgressList();
            if (!m_simulationThread.IsAlive)
            {

                m_simulationThread = null;
                GC.Collect();               
                timerSimulation.Stop();
                this.Cursor = Cursors.Default;
                //if (m_bCommitted)
                //{
                //    CommitCell(m_nRowSingle, m_nColSingle);
                //}
                m_bNoToolTip = false;
                UpdateView();
            }
        }

		private void btnARAN_Click(object sender, EventArgs e)
		{
			if (dgvResultCommit.SelectedCells.Count == 1)
			{
				string videoLog = Environment.GetEnvironmentVariable("videolog").ToString();

				string routeID = "";
				string beginMP = "";
				string endMP = "";

				string sectionID = dgvResultCommit.Rows[dgvResultCommit.SelectedCells[0].RowIndex].Tag.ToString();
				string query = "SELECT sec.BEGIN_STATION, sec.END_STATION, seg.AA_ROUTE_ID_COND FROM SECTION_" + m_strNetworkID + " sec INNER JOIN SEGMENT_" + m_strNetworkID + "_NS0 seg ON sec.SECTIONID = seg.SECTIONID WHERE sec.SECTIONID = '" + sectionID + "'";

				DataSet routeData = DBMgr.ExecuteQuery(query);
				foreach (DataRow dr in routeData.Tables[0].Rows)
				{
					routeID = dr["AA_ROUTE_ID_COND"].ToString();
					beginMP = dr["BEGIN_STATION"].ToString();
					endMP = dr["END_STATION"].ToString();
				}
				Global.WriteOutput(videoLog + "?routeId=" + routeID + "&mp=" + beginMP + "&dir=5");
				Process.Start(videoLog + "?routeId=" + routeID + "&mp=" + beginMP + "&dir=5");
			}
			else
			{
				Global.WriteOutput("Multiple Rows found!");
			}
		}

        private void Cut_Click(object sender, EventArgs e)
        {
            List<CommitCopy> committedToCopy = GetSelectedTreatments();
            Global.PushDataToClipboard("CommittedList", committedToCopy);
            foreach (DataGridViewCell cell in dgvResultCommit.SelectedCells)
            {
                if (cell.Tag != null)
                {
                    String strDelete = "DELETE FROM COMMITTED_ WHERE COMMITID='" + cell.Tag.ToString() + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strDelete);
                        cell.Tag = null;
                        cell.Value = "";
                        cell.Style.BackColor = Color.White;
                    }
                    catch (Exception exception)
                    {
                        Global.WriteOutput("Error deleting committed project. " + exception.Message);

                    }
                }
            }


        }

        private void displayAttributesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> tableColumnNames = DBMgr.GetTableColumns("SIMULATION_" + m_strNetworkID + "_" + m_strSimulationID);
            
            // Dont show SECTIONID, internal to RoadCare.
            if (tableColumnNames.Contains("SECTIONID"))
            {
                tableColumnNames.Remove("SECTIONID");
            }
            List<string> attributesShownInGrid = new List<string>();
            foreach(DataGridViewColumn column in dgvResultCommit.Columns)
            {
                if (tableColumnNames.Contains(column.HeaderText))
                {
                    attributesShownInGrid.Add(column.HeaderText);
                }
            }
            attributesShownInGrid.Sort();
            FormAttributeResults formAttributeResults = new FormAttributeResults(tableColumnNames, attributesShownInGrid);
            if (formAttributeResults.ShowDialog() == DialogResult.OK)
            {
                // Add the column to the grid if its not already in the grid and it was selected for insertion.
                // If not selected for insertion, check to see if its in the grid.  If it is, remove it.
                foreach (string columnHeader in formAttributeResults.checkedListBoxAttributes.Items)
                {
                    if (!attributesShownInGrid.Contains(columnHeader) && formAttributeResults.checkedListBoxAttributes.CheckedItems.Contains(columnHeader))
                    {
                        // Add the column to the results grid.
                        DataGridViewColumn toInsert = new DataGridViewColumn();
                        toInsert.Name = columnHeader;
                        toInsert.HeaderText = columnHeader;
                        toInsert.CellTemplate = new DataGridViewTextBoxCell();
                        dgvResultCommit.Columns.Insert(2, toInsert);
                    }
                    if (attributesShownInGrid.Contains(columnHeader) && !formAttributeResults.checkedListBoxAttributes.CheckedItems.Contains(columnHeader))
                    {
                        dgvResultCommit.Columns.Remove(columnHeader);
                    }
                }
                string query = "SELECT SECTIONID, ";
                foreach (string attributeShown in formAttributeResults.checkedListBoxAttributes.CheckedItems)
                {
                    query += attributeShown + ", ";
                }
                query = query.Substring(0, query.LastIndexOf(','));
                query += " FROM SIMULATION_" + m_strNetworkID + "_" + m_strSimulationID + " ORDER BY SECTIONID";

                try
                {
                    DataSet dsShownAttributeData = DBMgr.ExecuteQuery(query);
                    foreach (DataRow dr in dsShownAttributeData.Tables[0].Rows)
                    {
                        // Skip over SECTIONID...should we even show SECTIONID as a choice? Its totally internal to RoadCare.
                        for (int i = 1; i < dr.ItemArray.Length; i++)
                        {
                            string sectionID = dr["SECTIONID"].ToString();
                            string columnHeaderText = dgvResultCommit.Columns[i + 1].HeaderText;
                            string cellData = dr[columnHeaderText].ToString();
                            int rowID = (int)_hashIDRow[sectionID];
                            dgvResultCommit.Rows[rowID].Cells[columnHeaderText].Value = cellData;
                        }
                    }
                }
                catch (Exception exc)
                {
                    Global.WriteOutput("Error: " + exc.Message);
                }
            }
        }


    
    }
}
