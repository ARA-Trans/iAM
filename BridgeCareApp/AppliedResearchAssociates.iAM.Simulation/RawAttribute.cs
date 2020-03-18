using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class RawAttribute
    {
        public string Name { get; }

        public List<RawAttributeDatum> Data { get; }

        // other things, too, like min, max, default, etc.
    }
}
