using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal sealed class TreatmentActivity
    {
        public TreatmentActivity(Treatment treatment, double cost)
        {
            Treatment = treatment ?? throw new ArgumentNullException(nameof(treatment));
            Cost = cost;
        }

        public ICollection<UnconditionalTreatmentConsequence> Consequences { get; } = new List<UnconditionalTreatmentConsequence>();

        public double Cost { get; }

        public ICollection<TreatmentScheduling> Schedulings { get; } = new List<TreatmentScheduling>();

        public Treatment Treatment { get; }
    }
}
