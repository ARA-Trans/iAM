using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class PerformanceAttribute
    {
        // This member makes this type "historical", and the simulation might warrant a separate
        // "evolving" type.
        public List<PerformanceAttributeDatum> Data { get; }

        public string Name { get; }
    }
}
