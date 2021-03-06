﻿using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.Segmentation;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public class AggregateDataSegment<T>
    {
        private List<AttributeDatum<T>> AttributeData { get; } = new List<AttributeDatum<T>>();

        public AggregateDataSegment(Segment<T> segment) => Segment = segment;

        public Segment<T> Segment { get; }

        public void AddDatum(AttributeDatum<T> datum) => AttributeData.Add(datum);

        public IEnumerable<(int, T)> ApplyAggregationRule(Attribute attribute, AggregationRule<T> aggregationRule)
        {
            var specifiedData = AttributeData.Where(_ => _.Attribute.Guid == attribute.Guid);
            return aggregationRule.Apply(specifiedData);
        }
    }
}
