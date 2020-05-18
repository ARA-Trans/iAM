using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public abstract class TextAggregationRule : AggregationRule<string>
    {
        public abstract override string Apply(IEnumerable<AttributeDatum<string>> attributeData);
    }
}
