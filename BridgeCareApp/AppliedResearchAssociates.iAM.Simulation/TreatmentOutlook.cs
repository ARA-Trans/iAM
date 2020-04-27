using System;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class TreatmentOutlook
    {
        public TreatmentOutlook(SectionContext sectionContext) => SectionContext = sectionContext ?? throw new ArgumentNullException(nameof(sectionContext));

        public SectionContext SectionContext { get; }

        public double TotalBenefit { get; set; }

        public double TotalCost { get; set; }
    }
}
