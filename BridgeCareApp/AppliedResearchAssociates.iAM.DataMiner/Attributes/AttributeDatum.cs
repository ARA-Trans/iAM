using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class AttributeDatum<T>
    {
        public Location Location { get; }
        public Attribute Attribute { get; }

        public AttributeDatum(Attribute attribute, T value, Location location)
        {
            Attribute = attribute;
            Value = value;
            Location = location;
        }
        public T Value { get; }
    }
}
