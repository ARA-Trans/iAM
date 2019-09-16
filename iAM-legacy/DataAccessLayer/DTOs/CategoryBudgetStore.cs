using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataAccessLayer.DTOs
{
    public class CategoryBudgetStore:BudgetStore
    {
        string _percentage;

        public string Percentage
        {
            get { return _percentage; }
            set { _percentage = value; }
        } 


        public CategoryBudgetStore()
        {
        }
        /// <summary>
        /// Create a new category budget store
        /// </summary>
        /// <param name="category">Budget category being spent</param>
        /// <param name="target">Amount targeted for budget category. "n/a" if not available</param>
        /// <param name="spent">Amonunt spent for each budget category. "n/a" if not available</param>
        public CategoryBudgetStore(string category, string target, string spent,string percentage)
        {
            this.Key = category;
            this.Target = target;
            this.Spent = spent;
            this.Percentage = percentage;
        }
       
        /// <summary>
        /// Create new category budget store.
        /// </summary>
        /// <param name="category">Budget category being spent</param>
        /// <param name="target">Amount targeted for each budget.  -1 if not available</param>
        /// <param name="spent">Amount spent for each budget. -1 if not available</param>
        public CategoryBudgetStore(string category, decimal target, decimal spent)
        {
            this.Key = category;

            if (target == decimal.MinusOne) this.Target = "n/a";
            else this.Target = target.ToString();

            if (spent == decimal.MinusOne) this.Spent = "n/a";
            else this.Spent = spent.ToString();
        }

        public override string ToString()
        {
            return "$" + this.Target + "(" + this.Key + ")";
        }
    }
}
