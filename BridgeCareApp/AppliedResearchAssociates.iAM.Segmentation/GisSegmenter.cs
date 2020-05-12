using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class GisSegmenter : Segmenter
    {
        public GisSegmenter(List<Location> locations, List<SegmentationRule> segmentationRules) : base(locations, segmentationRules)
        {
        }

        public override List<Location> Segment()
        {
            throw new NotImplementedException();
        }
    }
}
