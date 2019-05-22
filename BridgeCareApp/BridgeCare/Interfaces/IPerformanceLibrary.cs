using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IPerformanceLibrary
    {
        PerformanceLibraryModel GetScenarioPerformanceLibrary(int selectedScenarioId, BridgeCareContext db);

        PerformanceLibraryModel SaveScenarioPerformanceLibrary(PerformanceLibraryModel data, BridgeCareContext db);
    }
}