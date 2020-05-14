using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.SimulationOutput
{
    public sealed class PreferredTreatmentDetail
    {
        public List<BudgetDetail> DetailsOfBudgets { get; } = new List<BudgetDetail>();

        public string TreatmentName { get; set; }
    }
}
