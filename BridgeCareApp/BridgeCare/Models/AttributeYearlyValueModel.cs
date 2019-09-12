namespace BridgeCare.Models
{
    public class AttributeYearlyValueModel
    {
        public AttributeYearlyValueModel()
        {
        }

        public AttributeYearlyValueModel(int year, string value)
        {
            Year = year;
            Value = value;
        }

        public int Year { get; set; }
        public string Value { get; set; }
    }
}