using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class TextAttribute : Attribute
    {
        public List<AttributeDatum<string>> Data { get; }

        public string Default { get; }
    }
}
