using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    // TODO: Add timestamp to the attribute datum class.
    public class AttributeDatum<T>
    {
        public Location Location { get; }

        public Attribute Attribute { get; }

        public T Value { get; }

        public DateTime TimeStamp { get; }

        public AttributeDatum(Attribute attribute, T value, Location location, DateTime timeStamp)
        {
            Attribute = attribute;
            Value = value;
            Location = location;
            TimeStamp = timeStamp;
        }
    }
}
