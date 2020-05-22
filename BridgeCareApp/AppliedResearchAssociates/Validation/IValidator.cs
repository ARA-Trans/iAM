namespace AppliedResearchAssociates.Validation
{
    public interface IValidator
    {
        ValidationResultBag DirectValidationResults { get; }

        ValidatorBag Subvalidators { get; }
    }
}
