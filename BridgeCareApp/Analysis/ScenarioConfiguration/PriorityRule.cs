using System.Collections.Generic;
using AASHTOWare;
using ARA.iAM.Analysis.Interfaces;

namespace ARA.iAM.Analysis
{
    class PriorityRule: ICriteriaDriven
    {
        public Criterion Criterion { get; }
        private int priority;
        private Option<int> year;
        private Dictionary<BudgetID, double> budgetPercentages;
    }
}
