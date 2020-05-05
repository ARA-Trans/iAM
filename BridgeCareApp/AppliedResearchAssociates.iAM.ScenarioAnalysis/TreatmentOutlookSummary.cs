using System;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal sealed class TreatmentOutlookSummary
    {
        public TreatmentOutlookSummary(SectionContext context, SelectableTreatment candidateTreatment, double costPerUnitArea, double benefit, double? remainingLife)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            CandidateTreatment = candidateTreatment ?? throw new ArgumentNullException(nameof(candidateTreatment));
            CostPerUnitArea = costPerUnitArea;
            Benefit = benefit;
            RemainingLife = remainingLife;
        }

        public double Benefit { get; }

        public SelectableTreatment CandidateTreatment { get; }

        public SectionContext Context { get; }

        public double CostPerUnitArea { get; }

        public FeasibleTreatmentSummary FeasibleTreatmentSummary => new FeasibleTreatmentSummary(CostPerUnitArea, Benefit, RemainingLife);

        public double? RemainingLife { get; }
    }
}
