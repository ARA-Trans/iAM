using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM
{
    public class InvestmentPlan
    {
        // TODO: Split treatment configurations... Need detailed description of what the UI elements mean from Gregg.

        public List<BudgetCondition> BudgetConditions { get; }

        public List<Budget> Budgets { get; }

        public double DiscountRatePercentage { get; }

        public int FirstYearOfAnalysisPeriod { get; }

        public double InflationRatePercentage { get; }

        public int NumberOfYearsInAnalysisPeriod { get; }

        public IEnumerable<int> YearsOfAnalysis => Enumerable.Range(FirstYearOfAnalysisPeriod, NumberOfYearsInAnalysisPeriod);
    }
}
