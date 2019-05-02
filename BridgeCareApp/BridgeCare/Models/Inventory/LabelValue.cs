namespace BridgeCare.Models
{
    public class LabelValue
    {
        public LabelValue(string label, string value)
        {
            Label = label;
            Value = value;
        }

        public string Label { get; set; }

        public string Value { get; set; }
    }
}