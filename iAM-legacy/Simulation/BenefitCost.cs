using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation
{
    public class BenefitCost
    {
        private double _cost;
        private double _benefit;
        private double _benefitcost;

        public double Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }

        public double Benefit
        {
            get { return _benefit; }
            set { _benefit = value; }
        }

        public double Benefitcost
        {
            get { return _benefitcost; }
        }

        public BenefitCost(double benefit, double cost)
        {
            _benefit = benefit;
            _cost = cost;
            if (_cost < 0.01) _cost = 0.001;
            _benefitcost = _benefit / _cost;
        }

    }
}
