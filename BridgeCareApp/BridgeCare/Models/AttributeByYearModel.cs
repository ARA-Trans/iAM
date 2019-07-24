using System.Collections.Generic;

namespace BridgeCare.Models
{
    public class AttributeByYearModel
    {
        public string Name { get; set; }
        public List<AttributeYearlyValueModel> YearlyValues { get; set; }

        public AttributeByYearModel()
        {
            YearlyValues = new List<AttributeYearlyValueModel>();
        }
    }
}