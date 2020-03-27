using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    internal class CalculateEvaluateCompilerListener : CalculateEvaluateBaseListener
    {
        public CalculateEvaluateCompilerListener(Dictionary<string, Type> parameters) => Parameters = new Dictionary<string, Type>(parameters, parameters.Comparer);

        public Func<CalculationArguments, double> Calculator { get; private set; }

        public Func<EvaluationArguments, bool> Evaluator { get; private set; }

        private readonly IReadOnlyDictionary<string, Type> Parameters;
    }
}
