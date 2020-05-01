using System;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal sealed class TreatmentOutlookSummary
    {
        public TreatmentOutlookSummary(SectionContext context, Treatment candidateTreatment, double costPerUnitArea, double benefit, double? remainingLife)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            CandidateTreatment = candidateTreatment ?? throw new ArgumentNullException(nameof(candidateTreatment));
            CostPerUnitArea = costPerUnitArea;
            Benefit = benefit;
            RemainingLife = remainingLife;
        }

        public SectionContext Context { get; }

        public double Benefit { get; }

        public double CostPerUnitArea { get; }

        public Treatment CandidateTreatment { get; }

        public double? RemainingLife { get; }
    }
}
