using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class CalculatedField
    {
        public string Name { get; }

        public List<ConditionalEquation> ConditionalEquations { get; } = new List<ConditionalEquation>();
    }
}
