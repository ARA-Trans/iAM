using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeCare.Interfaces
{
    public interface IBridgeData
    {
        List<BridgeDataModel> GetBridgeData(List<int> BRKeys, BridgeCareContext dbContext);

        IQueryable<SectionDataModel> GetSectionData(SimulationModel simulationModel, BridgeCareContext dbContext);

        IQueryable<Type> GetSimulationData(SimulationModel simulationModel, BridgeCareContext dbContext);
    }
}
