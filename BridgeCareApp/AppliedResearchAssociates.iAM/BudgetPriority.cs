using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetPriority : IValidator
    {
        public IReadOnlyCollection<BudgetPercentage> BudgetPercentages => _BudgetPercentages;

        public Criterion Criterion { get; } = new Criterion();

        public ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (BudgetPercentages.Sum(budgetPercentage => budgetPercentage.Percentage) > 100)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "Sum of percentages is greater than 100."));
                }
                else if (BudgetPercentages.All(budgetPercentage => budgetPercentage.Percentage == 0))
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Warning, this, "All percentages are zero."));
                }

                return results;
            }
        }

        public int PriorityLevel { get; set; }

        public ICollection<IValidator> Subvalidators
        {
            get
            {
                var validators = new List<IValidator>();
                validators.AddRange(BudgetPercentages);
                validators.Add(Criterion);
                return validators;
            }
        }

        public int? Year { get; set; }

        public decimal GetBudgetPercentage(Budget budget) => PercentagePerBudget[budget].Percentage;

        internal void SynchronizeWithBudgets(IEnumerable<Budget> budgets)
        {
            _ = _BudgetPercentages.RemoveAll(budgetPercentage => !budgets.Contains(budgetPercentage.Budget));

            _BudgetPercentages.AddRange(
                from budget in budgets
                where !BudgetPercentages.Any(budgetPercentage => budgetPercentage.Budget == budget)
                select new BudgetPercentage(budget));

            PercentagePerBudget.Clear();
            foreach (var budgetPercentage in BudgetPercentages)
            {
                PercentagePerBudget.Add(budgetPercentage.Budget, budgetPercentage);
            }
        }

        private readonly List<BudgetPercentage> _BudgetPercentages = new List<BudgetPercentage>();

        private readonly Dictionary<Budget, BudgetPercentage> PercentagePerBudget = new Dictionary<Budget, BudgetPercentage>();
    }
}
