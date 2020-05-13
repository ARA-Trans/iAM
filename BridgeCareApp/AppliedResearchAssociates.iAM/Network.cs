using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Network
    {
        public Network(Explorer explorer) => Explorer = explorer ?? throw new ArgumentNullException(nameof(explorer));

        public Explorer Explorer { get; }

        public string Name { get; set; }

        public ICollection<SectionHistory> Sections { get; } = new List<SectionHistory>();

        public ICollection<Simulation> Simulations { get; } = new List<Simulation>();
    }
}
