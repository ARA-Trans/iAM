using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.Validation
{
    public static class Validation
    {
        public static ValidationResult Describe(this ValidationStatus status, string description) => new ValidationResult(status, description);

        public static ValidationContext Describe(this IEnumerable<ValidationResult> results, string description) => new ValidationContext(description, results);

        public static ValidationException ToException(this ValidationContext context, string message = null, Exception innerException = null) => new ValidationException(message, innerException) { Context = context };
    }
}
