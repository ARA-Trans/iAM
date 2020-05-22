using System.Collections.Generic;

namespace AppliedResearchAssociates.Validation
{
    public static class Validation
    {
        public static ValidationResultBag GetAllValidationResults(this IValidator validator)
        {
            var results = new ValidationResultBag();

            var visited = new HashSet<IValidator>();
            var queue = new Queue<IValidator>();
            queue.Enqueue(validator);
            while (queue.Count > 0)
            {
                validator = queue.Dequeue();
                if (validator != null && visited.Add(validator))
                {
                    results.Add(validator.DirectValidationResults);
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
