using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class TreatmentScheduling : IValidator
    {
        public int OffsetToFutureYear { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag();

        public SelectableTreatment Treatment { get; set; }

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (OffsetToFutureYear < 1)
            {
                results.Add(ValidationStatus.Error, "Offset to future year is less than one.", this, nameof(OffsetToFutureYear));
            }

            if (Treatment == null)
            {
                results.Add(ValidationStatus.Error, "Treatment is unset.", this, nameof(Treatment));
            }

            return results;
        }
    }
}
