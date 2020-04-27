using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IPerformanceLibrary
    {
        PerformanceLibraryModel GetAnySimulationPerformanceLibrary(int id, BridgeCareContext db);
        PerformanceLibraryModel GetPermittedSimulationPerformanceLibrary(int id, BridgeCareContext db, string username);

        PerformanceLibraryModel SaveAnySimulationPerformanceLibrary(PerformanceLibraryModel model, BridgeCareContext db);
        PerformanceLibraryModel SavePermittedSimulationPerformanceLibrary(PerformanceLibraryModel model, BridgeCareContext db, string username);
    }
}
