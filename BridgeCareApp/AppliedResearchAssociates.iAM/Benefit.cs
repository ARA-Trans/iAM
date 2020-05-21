using System;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Benefit : IValidator
    {
        public NumberAttribute Attribute
        {
            get => _Attribute;
            set
            {
                _Attribute = value;

                if (Attribute.IsDecreasingWithDeterioration)
                {
                    _LimitBenefit = LimitDecreasingBenefit;
                }
                else
                {
                    _LimitBenefit = LimitIncreasingBenefit;
                }
            }
        }
        public double LimitBenefit(double benefit) => Math.Max(0, _LimitBenefit(benefit));

        private NumberAttribute _Attribute;

        private Func<double, double> _LimitBenefit;

        public double BenefitLimit { get; set; }

        public ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (Attribute == null)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, this, "Attribute is unset."));
                }

                return results;
            }
        }

        public ICollection<IValidator> Subvalidators => new List<IValidator>();

        private double LimitDecreasingBenefit(double benefit) => benefit - BenefitLimit;

        private double LimitIncreasingBenefit(double benefit) => BenefitLimit - benefit;
    }
}
