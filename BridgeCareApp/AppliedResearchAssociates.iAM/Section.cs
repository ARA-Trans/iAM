using System;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Section : IValidator
    {
        public Section(Facility facility) => Facility = facility ?? throw new ArgumentNullException(nameof(facility));

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

        public Facility Facility { get; }

        public string Name { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag();
    }
}
