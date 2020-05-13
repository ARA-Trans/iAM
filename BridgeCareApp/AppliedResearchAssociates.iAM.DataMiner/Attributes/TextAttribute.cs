namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class TextAttribute : Attribute
    {
        public TextAttribute(string name, AttributeConnection attributeConnection, string defaultValue) :
            base(name, attributeConnection) => DefaultValue = defaultValue;

        public string DefaultValue { get; }
    }
}
