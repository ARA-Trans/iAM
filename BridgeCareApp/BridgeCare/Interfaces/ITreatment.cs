using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface ITreatments
    {
        TreatmentsModel GetTreatments(int simulationID, BridgeCareContext db);
       
        TreatmentsModel UpsertTreatment(TreatmentsModel data, BridgeCareContext db);
    }
}