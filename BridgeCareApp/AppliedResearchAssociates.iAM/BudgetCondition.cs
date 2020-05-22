using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetCondition : IValidator
    {
        public Budget Budget { get; set; }

        public Criterion Criterion { get; } = new Criterion();

        public ValidationResultBag DirectValidationResults
        {
            get
            {
                var results = new ValidationResultBag();

                if (Budget == null)
                {
                    results.Add(ValidationStatus.Error, "Budget is unset.", this, nameof(Budget));
                }

                return results;
            }
        }

        public ValidatorBag Subvalidators => new ValidatorBag { Criterion };
    }
}
