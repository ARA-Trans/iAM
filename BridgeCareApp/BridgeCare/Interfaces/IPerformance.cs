using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface IPerformance
    {
        IQueryable<PerformanceModel> GetPerformance(SimulationModel data, BridgeCareContext db);
    }
}