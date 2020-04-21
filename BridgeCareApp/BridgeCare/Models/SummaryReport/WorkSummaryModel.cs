using System.Collections.Generic;
using BridgeCare.Models.SummaryReport;
using BridgeCare.Models.SummaryReport.ParametersTAB;

namespace BridgeCare.Models
{
    public class WorkSummaryModel
    {
        public List<SimulationDataModel> SimulationDataModels { get; set; }

        public List<BridgeDataModel> BridgeDataModels { get; set; }

        public List<string> Treatments { get; set; }

        public List<BudgetsPerBRKey> BudgetsPerBRKeys { get; set; }

        public List<UnfundedRecommendationModel> UnfundedRecommendations { get; set; }

        public ParametersModel ParametersModel { get; set; }
    }
}
