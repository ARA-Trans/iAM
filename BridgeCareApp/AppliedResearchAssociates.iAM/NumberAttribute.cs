using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class NumberAttribute : Attribute<double>
    {
        public bool IsDecreasingWithDeterioration { get; }

        public double? Maximum { get; }

        public double? Minimum { get; }

        public override ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = base.ValidationResults;

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
                    else
                    {
                        if (double.IsNaN(minimum))
                        {
                            results.Add(ValidationStatus.Error.Describe("Minimum is not a number."));
                        }

                        if (double.IsNaN(maximum))
                        {
                            results.Add(ValidationStatus.Error.Describe("Maximum is not a number."));
                        }
                    }
                }

                return results;
            }
        }
    }
}
