using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner;
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
            // 1. Create a new SegmentedLocation whenever a new location is
            //    processed with "New Location - Begin" or "New Location - End"
            //    as the segmentation rule associated with the object
            // 2. Get a list of segmentaion critera
            // 3. Get a list of distinct years from each attribute's data
            // 4. Gregg says that in order to merge sections (specifically small
            //    extraneous sections left over from the segmentation process
            //    that we must keep like to like, this requires us to keep an
            //    AttributeDatum value to use in comparison

            foreach (location in locations)
            {
                Apply list of segmentation rules to the location
            }
            */
        }
    }
}
