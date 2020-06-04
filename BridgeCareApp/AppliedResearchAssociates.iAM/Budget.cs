using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Budget : IValidator
    {
        public string Name { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag { YearlyAmounts };

        public IReadOnlyList<BudgetAmount> YearlyAmounts => _YearlyAmounts;

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (string.IsNullOrWhiteSpace(Name))
            {
                results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
            }

            if (YearlyAmounts.Count == 0)
            {
                results.Add(ValidationStatus.Error, "There are no yearly amounts.", this, nameof(YearlyAmounts));
            }

            return results;
        }

        internal void SetNumberOfYears(int numberOfYears)
        {
            if (numberOfYears <= 0)
            {
                _YearlyAmounts.Clear();
            }
            else if (numberOfYears < _YearlyAmounts.Count)
            {
                _YearlyAmounts.RemoveRange(numberOfYears, _YearlyAmounts.Count - numberOfYears);
            }
            else if (numberOfYears > _YearlyAmounts.Count)
            {
                _YearlyAmounts.AddRange(Enumerable.Range(0, numberOfYears - _YearlyAmounts.Count).Select(_ => new BudgetAmount()));
            }
        }

        private readonly List<BudgetAmount> _YearlyAmounts = new List<BudgetAmount>();
    }
}
