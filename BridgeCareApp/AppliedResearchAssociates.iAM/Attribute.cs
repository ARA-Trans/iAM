using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM
{
    public class Attribute
    {
        public string Name { get; set; }
    }

    public class Attribute<T> : Attribute
    {
        // [REVIEW] Consider removing this member in favor of separately defining one or more
        // AttributeDataProvider/AttributeDataSource types, which would use this type (as metadata)
        // to retrieve the corresponding data with respect to system constraints, e.g. not enough
        // memory to hold all data at once.
        public IEnumerable<AttributeDatum<T>> Data => throw new NotImplementedException();

        public T DefaultValue { get; set; }
    }
}
