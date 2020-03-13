using System;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Criterion
    {
        public string Expression { get; }

        public Func<object[], bool> Evaluate { get; }
    }
}
