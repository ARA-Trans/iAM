using BridgeCare.Models;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface IPerformanceLibrary
    {
        PerformanceLibraryModel GetScenarioPerformanceLibrary(int selectedScenarioId, BridgeCareContext db);

        void SaveScenarioPerformanceLibrary(PerformanceLibraryModel data, BridgeCareContext db);
    }
}