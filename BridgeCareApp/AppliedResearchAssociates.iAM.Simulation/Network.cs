using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Network
    {
        public string Name { get; }

        public List<Simulation> Simulations { get; } = new List<Simulation>();
    }
}
