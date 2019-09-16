using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class OCIWeightStore
    {
        string _conditionCategory;
        int _weight;
        string _criteria;

        /// <summary>
        /// Condition category index field
        /// </summary>
        public string ConditionCategory
        {
            get { return _conditionCategory; }
            set { _conditionCategory = value; }
        }


        /// <summary>
        /// Weighting to apply to Condition Index
        /// </summary>
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


        public OCIWeightStore()
        {

        }


        public OCIWeightStore(string conditionCategory, int weight, string criteria)
        {
            _conditionCategory = conditionCategory;
            _weight = weight;
            _criteria = criteria;
        }

        public override string ToString()
        {
            return _conditionCategory + "(" + _weight + ")";
        }

    }
}
