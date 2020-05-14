using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class GisSegmenter : Segmenter
    {
        public GisSegmenter(List<Location> locations, List<SegmentationRule> segmentationRules) : base(locations, segmentationRules)
        {
        }

        public override List<Location> CreateSegmentBreaks<T>(IEnumerable<AttributeDatum<T>> attributeData)
        {
            throw new NotImplementedException();
        }
    }
}
