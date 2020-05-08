namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class Route
    {
        public string UniqueIdentifier { get; }

        public Route(string uniqueIdentifier) => UniqueIdentifier = uniqueIdentifier;
    }
}
