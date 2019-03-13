using BridgeCare.Models;
using System.Threading.Tasks;

namespace BridgeCare.Interfaces
{
    public interface IRunSimulation
    {
        Task<string> Start(SimulationModel data);
    }
}