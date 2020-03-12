using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Collections;

namespace Simulation
{
    public class Consequences
    {
        private bool _isDefault;
        private Criterias _criteria;
        private List<AttributeChange> _attributeChange = new List<AttributeChange>();
        private String _commitID;
        private String _treatmentID;
        private String _treatment;
        private List<String> _attributes = new List<String>();
        private string _compoundTreatment;
        public List<String> _attributesEquation = new List<String>();
        String _consequenceEquation;
        public CalculateEvaluate.CalculateEvaluate _calculate;
        CompilerResults _compilerResultsEquation;
        private string _id;
        private string _table;
        private string _criteriaColumn;

        public bool IsEquation { get; set; }

        /// <summary>
        /// Gets/Sets whether this is a default Consequence for a treatment
        /// </summary>
        public bool Default
        {
            get { return _isDefault; }
            set { _isDefault = value; }
        }

        /// <summary>
        /// Gets/Sets whether this is the criteria for this consequence
        /// </summary>
        public Criterias Criteria
        {
            get { return _criteria; }
            set { _criteria = value; }
        }

        /// <summary>
        /// Get list of attributes  and there changes  by this consequence
        /// </summary>
        public List<AttributeChange> AttributeChange
        {
            get { return _attributeChange; }
        }
        
        /// <summary>
        /// Attributes in Consequence equation.
        /// </summary>
        public List<String> Attributes
        {
            get { return _attributes; }
        }

        /// <summary>
        /// Gets/Sets ConsequenceID which retrieves list of attribute changes
        /// </summary>
        public String CommitID
        {
            get { return _commitID; }
            set { _commitID = value; }
        }

        /// <summary>
        /// Gets/Sets TreatmentID which retrieves list of attribute changes
        /// </summary>
        public String TreatmentID
        {
            get { return _treatmentID; }
            set { _treatmentID = value; }
        }

        /// <summary>
        /// Gets/Sets Treatment which retrieves list of attribute changes
        /// </summary>
        public String Treatment
        {
            get { return _treatment; }
            set { _treatment = value; }
        }

        /// <summary>
        /// Compound treatement equations
        /// </summary>
        public String CompoundTreatment
        {
            get { return _compoundTreatment; }
        }

        /// <summary>
        /// Get/Set Cost equation as String.  Initializing non-funciton equations.
        /// </summary>
        public String Equation
        {
            get { return _consequenceEquation; }
            set
            {
                _consequenceEquation = value;
                // Get list of attributes
                if (!string.IsNullOrWhiteSpace(_consequenceEquation))
                {
                    IsEquation = true;
                    _attributesEquation = SimulationMessaging.ParseAttribute(_consequenceEquation);
                    if (_calculate == null)
                    {
                        _calculate = new CalculateEvaluate.CalculateEvaluate();
                        _calculate.BuildClass(_consequenceEquation, true, "CONSEQUENCES_BINARY_EQUATION_" + _id);

                        if (_calculate.m_cr == null)
                        {
                            _compilerResultsEquation = _calculate.CompileAssembly();
                            SimulationMessaging.SaveSerializedCalculateEvaluate("CONSEQUENCES", "BINARY_EQUATION", _id, _calculate);
                        }
                    }
                }
                else
                {
                    IsEquation = false;
                    _attributes = new List<string>();
                }
            }
        }

        public Consequences()
        {
            _table = null;
            _id = null;
            _criteriaColumn = null;
            _criteria = new Criterias();
        }

        public Consequences(string id)
        {
            _table = "CONSEQUENCES";
            _criteriaColumn = "BINARY_CRITERIA";
            _id = id;
            _criteria = new Criterias(_table, _criteriaColumn, _id);
        }

        /// <summary>
        /// Add a AttributeChange to consequence.  Consequences are stored for TREATMENTS so not continously SELECTed.  They are not stored for COMMITTED which are individually queried,
        /// </summary>
        /// <param name="change"></param>
        public void AddAttributeChange(AttributeChange change)
        {
            _attributeChange.Add(change);
        }

        /// <summary>
        /// Loads all the consequences that are part of a compound treatment.
        /// </summary>
        /// <param name="strAttribute"></param>
        /// <param name="strChange"></param>
        public void LoadAttributeChange(String strAttribute, String strChange)
        {
            AttributeChange attributeChange = new AttributeChange();
            attributeChange.Attribute = strAttribute;
            _attributes.Add(strAttribute);
            attributeChange.Change = strChange;


            if (SimulationMessaging.AttributeMaximum.Contains(strAttribute))
            {
                attributeChange.Maximum = SimulationMessaging.AttributeMaximum[strAttribute].ToString();
            }
            if (SimulationMessaging.AttributeMinimum.Contains(strAttribute))
            {
                attributeChange.Minimum = SimulationMessaging.AttributeMinimum[strAttribute].ToString();
            }
            AddAttributeChange(attributeChange);
        }

        /// <summary>
        /// Get consequence given an input value set.
        /// </summary>
        /// <param name="hashAttributeValue"></param>
        /// <returns></returns>
        public double GetConsequence(Hashtable hashAttributeValue)
        {
            int i = 0;
            object[] input = new object[_attributesEquation.Count];
            foreach (String str in _attributesEquation)
            {
                input[i] = hashAttributeValue[str];
                i++;
            }
            try
            {
                object result = _calculate.RunMethod(input);
                return (double)result;
            }
            catch (Exception exc)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error in RunMethod.   " + _calculate.OriginalInput + " " + exc.Message));
                return 0;
            }
        }

        /// <summary>
        /// Sets the function version of consequences.
        /// </summary>
        /// <param name="equation"></param>
        public void SetFunction(string equation)
        {
            _consequenceEquation = equation;
            IsEquation = true;
            _attributesEquation = SimulationMessaging.ParseAttribute(_consequenceEquation);
            if (_calculate == null)
            {
                _calculate = new CalculateEvaluate.CalculateEvaluate();
                string functionEquation = _consequenceEquation;

                foreach (String attribute in _attributesEquation)
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

                _calculate.BuildFunctionClass(_consequenceEquation, "double", "CONSEQUENCES_BINARY_EQUATION_" + _id);

                if (_calculate.m_cr == null)
                {
                    _compilerResultsEquation = _calculate.CompileAssembly();
                    SimulationMessaging.SaveSerializedCalculateEvaluate("CONSEQUENCES", "BINARY_EQUATION", _id, _calculate);
                }
            }
        }
    }
}
