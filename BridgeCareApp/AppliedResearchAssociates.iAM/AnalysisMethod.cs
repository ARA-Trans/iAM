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

        public Benefit Benefit { get; } = new Benefit();

        public IReadOnlyCollection<BudgetPriority> BudgetPriorities => _BudgetPriorities;

        public ICollection<DeficientConditionGoal> DeficientConditionGoals { get; } = new SetWithoutNulls<DeficientConditionGoal>();

        public string Description { get; set; }

        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (AgeAttribute == null)
                {
                    results.Add(ValidationStatus.Error, "Age attribute is unset.", this, nameof(AgeAttribute));
                }

                if (!OptimizationStrategy.IsDefined())
                {
                    results.Add(ValidationStatus.Error, MessageStrings.InvalidOptimizationStrategy, this, nameof(OptimizationStrategy));
                }

                if (!SpendingStrategy.IsDefined())
                {
                    results.Add(ValidationStatus.Error, MessageStrings.InvalidSpendingStrategy, this, nameof(SpendingStrategy));
                }

                if (BudgetPriorities.Select(priority => (priority.PriorityLevel, priority.Year)).Distinct().Count() < BudgetPriorities.Count)
                {
                    results.Add(ValidationStatus.Error, "At least one priority level-year is represented more than once.", this, nameof(BudgetPriorities));
                }

                var deficientConditionGoalNames = GetNames(DeficientConditionGoals);
                if (deficientConditionGoalNames.Distinct().Count() < deficientConditionGoalNames.Count)
                {
                    results.Add(ValidationStatus.Error, "Multiple deficient condition goals have the same name.", this, nameof(DeficientConditionGoals));
                }

                var targetConditionGoalNames = GetNames(TargetConditionGoals);
                if (targetConditionGoalNames.Distinct().Count() < targetConditionGoalNames.Count)
                {
                    results.Add(ValidationStatus.Error, "Multiple target condition goals have the same name.", this, nameof(TargetConditionGoals));
                }

                var remainingLifeLimitsWithBlankCriterion = RemainingLifeLimits.Where(limit => limit.Criterion.ExpressionIsBlank).ToArray();
                if (remainingLifeLimitsWithBlankCriterion.Select(limit => limit.Attribute).Distinct().Count() < remainingLifeLimitsWithBlankCriterion.Length)
                {
                    results.Add(ValidationStatus.Warning, "At least one attribute has more than one remaining life limit with a blank criterion.", this, nameof(RemainingLifeLimits));
                }

                return results;
            }
        }

        public Criterion JurisdictionCriterion { get; } = new Criterion();

        public OptimizationStrategy OptimizationStrategy { get; set; }

        public ICollection<RemainingLifeLimit> RemainingLifeLimits { get; } = new SetWithoutNulls<RemainingLifeLimit>();

        public bool ShouldApplyMultipleFeasibleCosts { get; set; }

        public SpendingStrategy SpendingStrategy { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag { Benefit, BudgetPriorities, DeficientConditionGoals, JurisdictionCriterion, RemainingLifeLimits, TargetConditionGoals };

        public ICollection<TargetConditionGoal> TargetConditionGoals { get; } = new SetWithoutNulls<TargetConditionGoal>();

        public bool UseExtraFundsAcrossBudgets { get; set; }

        public NumberAttribute Weighting { get; set; }

        public BudgetPriority AddBudgetPriority()
        {
            var budgetPriority = new BudgetPriority();
            budgetPriority.SynchronizeWithBudgets(Simulation.InvestmentPlan.Budgets);
            _BudgetPriorities.Add(budgetPriority);
            return budgetPriority;
        }

        public bool RemoveBudgetPriority(BudgetPriority budgetPriority) => _BudgetPriorities.Remove(budgetPriority);

        private readonly List<BudgetPriority> _BudgetPriorities = new List<BudgetPriority>();

        private readonly Simulation Simulation;

        private static IReadOnlyCollection<string> GetNames(IEnumerable<ConditionGoal> conditionGoals) => conditionGoals.Select(conditionGoal => conditionGoal.Name).Where(name => !string.IsNullOrWhiteSpace(name)).ToArray();
    }
}
