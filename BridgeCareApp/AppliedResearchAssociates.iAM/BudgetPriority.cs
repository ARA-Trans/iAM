using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetPriority : IValidator
    {
        public BudgetPriority(Explorer explorer) => Criterion = new Criterion(explorer ?? throw new ArgumentNullException(nameof(explorer)));

        public IReadOnlyCollection<BudgetPercentagePair> BudgetPercentagePairs => _BudgetPercentagePairs;

        public Criterion Criterion { get; }

        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (BudgetPercentagePairs.All(budgetPercentage => budgetPercentage.Percentage == 0))
                {
                    results.Add(ValidationStatus.Warning, "All percentages are zero.", this, nameof(BudgetPercentagePairs));
                }

                return results;
            }
        }

        public int PriorityLevel { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag { BudgetPercentagePairs, Criterion };

        public int? Year { get; set; }

        public BudgetPercentagePair GetBudgetPercentagePair(Budget budget) => PairByBudget[budget];

        internal void SynchronizeWithBudgets(IEnumerable<Budget> budgets)
        {
            _ = _BudgetPercentagePairs.RemoveAll(budgetPercentage => !budgets.Contains(budgetPercentage.Budget));

            _BudgetPercentagePairs.AddRange(
                from budget in budgets
                where !BudgetPercentagePairs.Any(budgetPercentage => budgetPercentage.Budget == budget)
                select new BudgetPercentagePair(budget));

            PairByBudget.Clear();
            foreach (var pair in BudgetPercentagePairs)
            {
                PairByBudget.Add(pair.Budget, pair);
            }
        }

        private readonly List<BudgetPercentagePair> _BudgetPercentagePairs = new List<BudgetPercentagePair>();

        private readonly Dictionary<Budget, BudgetPercentagePair> PairByBudget = new Dictionary<Budget, BudgetPercentagePair>();
    }
}
