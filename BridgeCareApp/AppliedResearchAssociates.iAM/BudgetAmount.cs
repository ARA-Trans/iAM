using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetAmount : IValidator
    {
        public ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (Value < 0)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "Value is less than zero."));
                }

                return results;
            }
        }

        public ICollection<IValidator> Subvalidators => new List<IValidator>();

        public decimal Value { get; set; }
    }
}
