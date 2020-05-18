using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public class PredominantAggregationRule : TextAggregationRule
    {
        public override string Apply(IEnumerable<AttributeDatum<string>> attributeData)
        {
            return attributeData
                .GroupBy(_ => _.Value)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .Where(_ => _ != null)
                .First();
        }
    }
}
