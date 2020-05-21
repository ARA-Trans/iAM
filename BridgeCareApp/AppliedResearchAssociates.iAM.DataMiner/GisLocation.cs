namespace AppliedResearchAssociates.iAM.DataMiner
{
    public class GisLocation : Location
    {
        public GisLocation(string wellKnownTextString)
        {
            WellKnownTextString = wellKnownTextString;
        }

        public string WellKnownTextString { get; }

        public override bool MatchOn(Location location)
        {
            throw new System.NotImplementedException();
        }
    }
}
