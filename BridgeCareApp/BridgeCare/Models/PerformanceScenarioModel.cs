namespace BridgeCare.Models
{
    public class PerformanceScenarioModel
    {
        public PerformanceScenarioModel()
        {
            Performance = new PerformanceModel();
        }

        public int PerformanceId { get; set; }
        public int SimulationId { get; set; }
        public PerformanceModel Performance { get; set; }
    }
}