using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DatabaseManager;
using System.Collections;
using RoadCare3.Properties;
using RoadCareDatabaseOperations;
using DataObjects;
namespace RoadCare3
{
    public partial class FormPerformanceEquations : BaseForm
    {
        private String m_strNetwork;
		private String m_strNetworkID;
		private String m_strSimulation;
        private String m_strSimulationID;
        private bool m_bUpdate = false;
        Hashtable m_hashAttributeYear;
        private object m_oLastCell;

        public FormPerformanceEquations(String strNetwork, String strSimulation, String strSimulationID,Hashtable hashAttributeYear)
        {
            m_strNetwork = strNetwork;
			m_strNetworkID = DBOp.GetNetworkIDFromName( strNetwork );
            m_strSimulation = strSimulation;
            m_strSimulationID = strSimulationID;
            m_hashAttributeYear = hashAttributeYear;
            InitializeComponent();
        }

        private void FormPerformanceEquations_Load(object sender, EventArgs e)
        {
			SecureForm();

			FormLoad(Settings.Default.SIMULATION_IMAGE_KEY, Settings.Default.SIMULATION_IMAGE_KEY_SELECTED);
            this.labelPerformance.Text = "Performance Equations for " + m_strSimulation + " : " + m_strNetwork;
            Global.LoadAttributes();

            foreach(String strAttribute in Global.Attributes)
            {
                this.Attribute.Items.Add(strAttribute);
            }

            List<Performance> performances = null;
            try
            {
                performances = DBOp.GetPerformanceEquations(m_strSimulationID);
            }
            catch (Exception except)
            {
                Global.WriteOutput("Error retrieving performance equations: " + except.Message);
                return;
            }

            List<String> listDelete = new List<String>();
            foreach(Performance performance in performances)
            {
                if (!Global.Attributes.Contains(performance.Attribute))
                {
                    listDelete.Add(performance.PerformanceID);
                }
                else
                {
                    object[] dataRow = { performance.Attribute, performance.Name, performance.Equation, performance.Criteria.Replace("|", "'"), performance.IsShift };
                    int nIndex = dgvPerfomance.Rows.Add(dataRow);
                    dgvPerfomance.Rows[nIndex].Tag = performance;
                }
            }

            try
            {
                DBOp.DeletePerformanceEquations(listDelete);
            }
            catch (Exception except)
            {
                Global.WriteOutput("Error deleting performance equations with deleted attributes: " + except.Message);
                return;
            }


            dgvPerfomance.Columns[2].ReadOnly = true;
            dgvPerfomance.Columns[3].ReadOnly = true;
            m_bUpdate = true;
        }

		protected void SecureForm()
		{
			LockDataGridView( dgvPerfomance );
			if( Global.SecurityOperations.CanModifySimulationPerformance( m_strNetworkID, m_strSimulationID ) )
			{
				UnlockDataGridViewForCreateDestroy( dgvPerfomance );
			}
		}

        private void dgvPerfomance_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_bUpdate) return;
            int nRow = e.RowIndex;
            int nCol = e.ColumnIndex;
            Performance performance = (Performance) dgvPerfomance.Rows[nRow].Tag;
            if (performance != null)
            {
                DataGridViewRow row = dgvPerfomance.Rows[nRow];
				DataGridViewColumn col = dgvPerfomance.Columns[nCol];
                String strValue = "";
                if (row.Cells[nCol].Value != null)
                {
                    strValue = row.Cells[nCol].Value.ToString();
                }
                String strUpdate;
                if (nCol == 0)
                {
                    performance.Attribute = strValue;
                    strUpdate = "UPDATE PERFORMANCE SET ATTRIBUTE_='" + strValue + "' ";
                }
                else if (nCol == 1)
                {
                    dgvPerfomance.Name = strValue;
                    strUpdate = "UPDATE PERFORMANCE SET EQUATIONNAME='" + strValue + "' ";
                }
                else if (nCol == 3)
                {
                    String strCriteria = strValue.Replace("'", "|");
                    strUpdate = "UPDATE PERFORMANCE SET CRITERIA='" + strCriteria + "'";
                }
                else if (nCol == 2)
                {
                    if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
                    {
                        if (performance.IsPiecewise)
                        {
                            strUpdate = "UPDATE PERFORMANCE SET EQUATION='" + performance.Equation + "', PIECEWISE='1'";
                        }
                        else
                        {
                            strUpdate = "UPDATE PERFORMANCE SET EQUATION='" + performance.Equation + "', PIECEWISE='0'";
                        }

                        if (performance.IsFunction)
                        {
                            strUpdate += ",ISFUNCTION='1'";
                        }
                        else
                        {
                            strUpdate += ",ISFUNCTION='0'";
                        }
                    }
                    else
                        strUpdate = "UPDATE PERFORMANCE SET EQUATION='" + performance.Equation + "', PIECEWISE='" + performance.IsPiecewise.ToString() + "', ISFUNCTION='" + performance.IsFunction.ToString() + "'";
                }
                else if (nCol == 4)
                {
                    DataGridViewCheckBoxCell checkbox = (DataGridViewCheckBoxCell)row.Cells[nCol];
                    performance.IsShift = (bool) checkbox.Value;
                    if (DBMgr.NativeConnectionParameters.Provider == "ORACLE")
                    {
                        if (performance.IsShift)
                        {
                            strUpdate = "UPDATE PERFORMANCE SET SHIFT='1'";
                        }
                        else
                        {     
                            strUpdate = "UPDATE PERFORMANCE SET SHIFT='0'";
                        }
                    }
                    else
                    {
                        strUpdate = "UPDATE PERFORMANCE SET SHIFT='" + performance.IsShift.ToString() + "'";
                    }
                }
                else
                {
                    return;
                }
                strUpdate += " WHERE PERFORMANCEID='" + performance.PerformanceID + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch (Exception except)
                {
                    m_bUpdate = false;
                    dgvPerfomance.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastCell;
                    Global.WriteOutput("Error updating Performance:" + except.Message.ToString());
                    m_bUpdate = true;
                }

            }//Insert
            else
            {
                String strAttribute = "";
                String strCriteria = "";
                String strEquationName = "";
                String strEquations = "";
                String strShift = "false";
                DataGridViewRow row = dgvPerfomance.Rows[nRow];
                performance = new Performance();

                if (nCol == 0)
                {
                    strAttribute = row.Cells[nCol].Value.ToString();
                    performance.Attribute = strAttribute;
                }
                else if (nCol == 1)
                {
                    strEquationName = row.Cells[nCol].Value.ToString();
                    performance.Name = strEquationName;
                }
                else if (nCol == 3)
                {
                    strCriteria = row.Cells[nCol].Value.ToString().Replace("'","|");
                    performance.Criteria = strCriteria;
                }
                else if (nCol == 2)
                {
                    strEquations = row.Cells[nCol].Value.ToString();
                    performance.Equation = strEquations;
                }
                else if (nCol == 4)
                {
                    strShift = row.Cells[nCol].Value.ToString();
                    performance.IsShift = Convert.ToBoolean(row.Cells[nCol].Value);
                }
                else
                {
                    return;
                }
				String strInsert = "";
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						strInsert = "INSERT INTO PERFORMANCE (SIMULATIONID,ATTRIBUTE_,EQUATIONNAME,CRITERIA,EQUATION,SHIFT) VALUES ('" + m_strSimulationID + "','" + strAttribute + "','" + strEquationName + "','" + strCriteria + "','" + strEquations + "','" + strShift + "')";
						break;
					case "ORACLE":
						strInsert = "INSERT INTO PERFORMANCE (SIMULATIONID,ATTRIBUTE_,EQUATIONNAME,CRITERIA,EQUATION,SHIFT) VALUES ('" + m_strSimulationID + "','" + strAttribute + "','" + strEquationName + "','" + strCriteria + "','" + strEquations + "','" + (strShift.ToLower() != "false" ? "1" : "0") + "')";
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}
				try
                {
                    DBMgr.ExecuteNonQuery(strInsert);
					String strIdentity = "";
					switch (DBMgr.NativeConnectionParameters.Provider)
					{
						case "MSSQL":
							strIdentity = "SELECT IDENT_CURRENT ('PERFORMANCE') FROM PERFORMANCE";
							break;
						case "ORACLE":
							//strIdentity = "SELECT PERFORMANCE_PERFORMANCEID_SEQ.CURRVAL FROM DUAL";
							//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'PERFORMANCE_PERFORMANCEID_SEQ'";
							strIdentity = "SELECT MAX(PERFORMANCEID) FROM PERFORMANCE";
							break;
						default:
							throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
							//break;
					}
					DataSet ds = DBMgr.ExecuteQuery( strIdentity );
                    performance.PerformanceID = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    dgvPerfomance.Rows[e.RowIndex].Tag = performance;
                }
                catch (Exception except)
                {
                    m_bUpdate = false;
                    dgvPerfomance.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = m_oLastCell;
                    Global.WriteOutput("Error inserting new Performance:" + except.Message.ToString());
                    m_bUpdate = true;
                }
            }
        }

        private void editCriteriaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int nRow = 0;
            if (dgvPerfomance.SelectedRows.Count > 0 || dgvPerfomance.SelectedCells.Count > 0)
            {
                if (dgvPerfomance.SelectedRows.Count > 0)
                {
                    nRow = dgvPerfomance.SelectedRows[0].Index;
                }
                else if (dgvPerfomance.SelectedCells.Count > 0)
                {
                    nRow = dgvPerfomance.SelectedCells[0].RowIndex;
                }
            }
            else
            {
                return;
            }
            DataGridViewRow row = dgvPerfomance.Rows[nRow];
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

        private void dgvPerfomance_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (e.Row.Tag != null)
            {
                
                if(e.Row.Tag is Performance)
                {
                    Performance performance = (Performance) e.Row.Tag;    
                

                    String strDelete = "DELETE FROM PERFORMANCE WHERE PERFORMANCEID='" + performance.PerformanceID + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strDelete);
                    }
                    catch (Exception except)
                    {
                        Global.WriteOutput("Error Deleting Performance Equations:" + except.Message.ToString());
                    }
                }
            }
        }

        private void dgvPerfomance_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            m_oLastCell = dgvPerfomance.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        }

        private void FormPerformanceEquations_FormClosed(object sender, FormClosedEventArgs e)
        {
			FormUnload();
            FormManager.RemoveFormPerformanceEquations(this);
        }

        private void dgvPerfomance_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
			if( e.ColumnIndex >= 2 )
			{
				if( Global.SecurityOperations.CanModifySimulationPerformance( m_strNetworkID, m_strSimulationID ) )
				{
					if( dgvPerfomance.Rows[e.RowIndex].Cells[0].Value == null )
					{
						Global.WriteOutput( "Error: Select a performance attribute before editing the performance equation or criteria." );
						return;
					}

					String strAttribute = dgvPerfomance.Rows[e.RowIndex].Cells[0].Value.ToString();
					if( strAttribute.Trim().Length == 0 )
					{
						Global.WriteOutput( "Error: Select a performance attribute before editing the performance equation or criteria." );
						return;
					}

					String strCriteria = "";
					if( e.ColumnIndex == 2 )
					{
                        Performance performance = (Performance)dgvPerfomance.Rows[e.RowIndex].Tag;
                        if (performance == null) performance = new Performance();
						FormEditEquation formEditEquation = new FormEditEquation( performance);

                        
                        if( formEditEquation.ShowDialog() == DialogResult.OK )
						{
							dgvPerfomance.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = performance.Equation;
                            dgvPerfomance.Update();
						}
					}
					else if( e.ColumnIndex == 3 )
					{
						if( dgvPerfomance.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null )
						{
							strCriteria = dgvPerfomance.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
						}
						FormAdvancedSearch formAdvancedSearch = new FormAdvancedSearch( m_strNetwork, m_hashAttributeYear, strCriteria, true );
						if( formAdvancedSearch.ShowDialog() == DialogResult.OK )
						{
							dgvPerfomance[e.ColumnIndex, e.RowIndex].Value = formAdvancedSearch.GetWhereClause();
                        }
                        dgvPerfomance.Update();
					}
				}
			}
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteRows();
        }

        private void PasteRows()
        {
            string s = Clipboard.GetText();
            s = s.Replace("\r\n", "\n");
            string[] lines = s.Split('\n');
            foreach (string line in lines)
            {
                string[] values = line.Split('\t');
                if (values.Length == 4)
                {
                    int nRow = dgvPerfomance.Rows.Add(values[0], values[1], values[2], values[3]);

                    String strInsert = "INSERT INTO PERFORMANCE (SIMULATIONID,ATTRIBUTE_,EQUATIONNAME,EQUATION,CRITERIA) VALUES ('" + m_strSimulationID + "','" + values[0].ToString() + "','" + values[1].ToString() + "','" + values[2].ToString() + "','" + values[3].ToString().Replace("'", "|") + "')";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strInsert);
						String strIdentity = "";
							switch( DBMgr.NativeConnectionParameters.Provider )
							{
								case "MSSQL":
									strIdentity = "SELECT IDENT_CURRENT ('PERFORMANCE') FROM PERFORMANCE";
									break;
								case "ORACLE":
									//strIdentity = "SELECT PERFORMANCE_PERFORMANCEID_SEQ.CURRVAL FROM DUAL";
									//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'PERFORMANCE_PERFORMANCEID_SEQ'";
									strIdentity = "SELECT MAX(PERFORMANCEID) FROM PERFORMANCE";
									break;
								default:
									throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
									//break;
							}
						DataSet ds = DBMgr.ExecuteQuery( strIdentity );
                        strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                        dgvPerfomance.Rows[nRow].Tag = strIdentity;
                    }
                    catch (Exception except)
                    {
                        m_bUpdate = false;
                        Global.WriteOutput("Error inserting new Performance:" + except.Message.ToString());
                        m_bUpdate = true;
                    }
                }
            }
        }


        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataObject data = dgvPerfomance.GetClipboardContent();
            Clipboard.SetDataObject(data, true);
        }

        private void dgvPerfomance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                DataObject d = dgvPerfomance.GetClipboardContent();
                Clipboard.SetDataObject(d);
            }

            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteRows();
            }

            if (e.KeyCode == Keys.Delete)
            {

            }
        }

        private void editEquationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPerfomance.SelectedCells == null) return;

            int nRow = dgvPerfomance.SelectedCells[0].RowIndex;
            String strEquation = dgvPerfomance[2, nRow].Value.ToString();
            String strAttribute = dgvPerfomance[0, nRow].Value.ToString();


			FormEditEquation formEditEquation = new FormEditEquation(strEquation, strAttribute);
            if (formEditEquation.ShowDialog() == DialogResult.OK)
            {
                dgvPerfomance.Rows[nRow].Cells[2].Value = formEditEquation.Equation;
                dgvPerfomance.Update();
            }
        }


    }
}
