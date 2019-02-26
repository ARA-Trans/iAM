namespace BridgeCare.Models
{
    public class AttributeYearlyValueModel
    {
        public AttributeYearlyValueModel()
        {
        }

        public AttributeYearlyValueModel(int setyear, string setvalue)
        {
            Year = setyear;
            Value = setvalue;
        }

        public int Year { get; set; }
        public string Value { get; set; }
    }
}