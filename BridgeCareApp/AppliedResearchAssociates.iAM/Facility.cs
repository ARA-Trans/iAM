using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Facility : IValidator
    {
        public ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, Name, "Name is blank."));
                }

                return results;
            }
        }

        public Box<string> Name { get; } = new Box<string>();

        public ICollection<IValidator> Subvalidators => new List<IValidator>();
    }
}
