using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class AnalysisMethod : IValidator
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

        public ICollection<DeficientConditionGoal> DeficientConditionGoals { get; } = new CollectionWithoutNulls<DeficientConditionGoal>();

        public string Description { get; set; }

        public Criterion JurisdictionCriterion { get; } = new Criterion();

        public OptimizationStrategy OptimizationStrategy { get; set; }

        public ICollection<RemainingLifeLimit> RemainingLifeLimits { get; } = new CollectionWithoutNulls<RemainingLifeLimit>();

        public bool ShouldApplyMultipleFeasibleCosts { get; set; }

        public SpendingStrategy SpendingStrategy { get; set; }

        public ICollection<TargetConditionGoal> TargetConditionGoals { get; } = new CollectionWithoutNulls<TargetConditionGoal>();

        public bool UseExtraFundsAcrossBudgets { get; set; }

        public ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (AgeAttribute == null)
                {
                    results.Add(ValidationStatus.Error.Describe("Age attribute is unset."));
                }

                if (AreaAttribute == null)
                {
                    results.Add(ValidationStatus.Error.Describe("Area attribute is unset."));
                }

                if (Benefit == null)
                {
                    results.Add(ValidationStatus.Error.Describe("Benefit is unset."));
                }

                if (BudgetPriorities.Select(priority => (priority.PriorityLevel, priority.Year)).Distinct().Count() < BudgetPriorities.Count)
                {
                    results.Add(ValidationStatus.Error.Describe("At least one priority level-year is represented more than once."));
                }

                var deficientConditionGoalNames = GetNames(DeficientConditionGoals);
                if (deficientConditionGoalNames.Distinct().Count() < deficientConditionGoalNames.Count)
                {
                    results.Add(ValidationStatus.Error.Describe("Multiple deficient condition goals have the same name."));
                }

                var targetConditionGoalNames = GetNames(TargetConditionGoals);
                if (targetConditionGoalNames.Distinct().Count() < targetConditionGoalNames.Count)
                {
                    results.Add(ValidationStatus.Error.Describe("Multiple target condition goals have the same name."));
                }

                if (!OptimizationStrategy.IsDefined())
                {
                    results.Add(ValidationStatus.Error.Describe("Invalid optimization strategy."));
                }

                if (!SpendingStrategy.IsDefined())
                {
                    results.Add(ValidationStatus.Error.Describe("Invalid spending strategy."));
                }

                var remainingLifeLimitsWithBlankCriterion = RemainingLifeLimits.Where(limit => limit.Criterion.ExpressionIsBlank).ToArray();
                if (remainingLifeLimitsWithBlankCriterion.Select(limit => limit.Attribute).Distinct().Count() < remainingLifeLimitsWithBlankCriterion.Length)
                {
                    results.Add(ValidationStatus.Warning.Describe("At least one attribute has more than one remaining life limit with a blank criterion."));
                }

                return results;
            }
        }

        public NumberAttribute Weighting { get; set; }

        public BudgetPriority AddBudgetPriority()
        {
            var budgetPriority = new BudgetPriority();
            budgetPriority.SynchronizeWithBudgets(Simulation.InvestmentPlan.Budgets);
            _BudgetPriorities.Add(budgetPriority);
            return budgetPriority;
        }

        public double LimitBenefit(double benefit) => Math.Max(0, _LimitBenefit(benefit));

        public bool RemoveBudgetPriority(BudgetPriority budgetPriority) => _BudgetPriorities.Remove(budgetPriority);

        private readonly List<BudgetPriority> _BudgetPriorities = new List<BudgetPriority>();

        private readonly Simulation Simulation;

        private NumberAttribute _Benefit;

        private Func<double, double> _LimitBenefit;

        private static ICollection<string> GetNames(IEnumerable<ConditionGoal> conditionGoals) => conditionGoals.Select(conditionGoal => conditionGoal.Name).Where(name => !string.IsNullOrWhiteSpace(name)).ToArray();

        private double LimitDecreasingBenefit(double benefit) => benefit - BenefitLimit;

        private double LimitIncreasingBenefit(double benefit) => BenefitLimit - benefit;
    }
}
