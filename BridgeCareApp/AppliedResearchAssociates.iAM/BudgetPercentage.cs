using System;

namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetPercentage
    {
        public Budget Budget { get; }

        public double Percentage { get; set; }

        internal BudgetPercentage(Budget budget) => Budget = budget ?? throw new ArgumentNullException(nameof(budget));
    }
}
