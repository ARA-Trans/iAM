using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class TreatmentConsiderationDetail
    {
        public TreatmentConsiderationDetail(string treatmentName)
        {
            if (string.IsNullOrWhiteSpace(treatmentName))
            {
                throw new ArgumentException("Treatment name is blank.", nameof(treatmentName));
            }

            TreatmentName = treatmentName;
        }

        public List<BudgetDetail> BudgetDetails { get; } = new List<BudgetDetail>();

        public string TreatmentName { get; }
    }
}
