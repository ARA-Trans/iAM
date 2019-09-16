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
    public partial class FormResultsTarget : ToolWindow
    {
        private String m_strSimulationID;
        private String m_strNetworkID;
        private int m_nStart;
        private int m_nPeriod;
        private bool m_bChange = false;
        private Hashtable m_hashIDTargets = new Hashtable();

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


        public FormResultsTarget()
        {
            InitializeComponent();
        }

        private void FormResultsTarget_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormManager.RemoveResultsTargetWindow();
        }

        private void FormResultsTarget_Load(object sender, EventArgs e)
        {
			SecureForm();
        }

		protected void SecureForm()
		{
			LockDataGridView( dgvTarget );
			LockTextBox( textBoxTargetCriteria );
			if( Global.SecurityOperations.CanModifySimulationResults( m_strNetworkID, m_strSimulationID ) )
			{
				UnlockDataGridViewForCreateDestroy( dgvTarget );
				UnlockTextBox( textBoxTargetCriteria );
			}
		}


        public void UpdateTargetGrid()
        {

            m_bChange = false;
            m_hashIDTargets.Clear();

            dgvTarget.Rows.Clear();
            dgvTarget.Columns.Clear();
            dgvTarget.ReadOnly = true;
            if (Period == 0) return;

            dgvTarget.ColumnCount = 2 + this.Period;
            for (int i = 2; i < Period + 2; i++)
            {
                int nYear = StartYear + i - 2;
                dgvTarget.Columns[i].HeaderText = nYear.ToString();
                dgvTarget.Columns[i].Name = nYear.ToString();

            }
            dgvTarget.Columns[0].HeaderText = "Attribute";
            dgvTarget.Columns[1].HeaderText = "Group";


            Hashtable hashIDYearsValues = new Hashtable();
            Hashtable hashYearValue;
            List<String> listTargetID = new List<String>();

            String strSelect = "SELECT TARGETID,YEARS,TARGETMET FROM TARGET_" + NetworkID + "_" + SimulationID + " WHERE ISDEFICIENT=0";
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
                        hashIDYearsValues.Add(strTargetID,hashYearValue);
                        listTargetID.Add(strTargetID);
                    }

                    hashYearValue.Add(strYear, strTargetMet);
                }
           
            }
            catch(Exception exception)
            {
                Global.WriteOutput("Error: Loading targets. " + exception.Message);
                return;
            }


            List<String> listRows = new List<string>();
            Hashtable hashIDRowKey = new Hashtable();

			strSelect = "SELECT ID_,ATTRIBUTE_,YEARS,TARGETMEAN,TARGETNAME,CRITERIA FROM TARGETS WHERE SIMULATIONID='" + SimulationID + "'";
            try
            {
                String strRowKey;

                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    TargetParameters target = new TargetParameters();

                    target.m_strID = row[0].ToString();
                    target.m_strAttribute = row[1].ToString();
                    target.m_strYear = row[2].ToString();
                    target.m_strTargetMean = row[3].ToString();
                    target.m_strName = row[4].ToString();
                    target.m_strCriteria = row[5].ToString();

                    strRowKey = target.m_strAttribute + "|" + target.m_strName + "|" + target.m_strCriteria;


                    if (!listRows.Contains(strRowKey))
                    {
                        listRows.Add(strRowKey);
                        target.m_nRow = listRows.Count - 1;
                    }
                    else
                    {
                        target.m_nRow = listRows.IndexOf(strRowKey);
                    }
                  
                    m_hashIDTargets.Add(target.m_strID, target);

                }

            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Loading targets. " + exception.Message);
                return;
            }

            foreach (string strRowKey in listRows)
            {
                string[] rowHeader = strRowKey.Split('|');
                object[] oRow = {rowHeader[0].ToString(),rowHeader[1].ToString()};
                dgvTarget.Rows.Add(oRow);
            }



            foreach (String strID in listTargetID)
            {
                hashYearValue = (Hashtable)hashIDYearsValues[strID];
                TargetParameters target = (TargetParameters)m_hashIDTargets[strID];
                if (target == null) continue;

                foreach (String strKey in hashYearValue.Keys)
                {
                    String strValue = (String)hashYearValue[strKey];

                    dgvTarget.Rows[target.m_nRow].Cells[strKey].Value = strValue.ToString() + "/" + target.m_strTargetMean.ToString() ;
                    dgvTarget.Rows[target.m_nRow].Cells[strKey].Tag = strID;

                    float fValue = 0;
                    float fTarget = 0;
                    float.TryParse(strValue, out fValue);
                    float.TryParse(target.m_strTargetMean, out fTarget);


                    if (Global.GetAttributeAscending(target.m_strAttribute))
                    {

                        if (fValue < fTarget)
                        {
                            dgvTarget.Rows[target.m_nRow].Cells[strKey].Style.BackColor = Color.LightCoral;
                        }
                        else
                        {
                            dgvTarget.Rows[target.m_nRow].Cells[strKey].Style.BackColor = Color.LightGreen;
                        }
                    }
                    else
                    {
                        if (fValue > fTarget)
                        {
                            dgvTarget.Rows[target.m_nRow].Cells[strKey].Style.BackColor = Color.LightCoral;
                        }
                        else
                        {
                            dgvTarget.Rows[target.m_nRow].Cells[strKey].Style.BackColor = Color.LightGreen;
                        }
                    }
                }
            }
            m_bChange = true;
        }

        private void dgvTarget_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_bChange) return;
            //Get the Criteria from the TreatmentID tag.
            textBoxTargetCriteria.Text = "";
            if (dgvTarget.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag != null)
            {
                String strTag = dgvTarget.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag.ToString();
                TargetParameters target = (TargetParameters)m_hashIDTargets[strTag];
                textBoxTargetCriteria.Text = target.m_strCriteria.Replace("|","'");
            }
        }
    }

    public class TargetParameters
    {
        public String m_strID;
        public String m_strYear;
        public String m_strAttribute;
        public String m_strTargetMean;
        public String m_strName;
        public String m_strCriteria;
        public int m_nRow;
    }
}
