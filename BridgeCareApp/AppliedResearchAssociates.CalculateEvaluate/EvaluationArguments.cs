using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class EvaluationArguments
    {
        public Dictionary<string, DateTime> Dates { get; } = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, double> Numbers { get; } = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, string> Strings { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
