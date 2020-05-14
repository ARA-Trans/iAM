using System;
using System.Collections;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class SectionHistory
    {
        public SectionHistory(Section section) => Section = section ?? throw new ArgumentNullException(nameof(section));

        public Section Section { get; }

        public IDictionary<int, T> GetAttributeHistory<T>(Attribute<T> attribute)
        {
            if (!HistoryPerAttribute.TryGetValue(attribute, out var history))
            {
                history = new Dictionary<int, T>();
                HistoryPerAttribute.Add(attribute, history);
            }

            return (Dictionary<int, T>)history;
        }

        private readonly Dictionary<Attribute, IDictionary> HistoryPerAttribute = new Dictionary<Attribute, IDictionary>();
    }
}
