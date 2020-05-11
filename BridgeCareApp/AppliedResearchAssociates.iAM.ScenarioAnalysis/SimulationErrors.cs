using System;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal static class SimulationErrors
    {
        public static Exception CostOfPassiveTreatmentIsNonZero => new InvalidOperationException("Cost of passive treatment is non-zero.");

        public static Exception CostOfScheduledEventCannotBeCovered => new InvalidOperationException("Cost of scheduled event cannot be covered.");

        public static Exception InvalidOptimizationStrategy => new InvalidOperationException("Invalid optimization strategy.");

        public static Exception InvalidSpendingStrategy => new InvalidOperationException("Invalid spending strategy.");

        public static Exception OutlookShouldNeverConsumeProgress => new InvalidOperationException("Outlook should never consume progress.");

        public static Exception YearIsAlreadyScheduled => new InvalidOperationException("Year is already scheduled.");

        public static Exception RemainingCostIsNegative => new InvalidOperationException("Remaining cost is negative.");

        public static Exception RemainingLifeOptimizationHasNoLimits => new InvalidOperationException("Remaining-life optimization has no limits.");
    }
}
