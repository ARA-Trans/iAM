using System;

namespace AppliedResearchAssociates.Validation
{
    public class ValidationResult
    {
        public object Context { get; }

        public string Description { get; }

        public ValidationStatus Status { get; }

        public static ValidationResult Create<T>(ValidationStatus status, T context, string description) where T : class => new ValidationResult(status, context, description);

        public override string ToString() => $"[{Status}] {Description}";

        private ValidationResult(ValidationStatus status, object context, string description)
        {
            Status = status;
            Context = context ?? throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description is blank.", nameof(description));
            }

            Description = description;
        }
    }
}
