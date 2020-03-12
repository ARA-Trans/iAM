using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalculateEvaluate;
using System.CodeDom.Compiler;

namespace Simulation
{
    public class Jurisdiction
    {
        String _criteria;
        CalculateEvaluate.CalculateEvaluate _evaluate;

        public String NetworkID { get; set; }
        public String Table { get; set; }
        public String Column { get; set; }
        
        public bool Default { get; set; }

        List<String> _attributesCriteria;
        CompilerResults _compilerResultsCriteria;
        List<String> _errors = new List<String>();
        
        public Jurisdiction(String networkID)
        {
            this.NetworkID = networkID;
            this.Table = "JURISDICITION";
            this.Column = "BINARY_CRITERIA";
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
                    _evaluate.BuildClass(strCriteria, false, this.Table + "_" + this.Column + "_" + this.NetworkID);

                    if (_evaluate.m_cr == null)
                    {
                        _compilerResultsCriteria = _evaluate.CompileAssembly();
                        if (this.Table != null)
                        {
                            SimulationMessaging.SaveSerializedCalculateEvaluate(this.Table, this.Column, this.NetworkID, _evaluate);
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

        public CalculateEvaluate.CalculateEvaluate Evaluate
        {
            get { return _evaluate; }
            set { _evaluate = value; }
        }
    }
}
