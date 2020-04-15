using System;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public sealed class Criterion : CompilableExpression
    {
        public Criterion(CalculateEvaluateCompiler compiler) => Compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));

        public bool Evaluate(CalculateEvaluateArgument argument)
        {
            Prepare();
            return Evaluator(argument);
        }

        protected override void Compile() => Evaluator = Compiler.GetEvaluator(Expression);

        private readonly CalculateEvaluateCompiler Compiler;

        private Evaluator Evaluator;
    }
}
