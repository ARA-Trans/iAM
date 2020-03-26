using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    internal class CalculateEvaluateCompilerListener : CalculateEvaluateBaseListener
    {
        public CalculateEvaluateCompilerListener(Dictionary<string, ParameterType> parameters) => Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

        public Expression<Func<CalculationArguments, double>> CalculatorExpression { get; private set; }

        public Expression<Func<EvaluationArguments, bool>> EvaluatorExpression { get; private set; }

        private readonly Dictionary<string, ParameterType> Parameters;
    }
}
