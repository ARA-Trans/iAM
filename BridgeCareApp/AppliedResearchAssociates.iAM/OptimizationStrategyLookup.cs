using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class OptimizationStrategyLookup
    {
        [Obsolete]
        public static string ConditionalRslCost => "Conditional RSL/Cost";

        public static string IncrementalBenefitCost => "Incremental Benefit/Cost";

        public static OptimizationStrategyLookup Instance { get; } = new OptimizationStrategyLookup();

        public static string MaximumBenefit => "Maximum Benefit";

        public static string MaximumRemainingLife => "Maximum Remaining Life";

        [Obsolete]
        public static string MultiYearIncrementalBenefitCost => "Multi-year Incremental Benefit/Cost";

        [Obsolete]
        public static string MultiYearMaximumBenefit => "Multi-year Maximum Benefit";

        [Obsolete]
        public static string MultiYearMaximumLife => "Multi-year Maximum Life";

        [Obsolete]
        public static string MultiYearRemainingLifeCost => "Multi-year Remaining Life/Cost";

        public static string RemainingLifeCost => "Remaining Life/Cost";

        public OptimizationStrategy this[string label] => StrategyPerLabel[label?.Trim()];

        private static readonly IReadOnlyDictionary<string, OptimizationStrategy> StrategyPerLabel = new Dictionary<string, OptimizationStrategy>(StringComparer.OrdinalIgnoreCase)
        {
            [IncrementalBenefitCost] = OptimizationStrategy.BenefitToCostRatio,
            [MaximumBenefit] = OptimizationStrategy.Benefit,
            [RemainingLifeCost] = OptimizationStrategy.RemainingLifeToCostRatio,
            [MaximumRemainingLife] = OptimizationStrategy.RemainingLife,
            [MultiYearIncrementalBenefitCost] = OptimizationStrategy.BenefitToCostRatio,
            [MultiYearMaximumBenefit] = OptimizationStrategy.Benefit,
            [MultiYearRemainingLifeCost] = OptimizationStrategy.RemainingLifeToCostRatio,
            [MultiYearMaximumLife] = OptimizationStrategy.RemainingLife,
            [ConditionalRslCost] = OptimizationStrategy.RemainingLifeToCostRatio,
        };

        private OptimizationStrategyLookup()
        {
        }
    }
}
