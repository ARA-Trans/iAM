using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IDeficient
    {
        DeficientLibraryModel GetSimulationDeficientLibrary(int id, BridgeCareContext db);
        DeficientLibraryModel SaveSimulationDeficientLibrary(DeficientLibraryModel model, BridgeCareContext db);
    }
}