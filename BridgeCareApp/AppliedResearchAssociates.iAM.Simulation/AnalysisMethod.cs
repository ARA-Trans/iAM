using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class AnalysisMethod
    {
        // ???
        public object Benefit { get; }

        // ???
        public int BenefitLimit { get; }

        public List<BudgetPriority> BudgetPriorities { get; } = new List<BudgetPriority>();

        public List<DeficientConditionGoal> DeficientConditionGoals { get; } = new List<DeficientConditionGoal>();

        public string Description { get; }

        public Criterion JurisdictionCriterion { get; }

        public IOptimizationStrategy OptimizationStrategy { get; }

        public ISpendingStrategy SpendingStrategy { get; }

        public List<TargetConditionGoal> TargetConditionGoals { get; } = new List<TargetConditionGoal>();

        // ???
        public object Weighting { get; }
    }
}
