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

        public SelectableTreatment DesignatedPassiveTreatment { get; }

        public List<PerformanceCurve> PerformanceCurves { get; }

        public List<SimulationResult> Results { get; }

        public List<SelectableTreatment> Treatments { get; }

        public List<SelectableTreatment> GetActiveTreatments()
        {
            var result = Treatments.ToList();
            _ = result.Remove(DesignatedPassiveTreatment);
            return result;
        }
    }
}
