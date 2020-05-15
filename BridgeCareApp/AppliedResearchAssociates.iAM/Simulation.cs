using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.SimulationOutput;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Simulation
    {
        public Simulation(Network network)
        {
            Network = network ?? throw new ArgumentNullException(nameof(network));

            AnalysisMethod = new AnalysisMethod(this);
            InvestmentPlan = new InvestmentPlan(this);
        }

        public AnalysisMethod AnalysisMethod { get; }

        public List<CommittedProject> CommittedProjects { get; } = new List<CommittedProject>();

        public SelectableTreatment DesignatedPassiveTreatment { get; set; }

        public InvestmentPlan InvestmentPlan { get; }

        public string Name { get; set; }

        public Network Network { get; }

        public int NumberOfYearsOfTreatmentOutlook { get; set; } = 100;

        public List<PerformanceCurve> PerformanceCurves { get; } = new List<PerformanceCurve>();

        public List<SimulationYearDetail> Results { get; } = new List<SimulationYearDetail>();

        public List<SelectableTreatment> Treatments { get; } = new List<SelectableTreatment>();

        public IReadOnlyCollection<SelectableTreatment> GetActiveTreatments()
        {
            var result = Treatments.ToHashSet();
            _ = result.Remove(DesignatedPassiveTreatment);
            return result;
        }
    }
}
