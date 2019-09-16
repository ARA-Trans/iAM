using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class OMSCategoryPrediction
    {
        string _conditionCategory;
        List<OMSPredictionPoints> _points;

        public string ConditionCategory
        {
            get { return _conditionCategory; }
            set { _conditionCategory = value; }
        }

        public List<OMSPredictionPoints> Points
        {
            get { return _points; }
            set { _points = value; }
        }

        public OMSCategoryPrediction()
        {
        }


        public OMSCategoryPrediction(string conditionCategory)
        {
            _conditionCategory = conditionCategory;
            _points = new List<OMSPredictionPoints>();
        }

        public string GetPiecewiseCurve()
        {
            string piecewise = "";
            foreach (OMSPredictionPoints point in _points)
            {
                piecewise += point;
            }
            return piecewise;
        }
    }
}
