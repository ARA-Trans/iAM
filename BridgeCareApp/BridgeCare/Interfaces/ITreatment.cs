using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ITreatments
    {
        IQueryable<TreatmentScenarioModel> GetTreatment(SimulationModel data, BridgeCareContext db);

        int CreateTreatment(TreatmentScenarioModel data, BridgeCareContext db);

        int UpdateTreatment(TreatmentScenarioModel data, BridgeCareContext db);

        int UpsertTreatment(TreatmentScenarioModel data, BridgeCareContext db);

    }
}