using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class LinearLocation : Location
    {
        public Route Route { get; }
        public double Start { get; }

        public double End { get; }

        // The uniqueIdentifier can really be any uniquely identifiable string
        // of characters. (ROUTE-BMP-EMP-DIR for example).
        public LinearLocation(Route route, double start, double end)
        {
            Route = route;
            Start = start;
            End = end;
        }
    }
}
