using System;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class RemainingLifeLimit : IValidator
    {
        public RemainingLifeLimit(Explorer explorer) => Criterion = new Criterion(explorer ?? throw new ArgumentNullException(nameof(explorer)));

        public INumericAttribute Attribute { get; set; }

        public Criterion Criterion { get; }

        public ValidatorBag Subvalidators => new ValidatorBag { Criterion };

        public double Value { get; set; }

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (Attribute == null)
            {
                results.Add(ValidationStatus.Error, "Attribute is unset.", this, nameof(Attribute));
            }

            return results;
        }
    }
}
