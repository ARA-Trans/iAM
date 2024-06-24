using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class AttributeConnection
    {
        public abstract string DataRetrievalCommand { get; }
        public abstract string ConnectionInformation { get; }
        public abstract IEnumerable<(Location location, T value)> GetData<T>();
    }
}
