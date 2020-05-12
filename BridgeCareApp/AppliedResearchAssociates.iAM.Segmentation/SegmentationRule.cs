using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public abstract class SegmentationRule
    {
        public SegmentationRule(Criterion criterion)
        {
            Criterion = criterion;
        }

        public Criterion Criterion { get; }
    }
}
