using System.Collections.Generic;
using AppliedResearchAssociates.iAM.SimulationOutput;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Simulation
    {
        public AnalysisMethod AnalysisMethod { get; }

        public List<CommittedProject> CommittedProjects { get; }

        public SelectableTreatment DesignatedPassiveTreatment { get; }

        public InvestmentPlan InvestmentPlan { get; }

        public string Name { get; }

        public Network Network { get; }

        public int NumberOfYearsOfTreatmentOutlook { get; } = 100;

        public List<PerformanceCurve> PerformanceCurves { get; }

        public List<SimulationYearDetail> Results { get; } = new List<SimulationYearDetail>();

        public List<SelectableTreatment> Treatments { get; }

        public HashSet<SelectableTreatment> GetActiveTreatments()
        {
            var result = Treatments.ToHashSet();
            _ = result.Remove(DesignatedPassiveTreatment);
            return result;
        }
    }
}
