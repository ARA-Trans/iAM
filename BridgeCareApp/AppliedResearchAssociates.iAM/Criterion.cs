using System;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM
{
    public sealed class Criterion : CompilableExpression
    {
        public Criterion(CalculateEvaluateCompiler compiler) => Compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));

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

        private readonly CalculateEvaluateCompiler Compiler;

        private Evaluator Evaluator;
    }
}
