using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ITreatmentLibrary
    {
        TreatmentLibraryModel GetSimulationTreatmentLibrary(int selectedScenarioId, BridgeCareContext db);
        TreatmentLibraryModel GetOwnedSimulationTreatmentLibrary(int selectedScenarioId, BridgeCareContext db, string username);
        TreatmentLibraryModel SaveSimulationTreatmentLibrary(TreatmentLibraryModel data, BridgeCareContext db);
        TreatmentLibraryModel SaveOwnedSimulationTreatmentLibrary(TreatmentLibraryModel data, BridgeCareContext db, string username);
    }
}