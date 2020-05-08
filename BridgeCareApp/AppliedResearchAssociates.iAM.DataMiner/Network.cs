using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Segmentation;

namespace AppliedResearchAssociates.iAM.DataMiner.NetworkDefinition
{
    public class Network
    {
        public Network(List<Location> locations)
        {
            Locations = locations;
        }

        public List<Location> Locations { get; }

        public void Segment(Segmenter segmenter)
        {

        }
    }
}
