using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetPriority
    {
        public IReadOnlyCollection<BudgetPercentage> BudgetPercentages => _BudgetPercentages;

        public Criterion Criterion { get; } = new Criterion();

        public int PriorityLevel { get; set; }

        public int? Year { get; set; }

        internal void SynchronizeWithBudgets(IEnumerable<Budget> budgets)
        {
            _ = _BudgetPercentages.RemoveAll(budgetPercentage => !budgets.Contains(budgetPercentage.Budget));

            _BudgetPercentages.AddRange(
                from budget in budgets
                where !BudgetPercentages.Any(budgetPercentage => budgetPercentage.Budget == budget)
                select new BudgetPercentage(budget));
        }

        private List<BudgetPercentage> _BudgetPercentages = new List<BudgetPercentage>();
    }
}
