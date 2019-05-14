using BridgeCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ISimulationAnalysis
    {
        SimulationAnalysisModel GetSimulationAnalyis(int simulationId, BridgeCareContext db);

        void UpdateSimulationAnalyis(SimulationAnalysisModel model, BridgeCareContext db);
    }
}