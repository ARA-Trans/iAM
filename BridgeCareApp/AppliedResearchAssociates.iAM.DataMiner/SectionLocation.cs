namespace AppliedResearchAssociates.iAM.DataMiner
{
    public class SectionLocation : Location
    {
        public SectionLocation(string uniqueIdentifier) => UniqueIdentifier = uniqueIdentifier;

        public string UniqueIdentifier { get; }

        public override bool MatchOn(Location location)
        {
            throw new System.NotImplementedException();
        }
    }
}
