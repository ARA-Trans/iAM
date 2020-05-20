using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class AttributeConnection
    {
        public abstract void Connect();

        public abstract IEnumerable<(Location location, T value)> GetData<T>();
    }
}
