using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Budget : IValidator
    {
        public string Name { get; set; }

        public ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationStatus.Error.Describe("Name is blank."));
                }

                if (YearlyAmounts.Count == 0)
                {
                    results.Add(ValidationStatus.Error.Describe("There are no yearly amounts."));
                }
                else if (YearlyAmounts.Any(amount => amount < 0))
                {
                    results.Add(ValidationStatus.Warning.Describe("At least one yearly amount is less than zero."));
                }

                return results;
            }
        }

        public IReadOnlyList<decimal> YearlyAmounts => _YearlyAmounts;

        public void SetYearlyAmount(int index, decimal amount) => _YearlyAmounts[index] = amount;

        internal void SetNumberOfYears(int numberOfYears)
        {
            if (numberOfYears < _YearlyAmounts.Count)
            {
                _YearlyAmounts.RemoveRange(numberOfYears, _YearlyAmounts.Count - numberOfYears);
            }
            else if (numberOfYears > _YearlyAmounts.Count)
            {
                _YearlyAmounts.AddRange(Enumerable.Repeat(0m, numberOfYears - _YearlyAmounts.Count));
            }
        }

        private readonly List<decimal> _YearlyAmounts = new List<decimal>();
    }
}
