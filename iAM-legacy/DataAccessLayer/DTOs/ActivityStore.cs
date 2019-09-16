using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataAccessLayer.DTOs
{
    /// <summary>
    /// Activity stores all the information about a Treatment (Roadcare Treatment = Cartegraph Activity)
    /// </summary>
    /// 
    [DataContract]
    public class ActivityStore
    {
        string _activity;
        string _activityID;
        bool _isRepeatActivity;
        bool _isExclusive;
        bool _isSelected;
        int _repeatInterval = -1;
        int _repeatStart = -1;
        List<ActivityConditionIndex> _conditionIndexes;
        string _criteria;
        string _whereClause;
        string _assetName;
        string _cost;
        string _costField;
        List<string> _budgets;
        int _beforeSame;
        string _omsOID;
        string _rawCriteria;
        string _rawCost;
        [DataMember]
        public string Activity
        {
            get { return _activity; }
            set { _activity = value; }
        }

        [DataMember]
        public string ActivityID
        {
            get { return _activityID; }
            set { _activityID = value; }
        }

        [DataMember]
        public int RepeatInterval
        {
            get { return _repeatInterval; }
            set { _repeatInterval = value; }
        }
        [DataMember]
        public int RepeatStart
        {
            get { return _repeatStart; }
            set { _repeatStart = value; }
        }

        [DataMember]
        public List<ActivityConditionIndex> ConditionIndexes
        {
            get { return _conditionIndexes; }
            set { _conditionIndexes = value; }
        }
        [DataMember]
        public string Criteria
        {
            get { return _criteria; }
            set { _criteria = value; }
        }
        [DataMember]
         public bool IsRepeatActivity
        {
            get { return _isRepeatActivity; }
            set { _isRepeatActivity = value; }
        }
        [DataMember]
        public bool IsExclusive
        {
            get { return _isExclusive; }
            set { _isExclusive = value; }
        }

        [DataMember]
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        [DataMember]
        public string Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        [DataMember]
        public string CostField
        {
            get { return _costField; }
            set { _costField = value; }
        }

        [DataMember]
        public int YearSame
        {
            get { return _beforeSame; }
            set { _beforeSame = value; }
        }

        [DataMember]
        public List<string> Budgets
        {
            get { return _budgets; }
            set { _budgets = value; }
        }



        [DataMember]
        public string OmsOID
        {
            get { return _omsOID; }
            set { _omsOID = value; }
        }
        [DataMember]
        public string RawCriteria
        {
            get { return _rawCriteria; }
            set { _rawCriteria = value; }
        }


        [DataMember]
        public string RawCost
        {
            get { return _rawCost; }
            set { _rawCost = value; }
        }





        public ActivityStore(string assetName)
        {
            _assetName = assetName;
        }

        public ActivityStore()
        {
        }

        private static string RemoveDoubleWhiteSpace(string input)
        {
            while (input.Contains("  "))
            {
                input = input.Replace("  ", " ");
            }
            return input;
        }



        /// <summary>
        /// Parses a DecisionEngine WHERE clause into a display element.
        /// </summary>
        public void ParseCriteria(string criteria, List<ConsequenceStore> consequences)
        {
            if (criteria != null)
            {
                criteria = RemoveDoubleWhiteSpace(criteria);
                _whereClause = criteria;
 
                List<string> listCriteria = new List<string>();//List of omsExpressions
                int lastOpen = -1;
                int i = 0;
                while (i < _whereClause.Length)//Find all expressions
                {
                    if (_whereClause[i] == '(')
                    {
                        lastOpen = i;
                    }
                    if (_whereClause[i] == ')' && lastOpen >= 0)
                    {
                        string expression = _whereClause.Substring(lastOpen, i - lastOpen + 1);
                        if (expression.Contains("["))
                        {
                            byte numberExpressions = (byte)listCriteria.Count;
                            string placeHolder = numberExpressions.ToString("x2");
                            listCriteria.Add(expression);
                            _whereClause = _whereClause.Replace(expression, placeHolder);
                            i = 0;
                        }
                        lastOpen = -1;
                    }
                    else
                    {
                        i++;
                    }
                }

                OMSAssetConditionIndexStore oci = OMS.GetAssetConditionIndex(_assetName);
                _conditionIndexes = new List<ActivityConditionIndex>();
                foreach (OMSConditionIndexStore ci in oci.ConditionIndexes)
                {
                    List<string> conditionExpressions = new List<string>();
                    for (int j = 0; j < listCriteria.Count; j++)
                    {
                        string expression = listCriteria[j];
                        if (expression != null)
                        {
                            string attribute = "[__" + ci.AttributeDE + "]";
                            if (expression.Contains(attribute))
                            {
                                conditionExpressions.Add(expression);

                                //Remove expression from criteria to get non-condition index criteria
                                criteria = criteria.Replace("AND " + expression, "");
                                criteria = criteria.Replace(expression, "");

                                listCriteria[j] = null;
                            }
                        }
                    }
                    ActivityConditionIndex activityConditionIndex = new ActivityConditionIndex(ci, conditionExpressions, consequences);
                    _conditionIndexes.Add(activityConditionIndex);
                }

                //Pickup OverallConditionIndex
                List<string> OCIExpressions = new List<string>();
                for (int j = 0; j < listCriteria.Count; j++)
                {
                    string expression = listCriteria[j];
                    if (expression != null)
                    {
                        string attribute = "[OverallConditionIndex]";
                        if (expression.Contains(attribute))
                        {
                            OCIExpressions.Add(expression);
               
                            //Remove expression from criteria to get non-condition index criteria
                            criteria = criteria.Replace("AND " + expression, "");
                            criteria = criteria.Replace(expression, "");
                            
                            listCriteria[j] = null;
                        }
                    }
                }
                ActivityConditionIndex activityOCI = new ActivityConditionIndex(OCIExpressions, consequences);
                _conditionIndexes.Add(activityOCI);

                
                _criteria = criteria.Trim();
                if(_criteria.Length > 3 && _criteria.Substring(0,3) == "AND")
                {
                    _criteria = _criteria.Substring(3);
                }

            }
        }
    }
}
