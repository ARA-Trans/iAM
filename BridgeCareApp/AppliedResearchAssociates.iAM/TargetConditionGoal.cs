using System;

namespace AppliedResearchAssociates.iAM
{
    public sealed class TargetConditionGoal : ConditionGoal
    {
        public TargetConditionGoal(Explorer explorer) : base(explorer)
        {
        }

        public override INumericAttribute Attribute
        {
            get => base.Attribute;
            set
            {
                base.Attribute = value;

                if (Attribute == null)
                {
                    _IsMet = null;
                }
                else if (Attribute.IsDecreasingWithDeterioration)
                {
                    _IsMet = ActualIsGreaterThanOrEqualToTarget;
                }
                else
                {
                    _IsMet = ActualIsLessThanOrEqualToTarget;
                }
            }
        }

        public double Target { get; set; }

        public int? Year { get; set; }

        public override bool IsMet(double actual) => _IsMet(actual);

        private Func<double, bool> _IsMet;

        private bool ActualIsGreaterThanOrEqualToTarget(double actual) => actual >= Target;

        private bool ActualIsLessThanOrEqualToTarget(double actual) => actual <= Target;
    }
}
