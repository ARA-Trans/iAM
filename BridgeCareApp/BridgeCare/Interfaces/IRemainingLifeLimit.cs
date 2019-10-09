using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IRemainingLifeLimit
    {
        RemainingLifeLimitLibraryModel GetSimulationRemainingLifeLimitLibrary(int id, BridgeCareContext db);
        RemainingLifeLimitLibraryModel SaveSimulationRemainingLifeLimitLibrary(RemainingLifeLimitLibraryModel model, BridgeCareContext db);
    }
}