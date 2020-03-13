using System;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Section
    {
        public Facility Facility { get; }

        public string Label { get; }

        internal void ApplyDeteriorate(PerformanceCurve curve, int year)
        {
            if (curve.ConditionalEquation.Criterion.Evaluate(null))
            {

            }
        }
    }
}
