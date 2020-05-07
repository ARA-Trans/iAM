using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class Route
    {
        public string UniqueIdentifier { get; }

        public Route(string uniqueIdentifier) => UniqueIdentifier = uniqueIdentifier;
    }
}
