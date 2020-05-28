using System;
using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public sealed class NumberFunctionDescription
    {
        public NumberFunctionDescription(string name, IEnumerable<string> parameterNames)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ParameterNames = parameterNames?.ToList() ?? throw new ArgumentNullException(nameof(parameterNames));
        }

        public string Name { get; }

        public IReadOnlyList<string> ParameterNames { get; }
    }
}
