using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class SpendingStrategyLookup
    {
        public static string AsBudgetPermits => "As Budget Permits";

        public static SpendingStrategyLookup Instance { get; } = new SpendingStrategyLookup();

        public static string NoSpending => "No Spending";

        public static string TargetsDeficientMet => "Targets/Deficient Met";

        public static string Unlimited => "Unlimited";

        public static string UntilDeficientMet => "Until Deficient Met";

        public static string UntilTargetsMet => "Until Targets Met";

        public SpendingStrategy this[string label] => StrategyPerLabel[label?.Trim()];

        private static readonly IReadOnlyDictionary<string, SpendingStrategy> StrategyPerLabel = new Dictionary<string, SpendingStrategy>(StringComparer.OrdinalIgnoreCase)
        {
            [NoSpending] = SpendingStrategy.NoSpending,
            [AsBudgetPermits] = SpendingStrategy.AsBudgetPermits,
            [UntilTargetsMet] = SpendingStrategy.UntilTargetConditionGoalsMet,
            [UntilDeficientMet] = SpendingStrategy.UntilDeficientConditionGoalsMet,
            [TargetsDeficientMet] = SpendingStrategy.UntilTargetAndDeficientConditionGoalsMet,
            [Unlimited] = SpendingStrategy.UnlimitedSpending,
        };

        private SpendingStrategyLookup()
        {
        }
    }
}
