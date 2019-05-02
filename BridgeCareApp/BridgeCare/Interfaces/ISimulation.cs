using BridgeCare.Models;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ISimulation
    {
        IQueryable<SimulationModel> GetAllSimulations();

        IEnumerable<SimulationModel> GetSelectedSimulation(int id);

        void UpdateName(SimulationModel model);

        int CreateNewSimulation(int networkId, string simulationName, BridgeCareContext db);
    }
}