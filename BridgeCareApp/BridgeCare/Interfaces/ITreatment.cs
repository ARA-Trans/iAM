using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ITreatment
    {
        IQueryable<TreatmentScenarioModel> GetPerformance(SimulationModel data, BridgeCareContext db);

        void UpdatePerformanceScenario(TreatmentScenarioModel data, BridgeCareContext db);
    }
}