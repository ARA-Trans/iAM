using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class PerformanceAttribute
    {
        // This member makes this type "historical", and the simulation might warrant a separate
        // "evolving" type.
        public List<PerformanceAttributeDatum> Data { get; } = new List<PerformanceAttributeDatum>();

        public string Name { get; }
    }
}
