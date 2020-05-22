using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetAmount : IValidator
    {
        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (Value < 0)
                {
                    results.Add(ValidationStatus.Error, "Value is less than zero.", this, nameof(Value));
                }

                return results;
            }
        }

        public ValidatorBag Subvalidators => new ValidatorBag();

        public decimal Value { get; set; }
    }
}
