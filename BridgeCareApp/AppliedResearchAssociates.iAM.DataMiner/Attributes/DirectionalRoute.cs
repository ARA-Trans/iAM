namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class DirectionalRoute : Route
    {
        public string Name { get; }

        public Direction Direction { get; }

        public DirectionalRoute(string name, Direction direction, string uniqueIdentifier) : base(uniqueIdentifier)
        {
            Name = name;
            Direction = direction;
        }
    }
}
