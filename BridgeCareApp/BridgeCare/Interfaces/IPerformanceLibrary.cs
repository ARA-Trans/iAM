using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IPerformanceLibrary
    {
        PerformanceLibraryModel GetAnySimulationPerformanceLibrary(int id, BridgeCareContext db);
        PerformanceLibraryModel GetOwnedSimulationPerformanceLibrary(int id, BridgeCareContext db, string username);

        PerformanceLibraryModel SaveAnySimulationPerformanceLibrary(PerformanceLibraryModel model, BridgeCareContext db);
        PerformanceLibraryModel SaveOwnedSimulationPerformanceLibrary(PerformanceLibraryModel model, BridgeCareContext db, string username);
    }
}