using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Collections;
using SimulationDataAccess.DTO;

namespace Simulation
{
    class Deteriorate
    {
        private string _performanceID;
        private string _table;
        private string _column;

        private String _attribute;
        private String _group = "";
        private String _equation;
        private String _criteria;
        private bool _isAscending;// Variables that decrease as they deteriorate (PCI, CRS).  Ascending FALSE is (IRI)
        private double _benefitLimit = 0;
        private double _remainingLifeLimit = 0;
        private bool _isModule = false;
        private bool _isCRS = false;
        private bool _isShift = false;
        private bool _isDefault = false;
        private Piecewise _piecewsie = null;
        CalculateEvaluate.CalculateEvaluate _calculate = null;
        CalculateEvaluate.CalculateEvaluate _evaluate = null;        
        CompilerResults _compilerResultsCriteria;
        CompilerResults _compilerResultsEquation;
        List<String> _attributesCriteria;
        List<String> _attributesEquation;
        List<String> _errors = new List<String>();
        bool _isAgeOnly = false;
        double[] _answer;

        public bool IsOCI { get; set; }

        public CalculateEvaluate.CalculateEvaluate Evaluate
        {
            set { _evaluate = value; }
            get { return _evaluate; }

        }

        public CalculateEvaluate.CalculateEvaluate Calculate
        {
            set { _calculate = value; }
            get { return _calculate; }

        }
        /// <summary>
        /// List of compile errors from the deterioration curve.
        /// </summary>
        public List<String> Errors
        {
            get { return _errors; }
        }

        /// <summary>
        /// Get/Set Simulation Attribute
        /// </summary>
        public String Attribute
        {
            get { return _attribute; }
            set
            {
                _attribute = value;
                _isAscending = SimulationMessaging.GetAttributeAscending(_attribute);
                
            }
        }

        public bool IsModule
        {
            get { return _isModule; }
            set { _isModule = value; }

        }

        public bool IsCRS
        {
            get { return _isCRS; }
            set { _isCRS = value; }
        }


        /// <summary>
        /// Benefit cost limit for this Attribute
        /// </summary>
        public double BenefitLimit
        {
            get { return _benefitLimit; }
            set { _benefitLimit = value; }
        }

        public double RemainingLifeLimit
        {
            get { return _remainingLifeLimit; }
            set { _remainingLifeLimit = value; }
        }

        /// <summary>
        /// Get/Set Display name for Deterioration/Performance curve
        /// </summary>
        public String Group
        {
            get { return _group; }
            set { _group = value; }
        }
        /// <summary>
        /// Get/Set Deterioration/Performance curve as String
        /// </summary>
        public String Equation
        {
            get { return _equation; }
            set
            { 
                _equation = value;
                //m_strEquation = m_strEquation.ToUpper();

                // Get list of attributes
                _attributesEquation = SimulationMessaging.ParseAttribute(_equation);
                if (_attributesEquation == null)
                {
                    return;
                }

                if (_attributesEquation.Count == 1 && _attributesEquation[0].ToUpper() == "AGE")
                {
                    _isAgeOnly=true;
                }

				//this will crash the program for equations with less than 6 characters
                //if (m_strEquation.Substring(0, 6) == "MODULE")
				if( _equation.Length >= 6 && _equation.Substring( 0, 6 ) == "MODULE" )
					{
                    IsModule = true;
					//this will crash the program for equations with less than 10 characters
					//if (m_strEquation.Substring(0, 6) == "MODULE")
					if( _equation.Length >= 10 && _equation.Substring( 7, 3 ) == "CRS" )
                    {
                        IsCRS = true;
                    }
                    return;
                }
                else
                {
                    if (_calculate == null)
                    {
                        _calculate = new CalculateEvaluate.CalculateEvaluate();
                        _calculate.BuildClass(_equation, true, cgOMS.Prefix + _table + "_BINARY_EQUATION_" + _performanceID);


                        if (_calculate.m_cr == null)
                        {
                            _compilerResultsEquation = _calculate.CompileAssembly();
                            if (_table != null)
                            {
                                SimulationMessaging.SaveSerializedCalculateEvaluate(cgOMS.Prefix + _table, "BINARY_EQUATION", _performanceID, _calculate);
                            }
                        }

                        
                        if (_calculate.m_listError.Count > 0)
                        {
                            foreach (String str in _calculate.m_listError)
                            {
                                _errors.Add(str);
                            }
                        }
                    }
                    //If [AGE] is the only variable.  This only needs to be solved once so might 
                    //as well solve it right now.
                    if (_isAgeOnly)
                    {
                        Hashtable hash = new Hashtable();
                        hash.Add("AGE", "0");
                        this.Solve(hash);
                    }
                }
            }
        }

        public Piecewise PiecewiseEquation
        {
            get { return _piecewsie; }
            set { _piecewsie = value; }
        }

        public bool Shift
        {
            get { return _isShift; }
            set { _isShift = value; }
        }

        public bool Default
        {
            get { return _isDefault; }
            set { _isDefault = value; }
        }        
        /// <summary>
        /// Get/Set Criteria for Deterioration/Performance curve
        /// </summary>
        public String Criteria
        {
            get { return _criteria; }
            set 
            {
                _criteria = value;
    //            m_strCriteria = m_strCriteria.ToUpper();

                //If m_strCriteria is Empty, this means TRUE for all cases and does not need to be evaluated.
                if (_criteria.TrimEnd().Length == 0)
                {
                    this.Default = true;
                    _attributesCriteria = new List<String>();
                    return;
                }

                _attributesCriteria = SimulationMessaging.ParseAttribute(_criteria);
                if (_attributesCriteria == null)
                {
                    return;
                }

                if (_evaluate == null)
                {
                    String strCriteria = _criteria;
                    foreach (String str in _attributesCriteria)
                    {
                        String strType = SimulationMessaging.GetAttributeType(str);
                        if (strType == "STRING")
                        {
                            String strOldValue = "[" + str + "]";
                            String strNewValue = "[@" + str + "]";
                            strCriteria = strCriteria.Replace(strOldValue, strNewValue);
                        }
                        else if (strType == "DATETIME")
                        {
                            String strOldValue = "[" + str + "]";
                            String strNewValue = "[$" + str + "]";
                            strCriteria = strCriteria.Replace(strOldValue, strNewValue);
                        }
                    }
                    _evaluate = new CalculateEvaluate.CalculateEvaluate();
                    _evaluate.BuildClass(strCriteria, false,  _table + "_" + _column + "_" + _performanceID);
                    
                    if (_evaluate.m_cr == null)
                    {
                        _compilerResultsCriteria = _evaluate.CompileAssembly();
                        if (_table != null)
                        {
                            SimulationMessaging.SaveSerializedCalculateEvaluate( _table, _column, _performanceID, _evaluate);
                        }
                    }

                    if (_evaluate.m_listError.Count > 0)
                    {
                        foreach (String str in _evaluate.m_listError)
                        {
                            _errors.Add(str);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Get the compiled Equation
        /// </summary>
        public CompilerResults CompiledEquation
        {
            get { return _compilerResultsEquation; }
        }

        /// <summary>
        /// Get the compiled Criteria
        /// </summary>
        
        public CompilerResults CompiledCriteria
        {
            get { return _compilerResultsCriteria; }
        }

        public List<String> CriteriaAttributes
        {
            get { return _attributesCriteria; }
        }

        public List<String> EquationAttributes
        {
            get { return _attributesEquation; }
            set { _attributesEquation = value; }
        }

        public string PerformanceID
        {
            get { return _performanceID; }
            set { _performanceID = value; }
        }

        public Deteriorate()
        {
            _performanceID = null;
            _table = null;
            _column = null;
            this.IsOCI = false;
        }
        
        public Deteriorate(string table, string column, string id)
        {
            _performanceID = id;
            _table = table;
            _column = column;
            this.IsOCI = false;
        }

        public void Solve(Hashtable hashAttributeValue)
        {
            double dValue;
            double dMinimum;
            double dMaximum;

            _answer = new double[100];
            if (IsModule)
            {
                if (IsCRS)
                {
                    String strType = hashAttributeValue[_attributesEquation[0].ToString()].ToString();
                    double dCRS = Convert.ToDouble(hashAttributeValue[_attributesEquation[1].ToString()]);
                    double dThick = Convert.ToDouble(hashAttributeValue[_attributesEquation[2].ToString()]);
                    double dESAL = Convert.ToDouble(hashAttributeValue[_attributesEquation[3].ToString()]);
                    double dESALGrowth = Convert.ToDouble(hashAttributeValue[_attributesEquation[4].ToString()]);

                    double dRSL;
                    double dBenefit;
                    _answer[0] = dCRS;
                    for (int n = 1; n < 100; n++)
                    {
                        double dAge = (double)n;
                        dValue = SimulationMessaging.crs.CalculateCRSBenefit(false, strType, dCRS, dThick, dESAL, dESALGrowth, 1, dAge, SimulationMessaging.Method.BenefitLimit, out dBenefit, out dRSL);
                        try
                        {
                            if (SimulationMessaging.GetAttributeMaximum(this.Attribute, out dMaximum))
                            {
                                if (dValue > dMaximum) dValue = dMaximum;
                            }

                            if (SimulationMessaging.GetAttributeMinimum(this.Attribute, out dMinimum))
                            {
                                if (dValue < dMinimum) dValue = dMinimum;
                            }
                        }
                        catch
                        {
                            String strValue = SimulationMessaging.GetAttributeDefault(this.Attribute);
                            dValue = double.Parse(strValue);
                        }
                        _answer[n] = dValue;

                    }
                    return;
                }
                else if(IsOCI)
                {
                    



                }
            }

            object[] input = new object[_attributesEquation.Count];

            int i = 0;
            int nAge = -1;
            foreach(String str in _attributesEquation)
            {
                if (str == "AGE") nAge = i;
                input[i] = hashAttributeValue[str];;
                i++;
            }

            for (int n = 0; n < 100; n++)
            {
                double dAge = (double)n;
                if (nAge >= 0)
                {
                    input[nAge] = dAge;
                }
				try
				{
                    object result = _calculate.RunMethod(input);
					try
					{
						dValue = (double)result;
						if (SimulationMessaging.GetAttributeMaximum(this.Attribute, out dMaximum))
						{
							if (dValue > dMaximum) dValue = dMaximum;
						}

						if (SimulationMessaging.GetAttributeMinimum(this.Attribute, out dMinimum))
						{
							if (dValue < dMinimum) dValue = dMinimum;
						}
					}
					catch
					{
						String strValue = SimulationMessaging.GetAttributeDefault(this.Attribute);
						dValue = double.Parse(strValue);
					}
					_answer[n] = dValue;
				}
				catch(Exception exc)
				{
					SimulationMessaging.AddMessage(new SimulationMessage("Error in RunMethod.   " + _calculate.OriginalInput + " " + exc.Message)); 
				}

            }
            return;
        }

        public double IterateOneYear(Hashtable hashAttributeValues, out bool bOutOfRange)
        {
            double apparentAge;
            if (this.PiecewiseEquation == null)
            {
                return IterateOneYearEquation(hashAttributeValues,0,out apparentAge, out bOutOfRange);
            }
            else
            {
                return IterateSpanPiecewise(hashAttributeValues,1, out bOutOfRange);
            }
        }


        public double IterateOneYear(Hashtable hashAttributeValues, int apparentAgeHint, out double apparentAge)
        {
            if (PiecewiseEquation != null)
            {
                return IterateSpanPiecewise(hashAttributeValues, 1, apparentAgeHint, out apparentAge);
            }


            
            return IterateOneYearEquation(hashAttributeValues, apparentAgeHint,out apparentAge, out bool bOutOfRange);

        }


        /// <summary>
        /// New method with Apparent Age Hint
        /// </summary>
        /// <param name="hashAttributeValues"></param>
        /// <param name="span"></param>
        /// <param name="apparentAgeHint"></param>
        /// <param name="apparentAge"></param>
        /// <returns></returns>
        public double IterateSpanPiecewise(Hashtable hashAttributeValues, double span,int apparentAgeHint, out double  apparentAge)
        {
            double dValue = double.Parse(hashAttributeValues[this.Attribute].ToString());
            double dAnswer = double.NaN;
            if (Shift)
            {
                var dAge = double.Parse(hashAttributeValues["AGE"].ToString());
                dAnswer = this.PiecewiseEquation.GetNextValue(dValue, dAge, span,out apparentAge).Value;
                
            }
            else
            {
                dAnswer = this.PiecewiseEquation.GetNextValue(dValue, span,apparentAgeHint, out apparentAge).Value;
            }

            return dAnswer;
        }




        public double IterateSpanPiecewise(Hashtable hashAttributeValues ,double span, out bool bOutOfRange)
        {
            double apparentAge = 0;
            bOutOfRange = false;
            double dValue = double.Parse(hashAttributeValues[this.Attribute].ToString());
            double dAnswer = double.NaN;
            double dAge = 0;
            if (Shift)
            {
                dAge = double.Parse(hashAttributeValues["AGE"].ToString());
                dAnswer = this.PiecewiseEquation.GetNextValue(dValue, dAge,span, out apparentAge).Value;
            }
            else
            {
                dAnswer = this.PiecewiseEquation.GetNextValue(dValue,span).Value;
            }

            double dMinimum;
            double dMaximum;
            if (SimulationMessaging.GetAttributeMinimum(this.Attribute, out dMinimum))
            {
                if (dMinimum > dAnswer) dAnswer = dMinimum;
            }
            if (SimulationMessaging.GetAttributeMaximum(this.Attribute, out dMaximum))
            {
                if (dMaximum < dAnswer) dAnswer = dMaximum;
            }
            return dAnswer;
        }

        /// <summary>
        /// 
        /// Calculates the next years values AND files in the Answer array for base RemainingLife and Benefit cost.
        /// </summary>
        /// <param name="hashAttributeValues">Values for other attributes</param>
        /// <returns>Value for next year</returns>
        public double IterateOneYearEquation(Hashtable hashAttributeValues, int apparentAgeHint, out double apparentAge, out bool bOutOfRange)
        {
            double increment;
            double delta;
            double ratio;
            double dAnswer;
            double dAge = 0;
            apparentAge = 0;
            if (Shift)
            {
                dAge = double.Parse(hashAttributeValues["AGE"].ToString());
            }

            bOutOfRange = true;
            double dValue = double.Parse(hashAttributeValues[this.Attribute].ToString());
            //TODO: Does this need to be solved everytime on
            if (!_isAgeOnly) Solve(hashAttributeValues);

            //Check if 99 year value is input
            if (dValue > _answer[98] && !SimulationMessaging.GetAttributeAscending(Attribute))
            {
                bOutOfRange = false;
                return dValue;
            }
            else if(dValue < _answer[98] && SimulationMessaging.GetAttributeAscending(Attribute))
            {
                bOutOfRange = false;
                return dValue;
            }
            double dMinimum;
            double dMaximum;

            for (int i = 0; i < 98; i++)
            {
                //This eliminates ascending / descending problem.  Just looking for a number in between.
                if ((_answer[i] >= dValue && dValue > _answer[i + 1]) || (_answer[i] <= dValue && dValue < _answer[i + 1]))
                {
                    if (!Shift || i == 0 || dAge == 0)
                    {
                        delta = dValue - _answer[i + 1];
                        increment = _answer[i] - _answer[i + 1];
                        if (increment == 0) return _answer[i + 1];
                        ratio = 1 - (delta / increment);
                        increment = _answer[i + 1] - _answer[i + 2];
                        delta = ratio * increment;
                        dAnswer = _answer[i + 1] - delta;
                    }
                    else // Must be SHIFT and i not 0
                    {
                        delta = dValue - _answer[i + 1];
                        increment = _answer[i] - _answer[i + 1];
                        double dApparentAge = (double)i  + (1 - (delta / increment));// Determine apparent age
                        double dApparentRatio =  dApparentAge/dAge;// Determine rate that age is changing.
                        double dNextAge = dApparentRatio + dApparentAge;
                        int iNextAge = (int)Math.Floor(dNextAge);
                        double dNextRatio = dNextAge - Math.Floor(dNextAge);
                        if (iNextAge < 99)
                        {
                            increment = _answer[iNextAge] - _answer[iNextAge+1];
                            delta = dNextRatio * increment;
                            dAnswer = _answer[iNextAge] - delta;
                        }
                        else
                        {
                            dAnswer = _answer[99];
                        }
                    }
                    if (SimulationMessaging.GetAttributeMinimum(this.Attribute, out dMinimum))
                    {
                        if (dMinimum > dAnswer) return dMinimum;
                    }
                    if (SimulationMessaging.GetAttributeMaximum(this.Attribute, out dMaximum))
                    {
                        if (dMaximum < dAnswer) return dMaximum;
                    }
                    bOutOfRange = false;
                    return dAnswer;
                }
            }

            dValue = _answer[0];
            if(SimulationMessaging.GetAttributeMinimum(this.Attribute,out dMinimum))
            {
                if(dMinimum > dValue) return dMinimum;
            }
            
            if(SimulationMessaging.GetAttributeMaximum(this.Attribute,out dMaximum))
            {
                if(dMaximum < dValue) return dMaximum;
            }
            return dValue;
        }

        public double CalculateBenefit(Hashtable hashAttributeValues)
        {
            if (this.PiecewiseEquation == null)
            {
                return CalculateBenefitEquation(hashAttributeValues);
            }
            else
            {
                return CalculateBenefitPiecewise(hashAttributeValues);
            }
        }

        public double CalculateBenefitPiecewise(Hashtable hashAttributeValues)
        {
            double benefit = 0;
            double dValue = double.Parse(hashAttributeValues[this.Attribute].ToString());
            if (this.Shift)
            {
                double dAge = double.Parse(hashAttributeValues["AGE"].ToString());
                benefit = this.PiecewiseEquation.GetBenefit(dValue, SimulationMessaging.Method.BenefitLimit, dAge);
            }
            else
            {
                benefit = this.PiecewiseEquation.GetBenefit(dValue, SimulationMessaging.Method.BenefitLimit);
            }
            return benefit;
        }
        
        public double CalculateBenefitEquation(Hashtable hashAttributeValues)
        {
            double delta;
            double increment;
            double ratio;
            double dSumBenefit = 0;
            double dBenefit;
            double dValue = double.Parse(hashAttributeValues[this.Attribute].ToString());
            double dAge = 0;
            if (!_isAgeOnly && !IsCRS) Solve(hashAttributeValues);


            if (Shift)
            {
                dAge = double.Parse(hashAttributeValues["AGE"].ToString());
            }


            if (_answer[0] == _answer[1])//No change by age.
            {
                return _answer[0];
            }
            else
            {
                for (int i = 0; i < 98; i++)
                {
                    //This eliminates ascending / descending problem.  Just looking for a number in between.
                    if ((_answer[i] >= dValue && dValue > _answer[i + 1]) || (_answer[i] <= dValue && dValue < _answer[i + 1]))
                    {
                        delta = dValue - _answer[i + 1];
                        increment = _answer[i] - _answer[i + 1];
                        ratio = 1 - (delta / increment);
                        for (int j = i; j < 99; j++)
                        {
                            increment = _answer[j] - _answer[j + 1];
                            delta = ratio * increment;

                            if (this._isAscending)
                            {
                                dBenefit = _answer[j] - delta - SimulationMessaging.Method.BenefitLimit;
                                if (dBenefit > 0) dSumBenefit += dBenefit;
                            }
                            else
                            {
                                dBenefit = SimulationMessaging.Method.BenefitLimit - _answer[j] + delta;
                                if (dBenefit > 0) dSumBenefit += dBenefit;
                            }
                        }

                        if (Shift && i > 0 && dAge > 0)
                        {
                            double dApparentAge = (double)i + ratio;
                            double dAgeRatio = dApparentAge / dAge;
                            dSumBenefit = dSumBenefit / dAgeRatio;
                        }
                        return dSumBenefit;
                    }
                }

            }


            return dSumBenefit;
        }

        public bool CalculateRemainingLife(Hashtable hashAttributeValues, Hashtable hashPrevious, out double dRL)
        {
            if (this.PiecewiseEquation == null)
            {
                return CalculateRemainingLifeEquation(hashAttributeValues, hashPrevious, out dRL);
            }
            else
            {
                return CalculateRemainingLifePiecewise(hashAttributeValues, hashPrevious, out dRL);
            }
        }



        public bool CalculateRemainingLifePiecewise(Hashtable hashAttributeValues, Hashtable hashPrevious, out double dRL)
        {
            double dLimit;
            double dRemainingLife = 0;
            dRL = dRemainingLife;

            if (!SimulationMessaging.GetDeficientLevel(this.Attribute, hashPrevious, out dLimit)) return false;
            this.RemainingLifeLimit = dLimit;
            
            double dValue = double.Parse(hashAttributeValues[this.Attribute].ToString());
            if (Shift)
            {
                double dAge = double.Parse(hashAttributeValues["AGE"].ToString());
                dRL = this.PiecewiseEquation.GetRemainingLife(dValue, dLimit, dAge);
            }
            else
            {
                dRL = this.PiecewiseEquation.GetRemainingLife(dValue, dLimit);
            }
            return true;
        }

        public bool CalculateRemainingLifeEquation(Hashtable hashAttributeValues, Hashtable hashPrevious, out double dRL)
        {
            double increment;
            double delta;
            double ratioAnswer=0;
            int iAnswer = -1;
            double ratioDeficient=0;
            int iDeficient =-1;
            double dRemainingLife = 0;
            dRL = dRemainingLife;
            
            double dValue = double.Parse(hashAttributeValues[this.Attribute].ToString());

            if (!_isAgeOnly) Solve(hashPrevious);
            double dAge = 0;
            if (Shift)
            {
                dAge = double.Parse(hashAttributeValues["AGE"].ToString());
            }
            
            double dLimit;
            if (!SimulationMessaging.GetDeficientLevel(this.Attribute, hashPrevious, out dLimit)) return false;
            this.RemainingLifeLimit = dLimit;
            for (int i = 0; i < 98; i++)
            {
                //This eliminates ascending / descending problem.  Just looking for a number in between.
                if ((_answer[i] >= dValue && dValue > _answer[i + 1]) || (_answer[i] <= dValue && dValue < _answer[i + 1]))
                {

                    delta = dValue - _answer[i + 1];
                    increment = _answer[i] - _answer[i + 1];
                    if (increment == 0)
                    {
                        ratioAnswer = 0;
                    }
                    else
                    {
                        ratioAnswer = 1 - (delta / increment);
                    }
                    iAnswer = i;
                }


                if ((_answer[i] >= this.RemainingLifeLimit && this.RemainingLifeLimit > _answer[i + 1]) || (_answer[i] <= this.RemainingLifeLimit && this.RemainingLifeLimit < _answer[i + 1]))
                {
                    delta = this.RemainingLifeLimit - _answer[i + 1];
                    increment = _answer[i] - _answer[i + 1];
                    if (increment == 0)
                    {
                        ratioDeficient = 0;
                    }
                    else
                    {
                        ratioDeficient = 1-(delta / increment);
                    }
                    iDeficient = i;
                }

                if (iAnswer >= 0 && iDeficient >= 0)
                {
                    //Then we have solve remaining life
                    dRemainingLife = ((double)iDeficient + ratioDeficient) - ((double)iAnswer + ratioAnswer);
                    dRL = dRemainingLife;

                    if (Shift && i > 0 && dAge > 0)
                    {
                        double dApparentAge = (double)iAnswer+ratioAnswer;
                        double dAgeRatio = dApparentAge/(dAge);
                        dRL = dRL / dAgeRatio;
                    }
                    
                    if (dRL < 0) dRL = 0;
                    return true;
                }
            }

            if (iAnswer >= 0 && iDeficient == -1)
            {
                dRemainingLife = 99;
                dRL = dRemainingLife;
                return true;
            }

            if (iDeficient >= 0 && iAnswer == -1)
            {
                dRemainingLife = 0;
                dRL = dRemainingLife;
                return true;
            }
            dRL = 0;
            return false;
        }


        /// <summary>
        /// Calculates life extension between current value and normalized value.
        /// </summary>
        /// <param name="hashAttributeValues"></param>
        /// <param name="normalized"></param>
        /// <returns></returns>
        public double CalculateExtension(Hashtable hashAttributeValues, double normalized)
        {
            if (this.PiecewiseEquation == null)
            {
                return CalculateExtensionEquation(hashAttributeValues, normalized);
            }
            else
            {
                return CalculateExtensionPiecewise(hashAttributeValues, normalized);
            }
        }

        private double CalculateExtensionPiecewise(Hashtable hashAttributeValues, double normalized)
        {
            double dValue = double.Parse(hashAttributeValues[this.Attribute].ToString());
            if (Shift)
            {
                double dAge = double.Parse(hashAttributeValues["AGE"].ToString());
                return this.PiecewiseEquation.GetRemainingLife(dValue, normalized, dAge);
            }
            else
            {
                return this.PiecewiseEquation.GetRemainingLife(dValue, normalized);
            }
        }

        private double CalculateExtensionEquation(Hashtable hashAttributeValues, double normalized)
        {
            double increment;
            double delta;
            double ratioAnswer = 0;
            int iAnswer = -1;
            double ratioDeficient = 0;
            int iDeficient = -1;
            double extension = 0;

            double dValue = double.Parse(hashAttributeValues[this.Attribute].ToString());

            if (!_isAgeOnly) Solve(hashAttributeValues);
            double dAge = 0;
            if (Shift)
            {
                dAge = double.Parse(hashAttributeValues["AGE"].ToString());
            }

            for (int i = 0; i < 98; i++)
            {
                //This eliminates ascending / descending problem.  Just looking for a number in between.
                if ((_answer[i] >= dValue && dValue > _answer[i + 1]) || (_answer[i] <= dValue && dValue < _answer[i + 1]))
                {

                    delta = dValue - _answer[i + 1];
                    increment = _answer[i] - _answer[i + 1];
                    if (increment == 0)
                    {
                        ratioAnswer = 0;
                    }
                    else
                    {
                        ratioAnswer = 1 - (delta / increment);
                    }
                    iAnswer = i;
                }


                if ((_answer[i] >= normalized && normalized > _answer[i + 1]) || (_answer[i] <= normalized && normalized < _answer[i + 1]))
                {
                    delta = normalized - _answer[i + 1];
                    increment = _answer[i] - _answer[i + 1];
                    if (increment == 0)
                    {
                        ratioDeficient = 0;
                    }
                    else
                    {
                        ratioDeficient = 1 - (delta / increment);
                    }
                    iDeficient = i;
                }

                if (iAnswer >= 0 && iDeficient >= 0)
                {
                    //Then we have solve remaining life
                    extension = ((double)iDeficient + ratioDeficient) - ((double)iAnswer + ratioAnswer);

                    if (Shift && i > 0 && dAge > 0)
                    {
                        double dApparentAge = (double)iAnswer + ratioAnswer;
                        double dAgeRatio = dApparentAge / (dAge);
                        extension = extension / dAgeRatio;
                    }

                    if (extension < 0) extension = 0;
                }
            }
            return extension;
        }






        public bool IsCriteriaMet(Hashtable hashAttributeValue)
        {
            if (string.IsNullOrWhiteSpace(_criteria)) return true;
            object[] input = new object[_attributesCriteria.Count];
            int i = 0;
            foreach (String str in _attributesCriteria)
            {
                input[i] = hashAttributeValue[str];
                i++;
            }
			try
			{
                if (_evaluate.m_methodInfo == null)
                {
                    if (_evaluate.m_cr == null)
                    {
                        _evaluate.BuildClass(_evaluate.Expression, false, cgOMS.Prefix + _table + "_" + _column + "_" + _performanceID);
                        _evaluate.CompileAssembly();
                        SimulationMessaging.SaveSerializedCalculateEvaluate(cgOMS.Prefix + _table, _column, _performanceID, _evaluate);
                    }
                }
                bool isMet = (bool)_evaluate.RunMethod(input);
                return isMet;
			}
			catch(Exception exc)
			{
				SimulationMessaging.AddMessage(new SimulationMessage("Error in RunMethod.   " + _evaluate.OriginalInput + " " + exc.Message));
                return false;
			}
        }


        public double ApplyDeterioration(Hashtable hashAttributeValue)
        {
			object[] input = new object[_attributesEquation.Count];

			int i = 0;
			foreach (String str in _attributesEquation)
			{
				input[i] = hashAttributeValue[str];
				i++;
			}

			double dReturn = 0;
			try
			{
				dReturn = (double)_calculate.RunMethod(input);
			}
            catch { SimulationMessaging.AddMessage(new SimulationMessage("Error: solving calculated fields for Compound Treatment.")); }
			return dReturn;
			
        }

        internal void SetFunction(string equation)
        {
            _equation = equation;

            // Get list of attributes
            _attributesEquation = SimulationMessaging.ParseAttribute(_equation);
            if (_attributesEquation == null)
            {
                return;
            }

            if (_attributesEquation.Count == 1 && _attributesEquation[0].ToUpper() == "AGE")
            {
                _isAgeOnly=true;
            }

			if( _equation.Length >= 6 && _equation.Substring( 0, 6 ) == "MODULE" )
				{
                IsModule = true;
				//this will crash the program for equations with less than 10 characters
				//if (m_strEquation.Substring(0, 6) == "MODULE")
				if( _equation.Length >= 10 && _equation.Substring( 7, 3 ) == "CRS" )
                {
                    IsCRS = true;
                }
                else if (_equation.Length >= 10 && _equation.Substring(7, 3) == "OCI")
                {
                    IsOCI = true;
                }
                return;
            }
            else
            {
                if (_calculate == null)
                {
                    _calculate = new CalculateEvaluate.CalculateEvaluate();
                 
                    //Allow functions to utilize strings and dates.
                    string functionEquation = _equation;
                    List<string> attributes = SimulationMessaging.ParseAttribute(functionEquation);

                    foreach (String attribute in attributes)
                    {
                        String attributeType = SimulationMessaging.GetAttributeType(attribute);
                        if (attributeType == "STRING")
                        {
                            String oldValue = "[" + attribute + "]";
                            String newValue = "[@" + attribute + "]";
                            functionEquation = functionEquation.Replace(oldValue, newValue);
                        }
                        else if (attributeType == "DATETIME")
                        {
                            String oldValue = "[" + attribute + "]";
                            String newValue = "[$" + attribute + "]";
                            functionEquation = functionEquation.Replace(oldValue, newValue);
                        }
                    }
                    _calculate.BuildFunctionClass(functionEquation, "double", cgOMS.Prefix + _table + "_BINARY_EQUATION_" + _performanceID);

                    if (_calculate.m_cr == null)
                    {
                        _compilerResultsEquation = _calculate.CompileAssembly();
                        if (_table != null)
                        {
                            SimulationMessaging.SaveSerializedCalculateEvaluate(cgOMS.Prefix + _table, "BINARY_EQUATION", _performanceID, _calculate);
                        }
                    }

                    if (_calculate.m_listError.Count > 0)
                    {
                        foreach (String str in _calculate.m_listError)
                        {
                            _errors.Add(str);
                        }
                    }
                }
                //If [AGE] is the only variable.  This only needs to be solved once so might 
                //as well solve it right now.
                if (_isAgeOnly)
                {
                    Hashtable hash = new Hashtable();
                    hash.Add("AGE", "0");
                    this.Solve(hash);
                }
            }
        }

        public override string ToString()
        {
            return _attribute + "(" + _criteria + ")";
        }
    }
}
