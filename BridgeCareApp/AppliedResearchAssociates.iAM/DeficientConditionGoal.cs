using System;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class DeficientConditionGoal : ConditionGoal
    {
        public DeficientConditionGoal(Explorer explorer) : base(explorer)
        {
        }

        public double AllowedDeficientPercentage { get; set; }

        public override INumericAttribute Attribute
        {
            get => base.Attribute;
            set
            {
                base.Attribute = value;

                if (Attribute == null)
                {
                    _LevelIsDeficient = null;
                }
                else if (Attribute.IsDecreasingWithDeterioration)
                {
                    _LevelIsDeficient = LevelIsLessThanLimit;
                }
                else
                {
                    _LevelIsDeficient = LevelIsGreaterThanLimit;
                }
            }
        }

        public double DeficientLimit { get; set; }

        public override ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = base.DirectValidationResults;

                if (AllowedDeficientPercentage < 0)
                {
                    results.Add(ValidationStatus.Error, "Allowed deficient percentage is less than zero.", this, nameof(AllowedDeficientPercentage));
                }
                else if (AllowedDeficientPercentage == 0)
                {
                    results.Add(ValidationStatus.Warning, "Allowed deficient percentage is zero.", this, nameof(AllowedDeficientPercentage));
                }
                else if (AllowedDeficientPercentage == 100)
                {
                    results.Add(ValidationStatus.Warning, "Allowed deficient percentage is 100.", this, nameof(AllowedDeficientPercentage));
                }
                else if (AllowedDeficientPercentage > 100)
                {
                    results.Add(ValidationStatus.Error, "Allowed deficient percentage is greater than 100.", this, nameof(AllowedDeficientPercentage));
                }

                return results;
            }
        }

        public override bool IsMet(double actual) => actual <= AllowedDeficientPercentage;

        public bool LevelIsDeficient(double level) => _LevelIsDeficient(level);

        private Func<double, bool> _LevelIsDeficient;

        private bool LevelIsGreaterThanLimit(double level) => level > DeficientLimit;

        private bool LevelIsLessThanLimit(double level) => level < DeficientLimit;
    }
}
