using System;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public class Criterion
    {
        public Criterion(CalculateEvaluateCompiler compiler) => Compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));

        public string Expression
        {
            get => _Expression;
            set
            {
                if (_Expression != value)
                {
                    _Expression = value;

                    Evaluator = Compiler.GetEvaluator(_Expression);
                }
            }
        }

        public bool Evaluate(EvaluationArguments arguments) => Evaluator(arguments);

        private readonly CalculateEvaluateCompiler Compiler;

        private string _Expression;

        private Func<EvaluationArguments, bool> Evaluator;
    }
}
