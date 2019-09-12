using BridgeCare.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface IBridgeData
    {
        List<BridgeDataModel> GetBridgeData(List<int> BRKeys, BridgeCareContext dbContext);

        IQueryable<Section> GetSectionData(SimulationModel simulationModel, BridgeCareContext dbContext);

        DataTable GetSimulationData(SimulationModel simulationModel, BridgeCareContext dbContext, List<int> simulationYears);

        IQueryable<ReportProjectCost> GetReportData(SimulationModel simulationModel, BridgeCareContext dbContext, List<int> simulationYears);

        List<string> GetSummaryReportMissingAttributes(int simulationId, int networkId, BridgeCareContext db);
    }
}