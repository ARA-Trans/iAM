using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class SegmentBreak<T>
    {
        public SegmentBreak(AttributeDatum<T> attributeDatum) => AttributeDatum = attributeDatum;
        public AttributeDatum<T> AttributeDatum { get; }
    }
}
