using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ITarget
    {
        TargetReportModel GetTarget(SimulationModel data, int[] totalYears);
        TargetLibraryModel GetScenarioTargetLibrary(int simulationId, BridgeCareContext db);
        TargetLibraryModel SaveScenarioTargetLibrary(TargetLibraryModel data, BridgeCareContext db);
    }
}