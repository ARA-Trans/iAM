using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ISimulationAnalysis
    {
        SimulationAnalysisModel GetSimulationAnalysis(int id, BridgeCareContext db);
        void UpdateSimulationAnalysis(SimulationAnalysisModel model, BridgeCareContext db);
    }
}