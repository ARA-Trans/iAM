using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class CalculationArguments
    {
        public Dictionary<string, double> Numbers { get; } = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
    }
}
