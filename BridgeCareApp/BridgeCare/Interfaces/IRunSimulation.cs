using BridgeCare.Models;
using System.Threading.Tasks;

namespace BridgeCare.Interfaces
{
    public interface IRunSimulation
    {
        void SetLastRunDate(int simulationId, BridgeCareContext db);

        Task<string> Start(SimulationModel data);
    }
}