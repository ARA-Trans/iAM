using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public abstract class AggregationRule<T>
    {
        public abstract IEnumerable<(DateTime, T)> Apply(IEnumerable<AttributeDatum<T>> attributeData);
    }
}
