using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public sealed class CalculateEvaluateArgument
    {
        public IDictionary<string, DateTime> Dates { get; } = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        public IDictionary<string, double> Numbers { get; } = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        public IDictionary<string, string> Strings { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
