using System;

namespace AppliedResearchAssociates.iAM
{
    internal sealed class TreatmentProgress
    {
        public TreatmentProgress(Treatment treatment, decimal cost)
        {
            Treatment = treatment ?? throw new ArgumentNullException(nameof(treatment));
            Cost = cost;
        }

        public decimal Cost { get; }

        public bool IsComplete { get; set; }

        public Treatment Treatment { get; }
    }
}
