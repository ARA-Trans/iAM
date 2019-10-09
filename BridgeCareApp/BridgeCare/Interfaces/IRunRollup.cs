using BridgeCare.Models;
using System.Threading.Tasks;

namespace BridgeCare.Interfaces
{
    public interface IRunRollup
    {
        void SetLastRunDate(int networkId, BridgeCareContext db);
        Task<string> RunRollup(SimulationModel model);
    }
}