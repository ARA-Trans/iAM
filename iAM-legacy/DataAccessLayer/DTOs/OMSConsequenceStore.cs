using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class OMSConsequenceStore
    {
        string _conditionCategory;
        string _impact;
        string _impactUnit;

        public string Impact
        {
            get { return _impact; }
            set { _impact = value; }
        }

        public string ImpactUnit
        {
            get { return _impactUnit; }
            set { _impactUnit = value; }
        }

        public string ConditionCategory
        {
            get { return _conditionCategory; }
            set { _conditionCategory = value; }
        }
        public OMSConsequenceStore()
        {
        }

        public OMSConsequenceStore(string conditionCategory, string impact, string impactUnit)
        {
            _conditionCategory = conditionCategory;
            _impact = impact;
            _impactUnit = impactUnit;
        }

        public string GetDecisionEngineConsequence()
        {
            string change = "";
            if(_impactUnit == "Absolute")
            {
                change = _impact;
            }
            else
            {
                string suffix = _impactUnit;
                if (_impactUnit == "Relative") suffix = "";
                if(_impact.Contains("-"))
                {
                    change = _impact + suffix;
                }
                else
                {
                    change = "+" + _impact + suffix;
                }
            }
            return change;
        }

        public override string ToString()
        {
            return _conditionCategory + "(" + GetDecisionEngineConsequence() + ")";
        }
    }
}
