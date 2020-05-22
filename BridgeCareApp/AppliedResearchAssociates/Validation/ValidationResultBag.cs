using System.Collections;
using System.Collections.Generic;

namespace AppliedResearchAssociates.Validation
{
    public sealed class ValidationResultBag : IReadOnlyCollection<ValidationResult>
    {
        public int Count => ((IReadOnlyCollection<ValidationResult>)Results).Count;

        public void Add(ValidationStatus status, string message, object target, string key = null) => Results.Add(new ValidationTarget(target, key).CreateResult(status, message));

        public void Add(IEnumerable<ValidationResult> results) => Results.UnionWith(results);

        public IEnumerator<ValidationResult> GetEnumerator() => ((IEnumerable<ValidationResult>)Results).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Results).GetEnumerator();

        private readonly HashSet<ValidationResult> Results = new HashSet<ValidationResult>();
    }
}
