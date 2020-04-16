namespace AppliedResearchAssociates.iAM.Simulation
{
    public sealed class NumberAttribute : Attribute<double>
    {
        public Deterioration Deterioration { get; }

        public double? Maximum { get; }

        public double? Miniumum { get; }
    }
}
