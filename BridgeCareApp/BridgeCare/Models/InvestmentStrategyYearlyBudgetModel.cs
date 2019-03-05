using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models
{
    public class InvestmentStrategyYearlyBudgetModel
    {
        public int Year { get; set; }
        public List<InvestmentStrategyBudgetModel> Budget { get; set; }
    }
}