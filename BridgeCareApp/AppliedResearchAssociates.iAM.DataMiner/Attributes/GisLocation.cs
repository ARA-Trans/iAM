namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
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
