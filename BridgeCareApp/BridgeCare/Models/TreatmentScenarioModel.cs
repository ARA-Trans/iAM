using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BridgeCare.Models
{
    public class TreatmentScenarioModel
    {
        public int TreatementId { get; set; }
        public int SimulationId { get; set; }
        public TreatmentModel Treatement { get; set; }
        public List<CostModel> Cost { get; set; }

        public List<FeasibilityModel> Feasibilities { get; set; }

        public List<ConsequenceModel> Consequences { get; set; }
    }
}