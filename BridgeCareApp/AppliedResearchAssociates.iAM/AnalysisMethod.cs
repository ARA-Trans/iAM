using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class AnalysisMethod : IValidator
    {
        public AnalysisMethod(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        public Box<NumberAttribute> AgeAttribute { get; } = new Box<NumberAttribute>();

        public Box<NumberAttribute> AreaAttribute { get; } = new Box<NumberAttribute>();

        public Benefit Benefit { get; } = new Benefit();

        public IReadOnlyCollection<BudgetPriority> BudgetPriorities => _BudgetPriorities;

        public ICollection<DeficientConditionGoal> DeficientConditionGoals { get; } = new ListWithoutNulls<DeficientConditionGoal>();

        public string Description { get; set; }

        public ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (AgeAttribute.Value == null)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, AgeAttribute, "Age attribute is unset."));
                }

                if (AreaAttribute.Value == null)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, AreaAttribute, "Area attribute is unset."));
                }

                if (!OptimizationStrategy.Value.IsDefined())
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, OptimizationStrategy, "Invalid optimization strategy."));
                }

                if (!SpendingStrategy.Value.IsDefined())
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, SpendingStrategy, "Invalid spending strategy."));
                }

                if (BudgetPriorities.Select(priority => (priority.PriorityLevel, priority.Year)).Distinct().Count() < BudgetPriorities.Count)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, BudgetPriorities, "At least one priority level-year is represented more than once."));
                }

                var deficientConditionGoalNames = GetNames(DeficientConditionGoals);
                if (deficientConditionGoalNames.Distinct().Count() < deficientConditionGoalNames.Count)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, DeficientConditionGoals, "Multiple deficient condition goals have the same name."));
                }

                var targetConditionGoalNames = GetNames(TargetConditionGoals);
                if (targetConditionGoalNames.Distinct().Count() < targetConditionGoalNames.Count)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, TargetConditionGoals, "Multiple target condition goals have the same name."));
                }

                var remainingLifeLimitsWithBlankCriterion = RemainingLifeLimits.Where(limit => limit.Criterion.ExpressionIsBlank).ToArray();
                if (remainingLifeLimitsWithBlankCriterion.Select(limit => limit.Attribute).Distinct().Count() < remainingLifeLimitsWithBlankCriterion.Length)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Warning, RemainingLifeLimits, "At least one attribute has more than one remaining life limit with a blank criterion."));
                }

                return results;
            }
        }

        public Criterion JurisdictionCriterion { get; } = new Criterion();

        public Box<OptimizationStrategy> OptimizationStrategy { get; } = new Box<OptimizationStrategy>();

        public ICollection<RemainingLifeLimit> RemainingLifeLimits { get; } = new ListWithoutNulls<RemainingLifeLimit>();

        public bool ShouldApplyMultipleFeasibleCosts { get; set; }

        public Box<SpendingStrategy> SpendingStrategy { get; } = new Box<SpendingStrategy>();

        public ICollection<IValidator> Subvalidators
        {
            get
            {
                var validators = new List<IValidator>();
                validators.Add(Benefit);
                validators.AddMany(BudgetPriorities);
                validators.AddMany(DeficientConditionGoals);
                validators.Add(JurisdictionCriterion);
                validators.AddMany(RemainingLifeLimits);
                validators.AddMany(TargetConditionGoals);
                return validators;
            }
        }

        public ICollection<TargetConditionGoal> TargetConditionGoals { get; } = new ListWithoutNulls<TargetConditionGoal>();

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

        private static ICollection<string> GetNames(IEnumerable<ConditionGoal> conditionGoals) => conditionGoals.Select(conditionGoal => conditionGoal.Name.Value).Where(name => !string.IsNullOrWhiteSpace(name)).ToArray();
    }
}
