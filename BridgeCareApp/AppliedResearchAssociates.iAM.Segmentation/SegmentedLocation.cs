using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class SegmentedLocation<T>
    {
        public Guid Guid { get; }
        public Location Location { get; }
        public AttributeDatum<T> AttributeDatum { get; }

        public SegmentedLocation(Location location, AttributeDatum<T> attributeDatum)
        {
            Location = location;
            AttributeDatum = attributeDatum;
        }
    }
}
