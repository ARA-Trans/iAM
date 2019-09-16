using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;
using CalculateEvaluate;
using Simulation;
using System.Collections;
using System.CodeDom.Compiler;
using System.IO;
using RoadCareDatabaseOperations;
using RoadCareGlobalOperations;

namespace RoadCare3
{
    public partial class FormEditAssetEquation : Form
    {
        private String m_strEquation;
        private String m_strAttribute;
        public CalculateEvaluate.CalculateEvaluate calculate;
        //CalculateEvaluate.CalculateEvaluate calculateCheck;
        Hashtable m_hashFunction = new Hashtable();
        Hashtable m_hashColumnType = new Hashtable();
        String m_strType;
        List<String> m_listAttributes;

        public String Equation
        {
            get { return m_strEquation; }
            set { m_strEquation = value; }
        }
        
        public FormEditAssetEquation(String strEquation,String strAttribute,String strType)
        {
            InitializeComponent();
            m_strEquation = strEquation;
            m_strAttribute = strAttribute;
            m_strType = strType;
        }

        private void FormEditEquation_Load(object sender, EventArgs e)
        {
            List<String> listAttribute = new List<String>();
            try
            {
                DataSet ds = DBMgr.GetTableColumnsWithTypes(m_strAttribute);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    String strColumn = row["column_name"].ToString();
                    String strType = row["data_type"].ToString();
                    listBoxAttribute.Items.Add(strColumn);
                    m_hashColumnType.Add(strColumn, strType);
                }
            }
            catch(Exception error)
            {
                Global.WriteOutput("Error: Error in filling attribute list for Edit Equation." + error.Message);
            }

            textBoxEquation.Text = m_strEquation;
            FillDefault();
        }

        private void FillDefault()
        {
            List<String> listAssetProperties = new List<String>();
            foreach (DataGridViewRow row in dgvDefault.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    listAssetProperties.Add(row.Cells[0].Value.ToString());
                }

            }


            List<String> listAttributesEquation = new List<string>();
            ParseAsset(textBoxEquation.Text,out listAttributesEquation);
            foreach (String strAssetProperty in listAttributesEquation)
            {
                if(!listAssetProperties.Contains(strAssetProperty))
                {
                    dgvDefault.Rows.Add(strAssetProperty);
                }
            }
      
        }


        private bool CheckEquation()
        {

            textBoxCompile.Text = "";

            m_strEquation = textBoxEquation.Text;
            m_strEquation = m_strEquation.Trim();


            m_strEquation = ParseAsset(m_strEquation,out m_listAttributes);
            if (m_strEquation == "") return false;

            calculate = new CalculateEvaluate.CalculateEvaluate();
            //Determines if CodeDom returns a string or a double
            if (m_strType == "STRING")
            {
                calculate.BuildFunctionClass(m_strEquation, "String", null);
            }
            else
            { 
                calculate.BuildFunctionClass(m_strEquation, "double", null);
            }
            CompilerResults m_crEquation = calculate.CompileAssembly();


			if (calculate.m_listError.Count > 0)
            {
                foreach (String str in calculate.m_listError)
                {
                    textBoxCompile.Text = textBoxCompile.Text + str + "\r\n";
                }
            }

            if (textBoxCompile.Text.Length == 0)
            {
                textBoxCompile.Text = "Compilation sucessful. Results that appear in right grid calculated using default values.";
				SaveCompiledEquationToDatabase();
            }
            else
            {
                System.Media.SystemSounds.Exclamation.Play();
                return false;
            }
            FillDefault();

            return true;

        }

		private void SaveCompiledEquationToDatabase()
		{
            //try
            //{
            //    DBOp.SaveBinaryObjectToDatabase(m_performanceID, "PERFORMANCEID", "PERFORMANCE", "EQUATION_BINARY", compiledEquation);
            //    compiledEquation = DBOp.GetBinaryObjectFromDatabase(m_performanceID, "PERFORMANCEID", "PERFORMANCE", "EQUATION_BINARY");
            //}
            //catch (Exception exc)
            //{
            //    Global.WriteOutput("Error saving binary file to database. " + exc.Message);
            //}
			
				
		}

        private double GetCheckValue(String strAttribute)
        {
            double dReturn = 0;
            foreach (DataGridViewRow row in dgvDefault.Rows)
            {
                if (strAttribute == row.Cells[0].Value.ToString())
                {
                    double.TryParse(row.Cells[1].Value.ToString(), out dReturn);
                }
            }
            return dReturn;
        }

        private string GetCheckString(String strAttribute)
        {
            string strReturn = "";
            foreach (DataGridViewRow row in dgvDefault.Rows)
            {
                if (strAttribute == row.Cells[0].Value.ToString())
                {
                    strReturn = row.Cells[1].Value.ToString();
                }
            }
            return strReturn;
        }


        /// <summary>
        /// Checks MODULE for correct format.
        /// </summary>
        /// <returns></returns>
        private bool CheckModule()
        {
            return false;
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
             CheckEquation();
        }
        //public bool Solve(Hashtable hashAttributeValue, List<String> listAttributeEquation)
        //{
        //    double[] m_Answer = new double[100];
        //    object[] input = new object[listAttributeEquation.Count];
 
        //    int i = 0;
        //    int nAge = -1;
        //    foreach (String str in listAttributeEquation)
        //    {
        //        String strValue = Global.GetAttributeDefault(str);
        //        if (str == "AGE") nAge = i;
        //        if (Global.GetAttributeType(str) == "STRING")
        //        {
        //            input[i] = GetCheckString(str);
        //        }
        //        else
        //        {
        //            input[i] = GetCheckValue(str);
        //        }
        //        i++;
        //    }
        //    double dValue;
        //    double dMinimum;
        //    double dMaximum;

        //    for (int n = 0; n < 100; n++)
        //    {
        //        double dAge = (double)n;
        //        if (nAge >= 0)
        //        {
        //            input[nAge] = dAge;
        //        }


        //        object result = 0; ;
        //        try
        //        {
        //            result = calculateCheck.RunMethod(input);
        //        }
        //        catch(ArgumentException exception)
        //        {
        //            System.Media.SystemSounds.Exclamation.Play();
        //            textBoxCompile.Text = textBoxCompile.Text + "Error: " +  exception.Message + "\r\n";
        //            return false;
        //        }
                
        //        try
        //        {
        //            dValue = (double)result;
        //            if (Global.GetAttributeMaximum(m_strAttribute, out dMaximum))
        //            {
        //                if (dValue > dMaximum) dValue = dMaximum;
        //            }

        //            if (Global.GetAttributeMinimum(m_strAttribute, out dMinimum))
        //            {
        //                if (dValue < dMinimum) dValue = dMinimum;
        //            }
        //        }
        //        catch
        //        {
        //            String strValue = Global.GetAttributeDefault(m_strAttribute);
        //            dValue = double.Parse(strValue);
        //        }
        //        m_Answer[n] = dValue;
 
        //    }
        //    return true;
        //}

        private void listBoxAttribute_DoubleClick(object sender, EventArgs e)
        {

            int nCursor = textBoxEquation.SelectionStart;
            if (listBoxAttribute.SelectedItem == null) return;
            String strAttribute = listBoxAttribute.SelectedItem.ToString();
            
            strAttribute = "[" + strAttribute + "]";

            textBoxEquation.Text = textBoxEquation.Text.Insert(nCursor,strAttribute);
            textBoxEquation.SelectionStart = nCursor + strAttribute.Length;
            textBoxEquation.Focus();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (CheckEquation())
            {
                this.DialogResult = DialogResult.OK;
                m_strEquation = textBoxEquation.Text;
                this.Close();
            }
        }

 
 
        private void listBoxAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nLength = 0;
            if (textBoxEquation.SelectionLength > 0)
            {
                nLength = 0;
            }
            int nCursor = textBoxEquation.SelectionStart;
            if (nLength > 0)
            {
                textBoxEquation.Text = textBoxEquation.Text.Remove(nCursor, nLength);
            }
        }

        private String ParseAsset(String strExpression, out List<String> listAttributes)
        {
            listAttributes = new List<String>();
            listAttributes.Clear();
            String strAttribute;
            int nOpen = -1;

            for (int i = 0; i < strExpression.Length; i++)
            {
                if (strExpression.Substring(i, 1) == "[")
                {
                    nOpen = i;
                    continue;
                }

                if (strExpression.Substring(i, 1) == "]" && nOpen > -1)
                {
                    //Get the value between open and (i)
                    strAttribute = strExpression.Substring(nOpen + 1, i - nOpen - 1);
                    if (!listAttributes.Contains(strAttribute))
                    {
                        listAttributes.Add(strAttribute);
                    }
                }
            }


            foreach (String strProperty in listAttributes)
            {
                if (m_hashColumnType.Contains(strProperty))
                {
                    String strType = m_hashColumnType[strProperty].ToString();
                    if (strType == "varchar")
                    {
                        strExpression = strExpression.Replace("[" + strProperty + "]", "[@" + strProperty + "]");
                    }

                }
                else
                {
                    textBoxCompile.Clear();
                    textBoxCompile.Text = strProperty + " is not a property of asset type " + m_strAttribute;
                    return "";
                }
            }
            return strExpression;
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            List<String> listAssetsProperties;
            String strEquation = textBoxEquation.Text;
            if (!CheckEquation()) return;
            calculate.m_bCalculate = true;
            ParseAsset(strEquation, out listAssetsProperties);
            object[] values = new object[listAssetsProperties.Count];
            int i = 0;
            if (listAssetsProperties.Count > 0)
            {
                foreach (DataGridViewRow row in dgvDefault.Rows)
                {

                    if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                    {
                        String asset = row.Cells[0].Value.ToString();
                        String strValue = row.Cells[1].Value.ToString();
                        String strType = m_hashColumnType[asset].ToString();
                        if (strType != "varchar")
                        {
                            values[i] = double.Parse(strValue);
                        }
                        else
                        {
                            values[i] = strValue;

                        }
                        i++;
                    }
                }
            }
			try
			{
				object result = calculate.RunMethod(values);
				textBoxReturn.Text = result.ToString();
			}
			catch(Exception exc)
			{
				Global.WriteOutput("Error in asset equation. " + exc.Message);
			}
        }
    }
}
