using System.Collections.Generic;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ITarget
    {
        TargetReportModel GetTarget(SimulationModel data, int[] totalYears);
        List<TargetModel> GetTargets(int simulationId, BridgeCareContext db);
        List<TargetModel> SaveTargets(int simulationId, List<TargetModel> data, BridgeCareContext db);
    }
}