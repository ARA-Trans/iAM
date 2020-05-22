using System;

namespace AppliedResearchAssociates.iAM.Analysis
{
    internal sealed class TreatmentOption
    {
        public TreatmentOption(SectionContext context, SelectableTreatment candidateTreatment, double costPerUnitArea, double benefit, double? remainingLife)
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

        public TreatmentOptionDetail Detail => new TreatmentOptionDetail(CandidateTreatment.Name, CostPerUnitArea, Benefit, RemainingLife);

        public double? RemainingLife { get; }
    }
}
