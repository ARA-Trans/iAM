using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Criterion : AttributeExpression
    {
        public bool? Evaluate(CalculateEvaluateArgument argument)
        {
            EnsureCompiled();
            return Evaluator?.Invoke(argument);
        }

        protected override void Compile()
        {
            if (ExpressionIsBlank)
            {
                Evaluator = null;
            }
            else
            {
                try
                {
                    Evaluator = Explorer.Compiler.GetEvaluator(Expression);
                }
                catch (CalculateEvaluateException e)
                {
                    throw ExpressionCouldNotBeCompiled(e);
                }
            }
        }

        private Evaluator Evaluator;
    }
}
