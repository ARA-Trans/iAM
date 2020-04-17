using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Section
    {
        public double Area { get; } // isn't this "just another attribute"

        public CalculateEvaluateArgument Data { get; }

        public Facility Facility { get; }

        public string Label { get; }
    }
}
