using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class NumberAttribute : Attribute<double>
    {
        public bool IsDecreasingWithDeterioration { get; set; }

        public double? Maximum { get; set; }

        public double? Minimum { get; set; }

        public override ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = base.ValidationResults;

                if (Minimum.HasValue && double.IsNaN(Minimum.Value))
                {
                    results.Add(ValidationStatus.Error.Describe("Minimum is not a number."));
                }

                if (Maximum.HasValue && double.IsNaN(Maximum.Value))
                {
                    results.Add(ValidationStatus.Error.Describe("Maximum is not a number."));
                }

                if (Minimum is double minimum && Maximum is double maximum)
                {
                    if (minimum == maximum)
                    {
                        results.Add(ValidationStatus.Warning.Describe("Minimum is equal to maximum."));
                    }
                    else if (minimum > maximum)
                    {
                        results.Add(ValidationStatus.Error.Describe("Minimum is greater than maximum."));
                    }
                }

                return results;
            }
        }

        internal NumberAttribute(Explorer explorer) : base(explorer)
        {
        }
    }
}
