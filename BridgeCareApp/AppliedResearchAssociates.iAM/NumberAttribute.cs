using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class NumberAttribute : Attribute<double>
    {
        public NumberAttribute(Explorer explorer) : base(explorer)
        {
        }

        public override ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = base.DirectValidationResults;

                if (Minimum.HasValue && double.IsNaN(Minimum.Value))
                {
                    results.Add(ValidationStatus.Error, "Minimum is not a number.", this, nameof(Minimum));
                }

                if (Maximum.HasValue && double.IsNaN(Maximum.Value))
                {
                    results.Add(ValidationStatus.Error, "Maximum is not a number.", this, nameof(Minimum));
                }

                if (Minimum.Value is double minimum && Maximum.Value is double maximum)
                {
                    if (minimum == maximum)
                    {
                        results.Add(ValidationStatus.Warning, "Minimum is equal to maximum.", this);
                    }
                    else if (minimum > maximum)
                    {
                        results.Add(ValidationStatus.Error, "Minimum is greater than maximum.", this);
                    }
                }

                return results;
            }
        }

        public bool IsDecreasingWithDeterioration { get; set; }

        public double? Maximum { get; set; }

        public double? Minimum { get; set; }
    }
}
