using System;

namespace AppliedResearchAssociates.iAM.Analysis
{
    internal sealed class BudgetContext
    {
        public BudgetContext(Budget budget) => Budget = budget ?? throw new ArgumentNullException(nameof(budget));

        public Budget Budget { get; }

        public decimal CurrentAmount { get; private set; }

        public decimal? CurrentPrioritizedAmount { get; private set; }

        public BudgetPriority Priority
        {
            set
            {
                var prioritizedFraction = value?.GetBudgetPercentagePair(Budget).Percentage / 100;
                CurrentPrioritizedAmount = CurrentAmount * prioritizedFraction;
            }
        }

        public void AllocateCost(decimal cost)
        {
            CurrentAmount -= cost;
            CurrentPrioritizedAmount -= cost;
        }

        public void MoveToNextYear()
        {
            CurrentAmount += Budget.YearlyAmounts[++CurrentYearIndex].Value;
            CurrentPrioritizedAmount = null;
        }

        private int CurrentYearIndex = -1;
    }
}
