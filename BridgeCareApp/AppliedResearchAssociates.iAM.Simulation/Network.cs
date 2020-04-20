using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Network
    {
        public Explorer Explorer { get; }

        public string Name { get; }

        public List<Section> Sections { get; }

        public List<Simulation> Simulations { get; }
    }
}
