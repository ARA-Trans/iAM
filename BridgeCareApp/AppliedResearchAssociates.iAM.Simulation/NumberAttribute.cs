namespace AppliedResearchAssociates.iAM.Simulation
{
    public sealed class NumberAttribute : Attribute<double>
    {
        public bool IsDecreasingWithDeterioration { get; }

        public double? Maximum { get; }

        public double? Miniumum { get; }
    }
}
