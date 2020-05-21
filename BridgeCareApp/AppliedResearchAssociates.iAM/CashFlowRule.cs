using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class CashFlowRule : IValidator
    {
        public Criterion Criterion { get; } = new Criterion();

        public ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (string.IsNullOrWhiteSpace(Name))
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, Name, "Name is blank."));
                }

                if (DistributionRules.Count == 0)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, DistributionRules, "There are no distribution rules."));
                }

                return results;
            }
        }

        public ICollection<CashFlowDistributionRule> DistributionRules { get; } = new ListWithoutNulls<CashFlowDistributionRule>();

        public Box<string> Name { get; } = new Box<string>();

        public ICollection<IValidator> Subvalidators
        {
            get
            {
                var validators = DistributionRules.ToList<IValidator>();
                validators.Add(Criterion);
                return validators;
            }
        }
    }
}
