namespace BridgeCare.Models
{
    public class EquationValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }

        public EquationValidationResult() {}

        public EquationValidationResult(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }
    }
}
