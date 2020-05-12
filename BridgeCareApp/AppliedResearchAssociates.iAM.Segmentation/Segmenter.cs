using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public abstract class Segmenter<T>
    {
        public Segmenter(List<T> locations, List<Attribute> attributes, List<SegmentationRule> segmentationRules)
        {
            Locations = locations;
            Attributes = attributes;
            SegmentationRules = segmentationRules;
        }

        public abstract List<T> SegmentLocations();

        public List<T> Locations { get; }
        public List<Attribute> Attributes { get; }
        public List<SegmentationRule> SegmentationRules { get; }
    }
}
