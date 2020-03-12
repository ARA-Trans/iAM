namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Project
    {
        public int FirstYear { get; }

        // >= FirstYear
        public int LastYear { get; }

        public Section Section { get; }

        public Treatment Treatment { get; }
    }
}
