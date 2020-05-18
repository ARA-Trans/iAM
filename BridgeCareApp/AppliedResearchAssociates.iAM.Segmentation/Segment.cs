using System;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class Segment<T>
    {
        public Segment(Location segmentLocation, AttributeDatum<T> attributeDatum)
        {
            Location = segmentLocation;
            AttributeDatum = attributeDatum;
        }

        public Location Location { get; }
        public AttributeDatum<T> AttributeDatum { get; }
    }
}
