using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    /// <summary>
    /// Aggregates data by always selecting the last provided value in a particular interval
    /// </summary>
    public class LastAggregationRule<T> : AggregationRule<T>
    {
        public override IEnumerable<(int, T)> Apply(IEnumerable<AttributeDatum<T>> attributeData)
        {
            // ASSUMPTION:  All AttributeDatum are from the same Attribute
            // TODO: Force check?
            var timeInterval = attributeData.First().Attribute.Asset.TimeInterval;
            // var interval = new TimeIntervalYear(new DateTime(0, 1, 1));

            var distinctIntervals = attributeData.Select(_ => timeInterval.CalculateInterval(_.TimeStamp)).Distinct();
            foreach (var interval in distinctIntervals)
            {
                var currentIntervalAttributes = attributeData.Where(_ => timeInterval.CalculateInterval(_.TimeStamp) == interval);
                yield return (interval, currentIntervalAttributes
                    .OrderByDescending(_ => _.TimeStamp)
                    .First()
                    .Value);
            }
        }
    }
}
