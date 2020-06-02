using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class AnalysisMethod : IValidator
    {
        public AnalysisMethod(Simulation simulation)
        {
            Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

            JurisdictionCriterion = new Criterion(Simulation.Network.Explorer);
        }

        public Benefit Benefit { get; } = new Benefit();

        public IReadOnlyCollection<BudgetPriority> BudgetPriorities => _BudgetPriorities;

        public IReadOnlyCollection<DeficientConditionGoal> DeficientConditionGoals => _DeficientConditionGoals;

        public string Description { get; set; }

        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

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

        public Criterion JurisdictionCriterion { get; }

        public OptimizationStrategy OptimizationStrategy { get; set; }

        public IReadOnlyCollection<RemainingLifeLimit> RemainingLifeLimits => _RemainingLifeLimits;

        public bool ShouldApplyMultipleFeasibleCosts { get; set; }

        public bool ShouldUseExtraFundsAcrossBudgets { get; set; }

        public SpendingStrategy SpendingStrategy { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag { Benefit, BudgetPriorities, DeficientConditionGoals, JurisdictionCriterion, RemainingLifeLimits, TargetConditionGoals };

        public IReadOnlyCollection<TargetConditionGoal> TargetConditionGoals => _TargetConditionGoals;

        public INumericAttribute Weighting { get; set; }

        public BudgetPriority AddBudgetPriority()
        {
            var budgetPriority = new BudgetPriority(Simulation.Network.Explorer);
            budgetPriority.SynchronizeWithBudgets(Simulation.InvestmentPlan.Budgets);
            _BudgetPriorities.Add(budgetPriority);
            return budgetPriority;
        }

        public DeficientConditionGoal AddDeficientConditionGoal() => _DeficientConditionGoals.GetAdd(new DeficientConditionGoal(Simulation.Network.Explorer));

        public RemainingLifeLimit AddRemainingLifeLimit() => _RemainingLifeLimits.GetAdd(new RemainingLifeLimit(Simulation.Network.Explorer));

        public TargetConditionGoal AddTargetConditionGoal() => _TargetConditionGoals.GetAdd(new TargetConditionGoal(Simulation.Network.Explorer));

        public void Remove(TargetConditionGoal targetConditionGoal) => _TargetConditionGoals.Remove(targetConditionGoal);

        public void Remove(DeficientConditionGoal deficientConditionGoal) => _DeficientConditionGoals.Remove(deficientConditionGoal);

        public void Remove(BudgetPriority budgetPriority) => _BudgetPriorities.Remove(budgetPriority);

        public void Remove(RemainingLifeLimit remainingLifeLimit) => _RemainingLifeLimits.Remove(remainingLifeLimit);

        private readonly List<BudgetPriority> _BudgetPriorities = new List<BudgetPriority>();

        private readonly List<DeficientConditionGoal> _DeficientConditionGoals = new List<DeficientConditionGoal>();

        private readonly List<RemainingLifeLimit> _RemainingLifeLimits = new List<RemainingLifeLimit>();

        private readonly List<TargetConditionGoal> _TargetConditionGoals = new List<TargetConditionGoal>();

        private readonly Simulation Simulation;

        private static IReadOnlyCollection<string> GetNames(IEnumerable<ConditionGoal> conditionGoals) => conditionGoals.Select(conditionGoal => conditionGoal.Name).Where(name => !string.IsNullOrWhiteSpace(name)).ToArray();
    }
}
