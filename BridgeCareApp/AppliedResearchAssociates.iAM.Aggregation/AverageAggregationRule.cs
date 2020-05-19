using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public class AverageAggregationRule : NumericAggregationRule
    {
        public override IEnumerable<(int, double)> Apply(IEnumerable<AttributeDatum<double>> attributeData)
        {
            var distinctYears = attributeData.Select(_ => _.TimeStamp.Year).Distinct();
            foreach(var distinctYear in distinctYears)
            {
                var currentYearAttributeData = attributeData.Where(_ => _.TimeStamp.Year == distinctYear);
                yield return (currentYearAttributeData.First().TimeStamp.Year, currentYearAttributeData.Select(_ => _.Value).Average());
            }
        }
    }
}
