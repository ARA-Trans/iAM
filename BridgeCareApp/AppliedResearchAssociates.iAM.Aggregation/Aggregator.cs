using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.Segmentation;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public static class Aggregator
    {
        public static List<AggregateDataSegment<T>> Aggregate<T>(
            List<AttributeDatum<T>> attributeData,
            IEnumerable<Segment<T>> networkSegments)
        {
            var aggregateDataSegments = new List<AggregateDataSegment<T>>();

            // Copy the network segments into a new list of AggregateDataSegments
            foreach (var networkSegment in networkSegments)
            {
                aggregateDataSegments.Add(new AggregateDataSegment<T>(networkSegment));
            }

            foreach (var datum in attributeData)
            {
                AggregateDataSegment<T> matchingLocationSegment =
                    aggregateDataSegments.
                    FirstOrDefault(_ => datum.Location.MatchOn(_.Segment.Location));

                if (matchingLocationSegment != null)
                {
                    // Add the datum to the aggregation data segment
                    matchingLocationSegment.AddDatum(datum);
                }
                else
                {
                    // TODO: No matching segment for the current data. What do we do?
                }
            }
            return aggregateDataSegments;
        }
    }
}
