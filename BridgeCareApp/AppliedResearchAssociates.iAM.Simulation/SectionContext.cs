using System;
using System.Collections.Generic;
using AppliedResearchAssociates.CalculateEvaluate;

namespace AppliedResearchAssociates.iAM.Simulation
{
    public sealed class SectionContext : CalculateEvaluateArgument
    {
        // An instance of this type is conditionally thread-safe, i.e. multiple threads using only
        // "get" operations is safe, and multiple threads using only "set" operations is safe, but
        // multiple threads concurrently using both "get" and "set" operations could produce
        // inconsistent "get" results.

        public SectionContext(Section section) => Section = section ?? throw new ArgumentNullException(nameof(section));

        public SectionContext(SectionContext original) : base(original)
        {
            Section = original.Section;
            NumberCache.CopyFrom(original.NumberCache);
        }

        public IDictionary<int, Treatment> ScheduledTreatments { get; } = new Dictionary<int, Treatment>();

        public Section Section { get; }

        public override double GetNumber(string key)
        {
            if (!NumberCache.TryGetValue(key, out var number))
            {
                number = base.GetNumber(key);
                NumberCache[key] = number;
            }

            return number;
        }

        public override void SetNumber(string key, double value)
        {
            NumberCache.Clear();
            base.SetNumber(key, value);
        }

        public override void SetNumber(string key, Func<double> getValue)
        {
            NumberCache.Clear();
            base.SetNumber(key, getValue);
        }

        public override void SetText(string key, string value)
        {
            NumberCache.Clear();
            base.SetText(key, value);
        }

        private readonly IDictionary<string, double> NumberCache = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
    }
}
