using System;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetPercentagePair : IValidator
    {
        public BudgetPercentagePair(Budget budget) => Budget = budget ?? throw new ArgumentNullException(nameof(budget));

        public Budget Budget { get; }

        public decimal Percentage { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag();

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (Percentage < 0)
            {
                results.Add(ValidationStatus.Error, "Percentage is less than zero.", this, nameof(Percentage));
            }
            else if (Percentage > 100)
            {
                results.Add(ValidationStatus.Error, "Percentage is greater than 100.", this, nameof(Percentage));
            }

            return results;
        }
    }
}
