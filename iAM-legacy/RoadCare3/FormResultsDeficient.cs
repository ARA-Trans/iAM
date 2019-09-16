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
    public partial class FormResultsDeficient : ToolWindow
    {
        private String m_strSimulationID;
        private String m_strNetworkID;
        private int m_nStart;
        private int m_nPeriod;
        private bool m_bChange = false;

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
 
        public FormResultsDeficient()
        {
            InitializeComponent();
        }

        private void FormResultsDeficient_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormManager.RemoveResultsDeficientWindow();
        }

        public void UpdateDeficientGrid()
        {

            m_bChange = false;
            dgvDeficient.Rows.Clear();
            dgvDeficient.Columns.Clear();
            dgvDeficient.ReadOnly = true;
            if (Period == 0) return;

            dgvDeficient.ColumnCount = 2 + this.Period;
            for (int i = 2; i < Period + 2; i++)
            {
                int nYear = StartYear + i - 2;
                dgvDeficient.Columns[i].HeaderText = nYear.ToString();
                dgvDeficient.Columns[i].Name = nYear.ToString();

            }
            dgvDeficient.Columns[0].HeaderText = "Attribute";
            dgvDeficient.Columns[1].HeaderText = "Group";


            Hashtable hashIDYearsValues = new Hashtable();
            Hashtable hashYearValue;
            List<String> listTargetID = new List<String>();

            String strSelect = "SELECT TARGETID,YEARS,TARGETMET FROM TARGET_" + NetworkID + "_" + SimulationID + " WHERE ISDEFICIENT=1";
            try
            {
                String strYear = "";
                String strTargetMet = "";
                String strTargetID = "";

                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    strTargetID = row[0].ToString();
                    strYear = row[1].ToString();
                    strTargetMet = row[2].ToString();

                    if (hashIDYearsValues.Contains(strTargetID))
                    {
                        hashYearValue = (Hashtable)hashIDYearsValues[strTargetID];
                    }
                    else
                    {
                        hashYearValue = new Hashtable();
                        hashIDYearsValues.Add(strTargetID, hashYearValue);
                        listTargetID.Add(strTargetID);
                    }

                    hashYearValue.Add(strYear, strTargetMet);
                }

            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Loading targets. " + exception.Message);
                return;
            }



			strSelect = "SELECT ID_,ATTRIBUTE_,DEFICIENTNAME,DEFICIENT,PERCENTDEFICIENT,CRITERIA FROM DEFICIENTS WHERE SIMULATIONID='" + SimulationID + "'";

            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
            
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    object[] oDeficient = new object[Period + 2];
                    String strID = row[0].ToString();

                    oDeficient[0] = row[1].ToString();
                    oDeficient[1] = row[2].ToString();

                    String strDeficient = row[3].ToString();
                    String strPercentage = row[4].ToString();
                    String strCriteria = row[5].ToString();

                    hashYearValue = (Hashtable)hashIDYearsValues[strID];
                    
                    List<int> listDeficient = new List<int>();

                    foreach (String strKey in hashYearValue.Keys)
                    {
                        String strValue = (String)hashYearValue[strKey];
                        if (strValue == "") strValue = "0";
                        int nYear = int.Parse(strKey);
                        int nCol = nYear - StartYear + 2;
                        float fValue = float.Parse(strValue)*100;

                        oDeficient[nCol] = fValue.ToString() + "%/" + strPercentage + "% (" + strDeficient + ")";

                        float fPercentage = float.Parse(strPercentage);
                        if (fValue > fPercentage)
                        {
                            listDeficient.Add(nCol);
                        }
                    }
                    int nRow = dgvDeficient.Rows.Add(oDeficient);
                    dgvDeficient.Rows[nRow].Tag = strCriteria;

                    for (int i = 2; i < Period + 2; i++)
                    {
                        if (listDeficient.Contains(i))
                        {
                            dgvDeficient.Rows[nRow].Cells[i].Style.BackColor = Color.LightCoral;
                        }
                        else
                        {
                            dgvDeficient.Rows[nRow].Cells[i].Style.BackColor = Color.LightGreen;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Loading deficiency summary. " + exception.Message);
    
            }




            m_bChange = true;
        }

        private void dgvDeficient_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_bChange) return;
            if(dgvDeficient.Rows[e.RowIndex].Tag == null)return;
            textBoxDeficientCriteria.Text = dgvDeficient.Rows[e.RowIndex].Tag.ToString().Replace("|","'");






        }

		private void FormResultsDeficient_Load( object sender, EventArgs e )
		{
			SecureForm();
		}

		protected void SecureForm()
		{
			LockDataGridView( dgvDeficient );
			LockTextBox( textBoxDeficientCriteria );
			if( Global.SecurityOperations.CanModifySimulationResults( m_strNetworkID, m_strSimulationID ) )
			{
				UnlockDataGridViewForCreateDestroy( dgvDeficient );
				UnlockTextBox( textBoxDeficientCriteria );
			}
		}
    }
}
