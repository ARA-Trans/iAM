using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class LinearSegmenter : Segmenter<LinearLocation>
    {
        public LinearSegmenter(List<LinearLocation> locations, List<Attribute> attributes, List<SegmentationRule> segmentationRules) : base(locations, attributes, segmentationRules)
        {
        }

        public override List<SegmentedLocation> SegmentLocations()
        {
            foreach(var attribute in Attributes)
            {
                var currentAttributeValue = attribute
            }
        }
    }
}
