using System;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class Segment<T>
    {
        public Segment(Location segmentLocation, AttributeDatum<T> segmentationAttributeDatum)
        {
            Location = segmentLocation;
            SegmentationAttributeDatum = segmentationAttributeDatum;
        }

        public Location Location { get; }
        public AttributeDatum<T> SegmentationAttributeDatum { get; }
    }
}
