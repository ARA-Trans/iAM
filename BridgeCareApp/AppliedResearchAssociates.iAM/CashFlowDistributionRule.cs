using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CashFlowDistributionRule
    {
        public int YearsOfDuration => YearlyPercentages.Count;
        public double CostCeiling { get; }
        public List<double> YearlyPercentages { get; }
    }
}
