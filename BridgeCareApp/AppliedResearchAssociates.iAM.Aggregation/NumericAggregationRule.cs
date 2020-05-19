using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public abstract class NumericAggregationRule : AggregationRule<double>
    {
        public abstract override IEnumerable<(int, double)> Apply(IEnumerable<AttributeDatum<double>> attributeData);
    }
}
