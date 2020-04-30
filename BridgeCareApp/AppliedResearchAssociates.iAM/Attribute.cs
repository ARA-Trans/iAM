using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Attribute
    {
        public string Name { get; }
    }

    public class Attribute<T> : Attribute
    {
        public List<AttributeDatum<T>> Data { get; }

        public T DefaultValue { get; }
    }
}
