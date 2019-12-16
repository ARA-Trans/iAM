using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IPerformanceLibrary
    {
        PerformanceLibraryModel GetSimulationPerformanceLibrary(int id, BridgeCareContext db);
        PerformanceLibraryModel GetOwnedSimulationPerformanceLibrary(int id, BridgeCareContext db, string username);

        PerformanceLibraryModel SaveSimulationPerformanceLibrary(PerformanceLibraryModel model, BridgeCareContext db);
        PerformanceLibraryModel SaveOwnedSimulationPerformanceLibrary(PerformanceLibraryModel model, BridgeCareContext db, string username);
    }
}