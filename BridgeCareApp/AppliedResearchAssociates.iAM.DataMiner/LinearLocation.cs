﻿using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataMiner
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

        public override bool MatchOn(Location location)
        {
            if (location is LinearLocation linearLocation)
            {
                return
                    linearLocation.Start <= Start &&
                    linearLocation.End > End &&
                    linearLocation.Route.MatchOn(Route);
            }
            else
            {
                return false;
            }
        }
    }
}
