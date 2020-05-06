using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CashFlowRule
    {
        public string Name { get; }
        public Criterion Criterion { get; }
        public List<CashFlowDistributionRule> DistributionRules { get; }
    }
}
