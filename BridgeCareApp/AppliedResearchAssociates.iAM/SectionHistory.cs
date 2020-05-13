using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public sealed class SectionHistory
    {
        public SectionHistory(Section section) => Section = section ?? throw new ArgumentNullException(nameof(section));

        public IDictionary<NumberAttribute, IDictionary<int, double>> HistoryPerNumberAttribute { get; } = new Dictionary<NumberAttribute, IDictionary<int, double>>();

        public IDictionary<TextAttribute, IDictionary<int, string>> HistoryPerTextAttribute { get; } = new Dictionary<TextAttribute, IDictionary<int, string>>();

        public Section Section { get; }
    }
}
