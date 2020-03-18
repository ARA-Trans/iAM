namespace AppliedResearchAssociates.iAM.Simulation
{
    public class PerformanceCurve
    {
        public RawAttribute Attribute { get; }

        public Criterion Criterion { get; }

        // Input to this is all attribute values of previous year plus (probably) current age.
        public Equation Equation { get; }

        public string Name { get; }

        public bool Shift { get; } // ???
    }
}
