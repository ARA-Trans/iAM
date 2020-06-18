namespace AppliedResearchAssociates.iAM.DataMiner
{
    public abstract class Location
    {
        public Location(string uniqueIdentifier)
        {
            UniqueIdentifier = uniqueIdentifier;
        }
        public string UniqueIdentifier { get; }
        public abstract bool MatchOn(Location location);
    }
}
