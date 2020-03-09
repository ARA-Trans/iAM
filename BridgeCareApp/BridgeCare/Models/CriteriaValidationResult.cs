namespace BridgeCare.Models
{
    public class CriteriaValidationResult
    {
        public bool IsValid { get; set; }
        public int NumberOfResults { get; set; }
        public string Message { get; set; }

        public CriteriaValidationResult() {}

        public CriteriaValidationResult(bool isValid, int numberOfResults, string message)
        {
            IsValid = isValid;
            NumberOfResults = numberOfResults;
            Message = message;
        }
    }
}
