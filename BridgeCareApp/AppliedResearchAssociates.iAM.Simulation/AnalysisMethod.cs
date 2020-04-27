using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class AnalysisMethod
    {
        public NumberAttribute AgeAttribute { get; }

        public NumberAttribute AreaAttribute { get; }

        public NumberAttribute Benefit
        {
            get => _Benefit;
            set
            {
                switch (value.Deterioration)
                {
                case Deterioration.Decreasing:
                    _LimitBenefit = LimitDecreasingBenefit;
                    break;

                case Deterioration.Increasing:
                    _LimitBenefit = LimitIncreasingBenefit;
                    break;

                default:
                    throw new ArgumentException("Invalid deterioration.", nameof(value));
                }

                _Benefit = value;
            }
        }

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

        public UseOfExtraFunds UseOfExtraFunds { get; }

        public NumberAttribute Weighting { get; }

        public double LimitBenefit(double benefit) => Math.Max(0, _LimitBenefit(benefit));

        private NumberAttribute _Benefit;

        private Func<double, double> _LimitBenefit;

        private double LimitDecreasingBenefit(double benefit) => benefit - BenefitLimit;

        private double LimitIncreasingBenefit(double benefit) => BenefitLimit - benefit;
    }
}
