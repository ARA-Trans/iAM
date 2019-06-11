using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
  public interface IPriority
  {
    List<PriorityModel> GetPriorities(int selectedScenarioId, BridgeCareContext db);
    List<PriorityModel> SavePriorities(List<PriorityModel> data, BridgeCareContext db);
  }
}