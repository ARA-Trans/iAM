using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class LinearSegmenter : Segmenter
    {
        public LinearSegmenter(List<Location> locations, List<Attribute> attributes, List<SegmentationRule> segmentationRules) : base(locations, attributes, segmentationRules)
        {
        }

        public override List<Location> SegmentLocations()
        {
            throw new NotImplementedException();
        }
    }
}
