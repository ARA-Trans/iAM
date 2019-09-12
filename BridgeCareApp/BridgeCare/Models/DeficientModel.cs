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

        public DeficientModel(DeficientsEntity deficient)
        {
            Id = deficient.ID_.ToString();
            Attribute = deficient.ATTRIBUTE_;
            Name = deficient.DEFICIENTNAME;
            Deficient = deficient.DEFICIENT ?? 0;
            PercentDeficient = deficient.PERCENTDEFICIENT ?? 0;
            Criteria = deficient.CRITERIA;
        }

        public void UpdateDeficient(DeficientsEntity deficient)
        {
            deficient.ATTRIBUTE_ = Attribute;
            deficient.DEFICIENTNAME = Name;
            deficient.DEFICIENT = Deficient ?? 0;
            deficient.PERCENTDEFICIENT = PercentDeficient ?? 0;
            deficient.CRITERIA = Criteria;
        }
    }
}