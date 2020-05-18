namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class SimpleRoute : Route
    {
        public string Name { get; }

        public SimpleRoute(string name) : base(name) => Name = name;

        internal override bool Matches(Route route)
        {
            throw new System.NotImplementedException();
        }
    }
}
