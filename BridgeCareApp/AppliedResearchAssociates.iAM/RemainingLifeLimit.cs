using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class RemainingLifeLimit : IValidator
    {
        public NumberAttribute Attribute { get; set; }

        public Criterion Criterion { get; } = new Criterion();

        public double Value { get; set; }

        public ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (Attribute == null)
                {
                    results.Add(ValidationStatus.Error.Describe("Attribute is unset."));
                }

                return results;
            }
        }
    }
}
