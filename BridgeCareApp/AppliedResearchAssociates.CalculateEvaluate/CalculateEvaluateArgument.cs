using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public sealed class CalculateEvaluateArgument
    {
        public IDictionary<string, double> Number { get; } = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        public IDictionary<string, string> Text { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public IDictionary<string, DateTime> Timestamp { get; } = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
    }
}
