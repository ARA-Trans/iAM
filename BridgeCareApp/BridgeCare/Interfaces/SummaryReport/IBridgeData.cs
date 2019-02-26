using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeCare.Interfaces
{
    public interface IBridgeData
    {
        List<BridgeDataModel> GetBridgeData(List<int> BRKeys, BridgeCareContext dbContext);

        IQueryable<Section> GetSectionData(SimulationModel simulationModel, BridgeCareContext dbContext);

        DataTable GetSimulationData(SimulationModel simulationModel, BridgeCareContext dbContext, List<int> simulationYears);
        IQueryable<ReportProjectCost> GetReportData(SimulationModel simulationModel, BridgeCareContext dbContext, List<int> simulationYears);
    }
}
