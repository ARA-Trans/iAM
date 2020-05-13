using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class LinearSegmenter : Segmenter
    {
        public LinearSegmenter(List<Location> locations, List<SegmentationRule> segmentationRules) : base(locations, segmentationRules)
        {
        }

        public override List<SegmentedLocation> Segment<T>(IEnumerable<AttributeDatum<T>> attributeData)
        {
            
            foreach(var attributeDatum in attributeData)
            {

            }
            throw new NotImplementedException();
        }
    }
}
