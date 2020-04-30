using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Simulation
    {
        public AnalysisMethod AnalysisMethod { get; }

        public List<CommittedProject> CommittedProjects { get; }

        public InvestmentPlan InvestmentPlan { get; }

        public string Name { get; }

        public Network Network { get; }

        public SelectableTreatment DesignatedPassiveTreatment { get; }

        public List<PerformanceCurve> PerformanceCurves { get; }

        public List<SimulationResult> Results { get; }

        public List<SelectableTreatment> Treatments { get; }

        public HashSet<SelectableTreatment> GetActiveTreatments()
        {
            var result = Treatments.ToHashSet();
            _ = result.Remove(DesignatedPassiveTreatment);
            return result;
        }
    }
}
