using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    // [TODO] This output aggregate should probably have value semantics, and so it should not
    // reference any entity (i.e. input) types. But, maybe it's better to outsource "value-ness" to
    // additional "snapshotting" logic?
    public sealed class SimulationYear
    {
        //TODO
        public object ConditionGoalsProgress { get; }

        public IEnumerable<SimulationBudgetDetail> DetailPerBudget { get; }

        public IEnumerable<SimulationSectionDetail> DetailPerSection { get; }
    }
}
