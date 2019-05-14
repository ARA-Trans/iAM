using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ISimulationAnalysis
    {
        SimulationAnalysisModel GetSimulationAnalyis(int simulationId, BridgeCareContext db);

        void UpdateSimulationAnalyis(SimulationAnalysisModel model, BridgeCareContext db);
    }
}