using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataAccessLayer.DTOs
{
    [DataContract]
    public class ActivityConditionIndex
    {
        string _conditionIndex;
        double _minimumIndex =  0;
        double _maximumIndex = 100;
        string _impact;
        string _deConditionIndex;


        [DataMember]
        public string ConditionIndex
        {
            get { return _conditionIndex; }
            set { _conditionIndex = value; }
        }
        [DataMember]
        public double MinimumIndex
        {
            get { return _minimumIndex; }
            set { _minimumIndex = value; }
        }
        [DataMember]
        public double MaximumIndex
        {
            get { return _maximumIndex; }
            set { _maximumIndex = value; }
        }
        [DataMember]
        public string Impact
        {
            get { return _impact; }
            set { _impact = value; }
        }

        [DataMember]
        public string DEConditionIndex
        {
            get { return _deConditionIndex; }
            set { _deConditionIndex = value; }
        }

        public ActivityConditionIndex(OMSConditionIndexStore conditionIndex, List<string> expressions, List<ConsequenceStore> consequences)
        {
            _conditionIndex = conditionIndex.ConditionCategory;
            _deConditionIndex = conditionIndex.AttributeDE;

            ConsequenceStore consequence = consequences.Find(delegate(ConsequenceStore c) { return c.Attribute == "__" + conditionIndex.AttributeDE.Replace(" ", "").Replace("/", ""); });
            if (consequence != null)
            {
                _impact = consequence.Change;
            }
            
            foreach (string expression in expressions)
            {
                string partial = expression.Replace("(", "").Replace(")", "");
                if (partial.Contains("<"))// Maximum
                {
                    string[] values = partial.Split(' ');
                    string value = values[2].Replace("|", "");
                    _maximumIndex = double.Parse(value);
                }
                else if (partial.Contains(">")) //Minimum
                {
                    string[] values = partial.Split(' ');
                    string value = values[2].Replace("|", "");
                    _minimumIndex = double.Parse(value);
                }
            }
        }


        /// <summary>
        /// Parses Overall Condition Index
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="consequences"></param>
        public ActivityConditionIndex(List<string> expressions, List<ConsequenceStore> consequences)
        {
            _conditionIndex = "Overall Condition Index";
            ConsequenceStore consequence = consequences.Find(delegate(ConsequenceStore c) { return c.Attribute == "OverallConditionIndex"; });
            if (consequence != null)
            {
                _impact = consequence.Change;
            }

            foreach (string expression in expressions)
            {
                string partial = expression.Replace("(", "").Replace(")", "");
                if (partial.Contains("<"))// Maximum
                {
                    string[] values = partial.Split(' ');
                    string value = values[2].Replace("|", "");
                    _maximumIndex = double.Parse(value);
                }
                else if (partial.Contains(">")) //Minimum
                {
                    string[] values = partial.Split(' ');
                    string value = values[2].Replace("|", "");
                    _minimumIndex = double.Parse(value);
                }
            }
        }
      

        
        public ActivityConditionIndex(string conditionIndex, double minimumIndex, double maximumIndex, string impact)
        {
            _conditionIndex = conditionIndex;
            _minimumIndex = minimumIndex;
            _maximumIndex = maximumIndex;
            _impact = impact;
        }
    }
}
