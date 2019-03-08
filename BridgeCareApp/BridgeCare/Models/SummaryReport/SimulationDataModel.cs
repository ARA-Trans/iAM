using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class SimulationDataModel
    {
        public int SectionId { get; set; }

        public List<YearsData> YearsData { get; set; }
    }
}