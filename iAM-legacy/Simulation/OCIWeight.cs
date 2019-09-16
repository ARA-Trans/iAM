using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation
{
    public class OCIWeight
    {
        string _conditionIndex;
        int _weight;
        string _criteria;
        Criterias _evaluate;

        public Criterias Evaluate
        {
            get { return _evaluate; }
        }

        public string ConditionIndex
        {
            get { return _conditionIndex; }
            set { _conditionIndex = value; }
        }


        public int Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        public string Criteria
        {
            get { return _criteria; }
            set { _criteria = value; }
        }

        public OCIWeight(string conditionIndex, int weight, string criteria, String ociID)
        {
            _conditionIndex = conditionIndex;
            _weight = weight;
            _criteria = criteria;
            _evaluate = new Criterias("OCI_WEIGHTS","CRITERIA",ociID);
            if (criteria == null)
            {
                _evaluate.Criteria = "";
            }
            else
            {
                _evaluate.Criteria = criteria;
            }
        }
    }
}
