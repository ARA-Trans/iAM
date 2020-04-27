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
            TotalCostPerUnitArea += costPerUnitArea;
        }

        public void AccumulateBenefit() => TotalBenefit += SectionContext.GetBenefit();

        public double TotalBenefit { get; set; }

        public double TotalCostPerUnitArea { get; set; }
    }
}
