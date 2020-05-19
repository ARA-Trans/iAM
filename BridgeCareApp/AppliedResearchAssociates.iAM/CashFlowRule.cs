using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CashFlowRule : IValidator
    {
        public Criterion Criterion { get; } = new Criterion();

        public ICollection<CashFlowDistributionRule> DistributionRules { get; } = new SortedSet<CashFlowDistributionRule>(DistributionRuleComparer);

        public string Name { get; set; }

        public ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationStatus.Error.Describe("Name is blank."));
                }

                return results;
            }
        }

        private static readonly IComparer<CashFlowDistributionRule> DistributionRuleComparer = SelectionComparer<CashFlowDistributionRule>.Create(distributionRule => distributionRule.CostCeiling ?? decimal.MaxValue);
    }
}
