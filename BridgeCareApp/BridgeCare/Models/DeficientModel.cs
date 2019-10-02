using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class DeficientModel: CrudModel
    {
        public string Id { get; set; }
        public string Attribute { get; set; }
        public string Name { get; set; }
        public double? Deficient { get; set; }
        public double? PercentDeficient { get; set; }
        public string Criteria { get; set; }

        public DeficientModel() { }

        public DeficientModel(DeficientsEntity entity)
        {
            Id = entity.ID_.ToString();
            Attribute = entity.ATTRIBUTE_;
            Name = entity.DEFICIENTNAME;
            Deficient = entity.DEFICIENT ?? 0;
            PercentDeficient = entity.PERCENTDEFICIENT ?? 0;
            Criteria = entity.CRITERIA;
        }

        public void UpdateDeficient(DeficientsEntity entity)
        {
            entity.ATTRIBUTE_ = Attribute;
            entity.DEFICIENTNAME = Name;
            entity.DEFICIENT = Deficient ?? 0;
            entity.PERCENTDEFICIENT = PercentDeficient ?? 0;
            entity.CRITERIA = Criteria;
        }
    }
}