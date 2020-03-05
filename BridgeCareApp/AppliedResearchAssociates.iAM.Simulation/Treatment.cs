using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Treatment
    {
        public BudgetCategory BudgetCategory { get; }

        public List<TreatmentConsequence> Consequences { get; }

        public List<TreatmentCost> Costs { get; }

        public string Description { get; }

        public Criterion FeasibilityCriterion { get; }

        public string Name { get; }

        public int NumberOfYearsBeforeSubsequentApplicationOfAnyTreatment { get; }

        public int NumberOfYearsBeforeSubsequentApplicationOfSameTreatment { get; }
    }
}
