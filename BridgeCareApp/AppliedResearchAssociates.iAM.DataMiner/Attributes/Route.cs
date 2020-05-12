namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class Route
    {
        public string Identifier { get; }

        public Route(string identifier) => Identifier = identifier;
    }
}
