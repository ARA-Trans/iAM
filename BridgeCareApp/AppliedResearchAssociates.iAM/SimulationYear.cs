using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class SimulationBudgetDetail
    {
        // TODO
    }

    public sealed class SimulationYear
    {
        //TODO
        public object ConditionGoalsProgress { get; }

        public IDictionary<Budget, SimulationBudgetDetail> DetailPerBudget { get; } = new Dictionary<Budget, SimulationBudgetDetail>();

        public IDictionary<Section, SimulationSectionDetail> DetailPerSection { get; } = new Dictionary<Section, SimulationSectionDetail>();
    }
}
