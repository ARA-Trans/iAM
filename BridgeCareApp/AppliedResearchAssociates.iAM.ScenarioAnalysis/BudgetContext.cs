using System;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal sealed class BudgetContext
    {
        public BudgetContext(Budget budget) => Budget = budget ?? throw new ArgumentNullException(nameof(budget));

        public Budget Budget { get; }

        public decimal CurrentAmount { get; private set; }

        public void AllocateCost(decimal cost) => CurrentAmount -= cost;

        public void MoveToNextYear() => CurrentAmount += Budget.YearlyAmounts[++CurrentYearIndex];

        private int CurrentYearIndex = -1;
    }
}
