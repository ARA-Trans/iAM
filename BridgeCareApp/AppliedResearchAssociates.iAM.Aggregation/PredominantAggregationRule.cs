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
        public override IEnumerable<(DateTime, string)> Apply(IEnumerable<AttributeDatum<string>> attributeData)
        {
            var distinctYears = attributeData.Select(_ => _.TimeStamp.Year).Distinct();
            foreach (var distinctYear in distinctYears)
            {
                yield return (attributeData
                    .First(_ => _.TimeStamp.Year == distinctYear).TimeStamp, attributeData
                    .GroupBy(_ => _.Value)
                    .OrderByDescending(group => group.Count())
                    .Select(group => group.Key)
                    .Where(_ => _ != null)
                    .First());
            }
        }
    }
}
