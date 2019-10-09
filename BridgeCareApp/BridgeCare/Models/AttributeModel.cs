using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class AttributeModel
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public AttributeModel() { }

        public AttributeModel(AttributesEntity entity)
        {
            Name = entity.ATTRIBUTE_;
            Type = entity.Type_;
        }
    }
}