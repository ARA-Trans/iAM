using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
  public interface IPriority
    {
        PriorityLibraryModel GetSimulationPriorityLibrary(int id, BridgeCareContext db);
        PriorityLibraryModel GetOwnedSimulationPriorityLibrary(int id, BridgeCareContext db, string username);
        PriorityLibraryModel SaveSimulationPriorityLibrary(PriorityLibraryModel model, BridgeCareContext db);
        PriorityLibraryModel SaveOwnedSimulationPriorityLibrary(PriorityLibraryModel model, BridgeCareContext db, string username);
    }
}