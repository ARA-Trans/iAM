using System.Runtime.Serialization;

namespace BridgeCare.Models
{
    public class SimulationAnalysisModel
    {
        [DataMember(Name = "id")]
        public int simulationId { get; set; }

        [DataMember(Name = "startYear")]
        public int committed_start { get; set; }

        [DataMember(Name = "analysisPeriod")]
        public int committed_period { get; set; }

        [DataMember(Name = "optimizationType")]
        public string analysis { get; set; }

        [DataMember(Name = "budgetType")]
        public string budget_constraint { get; set; }

        [DataMember(Name = "benefitLimit")]
        public double benefit_limit { get; set; }

        [DataMember(Name = "description")]
        public string comments { get; set; }

        [DataMember(Name = "criteria")]
        public string jurisdiction { get; set; }
    }
}