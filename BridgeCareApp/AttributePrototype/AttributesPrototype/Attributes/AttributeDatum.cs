using System;
using System.Collections.Generic;
using System.Text;

namespace AttributesPrototype.Attributes
{
    public class AttributeDatum<T>
    {
        public Location Location { get; }

        public AttributeDatum(T value, Location location)
        {
            Value = value;
            Location = location;
        }
        public T Value { get; }
    }
}
