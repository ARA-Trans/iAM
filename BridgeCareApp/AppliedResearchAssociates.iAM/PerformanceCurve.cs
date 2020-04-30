namespace AppliedResearchAssociates.iAM
{
    public sealed class PerformanceCurve
    {
        public NumberAttribute Attribute { get; }

        public Criterion Criterion { get; }

        public Equation Equation { get; }

        public string Name { get; }

        public bool Shift { get; }
    }
}
