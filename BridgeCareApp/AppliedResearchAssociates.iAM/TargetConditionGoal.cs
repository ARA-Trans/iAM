using System;

namespace AppliedResearchAssociates.iAM
{
    public sealed class TargetConditionGoal : ConditionGoal
    {
        public TargetConditionGoal() => Attribute = new Box<NumberAttribute>(OnSetAttribute);

        public override Box<NumberAttribute> Attribute { get; }

        public double Target { get; set; }

        public int? Year { get; set; }

        public override bool IsMet(double actual) => _IsMet(actual);

        private Func<double, bool> _IsMet;

        private bool ActualIsGreaterThanOrEqualToTarget(double actual) => actual >= Target;

        private bool ActualIsLessThanOrEqualToTarget(double actual) => actual <= Target;

        private void OnSetAttribute()
        {
            if (Attribute.Value == null)
            {
                _IsMet = null;
            }
            else if (Attribute.Value.IsDecreasingWithDeterioration)
            {
                _IsMet = ActualIsGreaterThanOrEqualToTarget;
            }
            else
            {
                _IsMet = ActualIsLessThanOrEqualToTarget;
            }
        }
    }
}
