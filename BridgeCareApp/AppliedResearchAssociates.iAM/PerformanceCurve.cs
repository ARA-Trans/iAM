using System;
using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class PerformanceCurve : IValidator
    {
        public PerformanceCurve(Explorer explorer)
        {
            if (explorer is null)
            {
                throw new ArgumentNullException(nameof(explorer));
            }

            Criterion = new Criterion(explorer);
            Equation = new Equation(explorer);
        }

        public NumberAttribute Attribute { get; set; }

        public Criterion Criterion { get; }

        public Equation Equation { get; }

        public string Name { get; set; }

        public bool Shift { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag { Criterion, Equation };

        public ValidationResultBag GetDirectValidationResults()
        {
            var results = new ValidationResultBag();

            if (Attribute == null)
            {
                results.Add(ValidationStatus.Error, "Attribute is unset.", this, nameof(Attribute));
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                results.Add(ValidationStatus.Warning, "Name is blank.", this, nameof(Name));
            }

            return results;
        }
    }
}
