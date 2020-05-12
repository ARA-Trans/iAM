using System;

namespace AppliedResearchAssociates.Validation
{
    [Serializable]
    public class ValidationResult
    {
        public ValidationResult(ValidationStatus status, string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description is blank.", nameof(description));
            }

            Status = status;
            Description = description;
        }

        public string Description { get; }

        public ValidationStatus Status { get; }

        public override string ToString() => $"[{Status}] {Description}";
    }
}
