using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class LinearSegmenter : Segmenter
    {
        public LinearSegmenter(List<Location> networkDefinition, List<SegmentationRule> segmentationRules) : base(networkDefinition, segmentationRules)
        {
        }

        // Assumes that an attribute is defined which contains the pavement management sections.
        public override List<SegmentBreak<T>> CreateSegmentBreaks<T>(IEnumerable<AttributeDatum<T>> attributeData)
        {
            return (from attributeDatum in attributeData
                    let segmentBreak = new SegmentBreak<T>(attributeDatum)
                    select segmentBreak)
                    .ToList();
        }
    }
}
