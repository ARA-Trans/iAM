using System;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetPercentage : IValidator
    {
        public Budget Budget { get; }

        public ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (Percentage < 0)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, Percentage, "Percentage is less than zero."));
                }
                else if (Percentage > 100)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, Percentage, "Percentage is greater than 100."));
                }

                return results;
            }
        }

        public Box<decimal> Percentage { get; } = new Box<decimal>();

        public ICollection<IValidator> Subvalidators => throw new NotImplementedException();

        internal BudgetPercentage(Budget budget) => Budget = budget ?? throw new ArgumentNullException(nameof(budget));
    }
}
