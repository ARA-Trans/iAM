using System;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class TreatmentOutlook
    {
        public TreatmentOutlook(SectionContext sectionContext) => SectionContext = sectionContext ?? throw new ArgumentNullException(nameof(sectionContext));

        public SectionContext SectionContext { get; }

        public void ApplyTreatment(Treatment treatment, int year)
        {
            SectionContext.ApplyTreatment(treatment, year, out var costPerUnitArea);
            CumulativeCostPerUnitArea += costPerUnitArea;
        }

        public void AccumulateBenefit() => CumulativeBenefit += SectionContext.GetBenefit();

        public double CumulativeBenefit { get; private set; }

        public double CumulativeCostPerUnitArea { get; private set; }
    }
}
