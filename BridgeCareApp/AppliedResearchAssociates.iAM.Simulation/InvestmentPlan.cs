using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class InvestmentPlan
    {
        public List<BudgetCategory> BudgetCategories { get; }

        public decimal DiscountRatePercentage { get; }

        public int FirstYearOfAnalysisPeriod { get; }

        public decimal InflationRatePercentage { get; }

        public int NumberOfYearsInAnalysisPeriod { get; }
    }
}
