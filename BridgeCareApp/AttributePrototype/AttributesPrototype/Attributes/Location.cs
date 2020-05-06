using System;
using System.Collections.Generic;
using System.Text;

namespace AttributesPrototype.Attributes
{
    public abstract class Location
    {
        public Location(string uniqueIdentifier)
        {
            UniqueIdentifier = uniqueIdentifier;
        }
        protected string UniqueIdentifier { get; }
    }
}
