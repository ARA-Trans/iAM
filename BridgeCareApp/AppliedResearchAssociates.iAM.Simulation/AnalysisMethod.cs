using System;
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

        public ISpendingStrategy SpendingStrategy { get; }

        public List<TargetConditionGoal> TargetConditionGoals { get; }

        [Obsolete("Deprecated per Gregg's presentation on 2020-03-10.")]
        public object Weighting { get; }
    }
}
