using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CashFlowRule : IValidator
    {
        public Criterion Criterion { get; } = new Criterion();

        public ICollection<CashFlowDistributionRule> DistributionRules { get; } = new CollectionWithoutNulls<CashFlowDistributionRule>();

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

                if (DistributionRules.Count == 0)
                {
                    results.Add(ValidationStatus.Error.Describe("There are no distribution rules."));
                }

                return results;
            }
        }
    }
}
