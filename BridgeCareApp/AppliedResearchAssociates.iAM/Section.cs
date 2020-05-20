using System;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Section : IValidator
    {
        public Section(Facility facility) => Facility = facility ?? throw new ArgumentNullException(nameof(facility));

        public Facility Facility { get; }

        public string Name { get; set; }

        public ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationStatus.Error.Describe("Name is blank."));
                }

                return results;
            }
        }
    }
}
