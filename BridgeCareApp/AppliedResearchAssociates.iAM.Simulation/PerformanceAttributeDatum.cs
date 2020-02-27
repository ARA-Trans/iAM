using System;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class PerformanceAttributeDatum
    {
        public string BeginStation { get; }

        public DateTime Date { get; }

        public string Direction { get; }

        public string EndStation { get; }

        public string Route { get; }

        // May want to use decimal for monetary attributes, if they exist.
        public double Value { get; }
    }
}
