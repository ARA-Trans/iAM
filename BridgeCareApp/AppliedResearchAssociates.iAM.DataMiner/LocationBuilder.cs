using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    public static class LocationBuilder
    {
        public static Location CreateLocation(
            string uniqueIdentifier,
            string locationName = null,
            double? start = null,
            double? end = null,
            Direction? direction = null,
            string wellKnownText = null)
        {
            if(locationName != null && start != null && end != null && direction == null)
            {
                // Linear route data with no defined direction
                return new LinearLocation(new SimpleRoute(locationName), uniqueIdentifier, start.Value, end.Value);
            }
            else if (locationName != null && start != null & end != null && direction != null && uniqueIdentifier != null)
            {
                // Linear route data with a defined direction
                return new LinearLocation(new DirectionalRoute(locationName, direction.Value), uniqueIdentifier, start.Value, end.Value);
            }
            else if (uniqueIdentifier != null && wellKnownText != null)
            {
                return new GisLocation(wellKnownText, uniqueIdentifier);
            }
            else if(locationName == null && start == null && end == null && uniqueIdentifier != null)
            {
                return new SectionLocation(uniqueIdentifier);
            }
            else
            {
                throw new InvalidOperationException("Cannot determine location type from provided inputs.");
            }
            
        }
    }
}
