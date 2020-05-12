using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Segmentation
{
    public abstract class SegmentationRule
    {
        private const string RECORD_CHANGED_REASON = "Record change detected.";
        public SegmentationRule(Criterion criterion, string reason = RECORD_CHANGED_REASON)
        {
            Criterion = criterion;
            Reason = reason;
        }

        public Criterion Criterion { get; }
        public string Reason { get; }
    }
}
