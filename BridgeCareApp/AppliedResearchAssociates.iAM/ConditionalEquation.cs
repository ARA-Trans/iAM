using AppliedResearchAssociates.Validation;

namespace AppliedResearchAssociates.iAM
{
    public sealed class ConditionalEquation : IValidator
    {
        public Criterion Criterion { get; } = new Criterion();

        public ValidationResultBag DirectValidationResults => new ValidationResultBag();

        public Equation Equation { get; } = new Equation();

        public ValidatorBag Subvalidators => new ValidatorBag { Criterion, Equation };
    }
}
