using BridgeCare.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BridgeCare.Interfaces
{
    public interface IBridgeData
    {
        List<BridgeDataModel> GetBridgeData(List<int> brKeys, BridgeCareContext db);
        IQueryable<Section> GetSectionData(SimulationModel model, BridgeCareContext db);
        DataTable GetSimulationData(SimulationModel model, BridgeCareContext db, List<int> simulationYears);
        IQueryable<ReportProjectCost> GetReportData(SimulationModel model, BridgeCareContext db, List<int> simulationYears);
        List<string> GetSummaryReportMissingAttributes(int simulationId, int networkId, BridgeCareContext db);
        List<string> GetTreatments(int simulationId, BridgeCareContext db);
        List<string> GetBudgets(int simulationId, BridgeCareContext db);
    }
}
