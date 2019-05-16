using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class InvestmentLibraryYearlyBudgetModel
    {
        public InvestmentLibraryYearlyBudgetModel()
        {
            Budget = new List<InvestmentLibraryBudgetModel>();
        }

        public int Id { get; set; }
        public int Year { get; set; }
        public double? BudgetAmount { get; set; }
        public string BudgetName { get; set; }
        public List<InvestmentLibraryBudgetModel> Budget { get; set; }
    }
}