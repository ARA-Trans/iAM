using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public class Segment<T>
    {
        public Segment(AttributeDatum<T> attributeDatum) => AttributeDatum = attributeDatum;
        public AttributeDatum<T> AttributeDatum { get; }
    }
}
