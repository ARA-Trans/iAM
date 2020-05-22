using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.Validation
{
    public sealed class ValidationTarget
    {
        public ValidationTarget(object targetObject, string targetKey)
        {
            TargetObject = targetObject ?? throw new ArgumentNullException(nameof(targetObject));
            TargetKey = targetKey?.Trim() ?? "";
        }

        public string TargetKey { get; }

        public object TargetObject { get; }

        public ValidationResult CreateResult(ValidationStatus status, string message) => new ValidationResult(this, status, message);

        public override bool Equals(object obj) => obj is ValidationTarget target && EqualityComparer<object>.Default.Equals(TargetObject, target.TargetObject) && TargetKey == target.TargetKey;

        public override int GetHashCode() => HashCode.Combine(TargetObject, TargetKey);
    }
}
