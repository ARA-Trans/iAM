using System;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class RawAttributeDatum
    {
        public string BeginStation { get; }

        public DateTime Date { get; }

        public string Direction { get; }

        public string EndStation { get; }

        public string Route { get; }

        // Can also be a string, according to the UI. Maybe also a date?
        public double Value { get; }
    }
}
