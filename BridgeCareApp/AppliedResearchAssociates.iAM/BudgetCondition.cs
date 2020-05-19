using System.Collections.Generic;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class BudgetCondition : IValidator
    {
        public Budget Budget { get; set; }

        public Criterion Criterion { get; } = new Criterion();

        public ICollection<ValidationResult> ValidationResults
        {
            get
            {
                var results = new List<ValidationResult>();

                if (Budget == null)
                {
                    results.Add(ValidationStatus.Error.Describe("Budget is unset."));
                }

                return results;
            }
        }
    }
}
