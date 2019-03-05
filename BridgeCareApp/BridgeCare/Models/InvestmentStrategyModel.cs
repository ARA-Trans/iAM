using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class InvestmentStrategyModel
    {
        public InvestmentStrategyModel()
        {
            YearlyBudget = new List<InvestmentStrategyYearlyBudgetModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int FirstYear { get; set; }
        public int NumberYears { get; set; }
        public double InflationRate { get; set; }
        public double DiscountRate { get; set; }
        public string BudgetOrder { get; set; }

        public List<InvestmentStrategyYearlyBudgetModel> YearlyBudget { get; set; }
    }
}
