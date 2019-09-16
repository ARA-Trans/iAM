using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;
using RoadCareGlobalOperations;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data.OleDb;

namespace RoadCare3
{
    public partial class FormCalculatedField : BaseForm
    {
        private String m_strAttribute;
        private NetworkDefinition _currNetDef;

        public String Attribute
        {
            get { return m_strAttribute; }
        }
        public FormCalculatedField(String strAttribute, NetworkDefinition currNetDef)
        {
            InitializeComponent();

            _currNetDef = currNetDef;
            m_strAttribute = strAttribute;
            this.Text = this.Attribute;
            labelAttribute.Text = "Calculated Field: " + strAttribute;
            this.TabText = strAttribute;
        }

        //Updates database on edit of calculated equation field.
        private void FormCalculatedField_Load(object sender, EventArgs e)
        {
            String strSelect = "SELECT ID_,EQUATION, CRITERIA,ISFUNCTION FROM  ATTRIBUTES_CALCULATED WHERE ATTRIBUTE_='" + this.Attribute + "'";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    String strID = dr["ID_"].ToString();
                    String strEquation = dr["EQUATION"].ToString();
                    String strCriteria = dr["CRITERIA"].ToString().Replace("|","'");
                    String isFunction = "0";
                    bool bIsFunction = false;
                    if (dr["ISFUNCTION"] != DBNull.Value) bIsFunction = Convert.ToBoolean(dr["ISFUNCTION"]);
                    if (bIsFunction) isFunction = "1";

                    int nRow = dgvCalculated.Rows.Add(strEquation, strCriteria);
                    dgvCalculated.Rows[nRow].Tag = strID;
                    dgvCalculated.Rows[nRow].Cells[0].Tag = isFunction;
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Loading CALCULATED FIELD equations and criteria." + exception.Message);
            }
        }

        private void dgvCalculated_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					MSSQLdgvCalculatedCellDoubleClick(e);
					break;
				case "ORACLE":
					ORACLEdgvCalculatedCellDoubleClick(e);
					dgvCalculated.Rows.Clear();
					FormCalculatedField_Load( null, null );
					break;
				default:
					throw new NotImplementedException("Calculated Fields Provider not implemented.");
					//break;
			}
		}

		private void ORACLEdgvCalculatedCellDoubleClick(DataGridViewCellEventArgs e)
		{
			int nRow = e.RowIndex;
			int nColumn = e.ColumnIndex;
			String strID = "";

			if (nColumn == 0)//Edit equations
			{
                String strEquation = "";
                String isFunctionTag = "0";
                bool isFunction = false;
				if (dgvCalculated[nColumn, nRow].Value != null)
				{
					strEquation = dgvCalculated[nColumn, nRow].Value.ToString();
                    isFunctionTag = dgvCalculated[0, nRow].Tag.ToString();
                    if (isFunctionTag == "1")
                    {
                        isFunction = true;
                    }
				}
				FormEditEquation formEditEquation = new FormEditEquation(strEquation,false, isFunction);
				formEditEquation.CalculatedField = true;
				if (formEditEquation.ShowDialog() == DialogResult.OK)
				{
                    if (formEditEquation.IsFunction) dgvCalculated[0, nRow].Tag = "1";
                    else dgvCalculated[0, nRow].Tag = "0";
                    
                    dgvCalculated[e.ColumnIndex, e.RowIndex].Value = formEditEquation.Equation;
					if (dgvCalculated.Rows[nRow].Tag == null) //Insert new
					{
						dgvCalculated.Rows.Add();
						try
						{
							strEquation = formEditEquation.Equation;
							String strInsert = "INSERT INTO ATTRIBUTES_CALCULATED (ATTRIBUTE_,EQUATION,ISFUNCTION)VALUES('" + this.Attribute + "','" + strEquation + "','" + dgvCalculated[0,nRow].Tag.ToString() + "')";
							DBMgr.ExecuteNonQuery(strInsert);
							dgvCalculated.Rows[nRow].Tag = DBMgr.GetCurrentAutoIncrement("ATTRIBUTES_CALCULATED").ToString();
							dgvCalculated[nColumn, nRow].Value = strEquation;
							dgvCalculated.Update();
						}
						catch (Exception exception)
						{
							Global.WriteOutput("Error: Inserting EQUATION for Calculated Fields." + exception.Message);
						}


					}
					else //Update existing
					{
						try
						{
							strEquation = formEditEquation.Equation;
							strID = dgvCalculated.Rows[nRow].Tag.ToString();
							string strUpdate = "UPDATE ATTRIBUTES_CALCULATED SET EQUATION='" + strEquation + "', ISFUNCTION='" + dgvCalculated[0,nRow].Tag.ToString() + "' WHERE ID_='" + strID + "'";
							DBMgr.ExecuteNonQuery(strUpdate);
						}
						catch (Exception exception)
						{
							Global.WriteOutput("Error: Updating EQUATION for Calculated Fields." + exception.Message);
						}
					}
				}
			}
			else // Edit criteria
			{
				String strCriteria = "";
				if (dgvCalculated[nColumn, nRow].Value != null)
				{
					strCriteria = dgvCalculated[nColumn, nRow].Value.ToString();
				}
				FormAdvancedSearch formAdvancedSearch = new FormAdvancedSearch(strCriteria);
				if (formAdvancedSearch.ShowDialog() == DialogResult.OK)
				{
					//SqlParameter param0 = new SqlParameter("@criteria", SqlDbType.VarBinary, -1);
                    dgvCalculated[e.ColumnIndex, e.RowIndex].Value = formAdvancedSearch.Query;
					if (dgvCalculated.Rows[nRow].Tag == null) //Insert new
					{
						try
						{
							strCriteria = formAdvancedSearch.Query;
							dgvCalculated[nColumn, nRow].Value = strCriteria;
							strCriteria = strCriteria.Replace("'", "|");
							string strInsert = "INSERT INTO ATTRIBUTES_CALCULATED (ATTRIBUTE_,EQUATION,CRITERIA)VALUES('" + this.Attribute + "','','" + strCriteria + "')";
							DBMgr.ExecuteNonQuery(strInsert);
							dgvCalculated.Rows[nRow].Tag = DBMgr.GetCurrentAutoIncrement("ATTRIBUTES_CALCULATED");
							dgvCalculated.Rows.Add();
							dgvCalculated.Update();
							dgvCalculated.Refresh();
						}
						catch (Exception exception)
						{
							Global.WriteOutput("Error: Inserting Criteria for Calculated Fields." + exception.Message);
						}


					}
					else //Update existing
					{
						try
						{
							strCriteria = formAdvancedSearch.Query;
							dgvCalculated[nColumn, nRow].Value = strCriteria;
							strCriteria = strCriteria.Replace("'", "|");
							strID = dgvCalculated.Rows[nRow].Tag.ToString();
							string strUpdate = "UPDATE ATTRIBUTES_CALCULATED SET CRITERIA='" + strCriteria + "' WHERE ID_='" + strID + "'";
							DBMgr.ExecuteNonQuery(strUpdate);
							dgvCalculated.Update();
						}
						catch (Exception exception)
						{
							Global.WriteOutput("Error: Updating CRITERIA for Calculated Fields." + exception.Message);
						}
					}
				}
			}
            dgvCalculated.Update();
		}

		private void MSSQLdgvCalculatedCellDoubleClick(DataGridViewCellEventArgs e)
		{
			int nRow = e.RowIndex;
			int nColumn = e.ColumnIndex;
			String strID = "";

			if (nColumn == 0)//Edit equations
			{
				String strEquation = "";
                String isFunctionTag = "0";
                bool isFunction = false;
				if (dgvCalculated[nColumn, nRow].Value != null)
				{
					strEquation = dgvCalculated[nColumn, nRow].Value.ToString();
                }
				FormEditEquation formEditEquation = new FormEditEquation(strEquation,false,isFunction);
				formEditEquation.CalculatedField = true;
				if (formEditEquation.ShowDialog() == DialogResult.OK)
				{

					if (dgvCalculated.Rows[nRow].Tag == null) //Insert new
					{

						try
						{
							strEquation = formEditEquation.Equation;
						    dgvCalculated.Rows.Add();
                            String strInsert = "INSERT INTO ATTRIBUTES_CALCULATED (ATTRIBUTE_,EQUATION,ISFUNCTION)VALUES('" + this.Attribute + "','" + strEquation + "','0')";
							SqlCommand command = new SqlCommand(strInsert, DBMgr.NativeConnectionParameters.SqlConnection);
							command.ExecuteNonQuery();

							String strIdentity = "SELECT IDENT_CURRENT ('ATTRIBUTES_CALCULATED') FROM ATTRIBUTES_CALCULATED";
							DataSet ds = DBMgr.ExecuteQuery(strIdentity);
							strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
							dgvCalculated.Rows[nRow].Tag = strIdentity;
							dgvCalculated[nColumn, nRow].Value = strEquation;
							dgvCalculated.Update();
						    dgvCalculated[e.ColumnIndex, e.RowIndex].Value = formEditEquation.Equation;
                        }
						catch (Exception exception)
						{
							Global.WriteOutput("Error: Inserting EQUATION for Calculated Fields." + exception.Message);
						}


					}
					else //Update existing
					{
						try
						{
							strEquation = formEditEquation.Equation;

							strID = dgvCalculated.Rows[nRow].Tag.ToString();
							String strUpdate = "UPDATE ATTRIBUTES_CALCULATED SET EQUATION='" + strEquation + "', ISFUNCTION='0' WHERE ID_='" + strID + "'";
							SqlCommand command = new SqlCommand(strUpdate, DBMgr.NativeConnectionParameters.SqlConnection);
							command.ExecuteNonQuery();
						    dgvCalculated[e.ColumnIndex, e.RowIndex].Value = formEditEquation.Equation;
                        }
						catch (Exception exception)
						{
							Global.WriteOutput("Error: Updating EQUATION for Calculated Fields." + exception.Message);
						}
					}
				}
			}
			else // Edit criteria
			{
				String strCriteria = "";
				if (dgvCalculated[nColumn, nRow].Value != null)
				{
					strCriteria = dgvCalculated[nColumn, nRow].Value.ToString();
				}
				FormAdvancedSearch formAdvancedSearch = new FormAdvancedSearch(strCriteria);

				if (formAdvancedSearch.ShowDialog() == DialogResult.OK)
				{
                    dgvCalculated[nColumn, nRow].Value = formAdvancedSearch.Query;
					SqlParameter param0 = new SqlParameter("@criteria", SqlDbType.VarBinary, -1);
					if (dgvCalculated.Rows[nRow].Tag == null) //Insert new
					{
						try
						{
							strCriteria = formAdvancedSearch.Query;
							dgvCalculated[nColumn, nRow].Value = strCriteria;
							strCriteria = strCriteria.Replace("'", "|");
							String strInsert = "INSERT INTO ATTRIBUTES_CALCULATED (ATTRIBUTE_,EQUATION,CRITERIA)VALUES('" + this.Attribute + "','','" + strCriteria + "')";
							SqlCommand command = new SqlCommand(strInsert, DBMgr.NativeConnectionParameters.SqlConnection);
							command.ExecuteNonQuery();

							String strIdentity = "SELECT IDENT_CURRENT ('ATTRIBUTES_CALCULATED') FROM ATTRIBUTES_CALCULATED";
							DataSet ds = DBMgr.ExecuteQuery(strIdentity);
							strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
							dgvCalculated.Rows[nRow].Tag = strIdentity;
							dgvCalculated.Rows.Add();
							dgvCalculated.Update();
							dgvCalculated.Refresh();
						}
						catch (Exception exception)
						{
							Global.WriteOutput("Error: Inserting Criteria for Calculated Fields." + exception.Message);
						}


					}
					else //Update existing
					{
						try
						{
							strCriteria = formAdvancedSearch.Query;
							dgvCalculated[nColumn, nRow].Value = strCriteria;
							strCriteria = strCriteria.Replace("'", "|");
							strID = dgvCalculated.Rows[nRow].Tag.ToString();
							String strUpdate = "UPDATE ATTRIBUTES_CALCULATED SET CRITERIA='" + strCriteria + "' WHERE ID_='" + strID + "'";
							SqlCommand command = new SqlCommand(strUpdate, DBMgr.NativeConnectionParameters.SqlConnection);
							command.ExecuteNonQuery();
							dgvCalculated.Update();

						}
						catch (Exception exception)
						{
							Global.WriteOutput("Error: Updating CRITERIA for Calculated Fields." + exception.Message);
						}
					}
				}
			}
            dgvCalculated.Update();
		}

        private void FormCalculatedField_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormManager.RemoveFormCalculatedField(this);
        }

        private void dgvCalculated_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.Tag != null)
            {
                String strID = e.Row.Tag.ToString();
                String strDelete = "DELETE FROM ATTRIBUTES_CALCULATED WHERE ID_=" + strID;
                try
                {
                    DBMgr.ExecuteNonQuery(strDelete);
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error: Deleting Calculated Field Equations/Criteria." + exception.Message);
                }
            }
        }
    }
}
