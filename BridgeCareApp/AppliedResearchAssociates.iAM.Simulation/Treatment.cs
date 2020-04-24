using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Treatment
    {
        // Gregg's memo mentions a "cumulative cost" flag. Couldn't find it in the desktop UI.

        public List<Budget> BudgetCategories { get; }

        public List<TreatmentConsequence> Consequences { get; }

        public List<ConditionalEquation> CostEquations { get; }

        public string Description { get; }

        public Criterion FeasibilityCriterion { get; }

        public string Name { get; }

        public int ShadowForAnyTreatment { get; }

        public int ShadowForSameTreatment { get; }

        public List<TreatmentScheduling> Schedulings { get; }

        public List<TreatmentSupersession> Supersessions { get; }
    }
}
