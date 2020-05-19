using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public abstract class TextAggregationRule : AggregationRule<string>
    {
        public abstract override IEnumerable<(int, string)> Apply(IEnumerable<AttributeDatum<string>> attributeData);
    }
}
