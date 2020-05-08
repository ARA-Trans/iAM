using System;

namespace AppliedResearchAssociates.iAM
{
    public sealed class DeficientConditionGoal : ConditionGoal
    {
        public double AllowedDeficientPercentage { get; }

        public override NumberAttribute Attribute
        {
            get => base.Attribute;
            set
            {
                base.Attribute = value;

                if (Attribute.IsDecreasingWithDeterioration)
                {
                    _LevelIsDeficient = LevelIsLessThanLimit;
                }
                else
                {
                    _LevelIsDeficient = LevelIsGreaterThanLimit;
                }
            }
        }

        public double DeficientLimit { get; }

        public override bool IsMet(double actual) => actual <= AllowedDeficientPercentage;

        public bool LevelIsDeficient(double level) => _LevelIsDeficient(level);

        private Func<double, bool> _LevelIsDeficient;

        private bool LevelIsGreaterThanLimit(double level) => level > DeficientLimit;

        private bool LevelIsLessThanLimit(double level) => level < DeficientLimit;
    }
}
