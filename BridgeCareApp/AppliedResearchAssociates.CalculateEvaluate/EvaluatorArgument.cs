using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class EvaluatorArgument<T> : CalculatorArgument<T>
    {
        public IDictionary<string, DateTime> Dates { get; } = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        public IDictionary<string, string> Strings { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
