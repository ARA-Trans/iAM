using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class GisSegmenter : Segmenter
    {
        public GisSegmenter(List<Location> locations, List<Attribute> attributes, List<SegmentationRule> segmentationRules) : base(locations, attributes, segmentationRules)
        {
        }

        public override List<Location> SegmentLocations()
        {
            throw new NotImplementedException();
        }
    }
}
