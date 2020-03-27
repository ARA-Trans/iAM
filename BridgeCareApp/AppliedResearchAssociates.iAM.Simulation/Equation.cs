using System;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Equation
    {
        public Equation(CalculateEvaluateCompiler compiler) => Compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));

        public string Expression
        {
            get => _Expression;
            set
            {
                if (_Expression != value)
                {
                    Calculator = Compiler.GetCalculator(_Expression = value);
                }
            }
        }

        public double Calculate(CalculationArguments arguments) => Calculator(arguments);

        private readonly CalculateEvaluateCompiler Compiler;

        private string _Expression;

        private Func<CalculationArguments, double> Calculator;
    }
}
