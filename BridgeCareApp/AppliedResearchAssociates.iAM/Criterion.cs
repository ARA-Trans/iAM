using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Criterion : CompilableExpression
    {
        public CalculateEvaluateCompiler Compiler { get; set; }

        public bool? Evaluate(CalculateEvaluateArgument argument)
        {
            if (string.IsNullOrWhiteSpace(Expression))
            {
                return null;
            }

            EnsureCompiled();
            return Evaluator(argument);
        }

        protected override void Compile() => Evaluator = Compiler.GetEvaluator(Expression);

        private Evaluator Evaluator;
    }
}
