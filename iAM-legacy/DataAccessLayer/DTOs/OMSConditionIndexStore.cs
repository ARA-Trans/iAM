using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class OMSConditionIndexStore
    {
        string _attributeDE;
        string _conditionCategory;

        public string AttributeDE
        {
            get { return _attributeDE; }
            set { _attributeDE = value; }
        }

        public string ConditionCategory
        {
            get { return _conditionCategory; }
            set { _conditionCategory = value; }
        }

        public OMSConditionIndexStore(string conditionCategory)
        {
            _conditionCategory = conditionCategory;
            _attributeDE = conditionCategory.Replace(" ", "");
        }

        public override string ToString()
        {
            return _conditionCategory;
        }
    }
}
