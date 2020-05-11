using System;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal sealed class TreatmentProgress
    {
        public TreatmentProgress(Treatment treatment, double cost, bool isComplete)
        {
            Treatment = treatment ?? throw new ArgumentNullException(nameof(treatment));
            Cost = cost;
            IsComplete = isComplete;
        }

        public double Cost { get; }

        public bool IsComplete { get; }

        public Treatment Treatment { get; }
    }
}
