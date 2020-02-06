using System;

namespace Simulation.AnalysisProcessQueueing
{
    [Serializable]
    public class ResourceConsumptionStatistics
    {
        public long PeakVirtualMemorySize { get; set; }

        public long PeakWorkingSet { get; set; }
    }
}
