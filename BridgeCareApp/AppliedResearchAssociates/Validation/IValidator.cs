using System.Collections.Generic;

namespace AppliedResearchAssociates.Validation
{
    public interface IValidator
    {
        ICollection<ValidationResult> ValidationResults { get; }
    }
}
