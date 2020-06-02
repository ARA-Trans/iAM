using System;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Benefit : IValidator
    {
        public INumericAttribute Attribute
        {
            get => _Attribute;
            set
            {
                _Attribute = value;

                if (Attribute == null)
                {
                    _LimitValue = null;
                }
                else if (Attribute.IsDecreasingWithDeterioration)
                {
                    _LimitValue = LimitDecreasingValue;
                }
                else
                {
                    _LimitValue = LimitIncreasingValue;
                }
            }
        }

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

        public double Limit { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag();

        public double LimitValue(double benefit) => Math.Max(0, _LimitValue(benefit));

        private INumericAttribute _Attribute;

        private Func<double, double> _LimitValue;

        private double LimitDecreasingValue(double value) => value - Limit;

        private double LimitIncreasingValue(double value) => Limit - value;
    }
}
