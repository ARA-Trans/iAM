using System;

namespace AppliedResearchAssociates.Validation
{
    public sealed class ValidationTarget
    {
        public ValidationTarget(object targetObject, string targetKey)
        {
            Object = targetObject ?? throw new ArgumentNullException(nameof(targetObject));
            Key = targetKey?.Trim() ?? "";
        }

        public string Key { get; }

        public object Object { get; }

        public ValidationResult CreateResult(ValidationStatus status, string message) => new ValidationResult(this, status, message);

        public override bool Equals(object obj) => obj is ValidationTarget target && Object == target.Object && Key == target.Key;

        public override int GetHashCode() => HashCode.Combine(Object, Key);
    }
}
