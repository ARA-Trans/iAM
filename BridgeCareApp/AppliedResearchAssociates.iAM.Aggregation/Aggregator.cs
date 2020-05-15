using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.Segmentation;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    // Inputs to aggregator are sections to aggregate data to and attribute data
    // For each attribute, match on location data available, assign the
    // attribute data to the section If multiple attribute data are assigned to
    // a section for a given attribute, then use criteria specific rules for
    // aggregating those data to a single attribute datum.
    public static class Aggregator
    {
        public static List<AggregateDataSegment<T>> Aggregate<T>(Attribute attribute, List<Segment<T>> networkSegments)
        {
            var aggregateDataSegment = new List<AggregateDataSegment<T>>();
            var data = attribute.AttributeConnection.GetData<T>();
            foreach(var locationDatumTuple in data)
            {
                var locationMatch = locationDatumTuple.location.MatchLocation(networkSegments.Select(_ => _.SegmentLocation));
                aggregateDataSegment.Add(new AggregateDataSegment<T>(new Segment<T>(locationMatch,)

            }
        }
    }
}
