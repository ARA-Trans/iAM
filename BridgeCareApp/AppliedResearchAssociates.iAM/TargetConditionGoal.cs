using System;

namespace AppliedResearchAssociates.iAM
{
    public sealed class TargetConditionGoal : ConditionGoal
    {
        public override NumberAttribute Attribute
        {
            get => base.Attribute;
            set
            {
                base.Attribute = value;

                if (Attribute.IsDecreasingWithDeterioration)
                {
                    _IsMet = ActualIsGreaterThanOrEqualToTarget;
                }
                else
                {
                    _IsMet = ActualIsLessThanOrEqualToTarget;
                }
            }
        }

        public double Target { get; }

        public int? Year { get; }

        public override bool IsMet(double actual) => _IsMet(actual);

        private Func<double, bool> _IsMet;

        private bool ActualIsGreaterThanOrEqualToTarget(double actual) => actual >= Target;

        private bool ActualIsLessThanOrEqualToTarget(double actual) => actual <= Target;
    }
}
