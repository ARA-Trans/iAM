using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.Segmentation;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    // Inputs to aggregator are sections to aggregate data to and attribute data
    // For each attribute, match on location data available, assign the
    // attribute data to the section If multiple attribute data are assigned to
    // a section for a given attribute, then use criteria specific rules for
    // aggregating those data to a single attribute datum.
    public class Aggregator
    {
        public List<AggregateDataSegment<T>> Aggregate<T, TLocation>(
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
                // Find the AggregateDataSegment that matches the current
                // attribute datum's location
                AggregateDataSegment<T> matchingLocationSegment =
                    aggregateDataSegments.
                    FirstOrDefault(_ => datum.Location.MatchOn(_.Segment.Location));

                if (matchingLocationSegment != null)
                {
                    // Add the datum to the aggregation data segment
                    matchingLocationSegment.AddDatum(datum);
                }
            }
            return aggregateDataSegments;
        }
    }
}
