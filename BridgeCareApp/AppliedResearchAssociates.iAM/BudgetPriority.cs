using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetPriority
    {
        public IDictionary<Budget, double> BudgetPercentages { get; }

        public Criterion Criterion { get; }

        public int PriorityLevel { get; }

        public int? Year { get; }
    }
}
