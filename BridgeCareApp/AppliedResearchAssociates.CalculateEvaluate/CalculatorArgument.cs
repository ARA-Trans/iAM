using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class CalculatorArgument<T>
    {
        public IDictionary<string, T> Numbers { get; } = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
    }
}
