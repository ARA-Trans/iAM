using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class SimulationDataModel
    {
        public int SectionId { get; set; }

        public List<YearsData> YearsData { get; set; }

        // Used in some tab reports
        public string DeckArea { get; set; }
                
        public int BRKey { get; set; }
    }
}