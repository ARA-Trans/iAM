using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class EvaluationArguments
    {
        public Dictionary<string, DateTime> Dates { get; } = new Dictionary<string, DateTime>();

        public Dictionary<string, double> Numbers { get; } = new Dictionary<string, double>();

        public Dictionary<string, string> Strings { get; } = new Dictionary<string, string>();
    }
}
