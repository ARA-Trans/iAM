using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public abstract class AggregationRule<T>
    {
        public abstract IEnumerable<(int Year, T Value)> Apply(IEnumerable<AttributeDatum<T>> attributeData);
    }
}
