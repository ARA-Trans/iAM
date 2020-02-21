using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IDeficient
    {
        DeficientLibraryModel GetAnySimulationDeficientLibrary(int id, BridgeCareContext db);
        DeficientLibraryModel GetPermittedSimulationDeficientLibrary(int id, BridgeCareContext db, string username);
        DeficientLibraryModel SaveAnySimulationDeficientLibrary(DeficientLibraryModel model, BridgeCareContext db);
        DeficientLibraryModel SavePermittedSimulationDeficientLibrary(DeficientLibraryModel model, BridgeCareContext db, string username);
    }
}
