using System;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public sealed class Criterion : CompilableExpression
    {
        public Criterion(CalculateEvaluateCompiler compiler) => Compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));

        public bool Evaluate(CalculateEvaluateArgument argument)
        {
            if (!IsCompiled)
            {
                Evaluator = Compiler.GetEvaluator(Expression);
                IsCompiled = true;
            }

            return Evaluator(argument);
        }

        private readonly CalculateEvaluateCompiler Compiler;

        private Evaluator Evaluator;
    }
}
