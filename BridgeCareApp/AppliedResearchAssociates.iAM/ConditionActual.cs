using System;

namespace AppliedResearchAssociates.iAM
{
    internal sealed class ConditionActual
    {
        public ConditionActual(ConditionGoal goal, double value)
        {
            Goal = goal ?? throw new ArgumentNullException(nameof(goal));
            Value = value;
        }

        public ConditionGoal Goal { get; }

        public bool GoalIsMet => Goal.IsMet(Value);

        public double Value { get; }
    }
}
