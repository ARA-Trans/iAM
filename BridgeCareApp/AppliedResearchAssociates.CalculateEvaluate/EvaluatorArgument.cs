using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public class EvaluatorArgument<T> : CalculatorArgument<T>
    {
        public EvaluatorArgument() : this(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase), new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase))
        {
        }

        public IDictionary<string, DateTime> Dates { get; }

        public IDictionary<string, string> Strings { get; }

        public EvaluatorArgument<U> GetSharingArgument<U>() => new EvaluatorArgument<U>(Strings, Dates);

        private EvaluatorArgument(IDictionary<string, string> strings, IDictionary<string, DateTime> dates)
        {
            Strings = strings;
            Dates = dates;
        }
    }
}
