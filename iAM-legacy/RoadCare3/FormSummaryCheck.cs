using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;
using System.Collections;
using RoadCareDatabaseOperations;

namespace RoadCare3
{
    public partial class FormSummaryCheck : Form
    {
        private Hashtable hashNetworkNetworkID = new Hashtable();
        private Hashtable hashSimulationSimulationID = new Hashtable();
        public FormSummaryCheck()
        {
            InitializeComponent();
        }

        private void FormSummaryCheck_Load(object sender, EventArgs e)
        {
            Global.LoadAttributes();
            foreach (String str in Global.Attributes)
            {
                comboBoxAttribute.Items.Add(str);
            }


            String strSelect = "SELECT NETWORK_NAME, NETWORKID FROM NETWORKS";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                String strNetwork = row["NETWORK_NAME"].ToString();
                String strNetworkID = row["NETWORKID"].ToString();

                comboBoxNetwork.Items.Add(strNetwork);
                hashNetworkNetworkID.Add(strNetwork, strNetworkID);
            }

        }
        
        
        
        
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void comboBoxNetwork_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxSimulation.Items.Clear();
            hashSimulationSimulationID.Clear();

            String strNetwork = comboBoxNetwork.Text.ToString();
            String strNetworkID = hashNetworkNetworkID[strNetwork].ToString();

            String strSelect = "SELECT SIMULATION, SIMULATIONID FROM SIMULATIONS WHERE NETWORKID='" + strNetworkID + "'";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                String strSimulation = row["SIMULATION"].ToString();
                String strSimulationID = row["SIMULATIONID"].ToString();

                comboBoxSimulation.Items.Add(strSimulation);
                hashSimulationSimulationID.Add(strSimulation, strSimulationID);
            }
            comboBoxSimulation.Items.Add("");
        }

        private void comboBoxSummaryType_SelectedIndexChanged(object sender, EventArgs e)
        {
            String strType = comboBoxSummaryType.Text;
            comboBoxMethod.Items.Clear();

            switch (strType)
            {
                case "Number Summary":
                    comboBoxMethod.Items.Add("SUM");
                    comboBoxMethod.Items.Add("AVG");
                    comboBoxMethod.Items.Add("MIN");
                    comboBoxMethod.Items.Add("MAX");
                    break;

                case "Area Summary":
                    comboBoxMethod.Items.Add("PERCENTAGE");
                    comboBoxMethod.Items.Add("AREA");
                    break;
            }


        }



        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            String str = comboBoxSummaryType.Text;
            if (str == "") return;

            String strNetwork = comboBoxNetwork.Text;
            if(strNetwork == "") return;
            String strNetworkID = hashNetworkNetworkID[strNetwork].ToString();

            String strSimulation = comboBoxSimulation.Text;
            String strSimulationID = "";
            if(strSimulation != "")
            {
                strSimulationID = hashSimulationSimulationID[strSimulation].ToString();
            }
            String strAttribute = comboBoxAttribute.Text;
            if(strAttribute == "") return;

            String strCriteria = textBoxCriteria.Text;

            String strMethod = comboBoxMethod.Text;
            if(strMethod == "") return;

            String strYear = textBoxYear.Text;
            dgvLevelSummary.Rows.Clear();
            dgvSolution.Rows.Clear();
            String strValue;
            switch (str)
            {
                case "Number Summary":
                    strValue = DBOp.GetConditionSummary(strNetworkID, strSimulationID, strAttribute, strYear, strMethod, strCriteria);
                    dgvSolution.Rows.Add(strAttribute, strValue);
                    break;
                case "Area Summary":
                    List<String> listAttributes;
                    Hashtable hash = DBOp.GetPercentagePerStringAttribute(strNetworkID, strSimulationID, strAttribute, strYear, strMethod, strCriteria,false,out listAttributes);
                    foreach (String strValues in listAttributes)
                    {
                        dgvSolution.Rows.Add(strValues, hash[strValues].ToString());
                    }
                    //Get Levels
                    List<float> listLevel = GetLevels(strAttribute);
                    if (listLevel != null)
                    {
                        BuildLevelSummary(listLevel);
                    }
                    break;
            }

        }
        private void BuildLevelSummary(List<float> listFloat)
        {
            if (listFloat.Count == 0)
            {
                Global.WriteOutput("Error: Set Levels under Attribute Properties.");
                return;
            }
            float[] fLevelArea = new float[7]{0,0,0,0,0,0,0};
            
            if (listFloat[4] > listFloat[0])   //Last is greater than first
            {
                foreach (DataGridViewRow dr in dgvSolution.Rows)
                {
                    if (dr.Cells[0].Value == null) continue;
                    String strValue = dr.Cells[0].Value.ToString();
                    float fArea = float.Parse(dr.Cells[1].Value.ToString());
                    if (strValue != "NULL" && strValue != "")
                    {
                        float fValue = float.Parse(dr.Cells[0].Value.ToString());

                        if (fValue <= listFloat[0]) fLevelArea[0] += fArea;
                        else if (fValue > listFloat[0] && fValue <= listFloat[1]) fLevelArea[1] += fArea;
                        else if (fValue > listFloat[1] && fValue <= listFloat[2]) fLevelArea[2] += fArea;
                        else if (fValue > listFloat[2] && fValue <= listFloat[3]) fLevelArea[3] += fArea;
                        else if (fValue > listFloat[3] && fValue <= listFloat[4]) fLevelArea[4] += fArea;
                        else if (fValue > listFloat[4]) fLevelArea[5] += fArea;
                    }
                    else if (strValue == "NULL") //NULL values
                    {
                        fLevelArea[6] += fArea;
                    }
                }
                dgvLevelSummary.Rows.Add("<= " + listFloat[0].ToString(), fLevelArea[0].ToString());
                dgvLevelSummary.Rows.Add("<= " + listFloat[1].ToString(), fLevelArea[1].ToString());
                dgvLevelSummary.Rows.Add("<= " + listFloat[2].ToString(), fLevelArea[2].ToString());
                dgvLevelSummary.Rows.Add("<= " + listFloat[3].ToString(), fLevelArea[3].ToString());
                dgvLevelSummary.Rows.Add("<= " + listFloat[4].ToString(), fLevelArea[4].ToString());
                dgvLevelSummary.Rows.Add(">" + listFloat[4].ToString(), fLevelArea[5].ToString());
                dgvLevelSummary.Rows.Add("No Data", fLevelArea[6].ToString());
            }
            else // First is greater than last
            {
                foreach (DataGridViewRow dr in dgvSolution.Rows)
                {
                    if (dr.Cells[0].Value == null) continue;
                    String strValue = dr.Cells[0].Value.ToString();
                    float fArea = float.Parse(dr.Cells[1].Value.ToString());
                    if (strValue != "NULL" && strValue != "")
                    {
                        float fValue = float.Parse(dr.Cells[0].Value.ToString());

                        if (fValue >= listFloat[0]) fLevelArea[0] += fArea;
                        else if (fValue < listFloat[0] && fValue >= listFloat[1]) fLevelArea[1] += fArea;
                        else if (fValue < listFloat[1] && fValue >= listFloat[2]) fLevelArea[2] += fArea;
                        else if (fValue < listFloat[2] && fValue >= listFloat[3]) fLevelArea[3] += fArea;
                        else if (fValue < listFloat[3] && fValue >= listFloat[4]) fLevelArea[4] += fArea;
                        else if (fValue < listFloat[4]) fLevelArea[5] += fArea;
                    }
                    else if(strValue=="NULL") //NULL values
                    {
                        fLevelArea[6] += fArea;
                    }
                }
                dgvLevelSummary.Rows.Add(">= " + listFloat[0].ToString(), fLevelArea[0].ToString());
                dgvLevelSummary.Rows.Add(">= " + listFloat[1].ToString(), fLevelArea[1].ToString());
                dgvLevelSummary.Rows.Add(">= " + listFloat[2].ToString(), fLevelArea[2].ToString());
                dgvLevelSummary.Rows.Add(">= " + listFloat[3].ToString(), fLevelArea[3].ToString());
                dgvLevelSummary.Rows.Add(">= " + listFloat[4].ToString(), fLevelArea[4].ToString());
                dgvLevelSummary.Rows.Add("<" + listFloat[4].ToString(), fLevelArea[5].ToString());
                dgvLevelSummary.Rows.Add("No Data", fLevelArea[6].ToString());

            }

        }








        private List<float> GetLevels(String strAttribute)
        {
            List<float> listLevels = null;

            String strSelect = "SELECT TYPE_,LEVEL1, LEVEL2, LEVEL3, LEVEL4, LEVEL5 FROM ATTRIBUTES_ WHERE ATTRIBUTE_='" + strAttribute + "'";
            try
            {
                DataSet dsLevel = DBMgr.ExecuteQuery(strSelect);
                DataRow dr = dsLevel.Tables[0].Rows[0];

                if(dr["TYPE_"].ToString() == "NUMBER")
                {
                    listLevels = new List<float>();
                    listLevels.Add(float.Parse(dr["LEVEL1"].ToString()));
                    listLevels.Add(float.Parse(dr["LEVEL2"].ToString()));
                    listLevels.Add(float.Parse(dr["LEVEL3"].ToString()));
                    listLevels.Add(float.Parse(dr["LEVEL4"].ToString()));
                    listLevels.Add(float.Parse(dr["LEVEL5"].ToString()));
                }
            }
            catch(Exception exception)
            {
                Global.WriteOutput("Error: Filling level list." + exception.Message);
                listLevels.Clear();
            }

            return listLevels;
        }
    }
}