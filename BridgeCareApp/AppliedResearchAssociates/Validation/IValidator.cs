using System.Collections.Generic;

namespace AppliedResearchAssociates.Validation
{
    public interface IValidator
    {
        ICollection<ValidationResult> DirectValidationResults { get; }

        // TODO: When upgraded to netstandard2.1 (i.e. C# 8), use default implementation "=> new List<IValidator>();".
        ICollection<IValidator> Subvalidators { get; }
    }
}
