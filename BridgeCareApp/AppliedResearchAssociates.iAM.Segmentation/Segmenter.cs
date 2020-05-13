using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public abstract class Segmenter
    {
        public Segmenter(List<Location> locations, List<Attribute> attributes, List<SegmentationRule> segmentationRules)
        {
            Locations = locations;
            Attributes = attributes;
            SegmentationRules = segmentationRules;
        }

        public abstract List<Location> SegmentLocations();

        public List<Location> Locations { get; }
        public List<Attribute> Attributes { get; }
        public List<SegmentationRule> SegmentationRules { get; }
    }
}
