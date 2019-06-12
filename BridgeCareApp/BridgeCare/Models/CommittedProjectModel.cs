using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class CommittedProjectModel
    {
        public int SimulationId { get; set; }

        public int SectionId { get; set; }

        public int Years { get; set; }

        public string TreatmentName { get; set; }

        public int YearSame { get; set; }

        public int YearAny { get; set; }

        public string Budget { get; set; }

        public double Cost { get; set; }        

        public List<CommitConsequenceModel> CommitConsequences { get; set; }
    }
}