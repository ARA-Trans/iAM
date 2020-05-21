using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class NumberAttribute : Attribute<double>
    {
        public override ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = base.DirectValidationResults;

                if (Minimum.Value.HasValue && double.IsNaN(Minimum.Value.Value))
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, Minimum, "Minimum is not a number."));
                }

                if (Maximum.Value.HasValue && double.IsNaN(Maximum.Value.Value))
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, Minimum, "Maximum is not a number."));
                }

                if (Minimum.Value is double minimum && Maximum.Value is double maximum)
                {
                    if (minimum == maximum)
                    {
                        results.Add(ValidationResult.Create(ValidationStatus.Warning, this, "Minimum is equal to maximum."));
                    }
                    else if (minimum > maximum)
                    {
                        results.Add(ValidationResult.Create(ValidationStatus.Error, this, "Minimum is greater than maximum."));
                    }
                }

                return results;
            }
        }

        public bool IsDecreasingWithDeterioration { get; set; }

        public Box<double?> Maximum { get; } = new Box<double?>();

        public Box<double?> Minimum { get; } = new Box<double?>();

        internal NumberAttribute(Explorer explorer) : base(explorer)
        {
        }
    }
}
