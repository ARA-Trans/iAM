using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class OMSPredictionPoints
    {
        string _conditionCategory;
        double _year;
        double _indexValue;

        public string ConditionCategory
        {
            get { return _conditionCategory; }
            set { _conditionCategory = value; }
        }

        public double Year
        {
            get { return _year; }
            set { _year = value; }
        }

        public double IndexValue
        {
            get { return _indexValue; }
            set { _indexValue = value; }
        }

        public OMSPredictionPoints()
        {

        }

        public OMSPredictionPoints(string conditionCategory, double year, double indexValue)
        {
            _conditionCategory = conditionCategory;
            _year = year;
            _indexValue = indexValue;
        }

        public override string ToString()
        {
            return "(" + _indexValue.ToString() + "," + _year.ToString() + ")";
        }
    }
}
