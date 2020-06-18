namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class DirectionalRoute : Route
    {
        public Direction Direction { get; }

        public DirectionalRoute(string name, Direction direction) : base(name)
        {
            Direction = direction;
        }

        internal override bool MatchOn(Route route)
        {
            var directionalRoute = (DirectionalRoute)route;
            return ((Name == directionalRoute.Name &&
                Direction == directionalRoute.Direction) ||
                Name == directionalRoute.Name);
        }
    }
}
