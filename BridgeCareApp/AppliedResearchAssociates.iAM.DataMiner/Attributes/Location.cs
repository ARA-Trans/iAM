using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
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
