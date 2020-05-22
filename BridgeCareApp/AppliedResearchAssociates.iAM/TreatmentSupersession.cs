using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class TreatmentSupersession : IValidator
    {
        public Criterion Criterion { get; } = new Criterion();

        public SelectableTreatment Treatment { get; set; }

        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (Treatment == null)
                {
                    results.Add(ValidationStatus.Error, "Treatment is unset.", this, nameof(Treatment));
                }

                return results;
            }
        }

        public ValidatorBag Subvalidators => new ValidatorBag { Criterion };
    }
}
