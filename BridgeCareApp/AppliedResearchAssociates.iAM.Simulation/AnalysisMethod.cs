using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class AnalysisMethod
    {
        // ???
        public object Benefit { get; }

        // ???
        public int BenefitLimit { get; }

        public List<BudgetPriority> BudgetPriorities { get; }

        public List<DeficientConditionGoal> DeficientConditionGoals { get; }

        public string Description { get; }

        public Criterion JurisdictionCriterion { get; }

        public IOptimizationStrategy OptimizationStrategy { get; }

        public ISpendingStrategy SpendingStrategy { get; }

        public List<TargetConditionGoal> TargetConditionGoals { get; }

        // ???
        public object Weighting { get; }
    }
}
