using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMiner
{
    public abstract class Location
    {
        public abstract bool MatchOn(Location location);
    }
}
