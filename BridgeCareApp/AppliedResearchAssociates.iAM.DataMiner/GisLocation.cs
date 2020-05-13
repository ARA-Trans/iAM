namespace AppliedResearchAssociates.iAM.DataMiner
{
    public class GisLocation : Location
    {
        public GisLocation(string uniqueIdentifier, string wellKnownTextString)
        {
            UniqueIdentifier = uniqueIdentifier;
            WellKnownTextString = wellKnownTextString;
        }

        public string UniqueIdentifier { get; }

        public string WellKnownTextString { get; }
    }
}
