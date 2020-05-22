using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class PerformanceCurve : IValidator
    {
        public NumberAttribute Attribute { get; set; }

        public Criterion Criterion { get; } = new Criterion();

        public ValidationResultBag DirectValidationResults
        {
            get
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

        public Equation Equation { get; } = new Equation();

        public string Name { get; set; }

        public bool Shift { get; set; }

        public ValidatorBag Subvalidators => new ValidatorBag { Criterion, Equation };
    }
}
