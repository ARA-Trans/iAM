using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class InvestmentPlan : IValidator
    {
        public InvestmentPlan(Simulation simulation)
        {
            Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

            SynchronizeBudgetPriorities();
        }

        public IReadOnlyCollection<BudgetCondition> BudgetConditions => _BudgetConditions;

        public IReadOnlyList<Budget> Budgets => _Budgets;

        public IReadOnlyCollection<CashFlowRule> CashFlowRules => _CashFlowRules;

        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (NumberOfYearsInAnalysisPeriod < 1)
                {
                    results.Add(ValidationStatus.Error, "Number of years in analysis period is less than one.", this, nameof(NumberOfYearsInAnalysisPeriod));
                }

                return results;
            }
        }

        public double DiscountRatePercentage { get; set; }

        public int FirstYearOfAnalysisPeriod { get; set; }

        public double InflationRatePercentage { get; set; }

        public int NumberOfYearsInAnalysisPeriod
        {
            get => _NumberOfYearsInAnalysisPeriod;
            set
            {
                _NumberOfYearsInAnalysisPeriod = value;

                foreach (var budget in Budgets)
                {
                    budget.SetNumberOfYears(NumberOfYearsInAnalysisPeriod);
                }
            }
        }

        public ValidatorBag Subvalidators => new ValidatorBag { BudgetConditions, Budgets, CashFlowRules };

        public IEnumerable<int> YearsOfAnalysis => Enumerable.Range(FirstYearOfAnalysisPeriod, NumberOfYearsInAnalysisPeriod);

        public Budget AddBudget()
        {
            var budget = new Budget();
            budget.SetNumberOfYears(NumberOfYearsInAnalysisPeriod);
            _Budgets.Add(budget);
            SynchronizeBudgetPriorities();
            return budget;
        }

        public BudgetCondition AddBudgetCondition() => _BudgetConditions.GetAdd(new BudgetCondition(Simulation.Network.Explorer));

        public CashFlowRule AddCashFlowRule() => _CashFlowRules.GetAdd(new CashFlowRule(Simulation.Network.Explorer));

        public bool DecrementBudgetIndex(Budget budget)
        {
            var index = _Budgets.IndexOf(budget);
            if (index <= 0)
            {
                return false;
            }

            _Budgets.Swap(index - 1, index);
            return true;
        }

        public bool IncrementBudgetIndex(Budget budget)
        {
            var index = _Budgets.IndexOf(budget);
            if (index < 0 || index == _Budgets.Count - 1)
            {
                return false;
            }

            _Budgets.Swap(index, index + 1);
            return true;
        }

        public bool RemoveBudget(Budget budget)
        {
            if (!_Budgets.Remove(budget))
            {
                return false;
            }

            SynchronizeBudgetPriorities();
            return true;
        }

        public void RemoveBudgetCondition(BudgetCondition budgetCondition) => _BudgetConditions.Remove(budgetCondition);

        public void RemoveCashFlowRule(CashFlowRule cashFlowRule) => _CashFlowRules.Remove(cashFlowRule);

        private readonly List<BudgetCondition> _BudgetConditions = new List<BudgetCondition>();

        private readonly List<Budget> _Budgets = new List<Budget>();

        private readonly List<CashFlowRule> _CashFlowRules = new List<CashFlowRule>();

        private readonly Simulation Simulation;

        private int _NumberOfYearsInAnalysisPeriod;

        private void SynchronizeBudgetPriorities()
        {
            foreach (var budgetPriority in Simulation.AnalysisMethod.BudgetPriorities)
            {
                budgetPriority.SynchronizeWithBudgets(Budgets);
            }
        }
    }
}
