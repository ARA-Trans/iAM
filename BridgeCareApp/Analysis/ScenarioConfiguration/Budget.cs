using System.Collections.Generic;
using ARA.iAM.Analysis.Interfaces;

namespace ARA.iAM.Analysis
{
    class Budget: ICriteriaDriven
    {
        private BudgetID id;
        private string name;
        /// <summary>
        /// A mapping from years to the amount of money available
        /// </summary>
        private Dictionary<int, double> yearlyInvestment;
        /// <summary>
        /// Defines the assets for which this budget may be used to pay for treaments
        /// </summary>
        public Criterion Criterion { get; }
    }

    class BudgetID
    {
        // Needs to implement Equals and GetHashCode
    }
}
