using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public abstract class ConditionGoal : IValidator
    {
        public virtual Box<NumberAttribute> Attribute { get; } = new Box<NumberAttribute>();

        public Criterion Criterion { get; } = new Criterion();

        public Box<string> Name { get; } = new Box<string>();

        public virtual ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (Attribute.Value == null)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, Attribute, "Attribute is unset."));
                }

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, Name, "Name is blank."));
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

        public abstract bool IsMet(double actual);
    }
}
