using BridgeCare.Models;
using System.Threading.Tasks;

namespace BridgeCare.Interfaces
{
    public interface IRunSimulation
    {
        string Start(SimulationModel data);
    }
}
