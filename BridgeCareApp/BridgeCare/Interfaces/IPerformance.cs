using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface IPerformance
    {
        IQueryable<PerformanceScenarioModel> GetPerformance(SimulationModel data, BridgeCareContext db);

        void UpdatePerformanceScenario(PerformanceScenarioModel data, BridgeCareContext db);
    }
}