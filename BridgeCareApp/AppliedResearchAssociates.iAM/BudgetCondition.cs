using System;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetCondition : IValidator
    {
        public BudgetCondition(Explorer explorer) => Criterion = new Criterion(explorer ?? throw new ArgumentNullException(nameof(explorer)));

        public Budget Budget { get; set; }

        public Criterion Criterion { get; }

        public ValidatorBag Subvalidators => new ValidatorBag { Criterion };

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (Budget == null)
            {
                results.Add(ValidationStatus.Error, "Budget is unset.", this, nameof(Budget));
            }

            return results;
        }
    }
}
