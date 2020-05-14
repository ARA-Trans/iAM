namespace AppliedResearchAssociates.iAM
{
    public sealed class NumberAttribute : Attribute<double>
    {
        public bool IsDecreasingWithDeterioration { get; }

        public double? Maximum { get; }

        public double? Minimum { get; }
    }
}
