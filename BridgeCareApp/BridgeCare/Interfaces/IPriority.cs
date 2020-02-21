using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
  public interface IPriority
    {
        PriorityLibraryModel GetAnySimulationPriorityLibrary(int id, BridgeCareContext db);
        PriorityLibraryModel GetPermittedSimulationPriorityLibrary(int id, BridgeCareContext db, string username);
        PriorityLibraryModel SaveAnySimulationPriorityLibrary(PriorityLibraryModel model, BridgeCareContext db);
        PriorityLibraryModel SavePermittedSimulationPriorityLibrary(PriorityLibraryModel model, BridgeCareContext db, string username);
    }
}