using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Simulation
    {
        public AnalysisMethod AnalysisMethod { get; }

        // ??? Some kind of result data, only accessible after a simulation is run.
        public object Committed { get; }

        public InvestmentPlan InvestmentPlan { get; }

        public string Name { get; }

        public Network Network { get; }

        public Treatment DesignatedInactiveTreatment { get; }

        public List<PerformanceCurve> PerformanceCurves { get; }

        public List<SimulationResult> Results { get; }

        public List<Treatment> Treatments { get; }

        public List<Treatment> GetActiveTreatments()
        {
            var result = Treatments.ToList();
            _ = result.Remove(DesignatedInactiveTreatment);
            return result;
        }
    }
}
