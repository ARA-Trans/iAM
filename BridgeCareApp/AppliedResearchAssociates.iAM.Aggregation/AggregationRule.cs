using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public abstract class AggregationRule<T>
    {
        public abstract T Apply(IEnumerable<AttributeDatum<T>> attributeData);
    }
}
