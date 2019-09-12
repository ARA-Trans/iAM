using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IDeficient
    {
        DeficientLibraryModel GetScenarioDeficientLibrary(int simulationId, BridgeCareContext db);
        DeficientLibraryModel SaveScenarioDeficientLibrary(DeficientLibraryModel data, BridgeCareContext db);
    }
}