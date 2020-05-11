using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.NetworkDefinition
{
    public class Network
    {
        public Network(List<Location> locations)
        {
            Locations = locations;
        }

        public List<Location> Locations { get; }
    }
}
