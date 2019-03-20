using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class InvestmentStrategyYearlyBudgetModel
    {
        public InvestmentStrategyYearlyBudgetModel()
        {
            Budget = new List<InvestmentStrategyBudgetModel>();
        }

        public int Year { get; set; }
        public List<InvestmentStrategyBudgetModel> Budget { get; set; }
    }
}