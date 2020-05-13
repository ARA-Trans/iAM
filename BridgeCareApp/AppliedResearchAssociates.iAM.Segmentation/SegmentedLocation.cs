using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class SegmentedLocation
    {
        public Guid Guid { get; }
        public Location Location { get; }


        public SegmentedLocation(Location location)
        {
            Location = location;
        }
    }
}
