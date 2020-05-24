using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public class EvalStub
    {
        internal TemporalValue<T> Apply<T>(List<AttributeDatum<T>> attributeData, IEnumerable<AggregationRule<T>> aggregationRules)
        {
            throw new NotImplementedException();
        }
    }
}
