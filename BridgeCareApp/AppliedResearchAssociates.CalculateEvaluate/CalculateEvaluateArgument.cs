using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.CalculateEvaluate
{
    public sealed class CalculateEvaluateArgument
    {
        public double GetNumber(string key) => Number[key]();

        public string GetText(string key) => Text[key];

        public DateTime GetTimestamp(string key) => Timestamp[key];

        public void SetNumber(string key, double value) => SetNumber(key, () => value);

        public void SetNumber(string key, Func<double> getValue) => Number[key] = getValue;

        public void SetText(string key, string value) => Text[key] = value;

        public void SetTimestamp(string key, DateTime value) => Timestamp[key] = value;

        private readonly IDictionary<string, Func<double>> Number = new Dictionary<string, Func<double>>(StringComparer.OrdinalIgnoreCase);

        private readonly IDictionary<string, string> Text = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private readonly IDictionary<string, DateTime> Timestamp = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
    }
}
