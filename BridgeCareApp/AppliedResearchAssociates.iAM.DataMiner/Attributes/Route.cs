namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class Route
    {
        public string Name { get; }

        public Route(string name) => Name = name;

        // Determines if two routes match in a comparison.
        internal abstract bool MatchOn(Route route);
    }
}
