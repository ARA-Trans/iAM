using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class Attribute
    {
        public Attribute(string name, ConnectionProperties connectionProperties)
        {
            Guid = Guid.NewGuid();
            Name = name;
            ConnectionProperties = connectionProperties;
        }

        public Guid Guid { get; }
        public string Name { get; }
        public ConnectionProperties ConnectionProperties { get; }

        public abstract void Persist();
    }
}
