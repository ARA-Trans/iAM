using BridgeCare.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BridgeCare.Interfaces
{
    public interface ISimulation
    {
        List<SimulationModel> GetSimulations(BridgeCareContext db);
        List<SimulationModel> GetSimulations(BridgeCareContext db, UserInformationModel userInformation);
        SimulationModel CreateSimulation(CreateSimulationDataModel model, BridgeCareContext db);
        void UpdateSimulation(SimulationModel model, BridgeCareContext db);
        void DeleteSimulation(int id, BridgeCareContext db);
        Task<string> RunSimulation(SimulationModel model);
        void SetSimulationLastRunDate(int id, BridgeCareContext db);

    }
}