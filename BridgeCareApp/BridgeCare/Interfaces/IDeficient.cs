using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IDeficient
    {
        DeficientLibraryModel GetAnySimulationDeficientLibrary(int id, BridgeCareContext db);
        DeficientLibraryModel GetOwnedSimulationDeficientLibrary(int id, BridgeCareContext db, string username);
        DeficientLibraryModel SaveAnySimulationDeficientLibrary(DeficientLibraryModel model, BridgeCareContext db);
        DeficientLibraryModel SaveOwnedSimulationDeficientLibrary(DeficientLibraryModel model, BridgeCareContext db, string username);
    }
}