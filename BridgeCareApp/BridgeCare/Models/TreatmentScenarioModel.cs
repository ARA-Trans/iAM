namespace BridgeCare.Models
{
    public class TreatemntScenarioModel
    {
        public TreatementScenarioModel()
        {
            Treatement = new TreatementModel();
        }

        public int TreatemntId { get; set; }
        public int SimulationId { get; set; }
        public TreatementModel Treatemnt { get; set; }
    }
}