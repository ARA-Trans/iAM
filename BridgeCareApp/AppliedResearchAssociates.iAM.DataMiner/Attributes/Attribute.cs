using System;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public abstract class Attribute
    {
        public Attribute(string name, AttributeConnection attributeConnection, Asset asset)
        {
            Guid = Guid.NewGuid();
            Name = name;
            AttributeConnection = attributeConnection;
            Asset = asset;
        }

        public Guid Guid { get; }

        public string Name { get; }

        public AttributeConnection AttributeConnection { get; }

        /// <summary>
        /// The asset that this attribute is a member of.
        /// </summary>
        public Asset Asset { get; }
    }
}
