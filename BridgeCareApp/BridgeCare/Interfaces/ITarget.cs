using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ITarget
    {
        TargetReportModel GetTarget(SimulationModel model, int[] totalYears, BridgeCareContext db);
        TargetLibraryModel GetSimulationTargetLibrary(int id, BridgeCareContext db);
        TargetLibraryModel SaveSimulationTargetLibrary(TargetLibraryModel model, BridgeCareContext db);
    }
}