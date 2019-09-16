using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class PriorityFundStore
    {
        string _budget;
        double _funding;

        public string Budget
        {
            get { return _budget; }
            set { _budget = value; }
        }

        public double Funding
        {
            get { return _funding; }
            set { _funding = value; }
        }

        public PriorityFundStore()
        {
        }

        public PriorityFundStore(string budget, double funding)
        {
            _budget = budget;
            _funding = funding;
        }

    }
}
