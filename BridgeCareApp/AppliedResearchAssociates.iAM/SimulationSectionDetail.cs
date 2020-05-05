using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class SimulationSectionDetail
    {
        public SimulationSectionDetail(Treatment appliedTreatment, TreatmentReason reasonForAppliedTreatment)
        {
            AppliedTreatment = appliedTreatment ?? throw new ArgumentNullException(nameof(appliedTreatment));
            ReasonForAppliedTreatment = reasonForAppliedTreatment;
        }

        public Treatment AppliedTreatment { get; }

        public IDictionary<NumberAttribute, double> Number { get; } = new Dictionary<NumberAttribute, double>();

        public TreatmentReason ReasonForAppliedTreatment { get; }

        public IDictionary<SelectableTreatment, FeasibleTreatmentSummary> SummaryPerFeasibleTreatment { get; } = new Dictionary<SelectableTreatment, FeasibleTreatmentSummary>();

        public IDictionary<TextAttribute, string> Text { get; } = new Dictionary<TextAttribute, string>();
    }
}
