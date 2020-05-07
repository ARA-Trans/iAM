using System;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal sealed class BudgetContext
    {
        public Budget Budget { get; }
        public double CurrentAmount { get; private set; }

        private int CurrentYearIndex = -1;

        public BudgetContext(Budget budget) => Budget = budget ?? throw new ArgumentNullException(nameof(budget));

        public void MoveToNextYear() => CurrentAmount += Budget.YearlyAmounts[++CurrentYearIndex];
    }
}
