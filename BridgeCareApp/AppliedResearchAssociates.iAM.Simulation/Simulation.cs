using System.Collections.Generic;

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

        public List<PerformanceCurve> PerformancCurves { get; }

        public List<SimulationResult> Results { get; }

        public List<Treatment> Treatments { get; }
    }
}
