using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public class Treatment
    {
        public List<Budget> BudgetCategories { get; }

        public List<TreatmentConsequence> Consequences { get; }

        public List<ConditionalEquation> CostEquations { get; }

        public string Description { get; }

        public Criterion FeasibilityCriterion { get; }

        public string Name { get; }

        public int ShadowForAnyTreatment { get; }

        public int ShadowForSameTreatment { get; }

        public List<TreatmentScheduling> Schedulings { get; }
    }
}
