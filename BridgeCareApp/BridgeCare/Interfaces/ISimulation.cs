using BridgeCare.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BridgeCare.Interfaces
{
    public interface ISimulation
    {
        List<SimulationModel> GetSimulations(BridgeCareContext db);
        List<SimulationModel> GetPermittedSimulations(BridgeCareContext db, string username);
        SimulationModel CreateSimulation(CreateSimulationDataModel model, BridgeCareContext db);
        SimulationModel CloneSimulation(int simulationId, BridgeCareContext db, string username);
        void UpdateAnySimulation(SimulationModel model, BridgeCareContext db);
        void UpdatePermittedSimulation(SimulationModel model, BridgeCareContext db, string username);
        void DeleteAnySimulation(int id, BridgeCareContext db);
        void DeletePermittedSimulation(int id, BridgeCareContext db, string username);
        Task<string> RunSimulation(SimulationModel model);
        Task<string> RunPermittedSimulation(SimulationModel model, BridgeCareContext db, string username);
        void SetSimulationLastRunDate(int id, BridgeCareContext db);
        void SetPermittedSimulationUsers(int simulationId, List<SimulationUserModel> simulationUsers, BridgeCareContext db, string username);
        void SetAnySimulationUsers(int simulationId, List<SimulationUserModel> simulationUsers, BridgeCareContext db);
    }
}
