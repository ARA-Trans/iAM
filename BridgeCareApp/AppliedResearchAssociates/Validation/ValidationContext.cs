using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.Validation
{
    [Serializable]
    public class ValidationContext
    {
        private readonly List<ValidationResult> _Results;

        public ValidationContext(string description, IEnumerable<ValidationResult> results)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description is blank.", nameof(description));
            }

            Description = description;
            _Results = results?.ToList() ?? throw new ArgumentNullException(nameof(results));
        }

        public string Description { get; }

        public ICollection<ValidationResult> Results => _Results;

        public override string ToString() => string.Join(Environment.NewLine + "- ", Results.Prepend<object>(Description));
    }
}
