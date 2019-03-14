using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class AttributeByYearModel
    {
        public AttributeByYearModel()
        {
            YearlyValues = new List<AttributeYearlyValueModel>();
        }

        public string Name { get; set; }
        public List<AttributeYearlyValueModel> YearlyValues { get; set; }
    }
}