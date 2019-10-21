using System.Collections;
using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class YearlyBudgetAndCost
    {
        public Hashtable BudgetForYear;
        public List<CostDetails> CostDetails;

        public YearlyBudgetAndCost() { }

        public YearlyBudgetAndCost(Hashtable budgetForYear, List<CostDetails> costDetails)
        {
            BudgetForYear = budgetForYear;
            CostDetails = costDetails;
        }
    }
}