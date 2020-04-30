using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Network
    {
        //- Needs to have a representation of segmentation & roll-up. Is "roll-forward" the same thing as roll-up?

        public Explorer Explorer { get; }

        public string Name { get; }

        public List<Section> Sections { get; }

        public List<Simulation> Simulations { get; }
    }
}
