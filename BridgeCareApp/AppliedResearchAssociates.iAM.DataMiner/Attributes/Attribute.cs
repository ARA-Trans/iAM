using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class Attribute
    {
        public Attribute(string name, AttributeConnection attributeConnection)
        {
            Guid = Guid.NewGuid();
            Name = name;
            AttributeConnection = attributeConnection;
        }

        public Guid Guid { get; }

        public string Name { get; }

        public AttributeConnection AttributeConnection { get; }        
    }
}
