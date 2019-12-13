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
        void UpdateSimulation(SimulationModel model, BridgeCareContext db);
        void UpdateOwnedSimulation(SimulationModel model, BridgeCareContext db, UserInformationModel userInformation);
        void DeleteSimulation(int id, BridgeCareContext db);
        void DeleteOwnedSimulation(int id, BridgeCareContext db, UserInformationModel userInformation);
        Task<string> RunSimulation(SimulationModel model);
        void SetSimulationLastRunDate(int id, BridgeCareContext db);

    }
}