using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public class AverageAggregationRule : NumericAggregationRule
    {
        public override double Apply(IEnumerable<AttributeDatum<double>> attributeData)
        {
            return attributeData.Select(_ => _.Value).Average();
        }
    }
}
