using System;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class SectionContext
    {
        public SectionContext(Section section, CalculateEvaluateArgument data)
        {
            Section = section ?? throw new ArgumentNullException(nameof(section));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public CalculateEvaluateArgument Data { get; }

        public Section Section { get; }
    }
}
