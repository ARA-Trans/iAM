using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ITreatmentLibrary
    {
        TreatmentLibraryModel GetSimulationTreatmentLibrary(int selectedScenarioId, BridgeCareContext db);
        TreatmentLibraryModel SaveSimulationTreatmentLibrary(TreatmentLibraryModel data, BridgeCareContext db);
    }
}