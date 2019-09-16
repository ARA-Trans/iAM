using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using SimulationDataAccess;

namespace Simulation
{
    public class OverallConditionIndex
    {
        Dictionary<string, ConditionIndex> _overallConditionIndex;
        int _sumWeight = 0;

        public int SumWeight
        {
            get { return _sumWeight; }
            set { _sumWeight = value; }
        }

        public Dictionary<string, ConditionIndex> ConditionIndices
        {
            get { return _overallConditionIndex; }
            set { _overallConditionIndex = value; }
        }

        public OverallConditionIndex(string criteria)
        {
            if (SimulationMessaging.IsOMS)
            {
                _overallConditionIndex = new Dictionary<string, ConditionIndex>();

				List<OCIWeight> ociMatchingCriteria = SimulationMessaging.ConditionCategoryWeight.FindAll(delegate(OCIWeight o) { return o.Criteria == criteria; });
				if (ociMatchingCriteria != null)
				{
					foreach (OCIWeight ociWeight in ociMatchingCriteria)
					{
						_overallConditionIndex.Add(ociWeight.ConditionIndex, new ConditionIndex(ociWeight.Weight));
						_sumWeight += ociWeight.Weight;
					}
				}
            }
        }

        public void SetConditionIndex(string conditionIndex, string performanceID)
        {
            if (_overallConditionIndex.ContainsKey(conditionIndex))
            {
                _overallConditionIndex[conditionIndex].SetDeteriorate(performanceID);
            }
        }

        public void UpdateConditionIndex(string conditionIndex, double value)
        {
            if (_overallConditionIndex.ContainsKey(conditionIndex))
            {
                _overallConditionIndex[conditionIndex].SetApparentAge(value); ;
            }

        }

        public double AddYears(string conditionIndex, double years)
        {
            if (_overallConditionIndex.ContainsKey(conditionIndex))
            {
                return _overallConditionIndex[conditionIndex].AddYears(years);
            }
            return 0;
        }

        public double GetOCI()
        {
            double OCI = 0;
            foreach (string key in _overallConditionIndex.Keys)
            {
                OCI += _overallConditionIndex[key].GetWeightedConditionIndex();
            }
            OCI = OCI / _sumWeight;
            return OCI;
        }

        public void GetBenefitAndRemainingLife(Hashtable attributeValueBefore, Hashtable attributeValueAfter, out double benefit, out int remainingLife, double deficientOCI)
        {
            benefit = 0;
            foreach(string key in _overallConditionIndex.Keys)
            {
                _overallConditionIndex[key].SetApparentAge((double)attributeValueBefore[key]);
                _overallConditionIndex[key].SetBCApparentAge((double)attributeValueAfter[key]);
                benefit += _overallConditionIndex[key].GetBenefit();
            }
            benefit = benefit / _sumWeight;
            remainingLife = GetRemainingLife(deficientOCI);
        }

        public void UpdateApparentAge(Hashtable attributeValueBefore)
        {
            foreach (string key in _overallConditionIndex.Keys)
            {
                _overallConditionIndex[key].SetApparentAge((double)attributeValueBefore[key]);
            }
        }




        public int GetRemainingLife(double deficientOCI)
        {
            int remainingLife = 0;
            while (true)
            {
                double remainingLifeOCI = 0;
                foreach (string key in _overallConditionIndex.Keys)
                {
                    remainingLifeOCI += _overallConditionIndex[key].AddRemainingLifeYear();
                }
                remainingLifeOCI = remainingLifeOCI / _sumWeight;
                if (remainingLifeOCI <= deficientOCI)
                {
                    break;
                }
                else
                {
                    remainingLife++;
                }
            }
            return remainingLife;

        }
    }
}
