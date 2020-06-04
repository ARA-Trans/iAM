namespace AppliedResearchAssociates.Validation
{
    public interface IValidator
    {
        ValidatorBag Subvalidators { get; }

        ValidationResultBag GetDirectValidationResults();
    }
}
