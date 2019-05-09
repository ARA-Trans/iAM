using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ITreatments
    {
        TreatmentScenarioModel GetTreatment(int treatmentID, BridgeCareContext db);
        IQueryable<TreatmentScenarioModel> GetTreatments(int simulationID, BridgeCareContext db);
        TreatmentScenarioModel CreateTreatment(TreatmentScenarioModel data, BridgeCareContext db);

        TreatmentScenarioModel UpsertTreatment(TreatmentScenarioModel data, BridgeCareContext db);

    }
}