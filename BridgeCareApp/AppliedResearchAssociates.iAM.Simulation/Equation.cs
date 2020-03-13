using System;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Equation
    {
        public string Expression { get; }

        public Func<double[], double> Calculate { get; }
    }
}
