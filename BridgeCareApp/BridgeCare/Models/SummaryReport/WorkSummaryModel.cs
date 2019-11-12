using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class WorkSummaryModel
    {
        public List<SimulationDataModel> SimulationDataModels { get; set; }

        public List<BridgeDataModel> BridgeDataModels { get; set; }

        public List<string> Treatments { get; set; }
    }
}