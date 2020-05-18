using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class TreatmentSupersession : IValidator
    {
        public Criterion Criterion { get; } = new Criterion();

        public SelectableTreatment Treatment { get; set; }

        public ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (Treatment == null)
                {
                    results.Add(ValidationStatus.Error.Describe("Treatment is unset."));
                }

                return results;
            }
        }
    }
}
