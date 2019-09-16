using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer
{
    public class WorkPlanFilter
    {
        int _status =  -1;
        string _year;
        string _budget;
        string _activity;
        double _minimumCost = double.MinValue;
        double _maximumCost = double.MaxValue;
        double _minimumImpact = double.MinValue;
        double _maximumImpact = double.MaxValue;
        bool _useFilter = false;

        public bool UseFilter
        {
            get { return _useFilter; }
        }

        public int Status
        {
            get { return _status; }
        }

        public string Year
        {
            get { return _year; }
        }

        public string Budget
        {
            get { return _budget; }
        }

        public string Activity
        {
            get { return _activity; }
        }

        public double MaximumCost
        {
            get { return _maximumCost; }
        }

        public double MinimumImpact
        {
            get { return _minimumImpact; }
        }

        public double MinimumCost
        {
            get { return _minimumCost; }
        }

        public double MaximumImpact
        {
            get { return _maximumImpact; }
        }

        public WorkPlanFilter()
        {

        }

        public WorkPlanFilter(string status, string year, string budget, string activity, string costMin, string costMax, string impactMin, string impactMax)
        {
            _useFilter = true;
            
            _year = year;
            _budget = budget;
            _activity = activity;

            try
            {
                if (status != null) _status = Convert.ToInt32(status);
                if (costMin != null) _minimumCost = Convert.ToDouble(costMin);
                if (costMax != null) _maximumCost = Convert.ToDouble(costMax);
                if (impactMin != null) _minimumImpact = Convert.ToDouble(impactMin);
                if (impactMax != null) _maximumImpact = Convert.ToDouble(impactMax);
            }
            catch
            {

            }
        }

    }
}
