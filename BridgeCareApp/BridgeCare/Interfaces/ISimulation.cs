using BridgeCare.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BridgeCare.Interfaces
{
    public interface ISimulation
    {
        List<SimulationModel> GetSimulations(BridgeCareContext db);
        List<SimulationModel> GetOwnedSimulations(BridgeCareContext db, string username);
        SimulationModel CreateSimulation(CreateSimulationDataModel model, BridgeCareContext db);
        void UpdateAnySimulation(SimulationModel model, BridgeCareContext db);
        void UpdateOwnedSimulation(SimulationModel model, BridgeCareContext db, string username);
        void DeleteAnySimulation(int id, BridgeCareContext db);
        void DeleteOwnedSimulation(int id, BridgeCareContext db, string username);
        Task<string> RunSimulation(SimulationModel model);
        Task<string> RunOwnedSimulation(SimulationModel model, BridgeCareContext db, string username);
        void SetSimulationLastRunDate(int id, BridgeCareContext db);
        void SetOwnedSimulationUsers(int simulationId, List<SimulationUserModel> simulationUsers, BridgeCareContext db, string username);
        void SetAnySimulationUsers(int simulationId, List<SimulationUserModel> simulationUsers, BridgeCareContext db);
    }
}
