using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM
{
    public sealed class InvestmentPlan
    {
        public InvestmentPlan(Simulation simulation) => Simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));

        public List<BudgetCondition> BudgetConditions { get; }

        public IReadOnlyList<Budget> Budgets => _Budgets;

        public List<CashFlowRule> CashFlowRules { get; }

        public double DiscountRatePercentage { get; }

        public int FirstYearOfAnalysisPeriod { get; }

        public double InflationRatePercentage { get; }

        public int NumberOfYearsInAnalysisPeriod
        {
            get => _NumberOfYearsInAnalysisPeriod;
            set
            {
                _NumberOfYearsInAnalysisPeriod = value;

                foreach (var budget in Budgets)
                {
                    budget.NumberOfYears = NumberOfYearsInAnalysisPeriod;
                }
            }
        }

        public IEnumerable<int> YearsOfAnalysis => Enumerable.Range(FirstYearOfAnalysisPeriod, NumberOfYearsInAnalysisPeriod);

        public Budget AddBudget()
        {
            var budget = new Budget();
            budget.NumberOfYears = NumberOfYearsInAnalysisPeriod;
            _Budgets.Add(budget);
            SynchronizeBudgetPriorities();
            return budget;
        }

        public void DecrementBudgetIndex(Budget budget)
        {
            var index = _Budgets.IndexOf(budget);

            if (index < 0)
            {
                throw new ArgumentException("Budget has not been added.", nameof(budget));
            }

            if (index == 0)
            {
                throw new ArgumentException("Budget is already ordered first.", nameof(budget));
            }

            var otherBudget = _Budgets[index - 1];
            _Budgets[index - 1] = budget;
            _Budgets[index] = otherBudget;
        }

        public void IncrementBudgetIndex(Budget budget)
        {
            var index = _Budgets.IndexOf(budget);

            if (index < 0)
            {
                throw new ArgumentException("Budget has not been added.", nameof(budget));
            }

            if (index == _Budgets.Count - 1)
            {
                throw new ArgumentException("Budget is already ordered last.", nameof(budget));
            }

            var otherBudget = _Budgets[index + 1];
            _Budgets[index + 1] = budget;
            _Budgets[index] = otherBudget;
        }

        public void RemoveBudget(Budget budget)
        {
            if (!_Budgets.Remove(budget))
            {
                throw new ArgumentException("Budget is not present.");
            }

            SynchronizeBudgetPriorities();
        }

        private readonly List<Budget> _Budgets = new List<Budget>();

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
