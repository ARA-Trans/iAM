using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class InvestmentStrategyYearlyBudgetModel
    {
        public int Year { get; set; }
        public List<InvestmentStrategyBudgetModel> Budget { get; set; }
    }
}