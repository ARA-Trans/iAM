using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.ScenarioAnalysis
{
    internal sealed class TreatmentProgress
    {
        public Treatment Treatment { get; }

        public object Expenditures { get; } // TODO

        public List<UnconditionalTreatmentConsequence> Consequences { get; }

        public List<TreatmentScheduling> Schedulings { get; }
    }
}
