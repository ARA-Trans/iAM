using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation
{
    public class ConditionIndex
    {
        int _deteriorateIndex;
        double _fractionalYear;
        int _apparentAge;
        int _weight;

        public int Weight
        {
            get { return _weight; }
        }

        int _bcApparentAge;
        double _bcFractionalYear;
       
        public ConditionIndex(int weight)
        {
            _weight = weight;
        }

        public void SetDeteriorate(string deteriorateID)
        {
            _deteriorateIndex = SimulationMessaging.Deteriorates.FindIndex(delegate(Deteriorate d) { return d.PerformanceID == deteriorateID; });
        }

        public void SetApparentAge(double conditionIndexValue)
        {
            SimulationMessaging.Deteriorates[_deteriorateIndex].PiecewiseEquation.GetOMSApparentAge(conditionIndexValue,out _apparentAge,out _fractionalYear);            
        }

        public void SetBCApparentAge(double conditionIndexValue)
        {
            SimulationMessaging.Deteriorates[_deteriorateIndex].PiecewiseEquation.GetOMSApparentAge(conditionIndexValue, out _bcApparentAge, out _bcFractionalYear);
        }


        public double AddYears(double year)
        {
            int yearChange = Convert.ToInt32(year);
            _apparentAge += yearChange;
            _fractionalYear += (year - (double)yearChange);
            if (_fractionalYear > 1)
            {
                _apparentAge++;
                _fractionalYear -= 1;
            }
            return GetConditionIndex();
        }

        public double AddRemainingLifeYear()
        {
            double weightedCondition = 0;
            _bcApparentAge++;

            if (_bcApparentAge < SimulationMessaging.Deteriorates[_deteriorateIndex].PiecewiseEquation.YearlyAgeValue.Count - 1)
            {
                weightedCondition = SimulationMessaging.Deteriorates[_deteriorateIndex].PiecewiseEquation.YearlyAgeValue[_bcApparentAge].Value * _weight;
            }
            return weightedCondition;
        }



        public double GetConditionIndex()
        {
            return SimulationMessaging.Deteriorates[_deteriorateIndex].PiecewiseEquation.GetValue(_apparentAge, _fractionalYear);
        }

        public double GetWeightedConditionIndex()
        {
            _bcApparentAge = _apparentAge; //This so we can calculate remaining life
            return SimulationMessaging.Deteriorates[_deteriorateIndex].PiecewiseEquation.GetValue(_apparentAge, _fractionalYear) * _weight;

        }

        public double GetBenefit()
        {
            double benefit = 0;
            double delta = _fractionalYear - _bcFractionalYear;
            if (delta < 0)
            {
                delta += 1;
                _bcApparentAge--;
            }
            if (_bcApparentAge < 0) _bcApparentAge = 0;
            for (int i = _bcApparentAge; i < _apparentAge; i++)
            {
                benefit += SimulationMessaging.Deteriorates[_deteriorateIndex].PiecewiseEquation.YearlyAgeValue[i].Value;
            }
            benefit += SimulationMessaging.Deteriorates[_deteriorateIndex].PiecewiseEquation.YearlyAgeValue[_apparentAge].Value * delta;
            benefit *= _weight;
            return benefit;
        }
    }
}
