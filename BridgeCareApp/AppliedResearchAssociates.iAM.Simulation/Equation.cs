using System;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public sealed class Equation : CompilableExpression
    {
        public Equation(CalculateEvaluateCompiler compiler) => Compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));

        public double Calculate(CalculateEvaluateArgument argument)
        {
            if (!IsCompiled)
            {
                Calculator = Compiler.GetCalculator(Expression);
                IsCompiled = true;
            }

            return Calculator(argument);
        }

        private readonly CalculateEvaluateCompiler Compiler;

        private Calculator Calculator;
    }
}
