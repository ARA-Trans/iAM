namespace AppliedResearchAssociates.iAM.DataMiner
{
    public class SectionLocation : Location
    {
        public SectionLocation(string uniqueIdentifier) : base(uniqueIdentifier)
        {

        }

        public override bool MatchOn(Location location)
        {
            if (location is SectionLocation sectionLocation)
            {
                return sectionLocation.UniqueIdentifier == UniqueIdentifier;
            }
            else
            {
                return false;
            }
        }
    }
}
