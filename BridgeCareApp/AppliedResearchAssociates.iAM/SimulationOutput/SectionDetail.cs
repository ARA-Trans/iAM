using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.SimulationOutput
{
    public sealed class SectionDetail
    {
        public bool AppliedTreatmentIsDueToCommittedProject { get; set; }

        public List<BudgetDetail> BudgetDetailsOfAppliedTreatment { get; } = new List<BudgetDetail>();

        public List<TreatmentOptionDetail> DetailsOfFeasibleTreatments { get; } = new List<TreatmentOptionDetail>();

        public List<PreferredTreatmentDetail> DetailsOfPreferredTreatments { get; } = new List<PreferredTreatmentDetail>();

        public string FacilityName { get; set; }

        public string NameOfAppliedTreatment { get; set; }

        public string SectionName { get; set; }

        public Dictionary<string, double> ValuePerNumberAttribute { get; } = new Dictionary<string, double>();

        public Dictionary<string, string> ValuePerTextAttribute { get; } = new Dictionary<string, string>();
    }
}
