using System.Collections.Generic;

namespace AppliedResearchAssociates.Validation
{
    public static class Validation
    {
        public static void AddResult<T>(this ICollection<ValidationResult> results, ValidationStatus status, T context, string description) where T : class => results.Add(ValidationResult.Create(status, context, description));

        public static ICollection<ValidationResult> GetAllValidationResults(this IValidator validator)
        {
            var results = new ListWithoutNulls<ValidationResult>();

            var visited = new HashSet<IValidator>();
            var queue = new Queue<IValidator>();
            queue.Enqueue(validator);
            while (queue.Count > 0)
            {
                validator = queue.Dequeue();
                if (visited.Add(validator))
                {
                    results.AddMany(validator.DirectValidationResults);
                    foreach (var subvalidator in validator.Subvalidators)
                    {
                        queue.Enqueue(subvalidator);
                    }
                }
            }

            return results;
        }
    }
}
