using System;

namespace AppliedResearchAssociates.iAM.SimulationOutput
{
    public sealed class BudgetDetail
    {
        public BudgetDetail(string budgetName)
        {
            if (string.IsNullOrWhiteSpace(budgetName))
            {
                throw new ArgumentException("Budget name is blank.", nameof(budgetName));
            }

            BudgetName = budgetName;
        }

        public string BudgetName { get; }

        public BudgetReason BudgetReason { get; set; }

        public decimal CoveredCost { get; set; }
    }
}
