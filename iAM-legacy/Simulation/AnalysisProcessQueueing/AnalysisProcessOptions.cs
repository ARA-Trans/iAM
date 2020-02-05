using CommandLine;

namespace Simulation.AnalysisProcessQueueing
{
    public class AnalysisProcessOptions
    {
        [Option(Required = true)]
        public string ConnectionString { get; set; }

        [Option("api-call")]
        public bool IsApiCall { get; set; }

        [Option(Required = true)]
        public int NetworkId { get; set; }

        [Option(Required = true)]
        public string NetworkName { get; set; }

        [Option(Required = true)]
        public int SimulationId { get; set; }

        [Option(Required = true)]
        public string SimulationName { get; set; }

        [Option]
        internal string PipeHandle { get; set; }
    }
}
