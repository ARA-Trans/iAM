using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ISimulationAnalysis
    {
        SimulationAnalysisModel GetSimulationAnalysis(int simulationId, BridgeCareContext db);

        void UpdateSimulationAnalysis(SimulationAnalysisModel model, BridgeCareContext db);
    }
}