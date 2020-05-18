using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.Segmentation;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public class AggregateDataSegment<T>
    {
        private List<AttributeDatum<T>> AttributeData { get; } = new List<AttributeDatum<T>>();
        public AggregateDataSegment(Segment<T> segment) => Segment = segment;
        public Segment<T> Segment { get; }
        public T ComputeAggregateValue(Criterion criterion) => criterion.Apply(AttributeData);
        public void AddDatum(AttributeDatum<T> datum) => AttributeData.Add(datum);
    }
}
