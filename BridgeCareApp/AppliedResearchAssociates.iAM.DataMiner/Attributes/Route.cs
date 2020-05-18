using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class Route
    {
        public string Identifier { get; }

        public Route(string identifier) => Identifier = identifier;

        // Determines if two routes match in a comparison.
        internal abstract bool Matches(Route route);
    }
}
