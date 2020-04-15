using System;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public sealed class Equation : CompilableExpression
    {
        public Equation(CalculateEvaluateCompiler compiler) => Compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));

        public double Calculate(CalculateEvaluateArgument argument)
        {
            Prepare();
            return Calculator(argument);
        }

        protected override void Compile() => Calculator = Compiler.GetCalculator(Expression);

        private readonly CalculateEvaluateCompiler Compiler;

        private Calculator Calculator;
    }
}
