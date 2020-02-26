using System;
using System.Collections;
using System.Collections.Generic;



namespace Simulation
{   /// <summary>
/// Stores calculated attributes for simulation
/// </summary>
    internal class CalculatedAttribute
    {
        public string Attribute { get; }
        public int Id { get; }
        public bool Default { get; private set; }
        private string _equation;
        private string _criteria;
        public List<string> AttributesCriteria { get; private set; }
        public List<string> AttributesEquation { get; private set; }
        private CalculateEvaluate.CalculateEvaluate _evaluate;
        private CalculateEvaluate.CalculateEvaluate _calculate;

        public string Equation
        {
            get => _equation;
            set
            {
                _equation = value;
                // Get list of attributes
                AttributesEquation = SimulationMessaging.ParseAttribute(_equation);
                if (AttributesEquation == null) return;
                _calculate = new CalculateEvaluate.CalculateEvaluate();
                _calculate.BuildClass(_equation, true, "CALCULATED_BINARY_EQUATION_" + Id);
                _calculate.CompileAssembly();

            }
        }

        public string Criteria
        {
            get => _criteria;
            set
            {
                _criteria = value;
                if (_criteria.TrimEnd().Length == 0)
                {
                    Default = true;
                    AttributesCriteria = new List<string>();
                    return;
                }

                AttributesCriteria = SimulationMessaging.ParseAttribute(_criteria);
                if (AttributesCriteria == null)
                {
                    return;
                }

                if (_evaluate != null) return;
                var strCriteria = _criteria;
                foreach (var str in AttributesCriteria)
                {
                    var strType = SimulationMessaging.GetAttributeType(str);
                    if (strType == "STRING")
                    {
                        var strOldValue = "[" + str + "]";
                        var strNewValue = "[@" + str + "]";
                        strCriteria = strCriteria.Replace(strOldValue, strNewValue);
                    }
                    else if (strType == "DATETIME")
                    {
                        var strOldValue = "[" + str + "]";
                        var strNewValue = "[$" + str + "]";
                        strCriteria = strCriteria.Replace(strOldValue, strNewValue);
                    }
                }
                _evaluate = new CalculateEvaluate.CalculateEvaluate();
                _evaluate.BuildClass(strCriteria, false, "CALCULATED_BINARY_CRITERIA_" + Id);
                _evaluate.CompileAssembly();
            }
        }

        /// <summary>
        /// Attributes which are calculated for each time step of the simulation
        /// </summary>
        /// <param name="id">Database Id for calculated attribute</param>
        /// <param name="attribute">Attribute name</param>
        /// <param name="equation">Equation for calculating attribute</param>
        /// <param name="criteria">Criteria for determining if to use this equation</param>
        public CalculatedAttribute(int id, string attribute, string equation, string criteria)
        {
            Id = id;
            Attribute = attribute;
            Equation = equation;
            Criteria = criteria;
        }
        public bool IsCriteriaMet(Hashtable hashAttributeValue)
        {
            if (string.IsNullOrWhiteSpace(_criteria)) return true;
            var input = new object[AttributesCriteria.Count];
            var i = 0;
            foreach (var str in AttributesCriteria)
            {
                input[i] = hashAttributeValue[str];
                i++;
            }
            try
            {
                return (bool)_evaluate.RunMethod(input);
            }
            catch (Exception exc)
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error in RunMethod. "  + _evaluate.OriginalInput + " " + exc.Message));
                return false;
            }
        }

        public double Calculate(Hashtable hashAttributeValue)
        {
            var input = new object[AttributesEquation.Count];

            var i = 0;
            foreach (var str in AttributesEquation)
            {
                input[i] = hashAttributeValue[str];
                i++;
            }

            try
            {
                return (double) _calculate.RunMethod(input);
            }
            catch
            {
                SimulationMessaging.AddMessage(new SimulationMessage("Error: Resolving calculated field" + Attribute));
                return 0;
            }
        }
    }
}
