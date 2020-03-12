using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Collections;

namespace Simulation
{
    public class Costs
    {
        bool _isDefault;
        Criterias _criteria = new Criterias();

        String _costEquation;
        public CalculateEvaluate.CalculateEvaluate _calculate;
        public List<String> _attributesEquation = new List<String>();
        CompilerResults _compilerResultsEquation;
        float _cost;
        bool _isCompoundTreatment;
        string _compoundTreatment;
        string _costID;

        public Costs(string ID)
        {
            _costID = ID;
            _criteria = new Criterias("COSTS", "BINARY_CRITERIA", ID);
        }

        /// <summary>
        /// Unique primary key for cost table.
        /// </summary>
        public string CostID
        {
            get { return _costID; }
            set { _costID = value; }
        }

        /// <summary>
        /// Get/Sets whether this is a Default cost for a treatmen.
        /// </summary>
        public bool Default
        {
            get { return _isDefault; }
            set { _isDefault = value; }
        }

        /// <summary>
        /// Sets criteria for costs
        /// </summary>
        public Criterias Criteria
        {
            get { return _criteria; }
            set { _criteria = value; }
        }

        /// <summary>
        /// Sets costs for a specific cost/criteria pair.
        /// </summary>
        public float Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        /// <summary>
        /// Get/Set Cost equation as String
        /// </summary>
        public String Equation
        {
            get { return _costEquation; }
            set
            {
                _isCompoundTreatment = false;
                _costEquation = value;
                // Get list of attributes
                _attributesEquation = SimulationMessaging.ParseAttribute(_costEquation);
                if (_calculate == null)
                {
                    _calculate = new CalculateEvaluate.CalculateEvaluate();
                    _calculate.BuildClass(_costEquation, true, "COSTS_BINARY_EQUATION_" + CostID);

                    if (_calculate.m_cr == null)
                    {
                        _compilerResultsEquation = _calculate.CompileAssembly();
                        SimulationMessaging.SaveSerializedCalculateEvaluate("COSTS", "BINARY_EQUATION", CostID, _calculate);
                    }
                }
            }
        }

        public bool IsCompoundTreatment
        {
            get { return _isCompoundTreatment; }
        }

        public CalculateEvaluate.CalculateEvaluate Calculate
        {
            get { return _calculate; }
            set { _calculate = value; }
        }


        public double GetCost(Hashtable hashAttributeValue)
        {

            int i = 0;
            object[] input = new object[_attributesEquation.Count];
            foreach (String str in _attributesEquation)
            {
                if (hashAttributeValue[str] != null) input[i] = hashAttributeValue[str];
                else input[i] = 0;
                i++;
            }
			try
			{
				object result = _calculate.RunMethod(input);
				return (double)result;
			}
			catch(Exception exc)
			{
				SimulationMessaging.AddMessage(new SimulationMessage("Error in RunMethod.  " + _calculate.OriginalInput + " " + exc.Message));
				return 0;
			}
        }


        internal void SetFunction(string equation)
        {
            _costEquation = equation;

            // Get list of attributes
            _attributesEquation = SimulationMessaging.ParseAttribute(_costEquation);
            if (_attributesEquation == null)
            {
                return;
            }

            if (_calculate == null)
            {
                _calculate = new CalculateEvaluate.CalculateEvaluate();

                //Allow functions to utilize strings and dates.
                string functionEquation = _costEquation;
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

                _calculate.BuildFunctionClass(functionEquation, "double", "COSTS_BINARY_EQUATION_" + CostID);
                if (_calculate.m_cr == null)
                {
                    _compilerResultsEquation = _calculate.CompileAssembly();
                    SimulationMessaging.SaveSerializedCalculateEvaluate("COSTS", "BINARY_EQUATION", CostID, _calculate);
                }
            }
        }
    }
}
