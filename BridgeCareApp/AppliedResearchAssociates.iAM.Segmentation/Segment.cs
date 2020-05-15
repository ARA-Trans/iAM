using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class Segment<T>
    {
        public Segment(Location segmentLocation, AttributeDatum<T> attributeDatum)
        {
            SegmentLocation = segmentLocation;
            AttributeDatum = attributeDatum;
        }

        public Location SegmentLocation { get; }
        public AttributeDatum<T> AttributeDatum { get; }
    }
}
