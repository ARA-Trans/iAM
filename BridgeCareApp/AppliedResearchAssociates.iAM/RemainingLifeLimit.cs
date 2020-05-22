using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class RemainingLifeLimit : IValidator
    {
        public NumberAttribute Attribute { get; set; }

        public Criterion Criterion { get; } = new Criterion();

        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (Attribute == null)
                {
                    results.Add(ValidationStatus.Error, "Attribute is unset.", this, nameof(Attribute));
                }

                return results;
            }
        }

        public ValidatorBag Subvalidators => new ValidatorBag { Criterion };

        public double Value { get; set; }
    }
}
