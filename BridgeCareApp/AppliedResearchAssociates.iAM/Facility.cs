using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Facility : IValidator
    {
        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
                }

                return results;
            }
        }

        public string Name { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag();
    }
}
