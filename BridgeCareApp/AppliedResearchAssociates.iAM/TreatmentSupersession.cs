using System;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class TreatmentSupersession : IValidator
    {
        public TreatmentSupersession(Explorer explorer) => Criterion = new Criterion(explorer ?? throw new ArgumentNullException(nameof(explorer)));

        public Criterion Criterion { get; }

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

        public SelectableTreatment Treatment { get; set; }
    }
}
