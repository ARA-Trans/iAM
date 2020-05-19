using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public abstract class ConditionGoal : IValidator
    {
        public virtual NumberAttribute Attribute { get; set; }

        public Criterion Criterion { get; } = new Criterion();

        public string Name { get; set; }

        public virtual ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (Attribute == null)
                {
                    results.Add(ValidationStatus.Error.Describe("Attribute is unset."));
                }

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationStatus.Error.Describe("Name is blank."));
                }

                return results;
            }
        }

        public abstract bool IsMet(double actual);
    }
}
