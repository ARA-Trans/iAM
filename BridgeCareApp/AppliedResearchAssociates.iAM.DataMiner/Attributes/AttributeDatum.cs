using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class AttributeDatum<T>
    {
        public Location Location { get; }

        public Attribute Attribute { get; }

        protected T Value { get; }

        public AttributeDatum(Attribute attribute, T value, Location location)
        {
            Attribute = attribute;
            Value = value;
            Location = location;
        }
    }
}
