using System.Collections.Generic;
using AASHTOWare;
using ARA.iAM.Analysis.Interfaces;

namespace ARA.iAM.Analysis
{
    class Treatment: ICriteriaDriven
    {
        public Criterion Criterion { get; }
        private int yearsBeforeAny;
        private int yearsBeforeSame;
        private List<TreatmentCost> costs;
        private List<TreatmentConsequence> consequences;
        private List<BudgetID> budgets;
    }

    class TreatmentCost: ICriteriaDriven
    {
        public Criterion Criterion { get; }
        /// <summary>
        /// The equation used to calculate the treatment's cost for assets that match this criterion.
        /// If None, no cost
        /// </summary>
        private Option<Equation> costEquation;
    }

    class TreatmentConsequence: ICriteriaDriven
    {
        public Criterion Criterion { get; }
        private string attribute;
        /// <summary>
        /// The amount by which the attribute changes when the treatment is applied
        /// </summary>
        private double change;
        /// <summary>
        /// Optionally, an equation to dynamically decide how much the attribute changes
        /// when the treatment is applied
        /// </summary>
        private Option<Equation> changeEquation;
    }
}
