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

                return results;
            }
        }

        public IReadOnlyList<decimal> YearlyAmounts => _YearlyAmounts;

        public void SetYearlyAmount(int index, decimal amount) => _YearlyAmounts[index] = amount;

        internal int NumberOfYears
        {
            get => YearlyAmounts.Count;
            set
            {
                if (value < _YearlyAmounts.Count)
                {
                    _YearlyAmounts.RemoveRange(value, _YearlyAmounts.Count - value);
                }
                else if (value > _YearlyAmounts.Count)
                {
                    _YearlyAmounts.AddRange(Enumerable.Repeat(0m, value - _YearlyAmounts.Count));
                }
            }
        }

        private readonly List<decimal> _YearlyAmounts = new List<decimal>();
    }
}
