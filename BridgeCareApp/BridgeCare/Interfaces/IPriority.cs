using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
  public interface IPriority
  {
    PriorityLibraryModel GetScenarioPriorityLibrary(int simulationId, BridgeCareContext db);
    PriorityLibraryModel SaveScenarioPriorityLibrary(PriorityLibraryModel data, BridgeCareContext db);
  }
}