using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetCondition : IValidator
    {
        public Box<Budget> Budget { get; } = new Box<Budget>();

        public Criterion Criterion { get; } = new Criterion();

        public ICollection<ValidationResult> DirectValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (Budget.Value == null)
                {
                    results.Add(ValidationResult.Create(ValidationStatus.Error, Budget, "Budget is unset."));
                }

                return results;
            }
        }

        public ICollection<IValidator> Subvalidators => new List<IValidator>();
    }
}
