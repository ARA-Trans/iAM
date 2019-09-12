using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ITreatmentLibrary
    {
        TreatmentLibraryModel GetScenarioTreatmentLibrary(int selectedScenarioId, BridgeCareContext db);
       
        TreatmentLibraryModel SaveScenarioTreatmentLibrary(TreatmentLibraryModel data, BridgeCareContext db);
    }
}