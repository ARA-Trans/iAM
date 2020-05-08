using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal class SpendingStrategyBehaviorProvider
    {
        public virtual AllowedSpending AllowedSpending => AllowedSpending.Unlimited;

        public static SpendingStrategyBehaviorProvider GetInstance(SpendingStrategy spendingStrategy)
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

        public virtual bool GoalsAreMet(IEnumerable<ConditionActual> targetConditionActuals, IEnumerable<ConditionActual> deficientConditionActuals) => false;

        private SpendingStrategyBehaviorProvider()
        {
        }

        private static SpendingStrategyBehaviorProvider Default { get; } = new SpendingStrategyBehaviorProvider();

        private static bool GoalsAreMet(IEnumerable<ConditionActual> conditionActuals) => conditionActuals.All(actual => actual.GoalIsMet);

        private sealed class AsBudgetPermits : SpendingStrategyBehaviorProvider
        {
            public static AsBudgetPermits Instance { get; } = new AsBudgetPermits();

            public override AllowedSpending AllowedSpending => AllowedSpending.Budgeted;
        }

        private sealed class NoSpending : SpendingStrategyBehaviorProvider
        {
            public static NoSpending Instance { get; } = new NoSpending();

            public override AllowedSpending AllowedSpending => AllowedSpending.None;
        }

        private sealed class UntilDeficientConditionGoalsMet : SpendingStrategyBehaviorProvider
        {
            public static UntilDeficientConditionGoalsMet Instance { get; } = new UntilDeficientConditionGoalsMet();

            public override bool GoalsAreMet(IEnumerable<ConditionActual> targetConditionActuals, IEnumerable<ConditionActual> deficientConditionActuals) => GoalsAreMet(deficientConditionActuals);
        }

        private sealed class UntilTargetAndDeficientConditionGoalsMet : SpendingStrategyBehaviorProvider
        {
            public static UntilTargetAndDeficientConditionGoalsMet Instance { get; } = new UntilTargetAndDeficientConditionGoalsMet();

            public override bool GoalsAreMet(IEnumerable<ConditionActual> targetConditionActuals, IEnumerable<ConditionActual> deficientConditionActuals) => GoalsAreMet(targetConditionActuals) && GoalsAreMet(deficientConditionActuals);
        }

        private sealed class UntilTargetConditionGoalsMet : SpendingStrategyBehaviorProvider
        {
            public static UntilTargetConditionGoalsMet Instance { get; } = new UntilTargetConditionGoalsMet();

            public override bool GoalsAreMet(IEnumerable<ConditionActual> targetConditionActuals, IEnumerable<ConditionActual> deficientConditionActuals) => GoalsAreMet(targetConditionActuals);
        }
    }
}
