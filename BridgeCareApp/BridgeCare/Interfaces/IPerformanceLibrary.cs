using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IPerformanceLibrary
    {
        PerformanceLibraryModel GetSimulationPerformanceLibrary(int id, BridgeCareContext db);

        PerformanceLibraryModel SaveSimulationPerformanceLibrary(PerformanceLibraryModel model, BridgeCareContext db);
    }
}