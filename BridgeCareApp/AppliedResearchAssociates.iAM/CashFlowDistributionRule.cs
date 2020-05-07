using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CashFlowDistributionRule
    {
        public double CostCeiling { get; }

        public List<double> YearlyPercentages { get; }

        public int DurationInYears => YearlyPercentages.Count;
    }
}
