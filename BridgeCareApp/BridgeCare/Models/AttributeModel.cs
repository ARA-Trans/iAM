using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class AttributeModel
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public AttributeModel() { }

        public AttributeModel(AttributesEntity attributesEntity)
        {
            Name = attributesEntity.ATTRIBUTE_;
            Type = attributesEntity.Type_;
        }
    }
}