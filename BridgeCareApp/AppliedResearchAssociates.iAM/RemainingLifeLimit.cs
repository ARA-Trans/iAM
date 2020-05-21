using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class RemainingLifeLimit : IValidator
    {
        public Box<NumberAttribute> Attribute { get; } = new Box<NumberAttribute>();

        public Criterion Criterion { get; } = new Criterion();

        public ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (Attribute.Value == null)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, Attribute, "Attribute is unset."));
                }

                return results;
            }
        }

        public ICollection<IValidator> Subvalidators
        {
            get
            {
                var validators = new List<IValidator>();
                validators.Add(Criterion);
                return validators;
            }
        }

        public double Value { get; set; }
    }
}
