using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class InvestmentStrategyYearlyBudgetModel
    {
        public InvestmentStrategyYearlyBudgetModel()
        {
            Budget = new List<InvestmentStrategyBudgetModel>();
        }

        public int Id { get; set; }
        public int Year { get; set; }
        public double? BudgetAmount { get; set; }
        public string BudgetName { get; set; }
        public List<InvestmentStrategyBudgetModel> Budget { get; set; }
    }
}