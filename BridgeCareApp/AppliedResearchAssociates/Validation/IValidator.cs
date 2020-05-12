using System.Collections.Generic;

namespace AppliedResearchAssociates.Validation
{
    public interface IValidator
    {
        IEnumerable<ValidationResult> ValidationResults { get; }
    }
}
