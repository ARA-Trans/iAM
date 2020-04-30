using Antlr4.Runtime;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    internal sealed class CalculateEvaluateBailingParser : CalculateEvaluateParser
    {
        public CalculateEvaluateBailingParser(ITokenStream input) : base(input)
        {
            ErrorHandler = new BailErrorStrategy();
            RemoveErrorListeners();
        }
    }
}
