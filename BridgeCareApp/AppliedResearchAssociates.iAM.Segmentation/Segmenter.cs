using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

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
