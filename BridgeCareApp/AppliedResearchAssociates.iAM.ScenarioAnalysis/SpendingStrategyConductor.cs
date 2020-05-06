using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal class SpendingStrategyConductor
    {
        public virtual bool SpendingIsAllowed => true;

        public static SpendingStrategyConductor GetInstance(SpendingStrategy spendingStrategy)
        {
            switch (spendingStrategy)
            {
            case SpendingStrategy.NoSpending:
                return NoSpending.Instance;

            case SpendingStrategy.UnlimitedSpending:
                return Default;

            case SpendingStrategy.UntilTargetAndDeficientConditionGoalsMet:
                return UntilTargetAndDeficientConditionGoalsMet.Instance;

            case SpendingStrategy.UntilTargetConditionGoalsMet:
                return UntilTargetConditionGoalsMet.Instance;

            case SpendingStrategy.UntilDeficientConditionGoalsMet:
                return UntilDeficientConditionGoalsMet.Instance;

            case SpendingStrategy.AsBudgetPermits:
                return AsBudgetPermits.Instance;

            default:
                throw new ArgumentException("Invalid spending strategy.", nameof(spendingStrategy));
            }
        }

        public virtual bool BudgetIsSufficient() => true;

        public virtual bool GoalsAreMet(IEnumerable<ConditionActual> targetConditionActuals, IEnumerable<ConditionActual> deficientConditionActuals) => false;

        private SpendingStrategyConductor()
        {
        }

        private static SpendingStrategyConductor Default { get; } = new SpendingStrategyConductor();

        private static bool GoalsAreMet(IEnumerable<ConditionActual> conditionActuals) => conditionActuals.All(actual => actual.GoalIsMet);

        private sealed class AsBudgetPermits : SpendingStrategyConductor
        {
            public static AsBudgetPermits Instance { get; } = new AsBudgetPermits();

            public override bool BudgetIsSufficient() => throw new NotImplementedException();
        }

        private sealed class NoSpending : SpendingStrategyConductor
        {
            public static NoSpending Instance { get; } = new NoSpending();

            public override bool SpendingIsAllowed => false;
        }

        private sealed class UntilDeficientConditionGoalsMet : SpendingStrategyConductor
        {
            public static UntilDeficientConditionGoalsMet Instance { get; } = new UntilDeficientConditionGoalsMet();

            public override bool GoalsAreMet(IEnumerable<ConditionActual> targetConditionActuals, IEnumerable<ConditionActual> deficientConditionActuals) => GoalsAreMet(deficientConditionActuals);
        }

        private sealed class UntilTargetAndDeficientConditionGoalsMet : SpendingStrategyConductor
        {
            public static UntilTargetAndDeficientConditionGoalsMet Instance { get; } = new UntilTargetAndDeficientConditionGoalsMet();

            public override bool GoalsAreMet(IEnumerable<ConditionActual> targetConditionActuals, IEnumerable<ConditionActual> deficientConditionActuals) => GoalsAreMet(targetConditionActuals) && GoalsAreMet(deficientConditionActuals);
        }

        private sealed class UntilTargetConditionGoalsMet : SpendingStrategyConductor
        {
            public static UntilTargetConditionGoalsMet Instance { get; } = new UntilTargetConditionGoalsMet();

            public override bool GoalsAreMet(IEnumerable<ConditionActual> targetConditionActuals, IEnumerable<ConditionActual> deficientConditionActuals) => GoalsAreMet(targetConditionActuals);
        }
    }
}
