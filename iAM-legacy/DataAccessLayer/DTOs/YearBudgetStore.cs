using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class YearBudgetStore:BudgetStore
    {
        public YearBudgetStore()
        {
        }
        /// <summary>
        /// Create a new year budget store
        /// </summary>
        /// <param name="category">Budget year being spent</param>
        /// <param name="target">Amount targeted for budget category. "n/a" if not available</param>
        /// <param name="spent">Amonunt spent for each budget category. "n/a" if not available</param>
        public YearBudgetStore(string year, string target, string spent)
        {
            this.Key = year;
            this.Target = target;
            this.Spent = spent;
        }
       
        /// <summary>
        /// Create new year budget store.
        /// </summary>
        /// <param name="category">Budget year being spent</param>
        /// <param name="target">Amount targeted for each budget.  Nan if not available</param>
        /// <param name="spent">Amount spent for each budget. Nan if not available</param>
        public YearBudgetStore(string year, double target, double spent)
        {
            this.Key = year;

            if (target == double.NaN) this.Target = "n/a";
            else this.Target = target.ToString("f2");

            if (spent == double.NaN) this.Spent = "n/a";
            else this.Spent = spent.ToString("f2");
        }


        public override string ToString()
        {
            return "$" + this.Target + "(" + this.Key + ")";
        }
    }
}
