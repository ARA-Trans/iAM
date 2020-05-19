using System;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetPercentage : IValidator
    {
        public Budget Budget { get; }

        public decimal Percentage { get; set; }

        public ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (Percentage < 0)
                {
                    results.Add(ValidationStatus.Error.Describe("Percentage is less than zero."));
                }
                else if (Percentage > 100)
                {
                    results.Add(ValidationStatus.Error.Describe("Percentage is greater than 100."));
                }

                return results;
            }
        }

        internal BudgetPercentage(Budget budget) => Budget = budget ?? throw new ArgumentNullException(nameof(budget));
    }
}
