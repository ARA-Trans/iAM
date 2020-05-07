using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CashFlowRule
    {
        public Criterion Criterion { get; }

        public List<CashFlowDistributionRule> DistributionRules { get; }

        public string Name { get; }
    }
}
