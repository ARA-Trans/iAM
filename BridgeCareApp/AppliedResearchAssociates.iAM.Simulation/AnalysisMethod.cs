using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class AnalysisMethod
    {
        public NumberAttribute Benefit { get; }

        public double BenefitLimit { get; }

        public List<BudgetPriority> BudgetPriorities { get; }

        public List<DeficientConditionGoal> DeficientConditionGoals { get; }

        public string Description { get; }

        public Criterion JurisdictionCriterion { get; }

        public IOptimizationStrategy OptimizationStrategy { get; }

        public List<RemainingLifeLimit> RemainingLifeLimits { get; }

        public bool ShouldApplyMultipleFeasibleCosts { get; }

        public ISpendingStrategy SpendingStrategy { get; }

        public List<TargetConditionGoal> TargetConditionGoals { get; }

        public NumberAttribute Weighting { get; }
    }
}
