using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class SimulationActivityBudgetStore
    {
        string _activity;
        string _budget;
        string _cost;
        string _unit;

        public string Activity
        {
            get { return _activity; }
            set { _activity = value; }
        }

        public string Budget
        {
            get { return _budget; }
            set { _budget = value; }
        }

        public string Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        public SimulationActivityBudgetStore()
        {

        }

    }
}
