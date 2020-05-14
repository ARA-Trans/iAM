using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public abstract class Segmenter
    {
        public Segmenter(List<Location> locations, List<SegmentationRule> segmentationRules)
        {
            Locations = locations;
            SegmentationRules = segmentationRules;
        }

        // The result of calling this function is the creation of a new "network" of segmented assets.
        public abstract List<SegmentBreak<T>> CreateSegmentBreaks<T>(IEnumerable<AttributeDatum<T>> attributeData);

        public List<Location> Locations { get; }

        public List<SegmentationRule> SegmentationRules { get; }
    }
}
