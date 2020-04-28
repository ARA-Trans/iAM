using Antlr4.Runtime;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class CalculateEvaluateBailingLexer : CalculateEvaluateLexer
    {
        public CalculateEvaluateBailingLexer(ICharStream input) : base(input) => RemoveErrorListeners();

        public override void Recover(LexerNoViableAltException e) => throw new CalculateEvaluateLexingException(null, e);
    }
}
