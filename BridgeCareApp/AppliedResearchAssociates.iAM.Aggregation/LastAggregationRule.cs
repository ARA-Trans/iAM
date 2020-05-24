using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public class LastAggregationRule<T> : AggregationRule<T>
    {
        public override IEnumerable<(int, T)> Apply(IEnumerable<AttributeDatum<T>> attributeData)
        {
            throw new System.NotImplementedException();
        }
    }
}
