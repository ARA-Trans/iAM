using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CashFlowDistributionRule
    {
        public decimal CostCeiling { get; set; }

        public int NumberOfYears
        {
            get => YearlyPercentages.Count;
            set
            {
                if (value < YearlyPercentages.Count)
                {
                    YearlyPercentages.RemoveRange(value, YearlyPercentages.Count - value);
                }
                else if (value > YearlyPercentages.Count)
                {
                    YearlyPercentages.AddRange(Enumerable.Repeat(0m, value - YearlyPercentages.Count));
                }
            }
        }

        public List<decimal> YearlyPercentages { get; }
    }
}
