using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class InvestmentPlan
    {
        public List<BudgetCategory> BudgetCategories { get; }

        public double DiscountRatePercentage { get; }

        public int FirstYearOfAnalysisPeriod { get; }

        public double InflationRatePercentage { get; }

        public int NumberOfYearsInAnalysisPeriod { get; }
    }
}
