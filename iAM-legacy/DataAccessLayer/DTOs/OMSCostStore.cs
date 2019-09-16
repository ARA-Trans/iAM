using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class OMSCostStore
    {
        string _unit;
        double _costPerUnit;

        public double CostPerUnit
        {
            get { return _costPerUnit; }
            set { _costPerUnit = value; }
        }

        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        public OMSCostStore()
        {
        }

        public OMSCostStore(string unit, double costPerUnit)
        {
            _unit = unit;
            _costPerUnit = costPerUnit;
        }
    }
}
