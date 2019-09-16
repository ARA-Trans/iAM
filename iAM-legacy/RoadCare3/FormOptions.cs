using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;
using System.Collections;
using System.Data.SqlClient;

namespace RoadCare3
{
    public partial class FormOptions : Form
    {
        private Hashtable m_hashOptions = new Hashtable();
        public FormOptions()
        {
            InitializeComponent();
        }

        private void FormOptions_Load(object sender, EventArgs e)
        {
            String strSelect = "SELECT OPTION_NAME,OPTION_VALUE FROM OPTIONS";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    String strOptionName = row["OPTION_NAME"].ToString();
                    String strOptionValue = row["OPTION_VALUE"].ToString();

                    m_hashOptions.Add(strOptionName, strOptionValue);
                    dgvOptions.Rows.Add(strOptionName, strOptionValue);
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Loading Options table." + exception.Message);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
			String strOptionName = "";
			String strOptionNewValue = "";
            foreach (DataGridViewRow row in dgvOptions.Rows)
            {
				if (row.Cells[0].Value != null)
				{
					strOptionName = row.Cells[0].Value.ToString();

				}
				else
				{
					continue;
				}
				if (row.Cells[1].Value != null)
				{
					strOptionNewValue = row.Cells[1].Value.ToString();
				}
				else
				{
					continue;
				}

                String strOptionOldValue = m_hashOptions[strOptionName].ToString();

                if (strOptionName == "AREA_CALCULATION")
                {
                    if (!CheckLegalAreaEquations(strOptionNewValue))
                    {
                        return;
                    }
                }


                if (strOptionOldValue != strOptionNewValue)
                {
                    String strUpdate = "UPDATE OPTIONS SET OPTION_VALUE='" + strOptionNewValue + "' WHERE OPTION_NAME='" + strOptionName + "'";
                    try
                    {
                        DBMgr.ExecuteNonQuery(strUpdate);
                    }
                    catch(Exception exception)
                    {
                        Global.WriteOutput("Error: Updating Options." + exception.Message);
                    }
                }
            }
            this.Close();
        }


        private bool CheckLegalAreaEquations(String strArea)
        {
            if (strArea.Trim() == "")
            {
                Global.WriteOutput("Error: Definition for CALCULATION_AREA required");
                return false;
            }
            List<String> m_listArea = new List<String>();

            string[] listAreaParameters = strArea.Split(']');
            for (int i = 0; i < listAreaParameters.Length; i++)
            {
                if (listAreaParameters[i].Contains("["))
                {
                    m_listArea.Add(listAreaParameters[i].Substring(listAreaParameters[i].IndexOf('[') + 1));
                }
            }
            Global.LoadAttributes();

            foreach (String str in m_listArea)
            {
                if (str != "LENGTH" && str != "AREA")
                {

                    if (!Global.Attributes.Contains(str))
                    {
                        Global.WriteOutput("Error: Only [LENGTH], [AREA] and Attributes may be used to define AREA.  Unrecognized attributes " + str);
                        return false;
                    }
                }
            }

            CalculateEvaluate.CalculateEvaluate calculate = new CalculateEvaluate.CalculateEvaluate();
            calculate.BuildTemporaryClass(strArea,true);
            calculate.CompileAssembly();
            if(calculate.m_listError.Count > 0)
            {
                Global.WriteOutput("Error: Compiling area equations. " + calculate.m_listError[0].ToString());
                return false;
            }
            return true;
        }
    }
}
