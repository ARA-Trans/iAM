using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Budget : IValidator
    {
        public ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, Name, "Name is blank."));
                }

                if (YearlyAmounts.Count == 0)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, YearlyAmounts, "There are no yearly amounts."));
                }

                return results;
            }
        }

        public Box<string> Name { get; } = new Box<string>();

        public ICollection<IValidator> Subvalidators => YearlyAmounts.ToList<IValidator>();

        public IReadOnlyList<BudgetAmount> YearlyAmounts => _YearlyAmounts;

        internal void SetNumberOfYears(int numberOfYears)
        {
            if (numberOfYears < _YearlyAmounts.Count)
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
