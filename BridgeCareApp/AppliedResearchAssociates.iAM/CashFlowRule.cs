using System;
using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CashFlowRule : IValidator
    {
        public CashFlowRule(Explorer explorer) => Criterion = new Criterion(explorer ?? throw new ArgumentNullException(nameof(explorer)));

        public Criterion Criterion { get; }

        public ICollection<CashFlowDistributionRule> DistributionRules { get; } = new SetWithoutNulls<CashFlowDistributionRule>();

        public string Name { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag { Criterion, DistributionRules };

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (string.IsNullOrWhiteSpace(Name))
            {
                results.Add(ValidationStatus.Error, "Name is blank.", this, nameof(Name));
            }

            if (DistributionRules.Count == 0)
            {
                results.Add(ValidationStatus.Error, "There are no distribution rules.", this, nameof(DistributionRules));
            }

            return results;
        }
    }
}
