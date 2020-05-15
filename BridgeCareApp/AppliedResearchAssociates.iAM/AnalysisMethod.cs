using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class AnalysisMethod
    {
        public AnalysisMethod(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        public NumberAttribute AgeAttribute { get; set; }

        public NumberAttribute AreaAttribute { get; set; }

        public NumberAttribute Benefit
        {
            get => _Benefit;
            set
            {
                _Benefit = value;

                if (Benefit.IsDecreasingWithDeterioration)
                {
                    _LimitBenefit = LimitDecreasingBenefit;
                }
                else
                {
                    _LimitBenefit = LimitIncreasingBenefit;
                }
            }
        }

        public double BenefitLimit { get; set; }

        public IReadOnlyCollection<BudgetPriority> BudgetPriorities => _BudgetPriorities;

        public List<DeficientConditionGoal> DeficientConditionGoals { get; } = new List<DeficientConditionGoal>();

        public string Description { get; set; }

        public Criterion JurisdictionCriterion { get; } = new Criterion();

        public OptimizationStrategy OptimizationStrategy { get; set; }

        public List<RemainingLifeLimit> RemainingLifeLimits { get; } = new List<RemainingLifeLimit>();

        public bool ShouldApplyMultipleFeasibleCosts { get; set; }

        public SpendingStrategy SpendingStrategy { get; set; }

        public List<TargetConditionGoal> TargetConditionGoals { get; } = new List<TargetConditionGoal>();

        public bool UseExtraFundsAcrossBudgets { get; set; }

        public NumberAttribute Weighting { get; set; }

        public BudgetPriority AddBudgetPriority()
        {
            var budgetPriority = new BudgetPriority();
            budgetPriority.SynchronizeWithBudgets(Simulation.InvestmentPlan.Budgets);
            _BudgetPriorities.Add(budgetPriority);
            return budgetPriority;
        }

        public double LimitBenefit(double benefit) => Math.Max(0, _LimitBenefit(benefit));

        public void RemoveBudgetPriority(BudgetPriority budgetPriority)
        {
            if (!_BudgetPriorities.Remove(budgetPriority))
            {
                throw new ArgumentException("Budget priority is not present.");
            }
        }

        private readonly Simulation Simulation;

        private NumberAttribute _Benefit;

        private List<BudgetPriority> _BudgetPriorities = new List<BudgetPriority>();

        private Func<double, double> _LimitBenefit;

        private double LimitDecreasingBenefit(double benefit) => benefit - BenefitLimit;

        private double LimitIncreasingBenefit(double benefit) => BenefitLimit - benefit;
    }
}
