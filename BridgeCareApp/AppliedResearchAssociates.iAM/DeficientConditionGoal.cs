using System;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class DeficientConditionGoal : ConditionGoal
    {
        public DeficientConditionGoal() => Attribute = new Box<NumberAttribute>(OnSetAttribute);

        public Box<double> AllowedDeficientPercentage { get; } = new Box<double>();

        public override Box<NumberAttribute> Attribute { get; }

        public double DeficientLimit { get; set; }

        public override ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = base.DirectValidationResults;

                if (AllowedDeficientPercentage < 0)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, AllowedDeficientPercentage, "Allowed deficient percentage is less than zero."));
                }
                else if (AllowedDeficientPercentage == 0)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Warning, AllowedDeficientPercentage, "Allowed deficient percentage is zero."));
                }
                else if (AllowedDeficientPercentage == 100)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Warning, AllowedDeficientPercentage, "Allowed deficient percentage is 100."));
                }
                else if (AllowedDeficientPercentage > 100)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, AllowedDeficientPercentage, "Allowed deficient percentage is greater than 100."));
                }

                return results;
            }
        }

        public override bool IsMet(double actual) => actual <= AllowedDeficientPercentage;

        public bool LevelIsDeficient(double level) => _LevelIsDeficient(level);

        private Func<double, bool> _LevelIsDeficient;

        private bool LevelIsGreaterThanLimit(double level) => level > DeficientLimit;

        private bool LevelIsLessThanLimit(double level) => level < DeficientLimit;

        private void OnSetAttribute()
        {
            if (Attribute.Value == null)
            {
                _LevelIsDeficient = null;
            }
            else if (Attribute.Value.IsDecreasingWithDeterioration)
            {
                _LevelIsDeficient = LevelIsLessThanLimit;
            }
            else
            {
                _LevelIsDeficient = LevelIsGreaterThanLimit;
            }
        }
    }
}
