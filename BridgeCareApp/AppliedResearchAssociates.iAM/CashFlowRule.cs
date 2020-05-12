using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CashFlowRule
    {
        public Criterion Criterion { get; } = new Criterion();

        public List<CashFlowDistributionRule> DistributionRules { get; } = new List<CashFlowDistributionRule>();

        public string Name { get; set; }
    }
}
