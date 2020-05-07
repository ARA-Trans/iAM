using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class SimpleRoute : Route
    {
        public string Name { get; }
        public SimpleRoute(string name) : base(name) => Name = name;
    }
}
