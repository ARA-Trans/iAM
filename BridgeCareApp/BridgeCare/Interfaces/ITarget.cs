using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ITarget
    {
        TargetReportModel GetTarget(SimulationModel model, int[] totalYears, BridgeCareContext db);
        TargetLibraryModel GetSimulationTargetLibrary(int id, BridgeCareContext db);
        TargetLibraryModel GetOwnedSimulationTargetLibrary(int id, BridgeCareContext db, string username);
        TargetLibraryModel SaveSimulationTargetLibrary(TargetLibraryModel model, BridgeCareContext db);
        TargetLibraryModel SaveOwnedSimulationTargetLibrary(TargetLibraryModel model, BridgeCareContext db, string username);
    }
}