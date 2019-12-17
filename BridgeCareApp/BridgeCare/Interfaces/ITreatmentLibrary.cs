using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ITreatmentLibrary
    {
        TreatmentLibraryModel GetAnySimulationTreatmentLibrary(int selectedScenarioId, BridgeCareContext db);
        TreatmentLibraryModel GetOwnedSimulationTreatmentLibrary(int selectedScenarioId, BridgeCareContext db, string username);
        TreatmentLibraryModel SaveAnySimulationTreatmentLibrary(TreatmentLibraryModel data, BridgeCareContext db);
        TreatmentLibraryModel SaveOwnedSimulationTreatmentLibrary(TreatmentLibraryModel data, BridgeCareContext db, string username);
    }
}