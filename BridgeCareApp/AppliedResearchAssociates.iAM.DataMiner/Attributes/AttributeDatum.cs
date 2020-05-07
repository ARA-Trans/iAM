using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class AttributeDatum<T>
    {
        public Location Location { get; }
        public Attribute Attribute { get; }
        public T Default { get; }

        public AttributeDatum(Attribute attribute, T value, T _default, Location location)
        {
            Attribute = attribute;
            Value = value;
            Default = _default;
            Location = location;
        }
        public T Value { get; }
    }
}
