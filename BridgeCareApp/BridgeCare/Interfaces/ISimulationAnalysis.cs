using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ISimulationAnalysis
    {
        SimulationAnalysisModel GetAnySimulationAnalysis(int id, BridgeCareContext db);
        SimulationAnalysisModel GetPermittedSimulationAnalysis(int id, BridgeCareContext db, string username);
        void UpdateSimulationAnalysis(SimulationAnalysisModel model, BridgeCareContext db);
        void PartialUpdatePermittedSimulationAnalysis(SimulationAnalysisModel model, BridgeCareContext db, string username, bool updateWeighting = true);
    }
}