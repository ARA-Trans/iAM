using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
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
