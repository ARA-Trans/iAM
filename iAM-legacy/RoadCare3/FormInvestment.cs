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
using System.Runtime.InteropServices;
using System.Security.Policy;
using RoadCare3.Properties;
using RoadCareDatabaseOperations;

namespace RoadCare3
{
    public partial class FormInvestment : BaseForm
    {
        private String m_strNetwork;
		//dsmelser
		//need networkID for security
		private String m_strNetworkID;
        private String m_strSimulation;
        private String m_strSimID;
        private bool m_bUpdate = false;
        private bool m_bChange = false;
        private object m_oLastCell;
        private String m_strInflation;
        private String m_strDiscout;
        private String m_strStartYear;
        private List<string> Budgets = new List<string>();
        private Hashtable m_hashAttributeYear;

        public FormInvestment(String strNetwork, String strSimulation,String strSimID, Hashtable hashAttributeYear)
        {
            m_strNetwork = strNetwork;
			m_strNetworkID = DBOp.GetNetworkIDFromName( strNetwork );
            m_strSimulation = strSimulation;
            m_strSimID = strSimID;
            m_hashAttributeYear = hashAttributeYear;
            InitializeComponent();

        }

        private void FormInvestment_Load(object sender, EventArgs e)
        {
			SecureForm();

			FormLoad(Settings.Default.SIMULATION_IMAGE_KEY, Settings.Default.SIMULATION_IMAGE_KEY_SELECTED);

            labelInvestment.Text = "Investment " + m_strSimulation + " : " + m_strNetwork;
            DataSet ds = new DataSet();

            String strSelect = "SELECT FIRSTYEAR,NUMBERYEARS,INFLATIONRATE,DISCOUNTRATE,BUDGETORDER FROM INVESTMENTS WHERE SIMULATIONID='" + m_strSimID + "'";
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);

            }
            catch (Exception except)
            {
                Global.WriteOutput("Error: " + except.Message);
                return;
            }

            String strFirstYear = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            textBoxStartYear.Text = strFirstYear;
            cbYears.Text = ds.Tables[0].Rows[0].ItemArray[1].ToString();
            textBoxInflation.Text = ds.Tables[0].Rows[0].ItemArray[2].ToString();
            textBoxDiscount.Text = ds.Tables[0].Rows[0].ItemArray[3].ToString();
            textBoxBudgetOrder.Text = ds.Tables[0].Rows[0].ItemArray[4].ToString();
            m_bUpdate = true;
            UpdateInvestmentGrid();
            UpdateBudgetCriteria();

        }

		protected void SecureForm()
		{
			LockComboBox( cbYears );
			LockTextBox( textBoxStartYear );
			LockTextBox( textBoxInflation );
			LockTextBox( textBoxDiscount );
			LockButton( buttonEditOrder );
			LockDataGridView( dgvBudget );
				
			if( Global.SecurityOperations.CanModifySimulationInvestment( m_strNetworkID, m_strSimID ) )
			{
				UnlockComboBox( cbYears );
				UnlockTextBox( textBoxStartYear );
				UnlockTextBox( textBoxInflation );
				UnlockTextBox( textBoxDiscount );
				UnlockButton( buttonEditOrder );
				UnlockDataGridViewForModify( dgvBudget );
			}
		}

        private void UpdateInvestmentGrid()
        {
            dgvBudget.Rows.Clear();
            dgvBudget.Columns.Clear();
            dataGridViewBudgetCriteria.Columns[1].ReadOnly = true;
            m_bChange = false;

            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.HeaderText = "Years";
            column.ReadOnly = true;
            dgvBudget.Columns.Add(column);

            String strYear = textBoxStartYear.Text;
            int nStartYear = int.Parse(strYear);

            String strNumberYear = cbYears.Text;
            int nNumberYear = int.Parse(strNumberYear);

            string[] listBudgets = textBoxBudgetOrder.Text.Split(',');

            Budgets.Clear();

            foreach (string str in listBudgets)
            {
                column = new DataGridViewTextBoxColumn();
                column.HeaderCell.Value = str;
                column.Tag = str;
                column.Name = str;
                int nCol = dgvBudget.Columns.Add(column);
                dgvBudget.Columns[nCol].DefaultCellStyle.Format = "c";
                dgvBudget.Columns[nCol].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                Budgets.Add(str);
            }



            for (int nYear = nStartYear; nYear < nStartYear + nNumberYear; nYear++)
            {
                string[] strDataRow = {nYear.ToString()};
                int nIndex = dgvBudget.Rows.Add(strDataRow);
                dgvBudget.Rows[nIndex].Tag = nYear.ToString();

            }

            List<String> listDelete = new List<String>();

            String strSelect = "SELECT YEARID,YEAR_,BUDGETNAME,AMOUNT FROM YEARLYINVESTMENT WHERE SIMULATIONID='" +
                               m_strSimID + "' ORDER BY YEAR_";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                String strYearID = row[0].ToString();
                strYear = row[1].ToString();
                String strBudget = row[2].ToString();
                String strAmount = row[3].ToString();

                if (dgvBudget.Columns.Contains(strBudget))
                {
                    bool bYear = false;
                    foreach (DataGridViewRow dr in dgvBudget.Rows)
                    {
                        if (dr.Tag.ToString() == strYear)
                        {
                            bYear = true;
                            float fAmount = 0;
                            float.TryParse(strAmount, out fAmount);
                            dr.Cells[strBudget].Value = fAmount;
                            dr.Cells[strBudget].Tag = strYearID;
                        }
                    }

                    if (!bYear)
                    {
                        listDelete.Add(strYearID);
                    }
                }
                else
                {
                    listDelete.Add(strYearID);
                }
            }

            foreach (String strID in listDelete)
            {
                String strDelete = "DELETE FROM YEARLYINVESTMENT WHERE YEARID='" + strID + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strDelete);
                }
                catch (Exception except)
                {
                    Global.WriteOutput("Error: " + except.Message);
                }
            }







            m_bChange = true;

            //Delete extra budgets
            var delete = "DELETE FROM BUDGET_CRITERIA WHERE SIMULATIONID=" + m_strSimID;
            if (Budgets.Count > 0)
            {
                delete += " AND NOT (";
                var index = 0;
                foreach (var budget in Budgets)
                {
                    if (index > 0) delete += " OR ";
                    delete += "BUDGET_NAME='" + budget + "'";
                    index++;
                }

                delete += ")";

                try
                {
                    DBMgr.ExecuteNonQuery(delete);
                }
                catch (Exception except)
                {
                    Global.WriteOutput("Error deleting budgets from budget criteria: " + except.Message);
                }
            }

            //Build budget criteria
            dataGridViewBudgetCriteria.Rows.Clear();
            var select =
                "SELECT BUDGET_CRITERIA_ID, BUDGET_NAME, CRITERIA FROM BUDGET_CRITERIA WHERE SIMULATIONID='" +
                m_strSimID + "'";

            ds = DBMgr.ExecuteQuery(select);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var budgetCriteriaId = row["BUDGET_CRITERIA_ID"].ToString();
                var budgetName = row["BUDGET_NAME"].ToString();
                var criteria = "";
                if(row["CRITERIA"] != DBNull.Value) criteria = row["CRITERIA"].ToString();

                var rowIndex = dataGridViewBudgetCriteria.Rows.Add(budgetName, criteria.Replace("|","'"));
                dataGridViewBudgetCriteria.Rows[rowIndex].Tag = budgetCriteriaId;
            }
        }

        private void UpdateBudgetCriteria()
        {
            var comboBoxColumn = (DataGridViewComboBoxColumn)dataGridViewBudgetCriteria.Columns[0];
            comboBoxColumn.Items.Clear();
            foreach (var budget in Budgets)
            {
                comboBoxColumn.Items.Add(budget);
            }


        }


        private void buttonEditOrder_Click(object sender, EventArgs e)
        {
            FormEditBudgets formBudgets = new FormEditBudgets(textBoxBudgetOrder.Text.ToString());

            if (formBudgets.ShowDialog() == DialogResult.OK)
            {
                if (textBoxBudgetOrder.Text != formBudgets.m_strBudget)
                {
                    textBoxBudgetOrder.Text = formBudgets.m_strBudget.ToString();
                    UpdateBudget();
                    UpdateBudgetCriteria();
                }
            }
        }


        private void textBoxStartYear_Validating(object sender, CancelEventArgs e)
        {
            if (!m_bUpdate) return;
            int nYear = DateTime.Now.Year;

            String strYear = textBoxStartYear.Text.ToString();
            try
            {
                nYear = int.Parse(strYear);
            }
            catch
            {
                m_bUpdate = false;
                Global.WriteOutput("Error: " + strYear + " must be an integer number between 1900 and 2100.");
                textBoxStartYear.Text = m_strStartYear;
                m_bUpdate = true;
                return;
            }

            if (nYear > 2100 || nYear < 1900)
            {
                m_bUpdate = false;
                Global.WriteOutput("Error: Start year " + strYear + " must be an integer number between 1900 and 2100.");
                textBoxStartYear.Text = m_strStartYear;
                m_bUpdate = true;
                return;
            }

            String strUpdate = "UPDATE INVESTMENTS SET FIRSTYEAR='" + strYear + "' WHERE SIMULATIONID='" + m_strSimID + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);

            }
            catch (Exception except)
            {
                m_bUpdate = false;
                textBoxStartYear.Text = m_strStartYear;
                Global.WriteOutput("Error updating start year: " + except.Message);
                m_bUpdate = true;
                return;
            }
            UpdateInvestmentGrid();


        }

        private void textBoxInflation_Validated(object sender, EventArgs e)
        {
            if (!m_bUpdate) return;

            String strInflation = textBoxInflation.Text;
            strInflation = strInflation.Replace("%", "");


            String strUpdate = "UPDATE INVESTMENTS SET INFLATIONRATE='" + strInflation + "' WHERE SIMULATIONID='" + m_strSimID + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);

            }
            catch (Exception except)
            {
                m_bUpdate = false;
                textBoxInflation.Text = m_strInflation;
                m_bUpdate = true;
                Global.WriteOutput("Error updating Inflation rate: " + except.Message);
            }

        }

        private void textBoxDiscount_Validated(object sender, EventArgs e)
        {
            if (!m_bUpdate) return;
            String strDiscount = textBoxDiscount.Text;
            strDiscount = strDiscount.Replace("%", "");


            String strUpdate = "UPDATE INVESTMENTS SET DISCOUNTRATE='" + strDiscount + "' WHERE SIMULATIONID='" + m_strSimID + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);

            }
            catch (Exception except)
            {
                m_bUpdate = false;
                textBoxDiscount.Text = m_strDiscout;
                m_bUpdate = true;
                Global.WriteOutput("Error updating discount: " + except.Message);
            }
        }

        private void cbYears_Validated(object sender, EventArgs e)
        {

        }

        private void cbYears_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_bUpdate) return;
            String strYears = cbYears.Text;

            String strUpdate = "UPDATE INVESTMENTS SET NUMBERYEARS='" + strYears + "' WHERE SIMULATIONID='" + m_strSimID + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);

            }
            catch (Exception except)
            {
                Global.WriteOutput("Error: " + except.Message);
                return;
            }

            UpdateInvestmentGrid();
        }

        private void dgvBudget_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_bChange) return;
            DataGridViewRow row = dgvBudget.Rows[e.RowIndex];
            int nColumn = e.ColumnIndex;
            String strAmount;
            if (row.Cells[nColumn].Value != null)
            {
                strAmount = row.Cells[nColumn].Value.ToString();
            }
            else
            {
                strAmount = "";
            }
            strAmount = strAmount.Replace("$", "");
            strAmount = strAmount.Replace(",", "");
            String strID;

            if (row.Cells[nColumn].Tag != null)
            {
                strID = row.Cells[nColumn].Tag.ToString();

                String strUpdate = "UPDATE YEARLYINVESTMENT SET AMOUNT='" + strAmount + "' WHERE YEARID='" + strID + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch (Exception except)
                {
                    m_bChange = false;
                    dgvBudget.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastCell;
                    Global.WriteOutput("Error: " + except.Message);
                    m_bChange = true;
                    return;
                }
            }
            else
            {
                String strBudget = dgvBudget.Columns[nColumn].Name.ToString();
                String strYear = row.Tag.ToString();

                String strInsert = "INSERT INTO YEARLYINVESTMENT (SIMULATIONID,YEAR_,BUDGETNAME,AMOUNT) VALUES ('" + m_strSimID + "','" + strYear + "','" + strBudget + "','" + strAmount + "')";
                try
                {
                    DBMgr.ExecuteNonQuery(strInsert);
                }
                catch (Exception except)
                {
                    m_bChange = false;
                    dgvBudget.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastCell;
                    Global.WriteOutput("Investment amount Error: " + except.Message);
                    m_bChange = true;
                    return;
                }
				String strIdentity = "";
					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
							strIdentity = "SELECT IDENT_CURRENT ('YEARLYINVESTMENT') FROM YEARLYINVESTMENT";
							break;
						case "ORACLE":
							//strIdentity = "SELECT YEARLYINVESTMENT_YEARID_SEQ.CURRVAL FROM DUAL";
							//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'YEARLYINVESTMENT_YEARID_SEQ'";
							strIdentity = "SELECT MAX(YEARID) FROM YEARLYINVESTMENT";
							break;
						default:
							throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
							//break;
					}
				DataSet ds = DBMgr.ExecuteQuery( strIdentity );
                strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();

                row.Cells[nColumn].Tag = strIdentity;
            }
        }

        private void textBoxBudgetOrder_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxBudgetOrder_Validated(object sender, EventArgs e)
        {
            UpdateBudget();
        }

        private void UpdateBudget()
        {

            String strBudgetOrder = textBoxBudgetOrder.Text.ToString();


            String strUpdate = "UPDATE INVESTMENTS SET BUDGETORDER='" + strBudgetOrder + "' WHERE SIMULATIONID='" + m_strSimID + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);

            }
            catch (Exception except)
            {
                Global.WriteOutput("Error: " + except.Message);
                return;
            }
            UpdateInvestmentGrid();

        }

        private void FormInvestment_FormClosed(object sender, FormClosedEventArgs e)
        {
			FormUnload();
            FormManager.RemoveFormInvestment(this);
        }

        private void dgvBudget_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {
                m_oLastCell = dgvBudget.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            }
        }

        private void textBoxInflation_Enter(object sender, EventArgs e)
        {
            m_strInflation = textBoxInflation.Text;
        }

        private void textBoxDiscount_Enter(object sender, EventArgs e)
        {
            m_strDiscout = textBoxDiscount.Text;
        }

        private void textBoxStartYear_Enter(object sender, EventArgs e)
        {
            m_strStartYear = textBoxStartYear.Text;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in dgvBudget.SelectedCells)
            {
                if (cell.ColumnIndex > 0)
                {
                    String strBudgetName = dgvBudget.Columns[cell.ColumnIndex].Name.ToString();
                    String strYear = dgvBudget[0,cell.RowIndex].Value.ToString();
                    cell.Value = "0";
                }
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataObject d = dgvBudget.GetClipboardContent();
            Clipboard.SetDataObject(d);
        }

        private void dgvBudget_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                DataObject d = dgvBudget.GetClipboardContent();
                Clipboard.SetDataObject(d);
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteCells();
            }
            if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewCell cell in dgvBudget.SelectedCells)
                {
                    if (cell.ColumnIndex > 0)
                    {
                        String strBudgetName = dgvBudget.Columns[cell.ColumnIndex].Name.ToString();
                        String strYear = dgvBudget[0, cell.RowIndex].Value.ToString();
                        cell.Value = "0";
                    }
                }
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteCells();
        }


        private void PasteCells()
        {
            if (dgvBudget.SelectedCells == null) return;

            string s = Clipboard.GetText();
            s = s.Replace("\r\n", "\n");
            string[] lines = s.Split('\n');
            string[] cells;
            bool bSinglePaste = false;
            if (lines.Length == 1)
            {
                cells = lines[0].Split('\t');
                if (cells.Length == 1) bSinglePaste = true;
            }



            int nStartRow = dgvBudget.SelectedCells[0].RowIndex;
            int nStartColumn = dgvBudget.SelectedCells[0].ColumnIndex;
            int nRow = nStartRow;

            if (!bSinglePaste)
            {

                foreach (String line in lines)
                {
                    cells = line.Split('\t');
                    int nCol = nStartColumn;
                    foreach (string cell in cells)
                    {
                        if (nCol < dgvBudget.ColumnCount && nRow < dgvBudget.RowCount && nCol != 0)
                        {
                            if (cell.ToString() == "")
                            {

                                dgvBudget[nCol, nRow].Value = 0;
                            }
                            else
                            {
                                float fAmount = 0;
                                try
                                {
                                    fAmount = float.Parse(cell.ToString().Replace("$", "").Replace(",", ""));
                                    dgvBudget[nCol, nRow].Value = fAmount;
                                }
                                catch { }
                            }
                        }
                        nCol++;
                    }
                    nRow++;
                }
            }
            else
            {
                cells = lines[0].Split('\t');
                String strAmount = cells[0].ToString().Replace("$","").Replace(",","");
                try
                {
                    float fAmount = float.Parse(strAmount);

                    foreach (DataGridViewCell dgvCell in dgvBudget.SelectedCells)
                    {
                        if (dgvCell.ColumnIndex != 0)
                        {
                            dgvCell.Value = fAmount;

                        }
                    }
                }
                catch { }



            }
        }






        private void DataGridViewBudgetCriteria_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (e.Row.Tag != null)
            {
                String strDelete = "DELETE FROM BUDGET_CRITERIA WHERE BUDGET_CRITERIA_ID='" + e.Row.Tag.ToString() + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strDelete);
                }
                catch (Exception except)
                {
                    Global.WriteOutput("Error Deleting Budget Criteria:" + except.Message.ToString());
                }
            }
        }

        private void DataGridViewBudgetCriteria_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex != -1)
            {
                if (Global.SecurityOperations.CanModifySimulationTreatment(m_strNetworkID, m_strSimID))
                {
                    var strCriteria = "";
                    if (dataGridViewBudgetCriteria.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        strCriteria = dataGridViewBudgetCriteria.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    }
                    var formAdvancedSearch = new FormAdvancedSearch(m_strNetwork, m_hashAttributeYear, strCriteria, true);
                    if (formAdvancedSearch.ShowDialog() == DialogResult.OK)
                    {
                        dataGridViewBudgetCriteria[e.ColumnIndex, e.RowIndex].Value = formAdvancedSearch.GetWhereClause();
                    }
                    dataGridViewBudgetCriteria.Update();
                }
            }
        }

        private void DataGridViewBudgetCriteria_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                var strValue = "";
                if (dataGridViewBudgetCriteria.Rows[e.RowIndex].Tag == null)
                {


                    if (dataGridViewBudgetCriteria.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        strValue = dataGridViewBudgetCriteria.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    }
                    else
                    {
                        return;
                    }


                    var insert = "";
                    switch (e.ColumnIndex)
                    {
                        case 0:
                            insert = "INSERT INTO BUDGET_CRITERIA (SIMULATIONID, BUDGET_NAME) VALUES ('" +
                                     m_strSimID + "','" + strValue + "')";
                            break;
                        case 1:
                            insert = "INSERT INTO BUDGET_CRITERIA (SIMULATIONID, CRITERIA) VALUES ('" +
                                     m_strSimID + "','" + strValue.Replace("'","|") + "')";

                            break;
                        default:
                            return;
                    }



                    try
                    {
                        String strIdentity;
                        DBMgr.ExecuteNonQuery(insert);
                        switch (DBMgr.NativeConnectionParameters.Provider)
                        {
                            case "MSSQL":
                                strIdentity = "SELECT IDENT_CURRENT ('BUDGET_CRITERIA') FROM BUDGET_CRITERIA";
                                break;
                            case "ORACLE":
                                strIdentity = "SELECT MAX(BUDGET_CRITERIA_ID) FROM BUDGET_CRITERIA";
                                break;
                            default:
                                throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                                //break;
                        }

                        DataSet ds = DBMgr.ExecuteQuery(strIdentity);
                        dataGridViewBudgetCriteria.Rows[e.RowIndex].Tag = ds.Tables[0].Rows[0].ItemArray[0].ToString();

                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error insert budget criteria:" + except.Message.ToString());
                    }
                }
                else
                {
                    String strTag = dataGridViewBudgetCriteria.Rows[e.RowIndex].Tag.ToString();
                    String strUpdate = "";


                    if (dataGridViewBudgetCriteria.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        strValue = dataGridViewBudgetCriteria.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    }
                    else
                    {
                        return;
                    }

                    switch (e.ColumnIndex)
                    {
                        case 0:

                            strUpdate = "UPDATE BUDGET_CRITERIA SET BUDGET_NAME='" + strValue + "' WHERE BUDGET_CRITERIA_ID='" + strTag + "'";
                            break;
                        case 1:
                            strUpdate = "UPDATE BUDGET_CRITERIA SET CRITERIA='" + strValue.Replace("'","|") + "' WHERE BUDGET_CRITERIA_ID='" + strTag + "'";
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
    }
}
