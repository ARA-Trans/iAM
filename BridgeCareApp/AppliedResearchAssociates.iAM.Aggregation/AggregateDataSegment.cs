using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.Segmentation;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public class AggregateDataSegment<T>
    {
        public AggregateDataSegment(Segment<T> segment, IEnumerable<AttributeDatum<T>> attributeData)
        {
            Segment = segment;
            AttributeData = attributeData;
        }

        public Segment<T> Segment { get; }
        public IEnumerable<AttributeDatum<T>> AttributeData { get; }

        public T ComputeAggregateValue(Criterion criterion)
        {
            return criterion.Apply(AttributeData);
        }
    }
}
