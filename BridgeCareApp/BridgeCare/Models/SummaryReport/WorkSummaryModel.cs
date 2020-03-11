using System.Collections.Generic;
using BridgeCare.Models.SummaryReport;

namespace BridgeCare.Models
{
    public class WorkSummaryModel
    {
        public List<SimulationDataModel> SimulationDataModels { get; set; }

        public List<BridgeDataModel> BridgeDataModels { get; set; }

        public List<string> Treatments { get; set; }

        public List<BudgetsPerBRKey> BudgetsPerBRKeys { get; set; }
    }
}
