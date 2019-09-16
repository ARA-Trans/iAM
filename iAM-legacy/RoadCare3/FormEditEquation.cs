using DatabaseManager;
using DataObjects;
using Simulation;
using SimulationDataAccess.DTO;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace RoadCare3
{
    public partial class FormEditEquation : Form
    {
        private String m_strEquation;
        private String m_strAttribute;
        public CalculateEvaluate.CalculateEvaluate calculate;
        CalculateEvaluate.CalculateEvaluate calculateCheck;
        Hashtable m_hashFunction = new Hashtable();
        private bool m_bArea;
        private bool m_bCalculatedField = false;
        public bool IsConsequence { get; set; }
        private bool m_isPiecewise;
        private bool m_showPiecewise = false;
        private bool m_isFunction;

        private Performance _performance;

        public bool CalculatedField
        {
            get { return m_bCalculatedField; }
            set { m_bCalculatedField = value; }
        }

        public String Equation
        {
            get { return m_strEquation; }
            set { m_strEquation = value; }
        }

        public bool IsPiecewise
        {
            get { return m_isPiecewise; }
        }

        public bool IsFunction
        {
            get { return m_isFunction; }
        }

        /// <summary>
        /// Edit equations functions for CodeDom compilation
        /// </summary>
        /// <param name="performance"></param>
        public FormEditEquation(Performance performance)
        {
            InitializeComponent();
            m_strEquation = performance.Equation;
            m_strAttribute = performance.Attribute;
            m_bArea = false;
            IsConsequence = false;
            m_isPiecewise = performance.IsPiecewise;
            m_isFunction = performance.IsFunction;
            m_showPiecewise = true;
            checkBoxPiecewise.Visible = true;
            checkBoxPiecewise.Checked = m_isPiecewise;
            checkBoxAsFunction.Checked = m_isFunction;
            EnablePiecewise(m_isPiecewise);
            _performance = performance;
        }


        /// <summary>
        /// Edit equation for compound treatments
        /// </summary>
        /// <param name="strEquation"></param>
        /// <param name="strAttribute"></param>
        public FormEditEquation(String strEquation,String strAttribute)
        {
            InitializeComponent();
            m_strEquation = strEquation;
            m_strAttribute = strAttribute;
            m_bArea = false;
            IsConsequence = false;
            m_showPiecewise = false;
            checkBoxPiecewise.Visible = false;
            EnablePiecewise(false);
        }

        /// <summary>
        /// Edit equations where functions can be entered
        /// </summary>
        /// <param name="strEquation"></param>
        /// <param name="isFunction"></param>
        public FormEditEquation(String strEquation, bool isFunction)
        {
            InitializeComponent();
            m_strEquation = strEquation;
            m_strAttribute = "";
            m_bArea = true;
            m_isFunction = isFunction;
            EnablePiecewise(false);
        }
        
        /// <summary>
        /// Edit equations for calculated fields
        /// </summary>
        /// <param name="strEquation"></param>
        /// <param name="isIncludeArea"></param>
        /// <param name="isFunction"></param>
        public FormEditEquation(String strEquation, bool isIncludeArea, bool isFunction)
        {
            InitializeComponent();
            m_strEquation = strEquation;
            m_strAttribute = "";
            m_bArea = isIncludeArea;
            m_isFunction = isFunction;
            EnablePiecewise(false);
        }

        private void FormEditEquation_Load(object sender, EventArgs e)
        {
            richTextBoxEquation.SelectionTabs = new int[] { 25, 50, 75, 100 };
            Global.Attributes.Clear();
            Global.LoadAttributes();
            List<String> listAttribute = new List<String>();
   
            if (m_bArea)
            {
                listBoxAttribute.Items.Add("AREA");
                listBoxAttribute.Items.Add("LENGTH");
            }

            richTextBoxEquation.Text = m_strEquation;

            if (m_isPiecewise)
            {
                Piecewise pw = new Piecewise(richTextBoxEquation.Text);
                foreach (AgeValue ageValue in pw.AgeValueList)
                {
                    dgvPerformance.Rows.Add(ageValue.Age, ageValue.Value);
                }
            }
            checkBoxAsFunction.Checked = m_isFunction;
            ReloadAttributes();

            m_hashFunction.Add("E", "Represents the natural logarithmic base, specified by the constant, e.");
            m_hashFunction.Add("PI", "Represents the ratio of the circumference of a circle to its diameter, specified by the constant, pi.");

            m_hashFunction.Add("Abs(x)", "Returns the absolute value of a specified number.");
            m_hashFunction.Add("Acos(x)", "Returns the angle whose cosine is the specified number.");
            m_hashFunction.Add("Asin(x)", "Returns the angle whose sine is the specified number.");
            m_hashFunction.Add("Atan(x)", "Returns the angle whose tangent is the specified number.");
            m_hashFunction.Add("Atan(x,y)", "Returns the angle whose tangent is the quotient of two specified numbers.");
            m_hashFunction.Add("Ceiling(x)", "Returns the smallest whole number greater than or equal to the specified number.");
            m_hashFunction.Add("Cos(x)", "Returns the cosine of the specified angle.");
            m_hashFunction.Add("Cosh(x)", "Returns the hyperbolic cosine of the specified angle.");
            m_hashFunction.Add("Exp(x)", "Returns e raised to the specified power.");
            m_hashFunction.Add("Floor(x)", "Returns the largest whole number less than or equal to the specified number.");
            m_hashFunction.Add("IEEERemainder(x,y)", "Returns the remainder resulting from the division of a specified number by another specified number.");
            m_hashFunction.Add("Log(x)", "Returns the logarithm of a specified number.");
            m_hashFunction.Add("Log10(x)", "Returns the base 10 logarithm of a specified number.");
            m_hashFunction.Add("Max(x,y)", "Returns the larger of two specified numbers.");
            m_hashFunction.Add("Min(x,y)", "Returns the smaller of two numbers.");
            m_hashFunction.Add("Pow(x,y)", "Returns a specified number raised to the specified power.");
            m_hashFunction.Add("Round(x)", "Returns the number nearest the specified value.");
            m_hashFunction.Add("Sign(x)", "Returns a value indicating the sign of a number.");
            m_hashFunction.Add("Sin(x)", "Returns the sine of the specified angle.");
            m_hashFunction.Add("Sinh(x)", "Returns the hyperbolic sine of the specified angle.");
            m_hashFunction.Add("Sqrt(x)", "Returns the square root of a specified number.");
            m_hashFunction.Add("Tan(x)", "Returns the tangent of the specified angle.");
            m_hashFunction.Add("Tanh(x)", "Returns the hyperbolic tangent of the specified angle.");

            FillDefault();
        }

        private void FillDefault()
        {
            List<String> listError;
            //Get new attributes 
            List<String> listAttributesEquation = Global.TryParseAttribute(m_strEquation, out listError);
            //Get current attributes and values
            Hashtable hashAttributeValue = new Hashtable();

            foreach (DataGridViewRow row in dgvDefault.Rows)
            {
                hashAttributeValue.Add(row.Cells[0].Value, row.Cells[1].Value);
            }
            dgvDefault.Rows.Clear();

            foreach (String strAttribute in listAttributesEquation)
            {
                string strValue;
                if (hashAttributeValue.Contains(strAttribute))
                {
                    strValue = (string)hashAttributeValue[strAttribute];
                }
                else
                {
                    if (strAttribute == "AREA")
                    {
                        strValue = "5.0";
                    }
                    else if (strAttribute == "LENGTH")
                    {
                        strValue = "1.0";
                    }
                    else
                    {
                        strValue = Global.GetAttributeDefault(strAttribute);
                    }
                }
                dgvDefault.Rows.Add(strAttribute, strValue);
            }
        }

        private bool CheckAreaEquation()
        {

            textBoxCompile.Text = "";
            m_strEquation = richTextBoxEquation.Text;
            m_strEquation = m_strEquation.Trim();
            List<string> listError;
            List<String> listAttributesEquation = Global.TryParseAttribute(m_strEquation, out listError);            // See if listAttributeEquations is included in dgvDefault
            if (listError.Count > 0)
            {
                foreach (string str in listError)
                {
                    textBoxCompile.Text = textBoxCompile.Text + str + "\r\n";
                }
                return false;

            }
            FillDefault();

            if (m_strEquation.Trim().Length == 0)
            {
                textBoxCompile.Text = "Equation must be entered before selecting Check or OK.";
                System.Media.SystemSounds.Exclamation.Play();
                return false;
            }

            // Get list of attributes
            calculate = new CalculateEvaluate.CalculateEvaluate();

            bool isFunction = checkBoxAsFunction.Checked;
            if (isFunction) //Change to allow function equation to accept strings as properties.
            {
                string equation = m_strEquation;
                foreach (String attribute in listAttributesEquation)
                {
                    String attributeType = SimulationMessaging.GetAttributeType(attribute);
                    if (attributeType == "STRING")
                    {
                        String oldValue = "[" + attribute + "]";
                        String newValue = "[@" + attribute + "]";
                        equation = equation.Replace(oldValue, newValue);
                    }
                    else if (attributeType == "DATETIME")
                    {
                        String oldValue = "[" + attribute + "]";
                        String newValue = "[$" + attribute + "]";
                        equation = equation.Replace(oldValue, newValue);
                    }
                }
                calculate.BuildFunctionClass(equation, "double", null);
            }
            else
            {
                calculate.BuildTemporaryClass(m_strEquation, true);
            }
			try
			{
				CompilerResults m_crEquation = calculate.CompileAssembly();

				if( calculate.m_listError.Count > 0 )
				{
					foreach( String str in calculate.m_listError )
					{
						textBoxCompile.Text = textBoxCompile.Text + str + "\r\n";
					}
				}

				if( textBoxCompile.Text.Length == 0 )
				{
					textBoxCompile.Text = "Compilation sucessful. Results that appear in right grid calculated using default values.";
				}
				else
				{
					System.Media.SystemSounds.Exclamation.Play();
					return false;
				}
				//If [AGE] is the only variable.  This only needs to be solved once so might 
				//as well solve it right now.

				dgvPerformance.Rows.Clear();

				object[] input = new object[dgvDefault.Rows.Count];
				int i = 0;
				foreach( DataGridViewRow row in dgvDefault.Rows )
				{
                    string attribute = row.Cells[0].Value.ToString();
                    if(Global.GetAttributeType(attribute) == "STRING")
                    {
                        input[i] = row.Cells[1].Value;
                    }
                    else
                    {
                        double value = 0;
                        Double.TryParse(row.Cells[1].Value.ToString(), out value);
                        input[i] = value;
                    }


                    
					i++;
				}
				object result = calculate.RunMethod(input);

				calculate.m_assemblyInstance = null;
				calculate.methodInfo = null;

				dgvPerformance.ColumnCount = 1;
				if( this.CalculatedField )
				{
					dgvPerformance.Columns[0].HeaderText = "CALCULATED";
				}
				else
				{
					dgvPerformance.Columns[0].HeaderText = "COST";
				}
				dgvPerformance.Rows.Add( result );

				return true;
			}
			catch( Exception ex )
			{
				Global.WriteOutput( "ERROR: could not compile equation:" + ex.Message );
				return false;		
            }

        }

        /// <summary>
        /// Retrieves default values for showing typical equation results (i.e. numbers to but into input equation).
        /// </summary>
        /// <returns></returns>
        public Hashtable GetDefaultValues()
        {
            Hashtable values = new Hashtable();
            foreach (DataGridViewRow row in dgvDefault.Rows)
            {
                double d = 0;
                String strValue = row.Cells[1].Value.ToString();
                double.TryParse(strValue, out d);
                values.Add(row.Cells[0].Value.ToString(), d);
            }
            return values;
        }

		private bool CheckEquation()
        {

            bool isFunction = checkBoxAsFunction.Checked;

            textBoxCompile.Text = "";
            m_strEquation = richTextBoxEquation.Text;
            m_strEquation = m_strEquation.Trim();
            List<string> listError;
            List<String> listAttributesEquation = Global.TryParseAttribute(m_strEquation,out listError);            // See if listAttributeEquations is included in dgvDefault
            if (listError.Count > 0)
            {
                foreach (string str in listError)
                {
                    textBoxCompile.Text = textBoxCompile.Text + str + "\r\n";
                }
                return false;
            }
            FillDefault();
            if (m_strEquation.Length > 6)
            {
                if (m_strEquation.Substring(0, 6) == "MODULE")
                {
                    return CheckModule();
                }
                else if (m_strEquation.Contains("COMPOUND_TREATMENT")) return true;
            }

            if (m_strEquation.Trim().Length == 0)
            {
                textBoxCompile.Text = "Equation must be entered before selecting Check or OK.";
                System.Media.SystemSounds.Exclamation.Play();
                return false;
            }
            
            calculateCheck = new CalculateEvaluate.CalculateEvaluate();

            if (isFunction) //Change to allow function equation to accept strings as properties.
            {
                string equation = m_strEquation;
                foreach (String attribute in listAttributesEquation)
                {
                    String attributeType = SimulationMessaging.GetAttributeType(attribute);
                    if (attributeType == "STRING")
                    {
                        String oldValue = "[" + attribute + "]";
                        String newValue = "[@" + attribute + "]";
                        equation = equation.Replace(oldValue, newValue);
                    }
                    else if (attributeType == "DATETIME")
                    {
                        String oldValue = "[" + attribute + "]";
                        String newValue = "[$" + attribute + "]";
                        equation = equation.Replace(oldValue, newValue);
                    }
                }
                calculateCheck.BuildFunctionClass(equation,"double",null);
            }
            else
            {
                calculateCheck.BuildTemporaryClass(m_strEquation, true);
            }
			try
			{
                CompilerResults m_crEquation = calculateCheck.CompileAssembly();
                if (calculateCheck.m_listError.Count > 0)
				{
					foreach( String str in calculate.m_listError )
					{
						textBoxCompile.Text = textBoxCompile.Text + str + "\r\n";
					}
				}

				if( textBoxCompile.Text.Length == 0 )
				{
					textBoxCompile.Text = "Compilation successful. Results that appear in right grid calculated using default values.";
				}
				else
				{
					System.Media.SystemSounds.Exclamation.Play();
					return false;
				}
				//If [AGE] is the only variable.  This only needs to be solved once so might 
				//as well solve it right now.

                //Equations that are based only on age can only have 100 distinct values.   Fill the solution matrix and look up by age.
                Hashtable hash = GetDefaultValues();
				if( !Solve( hash, listAttributesEquation ) )
				{
					return false;
				}
				return true;
			}
			catch( Exception ex )
			{
				textBoxCompile.Text =  "ERROR: unable to compile equation: " + ex.Message;
				return false;  
			}
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
            //Currently check only CRS
            if (m_strEquation.Substring(7, 3) == "CRS")
            {
                List<String> listAttributesEquation = Global.ParseAttribute(m_strEquation);            // See if listAttributeEquations is included in dgvDefault
                if (listAttributesEquation.Count != 6) return false;

                CRS crs = new CRS();
                String strType = GetCheckString(listAttributesEquation[0].ToString());
                double dCRSOld = GetCheckValue(listAttributesEquation[1].ToString());
                double dThick = GetCheckValue(listAttributesEquation[2].ToString());
                double dESAL = GetCheckValue(listAttributesEquation[3].ToString());
                double dESALGrowth = GetCheckValue(listAttributesEquation[4].ToString());
                double dAge = GetCheckValue(listAttributesEquation[5].ToString());
                double dBenefit = 0;
                double dRSL = 0;
                double dMaximum;
                double dMinimum;
                dgvPerformance.Rows.Clear();
                dgvPerformance.Rows.Add(0, dCRSOld);
                for (int i = 1; i < 99; i++)
                {
                    dAge = (double)i;
                    double dValue = crs.CalculateCRSBenefit(false, strType, dCRSOld, dThick, dESAL, dESALGrowth, 1, dAge, 3, out dBenefit, out dRSL); 
                    if (Global.GetAttributeMaximum(m_strAttribute, out dMaximum))
                    {
                        if (dValue > dMaximum) dValue = dMaximum;
                    }

                    if (Global.GetAttributeMinimum(m_strAttribute, out dMinimum))
                    {
                        if (dValue < dMinimum) dValue = dMinimum;
                    }

                    dgvPerformance.Rows.Add(i.ToString(),dValue.ToString("f2"));
                }
                return true;
            }
            else if (m_strEquation.Substring(7, 3) == "OCI")
            {
                return true;
            }
            return false;
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            if (m_isPiecewise)
            {
                CheckPiecewise();
            }
            else
            {
                if (richTextBoxEquation.Text.Contains("COMPOUND_TREATMENT("))
                {
                    textBoxCompile.Text = richTextBoxEquation.Text + " compiles properly";
                }
                else
                {
                    if (m_bArea)
                    {
                        CheckAreaEquation();
                    }
                    else
                    {
                        CheckEquation();
                    }
                }
            }
        }
        public bool Solve(Hashtable hashAttributeValue, List<String> listAttributeEquation)
        {
            double[] m_Answer = new double[100];
            object[] input = new object[listAttributeEquation.Count];
            dgvPerformance.Rows.Clear();

            int i = 0;
            int nAge = -1;
            foreach (String str in listAttributeEquation)
            {
                String strValue = Global.GetAttributeDefault(str);
                if (str == "AGE") nAge = i;
                if (Global.GetAttributeType(str) == "STRING")
                {
                    input[i] = GetCheckString(str);
                }
                else
                {
                    input[i] = GetCheckValue(str);
                }
                i++;
            }
            double dValue;
            double dMinimum;
            double dMaximum;

            for (int n = 0; n < 100; n++)
            {
                double dAge = (double)n;
                if (nAge >= 0)
                {
                    input[nAge] = dAge;
                }

                object result = 0; ;
                try
                {
                    result = calculateCheck.RunMethod(input);
                }
                catch(ArgumentException exception)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    textBoxCompile.Text = textBoxCompile.Text + "Error: " +  exception.Message + "\r\n";
                    return false;
                }
                
                try
                {
                    dValue = (double)result;
                    if (Global.GetAttributeMaximum(m_strAttribute, out dMaximum))
                    {
                        if (dValue > dMaximum) dValue = dMaximum;
                    }

                    if (Global.GetAttributeMinimum(m_strAttribute, out dMinimum))
                    {
                        if (dValue < dMinimum) dValue = dMinimum;
                    }
                }
                catch
                {
                    String strValue = Global.GetAttributeDefault(m_strAttribute);
                    dValue = double.Parse(strValue);
                }
                m_Answer[n] = dValue;
                if (dValue == 0)
                {
                    dgvPerformance.Rows.Add(dAge.ToString(),"0");
                }
                else
                {
                    dgvPerformance.Rows.Add(dAge.ToString(), dValue.ToString(Global.GetAttributeFormat(m_strAttribute)));
                }
            }
            return true;
        }

        /// <summary>
        /// Intiate edit of a the rich text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxAttribute_DoubleClick(object sender, EventArgs e)
        {

            int nCursor = richTextBoxEquation.SelectionStart;

            String strAttribute = listBoxAttribute.SelectedItem.ToString();
            
            strAttribute = "[" + strAttribute + "]";

            richTextBoxEquation.Text = richTextBoxEquation.Text.Insert(nCursor, strAttribute);
            richTextBoxEquation.SelectionStart = nCursor + strAttribute.Length;
            richTextBoxEquation.Focus();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void buttonOK_Click(object sender, EventArgs e)
        {
            bool isPiecewise = checkBoxPiecewise.Checked;
            if (isPiecewise)
            {
                m_isPiecewise = true;
                m_strEquation = richTextBoxEquation.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                if (CheckEquation())
                {
                    m_isPiecewise = false;
                    m_isFunction = checkBoxAsFunction.Checked;
                    this.DialogResult = DialogResult.OK;
                    m_strEquation = richTextBoxEquation.Text;
                    this.Close();
                }
                else
                {
                    if (IsConsequence)
                    {
                        if (MessageBox.Show("This equation is for a consequence.  A blank consequence is allowable though not recommended.  Continiue?", "Consequence Equation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            m_strEquation = richTextBoxEquation.Text;
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                }
            }

            //Passes the resutl of the edit box back out to the grid.
            if(_performance != null)
            {
                _performance.Equation = m_strEquation;
                _performance.IsFunction = checkBoxAsFunction.Checked;
                _performance.IsPiecewise = m_isPiecewise;
            }
        }

        private void listBoxFunctions_SelectedIndexChanged(object sender, EventArgs e)
        {
			if (listBoxFunctions.SelectedItem != null)
			{
				String strFunction = listBoxFunctions.SelectedItem.ToString();
				textBoxDescription.Text = m_hashFunction[strFunction].ToString();
			}
        }

        private void listBoxFunctions_DoubleClick(object sender, EventArgs e)
        {
			if (listBoxFunctions.SelectedItem != null)
			{

				String strFunction = listBoxFunctions.SelectedItem.ToString();

                int nCursor = richTextBoxEquation.SelectionStart;

				strFunction = strFunction.Replace("(x)", "()");
				strFunction = strFunction.Replace("x,y", ",");

				strFunction = "~" + strFunction;

                richTextBoxEquation.Text = richTextBoxEquation.Text.Insert(nCursor, strFunction);

				nCursor += (strFunction.IndexOf('(') + 1);
                richTextBoxEquation.SelectionStart = nCursor;
                richTextBoxEquation.Focus();
			}
        }

        private void buttonPlus_Click(object sender, EventArgs e)
        {
            int nCursor = richTextBoxEquation.SelectionStart;
            richTextBoxEquation.Text = richTextBoxEquation.Text.Insert(nCursor, "+");
            nCursor++;
            richTextBoxEquation.SelectionStart = nCursor;
            richTextBoxEquation.Focus();
        }

        private void buttonMinus_Click(object sender, EventArgs e)
        {
            int nCursor = richTextBoxEquation.SelectionStart;
            richTextBoxEquation.Text = richTextBoxEquation.Text.Insert(nCursor, "-");
            nCursor++;
            richTextBoxEquation.SelectionStart = nCursor;
            richTextBoxEquation.Focus();
        }

        private void buttonMultiply_Click(object sender, EventArgs e)
        {
            int nCursor = richTextBoxEquation.SelectionStart;
            richTextBoxEquation.Text = richTextBoxEquation.Text.Insert(nCursor, "*");
            nCursor++;
            richTextBoxEquation.SelectionStart = nCursor;
            richTextBoxEquation.Focus();
        }

        private void buttonDivide_Click(object sender, EventArgs e)
        {
            int nCursor = richTextBoxEquation.SelectionStart;
            richTextBoxEquation.Text = richTextBoxEquation.Text.Insert(nCursor, "/");
            nCursor++;
            richTextBoxEquation.SelectionStart = nCursor;
            richTextBoxEquation.Focus();
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            int nCursor = richTextBoxEquation.SelectionStart;
            richTextBoxEquation.Text = richTextBoxEquation.Text.Insert(nCursor, "(");
            nCursor++;
            richTextBoxEquation.SelectionStart = nCursor;
            richTextBoxEquation.Focus();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            int nCursor = richTextBoxEquation.SelectionStart;
            richTextBoxEquation.Text = richTextBoxEquation.Text.Insert(nCursor, ")");
            nCursor++;
            richTextBoxEquation.SelectionStart = nCursor;
            richTextBoxEquation.Focus();
        }

        private void listBoxAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nLength = 0;
            if (richTextBoxEquation.SelectionLength > 0)
            {
                nLength = 0;
            }
            int nCursor = richTextBoxEquation.SelectionStart;
            if (nLength > 0)
            {
                richTextBoxEquation.Text = richTextBoxEquation.Text.Remove(nCursor, nLength);
            }
        }

        private void EnablePiecewise(bool isPiecewise)
        {
            if (m_showPiecewise) checkBoxPiecewise.Visible = true;
            else
            {
                checkBoxPiecewise.Visible = false;
                m_isPiecewise = false;
                isPiecewise = m_isPiecewise;
            }

            listBoxAttribute.Enabled = !isPiecewise;
            listBoxFunctions.Enabled = !isPiecewise;
            buttonPlus.Enabled = !isPiecewise;
            buttonMinus.Enabled = !isPiecewise;
            buttonMultiply.Enabled = !isPiecewise;
            buttonDivide.Enabled = !isPiecewise;
            dgvDefault.Enabled = !isPiecewise;
            richTextBoxEquation.Enabled = !isPiecewise;
        }

        private void checkBoxPiecewise_CheckedChanged(object sender, EventArgs e)
        {
            m_isPiecewise = checkBoxPiecewise.Checked;
            EnablePiecewise(m_isPiecewise);
        }

        private bool CheckPiecewise()
        {
            textBoxCompile.Text = "";
            //Check that complete pairs.
            //Check that at least one value available.
            //Check that no [Age] value is less than 0
            //Make sure two identical age values not entered.
            Dictionary<int, double> ageValues = new Dictionary<int, double>();
            foreach (DataGridViewRow row in dgvPerformance.Rows)
            {
                if (row.IsNewRow) continue;
                int age = -1;
                try
                {
                    age = Convert.ToInt32(row.Cells[0].Value);
                }
                catch
                {
                    textBoxCompile.Text += "Failure to convert [AGE]=" + row.Cells[0].Value.ToString() + " Row=" + row.Index.ToString() + "\n";
                    return false;
                }

                if (age < 0)
                {
                    textBoxCompile.Text = "Values for [AGE] must be 0 or greater.  Negative values for age not allowed.";
                    return false;
                }
                
                double value = double.NaN;
                try
                {
                    value = Convert.ToDouble(row.Cells[1].Value);
                }
                catch
                {
                    textBoxCompile.Text += "Failure to convert [VALUE]=" + row.Cells[1].Value.ToString() + " Row=" + row.Index.ToString() + "\n";
                    return false;
                }

                if (!ageValues.ContainsKey(age))
                {
                    ageValues.Add(age, value);
                }
                else
                {
                    textBoxCompile.Text = "Only unique integer values for [AGE] (first column) are allowed.";
                    return false;
                }
            }

            if (ageValues.Count < 1)
            {
                textBoxCompile.Text = "At least one age/value pair must be entered to use a piecewise analysis.";
                return false;
            }

            string piecewise = "";

            foreach (int key in ageValues.Keys)
            {
                piecewise += "(" + key.ToString() + "," + ageValues[key].ToString() + ")";
            }

            SimulationDataAccess.DTO.Piecewise temp = new SimulationDataAccess.DTO.Piecewise(piecewise);
            richTextBoxEquation.Text = temp.ToString();

            if (!string.IsNullOrWhiteSpace(temp.Errors))
            {
                textBoxCompile.Text = "PIECEWISE ERRORS:" + temp.Errors;
            }
            else
            {
                textBoxCompile.Text = "Piecewise equation input compile properly.";
            }


            return true;
        }

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string allText = Clipboard.GetText();
			dgvPerformance.Rows.Clear();

			string[] rows = allText.Split('\n');
			foreach (string row in rows)
			{
				string[] cells = row.Split('\t');
				if (cells[0] != "" && cells[1] != "")
				{
					dgvPerformance.Rows.Add(cells[0], cells[1]);
				}
			}
		}


        private void richTextBoxEquation_Enter(object sender, EventArgs e)
        {
           foreach(Control control in this.Controls)
           {
               control.TabStop = false;
           }
        }

        private void richTextBoxEquation_Leave(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                control.TabStop = true;
            }
        }

        private void checkBoxAsFunction_CheckedChanged(object sender, EventArgs e)
        {
            ReloadAttributes();
        }

        private void ReloadAttributes()
        {
            bool isFunction = checkBoxAsFunction.Checked;
            listBoxAttribute.Items.Clear();
            string select = null;
            listBoxAttribute.Items.Add("LENGTH");
            listBoxAttribute.Items.Add("AREA");
            if(isFunction)
            {
                select = "SELECT ATTRIBUTE_ FROM ATTRIBUTES_";
            }
            else
            {
                if (m_bCalculatedField)
                {
                    select = "SELECT ATTRIBUTE_ FROM ATTRIBUTES_ WHERE (CALCULATED='0' OR CALCULATED IS NULL) AND TYPE_='NUMBER'";
                }
                else
                {
                    select = "SELECT ATTRIBUTE_ FROM ATTRIBUTES_ WHERE TYPE_='NUMBER'";
                }
            }
        
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(select);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    String attribute = dr["ATTRIBUTE_"].ToString();
                    listBoxAttribute.Items.Add(attribute);
                }
            }
            catch(Exception error)
            {
                Global.WriteOutput("Error: Error in filling attribute list for Edit Equation." + error.Message);
            }
        }
    }
}
