using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.Aggregation
{
    public class Criterion
    {
        internal T Apply<T>(IEnumerable<AttributeDatum<T>> attributeData)
        {
            throw new NotImplementedException();
        }
    }
}