using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ITreatments
    {
        IQueryable<TreatmentScenarioModel> GetTreatment(SimulationModel data, BridgeCareContext db);
        
    }
}