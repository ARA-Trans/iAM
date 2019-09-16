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
    public partial class FormResultsBudget : ToolWindow
    {
        private String m_strSimulationID;
        private String m_strNetworkID;
        private int m_nStart;
        private int m_nPeriod;

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

        public int StartYear
        {
            get { return m_nStart; }
            set { m_nStart = value; }
        }

        public int Period
        {
            get { return m_nPeriod; }
            set { m_nPeriod = value; }
        }

        public FormResultsBudget()
        {
            InitializeComponent();
        }

        private void FormResultsBudget_Load(object sender, EventArgs e)
        {
			SecureForm();
        }

		protected void SecureForm()
		{
			LockDataGridView( dgvBudget );
			if( Global.SecurityOperations.CanModifySimulationResults( m_strNetworkID, m_strSimulationID ) )
			{
				UnlockDataGridViewForCreateDestroy( dgvBudget );
			}
		}

        public void UpdateBudgetGrid(Hashtable hashBudgetYearView)
        {

            dgvBudget.Rows.Clear();
            dgvBudget.Columns.Clear();
            if (Period == 0) return;

            dgvBudget.ColumnCount = 2 + this.Period;
            for (int i = 2; i < Period + 2; i++)
            {
                int nYear = StartYear + i - 2;
                dgvBudget.Columns[i].HeaderText = nYear.ToString();
                dgvBudget.Columns[i].DefaultCellStyle.Format = "c";

            }
            dgvBudget.Columns[0].HeaderText = "Budget";
            int nRow;

            String strSelect = "SELECT BUDGETORDER FROM INVESTMENTS WHERE SIMULATIONID='" + SimulationID + "'";
            try
            {
                object[] totalSpent = new object[Period+2];
                object[] totalTarget = new object[Period+2];
                object[] totalView = new object[Period + 2];
                for (int i = 2; i < Period+2; i++)
                {
                    totalSpent[i] = 0.0f;
                    totalTarget[i] = 0.0f;
                    totalView[i] = 0.0f;
                }
                totalView[0] = "Total";
                totalView[1] = "View";
                totalTarget[0] = "Total";
                totalTarget[1] = "Target";
                totalSpent[0] = "Total";
                totalSpent[1] = "Spent";



                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    string[] budgets = ds.Tables[0].Rows[0].ItemArray[0].ToString().Split(',');
                    

                    foreach (string str in budgets)
                    {
                        object[] amounts = new object[Period + 2];
                        
                        Hashtable hashYearView = null;
                        if (hashBudgetYearView.Contains(str))
                        {
                            hashYearView = (Hashtable)hashBudgetYearView[str];
                        }
                        amounts[0] = str;
                        amounts[1] = "View";
                        for (int n = 0; n < Period; n++)
                        {
                            int nYear = n + StartYear;

                            if (hashYearView != null)
                            {
                                if (hashYearView.Contains(nYear.ToString()))
                                {
                                    float fView = (float)hashYearView[nYear.ToString()];
                                    amounts[n + 2] = fView;
                                    totalView[n+2] = (float)totalView[n+2] + fView;
                                }
                                else
                                {
                                    amounts[n+2] = 0;
                                }
                            }
                        }
                        nRow = dgvBudget.Rows.Add(amounts);
                        dgvBudget.Rows[nRow].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        dgvBudget.Rows[nRow].DefaultCellStyle.BackColor = Color.LightGray;
                        dgvBudget.Rows[nRow].ReadOnly = true;               
                                        
                        
                        
                        
                        
                        amounts[0] = str;
                        amounts[1] = "Target";

                        strSelect = "SELECT AMOUNT FROM YEARLYINVESTMENT WHERE SIMULATIONID='" + SimulationID + "' AND BUDGETNAME='" + str + "' ORDER BY YEAR_";
                        DataSet dsAmount = DBMgr.ExecuteQuery(strSelect);
                        int nCount = 2;
                                                foreach (DataRow rowAmount in dsAmount.Tables[0].Rows)
                        {
                            float f = 0;
                            float.TryParse(rowAmount[0].ToString(), out f);
                            amounts[nCount] = f;
                            totalTarget[nCount] = f + (float)totalTarget[nCount];
                            nCount++;
                        }
                        nRow =   dgvBudget.Rows.Add(amounts);
                        dgvBudget.Rows[nRow].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        dgvBudget.Rows[nRow].DefaultCellStyle.BackColor = Color.LightGray;
                        dgvBudget.Rows[nRow].ReadOnly = true;
                        
                        amounts.Initialize();
                        amounts[0] = str;
                        amounts[1] = "Spent";

                        List<int> listOverBudget = new List<int>();
                        for (int n = 0; n < Period; n++)
                        {
                            int nYear = n + StartYear;
                            strSelect = "SELECT SUM(COST_) FROM REPORT_" +  NetworkID + "_" + SimulationID + " WHERE YEARS='" + nYear.ToString() + "' AND BUDGET='" + str + "'";
                            DataSet dsSpent = DBMgr.ExecuteQuery(strSelect);

                            float f = 0;
                            float.TryParse(dsSpent.Tables[0].Rows[0].ItemArray[0].ToString(), out f);

                            float fTarget = 0;
                            if (dgvBudget.Rows[nRow].Cells[n + 2].Value != null)
                            {
                                float.TryParse(dgvBudget.Rows[nRow].Cells[n + 2].Value.ToString(), out fTarget);
                            }
                            else
                            {
                                fTarget = 0;
                            }
                            if (f > fTarget)
                            {
                                listOverBudget.Add(n);

                            }
                            amounts[n + 2] = f;
                            totalSpent[n+2] = f + (float) totalSpent[n+2];
                        }
                        nRow = dgvBudget.Rows.Add(amounts);
                        dgvBudget.Rows[nRow].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        foreach (int n in listOverBudget)
                        {
                            dgvBudget.Rows[nRow].Cells[n+2].Style.ForeColor = Color.Red;

                        }
                    }
                }

                nRow = dgvBudget.Rows.Add(totalView);
                dgvBudget.Rows[nRow].DefaultCellStyle.BackColor = Color.LightGray;
                dgvBudget.Rows[nRow].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                

                nRow = dgvBudget.Rows.Add(totalTarget);
                dgvBudget.Rows[nRow].DefaultCellStyle.BackColor = Color.LightGray;
                dgvBudget.Rows[nRow].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                
                
                
                
                
                nRow = dgvBudget.Rows.Add(totalSpent);
                dgvBudget.Rows[nRow].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                for (int i = 2; i < Period + 2; i++)
                {
                    if ((float)totalSpent[i] > (float)totalTarget[i])
                    {
                        dgvBudget.Rows[nRow].Cells[i].Style.ForeColor = Color.Red;
                    }

                }


            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error in initializing investments." + exception.Message);

            }

           











        }

        private void FormResultsBudget_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormManager.RemoveResultsBudgetWindow();
        }





    }
}
