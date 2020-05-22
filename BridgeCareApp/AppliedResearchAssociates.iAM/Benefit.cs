using System;
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

        public double BenefitLimit { get; set; }

        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (Attribute == null)
                {
                    results.Add(ValidationStatus.Error, "Attribute is unset.", this, nameof(Attribute));
                }

                return results;
            }
        }

        public ValidatorBag Subvalidators => new ValidatorBag();

        public double LimitBenefit(double benefit) => Math.Max(0, _LimitBenefit(benefit));

        private NumberAttribute _Attribute;

        private Func<double, double> _LimitBenefit;

        private double LimitDecreasingBenefit(double benefit) => benefit - BenefitLimit;

        private double LimitIncreasingBenefit(double benefit) => BenefitLimit - benefit;
    }
}
