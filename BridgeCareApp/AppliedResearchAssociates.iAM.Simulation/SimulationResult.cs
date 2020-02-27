using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class SimulationResult
    {
        public Section Section { get; }

        public List<TreatmentApplication> YearlyTreatmentApplications { get; } = new List<TreatmentApplication>();
    }
}
