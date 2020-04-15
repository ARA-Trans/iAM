namespace AppliedResearchAssociates.iAM.Simulation
{
    public sealed class NumberAttribute : Attribute<double>
    {
        public double Maximum { get; }

        public double Minimum { get; }
    }
}
