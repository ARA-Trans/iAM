using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal sealed class TreatmentProgress
    {
        public Treatment Treatment { get; }

        public List<UnconditionalTreatmentConsequence> Consequences { get; } = new List<UnconditionalTreatmentConsequence>();
    }
}
