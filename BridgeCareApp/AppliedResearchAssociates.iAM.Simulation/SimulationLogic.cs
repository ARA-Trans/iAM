using System;

namespace AppliedResearchAssociates.iAM.Simulation
{
    internal static class SimulationLogic
    {
        public static void ApplyDeteriorate(this Section section, PerformanceCurve curve, int year)
        {
            object[] allValuesFromPreviousYear = null;

            if (curve.ConditionalEquation.Criterion.Evaluate(allValuesFromPreviousYear))
            {
                var value = curve.IterateOneYear(allValuesFromPreviousYear, out var isOutOfRange);

                //...
            }
        }

        public static double IterateOneYear(this PerformanceCurve curve, object[] allValuesFromPreviousYear, out bool isOutOfRange)
        {
            throw new NotImplementedException();
        }
    }
}
